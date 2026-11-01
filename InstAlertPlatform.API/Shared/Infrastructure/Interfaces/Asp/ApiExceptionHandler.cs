using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstAlertPlatform.API.Resources;
using InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.Extensions;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.Extensions.Localization;

namespace InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp;
public sealed class ApiExceptionHandler(IProblemDetailsService problems, ILogger<ApiExceptionHandler> logger, IStringLocalizer<SharedResource> localizer) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        if (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
            return false;
        var code = exception switch
        {
            DomainException domain => domain.Code,
            BadHttpRequestException => "validation",
            _ => PersistenceExceptionClassifier.Classify(exception) ?? "unexpected"
        };
        var status = ProblemDetailsAssembler.StatusCode(code);
        if (status >= 500)
            logger.LogError(exception, "API failure {Code} {TraceId}", code, context.TraceIdentifier);
        context.Response.StatusCode = status;
        var field = exception is DomainException domainError ? domainError.Field : "";
        var problem = ProblemDetailsAssembler.ToProblemDetails(code, field, context, localizer);
        return await problems.TryWriteAsync(new ProblemDetailsContext { HttpContext = context, ProblemDetails = problem });
    }
}
