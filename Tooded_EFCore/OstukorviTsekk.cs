using System.Drawing;

namespace Tooded_EFCore;

// The shop and checkout share the same cart; the receipt never owns a second copy.
public sealed class OstukorviTsekk : UserControl
{
    private readonly FlowLayoutPanel read = new()
    {
        Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.TopDown,
        WrapContents = false, Padding = new Padding(4)
    };
    private readonly Label kokku = new(), teade = new();
    private readonly Button maksa = new();
    public event EventHandler? Muudetud;
    public event EventHandler? Maksmine;

    public OstukorviTsekk()
    {
        BackColor = Color.White;
        Padding = new Padding(16);
        var pais = new Label
        {
            Text = "McDonald’s\nSINU TELLIMUS\n────────────────────────",
            Dock = DockStyle.Top, Height = 94, TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Consolas", 13, FontStyle.Bold), ForeColor = PoeStiil.Tekst
        };
        var jalus = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 168, ColumnCount = 1, RowCount = 3
        };
        jalus.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        jalus.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        jalus.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        kokku.Dock = DockStyle.Fill;
        kokku.Font = new Font("Consolas", 16, FontStyle.Bold);
        kokku.TextAlign = ContentAlignment.MiddleLeft;
        teade.Dock = DockStyle.Fill;
        teade.ForeColor = PoeStiil.Vaikne;
        teade.Font = new Font("Segoe UI", 9);
        maksa.Text = "Edasi maksma  ›";
        maksa.AccessibleName = "Ava maksmise aken";
        maksa.Dock = DockStyle.Fill;
        PoeStiil.Nupp(maksa);
        maksa.Click += (_, _) => { if (Ostukorv.SaabMaksta) Maksmine?.Invoke(this, EventArgs.Empty); };
        jalus.Controls.Add(kokku, 0, 0);
        jalus.Controls.Add(teade, 0, 1);
        jalus.Controls.Add(maksa, 0, 2);
        Controls.Add(read);
        Controls.Add(jalus);
        Controls.Add(pais);
        read.SizeChanged += (_, _) => SeaLaiused();
        Naita();
    }

    public void Naita()
    {
        int keritud = read.VerticalScroll.Value;
        read.SuspendLayout();
        foreach (Control rida in read.Controls.Cast<Control>().ToArray()) rida.Dispose();
        if (Ostukorv.Ridad.Count == 0)
        {
            read.Controls.Add(new Label
            {
                Text = "Sinu ostukorv on tühi.\n\nVali menüüst oma lemmikud!", Height = 150,
                TextAlign = ContentAlignment.MiddleCenter, ForeColor = PoeStiil.Vaikne
            });
        }
        foreach (var rida in Ostukorv.Ridad) read.Controls.Add(LooRida(rida));
        SeaLaiused();
        read.ResumeLayout(true);
        read.AutoScrollPosition = new Point(0, keritud);
        NaitaSummat();
    }

    private void SeaLaiused()
    {
        foreach (Control rida in read.Controls)
            rida.Width = Math.Max(290, read.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 12);
    }

    private Control LooRida(OstukorviRida rida)
    {
        var raam = new Panel { Width = 290, Height = 109, Margin = new Padding(0, 0, 0, 8) };
        var nimi = PoeStiil.Silt(rida.Toode.Toodenimetus, 0, 0, 290, 38, 10, true);
        nimi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        var hind = PoeStiil.Silt($"{rida.Toode.Hind:F2} €/tk", 0, 42, 85, 30, 9);
        var kogus = new NumericUpDown
        {
            Bounds = new Rectangle(88, 44, 65, 28), Minimum = 0,
            Maximum = Math.Max(rida.Kogus, rida.Toode.Kogus), Value = rida.Kogus,
            TextAlign = HorizontalAlignment.Center,
            AccessibleName = rida.Toode.Toodenimetus + " kogus"
        };
        var summa = PoeStiil.Silt($"{rida.Summa:F2} €", 158, 42, 81, 30, 10, true);
        summa.TextAlign = ContentAlignment.MiddleRight;
        var eemalda = new Button
        {
            Text = "×", Bounds = new Rectangle(247, 42, 35, 30),
            AccessibleName = "Eemalda " + rida.Toode.Toodenimetus
        };
        PoeStiil.Nupp(eemalda, false);
        var laos = PoeStiil.Silt($"Saadaval: {rida.Toode.Kogus} tk", 0, 78, 290, 22, 9,
            false, rida.Kogus > rida.Toode.Kogus ? PoeStiil.Oht : PoeStiil.Vaikne);
        bool taastan = false;
        kogus.ValueChanged += (_, _) =>
        {
            if (taastan) return;
            if (!Ostukorv.MuudaKogus(rida.Toode.Id, (int)kogus.Value))
            {
                taastan = true;
                kogus.Value = rida.Kogus;
                taastan = false;
                teade.Text = $"Saadaval on ainult {rida.Toode.Kogus} tk. Vähenda kogust või eemalda toode.";
                teade.ForeColor = PoeStiil.Oht;
                return;
            }
            if (kogus.Value == 0) Naita();
            else
            {
                summa.Text = $"{rida.Summa:F2} €";
                laos.ForeColor = rida.Kogus > rida.Toode.Kogus ? PoeStiil.Oht : PoeStiil.Vaikne;
                NaitaSummat();
            }
            Muudetud?.Invoke(this, EventArgs.Empty);
        };
        eemalda.Click += (_, _) =>
        {
            Ostukorv.Eemalda(rida.Toode.Id);
            Naita();
            Muudetud?.Invoke(this, EventArgs.Empty);
        };
        raam.Controls.AddRange(new Control[] { nimi, hind, kogus, summa, eemalda, laos });
        return raam;
    }

    private void NaitaSummat()
    {
        kokku.Text = $"KOKKU  {Ostukorv.Kokku:F2} €";
        maksa.Enabled = Ostukorv.SaabMaksta;
        teade.ForeColor = Ostukorv.Ridad.Count > 0 && !Ostukorv.SaabMaksta ? PoeStiil.Oht : PoeStiil.Vaikne;
        teade.Text = Ostukorv.Ridad.Count == 0 ? "Lisa tooteid, et jätkata."
            : !Ostukorv.SaabMaksta ? "Mõne toote laoseis on muutunud. Vähenda kogust või eemalda toode."
            : $"{Ostukorv.Ridad.Sum(r => (long)r.Kogus)} toodet · kogus 0 eemaldab toote.\nOst kinnitatakse järgmises aknas.";
    }
}
