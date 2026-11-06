namespace InstAlertPlatform.API.Payments.Application.Errors;
public enum EBeginSubscriptionCheckoutError
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
    PolicyUnavailable,
}
