using InstAlertPlatform.API.Resources;
using Microsoft.Extensions.Localization;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Domain.Model.Commands;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest;
[ApiController, Route("api/v1/businesses/{businessId}/subscription"), Tags("Payments")]
[ProducesResponseType(typeof(ProblemDetails), 400), ProducesResponseType(typeof(ProblemDetails), 401), ProducesResponseType(typeof(ProblemDetails), 403), ProducesResponseType(typeof(ProblemDetails), 404), ProducesResponseType(typeof(ProblemDetails), 409), ProducesResponseType(typeof(ProblemDetails), 503)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
public sealed class SubscriptionsController(ISubscriptionQueryService subscriptionQueries, ISubscriptionCommandService subscriptionCommands, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, ProducesResponseType(200), ProducesResponseType(204)]
    [ProducesResponseType(typeof(SubscriptionResource), 200)]
    public async Task<IActionResult> Get(string businessId, CancellationToken cancellationToken)
    {
        var snapshot = await subscriptionQueries.Handle(new GetCurrentSubscriptionQuery(businessId), cancellationToken);
        return snapshot is null ? NoContent() : Ok(SubscriptionResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot));
    }

    [HttpPost("checkout")]
    [ProducesResponseType(typeof(SubscriptionCheckoutResource), 200)]
    public async Task<IActionResult> Begin(string businessId, BeginSubscriptionCheckoutResource resource, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommands.Handle(BeginSubscriptionCheckoutCommandFromResourceAssembler.ToCommandFromResource(resource, businessId), cancellationToken);
        return ActionResultFromBeginSubscriptionCheckoutResultAssembler.ToActionResultFromBeginSubscriptionCheckoutResult(result, this, localizer, snapshot => Ok(SubscriptionCheckoutResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("verification")]
    [ProducesResponseType(typeof(SubscriptionResource), 200)]
    public async Task<IActionResult> Verify(string businessId, VerifySubscriptionResource resource, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommands.Handle(VerifySubscriptionCommandFromResourceAssembler.ToCommandFromResource(resource, businessId), cancellationToken);
        return ActionResultFromVerifySubscriptionResultAssembler.ToActionResultFromVerifySubscriptionResult(result, this, localizer, snapshot => Ok(SubscriptionResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("cancellation")]
    [ProducesResponseType(typeof(SubscriptionResource), 200)]
    public async Task<IActionResult> Cancel(string businessId, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommands.Handle(new CancelSubscriptionRenewalCommand(businessId), cancellationToken);
        return ActionResultFromCancelSubscriptionRenewalResultAssembler.ToActionResultFromCancelSubscriptionRenewalResult(result, this, localizer, snapshot => Ok(SubscriptionResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpGet("/api/v1/businesses/{businessId}/billing-history")]
    [ProducesResponseType(typeof(List<PaymentResource>), 200)]
    public async Task<IActionResult> History(string businessId, CancellationToken cancellationToken)
    {
        var snapshot = await subscriptionQueries.Handle(new GetBillingHistoryQuery(businessId), cancellationToken);
        return Ok(snapshot.Select(PaymentResourceFromSnapshotAssembler.ToResourceFromSnapshot).ToList());
    }

    [HttpPost("plan-change")]
    [ProducesResponseType(typeof(SubscriptionCheckoutResource), 200)]
    public async Task<IActionResult> Change(string businessId, ChangeSubscriptionPlanResource resource, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommands.Handle(ChangeSubscriptionPlanCommandFromResourceAssembler.ToCommandFromResource(resource, businessId), cancellationToken);
        return ActionResultFromChangeSubscriptionPlanResultAssembler.ToActionResultFromChangeSubscriptionPlanResult(result, this, localizer, snapshot => Ok(SubscriptionCheckoutResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }

    [HttpPost("plan-change/verification")]
    [ProducesResponseType(typeof(SubscriptionResource), 200)]
    public async Task<IActionResult> VerifyChange(string businessId, VerifySubscriptionPlanChangeResource resource, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommands.Handle(VerifySubscriptionPlanChangeCommandFromResourceAssembler.ToCommandFromResource(resource, businessId), cancellationToken);
        return ActionResultFromVerifySubscriptionPlanChangeResultAssembler.ToActionResultFromVerifySubscriptionPlanChangeResult(result, this, localizer, snapshot => Ok(SubscriptionResourceFromSnapshotAssembler.ToResourceFromSnapshot(snapshot)));
    }
}
