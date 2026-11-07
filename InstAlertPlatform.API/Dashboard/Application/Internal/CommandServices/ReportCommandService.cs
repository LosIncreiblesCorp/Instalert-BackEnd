using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Dashboard.Application.Errors;
using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Dashboard.Application.Internal.QueryServices;
using InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
using InstAlertPlatform.API.Dashboard.Domain.Model.Commands;
using InstAlertPlatform.API.Dashboard.Domain.Model.Queries;
using InstAlertPlatform.API.Dashboard.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using System.Text.Json;

namespace InstAlertPlatform.API.Dashboard.Application.Internal.CommandServices;
public sealed class ReportCommandService(IBusinessProfileQueryService business, IReportRepository reports, ICurrentActor actor, IUnitOfWork work, TimeProvider clock, JsonSerializerOptions snapshotJsonOptions, DashboardProjectionBuilder projectionBuilder) : IReportCommandService
{
    public async Task<Result<ReportSnapshot, EGenerateReportError>> Handle(GenerateReportCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var b = await business.Handle(new RequireBusinessAccessQuery(command.BusinessId, actor.UserId, true), cancellationToken);
            var now = DateTimeOffset.FromUnixTimeMilliseconds(clock.GetUtcNow().ToUnixTimeMilliseconds());
            var bounds = ReportingPeriod.Range(command.StartDate, command.EndDate, now);
            var summary = await projectionBuilder.ComposeReport(b, actor.UserId, bounds, cancellationToken);
            var report = new Report(command.BusinessId, command.StartDate, command.EndDate, bounds.End,
                JsonSerializer.Serialize(summary, snapshotJsonOptions), now, actor.UserId);
            await reports.AddAsync(report, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<ReportSnapshot, EGenerateReportError>.Success(ToReportSnapshot(report));
        }
        catch (DomainException error)
        {
            return new Result<ReportSnapshot, EGenerateReportError>.Failure(error.Code switch
            {
                "validation" => EGenerateReportError.InvalidInput,
                "unauthenticated" => EGenerateReportError.Unauthenticated,
                "forbidden" => EGenerateReportError.Forbidden,
                "not_found" => EGenerateReportError.NotFound,
                "conflict" => EGenerateReportError.Conflict,
                "concurrency" => EGenerateReportError.ConcurrencyConflict,
                "transition" => EGenerateReportError.InvalidTransition,
                "policy_unavailable" => EGenerateReportError.PolicyUnavailable,
                "unexpected" => EGenerateReportError.UnexpectedError,
                _ => EGenerateReportError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    private static ReportSnapshot ToReportSnapshot(Report r) => new(r.Id, r.BusinessId, r.StartDate, r.EndDate, r.CutoffAt, r.GeneratedAt, r.GenerationSource, r.GeneratedByUserId, r.SchemaVersion, JsonSerializer.Deserialize<JsonElement>(r.SummaryJson));
}
