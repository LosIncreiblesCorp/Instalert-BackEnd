namespace InstAlertPlatform.API.Business.Application.Errors;
public enum EUpdateStaffInvitationError
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
