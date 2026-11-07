using InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Dashboard.Domain.Repositories;
public interface IReportRepository : IBaseRepository<Report>
{
    Task<List<Report>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default);
}
