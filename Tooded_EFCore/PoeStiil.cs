using System.Drawing;
using System.Windows.Forms;

namespace Tooded_EFCore;

// Ühine visuaalne stiil. Designer-faile pole vaja muuta.
public static class PoeStiil
{
    public static readonly Color Taust = Color.FromArgb(244, 247, 252);
    public static readonly Color Tume = Color.FromArgb(9, 52, 118);
    public static readonly Color Peamine = Color.FromArgb(221, 34, 48);
    public static readonly Color PeamineHover = Color.FromArgb(192, 25, 39);
    public static readonly Color Tekst = Color.FromArgb(21, 41, 74);
    public static readonly Color Vaikne = Color.FromArgb(105, 119, 144);
    public static readonly Color Piir = Color.FromArgb(221, 229, 240);
    public static readonly Color Oht = Color.FromArgb(174, 28, 42);
    public static readonly Color Kampaania = Color.FromArgb(255, 220, 46);
    public static readonly Color HeleSinine = Color.FromArgb(229, 237, 250);

    public static void Vorm(Form vorm, string pealkiri)
    {
        vorm.Text = pealkiri;
        vorm.Font = new Font("Segoe UI", 10f);
        vorm.BackColor = Taust;
        vorm.StartPosition = FormStartPosition.CenterScreen;
    }

    public static Label Silt(string tekst, int x, int y, int laius, int korgus,
        float suurus = 10, bool paks = false, Color? varv = null)
    {
        return new Label
        {
            Text = tekst,
            Bounds = new Rectangle(x, y, laius, korgus),
            Font = new Font("Segoe UI", suurus, paks ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = varv ?? Tekst,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };
    }

    public static void Nupp(Button nupp, bool peamine = true, bool ohtlik = false)
    {
        nupp.FlatStyle = FlatStyle.Flat;
        nupp.FlatAppearance.BorderSize = peamine || ohtlik ? 0 : 1;
        nupp.FlatAppearance.BorderColor = Piir;
        nupp.BackColor = ohtlik ? Oht : peamine ? Peamine : Color.White;
        nupp.ForeColor = peamine || ohtlik ? Color.White : Tekst;
        nupp.Cursor = Cursors.Hand;
        nupp.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        nupp.FlatAppearance.MouseOverBackColor = ohtlik
            ? Color.FromArgb(155, 42, 52) : peamine ? PeamineHover : Taust;
        nupp.UseVisualStyleBackColor = false;
        nupp.TabStop = true;
    }

    public static void Sisend(TextBox box)
    {
        box.Font = new Font("Segoe UI", 10.5f);
        box.BorderStyle = BorderStyle.FixedSingle;
        box.BackColor = Color.White;
        box.ForeColor = Tekst;
    }

    public static void Tabel(DataGridView dgv)
    {
        dgv.BorderStyle = BorderStyle.None;
        dgv.BackgroundColor = Color.White;
        dgv.GridColor = Piir;
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.RowHeadersVisible = false;
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgv.ColumnHeadersHeight = 43;
        dgv.RowTemplate.Height = 39;
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Tume;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Tume;
        dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
        dgv.DefaultCellStyle.ForeColor = Tekst;
        dgv.DefaultCellStyle.BackColor = Color.White;
        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 239, 255);
        dgv.DefaultCellStyle.SelectionForeColor = Tekst;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 250, 252);
    }
}
