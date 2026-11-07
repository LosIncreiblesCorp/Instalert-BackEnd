using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Mapping.Domain.Repositories;
using System.Text.Json;
using InstAlertPlatform.API.Mapping.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using InstAlertPlatform.API.Mapping.Application.Services;
using InstAlertPlatform.API.Mapping.Application.Errors;
using InstAlertPlatform.API.Mapping.Domain.Model.Commands;

namespace InstAlertPlatform.API.Mapping.Application.Internal.CommandServices;
public sealed class AlertProjectionCommandService(IIncidentProjectionRepository incidents) : IAlertProjectionCommandService
{
    public async Task<Result<bool, EProjectAlertError>> Handle(ProjectAlertCommand command, CancellationToken token = default)
    {
        try
        {
            var fact = command;
            var p = await incidents.FindByIdAsync(fact.Id, token);
            if (p is null)
            {
                p = new IncidentProjection(fact.Id);
                await incidents.AddAsync(p, token);
            }

            p.Apply(fact.Revision, fact.BusinessId, fact.Kind, fact.Status, fact.ReportStatus, fact.Category, fact.Description, fact.Latitude, fact.Longitude, fact.CreatedAt);
            return new Result<bool, EProjectAlertError>.Success(true);
        }
        catch (DomainException error)
        {
            return new Result<bool, EProjectAlertError>.Failure(error.Code switch
            {
                "validation" => EProjectAlertError.InvalidInput,
                "forbidden" => EProjectAlertError.Forbidden,
                "not_found" => EProjectAlertError.NotFound,
                "conflict" => EProjectAlertError.Conflict,
                "concurrency" => EProjectAlertError.ConcurrencyConflict,
                "unexpected" => EProjectAlertError.UnexpectedError,
                _ => EProjectAlertError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }
}
