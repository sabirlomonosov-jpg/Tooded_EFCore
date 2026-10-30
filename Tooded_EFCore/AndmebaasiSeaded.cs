using Microsoft.Data.SqlClient;

namespace Tooded_EFCore;

// Store only the explicitly selected database name, never a password or full
// connection string. User/environment connection settings remain authoritative.
public sealed class AndmebaasiSeaded
{
    public const string VaikeUhendus = @"Server=(localdb)\MSSQLLocalDB;Database=Tooded_DB;Trusted_Connection=True;Encrypt=False;";
    public static AndmebaasiSeaded Kasutaja { get; } = new(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ToodedApp", "database-name.txt"));

    private readonly string fail;
    public AndmebaasiSeaded(string fail) => this.fail = fail;

    public string Loe(string? keskkonnaUhendus)
    {
        if (!string.IsNullOrWhiteSpace(keskkonnaUhendus)) return keskkonnaUhendus;
        if (!File.Exists(fail)) return VaikeUhendus;
        string nimi = File.ReadAllText(fail).Trim();
        KontrolliNime(nimi);
        return new SqlConnectionStringBuilder(VaikeUhendus) { InitialCatalog = nimi }.ConnectionString;
    }

    public static string LooUusUhendus() => new SqlConnectionStringBuilder(VaikeUhendus)
    {
        InitialCatalog = "Tooded_App_" + Guid.NewGuid().ToString("N")
    }.ConnectionString;

    public void SalvestaNimi(string nimi)
    {
        KontrolliNime(nimi);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(fail))!);
        string ajutine = fail + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(ajutine, nimi);
            File.Move(ajutine, fail, overwrite: true);
        }
        finally { if (File.Exists(ajutine)) File.Delete(ajutine); }
    }

    private static void KontrolliNime(string nimi)
    {
        if (!nimi.StartsWith("Tooded_App_", StringComparison.Ordinal) ||
            nimi.Length != 43 || !nimi[11..].All(Uri.IsHexDigit))
            throw new InvalidOperationException("Salvestatud andmebaasi valik ei ole korrektne. Kontrolli faili: database-name.txt.");
    }
}
