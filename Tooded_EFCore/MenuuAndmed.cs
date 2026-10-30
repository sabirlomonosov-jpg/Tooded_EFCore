namespace Tooded_EFCore;

// Estonian learning menu. Prices are examples in EUR, not verified restaurant prices.
public static class MenuuAndmed
{
    public sealed record Toit(string Nimi, string Kategooria, decimal Hind, string Pildifail, int PildiIndeks);
    public static readonly IReadOnlyList<Toit> Andmed = new List<Toit>
    {
        new("Big Mac", "Burgerid", 5.50m, "mcd-big-mac.png", 0),
        new("Juustuburger", "Burgerid", 2.20m, "mcd-cheeseburger.png", 1),
        new("Double Cheeseburger", "Burgerid", 3.80m, "mcd-double-cheeseburger.png", 2),
        new("McChicken", "Burgerid", 5.20m, "mcd-mcchicken.png", 3),
        new("Filet-O-Fish", "Burgerid", 4.90m, "mcd-filet-o-fish.png", 4),
        new("Chicken McNuggets 9 tk", "Kana", 5.90m, "mcd-nuggets.png", 5),
        new("Suured friikartulid", "Lisandid", 3.20m, "mcd-fries-large.png", 6),
        new("Väikesed friikartulid", "Lisandid", 2.20m, "mcd-fries-small.png", 7),
        new("Coca-Cola 0,4 l", "Joogid", 2.60m, "mcd-cola.png", 8),
        new("Fanta 0,4 l", "Joogid", 2.60m, "mcd-fanta.png", 9),
        new("McCafé must kohv", "McCafé", 2.00m, "mcd-coffee.png", 10),
        new("Vesi 0,5 l", "Joogid", 2.00m, "mcd-water.png", 11),
        new("McFlurry Oreo", "Magustoidud", 3.50m, "mcd-mcflurry.png", 12),
        new("Šokolaadi Sundae", "Magustoidud", 2.50m, "mcd-sundae.png", 13),
        new("Õunapirukas", "Magustoidud", 1.80m, "mcd-apple-pie.png", 14),
        new("Happy Meal hamburgeriga", "Happy Meal", 5.50m, "mcd-happy-meal.png", 15),
        new("Keskmised friikartulid", "Lisandid", 2.80m, "mcd-fries-medium.png", 6),
    }.AsReadOnly();
}
