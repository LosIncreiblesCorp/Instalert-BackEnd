using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Alert.Domain.Repositories;
public interface IAlertRecordRepository : IBaseRepository<AlertRecord>
{
    Task<List<AlertRecord>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default);
    Task<List<AlertRecord>> FindByEmployeeIdAndKindAndStatusAsync(string employeeId, string kind, string status, CancellationToken cancellationToken = default);
}
