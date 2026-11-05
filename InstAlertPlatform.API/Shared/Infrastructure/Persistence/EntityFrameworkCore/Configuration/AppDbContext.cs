using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.EntityFrameworkCore;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder builder) =>
        builder.Properties<DateTimeOffset>().HaveConversion<UtcTimestampConverter>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var business = builder.Entity<BusinessProfile>();
        business.HasKey(profile => profile.Id);
        business.Property(profile => profile.Id).IsRequired().ValueGeneratedOnAdd();
        business.Property(profile => profile.OwnerUserId).IsRequired().HasMaxLength(160);
        business.Property(profile => profile.TaxId)
            .HasConversion(value => value.Value, value => new TaxIdentifier(value))
            .IsRequired().HasMaxLength(11);
        business.HasIndex(profile => profile.TaxId).IsUnique();
        business.HasIndex(profile => profile.OwnerUserId);
        business.Property(profile => profile.Name).IsRequired().HasMaxLength(150);
        business.Property(profile => profile.Address).IsRequired().HasMaxLength(250);
        business.Property(profile => profile.Latitude).IsRequired();
        business.Property(profile => profile.Longitude).IsRequired();
        business.Property(profile => profile.SeatRevision).IsRequired().HasDefaultValue(0L);
        business.Property(profile => profile.CreatedAt).HasConversion<DateTime?>(
            value => value.HasValue ? value.Value.UtcDateTime : null,
            value => value.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null);
        business.Property(profile => profile.UpdatedAt).HasConversion<DateTime?>(
            value => value.HasValue ? value.Value.UtcDateTime : null,
            value => value.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null);

        var outbox = builder.Entity<OutboxMessage>();
        outbox.ToTable("outbox_messages");
        outbox.HasKey(x => x.Id);
        outbox.Property(x => x.EventType).IsRequired().HasMaxLength(100);
        outbox.Property(x => x.Payload).HasColumnType("longtext");
        outbox.HasIndex(x => x.ProcessedAt);

        var profile = builder.Entity<UserProfile>();
        profile.ToTable("profiles");
        profile.HasKey(x => x.Id);
        profile.Property(x => x.Id).IsRequired().HasMaxLength(80);
        profile.Property(x => x.Version).IsRequired().HasMaxLength(36).IsConcurrencyToken();
        profile.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        profile.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        profile.Property(x => x.Dni)
            .HasConversion(value => value.Value, value => new NationalIdentityNumber(value))
            .IsRequired().HasMaxLength(8);
        profile.Property(x => x.Phone).HasMaxLength(16);

        var preferences = builder.Entity<UserPreferences>();
        preferences.ToTable("user_preferences");
        preferences.HasKey(x => x.Id);
        preferences.Property(x => x.Id).IsRequired().HasMaxLength(160);
        preferences.Property(x => x.Version).IsRequired().HasMaxLength(36).IsConcurrencyToken();
        preferences.Property(x => x.Locale)
            .HasConversion(value => value.Value, value => new ApplicationLocale(value))
            .IsRequired().HasMaxLength(6);

        var plan = builder.Entity<Plan>();
        plan.ToTable("plans");
        plan.HasKey(x => x.Id);
        plan.Property(x => x.Id).IsRequired().HasMaxLength(80);
        plan.Property(x => x.Version).IsRequired().HasMaxLength(36).IsConcurrencyToken();
        plan.Property(x => x.Name).IsRequired().HasMaxLength(100);
        plan.Property(x => x.Price).HasPrecision(10, 2);
        plan.Property(x => x.Currency).IsRequired().HasMaxLength(3);
        plan.Property(x => x.BillingCycle).IsRequired().HasMaxLength(10);
        plan.Property(x => x.MaxEmployees).IsRequired();
        plan.Property(x => x.ProviderPlanId).IsRequired().HasMaxLength(100);
        plan.HasIndex(x => x.ProviderPlanId).IsUnique();
        plan.HasData(
            new { Id = "essential", Version = "catalog-v1", CreatedAt = (DateTimeOffset?)null, UpdatedAt = (DateTimeOffset?)null, Name = "Essential", Price = 15m, Currency = "USD", BillingCycle = "monthly", MaxEmployees = 4, ProviderPlanId = "P-9G467950ML2745201NLOCKGI" },
            new { Id = "professional", Version = "catalog-v1", CreatedAt = (DateTimeOffset?)null, UpdatedAt = (DateTimeOffset?)null, Name = "Professional", Price = 30m, Currency = "USD", BillingCycle = "monthly", MaxEmployees = 8, ProviderPlanId = "P-66N5798614080793BNLOCKNY" },
            new { Id = "business", Version = "catalog-v1", CreatedAt = (DateTimeOffset?)null, UpdatedAt = (DateTimeOffset?)null, Name = "Business", Price = 45m, Currency = "USD", BillingCycle = "monthly", MaxEmployees = 15, ProviderPlanId = "P-8D911646UD924981MNLOCK6Q" });

        var subscription = builder.Entity<Subscription>();
        subscription.ToTable("subscriptions");
        subscription.HasKey(x => x.Id);
        subscription.Property(x => x.Id).IsRequired().HasMaxLength(160);
        subscription.Property(x => x.Version).IsRequired().HasMaxLength(36).IsConcurrencyToken();
        subscription.Property(x => x.BusinessId).IsRequired().HasMaxLength(80);
        subscription.Property(x => x.PlanId).IsRequired().HasMaxLength(80);
        subscription.Property(x => x.ProviderSubscriptionId).IsRequired().HasMaxLength(100);
        subscription.Property(x => x.Status).IsRequired().HasMaxLength(20);
        subscription.HasIndex(x => x.BusinessId).IsUnique();
        subscription.HasIndex(x => x.ProviderSubscriptionId).IsUnique();
        subscription.HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);

        var member = builder.Entity<BusinessMember>();
        member.ToTable("business_members");
        member.HasKey(x => x.Id);
        member.Property(x => x.Id).IsRequired().HasMaxLength(160);
        member.Property(x => x.Version).IsRequired().HasMaxLength(36).IsConcurrencyToken();
        member.Property(x => x.BusinessId).IsRequired();
        member.Property(x => x.UserId).IsRequired().HasMaxLength(160);
        member.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
        member.Property(x => x.Email).IsRequired().HasMaxLength(254);
        member.Property(x => x.Role).IsRequired().HasMaxLength(30);
        member.Property(x => x.Status).IsRequired().HasMaxLength(20);
        member.HasIndex(x => new { x.BusinessId, x.Status });
        member.HasIndex(x => new { x.BusinessId, x.UserId }).IsUnique();
        member.HasIndex(x => new { x.UserId, x.Status });
        member.HasOne<BusinessProfile>().WithMany().HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        var invitation = builder.Entity<StaffInvitation>();
        invitation.ToTable("staff_invitations");
        invitation.HasKey(x => x.Id);
        invitation.Property(x => x.Id).IsRequired().HasMaxLength(160);
        invitation.Property(x => x.Version).IsRequired().HasMaxLength(36).IsConcurrencyToken();
        invitation.Property(x => x.BusinessId).IsRequired();
        invitation.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
        invitation.Property(x => x.Email).IsRequired().HasMaxLength(254);
        invitation.Property(x => x.Status).IsRequired().HasMaxLength(20);
        invitation.Property(x => x.DeliverySequence).IsRequired();
        invitation.Property(x => x.LastRequestedAt).IsRequired();
        invitation.HasIndex(x => new { x.BusinessId, x.Status });
        invitation.HasOne<BusinessProfile>().WithMany().HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        var alert = builder.Entity<AlertRecord>();
        alert.ToTable("alerts");
        alert.HasKey(x => x.Id);
        alert.Property(x => x.Id).IsRequired().HasMaxLength(160);
        alert.Property(x => x.Version).IsRequired().HasMaxLength(36).IsConcurrencyToken();
        alert.Property(x => x.BusinessId).IsRequired().HasMaxLength(80);
        alert.Property(x => x.EmployeeId).IsRequired().HasMaxLength(80);
        alert.Property(x => x.Kind).IsRequired().HasMaxLength(30);
        alert.Property(x => x.Status).IsRequired().HasMaxLength(30);
        alert.Property(x => x.ReportStatus).HasMaxLength(20);
        alert.Property(x => x.Category).IsRequired().HasMaxLength(80);
        alert.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        alert.Property(x => x.Location).IsRequired().HasMaxLength(250);
        alert.HasIndex(x => new { x.BusinessId, x.Status });
        alert.HasIndex(x => x.EmployeeId);

        builder.UseSnakeCaseNamingConvention();
    }
}
