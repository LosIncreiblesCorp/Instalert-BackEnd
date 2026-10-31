using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
public static class PersistenceExceptionClassifier
{
    public static string? Classify(Exception exception)
    {
        if (exception is DbUpdateConcurrencyException)
            return "concurrency";
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is not DbException)
                continue;
            if (current.GetType().Name == "MySqlException" && ReadNumber(current, "Number")is int mysql)
                return mysql switch
                {
                    1062 or 1451 or 1452 => "conflict",
                    1040 or 1042 or 1043 or 1045 or 1129 or 1130 or 2002 or 2003 or 2006 or 2013 => "provider_unavailable",
                    _ => "unexpected"
                };
            if (current.GetType().Name == "SqliteException" && ReadNumber(current, "SqliteExtendedErrorCode")is int sqlite)
                return sqlite switch
                {
                    1555 or 2067 or 787 => "conflict",
                    5 or 6 => "provider_unavailable",
                    _ => "unexpected"
                };
            return "unexpected";
        }

        return exception is DbUpdateException ? "unexpected" : null;
    }

    private static int? ReadNumber(Exception exception, string property) => exception.GetType().GetProperty(property)?.GetValue(exception)is int number ? number : null;
}
