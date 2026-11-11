using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Business.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyBusinessConfiguration(this ModelBuilder builder)
    {
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
    }
}
