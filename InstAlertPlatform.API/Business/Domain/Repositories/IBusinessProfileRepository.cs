using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Domain.Repositories;

public interface IBusinessProfileRepository : IBaseRepository<BusinessProfile>
{
    Task<IEnumerable<BusinessProfile>> FindByOwnerUserIdAsync(string ownerUserId,
        CancellationToken cancellationToken = default);

    Task<BusinessProfile?> FindByTaxIdAsync(TaxIdentifier taxId,
        CancellationToken cancellationToken = default);
}
