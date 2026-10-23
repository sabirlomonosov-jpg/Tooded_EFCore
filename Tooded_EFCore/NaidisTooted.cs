using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

// Õppematerjal. Tootenimed ja hinnanäited: maxima.ee/pakkumised (23.10.2026).
// Hinnad ei uuene automaatselt ega pruugi enam kehtida.
// Pildid on rakenduse enda lihtsad illustratsioonid, mitte kaupluse tootefotod.
public static class NaidisTooted
{
    public sealed record Naidis(string Nimi, string Kategooria, decimal Hind,
        decimal? VarasemHind, string Pildifail);

    public static readonly DateTime AndmeteKuupaev = new(2026, 10, 23);

    public static readonly IReadOnlyList<Naidis> Andmed = new List<Naidis>
    {
        new("Tume viinamari, 1 kg", "Puu- ja köögiviljad", 1.19m, 2.49m, "grapes.png"),
        new("Mandariin, 1 kg", "Puu- ja köögiviljad", 1.99m, 2.99m, "mandarin.png"),
        new("Kirss-ploomtomat, 250 g", "Puu- ja köögiviljad", 1.49m, 2.19m, "tomato.png"),
        new("Pikk kurk, 1 kg", "Puu- ja köögiviljad", 2.59m, 3.29m, "cucumber.png"),
        new("Kartul lahtine, 1 kg", "Puu- ja köögiviljad", 0.25m, 0.49m, "potato.png"),
        new("Hurmaa, 1 kg", "Puu- ja köögiviljad", 3.29m, 4.49m, "persimmon.png"),
        new("Või Tere, 82%, 200 g", "Piimatooted ja munad", 1.39m, 2.59m, "butter.png"),
        new("Grillbroiler Tallegg, 1 kg", "Lihatooted", 3.99m, 6.09m, "chicken.png"),
        new("Seaabaliha kamarata, kondita, 1 kg", "Lihatooted", 2.49m, 5.99m, "meat.png"),
        new("Kodune ahjupraad Rakvere, 1 kg", "Lihatooted", 5.99m, 7.89m, "roast.png"),
        new("Jahutatud Norra lõhefilee, 1 kg", "Kalatooted", 12.99m, null, "salmon.png"),
        new("Kreeka jogurti kook Halloween, Eesti Pagar, 350 g", "Kondiitritooted", 3.99m, 6.19m, "cake.png"),
        new("Jäätis Snickers, 48 g", "Külmutatud tooted", 0.79m, 1.29m, "icecream.png"),
        new("Hommikusöögibatoon Nestle, 6 x 25 g", "Kauasäilivad toidukaubad", 1.99m, 2.85m, "cereal.png"),
        new("Šokolaad Kalev, 270 g", "Maiustused", 6.99m, null, "chocolate.png"),
        new("Karastusjook Coca-Cola, 2 x 1,5 l", "Joogid", 3.59m, null, "cola.png"),
        new("Jahvatatud kohv Löfbergs, 500 g", "Kohv, tee", 10.69m, null, "coffee.png")
    };

    // Näitab varasemat hinda ainult siis, kui kasutaja pole näidishinda muutnud.
    // See on 23.10.2026 ajalooline võrdlus, mitte reaalajas sooduspakkumine.
    public static (decimal VarasemHind, int Protsent)? VordleHinda(Toode toode)
    {
        var naidis = Andmed.FirstOrDefault(x =>
            string.Equals(x.Nimi, toode.Toodenimetus, StringComparison.OrdinalIgnoreCase)
            && x.Hind == toode.Hind && x.VarasemHind.HasValue);
        if (naidis == null || naidis.VarasemHind is not decimal vana || vana <= naidis.Hind)
            return null;
        int protsent = (int)Math.Round((vana - naidis.Hind) * 100 / vana,
            MidpointRounding.AwayFromZero);
        return (vana, protsent);
    }

    public static int LisaAndmed(ToodedContext db)
    {
        Kaustad.Loo();
        var kategooriad = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var kategooria in db.Kategooriatabel.AsNoTracking().ToList())
        {
            if (!kategooriad.ContainsKey(kategooria.Kategooria_nimetus))
                kategooriad.Add(kategooria.Kategooria_nimetus, kategooria.Id);
        }
        var nimed = db.Toodetabel.AsNoTracking().Select(x => x.Toodenimetus)
            .ToList().ToHashSet(StringComparer.OrdinalIgnoreCase);
        var pildidLoodud = new List<string>();
        int lisatud = 0;
        try
        {
            using var tehing = db.Database.BeginTransaction();
            foreach (var naidis in Andmed)
            {
                if (nimed.Contains(naidis.Nimi)) continue;
                if (!kategooriad.TryGetValue(naidis.Kategooria, out int kategooriaId))
                {
                    var uus = new Kategooria { Kategooria_nimetus = naidis.Kategooria };
                    db.Kategooriatabel.Add(uus);
                    db.SaveChanges();
                    kategooriaId = uus.Id;
                    kategooriad.Add(naidis.Kategooria, kategooriaId);
                }

                string allikas = Path.Combine(AppContext.BaseDirectory, "DemoImages", naidis.Pildifail);
                string siht = Path.Combine(Kaustad.Pildid, "naidis_" + naidis.Pildifail);
                string? pildiNimi = null;
                if (File.Exists(allikas))
                {
                    if (!File.Exists(siht))
                    {
                        File.Copy(allikas, siht);
                        pildidLoodud.Add(siht);
                    }
                    pildiNimi = Path.GetFileName(siht);
                }

                db.Toodetabel.Add(new Toode
                {
                    Toodenimetus = naidis.Nimi,
                    Hind = naidis.Hind,
                    Kogus = 25, // Näidislaoseis, mitte päris MAXIMA laoseis.
                    KategooriaId = kategooriaId,
                    Pilt = pildiNimi
                });
                nimed.Add(naidis.Nimi);
                lisatud++;
            }
            db.SaveChanges();
            tehing.Commit();
            return lisatud;
        }
        catch
        {
            db.ChangeTracker.Clear();
            foreach (string tee in pildidLoodud)
            {
                try { File.Delete(tee); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            throw;
        }
    }
}
