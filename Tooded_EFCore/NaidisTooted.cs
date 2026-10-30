using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;

namespace Tooded_EFCore;

public static class NaidisTooted
{
    public static IReadOnlyList<MenuuAndmed.Toit> Andmed => MenuuAndmed.Andmed;

    // The bundled atlas works offline and is also copied to published builds.
    public static void ValmistaPildid()
    {
        Kaustad.Loo();
        var puuduvad = Andmed.Where(x => !File.Exists(Path.Combine(Kaustad.Pildid, x.Pildifail))).ToList();
        if (puuduvad.Count == 0) return;
        string allikas = Path.Combine(AppContext.BaseDirectory, "Assets", "menu-atlas.png");
        using var atlas = new Bitmap(allikas);
        foreach (var toit in puuduvad)
        {
            int x = toit.PildiIndeks % 4, y = toit.PildiIndeks / 4;
            var ala = new Rectangle(x * atlas.Width / 4, y * atlas.Height / 4,
                (x + 1) * atlas.Width / 4 - x * atlas.Width / 4,
                (y + 1) * atlas.Height / 4 - y * atlas.Height / 4);
            using var pilt = atlas.Clone(ala, PixelFormat.Format32bppArgb);
            pilt.Save(Path.Combine(Kaustad.Pildid, toit.Pildifail), ImageFormat.Png);
        }
    }

    // Add missing dishes only; never reset administrator prices or purchased stock.
    public static int LisaAndmed(ToodedContext db)
    {
        ValmistaPildid();
        using var tehing = db.Database.BeginTransaction();
        var kategooriad = db.Kategooriatabel.ToList();
        var nimed = db.Toodetabel.Select(x => x.Toodenimetus).ToList()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        int lisatud = 0;
        foreach (var toit in Andmed)
        {
            if (nimed.Contains(toit.Nimi)) continue;
            var kategooria = kategooriad.FirstOrDefault(x =>
                string.Equals(x.Kategooria_nimetus, toit.Kategooria, StringComparison.OrdinalIgnoreCase));
            if (kategooria == null)
            {
                kategooria = new Kategooria { Kategooria_nimetus = toit.Kategooria };
                db.Kategooriatabel.Add(kategooria);
                kategooriad.Add(kategooria);
            }
            db.Toodetabel.Add(new Toode { Toodenimetus = toit.Nimi, Hind = toit.Hind,
                Kogus = 25, Kategooria = kategooria, Pilt = toit.Pildifail });
            nimed.Add(toit.Nimi);
            lisatud++;
        }
        db.SaveChanges();
        tehing.Commit();
        return lisatud;
    }
}
