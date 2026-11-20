using InstAlertPlatform.API.Iam.Application.Services.Snapshots;
using InstAlertPlatform.API.Iam.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Iam.Application.Errors;

namespace InstAlertPlatform.API.Iam.Application.CommandServices;
public interface IUserCommandService
{
    Task<Result<AuthenticationSnapshot, EIdentityError>> Handle(RegisterAdministratorCommand command, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationSnapshot, EIdentityError>> Handle(SignInCommand command, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationSnapshot, EIdentityError>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken = default);
    Task RevokeCurrentTokens(CancellationToken cancellationToken = default);
}
