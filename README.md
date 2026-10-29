# InstAlert Web Services

ASP.NET Core REST API using C#, EF Core and MySQL. The backend follows the current CatchUp classroom structure: explicit command/query services, operation error enums, repositories, unit of work, REST resources and assemblers. Shared provides technical support. Business owns commerce profiles; personal data belongs to Profiles, employee memberships and invitations belong to Business, and subscriptions belong to Payments. Only the commerce-profile operations are implemented in this version.

## Run locally

Use the .NET 10 SDK, MySQL Server and the EF Core CLI. The API references EF Core 10.0.13; use the matching CLI version for migration tooling. The current local configuration targets `instalert_platform`. The server must be running and the configured database user must have the required database/schema permissions. Each team member must configure a connection for their own MySQL instance. Environment variables such as `ConnectionStrings__DefaultConnection` can override the file configuration.

```powershell
dotnet restore instalert-platform.sln
dotnet ef database update --project InstAlertPlatform.API
dotnet run --project InstAlertPlatform.API --launch-profile http
```

The configuration files follow the classroom example: `appsettings.json` provides the base settings, `appsettings.Development.json` provides local development settings, and `appsettings.Production.json` uses `%DATABASE_URL%`, `%DATABASE_USER%`, `%DATABASE_PASSWORD%` and `%DATABASE_SCHEMA%` in its connection template. `Program.cs` expands these environment variables before configuring MySQL. `DATABASE_URL` is the MySQL server host. Development can apply migrations on startup when `Database:ApplyMigrationsOnStartup` is enabled and the connection is configured. Production never applies them automatically. Configure `Cors:AllowedOrigins` for browser access; the development configuration permits the local frontend on port 5193.

OpenAPI: `http://localhost:5211/openapi/v1.json`. Scalar: `http://localhost:5211/scalar/v1`. English (`en-US`) is the default; `Accept-Language: es-419` selects Latin American Spanish for messages and operation documentation.

## Business profile API

| Method | Route | Operation |
|---|---|---|
| GET | `/api/v1/businesses` | List the caller's businesses |
| POST | `/api/v1/businesses` | Register a business |
| GET | `/api/v1/businesses/{id}` | Read an owned business |
| PUT | `/api/v1/businesses/{id}` | Update its name, address and coordinates |

Registration requires an eleven-digit textual `taxId`, `name`, `address`, `latitude` and `longitude`. Name and address limits are 150 and 250 characters. Coordinates must be finite and within latitude/longitude bounds. The registered tax identifier is immutable. The database enforces its uniqueness. Public IDs are serialized as strings; audit dates are assigned by the server and stored in UTC. Registration returns 201 and a Location header; empty lists return 200. Invalid requests return localized ProblemDetails, with field errors for model validation.

For local development without IAM, the `User-Id` header accepts only configured synthetic identities when `DevelopmentActors:Enabled` is true. `synthetic-admin` is the configured development administrator; `synthetic-employee` has no administrator privileges. The header is rejected outside Development. This delivery does not implement sign-in, credentials or production authentication. Ownership checks remain server-side, and the frontend role selection does not grant permissions.

## Verification

```powershell
dotnet build instalert-platform.sln
```
