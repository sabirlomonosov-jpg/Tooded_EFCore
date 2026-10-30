using System.Drawing;

namespace Tooded_EFCore;

public sealed class AndmebaasiVeaVorm : Form
{
    public AndmebaasiVeaVorm(Exception viga, string andmebaas, bool saabLuua)
    {
        PoeStiil.Vorm(this, "Andmebaasi ühendus");
        ClientSize = new Size(760, 550);
        MinimumSize = new Size(700, 500);
        Padding = new Padding(18);
        var selgitus = new Label
        {
            Dock = DockStyle.Top, Height = 190,
            Text = saabLuua
                ? $"Andmebaasi „{andmebaas}” ei saa avada.\n\n" +
                  "Võid luua selle Windowsi kasutaja jaoks eraldi uue poe andmebaasi. " +
                  "Vana andmebaasi ei muudeta ja selle andmeid uude poodi ei kopeerita. " +
                  "Uus pood alustab näidismenüü ja näidiskontodega.\n\n" +
                  "Kui vajad seniseid andmeid, sulge aken ja palu administraatoril ligipääs taastada."
                : "Andmebaasi ettevalmistamine ebaõnnestus. Viga on allpool.\n\n" +
                  "Olemasolevaid andmeid ei kustutatud. Kontrolli ühendust ja andmebaasi ligipääsu. " +
                  "Kui TOODED_CONNECTION_STRING on määratud, muuda ühendust selle kaudu.",
            ForeColor = PoeStiil.Tekst
        };
        var detailid = new TextBox
        {
            Dock = DockStyle.Fill, Multiline = true, ReadOnly = true,
            ScrollBars = ScrollBars.Vertical, Text = KaivituseKordus.Kirjelda(viga),
            BackColor = Color.White, Font = new Font("Consolas", 10)
        };
        var nupud = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 66,
            FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 16, 0, 0)
        };
        var sulge = new Button { Text = "Sulge", Size = new Size(100, 40), DialogResult = DialogResult.Cancel };
        PoeStiil.Nupp(sulge, false);
        nupud.Controls.Add(sulge);
        if (saabLuua)
        {
            var uus = new Button
            {
                Text = "Loo eraldi uus andmebaas", Size = new Size(270, 40),
                DialogResult = DialogResult.OK
            };
            PoeStiil.Nupp(uus);
            nupud.Controls.Add(uus);
        }
        Controls.Add(detailid);
        Controls.Add(nupud);
        Controls.Add(selgitus);
        CancelButton = sulge;
        // Enter must not silently choose a fresh, empty shop.
        ActiveControl = sulge;
    }
}
