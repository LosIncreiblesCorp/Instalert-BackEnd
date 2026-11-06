using InstAlertPlatform.API.Alert.Domain.Model.ValueObjects;

namespace InstAlertPlatform.API.Alert.Application.Services;
public sealed record PanicPreparationSnapshot(string PreparationToken, DateTimeOffset NotBefore, DateTimeOffset ExpiresAt, double GracePeriodSeconds);
