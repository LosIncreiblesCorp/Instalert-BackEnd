using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Business.Application.Services;
public interface IBusinessMemberCommandService
{
    Task<Result<MemberSnapshot, EChangeBusinessMemberStatusError>> Handle(ChangeBusinessMemberStatusCommand command, CancellationToken cancellationToken = default);
    Task<Result<MemberSnapshot, EUpdateBusinessMemberError>> Handle(UpdateBusinessMemberCommand command, CancellationToken cancellationToken = default);
}
