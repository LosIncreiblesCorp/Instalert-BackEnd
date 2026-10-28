using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Queries;

namespace InstAlertPlatform.API.Business.Application.Services;

public interface IBusinessProfileQueryService
{
    Task<IEnumerable<BusinessProfile>> Handle(GetBusinessProfilesByOwnerUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<BusinessProfile?> Handle(GetBusinessProfileByIdQuery query,
        CancellationToken cancellationToken = default);
}
