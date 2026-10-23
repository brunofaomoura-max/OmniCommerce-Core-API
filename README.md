# OmniCommerce Core API

## Visão geral
Esta solução demonstra uma **Web API de gestão de vendas/e‑commerce** construída com as melhores práticas de arquitetura corporativa:
- **Clean Architecture & DDD** – camadas bem separadas (Domain, Application, Infrastructure, API).
- **CQRS + MediatR** – comandos e consultas desacoplados.
- **EF Core (writes) + Dapper (reads)** – combina produtividade e performance.
- **JWT + Role‑Based Authorization** – segurança baseada em claims/roles (`Cliente`, `Admin`).
- **Global Exception Handling** – respostas padronizadas com *ProblemDetails* (RFC 7807).
- **Serilog** – logging estruturado.
- **Testes unitários** – cobertura com xUnit, Moq e FluentAssertions.

---

## Arquitetura
```
src/
├─ Api/                 # ASP.NET Core (controllers, middleware, Program.cs)
├─ Application/         # Use‑cases (Commands, Queries, DTOs, Pipeline, Interfaces)
├─ Domain/              # Entidades ricas, Value Objects, Domain Events
├─ Infrastructure/      # EF Core DbContext, Fluent mappings, Repositories, Dapper
└─ Tests/ (opcional)   # xUnit test project
```
* Cada camada depende apenas das camadas internas (Domain → Application → Infrastructure → API).
* Os **Domain Events** são publicados via MediatR (`PedidoCriadoEvent`).
* **Commands** (`CreateOrderCommand`) são validados automaticamente por `ValidationBehavior`.
* **Queries** (`GetOrderByIdQuery`) utilizam Dapper para leitura de alta performance.

---

## Como executar localmente

### Pré‑requisitos
- **.NET 8 SDK** (ou superior) – <https://dotnet.microsoft.com/download>
- **SQL Server** (ou outro provider configurado em `appsettings.json`).
- **Git** – já está instalado (versão 2.45.1).

### Passos
1. **Clonar o repo** (já está criado, então basta `git clone`).
2. **Restaurar dependências**
   ```powershell
   dotnet restore
   ```
3. **Configurar a connection string** – edite `src/Api/appsettings.json`:
   ```json
   "ConnectionStrings": { "DefaultConnection": "Server=localhost;Database=OmniCommerce;Trusted_Connection=True;" }
   ```
4. **Aplicar migrations** (EF Core) – cria as tabelas `Pedidos` e `ItensPedido`:
   ```powershell
   dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/Api
   dotnet ef database update --project src/Infrastructure --startup-project src/Api
   ```
5. **Rodar a API**
   ```powershell
   dotnet run --project src/Api/src/Api.csproj
   ```
   A aplicação ficará disponível em `https://localhost:5001` (ou `http://localhost:5000`).

### Testando os endpoints

#### 1 Criar pedido (Cliente)
```bash
curl -X POST https://localhost:5001/api/pedidos \
     -H "Authorization: Bearer <SEU_JWT_COM_ROLE_Cliente>" \
     -H "Content-Type: application/json" \
     -d '{
           "ClienteId":"cliente-123",
           "Itens":[{"ProdutoId":"d3b9f7a1-8f4a-4c2b-bd6a-7e5d8b7c9f12","Quantidade":2,"PrecoUnitario":49.90}]
         }' -k
```
- **Resposta 201** – contém o `Id` do pedido criado.
- Caso a validação falhe, o middleware devolve 400 com `ProblemDetails`.

#### 2 Obter pedido (Cliente ou Admin)
```bash
curl -X GET https://localhost:5001/api/pedidos/<pedidoId> \
     -H "Authorization: Bearer <SEU_JWT_COM_ROLE_Cliente_ou_Admin>" -k
```
- **200 OK** com o objeto `PedidoDto`.
- **404 Not Found** se o pedido não existir.

---

## Testes unitários
O projeto inclui um teste de sucesso para `CreateOrderCommandHandler`:
```csharp
public async Task Handle_ShouldCreateOrderAndReturnId_WhenCommandIsValid()
```
Execute todos os testes com:
```powershell
dotnet test
```

---

## Tecnologias usadas
| Camada | Biblioteca / Framework |
|--------|------------------------|
| **Domain** | .NET Standard, MediatR |
| **Application** | MediatR, FluentValidation |
| **Infrastructure** | EF Core, Dapper |
| **API** | ASP.NET Core, JWT Bearer, Serilog |
| **Testes** | xUnit, Moq, FluentAssertions |

---

## JWT Authentication

A API utiliza **JWT (JSON Web Token)** para autenticação e autorização baseada em claims.

### Configuração

No arquivo `src/Api/appsettings.json` adicione as seguintes chaves:
```json
"Jwt": {
  "Authority": "https://seu-dominio.com/",
  "Audience": "OmniCommerceApi",
  "Secret": "MySuperSecretKey12345" // chave simétrica usada somente em ambiente de desenvolvimento
}
```
- **Authority** – URL do provedor de identidade (ex.: IdentityServer, Azure AD).
- **Audience** – identificador da API que o token deve conter.
- **Secret** – chave simétrica para assinatura HS256 (apenas para testes locais). Em produção recomenda‑se usar um certificado ou um provedor externo.

### Geração de token de teste

Para gerar rapidamente um token de teste você pode usar o site **[jwt.io](https://jwt.io)** ou o script PowerShell abaixo:
```powershell
# Requires the System.IdentityModel.Tokens.Jwt package
Add-Type -AssemblyName "System.IdentityModel.Tokens.Jwt"
$secret = [System.Text.Encoding]::UTF8.GetBytes("MySuperSecretKey12345")
$tokenHandler = New-Object System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler
$claims = [System.Collections.Generic.List[System.Security.Claims.Claim]]::new()
$claims.Add((New-Object System.Security.Claims.Claim("sub","test-user")))
$claims.Add((New-Object System.Security.Claims.Claim("role","Cliente")))
$expires = (Get-Date).AddHours(1)
$tokenDescriptor = [Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor]@{
    Subject = New-Object System.Security.Claims.ClaimsIdentity($claims)
    Expires = $expires
    SigningCredentials = New-Object Microsoft.IdentityModel.Tokens.SigningCredentials(
        (New-Object Microsoft.IdentityModel.Tokens.SymmetricSecurityKey $secret),
        [Microsoft.IdentityModel.Tokens.SecurityAlgorithms]::HmacSha256Signature)
}
$token = $tokenHandler.CreateToken($tokenDescriptor)
$jwt = $tokenHandler.WriteToken($token)
Write-Output $jwt
```
Exemplo de token gerado (válido por 1 hora):
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0LXVzZXIiLCJyb2xlIjoiQ2xpZW50ZSIsImV4cCI6MTcwMDU0ODQwMH0.KzY4s8Z5K7bGQ9YVd0E2pVb5K0YV0tWcYj6L1Xn2YV8
```
Use este token nos cabeçalhos das requisições:
```bash
curl -X GET https://localhost:5001/api/pedidos/<pedidoId> \
     -H "Authorization: Bearer <TOKEN_AQUI>"
```
> **Atenção:** nunca envie a chave `Jwt:Secret` para repositórios públicos. Substitua‑a por uma chave segura antes de colocar a aplicação em produção.

---

## Badges (opcional – adicionados via GitHub Actions)
```
[![Build](https://github.com/brunofaomoura-max/OmniCommerce-Core-API/actions/workflows/build.yml/badge.svg)](https://github.com/brunofaomoura-max/OmniCommerce-Core-API/actions)
```
*Adicione o workflow `build.yml` ao repositório para gerar o badge.*

---

## Contribuição
1. Fork o repositório.
2. Crie uma branch para sua feature (`git checkout -b feat/nova-funcionalidade`).
3. Commit suas mudanças (`git commit -am 'Add new feature'`).
4. Push (`git push origin feat/nova-funcionalidade`).
5. Abra um Pull Request.

---
