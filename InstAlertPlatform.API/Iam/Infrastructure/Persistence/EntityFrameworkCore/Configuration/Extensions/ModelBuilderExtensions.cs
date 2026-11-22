using InstAlertPlatform.API.Iam.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        builder.Entity<User>().ToTable("users");
        builder.Entity<User>().Property(user => user.Email).HasMaxLength(254).IsRequired();
        builder.Entity<User>().HasIndex(user => user.Email).IsUnique();
        builder.Entity<User>().Property(user => user.PasswordHash).HasMaxLength(100).IsRequired();
        builder.Entity<User>().Property(user => user.Role).HasMaxLength(20).IsRequired();
        builder.Entity<User>().Property(user => user.SecurityStamp).HasMaxLength(32).IsRequired();
    }
}
