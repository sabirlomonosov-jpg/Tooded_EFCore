using System.Drawing;

namespace Tooded_EFCore;

public partial class FormMain : Form
{
    private readonly Button btnTooted = new(), btnPood = new(), btnArve = new();

    public FormMain()
    {
        PoeStiil.Vorm(this, "Tooded • Peamenüü");
        ClientSize = new Size(550, 575);
        MinimumSize = new Size(550, 575);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var pais = new Panel { Dock = DockStyle.Top, Height = 156, BackColor = PoeStiil.Tume };
        pais.Controls.Add(PoeStiil.Silt("TOODED+ / ÕPPEPOOD", 36, 25, 450, 36, 21, true, Color.White));
        pais.Controls.Add(PoeStiil.Silt("Tere tulemast, " + AktiivneKasutaja.Nimi, 36, 77,
            450, 27, 12, false, Color.FromArgb(196, 211, 224)));
        pais.Controls.Add(PoeStiil.Silt("Roll: " + AktiivneKasutaja.Roll, 36, 109,
            450, 23, 10, false, PoeStiil.Kampaania));

        var sisu = new Panel { Dock = DockStyle.Fill, Padding = new Padding(36, 25, 36, 22) };
        var valikud = PoeStiil.Silt("VALI TEGEVUS", 0, 0, 450, 24, 10, true, PoeStiil.Vaikne);
        valikud.Dock = DockStyle.Top;
        var kaart = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(28)
        };

        SeaNupp(btnTooted, "Toodete haldus", "Lisa ja muuda tooteid", 18,
            (_, _) => Ava(new Form2()), true);
        SeaNupp(btnPood, "Pood", "Vaata kaupu ja lisa ostukorvi", 115,
            (_, _) => Ava(new Form3()), true);
        SeaNupp(btnArve, "Ostukorv ja arve", "Kinnita ost ja loo PDF", 212,
            (_, _) => Ava(new ArveForm()), false);
        kaart.Controls.AddRange(new Control[] { btnTooted, btnPood, btnArve });
        btnTooted.Enabled = AktiivneKasutaja.OnAdmin;
        if (!AktiivneKasutaja.OnAdmin)
            btnTooted.Text = "Toodete haldus  (ainult admin)";

        sisu.Controls.Add(kaart);
        sisu.Controls.Add(valikud);
        Controls.Add(sisu);
        Controls.Add(pais);
    }

    private static void SeaNupp(Button nupp, string tekst, string vihje, int y,
        EventHandler kasitleja, bool peamine)
    {
        nupp.Text = tekst + "     ›";
        nupp.SetBounds(14, y, 416, 75);
        nupp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        PoeStiil.Nupp(nupp, peamine);
        nupp.TextAlign = ContentAlignment.MiddleLeft;
        nupp.Padding = new Padding(18, 0, 0, 0);
        nupp.AccessibleDescription = vihje;
        nupp.Click += kasitleja;
    }

    private void Ava(Form vorm)
    {
        using (vorm) vorm.ShowDialog(this);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        AktiivneKasutaja.LogiValja();
        base.OnFormClosed(e);
    }
}
