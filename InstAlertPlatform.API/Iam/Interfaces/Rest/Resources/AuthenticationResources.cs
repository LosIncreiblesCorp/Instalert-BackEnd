using System.ComponentModel.DataAnnotations;

namespace InstAlertPlatform.API.Iam.Interfaces.Rest.Resources;

public sealed record RegisterAdministratorResource(
    [Required(ErrorMessage = "IdentityEmailInvalid"), EmailAddress(ErrorMessage = "IdentityEmailInvalid"), StringLength(254)] string Email,
    [Required(ErrorMessage = "IdentityPasswordInvalid"), StringLength(72, MinimumLength = 10, ErrorMessage = "IdentityPasswordInvalid")] string Password);
public sealed record SignInResource(
    [Required(ErrorMessage = "IdentityEmailInvalid"), StringLength(254)] string Email,
    [Required(ErrorMessage = "IdentityPasswordInvalid"), StringLength(72)] string Password);
public sealed record ChangePasswordResource(
    [Required(ErrorMessage = "IdentityPasswordInvalid"), StringLength(72)] string CurrentPassword,
    [Required(ErrorMessage = "IdentityPasswordInvalid"), StringLength(72, MinimumLength = 10, ErrorMessage = "IdentityPasswordInvalid")] string NewPassword);
