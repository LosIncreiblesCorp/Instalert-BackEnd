using InstAlertPlatform.API.Contacts.Domain.Model.Aggregates;
using InstAlertPlatform.API.Contacts.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Contacts.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class EmergencyContactRepository(AppDbContext context) : BaseRepository<EmergencyContact>(context), IEmergencyContactRepository
{
    public Task<List<EmergencyContact>> FindByEmployeeIdAndNotEmailAsync(string employeeId, string? email, CancellationToken cancellationToken = default) => Context.Set<EmergencyContact>().Where(entity => entity.EmployeeId == employeeId && entity.Email != email).ToListAsync(cancellationToken);
    public Task<List<EmergencyContact>> FindByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default) => Context.Set<EmergencyContact>().Where(entity => entity.EmployeeId == employeeId).ToListAsync(cancellationToken);
}
