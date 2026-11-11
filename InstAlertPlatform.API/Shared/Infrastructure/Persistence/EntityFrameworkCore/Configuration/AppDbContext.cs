using InstAlertPlatform.API.Mapping.Domain.Model.Aggregates;
using InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Contacts.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Mapping.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder builder) => builder.Properties<DateTimeOffset>().HaveConversion<UtcTimestampConverter>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyAlertConfiguration();
        builder.ApplyMappingConfiguration();
        builder.ApplyNotificationsConfiguration();
        builder.Entity<Report>().ToTable("dashboard_reports");
        builder.Entity<Report>().Property(x => x.BusinessId).HasMaxLength(80).IsRequired();
        builder.Entity<Report>().Property(x => x.StartDate).HasConversion(value => value.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value)).HasColumnType("date");
        builder.Entity<Report>().Property(x => x.EndDate).HasConversion(value => value.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value)).HasColumnType("date");
        builder.Entity<Report>().Property(x => x.SummaryJson).HasColumnType("longtext").IsRequired();
        builder.Entity<Report>().Property(x => x.GenerationSource).HasMaxLength(20).IsRequired();
        builder.Entity<Report>().Property(x => x.GeneratedByUserId).HasMaxLength(160);
        builder.Entity<Report>().HasIndex(x => new { x.BusinessId, x.GeneratedAt }).HasDatabaseName("i_x_dashboard_reports_business_id_generated_at");
        builder.ApplyBusinessConfiguration();
        builder.ApplyContactsConfiguration();
        builder.ApplyPaymentsConfiguration();
        builder.ApplyProfilesConfiguration();
        builder.Entity<OutboxMessage>().ToTable("outbox_messages");
        builder.Entity<OutboxMessage>().Property(x => x.EventType).HasMaxLength(100);
        builder.Entity<OutboxMessage>().Property(x => x.Payload).HasColumnType("longtext");
        builder.Entity<OutboxMessage>().HasIndex(x => x.ProcessedAt);
        foreach (var entity in builder.Model.GetEntityTypes().Where(x => typeof(AuditedEntity).IsAssignableFrom(x.ClrType)))
        {
            builder.Entity(entity.ClrType).HasKey(nameof(AuditedEntity.Id));
            builder.Entity(entity.ClrType).Property(nameof(AuditedEntity.Id)).HasMaxLength(160);
            builder.Entity(entity.ClrType).Property(nameof(AuditedEntity.Version)).HasMaxLength(36).IsConcurrencyToken();
        }

        builder.ApplyProfilesFinalConfiguration();
        builder.ApplyPaymentsFinalConfiguration();
        builder.Entity<OutboxMessage>().Property(x => x.Id).HasMaxLength(255);
        builder.Entity<OutboxMessage>().Property(x => x.Version).HasColumnType("longtext");

        builder.UseSnakeCaseNamingConvention();
    }
}
