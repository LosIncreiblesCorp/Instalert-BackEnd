namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Resources;
public sealed record UpdateUserProfileResource(string FirstName, string LastName, string Dni, string? Phone);
