using Microsoft.Data.SqlClient;

namespace Tooded_EFCore;

// Retry only startup, never replay the purchase/PDF transaction automatically.
public static class KaivituseKordus
{
    public static void Kaivita(Action tegevus, Action<TimeSpan>? oota = null)
    {
        oota ??= aeg => Thread.Sleep(aeg);
        for (int katse = 1; ; katse++)
        {
            try { tegevus(); return; }
            catch (Exception ex) when (katse < 3 && OnAjutine(ex))
            {
                oota(TimeSpan.FromSeconds(katse));
            }
        }
    }

    public static bool OnAjutine(Exception viga)
    {
        if (OnLigipaasuViga(viga)) return false;
        for (Exception? ex = viga; ex != null; ex = ex.InnerException)
        {
            if (ex is TimeoutException) return true;
            if (ex is SqlException sql && sql.Errors.Cast<SqlError>().Any(e =>
                e.Number is -2 or 20 or 53 or 64 or 233 or 10928 or 10929
                    or 17142 or 18401 or 40197 or 40501 or 40613
                    or 10053 or 10054 or 10060 or 49918 or 49919 or 49920)) return true;
        }
        return false;
    }

    public static bool OnLigipaasuViga(Exception viga)
    {
        for (Exception? ex = viga; ex != null; ex = ex.InnerException)
            if (ex is SqlException sql && OnLigipaasuViga(sql.Errors.Cast<SqlError>().Select(e => e.Number)))
                return true;
        return false;
    }

    internal static bool OnLigipaasuViga(IEnumerable<int> numbrid) =>
        numbrid.Any(n => n is 4060 or 18456);

    public static string Kirjelda(Exception viga)
    {
        var read = new List<string>();
        for (Exception? ex = viga; ex != null; ex = ex.InnerException)
        {
            if (ex is SqlException sql)
            {
                foreach (SqlError error in sql.Errors)
                    read.Add($"SQL Server {error.Number}: {error.Message}");
            }
            else read.Add(ex.Message);
        }
        return string.Join("\n\n", read.Distinct());
    }
}
