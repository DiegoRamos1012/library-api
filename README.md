# Library API

API REST para gerenciamento de uma biblioteca, construída com ASP.NET Core. O
projeto está sendo desenvolvido de forma incremental: atualmente o escopo
implementado concentra-se na autenticação e no gerenciamento de usuários
internos da biblioteca. O esquema em `Library.Api/Docs/database_schema.sql`
também registra entidades previstas para as próximas funcionalidades.

## Sumário

- [Escopo atual](#escopo-atual)
- [Tecnologias e versões](#tecnologias-e-versões)
- [Decisões de arquitetura](#decisões-de-arquitetura)
- [Estrutura de pastas](#estrutura-de-pastas)
- [Pré-requisitos](#pré-requisitos)
- [Configuração local](#configuração-local)
- [Execução](#execução)
- [API disponível](#api-disponível)
- [Result Pattern](#result-pattern)
- [Persistência e migrations](#persistência-e-migrations)
- [Segurança](#segurança)
- [Evolução do projeto](#evolução-do-projeto)

## Escopo atual

### Implementado

- Login de usuários com e-mail e senha.
- Geração e validação de tokens JWT.
- Usuários com os papéis `Employee` e `Manager`.
- Criação de usuários, restrita ao papel `Manager`.
- Consulta de usuário por identificador.
- Alteração de nome, ativação e desativação de usuários.
- Hash de senhas com BCrypt.
- Persistência de usuários em PostgreSQL via Entity Framework Core.
- Seed opcional do primeiro usuário `Manager`.
- Erros de negócio representados por códigos estáveis e respostas HTTP
  padronizadas.

### Previsto no modelo de dados

O arquivo `Library.Api/Docs/database_schema.sql` descreve a evolução planejada
para membros, livros, empréstimos e auditoria. Essas tabelas ainda não fazem
parte do modelo EF Core atualmente registrado em `AppDbContext`; portanto, não
devem ser tratadas como endpoints disponíveis.

## Tecnologias e versões

As versões abaixo são as declaradas no projeto em `Library.Api/Library.Api.csproj`.

| Componente | Versão |
|---|---|
| .NET SDK / runtime alvo | 10.0 (`net10.0`) |
| ASP.NET Core | 10.0, incluído no SDK Web do .NET |
| C# | Versão padrão compatível com o .NET 10 |
| Entity Framework Core | 10.0.12 |
| Provedor PostgreSQL para EF Core (`Npgsql.EntityFrameworkCore.PostgreSQL`) | 10.0.3 |
| JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) | 10.0.12 |
| BCrypt (`BCrypt.Net-Next`) | 4.2.0 |
| Banco de dados | PostgreSQL (versão não fixada no repositório) |

Ao atualizar dependências, mantenha esta tabela e o arquivo `.csproj` alinhados.

## Decisões de arquitetura

### API modular por domínio

O código é organizado por módulos de negócio (`Auth` e `Users`), em vez de
separar o projeto apenas por tipo técnico. Cada módulo reúne controllers, DTOs,
serviços, domínio, repositórios e erros relacionados à sua responsabilidade.
Isso reduz o acoplamento e permite adicionar novos módulos, como `Members`,
`Books` e `Loans`, sem misturar suas regras às existentes.

### Separação de responsabilidades

- **Controllers** recebem requisições HTTP, aplicam autorização e transformam
  resultados em respostas.
- **Services** orquestram casos de uso e regras de aplicação.
- **Domain** mantém estado e invariantes da entidade.
- **Repositories** encapsulam consultas e gravações no banco.
- **Data** concentra `DbContext`, configurações, migrations e seed.
- **Common** contém componentes reutilizáveis, como `Result`, entidade base e
  conversão de erros para `ProblemDetails`.

### Injeção de dependência

As dependências são registradas em `Library.Api/Program.cs`. Repositórios e
serviços de aplicação usam ciclo de vida `Scoped`; `PasswordHasher`,
`JwtTokenService` e `TimeProvider` são `Singleton`; o `AppDbContext` é
registrado pelo `AddDbContext`.

### Persistência

O PostgreSQL é acessado pelo EF Core usando migrations. `BaseEntity` fornece
identificador e timestamps, e `AppDbContext` atualiza `CreatedAt` e `UpdatedAt`
em `SaveChangesAsync`. As relações destrutivas planejadas usam `RESTRICT`, para
evitar remoções em cascata acidentais.

## Estrutura de pastas

```text
Library-Api/
├── Library.Api/
│   ├── Common/
│   │   ├── Controllers/       # Base HTTP e conversão de erros
│   │   ├── Exceptions/         # Exceções de domínio
│   │   ├── Results/            # Result<T> e ResultError
│   │   └── Utils/              # Normalização de textos
│   ├── Config/Security/        # Opções JWT, hash de senha
│   ├── Data/
│   │   ├── Configurations/     # Mapeamentos EF Core
│   │   ├── Migrations/         # Histórico do schema
│   │   └── Seed/               # Dados iniciais
│   ├── Docs/                   # Documentação operacional e schema previsto
│   ├── Modules/
│   │   ├── Auth/               # Login e tokens
│   │   └── Users/              # Usuários e seus casos de uso
│   ├── Program.cs              # Composição da aplicação e pipeline
│   ├── Library.Api.csproj      # Framework e dependências
│   ├── appsettings.json        # Configuração não sensível
│   └── Properties/             # Perfis locais de execução
├── Library-Api.sln
└── README.md
```

Arquivos gerados em `bin/` e `obj/` não fazem parte do código-fonte e são
ignorados pelo Git.

## Pré-requisitos

- .NET 10 SDK.
- PostgreSQL em execução.
- Git.
- Ferramenta `dotnet-ef` para aplicar ou criar migrations:

```bash
dotnet tool install --global dotnet-ef
```

## Configuração local

Entre na pasta do projeto antes de executar os comandos:

```bash
cd Library.Api
```

As credenciais não devem ser colocadas no `appsettings.json`. Use User Secrets
no ambiente de desenvolvimento:

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=library-api;Username=postgres;PASSWORD"
dotnet user-secrets set "Jwt:Secret" "uma-chave-aleatoria-com-pelo-menos-32-caracteres"
dotnet user-secrets set "Seed:Manager:FullName" "Administrador da Biblioteca"
dotnet user-secrets set "Seed:Manager:Email" "manager@example.com"
dotnet user-secrets set "Seed:Manager:Password" "uma-senha-forte"
```

Crie o banco antes de aplicar a migration:

```sql
CREATE DATABASE "library-api";
```

O passo a passo completo, incluindo geração segura da chave JWT, localização
dos secrets e solução de problemas, está em
[`Library.Api/Docs/user-secrets-setup.pt-BR.md`](Library.Api/Docs/user-secrets-setup.pt-BR.md).

## Execução

Na raiz da solução:

```bash
dotnet restore
dotnet build
dotnet run --project Library.Api
```

Ou, dentro de `Library.Api`:

```bash
dotnet ef database update
dotnet run
```

Os perfis locais usam `Development` e disponibilizam, por padrão,
`http://localhost:5068` e `https://localhost:7271`. Na inicialização, o
`ManagerSeeder` cria o primeiro Manager somente quando a tabela `users` está
vazia e os três valores `Seed:Manager:*` estão configurados.

## API disponível

Todas as rotas abaixo usam JSON. Rotas protegidas exigem:

```http
Authorization: Bearer <accessToken>
```

### Autenticação

#### `POST /auth/login`

Não exige autenticação.

```json
{
  "email": "manager@example.com",
  "password": "uma-senha-forte"
}
```

Resposta `200 OK`:

```json
{
  "accessToken": "<jwt>"
}
```

Credenciais inválidas retornam `401 Unauthorized`.

### Usuários

Todas as rotas de usuários exigem autenticação. Operações marcadas como
`Manager` também exigem o claim de papel correspondente.

| Método e rota | Autorização | Sucesso |
|---|---|---|
| `POST /users` | Manager | `201 Created` |
| `GET /users/{id}` | Usuário autenticado | `200 OK` |
| `PATCH /users/{id}/change-user-name` | Manager | `200 OK` |
| `PATCH /users/{id}/deactivate` | Manager | `200 OK` |
| `PATCH /users/{id}/activate` | Manager | `200 OK` |

Exemplo de criação:

```json
{
  "fullName": "Maria da Silva",
  "email": "maria@example.com",
  "password": "uma-senha-forte",
  "role": "Employee"
}
```

Exemplo de alteração de nome:

```json
{
  "newFullName": "Maria Souza"
}
```

Validações de entrada são definidas nos DTOs. O identificador `{id}` deve ser
um GUID.

## Result Pattern

O projeto evita usar exceções para representar falhas esperadas de negócio.
`Result<T>` em `Common/Results/Result.cs` pode conter exatamente um valor de
sucesso ou um `ResultError`:

```csharp
var result = await userService.GetByIdAsync(id);

if (result.IsFailure)
    return ToProblem(result.Error);

return Ok(result.Value);
```

Regras principais:

- `Result<T>.Success(value)` cria um resultado bem-sucedido.
- `Result<T>.Failure(error)` cria uma falha tipada.
- `IsFailure` indica falha.
- `Value` só pode ser lido em sucesso; caso contrário lança
  `InvalidOperationException`.
- `Error` só pode ser lido em falha; caso contrário lança
  `InvalidOperationException`.
- `ResultError` contém `Code`, `Message` e uma lista opcional de `Details`.

Os serviços definem erros reutilizáveis em `UserErrors` e `AuthErrors`. O
`ApiControllerBase` converte seus códigos em `ProblemDetails` e status HTTP:

| Código | HTTP |
|---|---:|
| `USER_NOT_FOUND` | 404 |
| `USER_INVALID_INPUT` | 400 |
| `AUTH_INVALID_CREDENTIALS` | 401 |
| `USER_EMAIL_ALREADY_EXISTS` | 409 |
| `USER_ALREADY_DEACTIVATED` | 409 |
| `USER_ALREADY_ACTIVATED` | 409 |
| `USER_DEACTIVATED` | 409 |
| Código não mapeado | 500 |

Ao adicionar um erro de negócio, defina-o no módulo responsável e atualize o
mapeamento de `ApiControllerBase` quando ele exigir um status HTTP específico.
Não retorne mensagens diferentes para o mesmo código sem necessidade.

## Persistência e migrations

O modelo atualmente persistido contém `User`, configurado em
`Data/Configurations/UserConfiguration.cs`. Para atualizar o banco depois de
uma mudança de modelo:

```bash
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

As migrations devem ser versionadas em
`Library.Api/Data/Migrations`. O SQL em `Library.Api/Docs/database_schema.sql`
é uma referência do desenho geral e não substitui a execução das migrations.

## Segurança

- Segredos ficam fora do Git, preferencialmente em User Secrets no
  desenvolvimento e em variáveis de ambiente no deploy.
- A chave JWT deve ter pelo menos 32 caracteres e ser aleatória.
- Senhas são armazenadas somente como hashes BCrypt.
- Tokens validam issuer, audience, validade e chave de assinatura.
- Não inclua senhas, connection strings ou tokens em commits, logs ou exemplos
  reais.
- O acesso de Manager é controlado com `[Authorize(Roles = "Manager")]`.

## Evolução do projeto

Para manter este documento atualizado ao adicionar funcionalidades:

1. Inclua o novo módulo na árvore de pastas e descreva seu caso de uso.
2. Atualize a tabela de endpoints com método, rota, autorização e códigos de
   resposta.
3. Registre novas entidades, migrations e mudanças no `database_schema.sql`.
4. Documente novos códigos de `ResultError` na seção do Result Pattern.
5. Atualize a tabela de versões quando uma dependência for alterada.
6. Revise os comandos de configuração e os exemplos de payload.

O README descreve o estado do código; decisões futuras ou funcionalidades
ainda não implementadas devem ser identificadas como planejadas, em vez de
serem apresentadas como disponíveis.
