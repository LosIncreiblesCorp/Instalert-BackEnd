using InstAlertPlatform.API.Profiles.Domain.Model.Commands;
using InstAlertPlatform.API.Profiles.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform;
public static class UpdateUserProfileCommandFromResourceAssembler
{
    public static UpdateUserProfileCommand ToCommandFromResource(UpdateUserProfileResource resource, string userId) => new(userId, resource.FirstName, resource.LastName, resource.Dni, resource.Phone);
}
