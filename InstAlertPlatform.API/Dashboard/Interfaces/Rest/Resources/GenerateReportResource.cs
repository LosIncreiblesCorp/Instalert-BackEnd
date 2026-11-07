using System.ComponentModel.DataAnnotations;
namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest.Resources;
public sealed record GenerateReportResource([Required] DateOnly? StartDate, [Required] DateOnly? EndDate);
