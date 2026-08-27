# Honda Sensor Checker — Rastreabilidade e integração ACC

Aplicação Windows para controlar a montagem e a expedição de caixas de sensores Honda. O sistema relaciona cada sensor à Work Order SAP, ao produto, à SupplierBox de origem, à caixa ZF de destino e ao operador responsável. A integração ACC usa `PartTypeList` e `PartTypeData` para selecionar e configurar o Part Number da Work Order e `Load`/`Unload` para cada sensor.

Este documento serve como manual oficial para Operação, Manutenção e Engenharia. Ele descreve o comportamento implementado no código atual, os limites de responsabilidade de cada perfil, a integração ACC, a persistência dos dados, os procedimentos de diagnóstico e os testes mínimos para liberação.

| Documento | Aplicação |
|---|---|
| Sistema | Honda Sensor Checker |
| Plataforma | Windows Forms, .NET 10, x64 |
| Banco | SQLite local com migrations do Entity Framework Core |
| Integração | ZF ACCComm |
| Público | Operação, Manutenção e Engenharia |
| Fonte de verdade | Código, migrations e `appsettings.json` deste repositório |

> Sempre que houver diferença entre uma cópia impressa deste manual e o comportamento de uma versão instalada, preserve o processo, registre a versão do executável e acione a Engenharia. Não corrija rastreabilidade diretamente no banco sem análise e backup.

Para correções excepcionais no SQLite, consulte o [Manual de diagnóstico e correção do banco](docs/manual-manutencao-banco.md). Ele contém consultas, pré-condições, transações, auditoria e os casos que exigem reconciliação com o ACC.

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
- [Referência de métodos](#referência-de-métodos)
- [Testes de aceitação](#testes-de-aceitação)
- [Controle de mudanças](#controle-de-mudanças)
- [Manual de manutenção do banco de dados](docs/manual-manutencao-banco.md)

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

### Mapa da tela principal

| Área | Campo ou botão | Finalidade |
|---|---|---|
| Cabeçalho | `+ USUÁRIO` | Cadastro de operadores e administradores |
| Cabeçalho | `LOGS` | Consulta da auditoria persistida no banco |
| Cabeçalho | `+ PRODUTO` | Cadastro da relação prefixo / ZF PN / ELSEN-ELMOD |
| Cabeçalho | contador `000/000` | Quantidade de sensores bons lidos e meta da caixa |
| SAP - Work Order | Nº da ordem | Leitura e changeover da ordem no ACC |
| SAP - Work Order | PartNumber Final | Identificação do produto final de uma ordem nova |
| SAP - Work Order | Quantidade a enviar | Meta de sensores bons da caixa ZF |
| ZF - Logistic Label | Número Único | Identificação da SupplierBox |
| ZF - Logistic Label | PartNumber ZF | Produto da SupplierBox |
| ZF - Logistic Label | Quantidade da Caixa | Quantidade inicial ou saldo operacional |
| Sensor Checker | Leitura | Serial individual do sensor |
| Sensor Checker | painel colorido | Estado, orientação e recuperação da etapa atual |
| Ações | Consultar componente | Histórico de um serial |
| Ações | Consultar ordem / HU | HUs finalizadas por Work Order e seus componentes |
| Ações | Interromper processo | Pausa autorizada e persistente da caixa atual |
| Ações | Continuar processo | Retomada manual de uma caixa em andamento |
| Ações | Trocar SupplierBox | Substituição da origem logística sem mudar o produto |
| Ações | Marcar como scrap | `Unload NOK` exclusivo do último sensor pendente |

---

# Manual do Operador

## 1. Início da aplicação

1. Entre no Windows com seu usuário pessoal.
2. Abra o Sensor Checker.
3. Confirme que a tela mostra `LEIA A WORK-ORDER`.

Se aparecer `USUÁRIO NÃO REGISTRADO`, não prossiga. Solicite a um administrador o cadastro do seu ZF-ID.

A aplicação aceita somente uma instância por estação. Se já estiver aberta, uma segunda execução exibirá uma mensagem e será encerrada. Antes de concluir que o sistema não abriu, verifique a barra de tarefas e as janelas minimizadas.

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
3. Work Order: o mesmo formato válido lido no início (`O11...`, `O12...` ou `OD...`).
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
- situação `Load pendente`, `Em andamento`, `Finalizado` ou `Scrap - operador`, conforme o registro.

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

## 14. O que o operador não deve fazer

- Não misture part numbers na mesma caixa ZF ou durante a troca de SupplierBox.
- Não confirme o último sensor antes de garantir que todas as peças estão fisicamente seguras na caixa.
- Não tente marcar como scrap um sensor antigo da lista; somente o primeiro item ainda pode estar pendente no ACC.
- Não encerre a tela verde sem concluir a sequência da etiqueta final.
- Não abra uma segunda instância e não copie, substitua ou edite o banco com a aplicação aberta.
- Não repita scans após erro sem ler a mensagem do painel.
- Não use etiquetas, ordens ou seriais de teste no ambiente produtivo sem autorização.
- Não desligue a estação durante `LOAD`, `UNLOAD`, gravação da etiqueta final ou atualização do banco.

## 15. Encerramento e reinício

O fechamento inesperado não finaliza nem pausa automaticamente a caixa. Na próxima abertura, uma caixa ativa e não pausada é retomada automaticamente. Uma caixa interrompida pelo botão continua aguardando até ser escolhida em **CONTINUAR PROCESSO**.

Antes de desligamento planejado, escolha uma das condições:

1. Finalize normalmente a caixa e a etiqueta; ou
2. use **INTERROMPER PROCESSO**, conclua as três etapas de autorização e confirme que a tela voltou ao início.

Se houver um sensor em estado `Loaded`, sua identidade permanece no banco. A retomada não repete o `Load`; o sensor continua sendo o único candidato a `Unload OK` ou scrap. Se forem encontrados vários sensores `Loaded`, o sistema bloqueia a produção para reconciliação da Manutenção.

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
    "IpAddress": "10.219.61.136",
    "Port": 3802,
    "DllVersion": "2.1.0.0",
    "ProductType": "SENSOR",
    "Station": "20.1"
  }
}
```

A mesma `Station` é usada por `PartTypeList`, `PartTypeData`, `Load` e `Unload`.

Após alterar o arquivo, reinicie a aplicação. As configurações são carregadas durante a criação do processo.

A aplicação usa um mutex global do Windows e permite apenas uma instância por estação. Ao tentar abrir uma segunda instância, o usuário recebe uma mensagem e o segundo processo é encerrado antes de acessar o banco.

### Identificação da compilação Debug

Compilações `Debug` exibem a palavra **DEBUG**, em vermelho, ao lado do contador de sensores. Isso identifica uma versão de desenvolvimento, mas **não significa que o ACC esteja ignorado**.

O código mantém a constante histórica `O012345678912` para desenvolvimento, porém a ativação do bypass no changeover está desabilitada na versão atual. Portanto, inclusive em `Debug`, uma ordem aceita segue `PartTypeList`, `PartTypeData`, `Load` e `Unload` normalmente. Não use essa chave como método de teste sem ACC.

Em `Release`, o indicador DEBUG e os trechos protegidos por `#if DEBUG` não são compilados. A versão destinada ao time operacional deve sempre ser publicada em `Release`.

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
Test-NetConnection 10.219.61.136 -Port 3802
```

Interpretação:

- `TcpTestSucceeded: True`: a porta está acessível; prossiga com análise de estação, produto, DLL e resposta do ACC.
- `TcpTestSucceeded: False`: verifique cabo, VLAN, rota, firewall, servidor e serviço ACC.

Um teste TCP positivo não garante que a estação `20.1` ou o produto estejam configurados no ACC.

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

Identifique também qual comando falhou:

- `Load` do sensor atual: o serial novo não deve ser considerado aceito;
- `Unload OK` do sensor anterior: o próximo `Load` não é enviado e o sensor anterior continua pendente;
- `Unload NOK` de scrap: o sensor continua pendente e não deve ser removido fisicamente do controle até o sucesso;
- `Unload OK` do último sensor: a tela verde não é aberta enquanto a conclusão não for confirmada pelo ACC.

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

### `PROCESSO BLOQUEADO` ou reconciliação necessária

Esse bloqueio protege contra divergência entre o que o ACC aceitou e o que o SQLite conseguiu registrar. Não reinicie scans nem altere o banco para “liberar” a tela.

1. Preserve o arquivo `.log` do horário da falha.
2. Identifique serial, Work Order, `PartTypeID`, comando e retorno ACC.
3. Consulte no banco o `AccState` e os identificadores do ciclo.
4. Confirme no ACC se o `Load` ou `Unload` foi contabilizado.
5. Somente após reconciliar os dois lados, defina com Engenharia a ação corretiva.

### Aplicação retomou uma caixa inesperadamente

- confirme se a caixa possui `InProgress = true` e `IsPaused = false`;
- verifique se o fechamento anterior ocorreu sem **INTERROMPER PROCESSO**;
- consulte `CurrentSupplierBoxId`, sensores vinculados e logs do encerramento;
- não finalize ou apague a caixa apenas para limpar a tela.

### Vários sensores pendentes no ACC

O fluxo correto admite no máximo um sensor `Loaded` por caixa. Mais de um registro pendente indica dado legado, edição externa ou falha de persistência e provoca bloqueio crítico. Compare cada serial com o ACC antes de qualquer correção.

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
- marcar o último sensor como scrap reduz o contador de bons, mas não devolve a unidade ao saldo operacional, pois o sensor foi fisicamente consumido;
- finalizar a caixa debita o total real utilizado por SupplierBox;
- o débito final inclui sensores bons e sensores marcados como scrap;
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

O `PartTypeID`, o `PartDesc` escolhido e o retorno de `PartTypeData` formam o contexto ACC em memória. Quando um sensor é lido, seu `PartTypeID` e os identificadores retornados pelo `Load` também são persistidos no próprio registro. O comando `PartTypeData` baixa os parâmetros do Part Number e configura o ACC com o modelo em produção. O botão da SupplierBox apenas verifica que a descrição carregada continua correspondendo à Work Order atual. Ao retomar uma caixa após reiniciar a aplicação, `PartTypeList` e `PartTypeData` são executados novamente antes de restaurar o processo.

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
- `ZF.ACCComm.dll` com versão lógica configurada `2.1.0.0`;
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
| Work Order normal | `O11` + 10 dígitos | `O11##########` | 12 caracteres sem `O` |
| Work Order rework | `O12` + 10 dígitos | `O12##########` | 12 caracteres sem `O` |
| Work Order dummy | `OD` + 15 dígitos | `OD###############` | 16 caracteres sem `O` |
| PartNumber Final | `P` + 10 ou 11 caracteres | `PELMOD00660` | sem `P` |
| Número único SupplierBox | `S` + 10 caracteres | `S1234567890` | 10 caracteres sem `S` |
| ZF PartNumber | `P` + 8 caracteres | `PA013F520` | sem `P`; valida a SupplierBox |
| Quantidade SupplierBox | `Q` + 3 dígitos | `Q420` | inteiro positivo |
| Serial do sensor | 9 caracteres | conforme produto | completo; prefixo = 4 primeiros |
| Número caixa final | `1J` + 10 caracteres | `1J1234567890` | 10 caracteres sem `1J` |
| Work Order final | mesmo formato válido do início | `O11...`, `O12...` ou `OD...` | normalizada e validada contra o início |
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
  → Work Order válida
  → PartTypeList: uma correspondência
  → PartTypeData: PartTypeID carregado
  → produto e quantidade confirmados
  → SupplierBox confirmada
  → ZfBox InProgress = true
  → Sensor N: Loaded
  → próximo scan: Sensor N = UnloadedOk
  → Sensor N+1 = Loaded
  → opcional: último Loaded = UnloadedNok + IsScrap
  → quantidade de sensores bons atinge a meta
  → último sensor = UnloadedOk
  → Etiqueta final validada
  → ZfBox e sensores InProgress = false
```

Estados persistidos do sensor:

| `AccState` | Significado | Próxima ação permitida |
|---|---|---|
| `NotLoaded` | Estado reservado pelo modelo; ciclo ainda não iniciado | Não deve aparecer em produção normal |
| `Loaded` | ACC recebeu o componente e aguarda conclusão | `Unload OK` no próximo scan/finalização ou `Unload NOK` por scrap |
| `UnloadedOk` | Componente aceito no processo | Nenhuma nova chamada para o serial |
| `UnloadedNok` | Componente descarregado como NOK | Serial permanece bloqueado e auditado como scrap |

Estados persistidos da caixa:

| `InProgress` | `IsPaused` | Interpretação |
|---|---|---|
| `true` | `false` | Caixa ativa; retomada automática após reinício |
| `true` | `true` | Caixa interrompida; retomada manual |
| `false` | `false` | Caixa finalizada com HU e lote |

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

O processo usa o bit zero geral definido pela especificação: `statusBits=1/failureBits=0` para OK e `statusBits=0/failureBits=1` para NOK. Engenharia deve confirmar que a estação `20.1` não exige bits adicionais específicos do processo.

## 3. Falha local após sucesso no ACC

Se o `Load` concluir no ACC e a gravação local falhar, a aplicação tenta compensar imediatamente com `Unload NOK` e registra uma ocorrência crítica se a compensação também falhar. Se um `Unload` concluir e a atualização local falhar, o processo permanece bloqueado para intervenção da Manutenção, evitando o avanço silencioso.

## 4. Mais de uma caixa ativa

Na inicialização, se houver várias caixas não pausadas com `InProgress = true`, a aplicação registra um Warning e retoma somente a mais recente. As demais não são finalizadas automaticamente. Engenharia deve investigar a origem e reconciliar cada caixa antes de produção contínua.

## 5. Unicidade protegida pela aplicação

As validações de RE, ZF-ID, part numbers e serial são executadas pela aplicação; o modelo atual não declara índices únicos para todos esses campos no SQLite. Edição externa ou importação direta pode criar duplicidades que a interface normal impediria. Não utilize ferramentas de banco como caminho operacional.

---

# Referência de métodos

Esta seção é voltada a manutenção de software. Os nomes representam os pontos de entrada e regras mais importantes da versão atual.

## `Program.cs`

### `Program.Main()`

- inicializa o Windows Forms;
- cria o mutex global que impede duas instâncias;
- prepara log e caminho do banco;
- aplica migrations;
- executa o seed somente no primeiro banco;
- inicia a tela principal;
- registra falhas globais e encerramento.

### `ResolveDatabasePath(...)`

Resolve `C:\ProgramData\HondaSensorChecker\Database\HondaSensorChecker.db`. Se o novo arquivo não existir e o legado existir, aciona a migração física antes de abrir o contexto.

### `MoveLegacyDatabaseFiles(...)`

Move o banco legado e, se presentes, os arquivos `-wal` e `-shm`. Em falha parcial, tenta devolver os arquivos já movidos ao local original.

### `RegisterGlobalExceptionLogging()`

Registra exceções da thread de interface, exceções não tratadas do domínio e tasks não observadas no log técnico.

## `WorkOrderRules.cs`

### `TryNormalizeScannedLabel(...)`

Valida os formatos `OD`, `O11` e `O12`, converte para maiúsculas e remove somente a primeira letra `O`.

### `TryNormalizeForLookup(...)`

Permite às telas de consulta localizar uma ordem digitada com ou sem `O`, mantendo as mesmas regras de formato.

### `FormatStoredNumber(...)`

Recompõe o `O` para exibição de um número armazenado de forma normalizada.

## `Main.cs`

### `HSCMainForm_Load(...)`

Identifica o usuário do Windows, verifica seu cadastro, configura permissões administrativas e tenta retomar uma caixa ativa não pausada.

### `CleanForm()`

Limpa o contexto em memória e restaura o fluxo inicial. Não apaga caixas ou sensores persistidos.

### `txtWorkOrderNumber_Leave(...)`

Normaliza a Work Order e chama o carregamento do modelo ACC. Um NOK nessa etapa deve voltar para o campo da ordem, não para o sensor.

### `TryLoadAccPartTypeDataAsync(...)`

Valida a configuração, conecta ao ACC, executa `PartTypeList`, filtra a Description pela ordem, exige uma única correspondência e chama `PartTypeData` com o `PartTypeID` encontrado.

### `btnLogisticLabelOk_Click(...)`

Valida a SupplierBox e o produto travado, cria ou atualiza o contexto da caixa ZF e libera o scan. Não executa changeover nem troca o PartNumber ACC.

### `txtComponentSerial_KeyPress(...)`

Executa as validações locais, reserva estoque, conclui o sensor pendente com `Unload OK`, envia `Load` do novo sensor e persiste o estado `Loaded`. Um novo sensor só entra após a conclusão segura do anterior.

### `LoadSensorInAccAsync(...)`

Envia `Load` com estação, produto, versão, `PartTypeID` e serial. Retorna `CycleID` e `UnitPartTypeID` quando fornecidos pela DLL.

### `CompleteSensorInAccAsync(...)`

Envia `Unload OK` ou `Unload NOK`, atualiza `AccState`, horário, informações da resposta e, no NOK, a auditoria do scrap. Uma falha de persistência após sucesso externo bloqueia o processo.

### `TryCompensateUnpersistedLoadAsync(...)`

Se o ACC aceitou o `Load`, mas o sensor não pôde ser gravado, tenta `Unload NOK` compensatório. Falha nessa compensação é crítica.

### `ConfirmLastSensorAndFinalizeAsync()`

Exige confirmação física do último sensor. `SIM` conclui `Unload OK`; `NÃO` mantém o sensor `Loaded` para possível scrap.

### `btnRemoveSensor_Click(...)`

Aceita somente o primeiro item da lista quando ele é o sensor pendente. Solicita confirmação, executa `Unload NOK`, grava o scrap e reduz a contagem de sensores bons.

### `btnInterruptProcess_Click(...)`

Executa autorização por RE administrativo, duas confirmações e persiste `IsPaused = true` e a SupplierBox atual.

### `ResumePersistedProcessAsync(...)`

Recupera ordem, produto, caixa, sensores e estado ACC, refaz `PartTypeData` e restaura ou solicita a SupplierBox conforme o tipo de retomada.

### `TryResumeActiveProcessOnStartupAsync()`

Procura a caixa mais recente com `InProgress = true` e `IsPaused = false` e inicia a retomada automática.

### `RetryCurrentStageAfterNok()`

Reabre somente a etapa correspondente ao erro registrado em `_retryTarget`, impedindo saltos indevidos para o campo de sensor.

### `BlockProcessForMaintenance(...)`

Trava as ações de produção quando o estado externo do ACC pode ter divergido do banco local.

## `Windows/FinishedBox.cs`

### `PersistFinishedBox()`

Confirma que a quantidade de sensores bons é igual à meta e que todos os estados ACC estão resolvidos; grava HU e lote; finaliza caixa e sensores; debita todas as peças consumidas por SupplierBox; registra auditoria.

## Telas de consulta e administração

- `ComponentHistoryDialog.BuscarHistorico()`: consulta um serial e mostra rastreabilidade, operador e status.
- `WorkOrderFinishedBoxesDialog.SearchFinishedBoxes()`: lista HUs finalizadas da ordem.
- `WorkOrderFinishedBoxesDialog.OpenSelectedBox()`: abre os componentes da HU selecionada.
- `Logs.ApplyFilters()`: combina pesquisa, operador e período na tela de auditoria.
- `Products`: cria, edita e remove produtos respeitando validações e dependências.
- `Users`: cria, edita e remove usuários respeitando unicidade, permissão e dependências.

## Banco, migrations e repositórios

| Migration | Finalidade |
|---|---|
| `20260203231817_InitialCreate` | Estrutura inicial das entidades e relacionamentos |
| `20260818180000_AddPersistentProcessState` | Pausa persistente e SupplierBox corrente da caixa |
| `20260825120000_AddSensorAccLifecycleAndScrapAudit` | Estados do ciclo ACC e auditoria completa do scrap |

Os repositórios concentram operações de entidade. `UnitOfWorkRepository.Commit(...)` é o limite comum de persistência. O `DataContext` configura índices, relacionamentos e regras de exclusão restritiva.

---

# Testes de aceitação

Execute estes testes com etiquetas controladas e autorização do responsável pelo ACC. Preserve banco e logs do ciclo.

## 1. Inicialização

- [ ] Primeira execução cria pasta, banco, migrations, administrador inicial e produtos seed.
- [ ] Banco legado é movido com `-wal`/`-shm` quando aplicável.
- [ ] Banco já existente no caminho novo é preservado.
- [ ] Segunda instância é recusada.
- [ ] Usuário não cadastrado não entra no processo.
- [ ] Build Debug mostra `DEBUG`; Release não mostra.

## 2. Work Order e produto

- [ ] `O11` normal é aceita.
- [ ] `O12` rework é aceita.
- [ ] `OD` dummy é aceita.
- [ ] Formato incorreto é rejeitado.
- [ ] Zero correspondências no `PartTypeList` bloqueia o avanço.
- [ ] Mais de uma correspondência bloqueia o avanço.
- [ ] Correspondência única executa `PartTypeData` correto.
- [ ] Ordem existente recupera o produto.
- [ ] Ordem nova exige PartNumber final cadastrado.

## 3. SupplierBox

- [ ] Caixa existente recupera produto e saldo.
- [ ] Caixa nova exige `S...`, `P...` e `Q###` válidos.
- [ ] Produto diferente da Work Order é rejeitado.
- [ ] Troca manual mantém o mesmo produto e `PartTypeID`.
- [ ] Opções de zeragem e uso além do saldo são registradas.
- [ ] Quantidade exibida cai uma única unidade por sensor consumido.

## 4. Ciclo ACC e scrap

- [ ] Primeiro sensor recebe somente `Load`.
- [ ] Segundo sensor provoca `Unload OK` do primeiro antes do próprio `Load`.
- [ ] Falha no `Unload OK` impede o `Load` seguinte.
- [ ] Serial, prefixo ou produto inválido não consome estoque.
- [ ] Serial duplicado é rejeitado.
- [ ] Somente o primeiro item da lista permite scrap.
- [ ] Scrap executa `Unload NOK`, reduz o contador bom e não restaura saldo.
- [ ] Sensor scrapado não pode ser relido e identifica o operador.
- [ ] Último sensor exige confirmação física.
- [ ] Resposta `NÃO` mantém o último sensor disponível para scrap.
- [ ] Tela verde abre somente após todos os Unloads resolvidos.

## 5. Persistência e retomada

- [ ] Fechamento sem interrupção retoma automaticamente a caixa.
- [ ] Interrupção exige RE administrador e duas confirmações.
- [ ] Caixa pausada não retoma automaticamente.
- [ ] **CONTINUAR PROCESSO** recupera caixa, contador e sensores.
- [ ] `PartTypeData` é refeito na retomada.
- [ ] Sensor `Loaded` é recuperado sem repetir `Load`.
- [ ] Mais de um `Loaded` provoca bloqueio para Manutenção.

## 6. Finalização e consultas

- [ ] HU, PartNumber final, Work Order e lote são lidos na sequência.
- [ ] PartNumber ou Work Order divergente é rejeitado.
- [ ] Meta considera somente sensores bons.
- [ ] Débito da SupplierBox inclui bons e scraps, sem ficar negativo.
- [ ] Consulta de componente mostra operador do scan.
- [ ] Consulta de ordem mostra somente HUs finalizadas.
- [ ] Duplo clique e botão abrem os componentes da HU.
- [ ] Logs do banco e arquivo contêm o ciclo testado.

## 7. Falhas controladas

- [ ] Servidor ACC indisponível gera NOK e mantém o foco na etapa correta.
- [ ] Clique no painel vermelho retoma Work Order ou sensor conforme a origem.
- [ ] Falha local após `Load` tenta compensação com `Unload NOK`.
- [ ] Falha local após `Unload` bloqueia o processo para reconciliação.
- [ ] Reinício durante uma caixa não duplica contagem nem comando `Load`.

---

# Controle de mudanças

Ao liberar uma versão, atualize esta tabela e guarde o pacote publicado junto ao registro da validação.

| Data | Versão | Alteração | Responsável |
|---|---|---|---|
| 2026-08-25 | Atual | Manual consolidado com fluxo ACC adiado, scrap, persistência, consultas, logs e testes | Engenharia |

Informações que devem acompanhar cada release:

- hash ou tag do Git;
- versão do executável;
- versão física da `ZF.ACCComm.dll` e valor de `DllVersion` configurado;
- IP, porta, produto e Station do ambiente;
- migration mais recente aplicada;
- backup do banco anterior;
- resultado do checklist e responsável pelo teste operacional.

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
- [ ] `Load/Unload` validados com a estação `20.1`.
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
