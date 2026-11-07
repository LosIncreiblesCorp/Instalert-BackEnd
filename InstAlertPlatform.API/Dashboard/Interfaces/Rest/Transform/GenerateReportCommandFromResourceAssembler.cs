using InstAlertPlatform.API.Dashboard.Domain.Model.Commands;
using InstAlertPlatform.API.Dashboard.Interfaces.Rest.Resources;
namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest.Transform;
public static class GenerateReportCommandFromResourceAssembler
{
    public static GenerateReportCommand ToCommandFromResource(string businessId, GenerateReportResource resource) => new(businessId, resource.StartDate!.Value, resource.EndDate!.Value);
}
