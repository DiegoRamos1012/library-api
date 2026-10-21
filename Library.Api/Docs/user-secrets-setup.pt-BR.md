# Desenvolvimento local: configuração dos User Secrets

Este projeto mantém todas as credenciais **fora do Git**. Em desenvolvimento, os segredos
ficam nos **User Secrets** do .NET, que são guardados por máquina. Quando você começa a
trabalhar em uma máquina nova (um notebook, por exemplo), o projeto compila, mas falha na
subida até você cadastrar os segredos abaixo.

## Pré-requisitos

- .NET 10 SDK
- PostgreSQL instalado e rodando localmente
- Git
- Ferramenta do EF Core: `dotnet tool install --global dotnet-ef`
- Opcional: `openssl` (vem junto com o Git Bash) para gerar a chave do JWT

## Chaves necessárias

| Chave | Para que serve | Exemplo de valor |
|---|---|---|
| `ConnectionStrings:Default` | Conexão com o PostgreSQL | `Host=localhost;Port=5432;Database=library-api;Username=postgres;Password=SUA_SENHA_DO_BANCO` |
| `Jwt:Secret` | Chave de assinatura dos tokens de acesso (HS256, no mínimo 32 caracteres) | resultado de `openssl rand -base64 48` |
| `Seed:Manager:FullName` | Nome do primeiro Manager | `Administrador da Biblioteca` |
| `Seed:Manager:Email` | E-mail do primeiro Manager | `manager@example.com` |
| `Seed:Manager:Password` | Senha do primeiro Manager (use uma forte) | `SUA_SENHA_FORTE` |

As configurações do JWT que não são secretas (`Jwt:Issuer`, `Jwt:Audience` e
`Jwt:ExpirationMinutes`) já estão no `appsettings.json` e não precisam ser cadastradas.

## Passo a passo

### 1. Clone o repositório e entre na pasta do projeto

Todos os comandos `dotnet user-secrets` e `dotnet ef` precisam rodar na pasta que contém o
`Library.Api.csproj` (e não na raiz da solução). Como alternativa, acrescente
`--project Library.Api` em cada comando.

```
cd Library.Api
```

### 2. Crie o banco de dados

```
psql -U postgres
CREATE DATABASE "library-api";
\q
```

O nome tem hífen, então precisa de aspas duplas no SQL. Na connection string não precisa.

### 3. Gere a chave do JWT

```
openssl rand -base64 48
```

Copie o resultado. Não reaproveite uma frase simples: o que protege os tokens é a
aleatoriedade.

### 4. Cadastre os segredos

O projeto já tem um `UserSecretsId` no `Library.Api.csproj`, então o
`dotnet user-secrets init` **não** é necessário em uma máquina nova.

```
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=library-api;Username=postgres;Password=SUA_SENHA_DO_BANCO"
dotnet user-secrets set "Jwt:Secret" "COLE_AQUI_O_VALOR_GERADO"
dotnet user-secrets set "Seed:Manager:FullName" "Administrador da Biblioteca"
dotnet user-secrets set "Seed:Manager:Email" "manager@example.com"
dotnet user-secrets set "Seed:Manager:Password" "SUA_SENHA_FORTE"
```

### 5. Confira o que foi salvo

```
dotnet user-secrets list
```

Devem aparecer as cinco chaves acima.

### 6. Aplique as migrations

```
dotnet ef database update
```

### 7. Rode a API e faça login

Inicie a aplicação (Run do Rider ou `dotnet run`). Na primeira subida, se a tabela `users`
estiver vazia, o seeder cria o Manager a partir dos segredos `Seed:Manager:*`. Em seguida:

```
POST /auth/login
{ "email": "manager@example.com", "password": "SUA_SENHA_FORTE" }
```

A resposta traz o `accessToken`, que vai como Bearer token nas demais chamadas.

## Onde os segredos ficam guardados

- Windows: `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`
- Linux e macOS: `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`

O `UserSecretsId` é o GUID que está dentro do `Library.Api.csproj`. O arquivo é local da
máquina e nunca vai para o Git.

## Observações

- Os User Secrets só são carregados quando o ambiente é `Development`. O perfil de
  execução padrão do Rider já define `ASPNETCORE_ENVIRONMENT=Development`.
- No PowerShell, coloque entre aspas **simples** os valores que contêm `$`, senão o
  PowerShell os interpreta como variáveis.
- Evite `;` e `=` na senha do banco em desenvolvimento local, eles quebram a connection
  string se não estiverem entre aspas.
- O seeder só roda quando a tabela `users` está vazia. Para rodar o seed de novo em um
  banco de desenvolvimento: `TRUNCATE TABLE users;` e reinicie a aplicação.
- Alternativa aos User Secrets (CI, Docker mais adiante): variáveis de ambiente, usando
  dois underscores no lugar dos dois pontos, por exemplo `ConnectionStrings__Default` e
  `Jwt__Secret`.

## Solução de problemas

| Sintoma | Causa provável |
|---|---|
| `ArgumentNullException` na subida | Falta o `Jwt:Secret`, confira com `dotnet user-secrets list` |
| Erro sobre o tamanho da chave ao gerar token | O `Jwt:Secret` tem menos de 32 caracteres |
| `Manager seed failed: ...` na subida | Os valores do seed são inválidos (e-mail inválido, senha curta) |
| Password authentication failed for user | Usuário ou senha errados em `ConnectionStrings:Default` |
| Database "library-api" does not exist | O passo 2 foi pulado |
| 404 em uma rota que você acabou de criar | A aplicação ainda está rodando o build antigo, pare e rode de novo |
| Login devolve 401 com a senha certa | O usuário pode estar desativado, confira `IsActive` na tabela `users` |

## Regras de segurança

- Nunca coloque segredos no `appsettings.json`, no código, em commits ou em capturas de tela.
- Use um `Jwt:Secret` e uma senha de Manager diferentes em cada ambiente.
- Se um segredo vazar, troque. Mudar o `Jwt:Secret` invalida todos os tokens existentes.
