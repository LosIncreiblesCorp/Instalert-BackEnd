using InstAlertPlatform.API.Alert.Application.Services;
using InstAlertPlatform.API.Alert.Domain.Model.ValueObjects;
using System.Security.Cryptography;
using System.Text.Json;
using InstAlertPlatform.API.Shared.Domain.Model;
using Microsoft.AspNetCore.DataProtection;

namespace InstAlertPlatform.API.Alert.Infrastructure;
public sealed class PanicPreparationTokens(IDataProtectionProvider provider) : IPanicPreparationTokens
{
    private readonly IDataProtector protector = provider.CreateProtector("InstAlert.PanicPreparation.v1");
    public string Protect(PanicPreparation value) => protector.Protect(JsonSerializer.Serialize(value));
    public PanicPreparation Unprotect(string token)
    {
        try
        {
            return JsonSerializer.Deserialize<PanicPreparation>(protector.Unprotect(token)) ?? throw new DomainException("validation");
        }
        catch (CryptographicException)
        {
            throw new DomainException("validation", "preparationToken");
        }
        catch (JsonException)
        {
            throw new DomainException("validation", "preparationToken");
        }
    }
}
