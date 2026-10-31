using System.Text.Json;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Scalar.AspNetCore;
using InstAlertPlatform.API.Business.Application.Internal.CommandServices;
using InstAlertPlatform.API.Business.Application.Internal.QueryServices;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using InstAlertPlatform.API.Business.Interfaces.Rest.Transform;

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
    options.AddSchemaTransformer(LocalizedOpenApiAssembler.TransformSchemaAsync);
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
builder.Services.AddScoped<IBusinessProfileCommandService, BusinessProfileCommandService>();
builder.Services.AddScoped<IBusinessProfileQueryService, BusinessProfileQueryService>();
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
