using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;

namespace InstAlertPlatform.API.Business.Domain.Model.Aggregates;

public partial class BusinessProfile
{
    public int Id { get; private set; }
    public string OwnerUserId { get; private set; }
    public TaxIdentifier TaxId { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public BusinessProfile()
    {
        OwnerUserId = "";
        TaxId = null!;
        Name = "";
        Address = "";
    }

    public BusinessProfile(CreateBusinessProfileCommand command)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command.OwnerUserId);
        if (command.OwnerUserId.Length > 160)
            throw new ArgumentException("The owner identifier is too long.", nameof(command.OwnerUserId));

        var name = Normalize(command.Name, 150, nameof(command.Name));
        var address = Normalize(command.Address, 250, nameof(command.Address));
        ValidateCoordinates(command.Latitude, command.Longitude);

        OwnerUserId = command.OwnerUserId;
        TaxId = command.TaxId ?? throw new ArgumentException("The tax identifier is required.", nameof(command.TaxId));
        Name = name;
        Address = address;
        Latitude = command.Latitude;
        Longitude = command.Longitude;
    }

    public void Update(UpdateBusinessProfileCommand command)
    {
        if (command.TaxId is not null && command.TaxId != TaxId)
            throw new ArgumentException("The tax identifier cannot be changed.", nameof(command.TaxId));

        var name = Normalize(command.Name, 150, nameof(command.Name));
        var address = Normalize(command.Address, 250, nameof(command.Address));
        ValidateCoordinates(command.Latitude, command.Longitude);

        Name = name;
        Address = address;
        Latitude = command.Latitude;
        Longitude = command.Longitude;
    }

    private static string Normalize(string value, int maximumLength, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > maximumLength)
            throw new ArgumentException("The value is required and must fit its maximum length.", parameterName);

        return value.Trim();
    }

    private static void ValidateCoordinates(double latitude, double longitude)
    {
        if (!double.IsFinite(latitude) || latitude is < -90 or > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));
        if (!double.IsFinite(longitude) || longitude is < -180 or > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));
    }
}
