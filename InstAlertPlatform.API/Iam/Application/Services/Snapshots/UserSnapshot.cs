namespace InstAlertPlatform.API.Iam.Application.Services.Snapshots;

public sealed record SessionBusinessSnapshot(string Id, string Name, bool SubscriptionActive);
public sealed record UserSnapshot(string UserId, string Email, string Role, bool MustChangePassword,
    string OnboardingStep, bool HasApplicationAccess, bool ProfileCompleted, string? BusinessId,
    SessionBusinessSnapshot? Business);
