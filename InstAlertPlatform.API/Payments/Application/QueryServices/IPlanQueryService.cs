using InstAlertPlatform.API.Payments.Application.Services.Snapshots;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;

namespace InstAlertPlatform.API.Payments.Application.QueryServices;
public interface IPlanQueryService
{
    Task<List<PlanSnapshot>> Handle(GetPlansQuery query, CancellationToken cancellationToken = default);
    Task<PlanLimitSnapshot> Handle(GetPlanLimitsQuery query, CancellationToken cancellationToken = default);
}
