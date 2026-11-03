using System.Globalization;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Transform;

public static class BusinessResourceFromEntityAssembler
{
    public static BusinessResource ToResourceFromEntity(BusinessProfile profile) =>
        new(profile.Id.ToString(CultureInfo.InvariantCulture), profile.TaxId.Value, profile.Name, profile.Address,
            profile.Latitude, profile.Longitude, profile.CreatedAt, profile.UpdatedAt);
}
