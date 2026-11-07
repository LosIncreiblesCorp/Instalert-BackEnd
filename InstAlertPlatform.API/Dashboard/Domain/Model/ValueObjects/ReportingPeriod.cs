using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Dashboard.Domain.Model.ValueObjects;
public sealed record ReportingPeriod
{
    private ReportingPeriod(string? month, DateTimeOffset start, DateTimeOffset end)
    {
        Month = month;
        Start = start;
        End = end;
    }

    public string? Month { get; }
    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }

    public static string CurrentMonth(DateTimeOffset now) => TimeZoneInfo.ConvertTime(now, TimeZoneInfo.FindSystemTimeZoneById("America/Lima")).ToString("yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
    public static ReportingPeriod Bounds(string month)
    {
        Guard.Require(System.Text.RegularExpressions.Regex.IsMatch(month ?? "", @"^[2-9][0-9]{3}-(0[1-9]|1[0-2])$"), "validation", "month");
        var start = new DateTimeOffset(int.Parse(month![..4]), int.Parse(month[5..]), 1, 0, 0, 0, TimeSpan.FromHours(-5));
        Guard.Require(start.Year < 9999, "validation", "month");
        return new ReportingPeriod(month, start, start.AddMonths(1));
    }
}
