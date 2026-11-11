using InstAlertPlatform.API.Profiles.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Mapping.Domain.Model.Aggregates;
using InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Contacts.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
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
        builder.Entity<BusinessProfile>().ToTable("business_profiles");
        builder.Entity<BusinessProfile>().HasKey(x => x.Id);
        builder.Entity<BusinessProfile>().Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Entity<BusinessProfile>().Property(x => x.OwnerUserId).HasMaxLength(160);
        builder.Entity<BusinessProfile>().Property(x => x.Version).HasMaxLength(36).IsConcurrencyToken();
        builder.Entity<BusinessProfile>().HasIndex(x => x.OwnerUserId).IsUnique();
        builder.Entity<BusinessProfile>().Property(x => x.SeatRevision).HasDefaultValue(0L);
        builder.Entity<BusinessProfile>().Property(x => x.TaxId).HasConversion(value => value.Value, value => new TaxIdentifier(value)).HasMaxLength(11);
        builder.Entity<BusinessProfile>().HasIndex(x => x.TaxId).IsUnique();
        builder.Entity<BusinessProfile>().Property(x => x.Name).HasMaxLength(150);
        builder.Entity<BusinessProfile>().Property(x => x.Address).HasMaxLength(250);
        builder.Entity<BusinessMember>().ToTable("business_members");
        builder.Entity<BusinessMember>().Property(x => x.BusinessId).IsRequired();
        builder.Entity<BusinessMember>().Property(x => x.UserId).HasMaxLength(160);
        builder.Entity<BusinessMember>().Property(x => x.DisplayName).HasMaxLength(100);
        builder.Entity<BusinessMember>().Property(x => x.Email).HasMaxLength(254);
        builder.Entity<BusinessMember>().Property(x => x.Role).HasMaxLength(30);
        builder.Entity<BusinessMember>().Property(x => x.Status).HasMaxLength(20);
        builder.Entity<BusinessMember>().HasIndex(x => x.UserId).IsUnique();
        builder.Entity<BusinessMember>().HasIndex(x => new { x.BusinessId, x.Email }).IsUnique();
        builder.Entity<BusinessMember>().HasIndex(x => new { x.BusinessId, x.Status }).HasDatabaseName("i_x_business_members_business_id_status");
        builder.Entity<BusinessMember>().HasIndex(x => new { x.UserId, x.Status }).HasDatabaseName("i_x_business_members_user_id_status");
        builder.Entity<BusinessMember>().HasOne<BusinessProfile>().WithMany().HasForeignKey(x => x.BusinessId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("f_k_business_members__business_profile_business_id");
        builder.Entity<StaffInvitation>().ToTable("staff_invitations");
        builder.Entity<StaffInvitation>().Property(x => x.BusinessId).IsRequired();
        builder.Entity<StaffInvitation>().Property(x => x.DisplayName).HasMaxLength(100);
        builder.Entity<StaffInvitation>().Property(x => x.Email).HasMaxLength(254);
        builder.Entity<StaffInvitation>().Property(x => x.Status).HasMaxLength(20);
        builder.Entity<StaffInvitation>().Property<string>("PendingEmail").HasMaxLength(254)
            .HasComputedColumnSql("CASE WHEN `status` = 'pending' THEN `email` ELSE NULL END", stored: true);
        builder.Entity<StaffInvitation>().HasIndex("PendingEmail").IsUnique();
        builder.Entity<StaffInvitation>().HasIndex(x => new { x.BusinessId, x.Status });
        builder.Entity<StaffInvitation>().HasOne<BusinessProfile>().WithMany().HasForeignKey(x => x.BusinessId).OnDelete(DeleteBehavior.Restrict);
        builder.ApplyContactsConfiguration();
        builder.ApplyPaymentsConfiguration();
        builder.Entity<UserProfile>().ToTable("profiles");
        builder.Entity<UserProfile>().Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Entity<UserProfile>().Property(p => p.LastName).HasMaxLength(100).IsRequired();
        builder.Entity<UserProfile>().Property(p => p.Dni).HasConversion(value => value.Value, value => new NationalIdentityNumber(value)).HasMaxLength(8).IsRequired();
        builder.Entity<UserProfile>().Property(p => p.Phone).HasMaxLength(16);
        builder.Entity<UserPreferences>().ToTable("user_preferences");
        builder.Entity<UserPreferences>().Property(p => p.Locale).HasConversion(value => value.Value, value => new ApplicationLocale(value)).HasMaxLength(6).IsRequired();
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

        builder.Entity<UserProfile>().Property(x => x.Id).HasMaxLength(80);
        builder.ApplyPaymentsFinalConfiguration();
        builder.Entity<OutboxMessage>().Property(x => x.Id).HasMaxLength(255);
        builder.Entity<OutboxMessage>().Property(x => x.Version).HasColumnType("longtext");

        builder.UseSnakeCaseNamingConvention();
    }
}
