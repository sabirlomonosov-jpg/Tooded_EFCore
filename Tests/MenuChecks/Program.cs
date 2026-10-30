using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Tooded_EFCore;
using Tooded_EFCore.Migrations;

int count = 0;
void Check(string name, bool passed)
{
    if (!passed) throw new Exception("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
    count++;
}
var menu = MenuuAndmed.Andmed;
Check("17 distinct dishes and positive EUR prices", menu.Count == 17 &&
    menu.Select(x => x.Nimi).Distinct().Count() == 17 && menu.All(x => x.Hind > 0 && decimal.Round(x.Hind, 2) == x.Hind));
Check("all 16 food images mapped and filenames safe", menu.Select(x => x.PildiIndeks).Distinct().Order().SequenceEqual(Enumerable.Range(0, 16)) &&
    menu.All(x => Path.GetFileName(x.Pildifail) == x.Pildifail) && menu.Select(x => x.Pildifail).Distinct().Count() == 17);
using var db = new ToodedContext();
Check("three migrations discoverable in order", db.Database.GetMigrations().SequenceEqual(new[] {
    "20261023000100_InitialCreate", "20261023000200_LisaKasutajad", "20261030000100_McDonaldsMenu" }));
var migrator = db.GetService<IMigrator>();
string sql = migrator.GenerateScript(options: MigrationsSqlGenerationOptions.Idempotent);
Check("fresh database script contains all tables and menu", new[] { "CREATE TABLE [Toodetabel]", "CREATE TABLE [Kasutajad]", "Big Mac", "Happy Meal hamburgeriga" }.All(sql.Contains));
string upgrade = migrator.GenerateScript("20261023000200_LisaKasutajad", "20261030000100_McDonaldsMenu", MigrationsSqlGenerationOptions.Idempotent);
Check("existing database upgrade is guarded by migration history", upgrade.Contains("IF NOT EXISTS") && upgrade.Contains("20261030000100_McDonaldsMenu") && !upgrade.Contains("CREATE TABLE [Kasutajad]"));
Check("migration and catalogue prices/images/categories agree", menu.All(x =>
    upgrade.Contains("N'" + x.Nimi.Replace("'", "''") + "'") && upgrade.Contains("N'" + x.Pildifail + "'") &&
    upgrade.Contains(x.Hind.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)) && upgrade.Contains("N'" + x.Kategooria + "'")));
var operations = new McDonaldsMenu().UpOperations;
Check("data-only migration leaves schema and accounts intact", operations.Count == 1 && operations[0] is SqlOperation &&
    !upgrade.Contains("DROP TABLE") && !upgrade.Contains("TRUNCATE") && !upgrade.Contains("DELETE FROM [Kasutajad]"));
Check("replaced demo rows retain stock and identity", !upgrade.Contains("SET [Kogus]") && !upgrade.Contains("SET [Id]") &&
    upgrade.Split("UPDATE [Toodetabel]").Length - 1 == 17);
Check("no work after latest migration", string.IsNullOrWhiteSpace(migrator.GenerateScript("20261030000100_McDonaldsMenu", "20261030000100_McDonaldsMenu")));
Ostukorv.Tyhjenda();
Check("empty cart cannot proceed to checkout", !Ostukorv.SaabMaksta && Ostukorv.Kokku == 0);
var burger = new Toode { Id = 1, Toodenimetus = "Burger", Hind = 5.50m, Kogus = 5 };
var fries = new Toode { Id = 2, Toodenimetus = "Fries", Hind = 2.20m, Kogus = 3 };
Check("add items and aggregate duplicate products", Ostukorv.Lisa(burger, 2) && Ostukorv.Lisa(burger, 1) &&
    Ostukorv.Lisa(fries, 2) && Ostukorv.Ridad.Count == 2 && Ostukorv.Ridad[0].Kogus == 3 && Ostukorv.Kokku == 20.90m);
Check("reject invalid additions without mutating cart", !Ostukorv.Lisa(burger, 0) && !Ostukorv.Lisa(burger, -1) &&
    !Ostukorv.Lisa(burger, int.MaxValue) && Ostukorv.Ridad[0].Kogus == 3 && Ostukorv.Kokku == 20.90m);
Check("edit quantity updates line and receipt totals", Ostukorv.MuudaKogus(1, 5) &&
    Ostukorv.Ridad[0].Summa == 27.50m && Ostukorv.Kokku == 31.90m && Ostukorv.SaabMaksta);
Check("reject negative, excessive and missing-product edits", !Ostukorv.MuudaKogus(1, -1) &&
    !Ostukorv.MuudaKogus(1, 6) && !Ostukorv.MuudaKogus(99, 1) && Ostukorv.Ridad[0].Kogus == 5);
Check("zero quantity removes a row", Ostukorv.MuudaKogus(2, 0) && Ostukorv.Ridad.Count == 1 && Ostukorv.Kokku == 27.50m);
Ostukorv.VarskendaTooted(new[] { new Toode { Id = 1, Hind = 6m, Kogus = 2 } });
Check("stock and price refresh blocks unavailable quantities", Ostukorv.Kokku == 30m && !Ostukorv.SaabMaksta &&
    Ostukorv.Ridad[0].Kogus == 5);
Check("reducing quantity restores checkout", Ostukorv.MuudaKogus(1, 2) && Ostukorv.SaabMaksta && Ostukorv.Kokku == 12m);
Ostukorv.VarskendaTooted(Array.Empty<Toode>());
Check("deleted product remains removable but cannot be purchased", !Ostukorv.SaabMaksta &&
    !Ostukorv.MuudaKogus(1, 1) && Ostukorv.Eemalda(1) && Ostukorv.Ridad.Count == 0 && !Ostukorv.SaabMaksta);
Check("remove missing row is harmless", !Ostukorv.Eemalda(99));
Ostukorv.Lisa(burger, 1);
Ostukorv.Tyhjenda();
Check("successful checkout clears the shared cart", Ostukorv.Ridad.Count == 0 && Ostukorv.Kokku == 0 && !Ostukorv.SaabMaksta);
int attempts = 0;
var delays = new List<TimeSpan>();
KaivituseKordus.Kaivita(() =>
{
    attempts++;
    if (attempts < 3) throw new InvalidOperationException("EF wrapper", new TimeoutException("Cold start"));
}, delays.Add);
Check("startup retries nested transient failures with bounded backoff", attempts == 3 &&
    delays.SequenceEqual(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) }));
attempts = 0;
delays.Clear();
var permanent = new InvalidOperationException("Existing incompatible schema");
Exception? caught = null;
try { KaivituseKordus.Kaivita(() => { attempts++; throw permanent; }, delays.Add); }
catch (Exception ex) { caught = ex; }
Check("permanent failures preserve original exception and do not retry", attempts == 1 && delays.Count == 0 && ReferenceEquals(caught, permanent));
attempts = 0;
delays.Clear();
var timeout = new TimeoutException("LocalDB unavailable");
caught = null;
try { KaivituseKordus.Kaivita(() => { attempts++; throw timeout; }, delays.Add); }
catch (Exception ex) { caught = ex; }
Check("startup stops after three failed attempts", attempts == 3 && delays.Count == 2 && ReferenceEquals(caught, timeout));
attempts = 0;
delays.Clear();
KaivituseKordus.Kaivita(() => attempts++, delays.Add);
Check("healthy startup executes once without waiting", attempts == 1 && delays.Count == 0);
string detail = KaivituseKordus.Kirjelda(new InvalidOperationException("EF wrapper", new Exception("Underlying connection error")));
Check("startup error includes inner database failure", detail.Contains("EF wrapper") && detail.Contains("Underlying connection error"));
Check("purchase context keeps explicit-transaction-compatible execution strategy", !db.Database.CreateExecutionStrategy().RetriesOnFailure);
if (args.Contains("--sql")) SqlStartupChecks.Run(Check);
Console.WriteLine($"{count}/{count} checks passed. Live SQL Server checks run only with --sql.");
