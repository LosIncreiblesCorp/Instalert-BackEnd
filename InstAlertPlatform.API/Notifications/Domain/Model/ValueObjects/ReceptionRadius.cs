using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Notifications.Domain.Model.ValueObjects;
public sealed record ReceptionRadius
{
    public int Value { get; }

    public ReceptionRadius(int value)
    {
        Value = Validate(value);
    }

    private static int Validate(int value)
    {
        Guard.Require(value is 100 or 500 or 1000, "validation", "nearbyRadiusMeters");
        return value;
    }
}
