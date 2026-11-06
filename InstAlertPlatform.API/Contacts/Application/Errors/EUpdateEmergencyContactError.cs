namespace InstAlertPlatform.API.Contacts.Application.Errors;
public enum EUpdateEmergencyContactError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
}
