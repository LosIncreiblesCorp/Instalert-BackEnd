namespace InstAlertPlatform.API.Business.Application.Errors;
public enum EInviteStaffMemberError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
    CapacityExceeded,
    ProviderUnavailable,
    InvalidTransition,
    IdentityUnavailable,
}
