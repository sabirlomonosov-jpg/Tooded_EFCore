using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Tooded_EFCore;

public partial class ArveForm : Form
{
    private readonly ListBox listOstukorv = new();
    private readonly Label lblKokku = new();
    private readonly TextBox txtEmail = new();
    private readonly Button btnEemalda = new(), btnGenPDF = new(), btnSaadaEmail = new();
    private readonly Label lblInfo = new();
    private string? viimanePdf;

    public ArveForm()
    {
        
        PoeStiil.Vorm(this, "Tooted • Ostukorv ja arve");
        ClientSize = new Size(860, 575);
        MinimumSize = new Size(760, 525);

        var pais = new Panel { Dock = DockStyle.Top, Height = 96, BackColor = PoeStiil.Tume };
        pais.Controls.Add(PoeStiil.Silt("OSTUKORV JA ARVE", 25, 14, 760, 37, 20, true, Color.White));
        pais.Controls.Add(PoeStiil.Silt("Kontrolli kaupu, kinnita ost ja saada PDF e-postiga", 27, 53,
            760, 25, 10, false, Color.FromArgb(185, 208, 221)));

        var raam = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(18)
        };
        raam.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        raam.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 330));
        raam.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var vasak = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16) };
        var loendiPealkiri = PoeStiil.Silt("OSTUKORVI TOOTED", 0, 0, 440, 46, 12, true);
        loendiPealkiri.Dock = DockStyle.Top;
        listOstukorv.Dock = DockStyle.Fill;
        listOstukorv.Font = new Font("Segoe UI", 11f);
        listOstukorv.BackColor = Color.White;
        listOstukorv.BorderStyle = BorderStyle.None;
        listOstukorv.ItemHeight = 27;
        lblKokku.Dock = DockStyle.Bottom;
        lblKokku.Height = 58;
        lblKokku.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
        lblKokku.ForeColor = PoeStiil.Peamine;
        lblKokku.TextAlign = ContentAlignment.MiddleLeft;
        vasak.Controls.Add(listOstukorv);
        vasak.Controls.Add(loendiPealkiri);
        vasak.Controls.Add(lblKokku);

        var parem = new Panel { Dock = DockStyle.Fill, BackColor = Color.White,
            Margin = new Padding(14, 0, 0, 0), AutoScroll = true };
        parem.Controls.Add(PoeStiil.Silt("ARVE JA SAATMINE", 20, 18, 285, 30, 12, true));
        parem.Controls.Add(PoeStiil.Silt("Saaja e-post", 20, 60, 285, 22, 10, true));
        txtEmail.SetBounds(20, 86, 285, 34);
        txtEmail.PlaceholderText = "nimi@example.com";
        PoeStiil.Sisend(txtEmail);
        parem.Controls.Add(txtEmail);

        SeaNupp(btnEemalda, "Eemalda valitud toode", 144, btnEemalda_Click, false);
        SeaNupp(btnGenPDF, "Kinnita ost ja loo PDF", 195, btnGenPDF_Click, true);
        SeaNupp(btnSaadaEmail, "Saada arve e-postiga", 246, btnSaadaEmail_Click, false);
        lblInfo.SetBounds(20, 320, 285, 115);
        lblInfo.ForeColor = PoeStiil.Vaikne;
        lblInfo.Font = new Font("Segoe UI", 9.5f);
        lblInfo.Text = "PDF salvestatakse kausta\nAppData\\Local\\ToodedApp\\Arved.\n\nE-posti seadistust kontrolli ArveForm.cs failis.";
        parem.Controls.AddRange(new Control[] { btnEemalda, btnGenPDF, btnSaadaEmail, lblInfo });

        raam.Controls.Add(vasak, 0, 0);
        raam.Controls.Add(parem, 1, 0);
        Controls.Add(raam);
        Controls.Add(pais);
        NaitaKorvi();
    }

    private static void SeaNupp(Button nupp, string tekst, int y,
        EventHandler kasitleja, bool peamine)
    {
        nupp.Text = tekst;
        nupp.SetBounds(20, y, 285, 41);
        PoeStiil.Nupp(nupp, peamine);
        nupp.Click += kasitleja;
    }

    private void NaitaKorvi()
    {
        listOstukorv.Items.Clear();
        foreach (var rida in Ostukorv.Ridad)
            listOstukorv.Items.Add(rida);
        lblKokku.Text = $"Kokku: {Ostukorv.Kokku:F2} €";
        btnEemalda.Enabled = Ostukorv.Ridad.Count > 0;
        btnGenPDF.Enabled = Ostukorv.Ridad.Count > 0;
        btnSaadaEmail.Enabled = viimanePdf != null && File.Exists(viimanePdf);
    }

    private void btnEemalda_Click(object? sender, EventArgs e)
    {
        if (listOstukorv.SelectedItem is OstukorviRida rida)
        {
            Ostukorv.Ridad.Remove(rida);
            NaitaKorvi();
        }
    }

    private sealed record ArveRida(string Nimi, int Kogus, decimal Hind)
    {
        public decimal Summa => Hind * Kogus;
    }

    // Ostu kinnitamisel vähendatakse laoseisu ühe SQL-tehinguga.
    // Kui PDF-i salvestamine ebaõnnestub, pööratakse SQL-tehing tagasi.
    private void btnGenPDF_Click(object? sender, EventArgs e)
    {
        if (Ostukorv.Ridad.Count == 0)
        {
            MessageBox.Show("Ostukorv on tühi.");
            return;
        }

        string? pdfFail = null;
        try
        {
            Kaustad.Loo();
            pdfFail = Path.Combine(Kaustad.Arved,
                $"Arve_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.pdf");

            using var db = new ToodedContext();
            using var tehing = db.Database.BeginTransaction();
            var arveRead = new List<ArveRida>();

            foreach (var korviRida in Ostukorv.Ridad)
            {
                int id = korviRida.Toode.Id;
                int kogus = korviRida.Kogus;
                if (kogus <= 0)
                    throw new InvalidOperationException("Kauba kogus ei ole sobiv.");

                var toode = db.Toodetabel.AsNoTracking().SingleOrDefault(t => t.Id == id);
                if (toode == null)
                    throw new InvalidOperationException("Toodet enam andmebaasis ei ole.");

                int muudetud = db.Database.ExecuteSqlInterpolated(
                    $"UPDATE [Toodetabel] SET [Kogus] = [Kogus] - {kogus} WHERE [Id] = {id} AND [Kogus] >= {kogus}");
                if (muudetud != 1)
                    throw new InvalidOperationException($"Toodet '{toode.Toodenimetus}' pole piisavalt laos.");

                // Arvel kasutatakse ostuhetkel andmebaasis kehtinud hinda.
                arveRead.Add(new ArveRida(toode.Toodenimetus, kogus, toode.Hind));
            }

            // Kui kassas olevad hinnad on vahepeal muutunud, ärme müü ootamatu hinnaga.
            if (arveRead.Sum(x => x.Summa) != Ostukorv.Kokku)
                throw new InvalidOperationException(
                    "Toote hind on muutunud. Ava pood uuesti ja kontrolli ostukorvi.");

            GenereeriPdf(pdfFail, arveRead);
            tehing.Commit();
            viimanePdf = pdfFail;
            Ostukorv.Tyhjenda();
            NaitaKorvi();
            MessageBox.Show("Ost sooritatud ja PDF salvestatud:\n" + viimanePdf);
        }
        catch (Exception ex)
        {
            // Tehingu Dispose() teeb Commit()-ita automaatselt rollback'i.
            if (pdfFail != null)
            {
                try { if (File.Exists(pdfFail)) File.Delete(pdfFail); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            MessageBox.Show("Ostu või PDF-i loomise viga: " + ex.Message);
        }
    }

    private static void GenereeriPdf(string fail, List<ArveRida> read)
    {
        if (read.Count == 0) throw new InvalidOperationException("Ostukorv on tühi.");
        using var dokument = new PdfDocument();
        dokument.Info.Title = "Toodete poe arve";
        var pealkiri = new XFont("Arial", 20, XFontStyle.Bold);
        var tavaline = new XFont("Arial", 11, XFontStyle.Regular);
        var paks = new XFont("Arial", 13, XFontStyle.Bold);
        decimal summa = read.Sum(x => x.Summa);
        int lehti = Math.Max(1, (read.Count + 18) / 19);

        for (int leheNumber = 0; leheNumber < lehti; leheNumber++)
        {
            PdfPage leht = dokument.AddPage();
            using var gfx = XGraphics.FromPdfPage(leht);
            gfx.DrawString("ARVE", pealkiri, XBrushes.Black,
                new XRect(0, 35, leht.Width.Point, 35), XStringFormats.TopCenter);
            gfx.DrawString($"Kuupäev: {DateTime.Now:dd.MM.yyyy HH:mm}", tavaline, XBrushes.Black, 40, 95);
            gfx.DrawString($"Leht {leheNumber + 1}/{lehti}", tavaline, XBrushes.Black, 430, 95);
            double y = 135;
            foreach (var rida in read.Skip(leheNumber * 19).Take(19))
            {
                string nimi = rida.Nimi.Length > 42 ? rida.Nimi[..39] + "..." : rida.Nimi;
                gfx.DrawString(nimi, tavaline, XBrushes.Black, 40, y);
                gfx.DrawString($"{rida.Kogus} × {rida.Hind:F2} € = {rida.Summa:F2} €",
                    tavaline, XBrushes.Black, 305, y);
                y += 30;
            }
            if (leheNumber == lehti - 1)
                gfx.DrawString($"Kokku: {summa:F2} €", paks, XBrushes.Black, 40, y + 30);
        }
        dokument.Save(fail);
    }

    private async void btnSaadaEmail_Click(object? sender, EventArgs e)
    {
        if (viimanePdf == null || !File.Exists(viimanePdf))
        {
            MessageBox.Show("Genereeri enne PDF.");
            return;
        }
        string saaja = txtEmail.Text.Trim();
        if (!MailAddress.TryCreate(saaja, out _))
        {
            MessageBox.Show("Sisesta korrektne e-posti aadress.");
            return;
        }

        string saatja = Environment.GetEnvironmentVariable("TOODED_SMTP_USER") ?? "";
        string rakenduseParool = Environment.GetEnvironmentVariable("TOODED_SMTP_APP_PASSWORD") ?? "";
        if (string.IsNullOrWhiteSpace(saatja) || string.IsNullOrWhiteSpace(rakenduseParool))
        {
            MessageBox.Show("E-posti saatmise seaded puuduvad. Vaata README faili.");
            return;
        }
        btnSaadaEmail.Enabled = false;
        try
        {
            using var kiri = new MailMessage(saatja, saaja, "Arve", "Arve on manuses.");
            kiri.Attachments.Add(new Attachment(viimanePdf));
            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(saatja, rakenduseParool),
                EnableSsl = true
            };
            await smtp.SendMailAsync(kiri);
            MessageBox.Show("Arve saadetud!");
        }
        catch (Exception ex)
        {
            MessageBox.Show("E-posti saatmine ebaõnnestus: " + ex.Message);
        }
        finally
        {
            btnSaadaEmail.Enabled = viimanePdf != null && File.Exists(viimanePdf);
        }
    }
}
