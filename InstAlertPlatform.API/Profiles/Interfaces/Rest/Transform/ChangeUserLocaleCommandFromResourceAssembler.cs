using InstAlertPlatform.API.Profiles.Domain.Model.Commands;
using InstAlertPlatform.API.Profiles.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform;
public static class ChangeUserLocaleCommandFromResourceAssembler
{
    public static ChangeUserLocaleCommand ToCommandFromResource(ChangeUserLocaleResource resource, string userId) => new(userId, resource.Locale);
}
