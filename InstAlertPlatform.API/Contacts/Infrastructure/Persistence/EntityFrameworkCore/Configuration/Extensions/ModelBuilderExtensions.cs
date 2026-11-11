using InstAlertPlatform.API.Contacts.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Contacts.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyContactsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<EmergencyContact>()
            .ToTable("emergency_contacts");
        builder.Entity<EmergencyContact>()
            .Property(c => c.EmployeeId).HasMaxLength(80).IsRequired();

        builder.Entity<EmergencyContact>().HasIndex(c => c.EmployeeId);
        builder.Entity<EmergencyContact>().Property(c => c.FullName)
            .HasMaxLength(100).IsRequired();

        builder.Entity<EmergencyContact>()
            .Property(c => c.Relationship).HasMaxLength(60)
            .IsRequired();
        builder.Entity<EmergencyContact>()
            .Property(c => c.Phone).HasMaxLength(16)
            .IsRequired();
        builder.Entity<EmergencyContact>().Property(c => c.Email)
            .HasMaxLength(254);


        builder.Entity<EmergencyContact>().Property(c => c.Notes)
            .HasMaxLength(140);
    }
}
