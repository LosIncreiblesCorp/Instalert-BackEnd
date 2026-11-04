namespace InstAlertPlatform.API.Business.Application.Errors;
public enum EUpdateBusinessError
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
