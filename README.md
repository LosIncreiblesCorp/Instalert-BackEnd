# InstAlert Web Services

ASP.NET Core REST API using C#, EF Core and MySQL. The backend uses explicit command/query services, operation error enums, repositories, unit of work, REST resources and assemblers. Shared provides technical support. Business owns commerce profiles, employee memberships and invitations; personal data belongs to Profiles; subscriptions and billing records belong to Payments. This branch adds PayPal subscription checkout, server-side verification, plan changes, cancellation, billing history and webhook reconciliation.

## Run locally

Use the .NET 10 SDK, MySQL Server and the EF Core CLI. The API references EF Core 10.0.13; use the matching CLI version for migration tooling. The current local configuration targets `instalert_platform`. The server must be running and the configured database user must have the required database/schema permissions. Each team member must configure a connection for their own MySQL instance. Environment variables such as `ConnectionStrings__DefaultConnection` can override the file configuration.

```powershell
dotnet restore instalert-platform.sln
dotnet ef database update --project InstAlertPlatform.API
dotnet run --project InstAlertPlatform.API --launch-profile http
```

Configuration uses the standard ASP.NET files: `appsettings.json`, `appsettings.Development.json` and `appsettings.Production.json`. Configure the local MySQL connection and provider values in the existing Development settings. Environment variables and command-line arguments can override them. Production uses the `%DATABASE_URL%`, `%DATABASE_USER%`, `%DATABASE_PASSWORD%` and `%DATABASE_SCHEMA%` connection template, or `ConnectionStrings__DefaultConnection`.

The HTTP launch profile uses port 5221. Vite uses port 5173; Development CORS allows localhost and 127.0.0.1 on that port and the existing 5192/5193 ports. Set `Cors__AllowedOrigins__0` to the exact published frontend origin for a demonstration deployment. There is no default public deployment URL. Database migrations remain explicit outside Development; review `Database:ApplyMigrationsOnStartup`, workers and mail flags before running locally.

OpenAPI: `http://localhost:5221/openapi/v1.json`. Scalar: `http://localhost:5221/scalar/v1`. English (`en-US`) is the default; `Accept-Language: es-419` selects Latin American Spanish for messages and operation documentation.

## Business profile API

| Method | Route | Operation |
|---|---|---|
| GET | `/api/v1/businesses` | List the caller's businesses |
| POST | `/api/v1/businesses` | Register a business |
| GET | `/api/v1/businesses/{id}` | Read an owned business |
| PUT | `/api/v1/businesses/{id}` | Update its name, address and coordinates |

Registration requires an eleven-digit textual `taxId`, `name`, `address`, `latitude` and `longitude`. Name and address limits are 150 and 250 characters. Coordinates must be finite and within latitude/longitude bounds. The registered tax identifier is immutable. The database enforces its uniqueness. Public IDs are serialized as strings; audit dates are assigned by the server and stored in UTC. Registration returns 201 and a Location header; empty lists return 200. Invalid requests return localized ProblemDetails, with field errors for model validation.

This delivery uses two demonstration actors, `synthetic-admin` and `synthetic-employee`, selected through the `User-Id` header. In Development, `DevelopmentActors:Enabled=true` enables them. A published demonstration running in Production must explicitly set `Demo__Enabled=true`; with this flag disabled, Production rejects these headers. The selector is not authentication, and this delivery does not create IAM accounts or credentials. The actor lists and administrator role mapping are defined in the public base settings and continue to apply in either environment.

`DevelopmentData:SeedOnStartup` loads the two profiles, a single owned commerce, the employee membership and the existing simulated Essential entitlement. Alternatively, run `dotnet run --project InstAlertPlatform.API --no-launch-profile -- --environment Development --seed-development true` after applying migrations. Production seeding requires explicit demo mode; startup seeding also requires `DevelopmentData__SeedOnStartup=true`. The seed reuses the administrator's existing commerce, preserves an inactive membership and stops if the fixture employee is assigned elsewhere or fixture ownership is ambiguous. It does not move employees or remove existing data.

Use `GET /api/v1/businesses` for each demo actor to obtain the database-generated commerce ID. `GET /api/v1/business-members/me` returns the employee's association and status. The frontend connection context uses these existing endpoints rather than a fixed commerce ID.

For read-only connection checks, disable `Database__ApplyMigrationsOnStartup`, `DevelopmentData__SeedOnStartup`, `Workers__Enabled` and `Email__Enabled`.

## Staff and personal profiles

Business owns operational memberships and invitation history. An administrator owns one commerce; an employee remains associated with one commerce even while inactive. Registration checks ownership, acceptance checks all memberships, and unique database indexes enforce owner and employee assignment. A normalized pending invitation email can reserve only one commerce at a time; cancellation releases that reservation. Membership deactivation releases the operational seat without deleting the association.

The `EnforceSingleCommerceAssignments` migration adds these unique indexes and a generated pending-email column. Existing duplicate owners, memberships or pending reservations must be reviewed before applying it. The migration does not delete or transfer records to resolve conflicts.

| Method | Route | Operation |
|---|---|---|
| GET | `/api/v1/businesses/{id}/seats` | Read server-calculated capacity |
| GET | `/api/v1/businesses/{id}/members` | Read staff |
| GET/POST | `/api/v1/businesses/{id}/invitations` | Read or create invitations |
| PUT | `/api/v1/business-members/{id}` | Edit employee name and email |
| POST | `/api/v1/business-members/{id}/activation` | Activate with available capacity |
| POST | `/api/v1/business-members/{id}/deactivation` | Deactivate while preserving the association |
| PUT | `/api/v1/staff-invitations/{id}` | Edit a pending invitation |
| POST | `/api/v1/staff-invitations/{id}/cancellation` | Cancel and release the reserved seat |
| POST | `/api/v1/staff-invitations/{id}/resend` | Request delivery again |
| GET/PUT | `/api/v1/profiles/{userId}` | Read or update the caller's personal data |
| GET/PUT | `/api/v1/profiles/{userId}/preferences` | Read or update the caller's locale |

The server checks commerce ownership, capacity, transitions and duplicate email reservations. Personal profiles preserve the textual eight-digit DNI; the optional phone is normalized to 7–15 digits with an optional leading `+`. This demonstration keeps two fixed actors. Invitations do not create credentials; employee account provisioning requires IAM in a later delivery.

## Alert lifecycle

| Method | Route | Operation |
|---|---|---|
| GET | `/api/v1/alerts?businessId={id}&page=1&pageSize=100` | Read authorized commerce history |
| POST | `/api/v1/alerts/panic-preparations` | Issue a scoped preparation without creating an alert |
| POST | `/api/v1/alerts/panic` | Confirm after the server-issued grace period |
| PUT | `/api/v1/alerts/{id}/location` | Update the caller's active panic location |
| POST | `/api/v1/alerts/{id}/resolution` | Resolve the caller's panic, leaving its report pending |
| PUT | `/api/v1/alerts/{id}/report-draft` | Save details of the caller's pending report |
| POST | `/api/v1/alerts/{id}/report-completion` | Complete the report while retaining resolved status |
| POST | `/api/v1/alerts/reports` | Submit a past incident, suspicious activity or other situation |
| GET/PUT | `/api/v1/alert-preferences/{userId}` | Read or save the caller's 1–60 second grace period |

Cancelling the browser countdown discards its preparation and does not call confirmation. Preparations do not persist alerts, publish alert events or queue deliveries. Confirmation verifies the protected user/commerce scope and server time; retrying a previously confirmed token returns that record, including when its preparation period has expired. An unused expired token is rejected.

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

## Emergency contacts API

The operational employee manages only their own emergency contacts. In Development, send `User-Id: synthetic-employee`; the Scalar/OpenAPI default for these routes is configured accordingly. `synthetic-admin` is not an operational member and receives 403. The development identity header is not production authentication; replace it with IAM-backed identity before production use.

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/v1/emergency-contacts` | List the caller's contacts |
| POST | `/api/v1/emergency-contacts` | Create a contact |
| GET | `/api/v1/emergency-contacts/{id}` | Read an owned contact |
| PUT | `/api/v1/emergency-contacts/{id}` | Update an owned contact |
| DELETE | `/api/v1/emergency-contacts/{id}` | Delete an owned contact |

Create/update JSON uses `fullName`, `relationship`, and `phone`; `email` and `notes` are optional. The server validates these fields and returns 201 on creation, 200 on reads/updates, and 204 on deletion. Missing identity returns 401, a non-operational user receives 403 for list/create, an unknown or another user's contact returns 404, and invalid data returns 400. An optional email address is stored as contact data; this feature does not send notifications. Delivery is handled in F09.

## Verification

```powershell
dotnet build instalert-platform.sln
```
