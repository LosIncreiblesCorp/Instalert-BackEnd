namespace InstAlertPlatform.API.Dashboard.Application.Services;
public sealed record ReportSnapshot(string Id, string BusinessId, DateOnly StartDate, DateOnly EndDate, DateTimeOffset CutoffAt, DateTimeOffset GeneratedAt, string GenerationSource, string? GeneratedByUserId, int SchemaVersion, System.Text.Json.JsonElement Summary);
