using InstAlertPlatform.API.Mapping.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Mapping.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyMappingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<IncidentProjection>().ToTable("incident_projections");
        builder.Entity<IncidentProjection>().Property(x => x.Category).HasMaxLength(80);
        builder.Entity<IncidentProjection>().Property(x => x.BusinessId).HasMaxLength(80);
        builder.Entity<IncidentProjection>().Property(x => x.Type).HasMaxLength(30);
        builder.Entity<IncidentProjection>().Property(x => x.Status).HasMaxLength(30);
        builder.Entity<IncidentProjection>().Property(x => x.Title).HasMaxLength(100);
        builder.Entity<IncidentProjection>().Property(x => x.Description).HasMaxLength(2000);
        builder.Entity<IncidentProjection>().HasIndex(x => x.Visible);
    }
}
