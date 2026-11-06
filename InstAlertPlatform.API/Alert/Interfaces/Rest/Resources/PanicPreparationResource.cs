using InstAlertPlatform.API.Alert.Domain.Model.ValueObjects;

namespace InstAlertPlatform.API.Alert.Interfaces.Rest.Resources;
public sealed record PanicPreparationResource(string PreparationToken, DateTimeOffset NotBefore, DateTimeOffset ExpiresAt, double GracePeriodSeconds);
