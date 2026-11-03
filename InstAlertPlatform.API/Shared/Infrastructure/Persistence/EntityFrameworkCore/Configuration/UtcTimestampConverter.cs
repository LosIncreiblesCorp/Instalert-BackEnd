using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
public sealed class UtcTimestampConverter() : ValueConverter<DateTimeOffset, DateTime>(value => value.UtcDateTime, value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));
