# Configuration and Secrets Runbook

This project uses environment-driven configuration with secrets kept out of source control.

## Supported environments

- `Development`
- `Staging`
- `Production`

## Configuration precedence

`RestaurantSystem.API` loads configuration in this order:

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (Development only)
4. Environment variables
5. Command-line arguments

Higher items are overridden by lower items.

## Required secret

The API requires:

- `ConnectionStrings:DefaultConnection`

If it is missing, the app fails fast at startup.

## Local development (User Secrets)

From the repo root:

```bash
dotnet user-secrets --project RestaurantSystem.API init
dotnet user-secrets --project RestaurantSystem.API set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=restaurant_db;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Disable"
dotnet user-secrets --project RestaurantSystem.API list
dotnet user-secrets set "Admin:Email" "admin@restaurant.com" -p RestaurantSystem.WebAPI
dotnet user-secrets set "Admin:Password" "StrongPassword123!" -p RestaurantSystem.WebAPI
dotnet user-secrets set "Admin:FirstName" "Admin" -p RestaurantSystem.WebAPI
dotnet user-secrets set "Admin:LastName" "User" -p RestaurantSystem.WebAPI

```

To remove a secret:

```bash
dotnet user-secrets --project RestaurantSystem.API remove "ConnectionStrings:DefaultConnection"
```

## Production (Container/K8s)

Inject connection string through environment variables:

```bash
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=restaurant_db;Username=app_user;Password=***;SSL Mode=Require;Trust Server Certificate=false
```

Notes:

- Do not store credentials in any tracked `appsettings*.json`.
- Prefer strong TLS settings in production (`SSL Mode=Require` and valid cert chain).
- Environment variables override Development User Secrets when both are set.

## Environment selection

With `dotnet run`:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project RestaurantSystem.API
ASPNETCORE_ENVIRONMENT=Staging dotnet run --project RestaurantSystem.API
ASPNETCORE_ENVIRONMENT=Production dotnet run --project RestaurantSystem.API
```

`launchSettings.json` now includes dedicated profiles for all three environments.

## EF Core logging safety

Sensitive EF options are disabled by default:

- `EfCore:EnableSensitiveDataLogging=false`
- `EfCore:EnableDetailedErrors=false`

Only enable these temporarily for debugging in non-production environments.
