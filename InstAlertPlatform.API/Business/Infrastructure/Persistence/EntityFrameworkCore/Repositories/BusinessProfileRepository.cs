using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class BusinessProfileRepository(AppDbContext context)
    : BaseRepository<BusinessProfile>(context), IBusinessProfileRepository
{
    public async Task<IEnumerable<BusinessProfile>> FindByOwnerUserIdAsync(string ownerUserId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<BusinessProfile>().AsNoTracking()
            .Where(profile => profile.OwnerUserId == ownerUserId).OrderBy(profile => profile.Id)
            .ToListAsync(cancellationToken);

    public Task<BusinessProfile?> FindByTaxIdAsync(TaxIdentifier taxId,
        CancellationToken cancellationToken = default) =>
        Context.Set<BusinessProfile>().FirstOrDefaultAsync(profile => profile.TaxId == taxId, cancellationToken);
}
