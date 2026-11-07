using InstAlertPlatform.API.Mapping.Domain.Model.Aggregates;
using InstAlertPlatform.API.Mapping.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Mapping.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class IncidentProjectionRepository(AppDbContext context) : BaseRepository<IncidentProjection>(context), IIncidentProjectionRepository
{
    public Task<List<IncidentProjection>> FindByVisibleAsync(bool visible, CancellationToken cancellationToken = default) => Context.Set<IncidentProjection>().Where(entity => entity.Visible == visible).ToListAsync(cancellationToken);
}
