using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Application.Internal.CommandServices;
public sealed class StaffInvitationCommandService(IBusinessMemberRepository members, IStaffInvitationRepository invitations, IOutboxMessageRepository outbox, IBusinessProfileQueryService access, IPlanQueryService limits, ICurrentActor actor, IUnitOfWork work, TimeProvider clock, IBusinessRevisionCommandService revisions) : IStaffInvitationCommandService
{
    public async Task<Result<InvitationSnapshot, EInviteStaffMemberError>> Handle(InviteStaffMemberCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var businessId = command.BusinessId;
            var details = command;
            await access.Handle(new RequireBusinessAccessQuery(businessId, actor.UserId, true), cancellationToken);
            await RequireSeat(businessId, cancellationToken);
            var invitation = new StaffInvitation(businessId, details.DisplayName, details.Email, clock.GetUtcNow());
            Guard.Require((await invitations.FindPendingByEmailAsync(invitation.Email, cancellationToken)).Count == 0, "conflict", "email");
            Guard.Require((await members.FindByEmailAsync(invitation.Email, cancellationToken)).Count == 0, "conflict", "email");
            var seatRevisionResult = (await revisions.Handle(new TouchBusinessSeatsCommand(businessId), cancellationToken));
            if (seatRevisionResult is Result<bool, ETouchBusinessSeatsError>.Failure seatRevisionFailure)
                throw new DomainException(seatRevisionFailure.Error switch
                {
                    ETouchBusinessSeatsError.NotFound => "not_found",
                    ETouchBusinessSeatsError.UnexpectedError => "unexpected",
                    _ => "unexpected"
                }, seatRevisionFailure.Field);
            await invitations.AddAsync(invitation, cancellationToken);
            await outbox.AddAsync(new OutboxMessage("business.invitation.requested.v1", new InvitationDeliveryFact(invitation.Id, businessId, actor.UserId, invitation.Email, invitation.DeliverySequence)), cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<InvitationSnapshot, EInviteStaffMemberError>.Success(ToInvitationSnapshot(invitation));
        }
        catch (DomainException error)
        {
            return new Result<InvitationSnapshot, EInviteStaffMemberError>.Failure(error.Code switch
            {
                "validation" => EInviteStaffMemberError.InvalidInput,
                "unauthenticated" => EInviteStaffMemberError.Unauthenticated,
                "forbidden" => EInviteStaffMemberError.Forbidden,
                "not_found" => EInviteStaffMemberError.NotFound,
                "conflict" => EInviteStaffMemberError.Conflict,
                "concurrency" => EInviteStaffMemberError.ConcurrencyConflict,
                "capacity" => EInviteStaffMemberError.CapacityExceeded,
                "transition" => EInviteStaffMemberError.InvalidTransition,
                "provider_unavailable" => EInviteStaffMemberError.ProviderUnavailable,
                "identity_unavailable" => EInviteStaffMemberError.IdentityUnavailable,
                "unexpected" => EInviteStaffMemberError.UnexpectedError,
                _ => EInviteStaffMemberError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<MemberSnapshot, EAcceptStaffInvitationError>> Handle(AcceptStaffInvitationCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var invitation = await invitations.FindByIdAsync(id, cancellationToken) ?? throw new DomainException("not_found");
            Guard.Require(!actor.IsAdministrator, "forbidden");
            var email = actor.VerifiedEmail;
            Guard.Require(!string.IsNullOrWhiteSpace(email), "identity_unavailable");
            Guard.Require(string.Equals(email, invitation.Email, StringComparison.OrdinalIgnoreCase), "forbidden");
            var current = (await members.FindByBusinessIdAndUserIdAsync(invitation.BusinessId.ToString(), actor.UserId, cancellationToken)).SingleOrDefault();
            if (invitation.Status == "accepted" && current is not null)
                return new Result<MemberSnapshot, EAcceptStaffInvitationError>.Success(ToMemberSnapshot(current));
            Guard.Require((await members.FindByUserIdAsync(actor.UserId, cancellationToken)).Count == 0, "conflict", "userId");
            Guard.Require((await members.FindByEmailAsync(invitation.Email, cancellationToken)).Count == 0, "conflict", "email");
            Guard.Require(invitation.Status == "pending" && current is null, "transition");
            var plan = await limits.Handle(new GetPlanLimitsQuery(invitation.BusinessId.ToString()), cancellationToken);
            var usage = await access.Handle(new GetBusinessSeatUsageQuery(invitation.BusinessId.ToString()), cancellationToken);
            Guard.Require(plan.SubscriptionActive && usage.ActiveEmployees + usage.ReservedSeats <= plan.MaxEmployees, "capacity");
            var member = new BusinessMember(invitation.BusinessId.ToString(), actor.UserId, invitation.DisplayName, invitation.Email);
            invitation.Accept();
            await members.AddAsync(member, cancellationToken);
            var seatRevisionResult = (await revisions.Handle(new TouchBusinessSeatsCommand(invitation.BusinessId.ToString()), cancellationToken));
            if (seatRevisionResult is Result<bool, ETouchBusinessSeatsError>.Failure seatRevisionFailure)
                throw new DomainException(seatRevisionFailure.Error switch
                {
                    ETouchBusinessSeatsError.NotFound => "not_found",
                    ETouchBusinessSeatsError.UnexpectedError => "unexpected",
                    _ => "unexpected"
                }, seatRevisionFailure.Field);
            await work.CompleteAsync(cancellationToken);
            return new Result<MemberSnapshot, EAcceptStaffInvitationError>.Success(ToMemberSnapshot(member));
        }
        catch (DomainException error)
        {
            return new Result<MemberSnapshot, EAcceptStaffInvitationError>.Failure(error.Code switch
            {
                "validation" => EAcceptStaffInvitationError.InvalidInput,
                "unauthenticated" => EAcceptStaffInvitationError.Unauthenticated,
                "forbidden" => EAcceptStaffInvitationError.Forbidden,
                "not_found" => EAcceptStaffInvitationError.NotFound,
                "conflict" => EAcceptStaffInvitationError.Conflict,
                "concurrency" => EAcceptStaffInvitationError.ConcurrencyConflict,
                "capacity" => EAcceptStaffInvitationError.CapacityExceeded,
                "transition" => EAcceptStaffInvitationError.InvalidTransition,
                "provider_unavailable" => EAcceptStaffInvitationError.ProviderUnavailable,
                "identity_unavailable" => EAcceptStaffInvitationError.IdentityUnavailable,
                "unexpected" => EAcceptStaffInvitationError.UnexpectedError,
                _ => EAcceptStaffInvitationError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<InvitationSnapshot, ECancelStaffInvitationError>> Handle(CancelStaffInvitationCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var invitation = await invitations.FindByIdAsync(id, cancellationToken) ?? throw new DomainException("not_found");
            await access.Handle(new RequireBusinessAccessQuery(invitation.BusinessId.ToString(), actor.UserId, true), cancellationToken);
            invitation.Cancel();
            var seatRevisionResult = (await revisions.Handle(new TouchBusinessSeatsCommand(invitation.BusinessId.ToString()), cancellationToken));
            if (seatRevisionResult is Result<bool, ETouchBusinessSeatsError>.Failure seatRevisionFailure)
                throw new DomainException(seatRevisionFailure.Error switch
                {
                    ETouchBusinessSeatsError.NotFound => "not_found",
                    ETouchBusinessSeatsError.UnexpectedError => "unexpected",
                    _ => "unexpected"
                }, seatRevisionFailure.Field);
            await work.CompleteAsync(cancellationToken);
            return new Result<InvitationSnapshot, ECancelStaffInvitationError>.Success(ToInvitationSnapshot(invitation));
        }
        catch (DomainException error)
        {
            return new Result<InvitationSnapshot, ECancelStaffInvitationError>.Failure(error.Code switch
            {
                "validation" => ECancelStaffInvitationError.InvalidInput,
                "unauthenticated" => ECancelStaffInvitationError.Unauthenticated,
                "forbidden" => ECancelStaffInvitationError.Forbidden,
                "not_found" => ECancelStaffInvitationError.NotFound,
                "conflict" => ECancelStaffInvitationError.Conflict,
                "concurrency" => ECancelStaffInvitationError.ConcurrencyConflict,
                "capacity" => ECancelStaffInvitationError.CapacityExceeded,
                "transition" => ECancelStaffInvitationError.InvalidTransition,
                "provider_unavailable" => ECancelStaffInvitationError.ProviderUnavailable,
                "identity_unavailable" => ECancelStaffInvitationError.IdentityUnavailable,
                "unexpected" => ECancelStaffInvitationError.UnexpectedError,
                _ => ECancelStaffInvitationError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<InvitationSnapshot, EResendStaffInvitationError>> Handle(ResendStaffInvitationCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var invitation = await invitations.FindByIdAsync(id, cancellationToken) ?? throw new DomainException("not_found");
            await access.Handle(new RequireBusinessAccessQuery(invitation.BusinessId.ToString(), actor.UserId, true), cancellationToken);
            invitation.Resend(clock.GetUtcNow());
            await outbox.AddAsync(new OutboxMessage("business.invitation.requested.v1", new InvitationDeliveryFact(invitation.Id, invitation.BusinessId.ToString(), actor.UserId, invitation.Email, invitation.DeliverySequence)), cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<InvitationSnapshot, EResendStaffInvitationError>.Success(ToInvitationSnapshot(invitation));
        }
        catch (DomainException error)
        {
            return new Result<InvitationSnapshot, EResendStaffInvitationError>.Failure(error.Code switch
            {
                "validation" => EResendStaffInvitationError.InvalidInput,
                "unauthenticated" => EResendStaffInvitationError.Unauthenticated,
                "forbidden" => EResendStaffInvitationError.Forbidden,
                "not_found" => EResendStaffInvitationError.NotFound,
                "conflict" => EResendStaffInvitationError.Conflict,
                "concurrency" => EResendStaffInvitationError.ConcurrencyConflict,
                "capacity" => EResendStaffInvitationError.CapacityExceeded,
                "transition" => EResendStaffInvitationError.InvalidTransition,
                "provider_unavailable" => EResendStaffInvitationError.ProviderUnavailable,
                "identity_unavailable" => EResendStaffInvitationError.IdentityUnavailable,
                "unexpected" => EResendStaffInvitationError.UnexpectedError,
                _ => EResendStaffInvitationError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<InvitationSnapshot, EUpdateStaffInvitationError>> Handle(UpdateStaffInvitationCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var invitation = await invitations.FindByIdAsync(id, cancellationToken) ?? throw new DomainException("not_found");
            await access.Handle(new RequireBusinessAccessQuery(invitation.BusinessId.ToString(), actor.UserId, true), cancellationToken);
            var email = Guard.Email(command.Email);
            Guard.Require((await invitations.FindPendingByEmailAsync(email, cancellationToken)).All(candidate => candidate.Id == invitation.Id), "conflict", "email");
            Guard.Require((await members.FindByEmailAsync(email, cancellationToken)).Count == 0, "conflict", "email");
            invitation.Update(command.DisplayName, email);
            await work.CompleteAsync(cancellationToken);
            return new Result<InvitationSnapshot, EUpdateStaffInvitationError>.Success(ToInvitationSnapshot(invitation));
        }
        catch (DomainException error)
        {
            return new Result<InvitationSnapshot, EUpdateStaffInvitationError>.Failure(error.Code switch
            {
                "validation" => EUpdateStaffInvitationError.InvalidInput,
                "unauthenticated" => EUpdateStaffInvitationError.Unauthenticated,
                "forbidden" => EUpdateStaffInvitationError.Forbidden,
                "not_found" => EUpdateStaffInvitationError.NotFound,
                "conflict" => EUpdateStaffInvitationError.Conflict,
                "concurrency" => EUpdateStaffInvitationError.ConcurrencyConflict,
                "capacity" => EUpdateStaffInvitationError.CapacityExceeded,
                "transition" => EUpdateStaffInvitationError.InvalidTransition,
                "provider_unavailable" => EUpdateStaffInvitationError.ProviderUnavailable,
                "identity_unavailable" => EUpdateStaffInvitationError.IdentityUnavailable,
                "unexpected" => EUpdateStaffInvitationError.UnexpectedError,
                _ => EUpdateStaffInvitationError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    private async Task RequireSeat(string businessId, CancellationToken cancellationToken)
    {
        var plan = await limits.Handle(new GetPlanLimitsQuery(businessId), cancellationToken);
        var usage = await access.Handle(new GetBusinessSeatUsageQuery(businessId), cancellationToken);
        Guard.Require(plan.SubscriptionActive && usage.ActiveEmployees + usage.ReservedSeats < plan.MaxEmployees, "capacity");
    }

    private static MemberSnapshot ToMemberSnapshot(BusinessMember m) => new(m.Id, m.BusinessId.ToString(), m.UserId, m.DisplayName, m.Email, m.Role, m.Status, m.Version);
    private static InvitationSnapshot ToInvitationSnapshot(StaffInvitation i) => new(i.Id, i.BusinessId.ToString(), i.DisplayName, i.Email, i.Status, i.LastRequestedAt, i.Version);
}
