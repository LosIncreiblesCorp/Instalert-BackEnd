using InstAlertPlatform.API.Profiles.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;

public sealed partial class UserPreferences : AuditedEntity
{
    private UserPreferences() { }

    public UserPreferences(string userId, string locale)
    {
        Id = Guard.Text(userId, 160, "userId");
        ChangeLocale(locale);
    }

    public ApplicationLocale Locale { get; private set; } = new("en_US");

    public void ChangeLocale(string locale) => Locale = new ApplicationLocale(locale);
}
