using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using InstAlertPlatform.API.Shared.Application.Patterns;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace InstAlertPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
public sealed class UtcIsoTimestampJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => DateTimeOffset.TryParse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var value) ? value : throw new JsonException("Invalid ISO timestamp.");
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) => writer.WriteStringValue(value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture));
}
