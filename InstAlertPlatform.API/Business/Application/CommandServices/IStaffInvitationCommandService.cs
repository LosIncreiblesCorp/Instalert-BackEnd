using InstAlertPlatform.API.Business.Application.Services.Snapshots;
using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Business.Application.CommandServices;
public interface IStaffInvitationCommandService
{
    Task<Result<InvitationSnapshot, EInviteStaffMemberError>> Handle(InviteStaffMemberCommand command, CancellationToken cancellationToken = default);
    Task<Result<MemberSnapshot, EAcceptStaffInvitationError>> Handle(AcceptStaffInvitationCommand command, CancellationToken cancellationToken = default);
    Task<Result<InvitationSnapshot, ECancelStaffInvitationError>> Handle(CancelStaffInvitationCommand command, CancellationToken cancellationToken = default);
    Task<Result<InvitationSnapshot, EResendStaffInvitationError>> Handle(ResendStaffInvitationCommand command, CancellationToken cancellationToken = default);
}
