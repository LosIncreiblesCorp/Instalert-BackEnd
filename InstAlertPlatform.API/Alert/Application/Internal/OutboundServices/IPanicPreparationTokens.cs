using InstAlertPlatform.API.Alert.Domain.Model.ValueObjects;

namespace InstAlertPlatform.API.Alert.Application.Internal.OutboundServices;
public interface IPanicPreparationTokens
{
    string Protect(PanicPreparation value);
    PanicPreparation Unprotect(string token);
}
