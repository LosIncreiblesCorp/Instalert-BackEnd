using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
public sealed partial class AlertRecord : AuditedEntity
{
    private AlertRecord()
    {
    }

    public string BusinessId { get; private set; } = "";
    public string EmployeeId { get; private set; } = "";
    public string Kind { get; private set; } = "";
    public string Status { get; private set; } = "";
    public string? ReportStatus { get; private set; }
    public string Category { get; private set; } = "";
    public string Location { get; private set; } = "";
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public DateTimeOffset? Moment { get; private set; }
    public string Description { get; private set; } = "";
    public DateTimeOffset? ActivatedAt { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public int Revision { get; private set; } = 1;

    public static AlertRecord Panic(string id, string businessId, string userId, string location, double? lat, double? lng, DateTimeOffset now)
    {
        Guard.Coordinates(lat, lng);
        return new()
        {
            Id = id,
            BusinessId = businessId,
            EmployeeId = userId,
            Kind = "panic",
            Status = "active",
            Location = location,
            Latitude = lat,
            Longitude = lng,
            ActivatedAt = now,
            CreatedAt = now
        };
    }

    public static AlertRecord Report(string businessId, string userId, string kind, string category, string description, string location, double? lat, double? lng, DateTimeOffset moment, DateTimeOffset now)
    {
        Guard.Require(kind is "past-incident" or "suspicious-activity" or "other-situation", "validation", "kind");
        Guard.Require(moment.Year >= 1000 && moment <= now, "validation", "moment");
        var a = new AlertRecord
        {
            BusinessId = businessId,
            EmployeeId = userId,
            Kind = kind,
            Status = "submitted",
            Moment = moment,
            SubmittedAt = now,
            CreatedAt = now
        };
        a.SetDetails(category, description, location, lat, lng, true);
        return a;
    }

    public void UpdateLocation(string location, double? latitude, double? longitude)
    {
        Guard.Require(Kind == "panic" && Status == "active", "transition");
        Guard.Coordinates(latitude, longitude);
        Guard.Require((location?.Length ?? 0) <= 250, "validation", "location");
        Location = location ?? "";
        Latitude = latitude;
        Longitude = longitude;
        Revision++;
    }

    public bool Resolve(DateTimeOffset now)
    {
        Guard.Require(Kind == "panic", "transition");
        if (Status == "resolved")
            return false;
        Guard.Require(Status == "active", "transition");
        Status = "resolved";
        ReportStatus = "pending";
        ResolvedAt = now;
        Revision++;
        return true;
    }

    public void Draft(string category, string description, string location, double? lat, double? lng, DateTimeOffset? moment)
    {
        Guard.Require(Kind == "panic" && Status == "resolved" && ReportStatus == "pending", "transition");
        SetDetails(category, description, location, lat, lng, false);
        Moment = moment;
        Revision++;
    }

    public void Complete(DateTimeOffset now)
    {
        Guard.Require(ReportStatus != "completed", "already_completed");
        Guard.Require(Kind == "panic" && Status == "resolved" && ReportStatus == "pending", "transition");
        Guard.Text(Category, 80, "category");
        Guard.Text(Description, 2000, "description");
        ReportStatus = "completed";
        CompletedAt = now;
        Revision++;
    }

    private void SetDetails(string category, string description, string location, double? lat, double? lng, bool required)
    {
        Guard.Coordinates(lat, lng);
        var c = category?.Trim() ?? "";
        var d = description?.Trim() ?? "";
        var l = location?.Trim() ?? "";
        Guard.Require(c.Length <= 80 && d.Length <= 2000 && l.Length <= 250, "validation");
        if (required)
        {
            Guard.Text(c, 80, "category");
            Guard.Text(d, 2000, "description");
            Guard.Text(l, 250, "location");
        }

        string[] allowed = Kind switch
        {
            "panic" or "past-incident" => ["robbery", "attemptedRobbery", "assault", "extortion", "vandalism", "other"],
            "suspicious-activity" => ["person", "vehicle", "unusualBehavior", "surveillance", "other"],
            _ => ["lowLighting", "policePresence", "roadBlockage", "other"]
        };
        Guard.Require(!required && c.Length == 0 || allowed.Contains(c), "validation", "category");
        Category = c;
        Description = d;
        Location = l;
        Latitude = lat;
        Longitude = lng;
    }
}
