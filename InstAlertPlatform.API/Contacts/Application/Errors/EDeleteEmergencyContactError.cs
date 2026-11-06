namespace InstAlertPlatform.API.Contacts.Application.Errors;
public enum EDeleteEmergencyContactError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
}
