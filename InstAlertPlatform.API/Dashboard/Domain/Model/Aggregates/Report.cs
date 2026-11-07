using InstAlertPlatform.API.Dashboard.Domain.Model.ValueObjects;
using System.Text.Json;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Dashboard.Domain.Model.Aggregates;
public sealed partial class Report : AuditedEntity
{
    private Report() { }

    public Report(string businessId, DateOnly startDate, DateOnly endDate, DateTimeOffset cutoffAt, string summaryJson, DateTimeOffset generatedAt, string userId)
    {
        var bounds = ReportingPeriod.Range(startDate, endDate, generatedAt);
        Guard.Require(cutoffAt == bounds.End, "validation", "cutoffAt");
        Guard.Require(!string.IsNullOrWhiteSpace(businessId) && !string.IsNullOrWhiteSpace(userId), "validation");
        using var json = JsonDocument.Parse(summaryJson);
        BusinessId = businessId;
        StartDate = startDate;
        EndDate = endDate;
        CutoffAt = cutoffAt;
        SummaryJson = summaryJson;
        GeneratedAt = generatedAt;
        GeneratedByUserId = userId;
    }

    public string BusinessId { get; private set; } = "";
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public DateTimeOffset CutoffAt { get; private set; }
    public string SummaryJson { get; private set; } = "";
    public DateTimeOffset GeneratedAt { get; private set; }
    public string GenerationSource { get; private set; } = "manual";
    public string? GeneratedByUserId { get; private set; }
    public int SchemaVersion { get; private set; } = 2;
}
