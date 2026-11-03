using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;

namespace InstAlertPlatform.API.Payments.Application.Services;
public interface IPlanQueryService
{
    Task<List<PlanSnapshot>> Handle(GetPlansQuery query, CancellationToken cancellationToken = default);
    Task<PlanLimitSnapshot> Handle(GetPlanLimitsQuery query, CancellationToken cancellationToken = default);
}
