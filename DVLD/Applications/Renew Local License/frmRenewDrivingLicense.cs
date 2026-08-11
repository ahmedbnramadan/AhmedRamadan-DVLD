using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmRenewDrivingLicense : Form
    {
        private Label   lblTitle;
        private TextBox txtLicenseID;
        private Button  btnFind, btnRenew, btnClose;
        private Panel   pnlCard;
        private Label   lblLicIDv, lblDriverv, lblClassv, lblIssuev, lblExpiryv, lblNewExpiryv;

        private clsLicense _license;

        public frmRenewDrivingLicense() { _Build(); }

        private void _Build()
        {
            this.Text = "Renew Driving License";
            this.Size = new Size(560, 380);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Renew Driving License",
                Font = new Font("Arial", 16F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(160, 18) };

            var lbl = new Label { Text = "License ID:", AutoSize = true, Location = new Point(30,65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            txtLicenseID = new TextBox { Location = new Point(130,62), Size = new Size(150,23) };
            txtLicenseID.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };

            btnFind = new Button { Text = "🔍 Find", Location = new Point(290,61), Size = new Size(80,26),
                BackColor = Color.FromArgb(0,120,215), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFind.FlatAppearance.BorderSize = 0;
            btnFind.Click += _Find;

            pnlCard = new Panel { Location = new Point(20,100), Size = new Size(510,175),
                BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Visible = false };

            int y = 15; const int s = 28, lx = 15, vx = 155;
            _R(pnlCard, "License ID:",   lx, vx, y, out _, out lblLicIDv,    Color.SteelBlue);          y += s;
            _R(pnlCard, "Driver:",       lx, vx, y, out _, out lblDriverv,   Color.FromArgb(30,80,160)); y += s;
            _R(pnlCard, "Class:",        lx, vx, y, out _, out lblClassv,    Color.Black);               y += s;
            _R(pnlCard, "Current Issue:",lx, vx, y, out _, out lblIssuev,    Color.Black);               y += s;
            _R(pnlCard, "Expires:",      lx, vx, y, out _, out lblExpiryv,   Color.DarkRed);             y += s;
            _R(pnlCard, "New Expiry:",   lx, vx, y, out _, out lblNewExpiryv,Color.DarkGreen);

            btnRenew = _Btn("🔄  Renew", 335, 295, Color.FromArgb(0,140,60));
            btnClose = _Btn("✖  Close", 445, 295, Color.FromArgb(192,50,50));
            btnRenew.Click += _Renew;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, lbl, txtLicenseID, btnFind, pnlCard, btnRenew, btnClose });
        }

        private void _Find(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLicenseID.Text, out int id)) { clsUtil.ShowWarning("Enter a valid License ID."); return; }
            _license = clsLicense.Find(id);
            if (_license == null) { clsUtil.ShowWarning("License not found."); pnlCard.Visible = false; return; }
            var lc = clsLicenseClass.Find(_license.LicenseClassID);
            lblLicIDv.Text    = _license.ID.ToString();
            lblDriverv.Text   = clsPerson.Find(_license.DriverID)?.FullName ?? "—";
            lblClassv.Text    = lc?.Name ?? "—";
            lblIssuev.Text    = clsFormat.DateShort(_license.IssueDate);
            lblExpiryv.Text   = clsFormat.DateShort(_license.ExpirationDate);
            lblNewExpiryv.Text = clsFormat.DateShort(
                DateTime.Now.AddYears(lc?.DefaultValidityLength ?? 5));
            pnlCard.Visible = true;
        }

        private void _Renew(object sender, EventArgs e)
        {
            if (_license == null) { clsUtil.ShowWarning("Find a license first."); return; }
            var lc = clsLicenseClass.Find(_license.LicenseClassID);
            _license.IssueDate       = DateTime.Now;
            _license.ExpirationDate  = DateTime.Now.AddYears(lc?.DefaultValidityLength ?? 5);
            _license.IsActive        = true;
            if (_license.Save()) { clsUtil.ShowInfo("License renewed successfully."); this.DialogResult = DialogResult.OK; this.Close(); }
            else clsUtil.ShowError("Failed to renew license.");
        }

        private static void _R(Panel p, string t, int tx, int vx, int y, out Label tl, out Label vl, Color c)
        {
            tl = new Label { Text = t, Location = new Point(tx,y), AutoSize = true, Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(80,80,90) };
            vl = new Label { Text = "—", Location = new Point(vx,y), AutoSize = true, Font = new Font("Microsoft Sans Serif", 9.5F), ForeColor = c };
            p.Controls.AddRange(new Control[] { tl, vl });
        }

        private static Button _Btn(string t, int x, int y, Color c)
        {
            var b = new Button { Text = t, Location = new Point(x,y), Size = new Size(105,32),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }
}