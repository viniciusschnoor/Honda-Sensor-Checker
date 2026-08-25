# Honda Sensor Checker

Aplicação Windows para controlar a montagem e a expedição de caixas de sensores Honda. O sistema relaciona cada sensor à Work Order SAP, ao produto, à SupplierBox de origem, à caixa ZF de destino e ao operador responsável. A integração ACC usa `PartTypeList` e `PartTypeData` para selecionar e configurar o Part Number da Work Order e `Load`/`Unload` para cada sensor.

Este documento serve como manual para Operação, Manutenção e Engenharia e descreve o comportamento implementado no código atual.

## Sumário

- [Visão geral](#visão-geral)
- [Manual do Operador](#manual-do-operador)
- [Manual da Manutenção](#manual-da-manutenção)
- [Manual da Engenharia](#manual-da-engenharia)
- [Formatos de leitura](#formatos-de-leitura)
- [Mensagens e diagnóstico](#mensagens-e-diagnóstico)
- [Arquitetura e dados](#arquitetura-e-dados)
- [Configuração e implantação](#configuração-e-implantação)
- [Pontos técnicos a verificar](#pontos-técnicos-a-verificar)

## Visão geral

O fluxo principal é:

1. Identificar automaticamente o operador pelo usuário do Windows.
2. Ler a Work Order e localizar no `PartTypeList` a descrição que contém seu número sem o prefixo `O`.
3. Executar `PartTypeData` com o `PartTypeID` encontrado para configurar o ACC.
4. Validar o part number final e selecionar a quantidade da caixa ZF.
5. Ler a etiqueta da SupplierBox.
6. Travar o produto para impedir mistura de part numbers na mesma caixa ZF.
7. Ler cada sensor, validar duplicidade e produto, e executar o `Load` no ACC.
8. Registrar cada sensor aprovado no banco local.
9. Permitir troca de SupplierBox, desde que seja do mesmo produto.
10. Ao atingir a quantidade definida, validar a etiqueta final e finalizar a caixa.

### Sinalização visual

| Cor | Significado |
|---|---|
| Amarelo | Aguardando leitura ou processando uma etapa |
| Verde | Sensor aprovado e processado no ACC |
| Vermelho | Leitura, regra de processo ou comunicação ACC rejeitada |

O contador no canto superior mostra `lidos/meta`, por exemplo `010/060`.

### Perfis de acesso

- Operador comum: executa o processo, consulta componentes, retoma caixas, troca SupplierBox e marca o último sensor como scrap.
- Administrador: possui também acesso a cadastro de usuários, cadastro de produtos e visualização dos logs.
- Usuário não cadastrado: a aplicação mostra `USUÁRIO NÃO REGISTRADO` e não libera o processo.

O usuário é localizado pelo ZF-ID cadastrado, comparado ao usuário atual do Windows sem domínio ou sufixo de e-mail.

---

# Manual do Operador

## 1. Início da aplicação

1. Entre no Windows com seu usuário pessoal.
2. Abra o Sensor Checker.
3. Confirme que a tela mostra `LEIA A WORK-ORDER`.

Se aparecer `USUÁRIO NÃO REGISTRADO`, não prossiga. Solicite a um administrador o cadastro do seu ZF-ID.

## 2. Ler a Work Order

Leia a etiqueta no campo **Nº da ordem**.

Os formatos aceitos estão descritos na seção [Formatos válidos de Work Order](#formatos-válidos-de-work-order): Dummy `OD...`, produção normal `O11...` e rework `O12...`.

Ao sair do campo, inclusive após pressionar `Enter`, a aplicação:

1. Remove o `O` inicial.
2. Executa `PartTypeList` na estação configurada.
3. Procura em `PartDesc`/Description um item que contenha o número da Work Order.
4. Exige exatamente uma correspondência para evitar a seleção ambígua de um `PartTypeID`.
5. Executa `PartTypeData` com o `PartTypeID` encontrado.
6. Guarda o `PartTypeID` e a descrição selecionada para os ciclos dos sensores.

Se nenhuma ordem coincidir, se mais de uma descrição coincidir ou se o `PartTypeData` falhar, os próximos campos permanecem bloqueados e a Work Order precisa ser corrigida ou relida.

### Work Order já conhecida

Se ela já existir no banco:

- o produto relacionado será recuperado;
- o campo **PartNumber Final** será preenchido automaticamente;
- a aplicação seguirá para a seleção da quantidade.

### Work Order nova

Se ela ainda não existir:

1. Leia o **PartNumber Final**.
2. O produto precisa estar cadastrado na tela de produtos.
3. A Work Order será criada e associada ao produto.

Formato do PartNumber Final:

```text
P + ELSEN/ELMOD cadastrado
Exemplos: PELMOD00660 ou PELSEN00100
```

Use o botão `✕` da seção SAP para cancelar e reiniciar o processo, se necessário.

## 3. Selecionar a quantidade da caixa ZF

Selecione uma das quantidades disponíveis:

- 3
- 10
- 60
- 420

Confira o produto e a quantidade e clique no botão verde `✓` da seção SAP.

Após a confirmação, a seção **ZF - Logistic Label** será liberada.

## 4. Ler a SupplierBox

### Número único

Leia o campo **Número Único**.

```text
S + 10 caracteres
Exemplo: S1234567890
```

O `S` existe somente na etiqueta. O banco armazena os 10 caracteres seguintes.

### SupplierBox já cadastrada

Quando a caixa já existir:

- o part number ZF será preenchido automaticamente;
- a quantidade restante registrada será mostrada;
- o produto será comparado com o produto da Work Order e da caixa ZF em andamento.

### SupplierBox nova

Quando a caixa ainda não existir, leia também:

PartNumber ZF:

```text
P + 8 caracteres
Exemplo: PA013F520
```

Quantidade da caixa:

```text
Q + 3 dígitos
Exemplo: Q420
```

A quantidade precisa ser maior que zero. Após essa leitura, a SupplierBox é cadastrada no banco.

## 5. Confirmar a SupplierBox e liberar o processo

Clique no botão verde `✓` da seção de logística.

Na confirmação, a aplicação:

1. Confirma que o `PartTypeData` da Work Order foi carregado no ACC.
2. Compara o PartNumber ZF da SupplierBox com o produto local.
3. Trava o produto durante toda a montagem da caixa ZF.
4. Cria a caixa ZF em andamento e libera a leitura dos sensores.

O botão da logística não executa `PartTypeList`, `PartTypeData` nem altera o `PartTypeID`. Se a configuração ACC da Work Order estiver ausente ou não corresponder à ordem corrente, a leitura dos sensores permanecerá bloqueada.

## 6. Ler os sensores

Posicione o sensor e leia o serial.

Formato esperado:

```text
9 caracteres no total
Os 4 primeiros caracteres identificam o produto
```

Para cada leitura, a aplicação verifica:

- existência do contexto da Work Order e da caixa;
- limite de quantidade da caixa ZF;
- comprimento do serial;
- prefixo cadastrado;
- compatibilidade com o produto travado;
- duplicidade no banco;
- duplicidade na caixa atual;
- disponibilidade da SupplierBox;
- existência do `PartTypeID` do ACC.

Depois das validações, o sistema:

1. Reserva uma unidade da SupplierBox em memória.
2. Se existir um sensor anterior pendente, executa `Unload OK` desse sensor.
3. Executa somente o `Load` do novo serial.
4. Grava o novo sensor como `Loaded`, mantendo-o como o único sensor pendente no ACC.
5. Atualiza a lista e o contador.

O sensor pendente recebe `Unload OK` somente quando o próximo sensor válido é lido. Se o ACC falhar antes do `Load` do novo sensor, a reserva da SupplierBox é restaurada e o novo sensor não é gravado localmente.

### Último sensor da caixa

Ao atingir a quantidade planejada, o sistema pergunta se todos os sensores estão seguros dentro da caixa:

- **SIM:** executa `Unload OK` do último sensor e, somente após sucesso, abre a tela verde da etiqueta final;
- **NÃO:** mantém o último sensor pendente para que o operador possa marcá-lo como scrap;
- falha no `Unload`: mantém a finalização bloqueada e não abre a tela verde.

### Recuperar após um NOK

Quando o painel estiver vermelho e começar com `NOK`, clique no próprio painel de resultado para repetir a etapa que realmente falhou.

- falha no `PartTypeList`, `PartTypeData` ou configuração da Work Order: retorna para `txtWorkOrderNumber`;
- falha de validação ou `Load/Unload` de um sensor: retorna para `txtComponentSerial`, desde que todo o contexto do processo ainda seja válido;
- contexto incompleto: bloqueia a leitura do sensor e retorna para a Work Order.

Não repita a leitura sem antes entender a mensagem apresentada.

## 7. Marcar o último sensor como scrap

1. Selecione o primeiro serial da lista, que corresponde ao último sensor lido.
2. Clique em **REMOVER (SCRAP)**.

O sistema:

- permite scrap somente para o sensor que ainda está com `Load` pendente;
- executa `Unload NOK` com `statusBits=0` e `failureBits=1`;
- mantém permanentemente o sensor no banco como scrap;
- registra o operador, data e hora do scrap;
- reduz o contador;
- não devolve a unidade ao saldo da SupplierBox, pois a peça foi consumida fisicamente;
- impede definitivamente uma nova leitura do mesmo serial.

Ao tentar reler o serial, a aplicação informa qual operador marcou o componente como scrap. Qualquer item que não seja o primeiro da lista permanece bloqueado para scrap porque já recebeu `Unload OK` quando o componente seguinte foi lido.

## 8. Trocar a SupplierBox

Use **TROCAR SUPPLIER BOX** quando a caixa física ficar vazia ou precisar ser substituída durante o processo.

1. Confirme a intenção de trocar a caixa.
2. Informe se deseja zerar o saldo restante da SupplierBox atual no sistema.
3. Leia a nova SupplierBox.
4. Confirme a etiqueta no botão verde `✓`.

Regras obrigatórias:

- a nova SupplierBox deve pertencer ao mesmo produto da caixa ZF em andamento;
- não é permitido alterar o part number;
- o produto e o `PartTypeID` permanecem travados;
- a troca não executa um novo `PartTypeData`;
- apenas a SupplierBox de origem e o saldo corrente são alterados.

Se o saldo chegar a zero durante a produção, o programa pergunta se ainda existem sensores fisicamente na caixa:

- **Sim:** permite continuar usando a caixa e registra o uso além do saldo calculado;
- **Não:** pausa a leitura e solicita outra SupplierBox.

## 9. Finalizar a caixa ZF

Quando o contador atingir a meta, a tela de finalização será aberta automaticamente.

Leia na ordem:

1. Número único da caixa final: `1J` + 10 caracteres.
2. PartNumber Final: `P` + 10 ou 11 caracteres.
3. Work Order: `O` + 12 caracteres.
4. Lote: `H` + 10 caracteres.

O PartNumber Final e a Work Order precisam ser iguais aos valores usados no início do processo.

Após a última leitura válida, a aplicação:

- grava o número único e o lote da caixa ZF;
- marca a caixa como finalizada;
- marca seus sensores como finalizados;
- calcula quantos sensores vieram de cada SupplierBox;
- debita essas quantidades do saldo persistido de cada SupplierBox, nunca deixando saldo negativo;
- registra a finalização no log;
- limpa a tela para a próxima Work Order.

Durante os scans, o saldo mostrado como `Q...` é apenas uma reserva operacional em memória. O campo persistido `SupplierBox.QtyRemaining` é debitado uma única vez na finalização, usando a quantidade real de sensores vinculados a cada SupplierBox. Isso evita débito duplicado em commits intermediários ou retomadas automáticas.

## 10. Interromper e deixar uma caixa aguardando

Use **INTERROMPER PROCESSO** quando uma caixa em andamento precisar ser temporariamente liberada da estação sem ser finalizada.

1. Informe o RE de qualquer usuário cadastrado como administrador.
2. Confirme os dados da Work Order e a quantidade já lida.
3. Reconfirme a interrupção na última pergunta.

Se qualquer etapa for cancelada, nada é alterado. Após a confirmação final, a caixa e seus sensores permanecem marcados como `Em andamento`, a tela é liberada e a ação é registrada com o operador atual e o administrador autorizador. Para retomá-la, use **CONTINUAR PROCESSO**.

Além de `InProgress`, a caixa recebe o estado persistente `IsPaused`. Isso diferencia uma interrupção autorizada de um fechamento inesperado da aplicação.

O botão fica desabilitado quando não há caixa em andamento ou durante uma comunicação de sensor com o ACC.

## 11. Continuar um processo interrompido

Use **CONTINUAR PROCESSO** somente na tela inicial, antes de iniciar outra Work Order.

1. Selecione uma caixa marcada como `Em andamento`.
2. Confirme a seleção.
3. A aplicação recuperará Work Order, produto, meta e sensores já lidos.
4. Antes de restaurar o processo, o sistema procura a Work Order no `PartTypeList` e executa novamente o `PartTypeData`.
5. Leia uma SupplierBox válida e confirme a logística para continuar.

As opções são mostradas no formato:

```text
WO ... | produto final | quantidade | identificação da caixa ZF
```

Caixas ainda sem etiqueta final aparecem como `(sem etiqueta)`.

### Retomada automática após reabertura

Se a aplicação for fechada sem usar **INTERROMPER PROCESSO**, a caixa continua ativa. Na próxima abertura, o sistema automaticamente:

1. identifica a caixa ativa mais recente;
2. recupera Work Order, produto, meta, sensores e contador;
3. executa novamente o `PartTypeData` da Work Order no ACC;
4. recupera a SupplierBox que estava em uso e recompõe o saldo operacional;
5. libera a leitura do próximo sensor.

Caixas pausadas explicitamente não são retomadas automaticamente e continuam disponíveis em **CONTINUAR PROCESSO**. Se a SupplierBox persistida não puder ser recuperada, o sistema preserva a caixa e solicita novamente a leitura da etiqueta logística.

## 12. Consultar um componente

1. Clique em **CONSULTAR COMPONENTE**.
2. Digite ou leia o serial.
3. Pressione `Enter` ou clique em pesquisar.

A consulta mostra:

- serial;
- data e hora da leitura;
- Work Order;
- SupplierBox;
- caixa ZF;
- nome do usuário que realizou o scan;
- situação `Em andamento` ou `Finalizado`.

## 13. Consultar caixas finalizadas por Work Order

1. Clique em **CONSULTAR ORDEM / HU**.
2. Digite ou escaneie a Work Order. A consulta aceita o número com ou sem a letra `O` inicial.
3. Pressione `Enter` ou clique em **PESQUISAR**.
4. Selecione uma HU e clique em **ABRIR COMPONENTES**, ou dê um duplo clique na linha.

A primeira tela mostra somente caixas finalizadas, com HU `1J...`, lote, quantidade de componentes, PartNumber final e usuário responsável pela caixa. A tela de detalhes mostra todos os sensores vinculados à HU, incluindo data e hora, usuário do scan, SupplierBox e status.

### Formatos válidos de Work Order

| Etiqueta | Tipo | Uso |
|---|---|---|
| `OD###############` | Dummy | Casos especiais autorizados pelo time de IT, como peças reclamadas pelo cliente |
| `O11##########` | Work Order | Processo normal de produção |
| `O12##########` | Work Order Rework | Peças que passaram por retrabalho |

Cada `#` representa um dígito. A aplicação remove somente a primeira letra `O` antes de gravar a ordem no banco e antes de procurar o valor na `Description` do `PartTypeList`. Assim, por exemplo, `O12##########` é tratado internamente como `12##########`.

---

# Manual da Manutenção

## 1. Dependências de funcionamento

A estação precisa de:

- Windows 64 bits;
- runtime compatível com `.NET 10 Desktop`;
- acesso de leitura e escrita a `C:\ProgramData`;
- arquivo `appsettings.json` no diretório da aplicação;
- arquivo `ZF.ACCComm.dll` no diretório publicado;
- rota de rede liberada até o servidor ACC;
- resolução normal do usuário do Windows.

## 2. Configuração atual do ACC

```json
{
  "Acc": {
    "IpAddress": "10.219.61.125",
    "Port": 3802,
    "DllVersion": "1.0.7.15",
    "ProductType": "SENSOR",
    "Station": "10.1"
  }
}
```

A mesma `Station` é usada por `PartTypeList`, `PartTypeData`, `Load` e `Unload`.

Após alterar o arquivo, reinicie a aplicação. As configurações são carregadas durante a criação do processo.

A aplicação usa um mutex global do Windows e permite apenas uma instância por estação. Ao tentar abrir uma segunda instância, o usuário recebe uma mensagem e o segundo processo é encerrado antes de acessar o banco.

### Bypass do ACC para desenvolvimento

Em uma compilação `Debug`, a Work Order de teste abaixo ativa o bypass do ACC:

```text
O012345678912
```

Nesse modo, o sistema mantém as validações e o fluxo local, mas não envia `PartTypeList`, `PartTypeData`, `Load` ou `Unload` ao ACC. A ativação e cada ciclo de sensor ignorado são registrados no log como eventos de nível `Warning`.

Para tornar o ambiente de teste evidente, compilações `Debug` exibem a palavra **DEBUG**, em vermelho, ao lado do contador de sensores.

O bypass é protegido por compilação condicional (`#if DEBUG`). Em uma compilação `Release`, a chave e a lógica de bypass não fazem parte do executável; a mesma Work Order segue o fluxo ACC normal.

## 3. Banco de dados

O banco SQLite fica em:

```text
C:\ProgramData\HondaSensorChecker\Database\HondaSensorChecker.db
```

Ao iniciar uma versão atualizada:

- se existir um banco apenas em `C:\ProgramData\HondaSensorChecker.db`, a aplicação move automaticamente o banco e seus arquivos SQLite auxiliares para a nova pasta;
- se não existir banco em nenhum dos locais, um banco novo é criado diretamente no novo caminho pelas migrations;
- se já existir banco no novo caminho, ele é utilizado normalmente e nenhuma migração do local antigo é executada.

Na inicialização, a aplicação:

1. verifica se o arquivo já existia;
2. aplica as migrations do Entity Framework;
3. se for o primeiro banco, cria o usuário atual como administrador;
4. cadastra a lista inicial de produtos.

### Backup recomendado

Antes de manutenção, atualização ou troca de computador:

1. Feche a aplicação.
2. Copie `C:\ProgramData\HondaSensorChecker\Database\HondaSensorChecker.db` para um local controlado.
3. Registre data, máquina e versão da aplicação.

Não copie o banco enquanto uma finalização ou leitura estiver sendo gravada.

## 4. Testes básicos de rede do ACC

No PowerShell da estação:

```powershell
Test-NetConnection 10.219.61.125 -Port 3802
```

Interpretação:

- `TcpTestSucceeded: True`: a porta está acessível; prossiga com análise de estação, produto, DLL e resposta do ACC.
- `TcpTestSucceeded: False`: verifique cabo, VLAN, rota, firewall, servidor e serviço ACC.

Um teste TCP positivo não garante que a estação `10.1` ou o produto estejam configurados no ACC.

## 5. Diagnóstico por sintoma

### `USUÁRIO NÃO REGISTRADO`

- confira o usuário atual do Windows;
- confirme que o ZF-ID cadastrado é igual ao login sem domínio;
- peça a um administrador para corrigir o cadastro;
- não substitua o banco sem backup.

### `CONFIGURAÇÃO DO ACC INCOMPLETA`

Confira todos os campos de `Acc` no `appsettings.json`. IP vazio, porta fora de `1..65535`, versão, tipo de produto ou estação vazios bloqueiam o processo.

### Work Order não encontrada no ACC

- confirme o número da Work Order sem o `O` inicial;
- confirme que esse número aparece no campo Description/`PartDesc` do cadastro ACC;
- confirme `ProductType` e `Station` no servidor ACC;
- consulte o `PartTypeList` no ambiente ACC;
- verifique se existe exatamente uma correspondência.

### Work Order possui múltiplas correspondências

Mais de um `PartDesc` contém o mesmo número. O sistema bloqueia o `PartTypeData` para não selecionar um `PartTypeID` arbitrariamente. Corrija o cadastro no ACC.

### `NOK ACC` durante sensor

- verifique rede e serviço ACC;
- confirme que o `PartTypeData` da Work Order foi concluído;
- confira a mensagem técnica exibida e o log;
- não force gravações diretamente no banco;
- após corrigir a causa, clique no painel NOK e releia o sensor.

### `PARTTYPEID NÃO CARREGADO`

- volte ao campo da Work Order e provoque novamente sua validação;
- confirme que a Description do `PartTypeList` contém a ordem;
- em uma retomada, selecione novamente a caixa para repetir o `PartTypeData`.

### Sensor duplicado

- use **CONSULTAR COMPONENTE**;
- verifique em qual Work Order, SupplierBox e caixa ZF ele foi utilizado;
- não apague registros diretamente para liberar uma nova leitura sem aprovação da Engenharia/Qualidade.

### Saldo da SupplierBox divergente

- compare quantidade física e quantidade mostrada;
- use a troca manual de SupplierBox quando a caixa física acabou;
- responda corretamente à pergunta sobre zerar o saldo;
- consulte os logs de mudança, overdraw e finalização.

### Não há caixa para continuar

Não existe nenhuma `ZfBox` com `InProgress = true`. Verifique se a caixa foi finalizada, se o banco correto está em uso ou se houve troca do arquivo do banco.

### Erro de banco ou commit

- feche instâncias duplicadas do programa;
- confira espaço em disco e permissão de escrita em `C:\ProgramData`;
- preserve o banco antes de qualquer reparo;
- registre a mensagem completa;
- escale para Engenharia se houver corrupção ou falha de migration.

## 6. Logs disponíveis

O sistema usa dois tipos complementares de log.

### Auditoria no banco

Administradores podem abrir **LOGS**. A tela ordena as entradas da mais recente para a mais antiga e mostra:

- data e hora;
- operador;
- descrição.

A tela permite pesquisar pela descrição ou pelo operador, filtrar por operador e período e consultar a mensagem completa no painel de detalhes. O contador no cabeçalho mostra quantos registros atendem aos filtros selecionados.

Eventos registrados incluem:

- criação de Work Order;
- criação, alteração e exclusão de produtos ou usuários;
- criação de SupplierBox;
- consulta e `PartTypeData` da Work Order no ACC, com sucesso ou falha;
- `Load/Unload` ACC bem-sucedido ou com falha;
- solicitação e confirmação de troca de SupplierBox;
- zeragem e uso além do saldo;
- remoção de sensor como scrap;
- finalização da caixa ZF.

Esses registros permanecem no SQLite porque fazem parte da rastreabilidade operacional e mantêm vínculo com o operador.

### Log técnico em arquivo

O diagnóstico detalhado da aplicação fica em:

```text
C:\ProgramData\HondaSensorChecker\Logs\
```

É criado um arquivo por dia:

```text
HondaSensorChecker-AAAA-MM-DD.log
```

O arquivo é texto UTF-8 e cada evento possui um bloco legível contendo:

- data e hora com fuso;
- nível (`Debug`, `Information`, `Warning`, `Error` ou `Critical`);
- nome estável do evento;
- mensagem;
- máquina, usuário Windows, processo e thread;
- operador, Work Order, produto, SupplierBox, ZfBox e contadores, quando disponíveis;
- endpoint, estação, `PartTypeID` e description do ACC;
- tipo, mensagem e stack trace completo da exceção.

O arquivo registra também inicialização, encerramento normal, falhas globais não tratadas, validações rejeitadas, cliques de retomada no painel NOK e falhas ao gravar a própria auditoria no banco.

Os arquivos são mantidos por 90 dias. Uma falha no log técnico ou na auditoria não deve derrubar o processo principal.

### Por que manter arquivo e banco

- banco: auditoria e rastreabilidade ligada às entidades do processo;
- arquivo: diagnóstico técnico detalhado, inclusive quando o banco está indisponível;
- manter somente o arquivo perderia as relações estruturadas com operador e processo;
- manter somente o banco dificultaria investigar stack traces, falhas de inicialização e problemas do próprio SQLite.

---

# Manual da Engenharia

## 1. Cadastro de usuários

Disponível somente para administradores em **+ USUÁRIO**.

Campos:

- `RE`;
- `ZF-ID`, usado para reconhecer o login do Windows;
- `NAME`;
- `ADMIN`.

Regras:

- todos os campos de texto são obrigatórios;
- RE e ZF-ID não podem se repetir;
- entradas são normalizadas para maiúsculas;
- linhas da grade podem ser editadas diretamente;
- o usuário atualmente conectado não pode excluir a si mesmo;
- usuários com sensores, caixas ou logs vinculados não podem ser excluídos devido às relações restritivas.

## 2. Cadastro de produtos

Disponível somente para administradores em **+ PRODUTO**.

Cada produto possui:

| Campo | Uso |
|---|---|
| Prefix | Quatro primeiros caracteres do serial do sensor |
| ZF PartNumber | Part number inicial usado para validar a SupplierBox |
| ELSEN/ELMOD | Part number final da Work Order e da caixa ZF |

Regras de criação:

- todos os campos são obrigatórios;
- ZF PartNumber e ELSEN/ELMOD não podem se repetir;
- ELSEN/ELMOD válido: `ELMOD` + 5 dígitos, `ELSEN` + 5 dígitos, ou `ELSENA` + 5 dígitos;
- valores são normalizados para maiúsculas.

Regras de edição:

- a grade permite editar os campos diretamente;
- a edição rejeita prefixo duplicado;
- produtos em uso devem preferencialmente permanecer imutáveis para preservar rastreabilidade.

Regras de exclusão:

- a exclusão é bloqueada quando existem sensores, SupplierBoxes, caixas ZF ou Work Orders vinculados;
- o sistema mostra a quantidade de dependências encontrada.

## 3. Modelo de rastreabilidade

Cada sensor guarda referências para:

- produto;
- operador;
- SupplierBox;
- Work Order;
- caixa ZF;
- data e hora;
- status em andamento/finalizado.

Isso permite rastrear o caminho completo do componente desde a SupplierBox até a caixa expedida.

## 4. Regras de integridade do processo

- uma caixa ZF possui um único produto e uma única Work Order;
- o `PartTypeID` é determinado pela Work Order e o produto é travado ao confirmar a SupplierBox;
- a troca de SupplierBox não pode mudar o produto;
- o serial não pode ser reutilizado em outra caixa;
- todos os sensores da caixa devem atingir a quantidade selecionada antes da finalização;
- o part number e a Work Order da etiqueta final devem corresponder ao contexto inicial;
- exclusões em cascata não são usadas; as principais relações têm `DeleteBehavior.Restrict`.

## 5. Comportamento do estoque

Durante a leitura, o saldo é mantido em memória para orientar o operador. O débito persistente no banco ocorre na finalização da caixa, agrupado pela SupplierBox gravada em cada sensor.

Consequências:

- uma caixa ZF pode consumir sensores de várias SupplierBoxes do mesmo produto;
- o histórico preserva a SupplierBox individual de cada sensor;
- remover um sensor em andamento ajusta o saldo em memória;
- finalizar a caixa debita o total real utilizado por SupplierBox;
- o saldo persistido nunca é reduzido abaixo de zero;
- a zeragem manual grava zero imediatamente.

## 6. Integração ACC

### Changeover pela Work Order

No evento `Leave` de `txtWorkOrderNumber`, depois de validar um dos três formatos aceitos:

```text
Etiqueta normal: O11##########
Valor procurado: 11##########
Comando: PartTypeList(Station, ProductType, DllVersion)
Filtro: PartDesc contém o número da Work Order
Resultado exigido: exatamente uma correspondência
Comando de configuração: PartTypeData(Station, ProductType, DllVersion, PartTypeID)
```

O `PartTypeID`, o `PartDesc` escolhido e o retorno de `PartTypeData` ficam somente em memória. O comando `PartTypeData` baixa os parâmetros do Part Number e configura o ACC com o modelo em produção. O botão da SupplierBox apenas verifica que a descrição carregada continua correspondendo à Work Order atual. Ao retomar uma caixa após reiniciar a aplicação, `PartTypeList` e `PartTypeData` são executados novamente antes de restaurar o processo.

### Ciclo do sensor

Para cada novo serial aprovado localmente:

```text
Sensor N: Load(Station, ProductType, DllVersion, PartTypeID, [serial N], null)
  ↓ próximo sensor válido
Sensor N: Unload(..., statusBits: 1, failureBits: 0, [serial N])
Sensor N+1: Load(..., [serial N+1], null)
```

Para scrap do único sensor pendente:

```text
Unload(..., statusBits: 0, failureBits: 1, [último serial])
```

As chamadas são síncronas na DLL e são executadas dentro de `Task.Run` para não bloquear a interface.

O banco persiste o estado `Loaded`, `UnloadedOk` ou `UnloadedNok`, além de `PartTypeID`, `CycleID`, `UnitPartTypeID`, horário do Unload e dados de auditoria do scrap. Isso permite restaurar uma caixa sem repetir o `Load` do último sensor.

## 7. Persistência e inicialização

Tecnologias:

- .NET 10 Windows Forms;
- Entity Framework Core 10;
- SQLite;
- injeção de dependências com Generic Host;
- `ZF.ACCComm.dll` versão configurada `1.0.7.15`;
- plataforma alvo x64.

Entidades:

| Entidade | Finalidade |
|---|---|
| Operator | Identidade e permissão administrativa |
| Product | Relação entre prefixo, ZF PN e ELSEN/ELMOD |
| SapWorkOrder | Ordem e produto esperado |
| SupplierBox | Origem logística e saldo |
| ZfBox | Caixa de destino e estado do processo |
| Sensor | Unidade rastreada e seus relacionamentos |
| Log | Auditoria operacional e administrativa |

O log técnico em arquivo não é uma entidade do banco; ele é produzido pelo `ApplicationFileLogger` em texto estruturado para leitura direta no Bloco de Notas ou em qualquer editor de logs.

## 8. Estrutura do código

```text
Main.cs                         Fluxo principal, regras, estoque e ACC
Windows/FinishedBox.cs          Validação e persistência da etiqueta final
Windows/ComponentHistoryDialog  Consulta de rastreabilidade por serial
Windows/ContinueProcessDialog   Seleção de caixas em andamento
Windows/Users.cs                Administração de operadores
Windows/Products.cs             Administração de produtos
Windows/Logs.cs                 Consulta de auditoria
Logging/ApplicationFileLogger  Log técnico diário, exceções e retenção
Data/Context                    DbContext e relacionamentos
Data/Repositories               Repositórios e Unit of Work
Models                          Entidades persistidas
Migrations                      Esquema do SQLite
Configuration/AccSettings.cs    Modelo de configuração do ACC
appsettings.json                Endpoint e parâmetros do ACC
Assemblies/ZF.ACCComm.dll       Biblioteca de comunicação ACC
```

## 9. Compilação

Na raiz do repositório:

```powershell
dotnet restore
dotnet build
```

Saída de desenvolvimento:

```text
bin\Debug\net10.0-windows\
```

Confirme que a saída contém:

- executável e assemblies da aplicação;
- `appsettings.json`;
- `ZF.ACCComm.dll`;
- bibliotecas nativas do SQLite.

## 10. Publicação

Exemplo de publicação dependente do runtime:

```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

Antes de liberar uma versão:

- valide o build sem erros;
- confirme os parâmetros do ACC do ambiente destino;
- verifique a presença da DLL ACC;
- faça backup do banco da estação;
- teste uma Work Order válida no `PartTypeList` e o respectivo `PartTypeData`;
- teste `Load/Unload` em ambiente autorizado;
- teste retomada de processo;
- teste troca de SupplierBox do mesmo produto e rejeição de produto diferente;
- teste finalização com mais de uma SupplierBox;
- registre versão, data e responsável pela liberação.

---

# Formatos de leitura

| Etapa | Formato | Exemplo | Valor armazenado/usado |
|---|---|---|---|
| Work Order | `O` + 12 caracteres | `O123456789012` | 12 caracteres sem `O` |
| PartNumber Final | `P` + 10 ou 11 caracteres | `PELMOD00660` | sem `P` |
| Número único SupplierBox | `S` + 10 caracteres | `S1234567890` | 10 caracteres sem `S` |
| ZF PartNumber | `P` + 8 caracteres | `PA013F520` | sem `P`; valida a SupplierBox |
| Quantidade SupplierBox | `Q` + 3 dígitos | `Q420` | inteiro positivo |
| Serial do sensor | 9 caracteres | conforme produto | completo; prefixo = 4 primeiros |
| Número caixa final | `1J` + 10 caracteres | `1J1234567890` | 10 caracteres sem `1J` |
| Work Order final | `O` + 12 caracteres | `O123456789012` | validada contra o início |
| Lote final | `H` + 10 caracteres | `H1234567890` | 10 caracteres sem `H` |

Todas as entradas são aparadas e convertidas para maiúsculas antes da validação.

---

# Mensagens e diagnóstico

| Mensagem | Causa provável | Ação |
|---|---|---|
| `USUÁRIO NÃO REGISTRADO` | ZF-ID não corresponde ao login Windows | Cadastrar/corrigir operador |
| `CONFIRA O NÚMERO DA WORK-ORDER` | Formato diferente de `O` + 12 | Reler etiqueta correta |
| `PARTNUMBER NÃO REGISTRADO` | Produto final ou ZF PN ausente | Acionar Engenharia |
| `PARTNUMBER ... NÃO COINCIDE` | SupplierBox de outro produto | Separar material e ler caixa correta |
| `CONFIGURAÇÃO DO ACC INCOMPLETA` | Campo ACC ausente/inválido | Corrigir `appsettings.json` e reiniciar |
| `Work Order ... não encontrada no PartTypeList` | Nenhum `PartDesc` contém a ordem | Conferir cadastro, station e product type |
| `Work Order ... possui ... correspondências` | Busca ambígua no ACC | Corrigir descriptions duplicadas/sobrepostas |
| `PARTTYPEID NÃO CARREGADO` | Changeover da Work Order não concluído | Validar novamente a Work Order |
| `NOK ACC` | Erro de conexão ou comando | Ver mensagem, rede, servidor e logs |
| `JÁ FOI EXPEDIDO EM OUTRA CAIXA` | Serial já existe no banco | Consultar componente e segregar peça |
| `JÁ FOI LIDO NESTA MESMA CAIXA` | Leitura duplicada local | Não repetir; conferir lista |
| `ESPERADO ... LIDO ...` | Prefixo de outro produto | Segregar sensor incorreto |
| `CAIXA ... JÁ COMPLETA` | Meta já atingida | Finalizar ou retomar corretamente |
| `CAIXA NÃO ENCONTRADA NO BANCO` | Referência inconsistente | Manutenção/Engenharia deve analisar banco |
| `LEITURA INCORRETA` na etiqueta final | Prefixo ou tamanho inválido | Reiniciar a sequência da etiqueta final |

---

# Arquitetura e dados

## Fluxo de estados

```text
Tela limpa
  → Work Order confirmada
  → SupplierBox confirmada
  → PartTypeID ACC carregado
  → ZfBox InProgress = true
  → Sensores InProgress = true
  → Meta atingida
  → Etiqueta final validada
  → ZfBox e sensores InProgress = false
```

## Relacionamentos principais

```text
Product
 ├─ SapWorkOrders
 ├─ SupplierBoxes
 ├─ ZfBoxes
 └─ Sensors

ZfBox
 ├─ SapWorkOrder
 ├─ Product
 ├─ Operator
 └─ Sensors

Sensor
 ├─ Product
 ├─ Operator
 ├─ SupplierBox
 ├─ SapWorkOrder
 └─ ZfBox
```

As exclusões relacionadas são restritas para preservar o histórico.

---

# Configuração e implantação

## Arquivo `appsettings.json`

| Campo | Descrição |
|---|---|
| `IpAddress` | Servidor ACC |
| `Port` | Porta TCP do ACC |
| `DllVersion` | Versão informada nos comandos |
| `ProductType` | Tipo de produto cadastrado no ACC |
| `Station` | Estação usada em todos os comandos |

O arquivo é copiado automaticamente para as saídas de build e publicação.

## DLL ACC

O projeto referencia:

```text
Assemblies\ZF.ACCComm.dll
```

A propriedade `Private=true` faz a DLL ser copiada para a saída. A referência deve permanecer versionada e alinhada ao valor de `DllVersion`.

## Segurança operacional

- Não teste `Load/Unload` em produção sem autorização.
- Não edite o banco diretamente com a aplicação aberta.
- Não substitua `appsettings.json` entre ambientes sem revisar IP, porta, produto e estação.
- Não reutilize banco de teste em produção.
- Preserve os logs e o banco ao investigar rastreabilidade.

---

# Pontos técnicos a verificar

Os itens abaixo foram identificados no mapeamento do código atual e devem ser avaliados antes da liberação produtiva.

## 1. Prefixo inicial duplicado

O seed inicial contém dois produtos com o prefixo `C2PK`. A identificação do produto do sensor usa apenas os quatro primeiros caracteres e seleciona o primeiro registro encontrado.

Antes da produção, cada modelo precisa ter um prefixo de sensor inequívoco ou a regra de identificação deve usar mais dados do serial.

## 2. Mapeamento dos bits de resultado

O processo usa o bit zero geral definido pela especificação: `statusBits=1/failureBits=0` para OK e `statusBits=0/failureBits=1` para NOK. Engenharia deve confirmar que a estação `10.1` não exige bits adicionais específicos do processo.

## 3. Falha local após sucesso no ACC

Se o `Load` concluir no ACC e a gravação local falhar, a aplicação tenta compensar imediatamente com `Unload NOK` e registra uma ocorrência crítica se a compensação também falhar. Se um `Unload` concluir e a atualização local falhar, o processo permanece bloqueado para intervenção da Manutenção, evitando o avanço silencioso.

---

# Checklist rápido de liberação

- [ ] Usuário operador cadastrado com ZF-ID correto.
- [ ] Produtos e prefixos revisados.
- [ ] Work Order e etiquetas de teste disponíveis.
- [ ] Banco em `C:\ProgramData` com permissão de escrita.
- [ ] Backup do banco realizado.
- [ ] `appsettings.json` revisado para o ambiente.
- [ ] `ZF.ACCComm.dll` presente na saída.
- [ ] Conectividade TCP com o ACC validada.
- [ ] `PartTypeList` encontra exatamente uma Description contendo a Work Order sem `O`.
- [ ] `PartTypeData` conclui com o `PartTypeID` encontrado.
- [ ] `Load/Unload` validados com a estação `10.1`.
- [ ] Ao ler o sensor seguinte, o anterior recebe `Unload OK` antes do novo `Load`.
- [ ] Somente o primeiro item da lista permite scrap e recebe `Unload NOK`.
- [ ] O último sensor exige confirmação antes da etiqueta final.
- [ ] Sensor scrapado não pode ser lido novamente e identifica o operador responsável.
- [ ] Leitura do sensor associada ao handler correto.
- [ ] Troca de SupplierBox rejeita produto diferente.
- [ ] Retomada de processo testada.
- [ ] Finalização e débito de múltiplas SupplierBoxes testados.
- [ ] Consulta de componente e logs verificados.
- [ ] Consulta por Work Order lista somente HUs finalizadas.
- [ ] Duplo clique ou botão em uma HU exibe todos os componentes da caixa.
