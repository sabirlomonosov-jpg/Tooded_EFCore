namespace Tooded_EFCore;

public class OstukorviRida
{
    public Toode Toode { get; set; } = null!;
    public int Kogus { get; set; }
    public decimal Summa => Toode.Hind * Kogus;

    public override string ToString() =>
        $"{Toode.Toodenimetus} – {Kogus} tk × {Toode.Hind:F2} € = {Summa:F2} €";
}

public static class Ostukorv
{
    public static List<OstukorviRida> Ridad { get; } = new();
    public static decimal Kokku => Ridad.Sum(r => r.Summa);

    // Arvestame nii sama kauba olemasoleva korvirea kui ka laoseisuga.
    public static bool Lisa(Toode toode, int kogus)
    {
        if (kogus <= 0 || toode.Kogus <= 0) return false;
        var olemas = Ridad.FirstOrDefault(r => r.Toode.Id == toode.Id);
        int korvis = olemas?.Kogus ?? 0;
        if ((long)korvis + kogus > toode.Kogus) return false;
        if (olemas != null)
        {
            olemas.Toode = toode;
            olemas.Kogus += kogus;
        }
        else Ridad.Add(new OstukorviRida { Toode = toode, Kogus = kogus });
        return true;
    }

    public static void Tyhjenda() => Ridad.Clear();

    public static bool MuudaKogus(int tooteId, int kogus)
    {
        var rida = Ridad.FirstOrDefault(r => r.Toode.Id == tooteId);
        if (rida == null || kogus < 0) return false;
        if (kogus == 0) return Eemalda(tooteId);
        if (kogus > rida.Toode.Kogus) return false;
        rida.Kogus = kogus;
        return true;
    }

    public static bool Eemalda(int tooteId) =>
        Ridad.RemoveAll(r => r.Toode.Id == tooteId) > 0;

    // Refresh prices and stock after returning from checkout or reloading the shop.
    // Keep unavailable rows visible so the customer can remove or reduce them.
    public static void VarskendaTooted(IEnumerable<Toode> tooted)
    {
        var hetkel = tooted.ToDictionary(t => t.Id);
        foreach (var rida in Ridad)
        {
            if (hetkel.TryGetValue(rida.Toode.Id, out var toode)) rida.Toode = toode;
            else rida.Toode.Kogus = 0;
        }
    }

    public static bool SaabMaksta => Ridad.Count > 0 &&
        Ridad.All(r => r.Kogus > 0 && r.Kogus <= r.Toode.Kogus);
}
