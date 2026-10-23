using System.Drawing;
using System.IO;

namespace Tooded_EFCore;

public static class Kaustad
{
    public static readonly string Juur = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ToodedApp");

    public static readonly string Pildid = Path.Combine(Juur, "Images");
    public static readonly string Arved = Path.Combine(Juur, "Arved");

    public static void Loo()
    {
        Directory.CreateDirectory(Pildid);
        Directory.CreateDirectory(Arved);
    }

    // Laadime koopia, et originaalfail ei jääks lukku.
    public static Image? LaePilt(string? failinimi)
    {
        if (string.IsNullOrWhiteSpace(failinimi)) return null;
        string tee = Path.Combine(Pildid, Path.GetFileName(failinimi));
        if (!File.Exists(tee)) return null;
        try
        {
            using var ajutine = Image.FromFile(tee);
            return new Bitmap(ajutine);
        }
        catch (ArgumentException) { return null; }
        catch (IOException) { return null; }
        catch (UnauthorizedAccessException) { return null; }
    }

    // Tagastab salvestatud failinime; originaali nimega konflikti pole.
    public static string KopeeriPilt(string allikas)
    {
        Loo();
        string laiend = Path.GetExtension(allikas).ToLowerInvariant();
        if (laiend != ".png" && laiend != ".jpg" && laiend != ".jpeg")
            throw new InvalidOperationException("Lubatud on ainult JPG ja PNG pildid.");
        string failinimi = Guid.NewGuid().ToString("N") + laiend;
        File.Copy(allikas, Path.Combine(Pildid, failinimi));
        return failinimi;
    }
}
