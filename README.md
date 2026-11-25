# InstAlert Backend

InstAlert is an incident management application for business administrators and operational employees. This repository contains its REST API, built with C#, ASP.NET Core 10, Entity Framework Core and MySQL. The frontend is maintained in a separate repository.

## Requirements

- .NET 10 SDK.
- MySQL with a running server and a database user allowed to create and update the application schema.
- EF Core CLI version 10.0.13.
- For the complete user journey: a PayPal Sandbox application and buyer account, and a Brevo transactional email account with a verified sender.

Run all commands below from the repository root, where `instalert-platform.sln` is located. Examples use PowerShell. Replace placeholders before executing them; do not wrap the whole command block in quotes.

```powershell
dotnet --version
dotnet tool install --global dotnet-ef --version 10.0.13
```

If the EF tool is already installed, use `dotnet tool update --global dotnet-ef --version 10.0.13` instead.

## 1. Configure the local API

ASP.NET loads `appsettings.json`, then the environment-specific file, then environment variables. Environment variables override file values; a double underscore represents a nested JSON property. The HTTP launch profile selects Development.

Set these values in the same terminal that will run migrations and start the API:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Port=3306;Database=instalert_platform;User ID=<MYSQL_USER>;Password=<MYSQL_PASSWORD>;"
$env:TokenSettings__Secret = "<PRIVATE_RANDOM_KEY_OF_AT_LEAST_32_BYTES>"
$env:TokenSettings__Issuer = "InstAlert"
$env:TokenSettings__Audience = "InstAlert.Web"
$env:TokenSettings__AccessTokenMinutes = "10080"
$env:Demo__Enabled = "false"
$env:DevelopmentActors__Enabled = "false"
$env:DevelopmentData__SeedOnStartup = "false"
$env:Database__ApplyMigrationsOnStartup = "false"
$env:Documentation__Enabled = "true"
$env:Workers__Enabled = "false"
$env:Email__Enabled = "false"
$env:Payments__Reconciliation__Enabled = "false"
```

Generate your own signing key, for example with PowerShell, and use the resulting value as `TokenSettings__Secret`:

```powershell
$keyBytes = New-Object byte[] 64
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($keyBytes)
[Convert]::ToBase64String($keyBytes)
$rng.Dispose()
```

Keep the key private and stable across restarts. Tokens last seven days and can also be revoked by the application. Issuer and audience identify the API and its client; they are not URLs or passwords.

The initial configuration disables email and background processing so you can prepare the database without dispatching queued messages. These terminal variables are temporary. You can also configure the existing Development settings privately; do not commit credentials.

## 2. Prepare the database and start

Make sure MySQL is running and the configured user can create the database, or create an empty `instalert_platform` database first. Then run:

```powershell
dotnet restore instalert-platform.sln
dotnet ef database update --project InstAlertPlatform.API -- --environment Development
dotnet run --project InstAlertPlatform.API --launch-profile http
```

Use the existing migrations; do not generate a new migration just to run the project. When upgrading a database containing data, review pending migrations and back it up first. Automatic startup migration is disabled by the overrides above.

| Address | Purpose |
| --- | --- |
| `http://localhost:5221` | API host |
| `http://localhost:5221/scalar/` | Interactive API documentation |
| `http://localhost:5221/openapi/v1.json` | OpenAPI specification |
| `http://localhost:5221/health/live` | Process health |
| `http://localhost:5221/health/ready` | Database connectivity and pending migration check |

A ready API returns 200 from `/health/ready`. This does not verify PayPal or Brevo credentials. Stop the API with Ctrl+C before changing its terminal configuration, then start it again.

## 3. Enable the complete application flow

### PayPal Sandbox

Set the following in the API terminal before starting it:

```powershell
$env:PayPal__ClientId = "<SANDBOX_CLIENT_ID>"
$env:PayPal__ClientSecret = "<SANDBOX_CLIENT_SECRET>"
$env:PayPal__Plans__essential = "<ESSENTIAL_SANDBOX_PLAN_ID>"
$env:PayPal__Plans__professional = "<PROFESSIONAL_SANDBOX_PLAN_ID>"
$env:PayPal__Plans__business = "<BUSINESS_SANDBOX_PLAN_ID>"
$env:Payments__Reconciliation__Enabled = "true"
$env:Payments__Reconciliation__IntervalSeconds = "300"
```

Use active monthly plans from the same Sandbox application: Essential USD 15 / 4 employee seats, Professional USD 30 / 8 seats, Business USD 45 / 15 seats. The database catalog includes provider IDs; override them when using your own application. Configure the same public Client ID in the frontend. Use a Sandbox personal buyer account to approve subscriptions; no real payment is required.

The backend verifies approval and periodically reconciles subscriptions. A webhook is optional. To use it, register a reachable HTTPS URL ending in `/api/v1/payments/paypal/webhook` in that Sandbox application and set `PayPal__WebhookId` to its webhook registration ID. For local callbacks, a public tunnel must forward to this API. The webhook ID is not the hostname or Render service ID.

### Brevo and background processing

```powershell
$env:Brevo__ApiKey = "<TRANSACTIONAL_API_KEY>"
$env:Email__FromAddress = "<VERIFIED_SENDER_EMAIL>"
$env:Email__FromName = "InstAlert"
$env:Workers__Enabled = "true"
$env:Email__Enabled = "true"
```

Workers process queued events and deliveries. In-app notifications require workers; email additionally requires Brevo and Email configuration. Enabling workers/email may process existing queued messages. Invitation emails contain a temporary password and setup instructions, without a sign-in link. The password expires after 24 hours; the employee must replace it before completing their profile and accepting the invitation.

### Frontend and CORS

Run the separate frontend at `http://localhost:5173` with `VITE_INSTALERT_PLATFORM_API_URL=http://localhost:5221/api/v1`. Development already allows that origin. For another frontend address, configure its exact origin, for example:

```powershell
$env:Cors__AllowedOrigins__0 = "http://localhost:5173"
```

An origin contains scheme, hostname and port, without a route such as `/api/v1`. Restart the API after changing configuration.

## 4. Try the user journey

1. Register an administrator through the frontend or `POST /api/v1/users` with `email` and `password`. Passwords need at least ten characters, a letter and a digit, and must fit within 72 UTF-8 bytes.
2. Sign in using `POST /api/v1/tokens`. For protected API calls, use `Authorization: Bearer <accessToken>`; in Scalar, enter the token in its Bearer authentication control.
3. Register the administrator's single business and complete their personal profile.
4. Select a plan, approve it with a Sandbox buyer and wait for server verification. Registration alone does not unlock operational screens.
5. Invite an employee with a reachable email. The invitation reserves a seat. The employee signs in with the temporary password, changes it, completes their profile and accepts the invitation.
6. Use alerts, mapping, notifications and dashboards. Employees manage their own emergency contacts.

There are no seeded login accounts to use for this flow. `GET /api/v1/sessions/current` reports the next onboarding step and current access. A user returning midway resumes the incomplete step. Each administrator owns one business and each employee remains associated with one business, even if deactivated.

## Production configuration

The Dockerfile builds the API and listens on `0.0.0.0:$PORT`, defaulting to port 10000. Build from the repository root with `docker build -t instalert-api .`. The image does not include Development settings. Docker hosts the API; MySQL must be provided separately.

For Render with Aiven, set `ASPNETCORE_ENVIRONMENT=Production`, the complete `ConnectionStrings__DefaultConnection` including Aiven's assigned port and TLS settings, and the JWT/provider variables above in the hosting environment. If the connection references a CA certificate, provide it at a path readable inside the container. Do not use a local Windows certificate path in Render.

Keep Demo, DevelopmentActors and DevelopmentData seeding disabled. Apply migrations explicitly before starting the new version; Docker build does not apply them. Set the health check to `/health/ready` and CORS to the exact published frontend origin. The Production defaults allow `https://instalert-c3f7f.web.app` and `https://instalert-c3f7f.firebaseapp.com`. Keep PayPal in Sandbox, even when ASP.NET runs in Production. Background processing pauses whenever the hosting instance is stopped or sleeping.

## Troubleshooting

| Symptom | Check |
| --- | --- |
| API fails at startup with token configuration errors | Supply a private signing key of at least 32 bytes, issuer and audience. |
| Database connection fails | Check MySQL is running, connection host/port, credentials and TLS configuration. |
| Readiness returns 503 | Check database access and apply pending migrations to that same database. |
| Protected endpoint returns 401 | Sign in and use a current Bearer token. Password changes and revocation invalidate old tokens. |
| Protected endpoint returns 403 | Check the user role, completed onboarding, membership and subscription. |
| Browser reports a CORS error | Check the API is reachable and the exact frontend origin is allowed. |
| Invitation email does not arrive | Check workers, Email, Brevo sender/key, delivery status and spam folder. |
| Subscription remains pending | Check Sandbox credentials, plan ownership and server verification/reconciliation. |

## Architecture and API reference

Nine bounded contexts organize the application: Iam, Business, Profiles, Alert, Contacts, Notifications, Payments, Mapping and Dashboard. Shared contains technical infrastructure. Each context separates Domain, Application, Infrastructure and Interfaces. The shared AppDbContext composes each context's EF mappings; repositories and unit of work persist changes. Commands change state, queries read it, and REST resources/assemblers define the HTTP contracts. Provider adapters stay in Infrastructure; events and the outbox coordinate asynchronous processing.

The following sections describe the principal API contracts. Scalar provides the generated operation and request schemas. English is the default; send `Accept-Language: es-419` for supported Spanish messages and operation documentation.

## Business profile API

| Method | Route | Operation |
|---|---|---|
| GET | `/api/v1/businesses` | List the caller's businesses |
| POST | `/api/v1/businesses` | Register a business |
| GET | `/api/v1/businesses/{id}` | Read an owned business |
| PUT | `/api/v1/businesses/{id}` | Update its name, address and coordinates |

Registration requires an eleven-digit textual `taxId`, `name`, `address`, `latitude` and `longitude`. Name and address limits are 150 and 250 characters. Coordinates must be finite and within latitude/longitude bounds. The registered tax identifier is immutable. The database enforces its uniqueness. Business profile responses omit the internal concurrency stamp; EF Core retains it in persistence. Public IDs are serialized as strings; audit dates are assigned by the server and stored in UTC. Registration returns 201 and a Location header; empty lists return 200. Invalid requests return localized ProblemDetails, with field errors for model validation.

Public registration creates administrator IAM accounts. Protected routes use Bearer JWT tokens. Keep `Demo__Enabled=false`, `DevelopmentActors__Enabled=false` and `DevelopmentData__SeedOnStartup=false`.

Use `GET /api/v1/sessions/current` and `GET /api/v1/businesses` to obtain the authenticated commerce ID. `GET /api/v1/business-members/me` returns the employee association. There is no role or commerce selector for unrelated accounts.

For read-only connection checks, disable `Database__ApplyMigrationsOnStartup`, `DevelopmentData__SeedOnStartup`, `Workers__Enabled` and `Email__Enabled`.

## Staff and personal profiles

Business owns operational memberships and invitation history. An administrator owns one commerce; an employee remains associated with one commerce even while inactive. Registration checks ownership, acceptance checks all memberships, and unique database indexes enforce owner and employee assignment. A normalized pending invitation email can reserve only one commerce at a time; cancellation releases that reservation. Membership deactivation releases the operational seat without deleting the association.

The `EnforceSingleCommerceAssignments` migration adds these unique indexes and a generated pending-email column. Existing duplicate owners, memberships or pending reservations must be reviewed before applying it. The migration does not delete or transfer records to resolve conflicts.

| Method | Route | Operation |
|---|---|---|
| GET | `/api/v1/businesses/{id}/seats` | Read server-calculated capacity |
| GET | `/api/v1/businesses/{id}/members` | Read staff |
| GET/POST | `/api/v1/businesses/{id}/invitations` | Read or create invitations |
| POST | `/api/v1/business-members/{id}/activation` | Activate with available capacity |
| POST | `/api/v1/business-members/{id}/deactivation` | Deactivate while preserving the association |
| POST | `/api/v1/staff-invitations/{id}/cancellation` | Cancel and release the reserved seat |
| POST | `/api/v1/staff-invitations/{id}/resend` | Request delivery again |
| GET/PUT | `/api/v1/profiles/{userId}` | Read or update the caller's personal data |
| GET/PUT | `/api/v1/profiles/{userId}/preferences` | Read or update the caller's locale |

The server checks commerce ownership, capacity, transitions and duplicate email reservations. Personal profiles preserve the textual eight-digit DNI; the optional phone is normalized to 7–15 digits with an optional leading `+`. Invitations provision employee IAM accounts with protected temporary credentials. Acceptance requires a personal password and complete profile.

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

Cancelling the browser countdown discards its preparation and does not call confirmation. Preparations do not persist alerts, publish alert events or queue deliveries. Confirmation verifies the protected user/commerce scope and server time; retrying a previously confirmed token returns that record, including when its preparation period has expired. An unused expired token is rejected. Public alert responses omit event revisions; internal alert facts retain them for ordered Mapping and Notifications processing.

Commerce members may read history; only an alert's creator can modify its location, resolve it or complete its report. Resolution is idempotent, completed reports cannot be edited, and occurrence dates must be within the MySQL UTC range and not in the future. Alert events are published only after persisted changes; external notification delivery is handled by Notifications.

## Payments API

Business subscription routes require an administrator bearer token, a personal password and ownership of the commerce. Checkout also requires a complete personal profile. Public registration creates administrators; it does not activate a subscription.

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

Configure `PayPal__ClientId` and `PayPal__ClientSecret` outside source control. The API always uses PayPal Sandbox, including Production. Optional `PayPal__Plans__essential`, `PayPal__Plans__professional` and `PayPal__Plans__business` override the catalog's provider plan IDs; each must be an active monthly USD plan with the matching price.

Checkout creates a provider subscription awaiting buyer approval and stores its ID. It never grants access on its own. The server verifies the provider plan, correlation, completed payment, paid period and seat capacity before activation. The browser receives only public checkout data.

Enable periodic synchronization with `Payments__Reconciliation__Enabled=true`; `Payments__Reconciliation__IntervalSeconds` defaults to 300 (30–3600). This worker checks pending approvals and known subscriptions using a separate scope per item. It does not depend on a webhook. The optional webhook additionally requires `PayPal__WebhookId` from the same Sandbox application.

Cancellation suspends future renewals and retains the paid period. `POST /api/v1/businesses/{businessId}/subscription/renewal` resumes a suspended agreement before that period expires. The worker finally cancels a scheduled agreement after expiry. Plan changes apply only after PayPal confirms the target plan and capacity is checked again; verifying a revision does not extend the paid period.

## Emergency contacts API

The operational employee manages only their own emergency contacts. These routes require an employee Bearer token, complete onboarding, an active membership and a current subscription. An administrator does not manage the employee personal contact list.

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/v1/emergency-contacts` | List the caller's contacts |
| POST | `/api/v1/emergency-contacts` | Create a contact |
| GET | `/api/v1/emergency-contacts/{id}` | Read an owned contact |
| PUT | `/api/v1/emergency-contacts/{id}` | Update an owned contact |
| DELETE | `/api/v1/emergency-contacts/{id}` | Delete an owned contact |

Create/update JSON uses `fullName`, `relationship`, and `phone`; `email` and `notes` are optional. The caller determines ownership; request bodies do not choose an employee or provide audit timestamps. Names and relationships are trimmed, phones are normalized to 7–15 digits with an optional leading `+`, empty email becomes `null`, and notes are limited to 140 characters. Inactive memberships receive 403 for reads and writes, while foreign or missing contact IDs return 404. Invalid updates preserve the saved record. The server validates these fields and returns 201 on creation, 200 on reads/updates, and 204 on deletion. Missing identity returns 401, a non-operational user receives 403 for list/create, an unknown or another user's contact returns 404, and invalid data returns 400. An optional email address is stored as contact data; this feature does not send notifications. Delivery is handled by Notifications.

## Notifications API

The inbox is scoped to the caller and an accessible commerce. Both authenticated roles use the same contract; inactive memberships cannot read or update their former commerce's notifications or preferences.

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/v1/notifications?businessId={id}&page=1&pageSize=100` | Read the caller's paginated inbox |
| POST | `/api/v1/notifications/{id}/reading` | Mark a received notification as read |
| GET/PUT | `/api/v1/notification-preferences/{userId}?businessId={id}` | Read or save the caller's nearby-alert preferences |
| GET | `/api/v1/notifications/deliveries` | Read the caller's external delivery states |

Inbox pages use newest creation time first and ID as a stable tie-breaker, with page sizes from 1 to 100. Reading is idempotent: the server assigns the first read time and rejects another recipient's ID with 404. Preference changes contain only `nearbyAlertsEnabled` and `nearbyRadiusMeters`, accepting 100, 500 or 1000 meters. Invalid changes preserve the previous configuration.

`Workers__Enabled=true` enables processing of committed alert events into in-app notifications. The backend determines recipients, active membership, preferences and distance from saved commerce coordinates; the browser does not create these records. Receipt is retained after a panic is resolved or preferences change. Email delivery additionally requires the separate Brevo and Email configuration; disabling Email does not disable the in-app inbox.

## Identity and access

IAM uses BCrypt password hashes and signed JWT bearer tokens. Public registration creates administrator accounts only; it does not create a commerce, subscription or employee membership. Employee credentials are prepared through the internal IAM facade for the invitation workflow.

| Method | Route | Purpose |
| --- | --- | --- |
| POST | `/api/v1/users` | Register an administrator with `email` and `password` |
| POST | `/api/v1/tokens` | Sign in with `email` and `password` |
| GET | `/api/v1/sessions/current` | Read the authenticated identity and onboarding status |
| PUT | `/api/v1/users/me/password` | Change a password using `currentPassword` and `newPassword` |
| DELETE | `/api/v1/tokens/current` | Revoke the account's existing access tokens |

Registration and sign-in are anonymous and rate limited. The other IAM endpoints require `Authorization: Bearer <accessToken>`; a demonstration `User-Id` header does not grant access to them. Password changes return a replacement token and invalidate previously issued tokens. Passwords require at least ten characters, a letter and a digit, and cannot exceed 72 UTF-8 bytes.

Configure these environment variables before starting the API:

```text
TokenSettings__Secret
TokenSettings__Issuer
TokenSettings__Audience
TokenSettings__AccessTokenMinutes
```

The signing key must contain at least 32 bytes. Access token duration defaults to 10080 minutes (7 days) and accepts 5 through 10080 minutes. Keep the signing key outside source control. Apply all existing migrations to the intended database before running the API.

Temporary employee credentials expire after 24 hours. IAM stores their hash and protects the temporary delivery payload with ASP.NET Core Data Protection. Existing personal passwords are never recovered or included in invitation messages. Set `DataProtection__KeysPath` to a writable persistent key directory and protect that directory before enabling delivery across restarts or multiple instances.

IAM is integrated with administrator onboarding, employee invitations and protected modules. The session's `hasApplicationAccess` value reflects profile completion, membership and paid entitlement; operational services enforce that access in addition to resource ownership.


## Employee invitations and access

Invitation and membership operations use bearer sessions. The administrator invites employees only into their own commerce and with a current subscription and an available seat. A pending invitation reserves one seat. IAM provisions an employee account or rotates its temporary credential; personal passwords are never replaced by an invitation resend. Pending invitations reserve the email across commerces, and inactive memberships retain their original association.

Employees change their temporary password, complete their personal profile, then accept an invitation addressed to their account email. `GET /api/v1/staff-invitations/me` lists only those pending invitations and includes the commerce name. Acceptance checks identity, profile, subscription and capacity again.

The administrator can deactivate or reactivate a membership, but cannot edit an incorporated employee's name, email or password. Correct a pending invitation's email by cancelling it and creating another invitation. Deactivation releases a seat without moving or deleting the employee's account; reactivation requires a valid subscription and capacity.

## Invitation credential delivery

Invitations carry a protected temporary credential through the outbox and email queue. Notifications checks the invitation email, pending state, delivery sequence, credential expiry and IAM security stamp immediately before sending. Cancelled or superseded deliveries are skipped. Credentials are cleared from completed delivery records. Personal passwords are never sent. Invitation emails contain the temporary password and setup instructions, without a sign-in link. Existing accounts with a personal password receive instructions to use that password.

Configure `Workers__Enabled=true`, `Email__Enabled=true`, `Brevo__ApiKey` and `Email__FromAddress` outside source control. `Email__FromName` is optional. Retain a persistent writable `DataProtection__KeysPath` across restarts so queued credentials can be opened by the delivery worker. Temporary passwords expire 24 hours after issuance; the invitation and reserved seat remain pending until accepted or cancelled. Resending rotates a provisional credential and does not reserve another seat.

## Project layout

Each bounded context keeps command and query service contracts in `Application/CommandServices` and `Application/QueryServices`, and implementations in `Application/Internal`. Read models are grouped under `Application/Services/Snapshots`; provider contracts and their message types are under `Application/Internal/OutboundServices`.

Published event contracts belong to the producing context's `Domain/Model/Events`. Consumers handle them under `Application/Internal/EventHandlers`. Integration messages retain the `alert.changed.v1` and `business.invitation.requested.v1` identifiers and their existing JSON fields. The shared outbox persists and dispatches these messages from `Shared/Infrastructure/Persistence/EntityFrameworkCore/Outbox`.

ACL interfaces live under `Interfaces/Acl`, with their implementations under `Application/Acl`. Provider adapters remain in Infrastructure: PayPal and Brevo services have dedicated directories, and hosted workers are grouped under `Infrastructure/BackgroundServices`. The shared `AppDbContext` continues to compose each context's ORM configuration.
