# InstAlert Web Services

ASP.NET Core REST API using C#, EF Core and MySQL. The backend follows the current CatchUp classroom structure: explicit command/query services, operation error enums, repositories, unit of work, REST resources and assemblers. Shared provides technical support. Business owns commerce profiles, employee memberships and invitations; personal data belongs to Profiles; subscriptions and billing records belong to Payments. This branch adds PayPal subscription checkout, server-side verification, plan changes, cancellation, billing history and webhook reconciliation.

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

In Development, `DevelopmentData:SeedOnStartup` creates an idempotent synthetic business owned by `synthetic-admin`, an active membership for `synthetic-employee`, and a 30-day simulated Essential subscription when those records do not already exist. Existing records are preserved. The business receives its database-generated ID on first startup; call `GET /api/v1/businesses` with `User-Id: synthetic-admin` or `User-Id: synthetic-employee` to retrieve it. No IAM accounts, credentials, or real payments are created.

## Payments API

All business subscription routes require `User-Id` in Development; use `synthetic-admin` for the seeded business. The server checks business ownership and validates the selected plan against the configured PayPal plan before returning checkout data.

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/v1/plans` | Read the plan catalog |
| GET | `/api/v1/businesses/{businessId}/subscription` | Read the current subscription |
| POST | `/api/v1/businesses/{businessId}/subscription/checkout` | Create a checkout intent; body: `{"planId":"essential"}` |
| POST | `/api/v1/businesses/{businessId}/subscription/verification` | Verify an approved PayPal subscription; body: `{"checkoutId":"…","subscriptionId":"…"}` |
| POST | `/api/v1/businesses/{businessId}/subscription/cancellation` | Schedule cancellation at the current period end |
| GET | `/api/v1/businesses/{businessId}/billing-history` | Read recorded payment transactions |
| POST | `/api/v1/businesses/{businessId}/subscription/plan-change` | Create a plan-change intent; body: `{"planId":"professional"}` |
| POST | `/api/v1/businesses/{businessId}/subscription/plan-change/verification` | Verify the PayPal plan change; body: `{"checkoutId":"…"}` |
| POST | `/api/v1/payments/paypal/webhook` | Verify and reconcile supported PayPal webhook events |

Configure PayPal outside source control with `PayPal__ClientId`, `PayPal__ClientSecret`, and `PayPal__WebhookId`. `PayPal__ApiBaseUrl` can override the API host; Development defaults to PayPal Sandbox and other environments default to PayPal Live. The webhook endpoint requires the real webhook ID from the corresponding PayPal environment; it returns `503` when that ID is not configured. Do not send `User-Id` as webhook authentication: PayPal transmission headers and server-side signature verification are required. Checkout creation does not itself create a PayPal subscription or charge a payment; the buyer approval and subsequent server verification are separate steps.

## Verification

```powershell
dotnet build instalert-platform.sln
```
