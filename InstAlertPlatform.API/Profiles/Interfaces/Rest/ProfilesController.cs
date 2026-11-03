using InstAlertPlatform.API.Resources;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Profiles.Application.Services;
using InstAlertPlatform.API.Profiles.Domain.Model.Commands;
using InstAlertPlatform.API.Profiles.Domain.Model.Queries;
using InstAlertPlatform.API.Profiles.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Profiles.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Profiles.Interfaces.Rest;
[ApiController, Route("api/v1/[controller]"), Tags("Profiles")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class ProfilesController(IProfileQueryService profileQueries, IProfileCommandService profileCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserProfileResource), 200)]
    public async Task<IActionResult> Get(string userId, CancellationToken cancellationToken)
    {
        var snapshot = await profileQueries.Handle(new GetUserProfileQuery(userId), cancellationToken);
        return Ok(UserProfileResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(UserProfileResource), 200)]
    public async Task<IActionResult> Update(string userId, UpdateUserProfileResource resource, CancellationToken cancellationToken)
    {
        var result = await profileCommands.Handle(UpdateUserProfileCommandFromResourceAssembler.ToCommandFromResource(resource, userId), cancellationToken);
        return ActionResultFromUpdateUserProfileResultAssembler.ToActionResultFromUpdateUserProfileResult(result, this, localizer, snapshot => Ok(UserProfileResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpGet("{userId}/preferences")]
    [ProducesResponseType(typeof(UserPreferencesResource), 200)]
    public async Task<IActionResult> Preferences(string userId, CancellationToken cancellationToken)
    {
        var snapshot = await profileQueries.Handle(new GetUserPreferencesQuery(userId), cancellationToken);
        return Ok(UserPreferencesResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpPut("{userId}/preferences")]
    [ProducesResponseType(typeof(UserPreferencesResource), 200)]
    public async Task<IActionResult> Locale(string userId, ChangeUserLocaleResource resource, CancellationToken cancellationToken)
    {
        var result = await profileCommands.Handle(ChangeUserLocaleCommandFromResourceAssembler.ToCommandFromResource(resource, userId), cancellationToken);
        return ActionResultFromChangeUserLocaleResultAssembler.ToActionResultFromChangeUserLocaleResult(result, this, localizer, snapshot => Ok(UserPreferencesResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }
}
