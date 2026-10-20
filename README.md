# E-Commerce Microservices

E-commerce backend split into independently deployable services behind an API gateway.
Each service follows **clean architecture** (Domain → Application → Infrastructure → Presentation)
and shares cross-cutting infrastructure through a common library.

```
                    ┌──────────────────────────┐
  client ─────────► │  API Gateway (Ocelot)    │  :5003  routing · caching · rate limiting · JWT
                    └──────┬─────────┬─────────┘
                           │         │         │
                 ┌─────────▼──┐ ┌────▼──────┐ ┌▼───────────┐
                 │ Auth API   │ │ Product   │ │ Order API  │
                 │   :5000    │ │ API :5001 │ │   :5002    │── calls Product API (Polly retry)
                 └────────────┘ └───────────┘ └────────────┘
                           \         |         /
                         eCommerceSharedLibrary (JWT, Serilog, global exception
                         handling, gateway-only middleware, generic repository)
```

## Services

| Folder | Responsibility | Highlights |
|---|---|---|
| `GatewayApi` | Single entry point | Ocelot routing, response caching, rate limiting on auth endpoints, JWT validation |
| `AuthApi` | Registration & login | BCrypt password hashing, JWT issuing |
| `ProductApi` | Product catalogue | Generic repository, EF Core, cached reads via the gateway |
| `OrderApi` | Orders | Resilient inter-service HTTP calls with a Polly retry pipeline |
| `eCommerceSharedLibrary` | Shared building blocks | JWT scheme, Serilog setup, global exception middleware, `ListenToOnlyApiGateway` middleware |

## Tech

.NET 8 · ASP.NET Core Web API · Entity Framework Core · SQL Server (LocalDB) · Ocelot · Polly · Serilog · BCrypt · Swagger · Docker

## Getting started

```bash
dotnet build ECommerce.Microservices.slnx
```

The JWT signing key is a placeholder. Set the same key for every service with user secrets:

```bash
dotnet user-secrets init --project AuthApi/AuthenticationApi.Presentation
dotnet user-secrets set "Authentication:Key" "<your-32+-character-key>" --project AuthApi/AuthenticationApi.Presentation
```

Repeat both commands for `ProductApi/ProdcutApi.Presentation`, `OrderApi/OrderApi.Presentation`
and `GatewayApi/ApiGateway.Presentation`, then apply the EF Core migrations and start each service.

> This repository consolidates the earlier standalone `ProductApi`, `OrderApi`, `AuthApi` and `GatewayApi` repositories.
