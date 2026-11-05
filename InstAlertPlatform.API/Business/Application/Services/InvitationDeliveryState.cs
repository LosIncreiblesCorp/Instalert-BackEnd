namespace InstAlertPlatform.API.Business.Application.Services;
public sealed record InvitationDeliveryState(string Email, bool Pending, int Sequence);
