namespace InstAlertPlatform.API.Business.Application.Services;
public sealed record InvitationDeliveryFact(string InvitationId, string BusinessId, string OwnerUserId, string Email, int Sequence);
