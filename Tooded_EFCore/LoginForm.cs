using System.Drawing;

namespace Tooded_EFCore;

public partial class LoginForm : Form
{
    private readonly TextBox txtEmail = new(), txtParool = new();
    private readonly Button btnLogin = new();
    private readonly Label lblVeateade = new();

    public LoginForm()
    {
        PoeStiil.Vorm(this, "McDonald’s • Sisselogimine");
        ClientSize = new Size(465, 540);
        MinimumSize = new Size(465, 540);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var pais = new Panel { Dock = DockStyle.Top, Height = 165, BackColor = PoeStiil.Punane };
        pais.Controls.Add(PoeStiil.Logo(35, 25));
        pais.Controls.Add(PoeStiil.Silt("McDonald’s", 113, 36, 320, 45, 26, true, Color.White));
        pais.Controls.Add(PoeStiil.Silt("Hea toit. Head hetked.", 39, 88, 390, 30,
            12, false, Color.White));

        var sisu = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(36) };
        sisu.Controls.Add(PoeStiil.Silt("LOGI SISSE", 35, 7, 395, 34, 15, true));
        sisu.Controls.Add(PoeStiil.Silt("E-posti aadress", 35, 56, 395, 23, 10, true));
        txtEmail.SetBounds(35, 84, 395, 38);
        txtEmail.PlaceholderText = "nimi@example.com";
        PoeStiil.Sisend(txtEmail);
        sisu.Controls.Add(txtEmail);

        sisu.Controls.Add(PoeStiil.Silt("Parool", 35, 138, 395, 23, 10, true));
        txtParool.SetBounds(35, 165, 395, 38);
        txtParool.PlaceholderText = "Sisesta parool";
        txtParool.UseSystemPasswordChar = true;
        PoeStiil.Sisend(txtParool);
        sisu.Controls.Add(txtParool);

        btnLogin.SetBounds(35, 230, 395, 45);
        btnLogin.Text = "Logi sisse  ›";
        PoeStiil.Nupp(btnLogin);
        btnLogin.Click += btnLogin_Click;
        sisu.Controls.Add(btnLogin);
        lblVeateade.SetBounds(35, 288, 395, 48);
        lblVeateade.ForeColor = PoeStiil.Oht;
        lblVeateade.TextAlign = ContentAlignment.MiddleLeft;
        sisu.Controls.Add(lblVeateade);
        AcceptButton = btnLogin;

        Controls.Add(sisu);
        Controls.Add(pais);
    }

    private void btnLogin_Click(object? sender, EventArgs e)
    {
        string email = txtEmail.Text.Trim().ToLowerInvariant();
        try
        {
            using var db = new ToodedContext();
            var kasutaja = db.Kasutajad.FirstOrDefault(k => k.Email == email);
            if (kasutaja == null || !PasswordHelper.Verify(txtParool.Text, kasutaja.ParoolHash))
            {
                lblVeateade.Text = "Vale e-post või parool.";
                txtParool.Clear();
                return;
            }
            AktiivneKasutaja.Nimi = kasutaja.Nimi;
            AktiivneKasutaja.Roll = kasutaja.Roll;
            var menyy = new FormMain();
            menyy.FormClosed += (_, _) => Close();
            menyy.Show();
            Hide();
        }
        catch (Exception ex)
        {
            lblVeateade.Text = "Sisselogimine ebaõnnestus.";
            MessageBox.Show("Andmebaasi viga: " + ex.Message);
        }
    }
}
