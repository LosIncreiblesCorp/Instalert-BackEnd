namespace InstAlertPlatform.API.Business.Application.Services;
public sealed record BusinessSnapshot(string Id, string OwnerUserId, string TaxId, string Name, string Address, double Latitude, double Longitude, DateTimeOffset? CreatedAt = null, DateTimeOffset? UpdatedAt = null);
