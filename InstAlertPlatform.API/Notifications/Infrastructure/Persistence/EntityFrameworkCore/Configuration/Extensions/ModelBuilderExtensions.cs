using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Notifications.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyNotificationsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<ReceivedNotification>().ToTable("notifications");
        builder.Entity<ReceivedNotification>().Property(x => x.SourceAlertId).HasMaxLength(80).IsRequired();
        builder.Entity<ReceivedNotification>().Property(x => x.RecipientUserId).HasMaxLength(160).IsRequired();
        builder.Entity<ReceivedNotification>().Property(x => x.TargetBusinessId).HasMaxLength(80).IsRequired();
        builder.Entity<ReceivedNotification>().Property(x => x.Type).HasMaxLength(30).IsRequired();
        builder.Entity<ReceivedNotification>().Property(x => x.Title).HasMaxLength(150).IsRequired();
        builder.Entity<ReceivedNotification>().Property(x => x.Message).HasMaxLength(300).IsRequired();
        builder.Entity<ReceivedNotification>().HasIndex(x => new { x.SourceAlertId, x.RecipientUserId, x.TargetBusinessId })
            .IsUnique().HasDatabaseName("ix_notifications_alert_recipient_business");

        builder.Entity<NotificationPreferences>().ToTable("notification_preferences");
        builder.Entity<NotificationPreferences>().Property(x => x.UserId).HasMaxLength(160).IsRequired();
        builder.Entity<NotificationPreferences>().Property(x => x.BusinessId).HasMaxLength(80).IsRequired();
        builder.Entity<NotificationPreferences>().Property(x => x.NearbyRadiusMeters)
            .HasConversion(radius => radius.Value, value => new ReceptionRadius(value));
        builder.Entity<NotificationPreferences>().HasIndex(x => new { x.UserId, x.BusinessId }).IsUnique();

        builder.Entity<DeliveryRequest>().ToTable("delivery_requests");
        builder.Entity<DeliveryRequest>().Property(x => x.SourceInvitationId).HasMaxLength(80);
        builder.Entity<DeliveryRequest>().Property(x => x.SourceAlertId).HasMaxLength(80).IsRequired();
        builder.Entity<DeliveryRequest>().Property(x => x.Channel).HasMaxLength(20).IsRequired();
        builder.Entity<DeliveryRequest>().Property(x => x.Destination).HasMaxLength(254).IsRequired();
        builder.Entity<DeliveryRequest>().Property(x => x.UserId).HasMaxLength(160).IsRequired();
        builder.Entity<DeliveryRequest>().Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Entity<DeliveryRequest>().HasIndex(x => new { x.Status, x.LeaseUntil, x.CreatedAt });
        builder.Entity<DeliveryRequest>().HasIndex(x => new { x.UserId, x.CreatedAt });
    }
}
