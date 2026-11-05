namespace InstAlertPlatform.API.Business.Domain.Model.Queries;
public sealed record RequireBusinessAccessQuery(string BusinessId, string UserId, bool Administrator);
