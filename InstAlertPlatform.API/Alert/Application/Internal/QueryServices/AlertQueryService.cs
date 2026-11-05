using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Domain.Model.Queries;
using InstAlertPlatform.API.Alert.Domain.Repositories;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Alert.Application.Internal.QueryServices;
public sealed class AlertQueryService(IAlertRecordRepository records, IBusinessProfileQueryService business, ICurrentActor actor) : IAlertQueryService
{
    public async Task<List<AlertFact>> Handle(GetAlertsQuery query, CancellationToken cancellationToken = default)
    {
        await business.Handle(new RequireBusinessAccessQuery(query.BusinessId, actor.UserId, false), cancellationToken);
        Guard.Require(query.Page >= 1 && query.PageSize is >= 1 and <= 100, "validation", "pagination");
        return (await records.FindByBusinessIdAsync(query.BusinessId, cancellationToken))
            .Where(alert => query.Status is null || alert.Status == query.Status)
            .OrderByDescending(alert => alert.CreatedAt)
            .Skip((int)Math.Min(int.MaxValue, ((long)query.Page - 1) * query.PageSize))
            .Take(query.PageSize)
            .Select(ToAlertFact)
            .ToList();
    }

    public async Task<AlertFact> Handle(GetAlertByIdQuery query, CancellationToken cancellationToken = default)
    {
        var alert = await records.FindByIdAsync(query.Id, cancellationToken) ?? throw new DomainException("not_found");
        await business.Handle(new RequireBusinessAccessQuery(alert.BusinessId, actor.UserId, false), cancellationToken);
        return ToAlertFact(alert);
    }

    private static AlertFact ToAlertFact(AlertRecord alert) => new(alert.Id, alert.BusinessId, alert.EmployeeId, alert.Kind, alert.Status, alert.ReportStatus, alert.Category, alert.Description, alert.Location, alert.Latitude, alert.Longitude, alert.Moment, alert.CreatedAt ?? DateTimeOffset.MinValue, alert.ActivatedAt, alert.ResolvedAt, alert.CompletedAt, alert.Revision);
}
