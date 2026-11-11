using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class BusinessMemberRepository(AppDbContext context) : BaseRepository<BusinessMember>(context), IBusinessMemberRepository
{
    public Task<List<BusinessMember>> FindByEmailAsync(string email, CancellationToken cancellationToken = default) => Context.Set<BusinessMember>().Where(entity => entity.Email == email).ToListAsync(cancellationToken);
    public Task<List<BusinessMember>> FindByBusinessIdAndEmailAsync(string businessId, string email, CancellationToken cancellationToken = default) => Context.Set<BusinessMember>().Where(entity => entity.BusinessId == int.Parse(businessId) && entity.Email == email).ToListAsync(cancellationToken);
    public Task<List<BusinessMember>> FindByBusinessIdAndStatusAsync(string businessId, string status, CancellationToken cancellationToken = default) => Context.Set<BusinessMember>().Where(entity => entity.BusinessId == int.Parse(businessId) && entity.Status == status).ToListAsync(cancellationToken);
    public Task<List<BusinessMember>> FindByBusinessIdAndUserIdAndStatusAsync(string businessId, string userId, string status, CancellationToken cancellationToken = default) => Context.Set<BusinessMember>().Where(entity => entity.BusinessId == int.Parse(businessId) && entity.UserId == userId && entity.Status == status).ToListAsync(cancellationToken);
    public Task<List<BusinessMember>> FindByBusinessIdAndUserIdAsync(string businessId, string userId, CancellationToken cancellationToken = default) => Context.Set<BusinessMember>().Where(entity => entity.BusinessId == int.Parse(businessId) && entity.UserId == userId).ToListAsync(cancellationToken);
    public Task<List<BusinessMember>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default) => Context.Set<BusinessMember>().Where(entity => entity.BusinessId == int.Parse(businessId)).ToListAsync(cancellationToken);
    public Task<List<BusinessMember>> FindByUserIdAndStatusAsync(string userId, string status, CancellationToken cancellationToken = default) => Context.Set<BusinessMember>().Where(entity => entity.UserId == userId && entity.Status == status).ToListAsync(cancellationToken);
    public Task<List<BusinessMember>> FindByUserIdAsync(string userId, CancellationToken cancellationToken = default) => Context.Set<BusinessMember>().Where(entity => entity.UserId == userId).ToListAsync(cancellationToken);
}
