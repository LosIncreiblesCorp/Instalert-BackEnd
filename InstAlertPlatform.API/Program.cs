using System.Text.Json;
using System.Text.Json.Nodes;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Seeders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using InstAlertPlatform.API.Business.Application.Internal.CommandServices;
using InstAlertPlatform.API.Business.Application.Internal.QueryServices;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using InstAlertPlatform.API.Business.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Profiles.Application.Internal.CommandServices;
using InstAlertPlatform.API.Profiles.Application.Internal.QueryServices;
using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Profiles.Domain.Repositories;
using InstAlertPlatform.API.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using InstAlertPlatform.API.Payments.Application.Internal.QueryServices;
using InstAlertPlatform.API.Payments.Application.Internal.CommandServices;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Repositories;
using InstAlertPlatform.API.Payments.Infrastructure;
using InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    Converters = { new UtcIsoTimestampJsonConverter() }
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentActor, HttpCurrentActor>();
builder.Services.AddSingleton<AuditableEntityInterceptor>();

builder.Services.AddLocalization();
builder.Services.AddRouting(options => options.LowercaseUrls = false);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new UtcIsoTimestampJsonConverter());
    })
    .AddDataAnnotationsLocalization(options =>
        options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource)))
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var localizer = context.HttpContext.RequestServices
                .GetRequiredService<IStringLocalizer<SharedResource>>();
            var factory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problem = factory.CreateValidationProblemDetails(
                context.HttpContext,
                context.ModelState,
                statusCode: StatusCodes.Status400BadRequest,
                title: localizer["InvalidModelStateTitle"].Value);
            problem.Extensions["code"] = "validation";
            var result = new BadRequestObjectResult(problem);
            result.ContentTypes.Add("application/problem+json");
            return result;
        };
    });

builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails.Status is null or >= 500)
        {
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
            context.ProblemDetails.Title = localizer["UnexpectedServerError"].Value;
            context.ProblemDetails.Detail = localizer["UnexpectedErrorProcessingRequest"].Value;
            context.ProblemDetails.Extensions["code"] = "unexpected";
        }
    };
});

builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer(LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Payments.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Alert.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    // TODO: luego reemplazar por token o IAM.
    if (builder.Environment.IsDevelopment())
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            foreach (var (path, pathItem) in document.Paths)
            {
                if (path.Equals("/api/v1/plans", StringComparison.OrdinalIgnoreCase) ||
                    path.Equals("/api/v1/payments/paypal/webhook", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (pathItem.Operations is not { } operations)
                    continue;

                foreach (var operation in operations.Values)
                {
                    operation.Parameters ??= [];
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "User-Id",
                        In = ParameterLocation.Header,
                        Required = true,
                        Description = "Synthetic development user ID.",
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Default = JsonValue.Create(
                                path.StartsWith("/api/v1/emergency-contacts", StringComparison.OrdinalIgnoreCase)
                                    ? "synthetic-employee"
                                    : "synthetic-admin")
                        }
                    });
                }
            }

            return Task.CompletedTask;
        });
    }
    options.AddSchemaTransformer(
        InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions.OpenApiLocalizationAssembler.TransformSchemaAsync);
});
builder.Services.AddDbContext<AppDbContext>((services, options) =>
{
    var template = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(template))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(template);
    options.UseMySQL(connectionString)
        .AddInterceptors(services.GetRequiredService<AuditableEntityInterceptor>())
        .UseLoggerFactory(services.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
builder.Services.AddScoped<IBusinessProfileRepository, BusinessProfileRepository>();
builder.Services.AddScoped<IBusinessMemberRepository, BusinessMemberRepository>();
builder.Services.AddScoped<IStaffInvitationRepository, StaffInvitationRepository>();
builder.Services.AddScoped<IBusinessProfileCommandService, BusinessProfileCommandService>();
builder.Services.AddScoped<IBusinessRevisionCommandService, BusinessRevisionCommandService>();
builder.Services.AddScoped<IBusinessProfileQueryService, BusinessProfileQueryService>();
builder.Services.AddScoped<IBusinessMemberQueryService, BusinessMemberQueryService>();
builder.Services.AddScoped<IBusinessMemberCommandService, BusinessMemberCommandService>();
builder.Services.AddScoped<IStaffInvitationQueryService, StaffInvitationQueryService>();
builder.Services.AddScoped<IStaffInvitationCommandService, StaffInvitationCommandService>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();
builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();
builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IPlanQueryService, PlanQueryService>();
builder.Services.AddHttpClient<PayPalSubscriptions>(client =>
{
    var configuredAddress = builder.Configuration["PayPal:ApiBaseUrl"];
    var defaultAddress = builder.Environment.IsDevelopment()
        ? "https://api-m.sandbox.paypal.com/"
        : "https://api-m.paypal.com/";
    var address = configuredAddress ?? defaultAddress;
    if (!Uri.TryCreate(address, UriKind.Absolute, out var baseAddress) ||
        baseAddress.Scheme != Uri.UriSchemeHttps ||
        !string.IsNullOrEmpty(baseAddress.UserInfo))
        throw new InvalidOperationException("PayPal:ApiBaseUrl must be an HTTPS URL without embedded credentials.");
    client.BaseAddress = baseAddress;
    client.Timeout = TimeSpan.FromSeconds(20);
});
builder.Services.AddScoped<IPayPalSubscriptions>(services => services.GetRequiredService<PayPalSubscriptions>());
builder.Services.AddScoped<IPayPalWebhookVerifier>(services => services.GetRequiredService<PayPalSubscriptions>());
builder.Services.AddScoped<ICheckoutIntentRepository, CheckoutIntentRepository>();
builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
builder.Services.AddScoped<IPayPalWebhookReceiptRepository, PayPalWebhookReceiptRepository>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
builder.Services.AddScoped<IPaymentReconciliationCommandService, PaymentReconciliationCommandService>();
builder.Services.AddScoped<InstAlertPlatform.API.Alert.Domain.Repositories.IAlertRecordRepository, InstAlertPlatform.API.Alert.Infrastructure.Persistence.EntityFrameworkCore.Repositories.AlertRecordRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Alert.Application.Services.IAlertCommandService, InstAlertPlatform.API.Alert.Application.Internal.CommandServices.AlertCommandService>();
builder.Services.AddScoped<InstAlertPlatform.API.Alert.Application.Services.IAlertQueryService, InstAlertPlatform.API.Alert.Application.Internal.QueryServices.AlertQueryService>();
builder.Services.AddScoped<InstAlertPlatform.API.Contacts.Domain.Repositories.IEmergencyContactRepository, InstAlertPlatform.API.Contacts.Infrastructure.Persistence.EntityFrameworkCore.Repositories.EmergencyContactRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Contacts.Application.Services.IEmergencyContactCommandService, InstAlertPlatform.API.Contacts.Application.Internal.CommandServices.EmergencyContactCommandService>();
builder.Services.AddScoped<InstAlertPlatform.API.Contacts.Application.Services.IEmergencyContactQueryService, InstAlertPlatform.API.Contacts.Application.Internal.QueryServices.EmergencyContactQueryService>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    if (origins.Length > 0) policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment() &&
    app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup") &&
    !string.IsNullOrWhiteSpace(app.Configuration.GetConnectionString("DefaultConnection")))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}

await DevelopmentDataSeeder.SeedAsync(app.Services);

string[] supportedCultures = ["en-US", "es-419"];
var localization = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localization.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localization);
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();
app.Run();
