namespace InstAlertPlatform.API.Payments.Application.Errors;
public enum EResumeSubscriptionRenewalError
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
}
