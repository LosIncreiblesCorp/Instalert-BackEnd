namespace InstAlertPlatform.API.Shared.Application.Patterns;
public interface ICurrentActor
{
    bool IsAdministrator => false;

    string UserId { get; }

    string? VerifiedEmail { get; }
}
