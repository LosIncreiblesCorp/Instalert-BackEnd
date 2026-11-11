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
public sealed class BusinessMemberCommandService(IBusinessMemberRepository members, IStaffInvitationRepository invitations, IBusinessProfileQueryService access, IPlanQueryService limits, ICurrentActor actor, IUnitOfWork work, IBusinessRevisionCommandService revisions) : IBusinessMemberCommandService
{
    public async Task<Result<MemberSnapshot, EChangeBusinessMemberStatusError>> Handle(ChangeBusinessMemberStatusCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var active = command.Active;
            var member = await members.FindByIdAsync(id, cancellationToken) ?? throw new DomainException("not_found");
            await access.Handle(new RequireBusinessAccessQuery(member.BusinessId.ToString(), actor.UserId, true), cancellationToken);
            if ((member.Status == "active") == active)
                return new Result<MemberSnapshot, EChangeBusinessMemberStatusError>.Success(ToMemberSnapshot(member));
            if (active)
                await RequireSeat(member.BusinessId.ToString(), cancellationToken);
            var seatRevisionResult = (await revisions.Handle(new TouchBusinessSeatsCommand(member.BusinessId.ToString()), cancellationToken));
            if (seatRevisionResult is Result<bool, ETouchBusinessSeatsError>.Failure seatRevisionFailure)
                throw new DomainException(seatRevisionFailure.Error switch
                {
                    ETouchBusinessSeatsError.NotFound => "not_found",
                    ETouchBusinessSeatsError.UnexpectedError => "unexpected",
                    _ => "unexpected"
                }, seatRevisionFailure.Field);
            member.SetActive(active);
            await work.CompleteAsync(cancellationToken);
            return new Result<MemberSnapshot, EChangeBusinessMemberStatusError>.Success(ToMemberSnapshot(member));
        }
        catch (DomainException error)
        {
            return new Result<MemberSnapshot, EChangeBusinessMemberStatusError>.Failure(error.Code switch
            {
                "validation" => EChangeBusinessMemberStatusError.InvalidInput,
                "unauthenticated" => EChangeBusinessMemberStatusError.Unauthenticated,
                "forbidden" => EChangeBusinessMemberStatusError.Forbidden,
                "not_found" => EChangeBusinessMemberStatusError.NotFound,
                "conflict" => EChangeBusinessMemberStatusError.Conflict,
                "concurrency" => EChangeBusinessMemberStatusError.ConcurrencyConflict,
                "capacity" => EChangeBusinessMemberStatusError.CapacityExceeded,
                "transition" => EChangeBusinessMemberStatusError.InvalidTransition,
                "provider_unavailable" => EChangeBusinessMemberStatusError.ProviderUnavailable,
                "identity_unavailable" => EChangeBusinessMemberStatusError.IdentityUnavailable,
                "unexpected" => EChangeBusinessMemberStatusError.UnexpectedError,
                _ => EChangeBusinessMemberStatusError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<MemberSnapshot, EUpdateBusinessMemberError>> Handle(UpdateBusinessMemberCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var details = command;
            var member = await members.FindByIdAsync(id, cancellationToken) ?? throw new DomainException("not_found");
            await access.Handle(new RequireBusinessAccessQuery(member.BusinessId.ToString(), actor.UserId, true), cancellationToken);
            var email = Guard.Email(details.Email);
            var existing = await members.FindByEmailAsync(email, cancellationToken);
            Guard.Require(existing.All(candidate => candidate.Id == member.Id), "conflict", "email");
            Guard.Require((await invitations.FindPendingByEmailAsync(email, cancellationToken)).Count == 0, "conflict", "email");
            member.Update(details.DisplayName, email);
            await work.CompleteAsync(cancellationToken);
            return new Result<MemberSnapshot, EUpdateBusinessMemberError>.Success(ToMemberSnapshot(member));
        }
        catch (DomainException error)
        {
            return new Result<MemberSnapshot, EUpdateBusinessMemberError>.Failure(error.Code switch
            {
                "validation" => EUpdateBusinessMemberError.InvalidInput,
                "unauthenticated" => EUpdateBusinessMemberError.Unauthenticated,
                "forbidden" => EUpdateBusinessMemberError.Forbidden,
                "not_found" => EUpdateBusinessMemberError.NotFound,
                "conflict" => EUpdateBusinessMemberError.Conflict,
                "concurrency" => EUpdateBusinessMemberError.ConcurrencyConflict,
                "capacity" => EUpdateBusinessMemberError.CapacityExceeded,
                "transition" => EUpdateBusinessMemberError.InvalidTransition,
                "provider_unavailable" => EUpdateBusinessMemberError.ProviderUnavailable,
                "identity_unavailable" => EUpdateBusinessMemberError.IdentityUnavailable,
                "unexpected" => EUpdateBusinessMemberError.UnexpectedError,
                _ => EUpdateBusinessMemberError.UnexpectedError
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
}
