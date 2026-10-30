using System.Drawing;
using System.Windows.Forms;

namespace Tooded_EFCore;

// Ühine visuaalne stiil. Designer-faile pole vaja muuta.
public static class PoeStiil
{
    public static readonly Color Punane = Color.FromArgb(218, 41, 28);
    public static readonly Color Taust = Color.FromArgb(250, 248, 244);
    public static readonly Color Tume = Color.FromArgb(32, 32, 32);
    public static readonly Color Peamine = Color.FromArgb(255, 188, 13);
    public static readonly Color PeamineHover = Color.FromArgb(242, 172, 0);
    public static readonly Color Tekst = Color.FromArgb(36, 36, 36);
    public static readonly Color Vaikne = Color.FromArgb(110, 107, 101);
    public static readonly Color Piir = Color.FromArgb(233, 229, 222);
    public static readonly Color Oht = Color.FromArgb(174, 28, 42);
    public static readonly Color Kampaania = Color.FromArgb(255, 188, 13);
    public static readonly Color HeleSinine = Color.FromArgb(255, 245, 213);

    public static Control Logo(int x, int y)
    {
        var logo = new Panel { Bounds = new Rectangle(x, y, 64, 64), BackColor = Punane };
        logo.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen = new Pen(Kampaania, 6);
            using var arch = new System.Drawing.Drawing2D.GraphicsPath();
            arch.AddBezier(10, 53, 10, 2, 32, 2, 32, 53);
            arch.StartFigure();
            arch.AddBezier(32, 53, 32, 2, 54, 2, 54, 53);
            e.Graphics.DrawPath(pen, arch);
        };
        return logo;
    }

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
        nupp.ForeColor = ohtlik ? Color.White : Tekst;
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
        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 241, 202);
        dgv.DefaultCellStyle.SelectionForeColor = Tekst;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 248, 244);
    }
}
