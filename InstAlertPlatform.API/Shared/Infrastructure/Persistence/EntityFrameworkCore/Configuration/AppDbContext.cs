using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.EntityFrameworkCore;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Model.ValueObjects;

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

        builder.UseSnakeCaseNamingConvention();
    }
}
