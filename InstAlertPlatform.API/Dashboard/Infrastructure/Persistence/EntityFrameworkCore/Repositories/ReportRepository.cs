using InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
using InstAlertPlatform.API.Dashboard.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Dashboard.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class ReportRepository(AppDbContext context) : BaseRepository<Report>(context), IReportRepository
{
    public Task<List<Report>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default) => Context.Set<Report>().Where(entity => entity.BusinessId == businessId).ToListAsync(cancellationToken);
}
