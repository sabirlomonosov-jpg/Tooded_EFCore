using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Tooded_EFCore;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            Kaustad.Loo();
            NaidisTooted.ValmistaPildid();
            if (!KaivitaAndmebaas()) return;
            Application.Run(new LoginForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Programmi käivitamine ebaõnnestus.\n" +
                "Kontrolli SQL Server LocalDB ühendust ja olemasoleva andmebaasi ligipääsu.\n" +
                "Ära kustuta olemasolevat andmebaasi.\n\n" + KaivituseKordus.Kirjelda(ex),
                "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static bool KaivitaAndmebaas()
    {
        string? keskkond = Environment.GetEnvironmentVariable("TOODED_CONNECTION_STRING");
        string uhendus = AndmebaasiSeaded.Kasutaja.Loe(keskkond);
        bool uusValik = false;
        while (true)
        {
            try
            {
                AndmebaasiAlgataja.Kaivita(LisaAlgandmed, uhendus);
                // Remember the selection only after migrations and seeding succeed.
                if (uusValik)
                    AndmebaasiSeaded.Kasutaja.SalvestaNimi(new SqlConnectionStringBuilder(uhendus).InitialCatalog);
                return true;
            }
            catch (Exception ex)
            {
                var seaded = new SqlConnectionStringBuilder(uhendus);
                bool saabLuua = string.IsNullOrWhiteSpace(keskkond)
                    && seaded.IntegratedSecurity
                    && seaded.DataSource.StartsWith(@"(localdb)\", StringComparison.OrdinalIgnoreCase)
                    && KaivituseKordus.OnLigipaasuViga(ex);
                using var aken = new AndmebaasiVeaVorm(ex, seaded.InitialCatalog, saabLuua);
                if (aken.ShowDialog() != DialogResult.OK) return false;
                uhendus = AndmebaasiSeaded.LooUusUhendus();
                uusValik = true;
            }
        }
    }

    private static void LisaAlgandmed(ToodedContext db)
    {
        LooAlgkasutajad(db);
        if (!db.Toodetabel.Any()) NaidisTooted.LisaAndmed(db);
    }

    private static void LooAlgkasutajad(ToodedContext db)
    {
        if (db.Kasutajad.Any()) return;
        // Õppeülesande näidiskontod. Päris kasutuses tuleks paroolid muuta.
        db.Kasutajad.AddRange(
            new Kasutaja { Nimi = "Admin", Email = "admin@admin.ee", ParoolHash = PasswordHelper.Hash("admin123"), Roll = "admin" },
            new Kasutaja { Nimi = "Kasutaja", Email = "kasutaja@pood.ee", ParoolHash = PasswordHelper.Hash("kasutaja123"), Roll = "kasutaja" });
        db.SaveChanges();
    }
}
