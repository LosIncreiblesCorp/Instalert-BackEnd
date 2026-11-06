using InstAlertPlatform.API.Alert.Domain.Model.ValueObjects;

namespace InstAlertPlatform.API.Alert.Application.Services;
public interface IPanicPreparationTokens
{
    string Protect(PanicPreparation value);
    PanicPreparation Unprotect(string token);
}
