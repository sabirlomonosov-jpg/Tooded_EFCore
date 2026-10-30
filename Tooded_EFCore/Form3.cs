using System.Drawing;
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

// McDonald’s-inspired learning menu. Prices are illustrative, not a live restaurant feed.
public partial class Form3 : Form
{
    private readonly ToodedContext _db = new();
    private readonly TabControl tabPood = new();
    private readonly TextBox txtOtsi = new();
    private readonly Label lblKorv = new(), lblTulemused = new();
    private readonly Button btnAvaKorv = new(), btnVarskenda = new();
    private readonly List<Image> laaditudPildid = new();
    private List<Toode> tooted = new();

    public Form3()
    {
        PoeStiil.Vorm(this, "Tooted • Õppepood");
        ClientSize = new Size(1240, 820);
        MinimumSize = new Size(900, 600);

        var pais = new Panel { Dock = DockStyle.Top, Height = 121, BackColor = PoeStiil.Punane };
        pais.Controls.Add(PoeStiil.Logo(24, 15));
        pais.Controls.Add(PoeStiil.Silt("McDonald’s", 99, 13, 280, 51, 28, true, Color.White));
        pais.Controls.Add(PoeStiil.Silt("I’m lovin’ it.", 100, 65, 260, 22, 10, true, PoeStiil.Kampaania));
        var tunnuslause = PoeStiil.Silt("Sinu lemmikud. Värskelt valmistatud.", 400, 31,
            490, 32, 10, false, Color.White);
        pais.Controls.Add(tunnuslause);
        lblKorv.SetBounds(400, 70, 570, 34);
        lblKorv.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblKorv.ForeColor = Color.White;
        lblKorv.TextAlign = ContentAlignment.MiddleLeft;

        btnAvaKorv.Text = "OSTUKORV  ›";
        btnAvaKorv.SetBounds(1015, 23, 195, 50);
        btnAvaKorv.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        PoeStiil.Nupp(btnAvaKorv);
        btnAvaKorv.BackColor = PoeStiil.Kampaania;
        btnAvaKorv.ForeColor = PoeStiil.Tume;
        btnAvaKorv.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 235, 102);
        btnAvaKorv.Click += (_, _) =>
        {
            using var arve = new ArveForm();
            arve.ShowDialog(this);
            LaePood(); // Laos võib olla pärast ostu vähem kaupa.
            NaitaKorviSeis();
        };
        pais.Controls.AddRange(new Control[] { lblKorv, btnAvaKorv });

        var tegevused = new Panel { Dock = DockStyle.Top, Height = 77,
            BackColor = Color.White, Padding = new Padding(24, 12, 24, 10) };
        var silt = PoeStiil.Silt("MEIE MENÜÜ", 5, 0, 300, 32, 13, true, PoeStiil.Tume);
        var margis = PoeStiil.Silt("Eesti · näidishinnad € · mitteametlik õppemenüü", 6, 34,
            610, 24, 9, false, PoeStiil.Vaikne);
        txtOtsi.SetBounds(712, 12, 315, 41);
        txtOtsi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        txtOtsi.PlaceholderText = "Otsi toodet või kategooriat...";
        PoeStiil.Sisend(txtOtsi);
        txtOtsi.TextChanged += (_, _) => NaitaKategooriad();
        btnVarskenda.SetBounds(1042, 12, 151, 41);
        btnVarskenda.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnVarskenda.Text = "Värskenda";
        PoeStiil.Nupp(btnVarskenda, false);
        btnVarskenda.Click += (_, _) => LaePood();
        tegevused.Controls.AddRange(new Control[] { silt, margis, txtOtsi, btnVarskenda });

        var kampaaniariba = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = PoeStiil.Kampaania };
        lblTulemused = PoeStiil.Silt("VALI OMA LEMMIK", 24, 0, 1120, 44,
            11, true, PoeStiil.Tume);
        kampaaniariba.Controls.Add(lblTulemused);

        var sisu = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16, 14, 16, 16) };
        tabPood.Dock = DockStyle.Fill;
        tabPood.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        tabPood.Padding = new Point(16, 9);
        sisu.Controls.Add(tabPood);
        Controls.Add(sisu);
        Controls.Add(kampaaniariba);
        Controls.Add(tegevused);
        Controls.Add(pais);

        void KohandaPaigutus()
        {
            tunnuslause.Visible = ClientSize.Width >= 1200;
            lblKorv.Width = Math.Max(150, ClientSize.Width - lblKorv.Left - 24);
            margis.Width = Math.Max(180, txtOtsi.Left - 20);
            lblTulemused.Width = Math.Max(200, ClientSize.Width - 48);
        }
        SizeChanged += (_, _) => KohandaPaigutus();
        KohandaPaigutus();
        LaePood();
        NaitaKorviSeis();
    }

    private void NaitaKorviSeis()
    {
        lblKorv.Text = $"Korvis: {Ostukorv.Ridad.Sum(r => r.Kogus)} tk    Kokku: {Ostukorv.Kokku:F2} €";
    }

    private void LaePood()
    {
        // AsNoTracking teeb laoseisu värskendamise lihtsamaks.
        tooted = _db.Toodetabel.AsNoTracking()
            .Include(t => t.Kategooria)
            .OrderBy(t => t.Toodenimetus)
            .ToList();
        NaitaKategooriad();
    }

    private void NaitaKategooriad()
    {
        string otsing = txtOtsi.Text.Trim();
        var sobivad = tooted.Where(t =>
            t.Toodenimetus.Contains(otsing, StringComparison.CurrentCultureIgnoreCase) ||
            t.Kategooria.Kategooria_nimetus.Contains(otsing, StringComparison.CurrentCultureIgnoreCase))
            .ToList();

        // Kustutame vanad kaardid ja nende pildiviited enne uut otsingut.
        var vanadLehed = tabPood.TabPages.Cast<TabPage>().ToList();
        tabPood.TabPages.Clear();
        foreach (var leht in vanadLehed) leht.Dispose();
        foreach (var pilt in laaditudPildid) pilt.Dispose();
        laaditudPildid.Clear();

        LisaLeht("Kogu menüü", sobivad);
        foreach (var grupp in sobivad.GroupBy(t => t.Kategooria.Kategooria_nimetus)
                     .OrderBy(x => x.Key))
            LisaLeht(grupp.Key, grupp.ToList());

        lblTulemused.Text = $"{sobivad.Count} VALIKUT  •  BURGERID, FRIKAD JA HEAD HETKED";
    }

    private void LisaLeht(string nimi, IReadOnlyList<Toode> read)
    {
        var leht = new TabPage($"{nimi} ({read.Count})")
        {
            BackColor = PoeStiil.Taust,
            Padding = new Padding(5)
        };
        var voog = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = PoeStiil.Taust,
            Padding = new Padding(5)
        };
        foreach (var toode in read)
            voog.Controls.Add(LooTootekaart(toode));
        leht.Controls.Add(voog);
        tabPood.TabPages.Add(leht);
    }

    private Panel LooTootekaart(Toode t)
    {
        var kaart = new Panel
        {
            Size = new Size(222, 337),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(9)
        };
        Image? sisu = Kaustad.LaePilt(t.Pilt);
        if (sisu != null) laaditudPildid.Add(sisu);
        var pilt = new PictureBox
        {
            Bounds = new Rectangle(10, 10, 200, 160),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.White,
            Image = sisu,
            Cursor = Cursors.Hand
        };
        if (sisu == null)
            pilt.BackColor = PoeStiil.HeleSinine;

        var nimi = PoeStiil.Silt(t.Toodenimetus, 12, 173, 199, 45, 10, true);
        nimi.TextAlign = ContentAlignment.TopLeft;
        nimi.AutoEllipsis = true;
        var hind = PoeStiil.Silt($"{t.Hind:F2} €", 12, 222, 116, 35, 16, true, PoeStiil.Tekst);
        var laos = PoeStiil.Silt($"Saadaval: {t.Kogus} tk", 12, 263, 191, 20,
            9, false, PoeStiil.Vaikne);
        var kogus = new NumericUpDown
        {
            Bounds = new Rectangle(12, 293, 61, 31),
            Minimum = 1,
            Maximum = Math.Max(1, t.Kogus),
            Value = 1,
            Font = new Font("Segoe UI", 10f),
            TextAlign = HorizontalAlignment.Center
        };
        var nupp = new Button
        {
            Bounds = new Rectangle(84, 290, 126, 36),
            Text = t.Kogus > 0 ? "Lisa korvi" : "Otsas",
            Enabled = t.Kogus > 0
        };
        PoeStiil.Nupp(nupp);
        nupp.Click += (_, _) =>
        {
            if (!Ostukorv.Lisa(t, (int)kogus.Value))
            {
                MessageBox.Show("Korvi ei saa lisada rohkem kaupa, kui on laos.");
                return;
            }
            NaitaKorviSeis();
        };
        pilt.Click += (_, _) => MessageBox.Show(
            $"{t.Toodenimetus}\nHind: {t.Hind:F2} €\nLaos: {t.Kogus} tk");
        kaart.Controls.AddRange(new Control[] { pilt, nimi, hind, laos, kogus, nupp });
        return kaart;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        foreach (var pilt in laaditudPildid) pilt.Dispose();
        laaditudPildid.Clear();
        _db.Dispose();
        base.OnFormClosed(e);
    }
}
