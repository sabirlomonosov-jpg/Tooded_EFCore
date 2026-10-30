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
            using (var db = new ToodedContext())
            {
                db.Database.Migrate();
                LooAlgkasutajad(db);
                // Täiesti tühja andmebaasi täidame õppeprojektis näidistoodetega.
                // Kui tooteid on juba olemas, ei muudeta olemasolevaid ridu.
                if (!db.Toodetabel.Any()) NaidisTooted.LisaAndmed(db);
            }
            Application.Run(new LoginForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Programmi käivitamine ebaõnnestus. Kontrolli SQL Server LocalDB ühendust.\n\n" + ex.Message,
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
