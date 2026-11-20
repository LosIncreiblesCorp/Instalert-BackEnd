namespace InstAlertPlatform.API.Business.Domain.Model.Events;
public sealed record StaffInvitationRequestedEvent(string InvitationId, string BusinessId, string OwnerUserId, string Email, int Sequence, string? Credential = null);
