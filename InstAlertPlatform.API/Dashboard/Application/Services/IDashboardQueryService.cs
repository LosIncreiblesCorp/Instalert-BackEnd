using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Domain.Model.Queries;

namespace InstAlertPlatform.API.Dashboard.Application.Services;
public interface IDashboardQueryService
{
    Task<DashboardSnapshot> Handle(GetCurrentDashboardQuery query, CancellationToken cancellationToken = default);
    Task<List<ReportSnapshot>> Handle(GetReportsQuery query, CancellationToken cancellationToken = default);
    Task<ReportSnapshot> Handle(GetReportByIdQuery query, CancellationToken cancellationToken = default);
}
