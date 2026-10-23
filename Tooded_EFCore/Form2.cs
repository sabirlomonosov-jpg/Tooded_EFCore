using System.Drawing;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

public partial class Form2 : Form
{
    private readonly ToodedContext _db = new();
    private string? valitudPilt; // Täistee uuele pildile; null tähendab, et pilti ei muudeta.

    private readonly DataGridView dataGridViewTooted = new();
    private readonly TextBox txtNimetus = new(), txtKogus = new(), txtHind = new();
    private readonly ComboBox comboKategooria = new();
    private readonly PictureBox picturePilt = new();
    private readonly Button btnValiPilt = new(), btnLisa = new(), btnUuenda = new(),
        btnKustuta = new(), btnTyhjenda = new(), btnNaidised = new();

    public Form2()
    {
        LooLiides();
        LaeKategooriad();
        LaeTooted();

        // Ligipääsupiirang ka siis, kui vorm avatakse mujalt kui peamenüüst.
        if (!AktiivneKasutaja.OnAdmin)
        {
            btnNaidised.Enabled = false;
            btnLisa.Enabled = btnUuenda.Enabled = btnKustuta.Enabled = btnValiPilt.Enabled = false;
            Shown += (_, _) =>
            {
                MessageBox.Show("Tootehaldus on ainult administraatorile.");
                Close();
            };
        }
    }

    private void LooLiides()
    {
        PoeStiil.Vorm(this, "Tooted • Toodete haldus");
        ClientSize = new Size(1170, 750);
        MinimumSize = new Size(920, 660);

        var pais = new Panel { Dock = DockStyle.Top, Height = 91, BackColor = PoeStiil.Tume };
        pais.Controls.Add(PoeStiil.Silt("TOODETE HALDUS", 25, 16, 650, 40, 20, true, Color.White));
        pais.Controls.Add(PoeStiil.Silt("Halda laoseisu, hindu ja tootepilte", 27, 54,
            630, 24, 10, false, Color.FromArgb(221, 235, 250)));
        btnNaidised.Text = "Lisa näidistooted";
        btnNaidised.SetBounds(906, 22, 237, 43);
        btnNaidised.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        PoeStiil.Nupp(btnNaidised, false);
        btnNaidised.BackColor = PoeStiil.Kampaania;
        btnNaidised.ForeColor = PoeStiil.Tume;
        btnNaidised.Click += btnNaidised_Click;
        pais.Controls.Add(btnNaidised);

        var raam = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(19),
            ColumnCount = 2,
            RowCount = 1,
            BackColor = PoeStiil.Taust
        };
        raam.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        raam.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 345));
        raam.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var vasak = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(15) };
        var loendPealkiri = PoeStiil.Silt("TOOTED", 0, 0, 650, 42, 12, true);
        loendPealkiri.Dock = DockStyle.Top;
        dataGridViewTooted.Dock = DockStyle.Fill;
        dataGridViewTooted.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewTooted.MultiSelect = false;
        dataGridViewTooted.ReadOnly = true;
        dataGridViewTooted.AllowUserToAddRows = false;
        dataGridViewTooted.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewTooted.CellClick += dataGridViewTooted_CellClick;
        PoeStiil.Tabel(dataGridViewTooted);
        vasak.Controls.Add(dataGridViewTooted);
        vasak.Controls.Add(loendPealkiri);

        var parem = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(12, 0, 0, 0),
            AutoScroll = true
        };
        parem.Controls.Add(PoeStiil.Silt("TOOTE ANDMED", 19, 12, 290, 30, 12, true));
        LisaSiltJaVali(parem, "Nimetus", txtNimetus, 56, "Toote nimetus");
        LisaSiltJaVali(parem, "Kogus", txtKogus, 128, "0 või rohkem");
        LisaSiltJaVali(parem, "Hind (€)", txtHind, 200, "Näiteks 2,50");

        parem.Controls.Add(PoeStiil.Silt("Kategooria", 19, 272, 295, 22, 10, true));
        comboKategooria.SetBounds(19, 296, 299, 32);
        comboKategooria.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        comboKategooria.DropDownStyle = ComboBoxStyle.DropDownList;
        comboKategooria.FlatStyle = FlatStyle.Flat;
        comboKategooria.Font = new Font("Segoe UI", 10.5f);
        parem.Controls.Add(comboKategooria);

        parem.Controls.Add(PoeStiil.Silt("Tootepilt", 19, 340, 295, 22, 10, true));
        picturePilt.SetBounds(19, 369, 299, 106);
        picturePilt.BackColor = PoeStiil.Taust;
        picturePilt.SizeMode = PictureBoxSizeMode.Zoom;
        picturePilt.BorderStyle = BorderStyle.FixedSingle;
        picturePilt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        parem.Controls.Add(picturePilt);

        SeaNupp(btnValiPilt, "Vali pilt", 485, 299, 34, btnValiPilt_Click, false);
        SeaNupp(btnLisa, "Lisa toode", 531, 145, 35, btnLisa_Click, true);
        SeaNupp(btnUuenda, "Uuenda", 531, 145, 35, btnUuenda_Click, false);
        SeaNupp(btnKustuta, "Kustuta", 577, 145, 35, btnKustuta_Click, false, true);
        SeaNupp(btnTyhjenda, "Tühjenda", 577, 145, 35, (_, _) => TyhjendaVorm(), false);
        btnUuenda.Left = 173;
        btnTyhjenda.Left = 173;
        parem.Controls.AddRange(new Control[]
        {
            btnValiPilt, btnLisa, btnUuenda, btnKustuta, btnTyhjenda
        });
        raam.Controls.Add(vasak, 0, 0);
        raam.Controls.Add(parem, 1, 0);
        Controls.Add(raam);
        Controls.Add(pais);
    }

    private static void LisaSiltJaVali(Panel vanem, string nimi, TextBox sisend,
        int y, string vihje)
    {
        vanem.Controls.Add(PoeStiil.Silt(nimi, 19, y, 295, 22, 10, true));
        sisend.SetBounds(19, y + 24, 299, 30);
        sisend.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sisend.PlaceholderText = vihje;
        PoeStiil.Sisend(sisend);
        vanem.Controls.Add(sisend);
    }

    private static void SeaNupp(Button nupp, string tekst, int y, int laius,
        int korgus, EventHandler kasitleja, bool peamine, bool oht = false)
    {
        nupp.Text = tekst;
        nupp.SetBounds(19, y, laius, korgus);
        PoeStiil.Nupp(nupp, peamine, oht);
        nupp.Click += kasitleja;
    }

    private void LaeKategooriad()
    {
        comboKategooria.DataSource = _db.Kategooriatabel.AsNoTracking()
            .OrderBy(k => k.Kategooria_nimetus).ToList();
        comboKategooria.DisplayMember = "Kategooria_nimetus";
        comboKategooria.ValueMember = "Id";
    }

    private void LaeTooted()
    {
        _db.ChangeTracker.Clear();
        dataGridViewTooted.DataSource = _db.Toodetabel.AsNoTracking()
            .OrderBy(t => t.Id)
            .Select(t => new
            {
                t.Id,
                t.Toodenimetus,
                t.Kogus,
                t.Hind,
                t.KategooriaId,
                Kategooria = t.Kategooria.Kategooria_nimetus,
                t.Pilt
            }).ToList();
        if (dataGridViewTooted.Columns["KategooriaId"] is DataGridViewColumn veerg)
            veerg.Visible = false;
        dataGridViewTooted.ClearSelection();
    }

    private void NaitaPilt(string? nimiVoiTee, bool uusPilt)
    {
        var vana = picturePilt.Image;
        picturePilt.Image = null;
        vana?.Dispose();
        if (string.IsNullOrWhiteSpace(nimiVoiTee)) return;
        try
        {
            if (uusPilt)
            {
                using var pilt = Image.FromFile(nimiVoiTee);
                picturePilt.Image = new Bitmap(pilt);
            }
            else
            {
                picturePilt.Image = Kaustad.LaePilt(nimiVoiTee);
            }
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or UnauthorizedAccessException)
        {
            MessageBox.Show("Pildi avamine ebaõnnestus: " + ex.Message);
        }
    }

    private void btnValiPilt_Click(object? sender, EventArgs e)
    {
        if (!AktiivneKasutaja.OnAdmin) return;
        using var dlg = new OpenFileDialog { Filter = "Pildid|*.jpg;*.jpeg;*.png" };
        if (dlg.ShowDialog() != DialogResult.OK) return;
        valitudPilt = dlg.FileName;
        NaitaPilt(valitudPilt, true);
    }

    private bool LoeVorm(Toode toode)
    {
        if (string.IsNullOrWhiteSpace(txtNimetus.Text))
        {
            MessageBox.Show("Sisesta toote nimetus.");
            return false;
        }
        if (!int.TryParse(txtKogus.Text, out int kogus) || kogus < 0)
        {
            MessageBox.Show("Kogus peab olema täisarv (0 või rohkem).");
            return false;
        }
        if (!decimal.TryParse(txtHind.Text.Trim().Replace(',', '.'),
                NumberStyles.Number, CultureInfo.InvariantCulture, out decimal hind)
            || hind < 0 || hind > 99999999.99m)
        {
            MessageBox.Show("Hind peab olema arv (0 või rohkem), nt 2,50.");
            return false;
        }
        if (comboKategooria.SelectedValue is not int kategooriaId)
        {
            MessageBox.Show("Vali kategooria.");
            return false;
        }
        toode.Toodenimetus = txtNimetus.Text.Trim();
        toode.Kogus = kogus;
        toode.Hind = hind;
        toode.KategooriaId = kategooriaId;
        return true;
    }

    private int? ValitudId()
    {
        if (dataGridViewTooted.SelectedRows.Count == 0 ||
            dataGridViewTooted.SelectedRows[0].Cells["Id"].Value is not int id)
        {
            MessageBox.Show("Vali tabelist toode.");
            return null;
        }
        return id;
    }

    private void TyhjendaVorm()
    {
        txtNimetus.Clear();
        txtKogus.Clear();
        txtHind.Clear();
        comboKategooria.SelectedIndex = -1;
        valitudPilt = null;
        NaitaPilt(null, false);
        dataGridViewTooted.ClearSelection();
    }

    private void btnLisa_Click(object? sender, EventArgs e)
    {
        if (!AktiivneKasutaja.OnAdmin) return;
        var uus = new Toode();
        if (!LoeVorm(uus)) return;
        string? uusFail = null;
        try
        {
            if (valitudPilt != null)
                uus.Pilt = uusFail = Kaustad.KopeeriPilt(valitudPilt);
            _db.Toodetabel.Add(uus);
            _db.SaveChanges();
            TyhjendaVorm();
            LaeTooted();
            MessageBox.Show("Toode lisatud.");
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            KustutaSalvestamataPilt(uusFail);
            MessageBox.Show("Toote lisamine ebaõnnestus: " + ex.Message);
        }
    }

    private void dataGridViewTooted_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var rida = dataGridViewTooted.Rows[e.RowIndex];
        txtNimetus.Text = Convert.ToString(rida.Cells["Toodenimetus"].Value);
        txtKogus.Text = Convert.ToString(rida.Cells["Kogus"].Value);
        txtHind.Text = Convert.ToString(rida.Cells["Hind"].Value);
        comboKategooria.SelectedValue = rida.Cells["KategooriaId"].Value;
        valitudPilt = null; // Ainult uut valitud pilti kopeeritakse.
        NaitaPilt(Convert.ToString(rida.Cells["Pilt"].Value), false);
    }

    private void btnUuenda_Click(object? sender, EventArgs e)
    {
        if (!AktiivneKasutaja.OnAdmin) return;
        if (ValitudId() is not int id) return;
        var toode = _db.Toodetabel.Find(id);
        if (toode == null)
        {
            MessageBox.Show("Toodet ei leitud.");
            return;
        }
        if (!LoeVorm(toode)) return;
        string? uusFail = null;
        try
        {
            if (valitudPilt != null)
                toode.Pilt = uusFail = Kaustad.KopeeriPilt(valitudPilt);
            _db.SaveChanges();
            TyhjendaVorm();
            LaeTooted();
            MessageBox.Show("Toode uuendatud.");
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            KustutaSalvestamataPilt(uusFail);
            MessageBox.Show("Toote uuendamine ebaõnnestus: " + ex.Message);
        }
    }

    private void btnKustuta_Click(object? sender, EventArgs e)
    {
        if (!AktiivneKasutaja.OnAdmin) return;
        if (ValitudId() is not int id) return;
        if (MessageBox.Show("Kas kustutan valitud toote?", "Kinnitus",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        try
        {
            var toode = _db.Toodetabel.Find(id);
            if (toode == null) return;
            _db.Toodetabel.Remove(toode);
            _db.SaveChanges();
            TyhjendaVorm();
            LaeTooted();
            MessageBox.Show("Toode kustutatud.");
        }
        catch (DbUpdateException ex)
        {
            _db.ChangeTracker.Clear();
            MessageBox.Show("Toodet ei saanud kustutada: " + ex.GetBaseException().Message);
        }
    }

    private static void KustutaSalvestamataPilt(string? nimi)
    {
        if (nimi == null) return;
        try { File.Delete(Path.Combine(Kaustad.Pildid, nimi)); }
        catch (IOException) { /* Põhiviga on juba kasutajale näidatud. */ }
        catch (UnauthorizedAccessException) { }
    }

    private void btnNaidised_Click(object? sender, EventArgs e)
    {
        if (!AktiivneKasutaja.OnAdmin) return;
        if (MessageBox.Show(
            "Kas lisada maxima.ee näidistooted (23.10.2026)? " +
            "Olemasolevaid tooteid ei muudeta. Hinnad on ainult õppeandmed.",
            "Näidistooted", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            != DialogResult.Yes) return;
        try
        {
            int lisatud = NaidisTooted.LisaAndmed(_db);
            LaeKategooriad();
            LaeTooted();
            MessageBox.Show(lisatud == 0
                ? "Näidistooted on juba olemas."
                : $"Lisati {lisatud} näidistoodet.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Näidistoodete lisamine ebaõnnestus: " + ex.Message);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        var pilt = picturePilt.Image;
        picturePilt.Image = null;
        pilt?.Dispose();
        _db.Dispose();
        base.OnFormClosed(e);
    }
}
