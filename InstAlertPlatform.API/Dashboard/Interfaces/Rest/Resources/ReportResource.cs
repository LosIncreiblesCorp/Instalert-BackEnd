namespace InstAlertPlatform.API.Dashboard.Interfaces.Rest.Resources;
public sealed record ReportResource(string Id, string BusinessId, DateOnly StartDate, DateOnly EndDate, DateTimeOffset CutoffAt, DateTimeOffset GeneratedAt, string GenerationSource, string? GeneratedByUserId, int SchemaVersion, System.Text.Json.JsonElement Summary);
