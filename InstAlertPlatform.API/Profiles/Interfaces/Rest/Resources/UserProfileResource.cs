namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Resources;
public sealed record UserProfileResource(string UserId, string FirstName, string LastName, string Dni, string? Phone, DateTimeOffset? UpdatedAt, string Version);
