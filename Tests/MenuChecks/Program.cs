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
Console.WriteLine($"{count}/{count} checks passed. SQL generation only; no live LocalDB connection was made.");
