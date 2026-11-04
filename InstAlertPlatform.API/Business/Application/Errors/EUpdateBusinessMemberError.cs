namespace InstAlertPlatform.API.Business.Application.Errors;
public enum EUpdateBusinessMemberError
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
