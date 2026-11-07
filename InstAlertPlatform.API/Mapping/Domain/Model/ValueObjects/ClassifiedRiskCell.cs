namespace InstAlertPlatform.API.Mapping.Domain.Model.ValueObjects;
public sealed record ClassifiedRiskCell(string Id, string Level, int IncidentCount, double[][] Bounds)
{
    public const int WindowDays = 30;
    private const double CellSize = 250;
    private const double EarthRadius = 6378137;
    private static (long X, long Y) Cell(double latitude, double longitude) => ((long)Math.Floor(EarthRadius * longitude * Math.PI / 180 / CellSize), (long)Math.Floor(EarthRadius * Math.Log(Math.Tan(Math.PI / 4 + latitude * Math.PI / 360)) / CellSize));
    public static string CellId(double latitude, double longitude)
    {
        var cell = Cell(latitude, longitude);
        return $"cell-{cell.X}-{cell.Y}";
    }

    private static double[] Point(double x, double y) => [x / EarthRadius * 180 / Math.PI, (2 * Math.Atan(Math.Exp(y / EarthRadius)) - Math.PI / 2) * 180 / Math.PI];
    public static IReadOnlyList<ClassifiedRiskCell> Classify(IEnumerable<RiskObservation> observations, DateTimeOffset asOf)
    {
        var start = asOf.AddDays(-WindowDays);
        return observations.Where(o => o.RegisteredAt >= start && o.RegisteredAt <= asOf && double.IsFinite(o.Latitude) && double.IsFinite(o.Longitude) && Math.Abs(o.Latitude) < 85 && Math.Abs(o.Longitude) <= 180).DistinctBy(o => o.Id).GroupBy(o => Cell(o.Latitude, o.Longitude)).Select(group => new ClassifiedRiskCell($"cell-{group.Key.X}-{group.Key.Y}", group.Count() <= 5 ? "low" : group.Count() <= 14 ? "medium" : "high", group.Count(), [Point(group.Key.X * CellSize, group.Key.Y * CellSize), Point((group.Key.X + 1) * CellSize, group.Key.Y * CellSize), Point((group.Key.X + 1) * CellSize, (group.Key.Y + 1) * CellSize), Point(group.Key.X * CellSize, (group.Key.Y + 1) * CellSize), Point(group.Key.X * CellSize, group.Key.Y * CellSize)])).OrderBy(cell => cell.Id).ToList();
    }
}
