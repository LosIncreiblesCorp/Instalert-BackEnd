namespace InstAlertPlatform.API.Business.Domain.Model.Queries;

public sealed record RequireBusinessAccessQuery(int BusinessId, string UserId, bool OwnerOnly);
