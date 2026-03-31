RestaurantSystem
=================

Lightweight .NET solution for managing restaurant operations (orders, menu, inventory, reservations, payments, reports, and auth).

Repository layout
-----------------

- src/RestaurantSystem.Domain/: domain entities, enums and domain exceptions.
- src/RestaurantSystem.Application/: application layer (CQRS features: Auth, Customers, Menu, Orders, Payments, Reservations, Inventory, Reviews, Reports). Start exploring the `Auth` feature for authentication flows.
- src/RestaurantSystem.Infrastructure/: persistence, EF Core configurations, identity implementation, services and migrations.
- src/RestaurantSystem.WebAPI/: HTTP API (controllers, middleware, filters). The API host entrypoint is `Program.cs` here.

Where to start
--------------

- API entrypoint and controllers: [RestaurantSystem.WebAPI/Controllers/AuthController.cs](RestaurantSystem.WebAPI/Controllers/AuthController.cs)
- Application features (commands/queries/DTOs): [RestaurantSystem.Application/Features](RestaurantSystem.Application/Features)
- Domain model and entities: [RestaurantSystem.Domain/Entities](RestaurantSystem.Domain/Entities)

Quick setup
-----------

Prerequisites: .NET SDK (6 or later) and the EF Core CLI if you intend to run migrations.

1. Build the solution:

	dotnet build RestaurantSystem.sln

2. Apply EF Core migrations (example):

	dotnet ef database update --project RestaurantSystem.Infrastructure --startup-project RestaurantSystem.WebAPI

3. Run the Web API:

	cd RestaurantSystem.WebAPI
	dotnet run

Notes
-----

- The `RestaurantSystem.Application` project follows a feature-focused structure (Commands/Queries/DTOs). Look in `Auth` to follow the authentication flow end-to-end.
- The `Infrastructure` project contains EF Core configurations, interceptors for auditing/soft-delete, and identity/token services.
- Use `Program.cs` in the WebAPI project to inspect registered services and middleware (exception handling, JWT middleware, etc.).
- To briefly expose Swagger in a deployed environment, set `Swagger:EnableInProduction` (or `Swagger__EnableInProduction=true`) so OpenAPI/UI are registered outside of development.
- Keep `Cors:AllowedOrigins` up to date with your live domain(s) plus `http://localhost:4200` so local front-end builds can still call the API without altering code.

