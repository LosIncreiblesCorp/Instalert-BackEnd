using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class StaffInvitationRepository(AppDbContext context) : BaseRepository<StaffInvitation>(context), IStaffInvitationRepository
{
    public Task<List<StaffInvitation>> FindPendingByEmailAsync(string email, CancellationToken cancellationToken = default) => Context.Set<StaffInvitation>().Where(entity => entity.Email == email && entity.Status == "pending").ToListAsync(cancellationToken);
    public Task<List<StaffInvitation>> FindByBusinessIdAndEmailAndStatusAsync(string businessId, string email, string status, CancellationToken cancellationToken = default) => Context.Set<StaffInvitation>().Where(entity => entity.BusinessId == int.Parse(businessId) && entity.Email == email && entity.Status == status).ToListAsync(cancellationToken);
    public Task<List<StaffInvitation>> FindByBusinessIdAndNotIdAndEmailAndStatusAsync(string businessId, string id, string email, string status, CancellationToken cancellationToken = default) => Context.Set<StaffInvitation>().Where(entity => entity.BusinessId == int.Parse(businessId) && entity.Id != id && entity.Email == email && entity.Status == status).ToListAsync(cancellationToken);
    public Task<List<StaffInvitation>> FindByBusinessIdAndStatusAsync(string businessId, string status, CancellationToken cancellationToken = default) => Context.Set<StaffInvitation>().Where(entity => entity.BusinessId == int.Parse(businessId) && entity.Status == status).ToListAsync(cancellationToken);
    public Task<List<StaffInvitation>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default) => Context.Set<StaffInvitation>().Where(entity => entity.BusinessId == int.Parse(businessId)).ToListAsync(cancellationToken);
}
