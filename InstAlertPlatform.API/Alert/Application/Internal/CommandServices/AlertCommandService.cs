using InstAlertPlatform.API.Alert.Application.Errors;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Domain.Repositories;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Alert.Application.Internal.CommandServices;
public sealed class AlertCommandService(IAlertRecordRepository records, IOutboxMessageRepository outbox, IBusinessProfileQueryService business, ICurrentActor actor, IUnitOfWork work, TimeProvider clock) : IAlertCommandService
{
    public async Task<Result<AlertFact, ECreateIncidentReportError>> Handle(CreateIncidentReportCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var c = command;
            await business.Handle(new RequireBusinessAccessQuery(c.BusinessId, actor.UserId, false), cancellationToken);
            var a = AlertRecord.Report(c.BusinessId, actor.UserId, c.Kind, c.Category, c.Description, c.Location, c.Latitude, c.Longitude, c.Moment, clock.GetUtcNow());
            await records.AddAsync(a, cancellationToken);
            await outbox.AddAsync(new OutboxMessage("alert.changed.v1", ToAlertFact(a)), cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<AlertFact, ECreateIncidentReportError>.Success(ToAlertFact(a));
        }
        catch (DomainException error)
        {
            return new Result<AlertFact, ECreateIncidentReportError>.Failure(error.Code switch
            {
                "validation" => ECreateIncidentReportError.InvalidInput,
                "unauthenticated" => ECreateIncidentReportError.Unauthenticated,
                "forbidden" => ECreateIncidentReportError.Forbidden,
                "not_found" => ECreateIncidentReportError.NotFound,
                "conflict" => ECreateIncidentReportError.Conflict,
                "concurrency" => ECreateIncidentReportError.ConcurrencyConflict,
                "transition" => ECreateIncidentReportError.InvalidTransition,
                "already_completed" => ECreateIncidentReportError.AlreadyCompleted,
                "unexpected" => ECreateIncidentReportError.UnexpectedError,
                _ => ECreateIncidentReportError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    private static AlertFact ToAlertFact(AlertRecord alert) => new(alert.Id, alert.BusinessId, alert.EmployeeId, alert.Kind, alert.Status, alert.ReportStatus, alert.Category, alert.Description, alert.Location, alert.Latitude, alert.Longitude, alert.Moment, alert.CreatedAt ?? DateTimeOffset.MinValue, alert.ActivatedAt, alert.ResolvedAt, alert.CompletedAt, alert.Revision);
}
