namespace InstAlertPlatform.API.Business.Interfaces.Rest.Resources;
public sealed record BusinessResource(string Id, string OwnerUserId, string TaxId, string Name, string Address, double Latitude, double Longitude, string Version, DateTimeOffset? CreatedAt = null, DateTimeOffset? UpdatedAt = null);
