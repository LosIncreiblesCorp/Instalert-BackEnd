using System.Net.Mail;
using System.Text.RegularExpressions;

namespace InstAlertPlatform.API.Shared.Domain.Model;
public static class Guard
{
    public static void Require(bool condition, string code = "validation", string field = "")
    {
        if (!condition)
            throw new DomainException(code, field);
    }

    public static string Text(string? value, int max, string field)
    {
        var text = value?.Trim() ?? "";
        Require(text.Length > 0 && text.Length <= max, "validation", field);
        return text;
    }

    public static string Email(string? value, string field = "email")
    {
        var text = Text(value, 254, field).ToLowerInvariant();
        Require(MailAddress.TryCreate(text, out var address) && address.Address == text, "validation", field);
        return text;
    }

    public static string Digits(string? value, int length, string field)
    {
        Require(value is not null && value.Length == length && value.All(c => c is >= '0' and <= '9'), "validation", field);
        return value!;
    }

    public static void Coordinates(double? latitude, double? longitude, bool required = false)
    {
        Require(!required || latitude.HasValue && longitude.HasValue, "validation", "coordinates");
        Require(latitude.HasValue == longitude.HasValue, "validation", "coordinates");
        if (latitude.HasValue)
            Require(double.IsFinite(latitude.Value) && double.IsFinite(longitude!.Value) && Math.Abs(latitude.Value) <= 90 && Math.Abs(longitude.Value) <= 180, "validation", "coordinates");
    }

    public static string Phone(string? value, bool required = false)
    {
        var text = Regex.Replace(value?.Trim() ?? "", @"[\s()-]", "");
        Require(!required && text.Length == 0 || Regex.IsMatch(text, @"^\+?[1-9][0-9]{6,14}$"), "validation", "phone");
        return text;
    }
}
