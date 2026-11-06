using InstAlertPlatform.API.Alert.Domain.Model.Commands;
using InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Transform;
public static class UpdatePanicReportDraftCommandFromResourceAssembler
{
    public static UpdatePanicReportDraftCommand ToCommandFromResource(UpdatePanicReportDraftResource resource, string id) => new(id, resource.Category, resource.Description, resource.Location, resource.Latitude, resource.Longitude, resource.Moment);
}
