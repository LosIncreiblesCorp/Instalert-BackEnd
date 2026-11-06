namespace InstAlertPlatform.API.Contacts.Application.Errors;
public enum ECreateEmergencyContactError
{
    InvalidInput,
    Unauthenticated,
    Forbidden,
    NotFound,
    Conflict,
    ConcurrencyConflict,
    UnexpectedError,
}
