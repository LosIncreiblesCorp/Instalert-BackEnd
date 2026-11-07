using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Payments.Infrastructure;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore;
using InstAlertPlatform.API.Alert.Infrastructure;
using InstAlertPlatform.API.Notifications.Infrastructure;
using InstAlertPlatform.API.Notifications.Application.Internal.CommandServices;
using InstAlertPlatform.API.Notifications.Application.Internal.QueryServices;
using InstAlertPlatform.API.Notifications.Application.Services;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Profiles.Application.Internal.CommandServices;
using InstAlertPlatform.API.Profiles.Application.Internal.QueryServices;
using InstAlertPlatform.API.Contacts.Application.Internal.CommandServices;
using InstAlertPlatform.API.Contacts.Application.Internal.QueryServices;
using InstAlertPlatform.API.Business.Application.Internal.CommandServices;
using InstAlertPlatform.API.Business.Application.Internal.QueryServices;
using InstAlertPlatform.API.Alert.Application.Internal.QueryServices;
using InstAlertPlatform.API.Alert.Application.Internal.CommandServices;
using InstAlertPlatform.API.Payments.Application.Internal.QueryServices;
using InstAlertPlatform.API.Payments.Application.Internal.CommandServices;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Contacts.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Resources;
using Microsoft.OpenApi;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using System.Globalization;
using System.Text.Json;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    Converters = { new UtcIsoTimestampJsonConverter() }
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddScoped<ICurrentActor, HttpCurrentActor>();
builder.Services.AddSingleton<AuditableEntityInterceptor>();
builder.Services.AddDbContext<AppDbContext>((provider, options) =>
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");
    Guard.Require(!string.IsNullOrWhiteSpace(connection), "provider_unavailable");
    options.UseMySQL(Environment.ExpandEnvironmentVariables(connection!)).AddInterceptors(provider.GetRequiredService<AuditableEntityInterceptor>());
});
builder.Services.AddHttpClient("brevo", client =>
{
    client.BaseAddress = new Uri("https://api.brevo.com/v3/");
    client.Timeout = TimeSpan.FromSeconds(20);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
builder.Services.AddSingleton<IDeliveryGateway, DeliveryGateway>();
builder.Services.AddScoped<DeliveryProcessor>();
builder.Services.AddHostedService<DeliveryWorker>();
builder.Services.AddScoped<DevelopmentDataSeeder>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPanicPreparationTokens, PanicPreparationTokens>();
builder.Services.AddScoped<OutboxProcessor>();
builder.Services.AddHostedService<OutboxWorker>();
builder.Services.AddHttpClient<PayPalSubscriptions>(client =>
{
    client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com/");
    client.Timeout = TimeSpan.FromSeconds(20);
});
builder.Services.AddScoped<IPayPalWebhookVerifier>(p => p.GetRequiredService<PayPalSubscriptions>());
builder.Services.AddScoped<IPayPalSubscriptions>(p => p.GetRequiredService<PayPalSubscriptions>());
builder.Services.AddScoped<InstAlertPlatform.API.Alert.Domain.Repositories.IAlertPreferencesRepository, InstAlertPlatform.API.Alert.Infrastructure.Persistence.EntityFrameworkCore.Repositories.AlertPreferencesRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Alert.Domain.Repositories.IAlertRecordRepository, InstAlertPlatform.API.Alert.Infrastructure.Persistence.EntityFrameworkCore.Repositories.AlertRecordRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Notifications.Domain.Repositories.IDeliveryRequestRepository, InstAlertPlatform.API.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories.DeliveryRequestRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Notifications.Domain.Repositories.INotificationPreferencesRepository, InstAlertPlatform.API.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories.NotificationPreferencesRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Notifications.Domain.Repositories.IReceivedNotificationRepository, InstAlertPlatform.API.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories.ReceivedNotificationRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Mapping.Domain.Repositories.IIncidentProjectionRepository, InstAlertPlatform.API.Mapping.Infrastructure.Persistence.EntityFrameworkCore.Repositories.IncidentProjectionRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Business.Domain.Repositories.IBusinessMemberRepository, InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Repositories.BusinessMemberRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Business.Domain.Repositories.IBusinessProfileRepository, InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Repositories.BusinessProfileRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Payments.Domain.Repositories.ICheckoutIntentRepository, InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories.CheckoutIntentRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Contacts.Domain.Repositories.IEmergencyContactRepository, InstAlertPlatform.API.Contacts.Infrastructure.Persistence.EntityFrameworkCore.Repositories.EmergencyContactRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Shared.Domain.Repositories.IOutboxMessageRepository, InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories.OutboxMessageRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Payments.Domain.Repositories.IPayPalWebhookReceiptRepository, InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories.PayPalWebhookReceiptRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Payments.Domain.Repositories.IPaymentTransactionRepository, InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories.PaymentTransactionRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Payments.Domain.Repositories.IPlanRepository, InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories.PlanRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Business.Domain.Repositories.IStaffInvitationRepository, InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Repositories.StaffInvitationRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Payments.Domain.Repositories.ISubscriptionRepository, InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories.SubscriptionRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Profiles.Domain.Repositories.IUserProfileRepository, InstAlertPlatform.API.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories.UserProfileRepository>();
builder.Services.AddScoped<InstAlertPlatform.API.Profiles.Domain.Repositories.IUserPreferencesRepository, InstAlertPlatform.API.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories.UserPreferencesRepository>();
builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();
builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();
builder.Services.AddScoped<IEmergencyContactCommandService, EmergencyContactCommandService>();
builder.Services.AddScoped<IEmergencyContactQueryService, EmergencyContactQueryService>();
builder.Services.AddScoped<IBusinessProfileCommandService, BusinessProfileCommandService>();
builder.Services.AddScoped<IBusinessProfileQueryService, BusinessProfileQueryService>();
builder.Services.AddScoped<IBusinessMemberCommandService, BusinessMemberCommandService>();
builder.Services.AddScoped<IBusinessMemberQueryService, BusinessMemberQueryService>();
builder.Services.AddScoped<IStaffInvitationCommandService, StaffInvitationCommandService>();
builder.Services.AddScoped<IStaffInvitationQueryService, StaffInvitationQueryService>();
builder.Services.AddScoped<IAlertCommandService, AlertCommandService>();
builder.Services.AddScoped<IAlertQueryService, AlertQueryService>();
builder.Services.AddScoped<InstAlertPlatform.API.Mapping.Application.Services.IAlertProjectionCommandService, InstAlertPlatform.API.Mapping.Application.Internal.CommandServices.AlertProjectionCommandService>();
builder.Services.AddScoped<InstAlertPlatform.API.Mapping.Application.Services.IMappingQueryService, InstAlertPlatform.API.Mapping.Application.Internal.QueryServices.MappingQueryService>();
builder.Services.AddScoped<InstAlertPlatform.API.Shared.Application.Patterns.IIntegrationEventHandler, InstAlertPlatform.API.Mapping.Infrastructure.AlertProjectionHandler>();
builder.Services.AddScoped<IIntegrationEventHandler, NearbyPanicHandler>();
builder.Services.AddScoped<IIntegrationEventHandler, InvitationDeliveryHandler>();
builder.Services.AddScoped<INearbyPanicCommandService, NearbyPanicCommandService>();
builder.Services.AddScoped<IInvitationDeliveryCommandService, InvitationDeliveryCommandService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();
builder.Services.AddScoped<INotificationCommandService, NotificationCommandService>();
builder.Services.AddScoped<INotificationPreferencesQueryService, NotificationPreferencesQueryService>();
builder.Services.AddScoped<INotificationPreferencesCommandService, NotificationPreferencesCommandService>();
builder.Services.AddScoped<IAlertPreferencesQueryService, AlertPreferencesQueryService>();
builder.Services.AddScoped<IAlertPreferencesCommandService, AlertPreferencesCommandService>();
builder.Services.AddScoped<IPlanQueryService, PlanQueryService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<IPaymentReconciliationCommandService, PaymentReconciliationCommandService>();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IBusinessRevisionCommandService, BusinessRevisionCommandService>();
builder.Services.AddRouting(options => options.LowercaseUrls = false);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention())).AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new UtcIsoTimestampJsonConverter())).AddDataAnnotationsLocalization(options => options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource))).ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context =>
{
    var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
    var problem = new ValidationProblemDetails(context.ModelState)
    {
        Status = 400,
        Title = localizer["InvalidModelStateTitle"].Value,
        Extensions =
        {
            {
                "code",
                "validation"
            }
        }
    };
    var result = new BadRequestObjectResult(problem);
    result.ContentTypes.Add("application/problem+json");
    return result;
});
builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer(InstAlertPlatform.API.Alert.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Mapping.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Notifications.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Business.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Contacts.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Payments.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddOperationTransformer(InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform.LocalizedOpenApiAssembler.TransformOperationAsync);
    options.AddSchemaTransformer(OpenApiLocalizationAssembler.TransformSchemaAsync);
    options.AddOperationTransformer((operation, context, token) =>
    {
        if (builder.Environment.IsDevelopment())
        {
            operation.Parameters ??= [];
            operation.Parameters.Add(new OpenApiParameter { Name = "User-Id", In = ParameterLocation.Header, Required = false, Description = "Configured synthetic caller; accepted only when DevelopmentActors:Enabled=true in Development. Not production authentication.", Schema = new OpenApiSchema { Type = JsonSchemaType.String } });
        }

        return Task.CompletedTask;
    });
});
builder.Services.AddLocalization();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    if (origins.Length > 0)
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
}));
var app = builder.Build();
if (app.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
        app.Logger.LogWarning("Development migrations were skipped: configure ConnectionStrings:DefaultConnection and start MySQL.");
    else
    {
        using var migrationScope = app.Services.CreateScope();
        await migrationScope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }
}

var seedOnly = args.Contains("--seed-development");
if (seedOnly || app.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("DevelopmentData:SeedOnStartup"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>().Seed(CancellationToken.None);
    if (seedOnly) return;
}

var localization = new RequestLocalizationOptions().SetDefaultCulture("en-US").AddSupportedCultures("en-US", "es-419").AddSupportedUICultures("en-US", "es-419");
localization.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
{
    var requested = context.Request.Headers.AcceptLanguage.ToString().Split(',')[0].Split(';')[0].Replace('_', '-');
    return Task.FromResult<ProviderCultureResult?>(requested is "en-US" or "es-419" ? new ProviderCultureResult(requested) : null);
}));
localization.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localization);
app.UseExceptionHandler();
app.UseCors();
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Documentation:Enabled"))
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/swagger/index.html", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.MapGet("/health/live", () => Results.Ok(new { status = "alive" })).WithTags("Health");
app.MapGet("/health/ready", async (AppDbContext context, CancellationToken token) => await context.Database.CanConnectAsync(token) && !(await context.Database.GetPendingMigrationsAsync(token)).Any() ? Results.Ok(new { status = "ready" }) : Results.StatusCode(503)).WithTags("Health").Produces(200).Produces(503).Produces<ProblemDetails>(500);
app.MapControllers();
app.Run();
public partial class Program;
