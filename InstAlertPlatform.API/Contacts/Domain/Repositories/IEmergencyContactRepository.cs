using InstAlertPlatform.API.Contacts.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Contacts.Domain.Repositories;
public interface IEmergencyContactRepository : IBaseRepository<EmergencyContact>
{
    Task<List<EmergencyContact>> FindByEmployeeIdAndNotEmailAsync(string employeeId, string? email, CancellationToken cancellationToken = default);
    Task<List<EmergencyContact>> FindByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
}
