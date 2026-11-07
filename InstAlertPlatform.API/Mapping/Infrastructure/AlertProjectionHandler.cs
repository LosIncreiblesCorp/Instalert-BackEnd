using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Mapping.Application.Errors;
using System.Text.Json;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Mapping.Application.Services;
using InstAlertPlatform.API.Mapping.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Mapping.Infrastructure;
public sealed class AlertProjectionHandler(IAlertProjectionCommandService commands) : IIntegrationEventHandler
{
    public string EventType => "alert.changed.v1";

    public async Task Handle(string payload, CancellationToken token)
    {
        var fact = JsonSerializer.Deserialize<AlertFact>(payload) ?? throw new InvalidOperationException("Invalid integration fact.");
        var projectionResult = (await commands.Handle(new ProjectAlertCommand(fact.Id, fact.BusinessId, fact.Kind, fact.Status, fact.ReportStatus, fact.Category, fact.Description, fact.Latitude, fact.Longitude, fact.CreatedAt, fact.Revision), token));
        if (projectionResult is Result<bool, EProjectAlertError>.Failure projectionFailure)
            throw new DomainException(projectionFailure.Error switch
            {
                EProjectAlertError.InvalidInput => "validation",
                EProjectAlertError.Forbidden => "forbidden",
                EProjectAlertError.NotFound => "not_found",
                EProjectAlertError.Conflict => "conflict",
                EProjectAlertError.ConcurrencyConflict => "concurrency",
                EProjectAlertError.UnexpectedError => "unexpected",
                _ => "unexpected"
            }, projectionFailure.Field);
    }
}
