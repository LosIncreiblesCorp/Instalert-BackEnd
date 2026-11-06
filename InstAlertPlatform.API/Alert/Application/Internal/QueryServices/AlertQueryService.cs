using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Alert.Application.Internal.QueryServices;
public sealed class AlertQueryService(IAlertRecordRepository records, IOutboxMessageRepository outbox, IBusinessProfileQueryService business, ICurrentActor actor) : IAlertQueryService
{
    public async Task<List<AlertFact>> Handle(GetAlertsQuery query, CancellationToken cancellationToken = default)
    {
        var businessId = query.BusinessId;
        var status = query.Status;
        var page = query.Page;
        var pageSize = query.PageSize;
        await business.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, false), cancellationToken);
        Guard.Require(page >= 1 && pageSize is >= 1 and <= 100, "validation", "pagination");
        return (await records.FindByBusinessIdAsync(businessId, cancellationToken)).Where(a => status is null || a.Status == status).OrderByDescending(a => a.CreatedAt).Skip((int)Math.Min(int.MaxValue, ((long)page - 1) * pageSize)).Take(pageSize).Select(ToAlertFact).ToList();
    }

    public async Task<AlertFact> Handle(GetAlertByIdQuery query, CancellationToken cancellationToken = default)
    {
        var id = query.Id;
        var a = await records.FindByIdAsync(id, cancellationToken) ?? throw new DomainException("not_found");
        await business.Handle(new RequireBusinessAccessQuery(a.BusinessId, actor.UserId, false), cancellationToken);
        return ToAlertFact(a);
    }

    public async Task<List<AlertFact>> Handle(GetAllAlertFactsQuery query, CancellationToken cancellationToken = default)
    {
        return (await records.ListAsync(cancellationToken)).Select(ToAlertFact).ToList();
    }

    public async Task<List<AlertFact>> Handle(GetAlertFactsAtQuery query, CancellationToken cancellationToken = default)
    {
        var cutoff = query.Cutoff;
        var events = await outbox.FindByEventTypeAsync("alert.changed.v1", cancellationToken);
        return events.Where(e => e.CreatedAt < cutoff).Select(e => System.Text.Json.JsonSerializer.Deserialize<AlertFact>(e.Payload)!).GroupBy(e => e.Id).Select(g => g.MaxBy(e => e.Revision)!).ToList();
    }

    private static AlertFact ToAlertFact(AlertRecord a) => new(a.Id, a.BusinessId, a.EmployeeId, a.Kind, a.Status, a.ReportStatus, a.Category, a.Description, a.Location, a.Latitude, a.Longitude, a.Moment, a.CreatedAt ?? DateTimeOffset.MinValue, a.ActivatedAt, a.ResolvedAt, a.CompletedAt, a.Revision);
}
