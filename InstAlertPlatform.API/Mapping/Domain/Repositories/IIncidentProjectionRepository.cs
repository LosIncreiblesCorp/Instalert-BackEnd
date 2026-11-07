using InstAlertPlatform.API.Mapping.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Mapping.Domain.Repositories;
public interface IIncidentProjectionRepository : IBaseRepository<IncidentProjection>
{
    Task<List<IncidentProjection>> FindByVisibleAsync(bool visible, CancellationToken cancellationToken = default);
}
