namespace InstAlertPlatform.API.Notifications.Domain.Model.Commands;
public sealed record QueueInvitationDeliveryCommand(string InvitationId, string OwnerUserId, string Email, int Sequence);
