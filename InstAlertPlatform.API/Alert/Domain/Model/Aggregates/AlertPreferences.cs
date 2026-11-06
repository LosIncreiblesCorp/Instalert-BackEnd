using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
public sealed partial class AlertPreferences : AuditedEntity
{
    private AlertPreferences()
    {
    }

    public AlertPreferences(string userId, int seconds = 5)
    {
        Id = userId;
        Change(seconds);
    }

    public int PanicGracePeriodSeconds { get; private set; } = 5;

    public void Change(int seconds)
    {
        Guard.Require(seconds is >= 1 and <= 60, "validation", "panicGracePeriodSeconds");
        PanicGracePeriodSeconds = seconds;
    }
}
