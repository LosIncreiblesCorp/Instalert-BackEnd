using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Application.Internal.QueryServices;

public class BusinessProfileQueryService(IBusinessProfileRepository repository) : IBusinessProfileQueryService
{
    public Task<IEnumerable<BusinessProfile>> Handle(GetBusinessProfilesByOwnerUserIdQuery query,
        CancellationToken cancellationToken = default) =>
        repository.FindByOwnerUserIdAsync(query.OwnerUserId, cancellationToken);

    public async Task<BusinessProfile?> Handle(GetBusinessProfileByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.FindByIdAsync(query.Id, cancellationToken);
        return profile?.OwnerUserId == query.OwnerUserId ? profile : null;
    }
}
