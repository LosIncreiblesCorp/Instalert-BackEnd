using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class CreateIncidentReportCommandFromResourceAssembler
{
    public static CreateIncidentReportCommand ToCommandFromResource(CreateIncidentReportResource resource) => new(resource.BusinessId, resource.Kind, resource.Category, resource.Description, resource.Location, resource.Latitude, resource.Longitude, resource.Moment);
}
