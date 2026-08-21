# Manual prático de Git e GitHub

> Um guia técnico e ilustrativo para entender o que o Git está fazendo, e não apenas decorar comandos.

## Sumário

1. [Git não é GitHub](#1-git-não-é-github)
2. [O mapa mental do Git](#2-o-mapa-mental-do-git)
3. [Instalação e configuração](#3-instalação-e-configuração)
4. [Criando ou copiando um repositório](#4-criando-ou-copiando-um-repositório)
5. [O ciclo diário: status, add, commit](#5-o-ciclo-diário-status-add-commit)
6. [Consultando histórico e diferenças](#6-consultando-histórico-e-diferenças)
7. [Branches e HEAD](#7-branches-e-head)
8. [Checkout, switch e restore](#8-checkout-switch-e-restore)
9. [Stash: a gaveta temporária](#9-stash-a-gaveta-temporária)
10. [Merge: juntando linhas de trabalho](#10-merge-juntando-linhas-de-trabalho)
11. [Conflitos](#11-conflitos)
12. [Rebase: reorganizando a base](#12-rebase-reorganizando-a-base)
13. [Reset, revert e recuperação](#13-reset-revert-e-recuperação)
14. [Repositórios remotos](#14-repositórios-remotos)
15. [Fetch, pull e push](#15-fetch-pull-e-push)
16. [Tags e versões](#16-tags-e-versões)
17. [GitHub: forks, Pull Requests e Issues](#17-github-forks-pull-requests-e-issues)
18. [GitHub CLI (`gh`)](#18-github-cli-gh)
19. [Arquivos especiais](#19-arquivos-especiais)
20. [Ferramentas avançadas](#20-ferramentas-avançadas)
21. [Receitas práticas](#21-receitas-práticas)
22. [Diagnóstico e primeiros socorros](#22-diagnóstico-e-primeiros-socorros)
23. [Catálogo de comandos](#23-catálogo-de-comandos)
24. [Regras de segurança](#24-regras-de-segurança)

---

## 1. Git não é GitHub

**Git** é o sistema que registra o histórico dos arquivos. Ele funciona localmente, mesmo sem internet.

**GitHub** é um serviço que hospeda repositórios Git e adiciona colaboração: Pull Requests, Issues, Actions, permissões, revisão de código e outras ferramentas.

Uma analogia:

```text
Git     = o sistema de histórico instalado no computador
GitHub  = o local na internet onde o histórico é compartilhado
gh      = programa de terminal para conversar com o GitHub
```

Portanto, `git commit` é um comando do Git. Já `gh pr create` é um comando da GitHub CLI.

---

## 2. O mapa mental do Git

### 2.1 As quatro áreas principais

```text
Pasta de trabalho      Staging Area       Repositório local       Remoto
(arquivos editáveis)   (próximo commit)   (commits no .git)       (GitHub)
        │                    │                    │                   │
        └── git add ────────>│                    │                   │
                             └── git commit ─────>│                   │
                                                  └── git push ─────>│
        <──────────────────────── git checkout/restore ──────────────┘
```

#### Pasta de trabalho — working tree

É o projeto que você abre no editor. Alterações feitas aqui ainda não fazem parte de um commit.

#### Staging area — index

É a seleção do que entrará no próximo commit. `git add` não envia nada ao GitHub; apenas prepara conteúdo.

#### Repositório local

Fica, em geral, dentro da pasta oculta `.git`. Contém commits, branches, tags e outras referências.

#### Repositório remoto

É outra cópia do repositório, normalmente no GitHub. `origin` é apenas o apelido convencional desse endereço.

### 2.2 O que é um commit?

Um commit é uma fotografia versionada dos arquivos rastreados, acompanhada de:

- identificador (hash);
- autor e data;
- mensagem;
- referência ao commit anterior, ou aos anteriores no caso de merge.

```text
A ← B ← C ← D
```

Cada letra representa um commit. O histórico é uma cadeia — tecnicamente, um grafo — de fotografias relacionadas.

### 2.3 Estados de um arquivo

```text
untracked  → arquivo novo que o Git ainda não acompanha
modified   → arquivo rastreado que foi alterado
staged     → conteúdo selecionado para o próximo commit
committed  → conteúdo registrado no histórico local
ignored    → arquivo que uma regra do .gitignore manda ignorar
```

---

## 3. Instalação e configuração

Ver a versão instalada:

```powershell
git --version
```

Configurar nome e e-mail usados nos commits:

```powershell
git config --global user.name "Seu Nome"
git config --global user.email "seu.email@exemplo.com"
```

Ver configurações e suas origens:

```powershell
git config --list --show-origin
```

Definir `main` como nome padrão de novas branches principais:

```powershell
git config --global init.defaultBranch main
```

Configurar o editor padrão, por exemplo o VS Code:

```powershell
git config --global core.editor "code --wait"
```

Existem três níveis comuns de configuração:

```text
--system  → todos os usuários do computador
--global  → seu usuário
--local   → somente o repositório atual
```

Uma configuração local prevalece sobre a global.

Ajuda oficial para qualquer comando:

```powershell
git help commit
git commit --help
git commit -h
```

---

## 4. Criando ou copiando um repositório

### 4.1 Começar a controlar uma pasta existente

```powershell
git init
```

Isso cria a estrutura `.git`. Não cria um repositório no GitHub e não envia arquivos.

Fluxo inicial típico:

```powershell
git init
git add .
git commit -m "Commit inicial"
```

### 4.2 Copiar um repositório existente

```powershell
git clone https://github.com/usuario/projeto.git
```

O clone normalmente cria:

- a pasta do projeto;
- o histórico local;
- a referência remota chamada `origin`;
- uma branch local acompanhando a branch principal remota.

Escolher nome da pasta:

```powershell
git clone https://github.com/usuario/projeto.git MinhaPasta
```

Copiar apenas a parte mais recente do histórico:

```powershell
git clone --depth 1 https://github.com/usuario/projeto.git
```

---

## 5. O ciclo diário: status, add, commit

### 5.1 Consultar antes de agir

```powershell
git status
git status --short --branch
```

`git status` é o painel do carro: informa branch, arquivos alterados, stage e relação com o remoto.

### 5.2 Preparar arquivos

Um arquivo específico:

```powershell
git add Main.cs
```

Todos os arquivos novos, modificados e removidos dentro da pasta atual:

```powershell
git add .
```

Todas as alterações do repositório:

```powershell
git add -A
```

Escolher trechos interativamente:

```powershell
git add -p
```

`git add -p` permite montar um commit limpo mesmo quando um arquivo contém mudanças de assuntos diferentes.

### 5.3 Retirar algo do stage sem apagar a alteração

```powershell
git restore --staged Main.cs
```

O arquivo volta a aparecer como modificado, mas seu conteúdo permanece.

### 5.4 Criar um commit

```powershell
git commit -m "Corrige validação do sensor"
```

Abrir o editor para escrever uma mensagem mais completa:

```powershell
git commit
```

Adicionar automaticamente arquivos já rastreados e criar o commit:

```powershell
git commit -am "Atualiza mensagens de erro"
```

O `-a` não inclui arquivos novos ainda não rastreados.

### 5.5 Corrigir o último commit

Alterar apenas a mensagem:

```powershell
git commit --amend -m "Nova mensagem"
```

Adicionar um arquivo esquecido:

```powershell
git add ArquivoEsquecido.cs
git commit --amend --no-edit
```

`--amend` substitui o commit por outro hash. Evite usá-lo em commits já compartilhados, salvo quando a equipe concordar com reescrita.

### 5.6 Remover e renomear

Remover arquivo da pasta e preparar a remoção:

```powershell
git rm Arquivo.cs
```

Parar de rastrear sem apagar do computador:

```powershell
git rm --cached appsettings.local.json
```

Renomear ou mover:

```powershell
git mv NomeAntigo.cs NomeNovo.cs
```

---

## 6. Consultando histórico e diferenças

### 6.1 Histórico

```powershell
git log
git log --oneline
git log --oneline --graph --decorate --all
```

Uma visualização compacta:

```text
* 6ad39b9 (HEAD -> main) Checkpoint
* 940187f beta3.0
* 437737f Primeiros Testes com ACC
```

Histórico de um arquivo:

```powershell
git log -- Main.cs
git log -p -- Main.cs
```

Pesquisar mensagens:

```powershell
git log --grep="sensor"
```

Pesquisar commits que adicionaram ou removeram determinado texto:

```powershell
git log -S "NomeDaFuncao" --oneline
```

### 6.2 Examinar um commit

```powershell
git show 4d73f24
git show --stat 4d73f24
git show 4d73f24:Main.cs
```

O último comando mostra `Main.cs` como ele existia naquele commit, sem trocar sua versão atual.

### 6.3 Diferenças

Alterações ainda fora do stage:

```powershell
git diff
```

Alterações que já estão no stage:

```powershell
git diff --staged
```

Diferença entre dois commits:

```powershell
git diff 4d73f24 6ad39b9
```

Somente resumo:

```powershell
git diff --stat 4d73f24 6ad39b9
```

Somente nomes:

```powershell
git diff --name-status 4d73f24 6ad39b9
```

### 6.4 Quem alterou cada linha?

```powershell
git blame Main.cs
```

`blame` serve para encontrar contexto histórico, não para procurar culpados.

---

## 7. Branches e HEAD

Uma branch é uma etiqueta móvel que aponta para um commit:

```text
A ← B ← C
        ↑
       main
        ↑
       HEAD
```

`HEAD` indica onde você está. Normalmente ele aponta para a branch atual.

### 7.1 Listar branches

```powershell
git branch
git branch --all
git branch --verbose --verbose
```

O asterisco marca a branch atual.

### 7.2 Criar e trocar

Criar sem trocar:

```powershell
git branch corrigir-leitura
```

Criar e trocar:

```powershell
git switch -c corrigir-leitura
```

Trocar para uma existente:

```powershell
git switch main
```

Voltar à branch anterior:

```powershell
git switch -
```

### 7.3 Renomear e apagar

Renomear a branch atual:

```powershell
git branch -m novo-nome
```

Apagar branch local já integrada:

```powershell
git branch -d corrigir-leitura
```

Forçar a exclusão local:

```powershell
git branch -D corrigir-leitura
```

`-D` pode remover a última referência fácil a commits ainda não integrados.

### 7.4 Detached HEAD

```powershell
git switch --detach 4d73f24
```

Resultado:

```text
HEAD → 4d73f24
main → 6ad39b9
```

Você está visitando uma fotografia antiga diretamente. A `main` continua apontando para a versão recente.

Para voltar:

```powershell
git switch main
```

Se criar trabalho importante em detached HEAD, preserve-o em uma branch:

```powershell
git switch -c experimento-antigo
```

---

## 8. Checkout, switch e restore

`checkout` é um comando antigo com duas responsabilidades: trocar branches/commits e restaurar arquivos. Os comandos modernos separam essas ações:

```text
git switch   → troca branch ou commit
git restore  → restaura conteúdo de arquivo
git checkout → pode fazer ambos; ainda é válido
```

### 8.1 Restaurar alteração não preparada

```powershell
git restore Main.cs
```

Isso substitui o arquivo pela versão do index e descarta a alteração local. Confira antes com `git diff`.

### 8.2 Restaurar um arquivo a partir de outro commit

```powershell
git restore --source=4d73f24 Main.cs
```

O conteúdo antigo aparece como uma alteração atual. Depois você pode examiná-lo e criar um novo commit.

### 8.3 Restaurar todo o projeto a partir de um commit

```powershell
git restore --source=4d73f24 --staged --worktree .
```

Isso não move a branch. Prepara uma nova versão cujo conteúdo rastreado equivale ao commit antigo. Deve ser usado conscientemente e, em geral, seguido de revisão e commit.

---

## 9. Stash: a gaveta temporária

Guardar alterações rastreadas:

```powershell
git stash push -m "Trabalho em andamento"
```

Incluir arquivos novos não rastreados:

```powershell
git stash push -u -m "Trabalho em andamento"
```

Listar:

```powershell
git stash list
```

Examinar:

```powershell
git stash show -p "stash@{0}"
```

Aplicar mantendo a cópia guardada:

```powershell
git stash apply "stash@{0}"
```

Aplicar e remover da lista se der certo:

```powershell
git stash pop
```

Criar uma branch diretamente a partir do contexto do stash:

```powershell
git stash branch recuperar-trabalho "stash@{0}"
```

Remover um stash:

```powershell
git stash drop "stash@{0}"
```

Remover todos:

```powershell
git stash clear
```

`clear` é destrutivo. Inspecione a lista antes.

---

## 10. Merge: juntando linhas de trabalho

Imagine duas linhas:

```text
          D ← E  feature
         /
A ← B ← C       main
```

Na `main`:

```powershell
git switch main
git merge feature
```

O resultado pode ser um merge commit:

```text
          D ← E
         /     \
A ← B ← C ←──── M  main
```

### 10.1 Fast-forward

Se a `main` não avançou depois da separação, o Git apenas move sua etiqueta:

```text
Antes:  A ← B ← C ← D ← E
              ↑         ↑
             main     feature

Depois: A ← B ← C ← D ← E
                            ↑
                      main, feature
```

### 10.2 Exigir um merge commit

```powershell
git merge --no-ff feature
```

### 10.3 Interromper um merge

```powershell
git merge --abort
```

### 10.4 Apagar a branch depois da integração

```powershell
git branch -d feature
```

Apagar a branch remota:

```powershell
git push origin --delete feature
```

---

## 11. Conflitos

Um conflito ocorre quando o Git não consegue decidir sozinho como combinar mudanças.

Exemplo no arquivo:

```text
<<<<<<< HEAD
versão da branch atual
=======
versão da outra branch
>>>>>>> feature
```

Procedimento:

1. Abra o arquivo.
2. Decida qual conteúdo deve permanecer.
3. Remova os marcadores.
4. Teste o projeto.
5. Marque o arquivo como resolvido.
6. Conclua a operação.

```powershell
git status
git add ArquivoComConflito.cs
git commit
```

Durante rebase, a conclusão usa:

```powershell
git rebase --continue
```

Para cancelar:

```powershell
git merge --abort
git rebase --abort
git cherry-pick --abort
git revert --abort
```

---

## 12. Rebase: reorganizando a base

Antes:

```text
          D ← E  feature
         /
A ← B ← C ← F ← G  main
```

Na `feature`:

```powershell
git switch feature
git rebase main
```

Depois:

```text
A ← B ← C ← F ← G  main
                  \
                   D' ← E'  feature
```

O conteúdo de `D` e `E` é reaplicado sobre `G`, gerando novos commits `D'` e `E'`, com novos hashes.

Regra prática:

> Não faça rebase de commits públicos que outras pessoas já usam, salvo acordo explícito da equipe.

### 12.1 Rebase interativo

```powershell
git rebase -i HEAD~4
```

Permite reorganizar commits recentes:

```text
pick   → manter
reword → alterar mensagem
edit   → pausar para alterar
squash → combinar com o anterior e editar mensagem
fixup  → combinar descartando a mensagem atual
drop   → remover
```

Cancelar:

```powershell
git rebase --abort
```

Continuar depois de resolver conflitos:

```powershell
git add .
git rebase --continue
```

---

## 13. Reset, revert e recuperação

### 13.1 Reset

`reset` move uma referência, normalmente a branch atual.

```powershell
git reset --soft HEAD~1
```

Move a branch um commit para trás e mantém as mudanças no stage.

```powershell
git reset --mixed HEAD~1
```

Mantém as mudanças na pasta, fora do stage. `--mixed` é o padrão.

```powershell
git reset --hard HEAD~1
```

Move a branch e força a pasta/index a corresponderem ao destino. Alterações locais podem ser perdidas.

Ilustração:

```text
Antes: A ← B ← C ← D
                  ↑
                 main

git reset --hard B

Depois: A ← B
             ↑
            main
```

### 13.2 Revert

```powershell
git revert D
```

Cria um novo commit que aplica o inverso de `D`:

```text
A ← B ← C ← D ← E
                ↑
            E desfaz D
```

O histórico é preservado. É a opção usual para desfazer algo já compartilhado.

Reverter sem criar o commit imediatamente:

```powershell
git revert --no-commit D
```

Reverter um merge exige escolher o pai principal:

```powershell
git revert -m 1 HASH_DO_MERGE
```

### 13.3 Recuperar com reflog

O reflog registra movimentos locais de referências:

```powershell
git reflog
```

Se um reset levou a branch ao lugar errado, localize o hash anterior e crie uma branch de resgate:

```powershell
git branch resgate HASH_ENCONTRADO
```

Ou retorne a branch, depois de verificar cuidadosamente:

```powershell
git reset --hard HASH_ENCONTRADO
```

O reflog é uma rede de segurança local, não um substituto para commits e backups remotos.

---

## 14. Repositórios remotos

Listar remotos:

```powershell
git remote -v
```

Adicionar:

```powershell
git remote add origin https://github.com/usuario/projeto.git
```

Ver detalhes:

```powershell
git remote show origin
```

Alterar URL:

```powershell
git remote set-url origin https://github.com/usuario/novo-projeto.git
```

Renomear:

```powershell
git remote rename origin github
```

Remover apenas a referência local ao remoto:

```powershell
git remote remove origin
```

Isso não apaga o repositório hospedado.

### 14.1 Branch local, remota e remote-tracking

```text
main          → sua branch local editável
origin/main   → sua última informação local sobre a main do origin
main no GitHub→ branch real armazenada no servidor
```

`origin/main` não consulta a internet em tempo real. Ela é atualizada por `fetch` ou `pull`.

---

## 15. Fetch, pull e push

### 15.1 Fetch

```powershell
git fetch origin
```

Baixa informações e objetos, atualizando referências como `origin/main`, mas não mistura nada na sua branch.

É como buscar as cartas na caixa de correio sem aplicá-las ao projeto.

Remover referências locais de branches remotas que já foram apagadas:

```powershell
git fetch --prune
```

### 15.2 Pull

```powershell
git pull
```

Em geral, equivale a:

```text
git fetch + integração na branch atual
```

Integração por merge:

```powershell
git pull --no-rebase
```

Integração por rebase:

```powershell
git pull --rebase
```

Permitir somente fast-forward:

```powershell
git pull --ff-only
```

`--ff-only` é conservador: falha em vez de criar uma integração inesperada.

### 15.3 Push

```powershell
git push origin main
```

No primeiro envio de uma nova branch:

```powershell
git push -u origin minha-branch
```

`-u` estabelece acompanhamento. Depois, normalmente basta `git push` e `git pull`.

### 15.4 Force push

Depois de reescrever commits já enviados, um push normal é recusado. Se a reescrita for intencional:

```powershell
git push --force-with-lease
```

Prefira `--force-with-lease` a `--force`: ele verifica se o remoto ainda está no estado esperado e reduz o risco de sobrescrever trabalho alheio.

---

## 16. Tags e versões

Tags são etiquetas geralmente fixas, úteis para marcar lançamentos.

Listar:

```powershell
git tag
```

Criar tag anotada:

```powershell
git tag -a v1.0.0 -m "Versão 1.0.0"
```

Marcar um commit específico:

```powershell
git tag -a v0.9.0 4d73f24 -m "Versão 0.9.0"
```

Enviar uma tag:

```powershell
git push origin v1.0.0
```

Enviar todas as tags locais ainda ausentes:

```powershell
git push origin --tags
```

Apagar localmente:

```powershell
git tag -d v1.0.0
```

Apagar no remoto:

```powershell
git push origin --delete v1.0.0
```

---

## 17. GitHub: forks, Pull Requests e Issues

### 17.1 Fork

Um fork é uma cópia de um repositório na sua conta do GitHub. É comum quando você não pode criar branches diretamente no projeto original.

Convenção frequente:

```text
origin   → seu fork
upstream → repositório original
```

```powershell
git remote add upstream https://github.com/organizacao/projeto.git
git fetch upstream
```

### 17.2 Pull Request

Um Pull Request não é um comando Git; é uma proposta no GitHub para integrar uma branch em outra.

Fluxo:

```text
criar branch → alterar → commit → push → abrir PR → revisar → integrar
```

Exemplo:

```powershell
git switch -c corrigir-dialogo
git add .
git commit -m "Corrige abertura do diálogo"
git push -u origin corrigir-dialogo
```

Depois, abra o Pull Request no GitHub ou com `gh pr create`.

### 17.3 Issue

Uma Issue registra trabalho, bug, discussão ou solicitação. Ela não altera o repositório por si só.

Mensagens como estas podem vincular ou fechar uma Issue quando o PR for integrado:

```text
Fixes #42
Closes #42
Resolves #42
```

### 17.4 Formas de integrar um PR

```text
Merge commit      → preserva os commits e cria um commit de junção
Squash and merge  → combina o PR em um único commit
Rebase and merge  → reaplica os commits sobre a ponta da base
```

Cada equipe deve estabelecer uma convenção.

---

## 18. GitHub CLI (`gh`)

A GitHub CLI controla recursos do GitHub pelo terminal. Ela não substitui o Git.

Autenticar:

```powershell
gh auth login
gh auth status
```

### 18.1 Repositórios

```powershell
gh repo view
gh repo clone usuario/projeto
gh repo create
gh repo fork
```

Abrir o repositório no navegador:

```powershell
gh repo view --web
```

### 18.2 Pull Requests

```powershell
gh pr list
gh pr status
gh pr view 123
gh pr create
gh pr checkout 123
gh pr diff 123
gh pr checks 123
```

Exemplos:

```powershell
gh pr create --base main --head corrigir-dialogo --fill
gh pr view 123 --web
```

Integrar exige autorização e deve respeitar o processo da equipe:

```powershell
gh pr merge 123 --merge
gh pr merge 123 --squash
gh pr merge 123 --rebase
```

Fechar sem integrar:

```powershell
gh pr close 123
```

### 18.3 Issues

```powershell
gh issue list
gh issue view 42
gh issue create
gh issue close 42
gh issue reopen 42
```

### 18.4 Actions e execuções

```powershell
gh workflow list
gh workflow view
gh workflow run NOME_DO_WORKFLOW
gh run list
gh run view ID
gh run watch ID
```

### 18.5 Releases

```powershell
gh release list
gh release view v1.0.0
gh release create v1.0.0
gh release download v1.0.0
```

### 18.6 Ajuda e descoberta

```powershell
gh help
gh pr --help
gh pr create --help
```

A GitHub CLI recebe novos recursos. A ajuda instalada é a referência exata para a versão disponível no computador.

---

## 19. Arquivos especiais

### 19.1 `.gitignore`

Define arquivos que não devem ser rastreados, como builds, caches e configurações pessoais.

Exemplo:

```gitignore
bin/
obj/
.vs/
*.user
appsettings.local.json
```

O `.gitignore` não remove arquivos que já foram commitados. Para parar de rastreá-los:

```powershell
git rm --cached -r .vs
git commit -m "Remove arquivos locais do Visual Studio do controle de versão"
```

Ver qual regra está ignorando um arquivo:

```powershell
git check-ignore -v caminho/do/arquivo
```

### 19.2 `.gitattributes`

Controla atributos como finais de linha, tratamento binário e drivers de diff/merge.

Exemplo:

```gitattributes
* text=auto
*.cs text eol=crlf
*.sh text eol=lf
*.png binary
```

### 19.3 `.gitkeep`

Não é um recurso especial do Git. É apenas uma convenção: um arquivo vazio usado para permitir que uma pasta sem outros arquivos apareça no repositório.

### 19.4 Submódulos

Um submódulo aponta para um commit de outro repositório:

```powershell
git submodule add URL pasta
git submodule update --init --recursive
```

Clone incluindo submódulos:

```powershell
git clone --recurse-submodules URL
```

---

## 20. Ferramentas avançadas

### 20.1 Cherry-pick

Aplica o conteúdo de um commit específico na branch atual:

```powershell
git cherry-pick HASH
```

É como retirar uma página de uma linha do tempo e reaplicá-la em outra. O novo commit terá outro hash.

Faixa de commits:

```powershell
git cherry-pick A^..B
```

Cancelar:

```powershell
git cherry-pick --abort
```

### 20.2 Bisect

Encontra, por busca binária, o commit que introduziu um problema:

```powershell
git bisect start
git bisect bad
git bisect good 4d73f24
```

O Git seleciona commits intermediários. Para cada um, teste e informe:

```powershell
git bisect good
git bisect bad
```

Ao terminar:

```powershell
git bisect reset
```

Também pode automatizar com um comando de testes:

```powershell
git bisect run dotnet test
```

### 20.3 Worktree

Permite ter mais de uma versão do repositório aberta em pastas separadas:

```powershell
git worktree add ..\Honda-antigo 4d73f24
```

Assim, a pasta atual pode continuar na `main`, enquanto outra pasta mostra o commit antigo.

Listar:

```powershell
git worktree list
```

Remover uma worktree quando não for mais necessária:

```powershell
git worktree remove ..\Honda-antigo
```

### 20.4 Clean

Ver o que seria apagado:

```powershell
git clean -nd
```

Apagar arquivos não rastreados:

```powershell
git clean -fd
```

`clean` pode apagar arquivos que nunca foram commitados. Sempre faça a simulação com `-n` primeiro.

### 20.5 Garbage collection e integridade

```powershell
git maintenance run
git gc
git fsck
```

São comandos de manutenção e diagnóstico; raramente são necessários no fluxo diário.

### 20.6 Arquivar uma versão

```powershell
git archive --format=zip --output=versao.zip 4d73f24
```

Cria um pacote dos arquivos rastreados naquele commit, sem incluir a pasta `.git`.

---

## 21. Receitas práticas

### 21.1 Restaurar temporariamente uma versão antiga sem perder o trabalho atual

Este é o caso concreto do projeto Honda Sensor Checker:

```text
Versão atual da main: 6ad39b9
Versão a ser testada: 4d73f24
Objetivo:              compilar 4d73f24 e depois continuar na versão atual
Condição:              existem alterações atuais ainda sem commit
```

Aqui, a palavra “restaurar” não significa apagar os commits posteriores nem transformar `4d73f24` na nova ponta da `main`. Significa somente montar temporariamente a fotografia antiga para executar e testar.

Não use neste cenário:

```powershell
git reset --hard 4d73f24
```

Esse comando moveria a `main` e poderia descartar alterações locais. Existem duas estratégias seguras.

#### Estratégia A — segunda pasta com `git worktree` (recomendada)

Esta é a opção mais confortável porque a pasta atual não muda. Não é necessário esconder suas modificações com stash.

Antes:

```text
Honda-Sensor-Checker\
│
├── HEAD → main → 6ad39b9
├── Main.cs com melhorias ainda sem commit
└── demais arquivos atuais
```

Crie uma segunda pasta ligada ao mesmo repositório:

```powershell
git worktree add --detach ..\Honda-Sensor-Checker-4d73f24 4d73f24
```

Depois:

```text
Honda-Sensor-Checker\                    Honda-Sensor-Checker-4d73f24\
├── main → 6ad39b9                       ├── HEAD → 4d73f24
├── suas alterações continuam aqui      ├── fotografia antiga limpa
└── seu trabalho atual                   └── pasta separada para compilar

       bancada atual                              bancada antiga
       não foi tocada                             criada pelo Git
```

O depósito de commits `.git` é compartilhado, mas cada worktree possui:

- sua própria pasta de arquivos;
- seu próprio `HEAD`;
- seu próprio stage;
- seus próprios resultados de compilação na respectiva pasta.

Entre na segunda pasta e compile:

```powershell
Set-Location ..\Honda-Sensor-Checker-4d73f24
dotnet clean HondaSensorChecker.csproj
dotnet build HondaSensorChecker.csproj --configuration Debug
```

Você pode abrir essa segunda solução no Visual Studio. Para não confundir as janelas, observe sempre o caminho completo exibido pelo editor.

Ao terminar, feche o Visual Studio e qualquer processo que esteja usando a versão antiga. Volte à pasta principal:

```powershell
Set-Location ..\Honda-Sensor-Checker
git worktree list
git worktree remove ..\Honda-Sensor-Checker-4d73f24
```

O que foi removido é somente a bancada adicional. A `main`, suas alterações atuais e os commits continuam na pasta original.

Se você tiver criado arquivos importantes e não commitados dentro da worktree antiga, o Git normalmente recusará a remoção. Inspecione-os e preserve-os; não use `--force` automaticamente.

Analogia: em vez de tirar todos os papéis da sua mesa atual, você pede outra mesa, coloca nela a edição antiga do projeto e trabalha nas duas sem misturar os documentos.

#### Estratégia B — guardar na gaveta e trocar a mesma pasta

Use esta estratégia quando você quer trabalhar na mesma pasta ou não deseja manter uma segunda cópia dos arquivos.

##### Passo 1 — identificar tudo que precisa ser protegido

```powershell
git status --short --branch
git diff
git diff --staged
```

Imagine este estado:

```text
HEAD → main → 6ad39b9

Pasta:
├── Main.cs modificado
├── Program.cs modificado
└── teste-manual.txt novo e ainda não rastreado

Stage:
└── parte da modificação de Main.cs já preparada
```

##### Passo 2 — guardar a bancada no stash

```powershell
git stash push -u -m "Trabalho atual antes de testar 4d73f24"
```

Por que usar `-u`?

```text
sem -u → guarda arquivos rastreados modificados, mas deixa arquivos novos
com -u → guarda também teste-manual.txt, que ainda é untracked
com -a → guardaria até arquivos ignorados; normalmente é desnecessário
```

Depois do stash:

```text
HEAD → main → 6ad39b9

Pasta: limpa, igual a 6ad39b9
Stage: limpo

Gaveta stash@{0}:
├── alterações de Main.cs
├── alterações de Program.cs
├── teste-manual.txt
└── informação sobre o conteúdo que estava no stage
```

Confirme que a gaveta existe e que a bancada ficou limpa:

```powershell
git stash list
git stash show --stat "stash@{0}"
git status
```

Não continue se `status` ainda mostrar alguma alteração importante que deveria ter sido guardada.

##### Passo 3 — visitar diretamente o commit antigo

```powershell
git switch --detach 4d73f24
```

Agora:

```text
HEAD → 4d73f24                   main → 6ad39b9
       você está aqui                   continua aqui

stash@{0} → suas alterações atuais permanecem guardadas
```

A `main` não voltou no tempo. Somente o `HEAD` saiu temporariamente da branch e apontou para a fotografia antiga. É exatamente isso que “detached HEAD” significa neste cenário.

Verifique:

```powershell
git status
git log -1 --oneline --decorate
```

O log deve mostrar `4d73f24`, e o status deve informar detached HEAD.

##### Passo 4 — limpar resultados de build e recompilar

```powershell
dotnet clean HondaSensorChecker.csproj
dotnet build HondaSensorChecker.csproj --configuration Debug
```

O `clean` evita executar por engano DLLs que tenham sobrado da versão recente. Em seguida, teste a aplicação antiga.

Evite fazer commits durante essa visita. Se descobrir uma correção que realmente precisa preservar a partir da versão antiga, primeiro crie uma branch:

```powershell
git switch -c correcao-baseada-em-4d73f24
```

##### Passo 5 — conferir o que o teste produziu

Antes de voltar, feche a aplicação e o Visual Studio e execute:

```powershell
git status --short
```

Arquivos de build normalmente são ignorados. Se algum arquivo rastreado tiver sido modificado pelo programa antigo, o Git pode impedir a troca para proteger o conteúdo. Não use `reset --hard` por reflexo: examine a mudança e decida se ela é descartável ou precisa ser guardada separadamente.

##### Passo 6 — voltar para a `main` recente

```powershell
git switch main
```

O estado será:

```text
HEAD → main → 6ad39b9

Pasta: versão recente limpa
Gaveta stash@{0}: seu trabalho ainda está protegido
```

Nesse instante, os commits recentes já voltaram, mas suas modificações não commitadas ainda estão na gaveta.

##### Passo 7 — restaurar as alterações com segurança

Em vez de começar com `stash pop`, use `apply`: ele aplica o conteúdo e conserva a gaveta enquanto você confere o resultado.

```powershell
git stash apply --index "stash@{0}"
git status
git diff
git diff --staged
```

O `--index` pede ao Git que também tente reconstruir quais mudanças estavam no stage. Sem ele, o conteúdo volta, mas a separação entre preparado e não preparado pode não ser restaurada como antes.

Depois:

```text
HEAD → main → 6ad39b9

Pasta:
├── Main.cs novamente modificado
├── Program.cs novamente modificado
└── teste-manual.txt restaurado

Stage:
└── tenta recuperar a preparação anterior

Gaveta stash@{0}:
└── ainda existe como cópia de segurança
```

Compile e confira seu trabalho atual. Somente quando tiver certeza de que tudo foi restaurado:

```powershell
git stash drop "stash@{0}"
```

`git stash pop` seria um atalho para aplicar e, se der certo, remover. Separar `apply` e `drop` é mais didático e conservador.

##### E se ocorrer conflito ao aplicar o stash?

Um conflito significa que o Git encontrou duas propostas incompatíveis para as mesmas linhas. O stash não é apagado por `apply`.

```powershell
git status
```

Resolva os marcadores nos arquivos, prepare o que foi resolvido com `git add` e valide o projeto. Mantenha o stash até confirmar a recuperação.

##### Resumo visual da estratégia com stash

```text
1. INÍCIO
   main/6ad39b9 + alterações na bancada

2. STASH
   main/6ad39b9 limpo + alterações na gaveta

3. SWITCH --DETACH
   HEAD/4d73f24 limpo + main preservada + alterações na gaveta

4. TESTE
   compila e executa a fotografia antiga

5. SWITCH MAIN
   HEAD/main/6ad39b9 limpo + alterações na gaveta

6. STASH APPLY --INDEX
   HEAD/main/6ad39b9 + alterações novamente na bancada/stage

7. STASH DROP
   remove a cópia da gaveta somente depois da conferência
```

#### Qual das duas estratégias escolher?

| Situação | Melhor escolha |
|---|---|
| Quero manter a versão atual aberta e compilar a antiga ao lado | `git worktree` |
| Tenho espaço para uma segunda pasta | `git worktree` |
| Preciso usar exatamente a mesma pasta | `stash` + `switch --detach` |
| Quero apenas ler um arquivo antigo | `git show COMMIT:arquivo` |
| Quero trazer um único arquivo antigo para o trabalho atual | `git restore --source=COMMIT arquivo` |
| Quero apagar commits recentes e mover a `main` | `reset` — não é este caso |
| Quero desfazer publicamente um erro preservando o histórico | `revert` — não é este caso |

Para este projeto, a estratégia com worktree é a mais isolada. A estratégia com stash também é segura quando todas as verificações acima são seguidas.

### 21.2 Criar uma melhoria isolada

```powershell
git switch main
git pull --ff-only
git switch -c melhorar-validacao

# editar e testar

git add -p
git diff --staged
git commit -m "Melhora validação dos dados do sensor"
git push -u origin melhorar-validacao
```

### 21.3 Desfazer o último commit local sem perder arquivos

Manter tudo preparado:

```powershell
git reset --soft HEAD~1
```

Manter tudo fora do stage:

```powershell
git reset HEAD~1
```

### 21.4 Desfazer um commit já publicado

```powershell
git switch main
git pull --ff-only
git revert HASH_DO_COMMIT
git push
```

### 21.5 Recuperar um arquivo antigo sem viajar no histórico

Somente visualizar:

```powershell
git show 4d73f24:Main.cs
```

Trazer a versão antiga como alteração atual:

```powershell
git restore --source=4d73f24 Main.cs
git diff -- Main.cs
```

### 21.6 Atualizar uma feature com a `main`

Com merge:

```powershell
git fetch origin
git switch minha-feature
git merge origin/main
```

Com rebase, se os commits ainda forem privados:

```powershell
git fetch origin
git switch minha-feature
git rebase origin/main
```

### 21.7 Publicar um repositório local novo

Depois de criar o repositório vazio no GitHub:

```powershell
git remote add origin URL_DO_REPOSITORIO
git branch -M main
git push -u origin main
```

---

## 22. Diagnóstico e primeiros socorros

### “Não sei o que está acontecendo”

Comece com:

```powershell
git status
git log --oneline --graph --decorate --all -20
git remote -v
```

### “Meu push foi rejeitado”

Provavelmente o remoto possui commits que sua branch local não tem:

```powershell
git fetch origin
git log --oneline --graph --decorate --all -20
```

Depois escolha conscientemente merge ou rebase. Não use force automaticamente.

### “Fiz um commit na branch errada”

Preserve-o primeiro:

```powershell
git branch branch-correta
```

Depois decida como a branch original deve voltar. Se o commit ainda não foi compartilhado:

```powershell
git reset --hard HEAD~1
git switch branch-correta
```

### “Apaguei um commit com reset”

```powershell
git reflog
git branch resgate HASH_ANTIGO
```

### “Quero cancelar a operação atual”

Leia primeiro:

```powershell
git status
```

Conforme a operação:

```powershell
git merge --abort
git rebase --abort
git cherry-pick --abort
git revert --abort
git bisect reset
```

### “Quero saber se um commit está em alguma branch”

```powershell
git branch --contains HASH
git branch -r --contains HASH
```

### “Quero localizar um commit perdido”

```powershell
git reflog --all
git fsck --no-reflogs --unreachable
```

Use `fsck` como diagnóstico avançado; os resultados exigem inspeção cuidadosa.

---

## 23. Catálogo de comandos

O Git possui comandos de uso diário, comandos especializados e comandos internos de baixo nível. Este catálogo organiza os principais por intenção.

| Intenção | Comandos principais |
|---|---|
| Criar/copiar | `init`, `clone` |
| Estado e preparação | `status`, `add`, `restore`, `rm`, `mv` |
| Registrar | `commit`, `notes` |
| Histórico e comparação | `log`, `show`, `diff`, `shortlog`, `describe`, `blame` |
| Branches e referências | `branch`, `switch`, `checkout`, `tag` |
| Integrar | `merge`, `rebase`, `cherry-pick`, `revert` |
| Trabalho temporário | `stash`, `worktree` |
| Remotos | `remote`, `fetch`, `pull`, `push` |
| Investigar bugs | `bisect`, `blame`, `grep`, `log -S` |
| Recuperar | `reflog`, `reset`, `restore`, `fsck` |
| Manutenção | `clean`, `gc`, `maintenance`, `prune`, `repack` |
| Exportar | `archive`, `bundle`, `format-patch` |
| Aplicar patches | `apply`, `am` |
| Subprojetos | `submodule`, `subtree` |
| Segurança | `verify-commit`, `verify-tag` |
| Ajuda/configuração | `help`, `config`, `version` |

Outros comandos especializados podem ser descobertos com:

```powershell
git help -a
git help -g
```

Os chamados comandos de “encanamento” (*plumbing*), como `cat-file`, `hash-object`, `ls-tree`, `rev-parse`, `update-index` e `write-tree`, expõem as estruturas internas do Git. Eles são úteis para scripts, ferramentas e estudo profundo, mas não são necessários para o trabalho cotidiano.

Exemplos de inspeção interna segura:

```powershell
git rev-parse HEAD
git cat-file -t HEAD
git cat-file -p HEAD
git ls-tree HEAD
```

Para a GitHub CLI:

```powershell
gh help
gh help reference
```

### 23.1 Como ler as fichas visuais

As fichas abaixo usam sempre as mesmas cinco caixas:

```text
┌──────────────┐  ┌────────┐  ┌──────────────────┐  ┌────────┐  ┌────────┐
│ seus arquivos│  │ stage  │  │ commits/branches │  │ origin │  │ GitHub │
│ working tree │  │ index  │  │      locais      │  │ remoto │  │  PRs   │
└──────────────┘  └────────┘  └──────────────────┘  └────────┘  └────────┘
```

Os símbolos significam:

```text
✓ altera essa caixa
— não altera essa caixa
↔ apenas consulta/compara
! pode descartar ou reescrever dados
```

Um comando pode mexer no histórico sem mexer nos arquivos, ou mexer nos arquivos sem mexer no histórico. Essa é a ideia mais importante destas fichas.

### 23.2 `git status` — olhar o painel do carro

```text
Arquivos ↔ | Stage ↔ | Histórico ↔ | Remoto — | GitHub —
```

O comando apenas lê o estado. Não corrige, não prepara e não envia nada.

```powershell
git status
git status --short
git status --short --branch
git status --ignored
```

Possibilidades:

| Forma | O que mostra |
|---|---|
| `status` | explicação completa do estado atual |
| `--short` | códigos compactos, como `M`, `A`, `D` e `??` |
| `--branch` | branch atual e diferença em relação ao remoto |
| `--ignored` | inclui arquivos ignorados |

Exemplo real: você editou `Main.cs`, preparou `Program.cs` e criou `anotacoes.txt`.

```text
 M Main.cs       → modificado apenas na pasta
M  Program.cs    → modificação já colocada no stage
?? anotacoes.txt → arquivo novo, ainda desconhecido pelo Git
```

Analogia: `status` é abrir a despensa e fazer um inventário; nenhum produto é movido.

### 23.3 `git add` — montar a caixa do próximo commit

```text
Arquivos ↔ | Stage ✓ | Histórico — | Remoto — | GitHub —
```

Antes:

```text
Pasta:  Main.cs(B), Program.cs(B), notas.txt(novo)
Stage:  vazio
Commit: Main.cs(A), Program.cs(A)
```

Depois de `git add Main.cs`:

```text
Pasta:  Main.cs(B), Program.cs(B), notas.txt(novo)
Stage:  Main.cs(B)
Commit: Main.cs(A), Program.cs(A)
```

O arquivo não sai da pasta. O Git coloca a versão B de `Main.cs` na caixa do próximo commit.

| Forma | Seleção feita |
|---|---|
| `git add Main.cs` | um arquivo específico |
| `git add Windows/` | mudanças sob uma pasta |
| `git add .` | mudanças sob a pasta atual |
| `git add -A` | todas as adições, edições e remoções do repositório |
| `git add -u` | mudanças em arquivos já rastreados; não inclui novos |
| `git add -p` | pergunta trecho por trecho |

Situação real para `-p`: você corrigiu um bug e também mudou uma cor no mesmo arquivo. Pode colocar apenas a correção no primeiro commit e deixar a cor para o próximo.

Se editar `Main.cs` novamente depois do `add`, existirão duas versões:

```text
Stage: Main.cs(B)
Pasta: Main.cs(C)
```

O commit levará B, não C. Confira com `git diff` e `git diff --staged`.

### 23.4 `git commit` — lacrar e catalogar a caixa

```text
Arquivos — | Stage ✓ | Histórico ✓ | Remoto — | GitHub —
```

```powershell
git commit -m "Corrige validação do sensor"
```

Antes:

```text
A ← B  main
Stage: correção em Main.cs
```

Depois:

```text
A ← B ← C  main
          C contém a fotografia preparada
Stage: vazio
```

O commit é local. A internet não é usada e o GitHub ainda não recebeu C.

| Forma | Efeito |
|---|---|
| `commit -m "..."` | cria commit com mensagem curta |
| `commit` | abre editor para mensagem detalhada |
| `commit -a` | prepara mudanças de arquivos rastreados e commita; ignora novos |
| `commit --amend` | substitui o último commit por outro |
| `commit --amend --no-edit` | troca o conteúdo sem mudar a mensagem |
| `commit --allow-empty` | cria commit mesmo sem mudança de arquivo |

`--amend` é como reabrir uma caixa já lacrada e emitir uma caixa nova com outra etiqueta. O hash muda. Se a caixa antiga já foi distribuída, outras pessoas ainda podem ter a versão anterior.

### 23.5 `git diff` — colocar duas versões lado a lado

```text
Arquivos ↔ | Stage ↔ | Histórico ↔ | Remoto — | GitHub —
```

```text
git diff           = pasta versus stage
git diff --staged  = stage versus último commit
git diff A B       = fotografia A versus fotografia B
```

Exemplo:

```text
Commit: velocidade = 10
Stage:  velocidade = 20
Pasta:  velocidade = 30
```

Então:

```text
git diff          mostra 20 → 30
git diff --staged mostra 10 → 20
```

Possibilidades úteis:

| Forma | Resultado |
|---|---|
| `diff -- Main.cs` | limita a um caminho |
| `diff --stat` | resumo por arquivo |
| `diff --name-only` | somente nomes |
| `diff --name-status` | nomes e tipo: adicionado, modificado, removido |
| `diff --word-diff` | destaca palavras, útil em textos |

Analogia: não troca nenhuma página; apenas coloca duas edições do livro abertas para comparação.

### 23.6 `git log`, `show` e `blame` — consultar o arquivo morto

```text
Arquivos — | Stage — | Histórico ↔ | Remoto — | GitHub —
```

`log` consulta a sequência de registros:

```powershell
git log --oneline --graph --decorate --all
git log -- Main.cs
git log -p -- Main.cs
git log --since="2026-01-01" --author="Vinicius"
```

`show` abre um registro específico:

```powershell
git show 4d73f24
git show --stat 4d73f24
git show 4d73f24:Main.cs
```

O último comando não restaura `Main.cs`; apenas imprime a edição antiga na tela.

`blame` anota cada linha com o último commit que a alterou:

```powershell
git blame -L 100,140 Main.cs
```

Exemplo real: uma validação estranha aparece na linha 120. `blame` encontra o commit; `show` revela o contexto completo; `log` mostra o que aconteceu antes e depois.

### 23.7 `git init` e `clone` — abrir um arquivo ou copiar o arquivo inteiro

#### `git init`

```text
Arquivos — | Stage cria | Histórico cria | Remoto — | GitHub —
```

```powershell
git init
git init --initial-branch=main
```

O Git cria `.git`, sua sala de arquivo vazia. Os arquivos existentes continuam onde estavam e inicialmente aparecem como `untracked`.

```text
Antes: Pasta comum com Main.cs
Depois: mesma pasta + .git; Main.cs ainda não está registrado
```

#### `git clone`

```text
Arquivos ✓ | Stage ✓ | Histórico ✓ | Remoto ↔ | GitHub ↔
```

```powershell
git clone URL
git clone URL MinhaPasta
git clone --branch beta URL
git clone --depth 1 URL
git clone --recurse-submodules URL
```

| Opção | O que chega ao computador |
|---|---|
| sem opção | histórico, branches remotas e arquivos da branch padrão |
| `--branch beta` | abre inicialmente a branch/tag escolhida |
| `--depth 1` | traz apenas a ponta recente; histórico antigo fica indisponível até aprofundar |
| `--recurse-submodules` | também baixa os repositórios referenciados como submódulos |

Analogia: `init` compra um fichário vazio para uma mesa existente; `clone` fotocopia um fichário preenchido e deixa o endereço do original anotado como `origin`.

### 23.8 `git branch` e `switch` — marcadores e mesas de trabalho

#### `git branch`

```text
Arquivos — | Stage — | Histórico ✓ referência | Remoto — | GitHub —
```

```powershell
git branch teste
```

Antes e depois:

```text
Antes: A ← B ← C  main
Depois:             main, teste → C
```

Nenhum arquivo muda. Apenas nasce outro marcador no commit C.

| Forma | Efeito |
|---|---|
| `branch` | lista branches locais |
| `branch -a` | inclui referências remotas |
| `branch nome` | cria sem trocar |
| `branch -m nome` | renomeia a atual |
| `branch -d nome` | apaga se o trabalho estiver integrado |
| `branch -D nome` | força exclusão, mesmo com trabalho exclusivo |
| `branch --contains HASH` | encontra branches que alcançam o commit |

Apagar a branch apaga o marcador, não imediatamente os objetos. Porém commits sem outro caminho podem ficar difíceis de encontrar.

#### `git switch`

```text
Arquivos ✓ | Stage ↔ | Histórico — | HEAD ✓ | Remoto —
```

```powershell
git switch main
git switch -c nova-feature
git switch -
git switch --detach 4d73f24
```

Analogia: cada branch é uma mesa com uma edição do projeto. `switch` muda a mesa em que você está sentado; `branch` apenas coloca ou remove placas com nomes.

Se alterações locais puderem acompanhar a troca sem serem sobrescritas, o Git pode carregá-las. Se a troca destruiria conteúdo, normalmente é recusada. Use `status` e `stash` para tornar a transição explícita.

### 23.9 `git restore` — substituir páginas sem mover o marcador

```text
Arquivos ! | Stage opcional ! | Histórico — | Remoto — | GitHub —
```

| Forma | Fonte → destino |
|---|---|
| `restore Main.cs` | stage → pasta; descarta edição não preparada |
| `restore --staged Main.cs` | HEAD → stage; mantém a edição na pasta |
| `restore --source=4d73f24 Main.cs` | commit antigo → pasta |
| `restore --source=4d73f24 --staged --worktree .` | commit antigo → stage e pasta |
| `restore -p Main.cs` | pergunta trecho por trecho |

Cena real:

```text
HEAD:   Main.cs(A)
Stage:  Main.cs(B)
Pasta:  Main.cs(C)
```

Após `git restore Main.cs`:

```text
HEAD:   A
Stage:  B
Pasta:  B   ← C foi descartada
```

Após `git restore --staged Main.cs` partindo da cena original:

```text
HEAD:   A
Stage:  A
Pasta:  C   ← trabalho continua, apenas saiu da caixa do commit
```

Analogia: você troca páginas sobre a mesa ou dentro da caixa ainda não lacrada. O arquivo histórico permanece intacto.

### 23.10 `git stash` — guardar a bancada em uma gaveta

```text
Arquivos ✓ | Stage ✓ | Histórico stash ✓ | Remoto — | GitHub —
```

```powershell
git stash push -u -m "Tela de diagnóstico incompleta"
```

Antes:

```text
Bancada: Main.cs editado + teste.txt novo
Gaveta:  vazia
```

Depois:

```text
Bancada: limpa, igual ao commit
Gaveta:  pacote "Tela de diagnóstico incompleta"
```

| Forma | Efeito |
|---|---|
| `stash push` | guarda alterações rastreadas |
| `push -u` | inclui arquivos novos não rastreados |
| `push -a` | inclui também ignorados; use com cuidado |
| `stash list` | lista gavetas |
| `stash show -p` | abre e examina uma gaveta |
| `stash apply` | copia da gaveta para a bancada e mantém o pacote |
| `stash pop` | aplica e remove o pacote se der certo |
| `stash branch nome` | cria uma mesa no contexto original do pacote |
| `stash drop` | descarta uma gaveta |
| `stash clear` | esvazia todas as gavetas |

O stash é local. Ele não aparece automaticamente em outro computador e não é um bom arquivo permanente.

### 23.11 `git merge` — juntar duas receitas

```text
Arquivos ✓ | Stage ✓ | Histórico ✓ | Remoto — | GitHub —
```

```powershell
git switch main
git merge feature
```

Cena real: na `main`, outra pessoa corrigiu o banco; na `feature`, você melhorou a tela. O merge tenta produzir uma edição contendo ambos.

```text
          D tela ← E tela
         /
A ← B ← C ← F banco
          \         \
           ───────── M tela+banco
```

| Forma | Comportamento |
|---|---|
| `merge feature` | fast-forward quando possível; senão cria merge commit |
| `merge --ff-only feature` | aceita apenas avanço simples; senão recusa |
| `merge --no-ff feature` | sempre registra uma junção explícita |
| `merge --squash feature` | prepara o resultado combinado, sem preservar a ligação como merge |
| `merge --abort` | tenta retornar ao estado anterior ao merge conflitante |

Conflito é o Git entregando duas versões da mesma frase e pedindo que um humano redija a versão final.

### 23.12 `git rebase` — mudar o ponto de partida da viagem

```text
Arquivos ✓ temporariamente | Stage ✓ | Histórico ! reescreve | Remoto — | GitHub —
```

```text
          D ← E feature
         /
A ← B ← C ← F ← G main
```

`git rebase main`, estando na feature:

```text
A ← B ← C ← F ← G main
                  \ D' ← E' feature
```

O Git não move fisicamente D e E: ele cria D' e E', novas edições com novos hashes, reaplicando as mudanças sobre G.

| Forma | Uso |
|---|---|
| `rebase main` | reaplica a branch atual sobre `main` |
| `rebase origin/main` | usa a visão local mais recente do remoto após `fetch` |
| `rebase -i HEAD~4` | reorganiza os quatro commits recentes |
| `rebase --continue` | continua depois de resolver conflito |
| `rebase --skip` | não reaplica o commit atual |
| `rebase --abort` | retorna ao ponto de início |

Analogia: você escreveu três capítulos a partir de uma edição antiga do livro. Rebase imprime novas cópias desses capítulos encaixadas na edição atual. Não faça isso em páginas públicas sem combinar com quem já as recebeu.

### 23.13 `git reset` — mover o marcador da branch

```text
Arquivos depende ! | Stage depende ! | Histórico/branch ! | Remoto — | GitHub —
```

Cena inicial:

```text
A ← B ← C  main/HEAD
```

| Forma | Branch | Stage | Pasta |
|---|---|---|---|
| `reset --soft B` | move para B | mantém conteúdo de C preparado | mantém conteúdo de C |
| `reset --mixed B` | move para B | volta a B | mantém conteúdo de C como edição |
| `reset --hard B` | move para B | volta a B | volta a B; edições podem ser perdidas |
| `reset Main.cs` | não move branch | retira arquivo do stage | mantém pasta |

Analogias:

```text
--soft  → deslacra a última caixa, mas deixa tudo embalado
--mixed → deslacra e tira os itens da caixa, deixando-os na bancada
--hard  → joga fora caixa e bancada para reproduzir a fotografia antiga
```

O remoto não muda até um `push`. Se C já foi publicado, prefira `revert` na maioria dos trabalhos colaborativos.

### 23.14 `git revert` — publicar uma errata

```text
Arquivos ✓ | Stage ✓ | Histórico ✓ novo commit | Remoto — | GitHub —
```

```text
A ← B ← C
        C introduziu um erro
```

Após `git revert C`:

```text
A ← B ← C ← D
            D aplica o contrário de C
```

C não é apagado. D é uma errata dizendo publicamente como desfazer C.

| Forma | Efeito |
|---|---|
| `revert HASH` | desfaz um commit e cria outro |
| `revert --no-commit HASH` | prepara a inversão para revisão/combinação |
| `revert A..B` | reverte uma faixa selecionada |
| `revert -m 1 MERGE` | reverte merge escolhendo o pai principal |
| `revert --abort` | cancela uma reversão em conflito |

É normalmente seguro para histórico compartilhado porque acrescenta informação em vez de trocar páginas já distribuídas.

### 23.15 `git remote` — agenda de endereços

```text
Arquivos — | Stage — | Histórico config ✓ | Servidor — | GitHub —
```

```powershell
git remote -v
git remote add origin URL
git remote set-url origin NOVA_URL
git remote rename origin github
git remote remove github
git remote show origin
```

Analogia: `origin` é o nome “Escritório” na agenda; a URL é o endereço. Apagar o contato não demole o escritório e não apaga nenhum repositório no GitHub.

```text
origin       → apelido local
origin/main  → fotografia local da última main conhecida naquele endereço
main remota  → branch que existe de verdade no servidor
```

### 23.16 `fetch`, `pull` e `push` — receber, aplicar e enviar correspondência

#### `git fetch`

```text
Arquivos — | Stage — | Histórico remoto-local ✓ | Servidor ↔ | GitHub —
```

```powershell
git fetch origin
git fetch --all --prune
```

Baixa cartas e atualiza o índice da correspondência, mas não cola nada no seu manuscrito. Depois você pode comparar:

```powershell
git log main..origin/main --oneline
git diff main origin/main
```

#### `git pull`

```text
Arquivos ✓ | Stage ✓ | Histórico ✓ | Servidor ↔ | GitHub —
```

`pull` é um pacote: primeiro `fetch`, depois integração.

| Forma | Integração usada |
|---|---|
| `pull --ff-only` | somente move a branch se não houver divergência |
| `pull --rebase` | reaplica seus commits locais sobre os recebidos |
| `pull --no-rebase` | usa merge se necessário |

Analogia: `fetch` recolhe a correspondência; `pull` recolhe e já incorpora as revisões ao seu documento.

#### `git push`

```text
Arquivos — | Stage — | Histórico ↔ | Servidor ✓ | GitHub indiretamente ✓
```

```powershell
git push origin main
git push -u origin minha-feature
git push origin --delete feature-antiga
git push --force-with-lease
```

`push` envia commits e move referências remotas. Ele não envia mudanças que estão apenas na pasta ou no stage.

```text
Pasta modificada ─X─> push
Stage preparado   ─X─> push
Commit local      ───> push
```

`--force-with-lease` é como substituir uma pasta no arquivo central somente se ninguém a atualizou desde sua última consulta. Ainda reescreve histórico e exige cuidado.

### 23.17 `git tag` — colocar um selo em uma fotografia

```text
Arquivos — | Stage — | Histórico referência ✓ | Remoto — | GitHub —
```

```powershell
git tag v1.0.0
git tag -a v1.0.0 -m "Primeira versão estável"
git tag -a v0.9.0 4d73f24 -m "Versão anterior"
git push origin v1.0.0
```

Branch é marcador de página que avança a cada commit. Tag é um selo de “edição 1.0” que normalmente permanece na mesma fotografia.

| Tipo | Característica |
|---|---|
| leve | apenas nome apontando para objeto |
| anotada (`-a`) | objeto próprio com autor, data e mensagem |
| assinada (`-s`) | anotada e assinada criptograficamente |

Criar uma tag é local; `git push origin v1.0.0` publica o selo.

### 23.18 `git cherry-pick` — transplantar uma mudança

```text
Arquivos ✓ | Stage ✓ | Histórico ✓ novo commit | Remoto — | GitHub —
```

```text
main:    A ← B ← C
hotfix:  A ← X
```

Estando na `main`, `git cherry-pick X` produz:

```text
main: A ← B ← C ← X'
```

X' contém a mudança de X, mas possui outro pai e outro hash.

| Forma | Efeito |
|---|---|
| `cherry-pick X` | aplica um commit |
| `cherry-pick A^..B` | aplica faixa incluindo A e B |
| `cherry-pick -n X` | aplica sem criar commit imediatamente |
| `cherry-pick --continue` | continua após conflito |
| `cherry-pick --abort` | cancela a operação completa |

Analogia: não se move a página original; fotocopia-se a correção e adapta-se a cópia para outra edição.

### 23.19 `git clean` — esvaziar itens não catalogados da bancada

```text
Arquivos não rastreados ! | Stage — | Histórico — | Remoto — | GitHub —
```

```powershell
git clean -nd
git clean -nfd
git clean -fd
git clean -fdx
```

| Opção | Significado |
|---|---|
| `-n` | simulação; não apaga |
| `-f` | autoriza exclusão |
| `-d` | inclui diretórios não rastreados |
| `-x` | inclui também ignorados, como builds e configurações locais |
| `-i` | modo interativo |

Um arquivo não rastreado nunca entrou no arquivo histórico. Se `clean` apagá-lo, o Git normalmente não tem uma versão para recuperar. Faça sempre `-n` primeiro.

### 23.20 `git worktree` — abrir uma segunda bancada

```text
Pasta adicional ✓ | Stage separado ✓ | Histórico compartilhado ↔ | Remoto —
```

```powershell
git worktree add ..\Honda-antigo 4d73f24
git worktree add -b hotfix ..\Honda-hotfix main
git worktree list
git worktree remove ..\Honda-antigo
```

Cena real:

```text
Honda-Sensor-Checker\  → main atual aberta no Visual Studio
Honda-antigo\          → commit 4d73f24 para compilar e comparar
```

As duas bancadas compartilham o depósito de commits, mas têm arquivos e stages separados. É mais confortável que alternar repetidamente com detached HEAD quando você precisa manter duas versões abertas.

### 23.21 `git bisect` — o investigador que divide suspeitos ao meio

```text
Arquivos ✓ troca commits | Stage — | Histórico — | Remoto —
```

Você sabe que `4d73f24` funcionava e `main` está quebrada:

```powershell
git bisect start
git bisect bad main
git bisect good 4d73f24
```

Se houver 128 commits suspeitos, o Git não pede 128 testes. Ele escolhe o commit do meio; cada resposta `good` ou `bad` elimina metade dos suspeitos, levando cerca de sete testes.

```powershell
git bisect good
git bisect bad
git bisect reset
```

Analogia: procurar uma palavra errada abrindo primeiro o livro no meio, em vez de ler página por página.

### 23.22 `git reflog` — câmera de segurança local

```text
Arquivos — | Stage — | Histórico ↔ registros locais | Remoto —
```

```powershell
git reflog
git reflog show main
```

O log conta a história dos commits. O reflog conta onde suas referências locais estiveram, inclusive antes de reset, rebase ou troca de branch.

```text
main@{0} agora:       B
main@{1} antes reset: D
```

Para colocar uma placa de resgate em D:

```powershell
git branch resgate D
```

O reflog não é sincronizado com o GitHub e expira. É uma câmera de segurança, não um backup eterno.

### 23.23 `git rm`, `mv` e `.gitignore` — retirar, mudar e não catalogar

#### `git rm`

```text
Arquivos ✓ | Stage ✓ remoção | Histórico — até commit
```

```powershell
git rm Arquivo.cs
git rm --cached segredo-local.json
```

Sem `--cached`, remove da pasta e prepara a remoção. Com `--cached`, mantém na pasta e prepara apenas a saída do controle de versão.

#### `git mv`

```text
Arquivos ✓ | Stage ✓ | Histórico — até commit
```

```powershell
git mv Antigo.cs Novo.cs
```

Equivale conceitualmente a mover no sistema e preparar remoção+adição. O Git detecta renomeamentos por semelhança de conteúdo ao comparar versões.

#### `.gitignore`

É a lista “não colocar no catálogo”. Ela afeta arquivos ainda não rastreados; não apaga do histórico algo já registrado.

### 23.24 GitHub CLI — comandar a plataforma, não seus arquivos

Os comandos `gh` geralmente não alteram working tree, stage ou commits. Eles consultam ou modificam objetos hospedados no GitHub.

```text
git commit  → cria história local
git push    → publica história Git
gh pr create→ cria uma proposta de integração no GitHub
```

#### Repositórios

| Comando | Cena real |
|---|---|
| `gh repo view` | abre a ficha do projeto hospedado |
| `gh repo clone dono/projeto` | pede ao Git para copiar o projeto autenticado |
| `gh repo create` | cria um repositório no GitHub; opções decidem público/privado e origem local |
| `gh repo fork` | cria uma cópia na sua conta e pode configurar remotos locais |

#### Pull Requests

| Comando | Efeito no GitHub |
|---|---|
| `gh pr list` | consulta propostas abertas/fechadas |
| `gh pr create` | abre proposta entre duas branches já publicadas |
| `gh pr checkout 123` | cria/troca branch local para examinar o PR; aqui arquivos locais mudam |
| `gh pr diff 123` | somente mostra diferenças |
| `gh pr checks 123` | consulta testes automáticos |
| `gh pr merge 123 --squash` | integra no servidor combinando mudanças em um commit |
| `gh pr close 123` | fecha a proposta sem integrar código |

Analogia: branch é o manuscrito; push o entrega à editora; PR é o pedido “por favor, revise e incorpore estas páginas”; merge é a decisão editorial de publicá-las.

#### Issues, Actions e releases

```text
gh issue create   → abre uma ficha de trabalho
gh workflow run   → solicita uma automação definida pelo projeto
gh run watch      → acompanha a execução, sem alterar seus arquivos
gh release create → publica uma edição no GitHub, normalmente associada a uma tag
```

Sempre consulte `gh COMANDO --help` porque permissões, campos e possibilidades dependem da versão da CLI e das regras do repositório.

---

## 24. Regras de segurança

1. Execute `git status` antes e depois de uma operação importante.
2. Use `git diff` e `git diff --staged` antes do commit.
3. Use `stash -u` ou um commit temporário antes de trocar de contexto.
4. Prefira `revert` para desfazer commits compartilhados.
5. Trate `reset --hard`, `clean -fd`, `branch -D` e `stash clear` como destrutivos.
6. Antes de reescrever histórico, crie uma branch ou tag de segurança.
7. Prefira `--force-with-lease` a `--force` quando um force push for realmente necessário.
8. Não faça rebase de histórico público sem combinar com a equipe.
9. Nunca versione senhas, tokens, chaves privadas ou dados sensíveis.
10. Um arquivo removido do commit atual pode continuar existindo no histórico; vazamentos exigem rotação do segredo e limpeza específica do histórico.

### O ritual de cinco perguntas

Antes de um comando que altera histórico ou arquivos, pergunte:

```text
1. Em qual branch estou?
2. Tenho alterações não commitadas?
3. Este commit já foi enviado?
4. Outra pessoa depende desse histórico?
5. Tenho uma referência de recuperação?
```

Comandos para responder:

```powershell
git status --short --branch
git diff
git diff --staged
git log --oneline --decorate -10
git reflog -10
```

---

## Encerramento

O Git fica mais simples quando cada comando é associado a um objeto:

```text
add      → move conteúdo para o stage
commit   → registra uma fotografia local
branch   → cria ou administra uma etiqueta móvel
HEAD     → indica onde você está
switch   → muda o lugar onde você está trabalhando
restore  → substitui conteúdo de arquivos
merge    → une linhas de desenvolvimento
rebase   → reaplica commits sobre outra base
reset    → move uma referência e, opcionalmente, ajusta arquivos
revert   → cria um commit que desfaz outro
fetch    → atualiza sua visão do remoto
pull     → busca e integra
push     → publica commits e referências
stash    → guarda trabalho incompleto temporariamente
```

Não é necessário memorizar tudo. O mais importante é identificar onde estão seus dados — pasta de trabalho, stage, commits locais ou remoto — e qual dessas áreas o próximo comando modificará.
