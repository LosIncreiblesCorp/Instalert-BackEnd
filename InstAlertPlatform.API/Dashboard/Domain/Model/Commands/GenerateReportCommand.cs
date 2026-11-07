namespace InstAlertPlatform.API.Dashboard.Domain.Model.Commands;
public sealed record GenerateReportCommand(string BusinessId, DateOnly StartDate, DateOnly EndDate);
