using InstAlertPlatform.API.Iam.Application.Services.Snapshots;
using System.Text;
using InstAlertPlatform.API.Iam.Application.CommandServices;
using InstAlertPlatform.API.Iam.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Domain.Model.Aggregates;
using InstAlertPlatform.API.Iam.Domain.Model.Commands;
using InstAlertPlatform.API.Iam.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Iam.Application.Errors;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Iam.Application.Internal.CommandServices;

public sealed class UserCommandService(IUserRepository users, IHashingService hashing, ITokenService tokens,
    ICurrentActor actor, IUnitOfWork work, TimeProvider clock) : IUserCommandService
{
    public async Task<Result<AuthenticationSnapshot, EIdentityError>> Handle(RegisterAdministratorCommand command, CancellationToken cancellationToken = default)
    {
        var email = Guard.Email(command.Email).ToLowerInvariant();
        ValidatePassword(command.Password);
        if (await users.FindByEmailAsync(email, cancellationToken) is not null)
            return new Result<AuthenticationSnapshot, EIdentityError>.Failure(EIdentityError.EmailRegistered);
        var user = new User(email, hashing.HashPassword(command.Password), "administrator");
        await users.AddAsync(user, cancellationToken);
        await work.CompleteAsync(cancellationToken);
        return new Result<AuthenticationSnapshot, EIdentityError>.Success(tokens.Create(user));
    }
    public async Task<Result<AuthenticationSnapshot, EIdentityError>> Handle(SignInCommand command, CancellationToken cancellationToken = default)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(command.Password) || Encoding.UTF8.GetByteCount(command.Password) > 72)
            return new Result<AuthenticationSnapshot, EIdentityError>.Failure(EIdentityError.InvalidCredentials);
        var user = await users.FindByEmailAsync(email, cancellationToken);
        if (user is null || !hashing.VerifyPassword(command.Password, user.PasswordHash)
            || !user.HasValidCredentials(clock.GetUtcNow()))
            return new Result<AuthenticationSnapshot, EIdentityError>.Failure(EIdentityError.InvalidCredentials);
        return new Result<AuthenticationSnapshot, EIdentityError>.Success(tokens.Create(user));
    }
    public async Task<Result<AuthenticationSnapshot, EIdentityError>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByIdAsync(actor.UserId, cancellationToken) ?? throw new DomainException("unauthenticated");
        Guard.Require(user.HasValidCredentials(clock.GetUtcNow()), "unauthenticated");
        if (string.IsNullOrEmpty(command.CurrentPassword) || Encoding.UTF8.GetByteCount(command.CurrentPassword) > 72
            || !hashing.VerifyPassword(command.CurrentPassword, user.PasswordHash))
            return new Result<AuthenticationSnapshot, EIdentityError>.Failure(EIdentityError.InvalidCredentials);
        ValidatePassword(command.NewPassword);
        Guard.Require(command.NewPassword != command.CurrentPassword, "validation", "newPassword");
        user.ChangePassword(hashing.HashPassword(command.NewPassword));
        await work.CompleteAsync(cancellationToken);
        return new Result<AuthenticationSnapshot, EIdentityError>.Success(tokens.Create(user));
    }
    public async Task RevokeCurrentTokens(CancellationToken cancellationToken = default)
    {
        var user = await users.FindByIdAsync(actor.UserId, cancellationToken) ?? throw new DomainException("unauthenticated");
        user.RevokeTokens();
        await work.CompleteAsync(cancellationToken);
    }
    private static void ValidatePassword(string password)
    {
        Guard.Require(password.Length >= 10 && Encoding.UTF8.GetByteCount(password) <= 72
            && password.Any(char.IsLetter) && password.Any(char.IsDigit), "validation", "password");
    }
}
