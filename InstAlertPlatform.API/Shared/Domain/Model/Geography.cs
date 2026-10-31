namespace InstAlertPlatform.API.Shared.Domain.Model;
public static class Geography
{
    public static double DistanceMeters(double latitude, double longitude, double otherLatitude, double otherLongitude)
    {
        Guard.Coordinates(latitude, longitude, true);
        Guard.Coordinates(otherLatitude, otherLongitude, true);
        static double Rad(double value) => value * Math.PI / 180;
        var a = Math.Pow(Math.Sin(Rad(otherLatitude - latitude) / 2), 2) + Math.Cos(Rad(latitude)) * Math.Cos(Rad(otherLatitude)) * Math.Pow(Math.Sin(Rad(otherLongitude - longitude) / 2), 2);
        return 6371000 * 2 * Math.Asin(Math.Sqrt(Math.Clamp(a, 0, 1)));
    }
}
