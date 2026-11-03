using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Profiles.Domain.Model.ValueObjects;
public sealed record ApplicationLocale
{
    public string Value { get; }

    public ApplicationLocale(string value)
    {
        Value = Validate(value);
    }

    private static string Validate(string value)
    {
        Guard.Require(value is "en_US" or "es_419", "validation", "locale");
        return value;
    }
}
