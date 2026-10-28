namespace InstAlertPlatform.API.Business.Domain.Model.ValueObjects;

public sealed record TaxIdentifier
{
    public string Value { get; }

    public TaxIdentifier(string value)
    {
        if (value is null || value.Length != 11 || !value.All(char.IsAsciiDigit))
            throw new ArgumentException("The tax identifier must contain eleven digits.", nameof(value));

        Value = value;
    }
}
