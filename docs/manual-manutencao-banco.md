# Honda Sensor Checker — Manual de diagnóstico e correção do banco

Este documento orienta Manutenção e Engenharia na análise de problemas que podem ser diagnosticados ou, em casos controlados, corrigidos no SQLite do Honda Sensor Checker.

O procedimento normal continua sendo usar a própria aplicação. A edição direta é excepcional e exige aplicação fechada, backup, autorização, transação, auditoria e validação.

> **Regra crítica:** o banco registra o estado local, mas o ACC é externo. Nunca altere `AccState`, `IsScrap`, `AccCycleId` ou o resultado de um sensor apenas para liberar a tela. Primeiro confirme no ACC quais comandos foram aceitos.

## Sumário

- [1. Níveis de risco](#1-níveis-de-risco)
- [2. Localização e backup](#2-localização-e-backup)
- [3. Estrutura e estados](#3-estrutura-e-estados)
- [4. Diagnóstico geral](#4-diagnóstico-geral)
- [5. Matriz de problemas](#5-matriz-de-problemas)
- [6. Correções controladas](#6-correções-controladas)
- [7. Casos que exigem reconciliação](#7-casos-que-exigem-reconciliação)
- [8. Auditoria obrigatória](#8-auditoria-obrigatória)
- [9. Validação e rollback](#9-validação-e-rollback)
- [10. Checklist](#10-checklist)

---

# 1. Níveis de risco

| Nível | Exemplos | Autorização mínima |
|---|---|---|
| Consulta | Listar pendências, localizar serial, conferir integridade | Manutenção |
| Correção local | Remover caixa vazia, pausar caixa, limpar SupplierBox corrente | Manutenção responsável, com backup |
| Rastreabilidade | Ajustar produto, Work Order, operador ou saldo | Engenharia/Qualidade |
| Reconciliação ACC | Alterar `Loaded`, resultado ou scrap | Engenharia + ACC/Qualidade |
| Recuperação estrutural | Corrupção, schema ou migration | Engenharia de software/DBA |

Uma correção só é aceitável quando:

1. o arquivo e o registro exatos foram identificados;
2. a aplicação está fechada;
3. existe backup válido;
4. as pré-condições foram comprovadas por `SELECT`;
5. a alteração usa transação e chave primária no `WHERE`;
6. uma auditoria é gravada;
7. as verificações de integridade continuam aprovadas.

---

# 2. Localização e backup

Banco ativo:

```text
C:\ProgramData\HondaSensorChecker\Database\HondaSensorChecker.db
```

Arquivos auxiliares possíveis:

```text
HondaSensorChecker.db-wal
HondaSensorChecker.db-shm
```

Banco legado:

```text
C:\ProgramData\HondaSensorChecker.db
```

Logs técnicos:

```text
C:\ProgramData\HondaSensorChecker\Logs\HondaSensorChecker-AAAA-MM-DD.log
```

## 2.1 Confirmar o arquivo

Registre caminho, máquina, tamanho, última alteração, versão do executável, última migration e uma Work Order conhecida. Não presuma que uma cópia chamada “backup” é o banco atualmente instalado.

```sql
SELECT MigrationId, ProductVersion
FROM __EFMigrationsHistory
ORDER BY MigrationId;
```

Migrations esperadas:

```text
20260203231817_InitialCreate
20260818180000_AddPersistentProcessState
20260825120000_AddSensorAccLifecycleAndScrapAudit
```

Se estiverem incompletas, não crie colunas manualmente. Use a versão correta da aplicação para aplicar migrations.

## 2.2 Fechar todas as conexões

1. Feche o Honda Sensor Checker.
2. Confirme no Gerenciador de Tarefas que o processo terminou.
3. Feche outros editores SQLite.
4. Não reabra a aplicação antes da validação final.

## 2.3 Criar backup consistente

Nome recomendado:

```text
HondaSensorChecker_before_<motivo>_AAAAMMDD_HHMMSS.db
```

Se houver `-wal`, ele pode conter transações confirmadas ainda não incorporadas ao `.db`. Use a API/comando de backup SQLite ou preserve `.db`, `-wal` e `-shm` juntos, com todas as conexões fechadas.

Exemplo no SQLite CLI:

```sql
.open "C:/ProgramData/HondaSensorChecker/Database/HondaSensorChecker.db"
.backup "C:/Backup/HondaSensorChecker_before_correction.db"
```

## 2.4 Integridade antes da alteração

```sql
PRAGMA foreign_keys = ON;
PRAGMA foreign_key_check;
PRAGMA integrity_check;
```

Esperado: nenhuma linha no `foreign_key_check` e `ok` no `integrity_check`. Se o banco já estiver corrompido, pare e preserve todos os arquivos.

## 2.5 Modelo de transação

```sql
PRAGMA foreign_keys = ON;
BEGIN IMMEDIATE;

-- Repetir o SELECT das pré-condições.
-- Executar uma única correção autorizada.
-- Conferir SELECT changes(); e consultar novamente.

ROLLBACK; -- durante a validação
-- COMMIT; -- somente após conferir tudo
```

Nunca use `UPDATE` ou `DELETE` sem `WHERE` contendo a chave primária identificada.

---

# 3. Estrutura e estados

## 3.1 Tabelas

| Tabela | Conteúdo |
|---|---|
| `Operators` | RE, ZF-ID, nome e permissão |
| `Products` | Prefixo, PartNumber ZF e ELSEN/ELMOD |
| `SapWorkOrders` | Ordem normalizada e produto |
| `SupplierBoxes` | Número único, quantidade, saldo e produto |
| `ZfBoxes` | Meta, HU, lote e estado da caixa |
| `Sensors` | Serial, rastreabilidade e ciclo ACC |
| `Logs` | Auditoria operacional/administrativa |

## 3.2 Estado da caixa

| `InProgress` | `IsPaused` | Interpretação |
|---:|---:|---|
| 1 | 0 | Ativa; retomada automática |
| 1 | 1 | Aguardando; retomada manual |
| 0 | 0 | Finalizada; deve possuir HU e lote |
| 0 | 1 | Inconsistente; investigar |

`CurrentSupplierBoxId` não possui foreign key declarada. Pode apontar para uma caixa inexistente ou de outro produto sem erro do SQLite.

## 3.3 Estado ACC do sensor

| Valor | Nome | Significado |
|---:|---|---|
| 0 | `NotLoaded` | Reservado; não esperado no fluxo normal |
| 1 | `Loaded` | `Load` aceito, aguardando `Unload` |
| 2 | `UnloadedOk` | `Unload OK` concluído |
| 3 | `UnloadedNok` | `Unload NOK` concluído |

Consistência esperada:

```text
IsScrap=0  → AccState normalmente 1 ou 2
IsScrap=1  → AccState=3
AccState=1 → AccUnloadTime NULL
AccState=2/3 → AccUnloadTime normalmente preenchido
IsScrap=1  → ScrappedTime, ScrapOperatorId e ScrapOperatorName preenchidos
```

Sensores anteriores à migration receberam `AccState=2` por padrão e podem não ter todos os IDs ACC.

## 3.4 Formatação armazenada

| Etiqueta | Valor no banco |
|---|---|
| Work Order `O11...`, `O12...`, `OD...` | sem o primeiro `O` |
| SupplierBox `S` + 10 caracteres | sem `S` |
| PartNumber ZF `P` + 8 caracteres | sem `P` |
| PartNumber final `P` + ELSEN/ELMOD | sem `P` |
| HU `1J` + 10 caracteres | sem `1J` |
| Lote `H` + 10 caracteres | sem `H` |

## 3.5 O contador não é uma coluna

O contador é recalculado pelos sensores não scrapados:

```sql
SELECT COUNT(*) AS GoodSensorCount
FROM Sensors
WHERE ZfBoxId = <ZF_BOX_ID>
  AND IsScrap = 0;
```

Não existe um campo de contador para editar. Deve-se corrigir a causa da divergência.

---

# 4. Diagnóstico geral

As consultas desta seção não alteram dados.

## 4.1 Caixas pendentes

```sql
SELECT
    z.ZfBoxId, z.InProgress, z.IsPaused, z.QtyToSend,
    z.UniqueNumber, z.Batch, z.CurrentSupplierBoxId,
    w.WorkOrderNumber, p.Prefix, p.StartPartNumber, p.EndPartNumber,
    o.Re, o.Name,
    COUNT(s.SensorId) AS TotalSensors,
    SUM(CASE WHEN s.IsScrap=0 THEN 1 ELSE 0 END) AS GoodSensors,
    SUM(CASE WHEN s.IsScrap=1 THEN 1 ELSE 0 END) AS ScrapSensors,
    MIN(s.ScannedTime) AS FirstScan,
    MAX(s.ScannedTime) AS LastScan
FROM ZfBoxes z
JOIN SapWorkOrders w ON w.SapWorkOrderId=z.SapWorkOrderId
JOIN Products p ON p.ProductId=z.ProductId
JOIN Operators o ON o.OperatorId=z.OperatorId
LEFT JOIN Sensors s ON s.ZfBoxId=z.ZfBoxId
WHERE z.InProgress=1
GROUP BY z.ZfBoxId
ORDER BY z.ZfBoxId;
```

`ZfBoxes` não possui data de criação. Para caixa sem sensores, um ID menor é apenas indício de antiguidade.

## 4.2 Sensores da caixa

```sql
SELECT
    SensorId, SerialNumber, ScannedTime, InProgress,
    AccState, AccPartTypeId, AccCycleId, AccUnitPartTypeId,
    AccUnloadTime, IsScrap, ScrappedTime,
    ScrapOperatorId, ScrapOperatorName, SupplierBoxId
FROM Sensors
WHERE ZfBoxId=<ZF_BOX_ID>
ORDER BY SensorId;
```

## 4.3 Caixas que retomam automaticamente

```sql
SELECT ZfBoxId, SapWorkOrderId, QtyToSend, CurrentSupplierBoxId
FROM ZfBoxes
WHERE InProgress=1 AND IsPaused=0
ORDER BY ZfBoxId DESC;
```

Se houver mais de uma, a aplicação retoma a mais recente e mantém as demais pendentes.

## 4.4 `Loads` pendentes

```sql
SELECT SensorId, SerialNumber, ZfBoxId, AccPartTypeId,
       AccCycleId, AccUnitPartTypeId, ScannedTime
FROM Sensors
WHERE AccState=1 AND IsScrap=0
ORDER BY ZfBoxId, ScannedTime;
```

O esperado é no máximo um `Loaded` por caixa ativa.

## 4.5 SupplierBox corrente inválida

```sql
SELECT z.ZfBoxId, z.ProductId AS BoxProductId,
       z.CurrentSupplierBoxId, sb.ProductId AS SupplierProductId,
       sb.UniqueNumber
FROM ZfBoxes z
LEFT JOIN SupplierBoxes sb ON sb.SupplierBoxId=z.CurrentSupplierBoxId
WHERE z.InProgress=1
  AND z.CurrentSupplierBoxId IS NOT NULL
  AND (sb.SupplierBoxId IS NULL OR sb.ProductId<>z.ProductId);
```

## 4.6 Finalização inconsistente

```sql
SELECT ZfBoxId, InProgress, IsPaused, UniqueNumber, Batch
FROM ZfBoxes
WHERE (InProgress=0 AND (UniqueNumber IS NULL OR Batch IS NULL OR IsPaused=1))
   OR (InProgress=1 AND (UniqueNumber IS NOT NULL OR Batch IS NOT NULL));
```

```sql
SELECT s.SensorId, s.SerialNumber, s.InProgress,
       z.InProgress AS BoxInProgress
FROM Sensors s
JOIN ZfBoxes z ON z.ZfBoxId=s.ZfBoxId
WHERE s.InProgress<>z.InProgress;
```

## 4.7 Duplicidades sem proteção física no schema

```sql
SELECT SerialNumber, COUNT(*) FROM Sensors
GROUP BY SerialNumber HAVING COUNT(*)>1;

SELECT WorkOrderNumber, COUNT(*) FROM SapWorkOrders
GROUP BY WorkOrderNumber HAVING COUNT(*)>1;

SELECT UniqueNumber, COUNT(*) FROM SupplierBoxes
GROUP BY UniqueNumber HAVING COUNT(*)>1;

SELECT Re, COUNT(*) FROM Operators
GROUP BY Re HAVING COUNT(*)>1;

SELECT ZfId, COUNT(*) FROM Operators
GROUP BY ZfId HAVING COUNT(*)>1;

SELECT StartPartNumber, COUNT(*) FROM Products
GROUP BY StartPartNumber HAVING COUNT(*)>1;

SELECT EndPartNumber, COUNT(*) FROM Products
GROUP BY EndPartNumber HAVING COUNT(*)>1;

SELECT Prefix, COUNT(*) FROM Products
GROUP BY Prefix HAVING COUNT(*)>1;
```

## 4.8 Scrap inconsistente

```sql
SELECT SensorId, SerialNumber, AccState, AccUnloadTime,
       IsScrap, ScrappedTime, ScrapOperatorId, ScrapOperatorName
FROM Sensors
WHERE (IsScrap=1 AND AccState<>3)
   OR (IsScrap=0 AND AccState=3)
   OR (IsScrap=1 AND
       (ScrappedTime IS NULL OR ScrapOperatorId IS NULL
        OR ScrapOperatorName IS NULL));
```

## 4.9 Saldo matematicamente impossível

```sql
SELECT *
FROM SupplierBoxes
WHERE QtySupplied<0
   OR QtyRemaining<0
   OR QtyRemaining>QtySupplied;
```

Um saldo dentro do intervalo ainda pode divergir da quantidade física.

## 4.10 Chaves órfãs

```sql
PRAGMA foreign_key_check;
```

Use também a seção 4.5, pois `CurrentSupplierBoxId` não é uma foreign key.

## 4.11 Violações de regras operacionais

Quantidade de caixa fora das opções da interface:

```sql
SELECT ZfBoxId, QtyToSend, InProgress
FROM ZfBoxes
WHERE QtyToSend NOT IN (3, 10, 60, 420)
   OR QtyToSend <= 0;
```

Caixa acima da meta ou finalizada com quantidade boa diferente:

```sql
SELECT z.ZfBoxId, z.QtyToSend, z.InProgress,
       SUM(CASE WHEN s.IsScrap=0 THEN 1 ELSE 0 END) AS GoodSensors
FROM ZfBoxes z
LEFT JOIN Sensors s ON s.ZfBoxId=z.ZfBoxId
GROUP BY z.ZfBoxId
HAVING GoodSensors>z.QtyToSend
    OR (z.InProgress=0 AND GoodSensors<>z.QtyToSend);
```

HU final repetida:

```sql
SELECT UniqueNumber, COUNT(*) AS Quantity,
       group_concat(ZfBoxId) AS ZfBoxIds
FROM ZfBoxes
WHERE UniqueNumber IS NOT NULL
GROUP BY UniqueNumber
HAVING COUNT(*)>1;
```

Ausência de administrador:

```sql
SELECT COUNT(*) AS ActiveAdministrators
FROM Operators
WHERE Admin=1;
```

Work Orders e SupplierBoxes sem uso não são necessariamente erros, mas merecem revisão se resultarem de testes ou cadastros cancelados:

```sql
SELECT w.SapWorkOrderId, w.WorkOrderNumber, w.ProductId
FROM SapWorkOrders w
WHERE NOT EXISTS (SELECT 1 FROM ZfBoxes z WHERE z.SapWorkOrderId=w.SapWorkOrderId)
  AND NOT EXISTS (SELECT 1 FROM Sensors s WHERE s.SapWorkOrderId=w.SapWorkOrderId);

SELECT sb.SupplierBoxId, sb.UniqueNumber, sb.ProductId,
       sb.QtySupplied, sb.QtyRemaining
FROM SupplierBoxes sb
WHERE NOT EXISTS (SELECT 1 FROM Sensors s WHERE s.SupplierBoxId=sb.SupplierBoxId)
  AND NOT EXISTS (
      SELECT 1 FROM ZfBoxes z WHERE z.CurrentSupplierBoxId=sb.SupplierBoxId);
```

---

# 5. Matriz de problemas

| Sintoma | Causa provável | Banco basta? | Seção |
|---|---|---:|---|
| Caixa antiga abre na inicialização | Ativa e não pausada | Sim se vazia; caso contrário, pausar | 6.1/6.2 |
| Duas caixas ativas | Fechamento/interrupção incompleta | Parcialmente | 6.1/6.2 |
| Caixa deve voltar à retomada automática | `IsPaused=1` | Sim, mas prefira a tela | 6.3 |
| Retomada usa SupplierBox errada | Referência inválida | Sim | 6.4 |
| Quantidade `Q...` divergente | Saldo local incorreto | Após contagem física | 6.5 |
| Usuário não reconhecido | ZF-ID/RE incorreto | Sim, prefira a tela | 6.6 |
| Nenhum administrador disponível | `Admin=0` para todos | Com autorização | 6.7 |
| Prefixo escolhe produto errado | Prefixo duplicado | Só sem dependências | 6.8 |
| WO aponta ao produto errado | Cadastro incorreto | Só sem produção | 6.9 |
| Caixa finalizada está pendente | Finalização incompleta | Exige análise ampla | 6.10 |
| Sensor finalizado continua ativo | Estado local incompleto | Se ACC/caixa resolvidos | 6.11 |
| Mais de um `Loaded` | Divergência ACC/banco | Não | 7 |
| Scrap retroativo | Processo externo/físico | Não | 7 |
| Serial duplicado | Importação/edição indevida | Não apagar sem análise | 7 |
| Banco `malformed` | Corrupção/cópia incompleta | Não por `UPDATE` | 9 |
| `database is locked` | Processo/editor aberto | Não requer edição | 9 |
| Coluna ausente | Migration não aplicada | Não criar manualmente | 2.1 |
| Meta fora de 3/10/60/420 | Edição externa ou dado legado | Só com caixa vazia | 4.11 |
| HU duplicada | Etiqueta repetida ou edição externa | Não, sem análise logística | 7 |
| WO/SupplierBox de teste sem uso | Cadastro cancelado | Sim, se todas as dependências forem zero | 6.12 |

---

# 6. Correções controladas

Substitua `<...>` somente após executar os diagnósticos. Não rode os exemplos como um lote cego.

## 6.1 Remover caixa pendente totalmente vazia

Permitido somente se `InProgress=1`, HU/lote/SupplierBox corrente são nulos e não existem sensores.

```sql
SELECT z.*, COUNT(s.SensorId) AS SensorCount
FROM ZfBoxes z
LEFT JOIN Sensors s ON s.ZfBoxId=z.ZfBoxId
WHERE z.ZfBoxId=<ZF_BOX_ID>
GROUP BY z.ZfBoxId;
```

```sql
PRAGMA foreign_keys=ON;
BEGIN IMMEDIATE;

DELETE FROM ZfBoxes
WHERE ZfBoxId=<ZF_BOX_ID>
  AND InProgress=1
  AND UniqueNumber IS NULL
  AND Batch IS NULL
  AND CurrentSupplierBoxId IS NULL
  AND NOT EXISTS (
      SELECT 1 FROM Sensors WHERE Sensors.ZfBoxId=ZfBoxes.ZfBoxId
  );

SELECT changes() AS DeletedRows; -- exatamente 1
-- Inserir auditoria da seção 8.
COMMIT;
```

Não use `InProgress=0`: a caixa vazia apareceria como finalizada sem HU/lote.

## 6.2 Pausar caixa real sem encerrá-la

```sql
BEGIN IMMEDIATE;
UPDATE ZfBoxes
SET IsPaused=1
WHERE ZfBoxId=<ZF_BOX_ID>
  AND InProgress=1
  AND IsPaused=0;
SELECT changes() AS UpdatedRows; -- esperado: 1
-- Auditoria.
COMMIT;
```

Isso impede a retomada automática, mas não executa `Unload` nem resolve um sensor `Loaded`.

## 6.3 Retirar a pausa

Prefira **CONTINUAR PROCESSO**, que refaz `PartTypeData`. A edição direta apenas torna a caixa candidata à retomada na abertura seguinte.

```sql
BEGIN IMMEDIATE;
UPDATE ZfBoxes
SET IsPaused=0
WHERE ZfBoxId=<ZF_BOX_ID>
  AND InProgress=1
  AND IsPaused=1;
SELECT changes() AS UpdatedRows;
-- Auditoria.
COMMIT;
```

Confirme antes que não existe outra caixa ativa não pausada.

## 6.4 Limpar SupplierBox corrente inválida

Definir `NULL` é mais seguro que adivinhar outra SupplierBox. O operador deverá reler a etiqueta.

```sql
BEGIN IMMEDIATE;
UPDATE ZfBoxes
SET CurrentSupplierBoxId=NULL
WHERE ZfBoxId=<ZF_BOX_ID>
  AND InProgress=1;
SELECT changes() AS UpdatedRows;
-- Auditoria.
COMMIT;
```

Para apontar outra caixa, comprove que o produto coincide:

```sql
SELECT z.ZfBoxId, z.ProductId, sb.SupplierBoxId,
       sb.ProductId, sb.UniqueNumber
FROM ZfBoxes z
JOIN SupplierBoxes sb ON sb.SupplierBoxId=<SUPPLIER_BOX_ID>
WHERE z.ZfBoxId=<ZF_BOX_ID>
  AND z.ProductId=sb.ProductId;
```

## 6.5 Corrigir saldo da SupplierBox

Faça contagem física e confira caixas finalizadas e em andamento.

```sql
SELECT SupplierBoxId, UniqueNumber, QtySupplied, QtyRemaining, ProductId
FROM SupplierBoxes
WHERE SupplierBoxId=<SUPPLIER_BOX_ID>;

SELECT COUNT(*) AS TotalLinked,
       SUM(CASE WHEN InProgress=1 THEN 1 ELSE 0 END) AS ActiveLinked,
       SUM(CASE WHEN IsScrap=1 THEN 1 ELSE 0 END) AS ScrapLinked
FROM Sensors
WHERE SupplierBoxId=<SUPPLIER_BOX_ID>;
```

```sql
BEGIN IMMEDIATE;
UPDATE SupplierBoxes
SET QtyRemaining=<NOVO_SALDO_CONFIRMADO>
WHERE SupplierBoxId=<SUPPLIER_BOX_ID>
  AND QtyRemaining=<SALDO_ANTERIOR>;
SELECT changes() AS UpdatedRows;
-- Auditoria com valores anterior e novo.
COMMIT;
```

Não aumente `QtySupplied` para esconder consumo e não devolva scrap ao saldo.

## 6.6 Corrigir RE, ZF-ID ou nome

Prefira **+ USUÁRIO**. Confirme antes que o novo valor não pertence a outro operador.

```sql
SELECT OperatorId, Re, ZfId, Name, Admin
FROM Operators
WHERE upper(Re)=upper('<NOVO_RE>')
   OR upper(ZfId)=upper('<NOVO_ZFID>');
```

```sql
BEGIN IMMEDIATE;
UPDATE Operators
SET Re=upper('<NOVO_RE>'),
    ZfId=upper('<NOVO_ZFID>'),
    Name=upper('<NOVO_NOME>')
WHERE OperatorId=<OPERATOR_ID>;
SELECT changes() AS UpdatedRows;
-- Auditoria atribuída a administrador autorizado.
COMMIT;
```

Não altere `OperatorId`; ele preserva os vínculos históricos.

## 6.7 Recuperar acesso administrativo

Somente com autorização formal e se não existir administrador utilizável.

```sql
SELECT OperatorId, Re, ZfId, Name, Admin
FROM Operators ORDER BY OperatorId;
```

```sql
BEGIN IMMEDIATE;
UPDATE Operators
SET Admin=1
WHERE OperatorId=<OPERADOR_AUTORIZADO>;
SELECT changes() AS UpdatedRows;
-- Auditoria com chamado/autorização.
COMMIT;
```

Nunca crie login genérico compartilhado.

## 6.8 Produto sem dependências

Use **+ PRODUTO**. Uma alteração direta só é simples se não houver referências:

```sql
SELECT
 (SELECT COUNT(*) FROM Sensors WHERE ProductId=p.ProductId) AS Sensors,
 (SELECT COUNT(*) FROM SupplierBoxes WHERE ProductId=p.ProductId) AS SupplierBoxes,
 (SELECT COUNT(*) FROM ZfBoxes WHERE ProductId=p.ProductId) AS ZfBoxes,
 (SELECT COUNT(*) FROM SapWorkOrders WHERE ProductId=p.ProductId) AS WorkOrders
FROM Products p
WHERE p.ProductId=<PRODUCT_ID>;
```

Todos devem ser zero para remoção simples. Para editar, confira duplicidades primeiro:

```sql
BEGIN IMMEDIATE;
UPDATE Products
SET Prefix=upper('<PREFIXO_4>'),
    StartPartNumber=upper('<ZF_PN_SEM_P>'),
    EndPartNumber=upper('<ELSEN_ELMOD_SEM_P>')
WHERE ProductId=<PRODUCT_ID>;
SELECT changes() AS UpdatedRows;
-- Auditoria.
COMMIT;
```

Com dependências, a alteração muda a interpretação histórica; escale.

## 6.9 Produto de Work Order ainda não utilizada

```sql
SELECT w.SapWorkOrderId, w.WorkOrderNumber, w.ProductId,
 (SELECT COUNT(*) FROM ZfBoxes z
  WHERE z.SapWorkOrderId=w.SapWorkOrderId) AS Boxes,
 (SELECT COUNT(*) FROM Sensors s
  WHERE s.SapWorkOrderId=w.SapWorkOrderId) AS Sensors
FROM SapWorkOrders w
WHERE w.SapWorkOrderId=<WORK_ORDER_ID>;
```

```sql
BEGIN IMMEDIATE;
UPDATE SapWorkOrders
SET ProductId=<PRODUCT_ID_CORRETO>
WHERE SapWorkOrderId=<WORK_ORDER_ID>
  AND NOT EXISTS (
      SELECT 1 FROM ZfBoxes WHERE SapWorkOrderId=<WORK_ORDER_ID>)
  AND NOT EXISTS (
      SELECT 1 FROM Sensors WHERE SapWorkOrderId=<WORK_ORDER_ID>);
SELECT changes() AS UpdatedRows;
-- Auditoria.
COMMIT;
```

Não altere o número da ordem para fazê-lo coincidir com outro `PartTypeID`.

## 6.10 Finalização local incompleta

Comprove HU, lote, quantidade boa, estados ACC e débito de cada SupplierBox:

```sql
SELECT z.ZfBoxId, z.QtyToSend, z.UniqueNumber, z.Batch,
 SUM(CASE WHEN s.IsScrap=0 THEN 1 ELSE 0 END) AS GoodSensors,
 SUM(CASE WHEN s.IsScrap=1 THEN 1 ELSE 0 END) AS ScrapSensors,
 SUM(CASE
   WHEN (s.IsScrap=0 AND s.AccState<>2)
     OR (s.IsScrap=1 AND s.AccState<>3)
   THEN 1 ELSE 0 END) AS UnresolvedAcc
FROM ZfBoxes z
LEFT JOIN Sensors s ON s.ZfBoxId=z.ZfBoxId
WHERE z.ZfBoxId=<ZF_BOX_ID>
GROUP BY z.ZfBoxId;
```

Não há `UPDATE` genérico seguro: a finalização normal altera caixa, sensores, saldos de várias SupplierBoxes e log no mesmo commit. Engenharia deve montar uma transação específica após reconciliar ACC e material físico.

## 6.11 Sensores ativos de caixa já finalizada

Somente se HU/lote, estoque e ACC estiverem resolvidos:

```sql
BEGIN IMMEDIATE;
UPDATE Sensors
SET InProgress=0
WHERE ZfBoxId=<ZF_BOX_ID>
  AND InProgress=1
  AND EXISTS (
      SELECT 1 FROM ZfBoxes z
      WHERE z.ZfBoxId=Sensors.ZfBoxId
        AND z.InProgress=0
        AND z.UniqueNumber IS NOT NULL
        AND z.Batch IS NOT NULL)
  AND ((IsScrap=0 AND AccState=2)
    OR (IsScrap=1 AND AccState=3));
SELECT changes() AS UpdatedRows;
-- Comparar com a quantidade prevista e auditar.
COMMIT;
```

## 6.12 Remover Work Order ou SupplierBox sem uso

Esses registros não afetam a retomada e podem ser mantidos. Remova somente se forem cadastros comprovadamente indevidos e não tiverem qualquer dependência.

Work Order:

```sql
BEGIN IMMEDIATE;
DELETE FROM SapWorkOrders
WHERE SapWorkOrderId=<WORK_ORDER_ID>
  AND NOT EXISTS (
      SELECT 1 FROM ZfBoxes WHERE SapWorkOrderId=<WORK_ORDER_ID>)
  AND NOT EXISTS (
      SELECT 1 FROM Sensors WHERE SapWorkOrderId=<WORK_ORDER_ID>);
SELECT changes() AS DeletedRows;
-- Auditoria.
COMMIT;
```

SupplierBox:

```sql
BEGIN IMMEDIATE;
DELETE FROM SupplierBoxes
WHERE SupplierBoxId=<SUPPLIER_BOX_ID>
  AND NOT EXISTS (
      SELECT 1 FROM Sensors WHERE SupplierBoxId=<SUPPLIER_BOX_ID>)
  AND NOT EXISTS (
      SELECT 1 FROM ZfBoxes
      WHERE CurrentSupplierBoxId=<SUPPLIER_BOX_ID>);
SELECT changes() AS DeletedRows;
-- Auditoria.
COMMIT;
```

Como a auditoria tem foreign key apenas para `Operators`, o log continua válido depois dessas remoções quando os IDs são descritos no texto.

---

# 7. Casos que exigem reconciliação

## 7.1 Alterar `Loaded` para `UnloadedOk`

Mudar 1 para 2 não envia `Unload`. Localize serial, `PartTypeID`, `CycleID` e horário; consulte o ACC; decida se falta executar o comando ou somente persistir um comando já aceito; documente a evidência antes de reconciliar.

## 7.2 Criar scrap retroativo

Scrap envolve `Unload NOK`, consumo físico e operador. Atualizar apenas `IsScrap=1` é incorreto.

## 7.3 Apagar serial duplicado

Compare caixas, ordens, horários, SupplierBoxes, operadores, estados/IDs ACC e logs. Preserve o registro que representa o evento real e documente qualquer consolidação.

## 7.4 Trocar produto de caixa com sensores

Alterar só `ZfBoxes.ProductId` cria divergência com sensores, Work Order e SupplierBoxes. A correção precisa tratar todo o grafo e o histórico.

## 7.5 Inventar HU ou lote

HU e lote vêm da etiqueta final. Nunca preencha valores fictícios apenas para retirar uma caixa das pendências.

## 7.6 Apagar logs, migrations ou sequências

- Não apague logs para esconder ocorrências.
- Não edite `__EFMigrationsHistory` para simular atualização.
- Não renumere IDs em `sqlite_sequence`.

---

# 8. Auditoria obrigatória

Toda manutenção deve possuir backup, chamado, consultas antes/depois, comando executado, responsável e validação.

Localize o operador responsável:

```sql
SELECT OperatorId, Re, ZfId, Name, Admin
FROM Operators
WHERE upper(ZfId)=upper('<ZF_ID_RESPONSAVEL>');
```

Modelo de log:

```sql
INSERT INTO Logs (Data, Description, OperatorId)
VALUES (
 datetime('now','localtime'),
 'Manual database maintenance. Ticket=<CHAMADO>, Action=<ACAO>, Target=<TABELA/ID>, Before=<ANTERIOR>, After=<NOVO>, Reason=<MOTIVO>.',
 <OPERATOR_ID_RESPONSAVEL>
);
```

Não atribua a manutenção a quem não participou. Se o técnico não estiver cadastrado, coloque nome, RE e chamado na descrição e use o administrador autorizador.

---

# 9. Validação e rollback

## 9.1 Após a correção

```sql
PRAGMA foreign_key_check;
PRAGMA integrity_check;
```

Depois:

1. repita o diagnóstico original;
2. confirme `changes()` e a auditoria;
3. feche o editor SQLite;
4. abra a aplicação;
5. valide usuário, pendências, Work Order, produto, contador e SupplierBox;
6. preserve o log técnico da primeira abertura;
7. não faça scan até confirmar o contexto.

## 9.2 Rollback aberto

```sql
ROLLBACK;
```

## 9.3 Alteração já confirmada

Feche tudo, preserve o banco alterado, restaure o backup e seus arquivos auxiliares, execute as verificações e registre o rollback. Não restaure backup antigo sobre produção nova sem avaliar todos os sensores posteriores.

## 9.4 Corrupção

Se `integrity_check` não retornar `ok`, pare a produção, preserve `.db`, `-wal`, `-shm` e logs, compare com o último backup e escale. Não tente consertar corrupção com updates isolados.

## 9.5 `database is locked`

Feche aplicação e editores. Não apague `-wal` ou `-shm` para remover o lock.

---

# 10. Checklist

- [ ] Aplicação e editores fechados.
- [ ] Caminho, máquina e versão confirmados.
- [ ] Backup consistente criado.
- [ ] `-wal`/`-shm` avaliados.
- [ ] Migration atual confirmada.
- [ ] Integridade aprovada antes.
- [ ] Sintoma e IDs registrados.
- [ ] ACC consultado se houver sensor/resultado.
- [ ] Pré-condições comprovadas.
- [ ] `BEGIN IMMEDIATE` utilizado.
- [ ] Chave primária presente no `WHERE`.
- [ ] `changes()` igual ao esperado.
- [ ] Auditoria inserida.
- [ ] `COMMIT` somente após conferência.
- [ ] Integridade aprovada depois.
- [ ] Editor fechado antes da aplicação.
- [ ] Retomada e contexto validados.
- [ ] Evidências anexadas ao chamado.

## Resumo de decisão

```text
Problema somente local, vazio ou referência recuperável?
  ├─ Sim → backup → pré-condições → transação → auditoria → validação
  └─ Não
      ↓
Envolve sensor, scrap, Load, Unload ou resultado ACC?
  ├─ Sim → parar produção → reconciliar ACC + banco
  └─ Não
      ↓
Altera produto, Work Order, estoque ou histórico?
  ├─ Sim → Engenharia/Qualidade + evidência física
  └─ Não → procedimento local deste manual
```
