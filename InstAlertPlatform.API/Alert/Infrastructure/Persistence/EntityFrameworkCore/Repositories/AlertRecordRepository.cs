using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Alert.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class AlertRecordRepository(AppDbContext context) : BaseRepository<AlertRecord>(context), IAlertRecordRepository
{
    public Task<List<AlertRecord>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default) => Context.Set<AlertRecord>().Where(entity => entity.BusinessId == businessId).ToListAsync(cancellationToken);
    public Task<List<AlertRecord>> FindByEmployeeIdAndKindAndStatusAsync(string employeeId, string kind, string status, CancellationToken cancellationToken = default) => Context.Set<AlertRecord>().Where(entity => entity.EmployeeId == employeeId && entity.Kind == kind && entity.Status == status).ToListAsync(cancellationToken);
}
