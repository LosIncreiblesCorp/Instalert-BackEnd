using InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Dashboard.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyDashboardConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Report>().ToTable("dashboard_reports");
        builder.Entity<Report>().Property(x => x.BusinessId).HasMaxLength(80).IsRequired();
        builder.Entity<Report>().Property(x => x.StartDate).HasConversion(value => value.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value)).HasColumnType("date");
        builder.Entity<Report>().Property(x => x.EndDate).HasConversion(value => value.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value)).HasColumnType("date");
        builder.Entity<Report>().Property(x => x.SummaryJson).HasColumnType("longtext").IsRequired();
        builder.Entity<Report>().Property(x => x.GenerationSource).HasMaxLength(20).IsRequired();
        builder.Entity<Report>().Property(x => x.GeneratedByUserId).HasMaxLength(160);
        builder.Entity<Report>().HasIndex(x => new { x.BusinessId, x.GeneratedAt }).HasDatabaseName("i_x_dashboard_reports_business_id_generated_at");
    }
}
