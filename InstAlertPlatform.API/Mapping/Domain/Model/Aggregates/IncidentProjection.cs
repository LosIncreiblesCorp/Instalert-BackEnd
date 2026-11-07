using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Mapping.Domain.Model.Aggregates;
public sealed partial class IncidentProjection : AuditedEntity
{
    private IncidentProjection()
    {
    }

    public IncidentProjection(string sourceId)
    {
        Id = sourceId;
    }

    public string BusinessId { get; private set; } = "";
    public string Type { get; private set; } = "";
    public string Category { get; private set; } = "";
    public string Status { get; private set; } = "";
    public string Title { get; private set; } = "";
    public string Description { get; private set; } = "";
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public DateTimeOffset ReportedAt { get; private set; }
    public int SourceRevision { get; private set; }
    public bool Visible { get; private set; }

    public void Apply(int revision, string businessId, string kind, string status, string? reportStatus, string category, string description, double? lat, double? lng, DateTimeOffset reportedAt)
    {
        if (revision <= SourceRevision)
            return;
        Guard.Coordinates(lat, lng);
        SourceRevision = revision;
        BusinessId = businessId;
        Type = kind;
        Status = status;
        Category = category;
        Title = category.Length > 0 ? category : kind;
        Description = description;
        Latitude = lat;
        Longitude = lng;
        ReportedAt = reportedAt;
        Visible = lat.HasValue && lng.HasValue && (kind != "panic" || status == "active" || reportStatus == "completed");
    }
}
