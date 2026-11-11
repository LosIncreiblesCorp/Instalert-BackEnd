using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Alert.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyAlertConfiguration(this ModelBuilder builder)
    {
        builder.Entity<AlertRecord>().ToTable("alerts");
        builder.Entity<AlertRecord>().Property(x => x.BusinessId).HasMaxLength(80);
        builder.Entity<AlertRecord>().Property(x => x.EmployeeId).HasMaxLength(80);
        builder.Entity<AlertRecord>().Property(x => x.Kind).HasMaxLength(30);
        builder.Entity<AlertRecord>().Property(x => x.Status).HasMaxLength(30);
        builder.Entity<AlertRecord>().Property(x => x.ReportStatus).HasMaxLength(20);
        builder.Entity<AlertRecord>().Property(x => x.Category).HasMaxLength(80);
        builder.Entity<AlertRecord>().Property(x => x.Description).HasMaxLength(2000);
        builder.Entity<AlertRecord>().Property(x => x.Location).HasMaxLength(250);
        builder.Entity<AlertRecord>().HasIndex(x => new { x.BusinessId, x.Status });
        builder.Entity<AlertRecord>().HasIndex(x => x.EmployeeId);
        builder.Entity<AlertPreferences>().ToTable("alert_preferences");
    }
}
