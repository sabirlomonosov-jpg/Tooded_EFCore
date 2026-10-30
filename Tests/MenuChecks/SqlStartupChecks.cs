using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tooded_EFCore;

internal static class SqlStartupChecks
{
    // Use a throwaway server, never the application database. Each run owns only
    // unique databases below and drops those exact names when finished.
    public static void Run(Action<string, bool> check)
    {
        string connection = Environment.GetEnvironmentVariable("TOODED_TEST_CONNECTION_STRING")
            ?? throw new InvalidOperationException("Set TOODED_TEST_CONNECTION_STRING for --sql.");
        var masterSettings = new SqlConnectionStringBuilder(connection) { InitialCatalog = "master" };
        using var master = new SqlConnection(masterSettings.ConnectionString);
        master.Open();
        var names = new List<string>();
        string NewName()
        {
            string name = "Tooded_Test_" + Guid.NewGuid().ToString("N");
            names.Add(name);
            return name;
        }
        ToodedContext Context(string name)
        {
            var settings = new SqlConnectionStringBuilder(connection) { InitialCatalog = name };
            return new ToodedContext(new DbContextOptionsBuilder<ToodedContext>()
                .UseSqlServer(settings.ConnectionString).Options);
        }
        void Command(string sql)
        {
            using var command = new SqlCommand(sql, master) { CommandTimeout = 60 };
            command.ExecuteNonQuery();
        }
        try
        {
            string fresh = NewName();
            using (var db = Context(fresh))
            {
                AndmebaasiAlgataja.Valmista(db);
                check("SQL: new database receives schema and 17 dishes", db.Toodetabel.Count() == 17 &&
                    db.Database.GetAppliedMigrations().Count() == 3);
                var first = db.Toodetabel.OrderBy(t => t.Id).First();
                first.Hind = 9.99m;
                first.Kogus = 7;
                db.SaveChanges();
            }
            using (var db = Context(fresh))
            {
                AndmebaasiAlgataja.Valmista(db);
                var first = db.Toodetabel.OrderBy(t => t.Id).First();
                check("SQL: repeat startup preserves existing prices and stock", first.Hind == 9.99m &&
                    first.Kogus == 7 && db.Toodetabel.Count() == 17);
            }
            string existing = NewName();
            Command($"CREATE DATABASE [{existing}]");
            using (var db = Context(existing))
            {
                AndmebaasiAlgataja.Valmista(db);
                check("SQL: existing empty database migrates without CREATE DATABASE collision", db.Toodetabel.Count() == 17);
            }
            string simultaneous = NewName();
            Task.WaitAll(Enumerable.Range(0, 2).Select(_ => Task.Run(() =>
            {
                using var db = Context(simultaneous);
                AndmebaasiAlgataja.Valmista(db);
            })).ToArray());
            using (var db = Context(simultaneous))
                check("SQL: simultaneous startup applies migrations once", db.Toodetabel.Count() == 17 &&
                    db.Database.GetAppliedMigrations().Count() == 3);
        }
        finally
        {
            SqlConnection.ClearAllPools();
            foreach (var name in names)
                Command($"IF DB_ID(N'{name}') IS NOT NULL BEGIN ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{name}]; END");
        }
    }
}
