using Microsoft.EntityFrameworkCore;

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
            AndmebaasiAlgataja.Kaivita(db =>
            {
                LooAlgkasutajad(db);
                // Täiesti tühja andmebaasi täidame õppeprojektis näidistoodetega.
                // Kui tooteid on juba olemas, ei muudeta olemasolevaid ridu.
                if (!db.Toodetabel.Any()) NaidisTooted.LisaAndmed(db);
            });
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
