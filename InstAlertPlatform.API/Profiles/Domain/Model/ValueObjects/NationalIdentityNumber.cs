using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Profiles.Domain.Model.ValueObjects;
public sealed record NationalIdentityNumber
{
    public string Value { get; }

    public NationalIdentityNumber(string value)
    {
        Value = Guard.Digits(value, 8, "dni");
    }
}
