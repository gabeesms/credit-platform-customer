# Credit Platform - Customer Service

Serviço responsável pelo cadastro e consulta dos dados cadastrais de clientes. CPF, nome, e-mail e data de nascimento são validados no domínio; este serviço não analisa crédito nem cria cartões.

## Stack e arquitetura

- .NET 10 e ASP.NET Core Web API
- Clean Architecture: API, Application, Domain e Infrastructure
- SQL Server com Entity Framework Core 10
- Swagger/OpenAPI e xUnit

O serviço é dono dos dados de Customer e do banco `CustomerServiceDb`. Nenhum outro serviço deve acessar esse banco diretamente. A API depende dos casos de uso e contratos da Application; a Infrastructure implementa o repositório e o mapeamento EF Core.

Decisões arquiteturais estão em [docs/architecture.md](docs/architecture.md) e [docs/adr/](docs/adr/).

## Executar localmente

Pré-requisitos: .NET SDK 10 e uma instância SQL Server acessível.

Configure a conexão local por User Secrets. Não grave credenciais em `appsettings*.json` nem no Git:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=CustomerServiceDb;Trusted_Connection=True;TrustServerCertificate=True" --project src/CustomerService.API/CustomerService.API.csproj
```

Se a instância exigir autenticação SQL, configure a connection string apropriada no User Secrets ou no provedor seguro do ambiente. Em produção, use configuração/secret manager do ambiente, não User Secrets.

Instale a ferramenta EF Core se necessário, aplique as migrations e inicie a API:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.12
dotnet ef database update --project src/CustomerService.Infrastructure/CustomerService.Infrastructure.csproj --startup-project src/CustomerService.API/CustomerService.API.csproj
dotnet run --project src/CustomerService.API/CustomerService.API.csproj
```

Swagger: `https://localhost:7087/swagger` ou `http://localhost:5175/swagger`, conforme o perfil de execução.

## Endpoints

### `POST /api/customers`

```json
{
	"name": "Ana Silva",
	"cpf": "529.982.247-25",
	"email": "ana@example.com",
	"birthDate": "1990-01-01T00:00:00Z"
}
```

Retorna `201 Created` e a localização do recurso. CPF inválido ou campos inválidos retornam `400`; CPF já cadastrado retorna `409`. Erros usam `ProblemDetails`.

### `GET /api/customers/{id}`

Retorna `200 OK` para um cliente existente e `404 Not Found` caso contrário. Falhas inesperadas retornam `500` sem detalhes internos no corpo.

Os contratos podem ser exercitados em [src/CustomerService.API/CustomerService.API.http](src/CustomerService.API/CustomerService.API.http).

## Migrations e testes

As migrations ficam em `src/CustomerService.Infrastructure/Persistence/Migrations`. Para criar uma migration após uma mudança de modelo:

```powershell
dotnet ef migrations add NomeDaMigration --project src/CustomerService.Infrastructure/CustomerService.Infrastructure.csproj --startup-project src/CustomerService.API/CustomerService.API.csproj
```

Execute build e testes com:

```powershell
dotnet build CustomerService.slnx
dotnet test CustomerService.slnx
```

Os testes unitários não dependem de SQL Server. Os testes HTTP usam um repositório em memória; valide migrations e conectividade em uma instância SQL Server separadamente.

## Segurança e limitações

- Não há autenticação/autorização configurada nesta etapa.
- Não há RabbitMQ nem eventos publicados pelo Customer Service.
- A conexão SQL Server é fornecida externamente; não há segredo versionado.
- Health checks e testes de integração com SQL Server ficam para uma etapa posterior, se necessários.
