using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

public static class AndmebaasiAlgataja
{
    public static void Kaivita(Action<ToodedContext> algandmed, string? uhendus = null)
    {
        KaivituseKordus.Kaivita(() =>
        {
            using var db = uhendus == null ? new ToodedContext() : new ToodedContext(
                new DbContextOptionsBuilder<ToodedContext>().UseSqlServer(uhendus).Options);
            try
            {
                Valmista(db);
                algandmed(db);
            }
            catch (Exception ex) when (KaivituseKordus.OnAjutine(ex))
            {
                // A failed startup must not leave a stale pooled LocalDB connection
                // for the next attempt. Other databases' pools remain untouched.
                if (db.Database.GetDbConnection() is SqlConnection sql)
                    SqlConnection.ClearPool(sql);
                throw;
            }
        });
    }

    public static void Valmista(ToodedContext db)
    {
        var seaded = new SqlConnectionStringBuilder(db.Database.GetConnectionString());
        string nimi = seaded.InitialCatalog;
        if (string.IsNullOrWhiteSpace(nimi))
            throw new InvalidOperationException("Ühenduses puudub andmebaasi nimi (Database).");

        // Check the server catalogue, not just EF's connection-based existence probe.
        // A nonpooled session releases its setup lock even when initialization fails.
        seaded.InitialCatalog = "master";
        seaded.AttachDBFilename = "";
        seaded.Pooling = false;
        using var master = new SqlConnection(seaded.ConnectionString);
        master.Open();
        using (var lukusta = master.CreateCommand())
        {
            lukusta.CommandText = """
                DECLARE @result int;
                EXEC @result = sys.sp_getapplock @Resource = @resource,
                    @LockMode = 'Exclusive', @LockOwner = 'Session', @LockTimeout = 30000;
                SELECT @result;
                """;
            lukusta.CommandTimeout = 35;
            lukusta.Parameters.AddWithValue("@resource", "ToodedApp:setup:" + nimi);
            if (Convert.ToInt32(lukusta.ExecuteScalar()) < 0)
                throw new InvalidOperationException("Teine rakendus uuendab andmebaasi. Proovi mõne hetke pärast uuesti.");
        }
        using (var kontroll = master.CreateCommand())
        {
            kontroll.CommandText = "SELECT state_desc FROM sys.databases WHERE name = @nimi";
            kontroll.Parameters.AddWithValue("@nimi", nimi);
            var olek = kontroll.ExecuteScalar() as string;
            if (olek == null)
            {
                using var loo = master.CreateCommand();
                using var nimed = new SqlCommandBuilder();
                loo.CommandText = "CREATE DATABASE " + nimed.QuoteIdentifier(nimi);
                loo.CommandTimeout = 60;
                try { loo.ExecuteNonQuery(); }
                catch (SqlException ex) when (ex.Number == 1801)
                {
                    // Another client created it after our check. Open it below;
                    // do not delete, rename, or overwrite the existing database.
                }
            }
            else if (olek != "ONLINE")
                throw new InvalidOperationException($"Andmebaas '{nimi}' on olekus {olek}. Kontrolli seda SQL Serveris; andmeid ei kustutatud.");
        }

        // Fail with the real access/attachment error before EF can try CREATE DATABASE.
        db.Database.OpenConnection();
        try { db.Database.Migrate(); }
        catch (SqlException ex) when (ex.Number == 2714)
        {
            throw new InvalidOperationException(
                $"Andmebaas '{nimi}' sisaldab juba tabeleid, kuid migratsioonide ajalugu ei vasta rakendusele. " +
                "Andmeid ei kustutatud. Säilita varukoopia ja kontrolli __EFMigrationsHistory tabelit. " +
                "Eraldi uue poe jaoks määra TOODED_CONNECTION_STRING kaudu teine andmebaasi nimi.", ex);
        }
        finally { db.Database.CloseConnection(); }
    }
}
