using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyProfilesConfiguration(this ModelBuilder builder)
    {
        builder.Entity<UserProfile>().ToTable("profiles");
        builder.Entity<UserProfile>().Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Entity<UserProfile>().Property(p => p.LastName).HasMaxLength(100).IsRequired();
        builder.Entity<UserProfile>().Property(p => p.Dni).HasConversion(value => value.Value, value => new NationalIdentityNumber(value)).HasMaxLength(8).IsRequired();
        builder.Entity<UserProfile>().Property(p => p.Phone).HasMaxLength(16);
        builder.Entity<UserPreferences>().ToTable("user_preferences");
        builder.Entity<UserPreferences>().Property(p => p.Locale).HasConversion(value => value.Value, value => new ApplicationLocale(value)).HasMaxLength(6).IsRequired();
    }

    public static void ApplyProfilesFinalConfiguration(this ModelBuilder builder)
    {
        builder.Entity<UserProfile>().Property(x => x.Id).HasMaxLength(80);
    }
}
