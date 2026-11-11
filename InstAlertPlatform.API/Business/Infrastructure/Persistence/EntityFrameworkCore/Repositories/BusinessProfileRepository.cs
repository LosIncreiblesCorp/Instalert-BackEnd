using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class BusinessProfileRepository(AppDbContext context) : BaseRepository<BusinessProfile>(context), IBusinessProfileRepository
{
    public Task<List<BusinessProfile>> FindByOwnerUserIdAsync(string ownerUserId, CancellationToken cancellationToken = default) => Context.Set<BusinessProfile>().Where(entity => entity.OwnerUserId == ownerUserId).ToListAsync(cancellationToken);
    public Task<List<BusinessProfile>> FindByTaxIdAsync(TaxIdentifier taxId, CancellationToken cancellationToken = default) => Context.Set<BusinessProfile>().Where(entity => entity.TaxId == taxId).ToListAsync(cancellationToken);
}
