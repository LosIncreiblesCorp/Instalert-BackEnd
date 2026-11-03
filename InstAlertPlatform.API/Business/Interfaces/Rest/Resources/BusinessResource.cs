namespace InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

public record BusinessResource(string Id, string TaxId, string Name, string Address,
    double Latitude, double Longitude, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);
