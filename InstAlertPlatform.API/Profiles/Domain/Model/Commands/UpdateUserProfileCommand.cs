namespace InstAlertPlatform.API.Profiles.Domain.Model.Commands;
public sealed record UpdateUserProfileCommand(string UserId, string FirstName, string LastName, string Dni, string? Phone);
