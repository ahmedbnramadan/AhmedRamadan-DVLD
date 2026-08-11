using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmReplaceLicense : Form
    {
        private Label   lblTitle;
        private TextBox txtLicenseID;
        private Button  btnFind, btnSave, btnClose;
        private Panel   pnlCard;
        private Label   lblLicIDv, lblDriverv, lblClassv, lblIssuev, lblExpiryv;
        private Label   lblReason; private ComboBox cbReason;

        private clsLicense _license;

        public frmReplaceLicense() { _Build(); }

        private void _Build()
        {
            this.Text = "Replace Lost / Damaged License"; this.Size = new Size(560, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Replace Driving License",
                Font = new Font("Arial", 16F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(145, 18) };

            var lbl = new Label { Text = "License ID:", AutoSize = true, Location = new Point(30,65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            txtLicenseID = new TextBox { Location = new Point(130,62), Size = new Size(150,23) };
            txtLicenseID.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };

            btnFind = new Button { Text = "🔍 Find", Location = new Point(290,61), Size = new Size(80,26),
                BackColor = Color.FromArgb(0,120,215), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFind.FlatAppearance.BorderSize = 0; btnFind.Click += _Find;

            pnlCard = new Panel { Location = new Point(20,100), Size = new Size(510,140),
                BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Visible = false };

            int y = 15; const int s = 28, lx = 15, vx = 150;
            _R(pnlCard, "License ID:", lx, vx, y, out _, out lblLicIDv,  Color.SteelBlue);          y += s;
            _R(pnlCard, "Driver:",     lx, vx, y, out _, out lblDriverv, Color.FromArgb(30,80,160)); y += s;
            _R(pnlCard, "Class:",      lx, vx, y, out _, out lblClassv,  Color.Black);               y += s;
            _R(pnlCard, "Issued:",     lx, vx, y, out _, out lblIssuev,  Color.Black);               y += s;
            _R(pnlCard, "Expires:",    lx, vx, y, out _, out lblExpiryv, Color.DarkRed);

            lblReason = new Label { Text = "Reason:", AutoSize = true, Location = new Point(30,255),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            cbReason = new ComboBox { Location = new Point(130,252), Size = new Size(200,23),
                DropDownStyle = ComboBoxStyle.DropDownList };
            cbReason.Items.AddRange(new object[] { "Lost", "Damaged" });
            cbReason.SelectedIndex = 0;

            btnSave  = _Btn("💾  Replace", 330, 305, Color.FromArgb(0,140,60));
            btnClose = _Btn("✖  Close",   445, 305, Color.FromArgb(192,50,50));
            btnSave.Click  += _Save;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lbl, txtLicenseID, btnFind, pnlCard, lblReason, cbReason, btnSave, btnClose });
        }

        private void _Find(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLicenseID.Text, out int id)) { clsUtil.ShowWarning("Enter a valid License ID."); return; }
            _license = clsLicense.Find(id);
            if (_license == null) { clsUtil.ShowWarning("License not found."); pnlCard.Visible = false; return; }
            lblLicIDv.Text  = _license.ID.ToString();
            lblDriverv.Text = clsPerson.Find(_license.DriverID)?.FullName ?? "—";
            // lblClassv.Text  = clsLicenseClass.Find(_license.LicenseClassID)?.ClassName ?? "—";
            lblIssuev.Text  = clsFormat.DateShort(_license.IssueDate);
            lblExpiryv.Text = clsFormat.DateShort(_license.ExpirationDate);
            pnlCard.Visible = true;
        }

        private void _Save(object sender, EventArgs e)
        {
            if (_license == null) { clsUtil.ShowWarning("Find a license first."); return; }
            // Mark old license inactive, create replacement
            _license.IsActive = false;
            _license.Save();
            var newLic = new clsLicense
            {
                DriverID        = _license.DriverID,
                LicenseClassID  = _license.LicenseClassID,
                IssueDate       = DateTime.Now,
                ExpirationDate  = _license.ExpirationDate,   // keep original expiry
                IsActive        = true,
                // IssueReason     = cbReason.Text,
                CreatedByUserID = clsGlobal.CurrentUserID
            };
            if (newLic.Save()) { clsUtil.ShowInfo($"Replacement license issued. New ID: {newLic.ID}"); this.DialogResult = DialogResult.OK; this.Close(); }
            else clsUtil.ShowError("Failed to issue replacement license.");
        }

        private static void _R(Panel p, string t, int tx, int vx, int y, out Label tl, out Label vl, Color c)
        {
            tl = new Label { Text = t, Location = new Point(tx,y), AutoSize = true, Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(80,80,90) };
            vl = new Label { Text = "—", Location = new Point(vx,y), AutoSize = true, Font = new Font("Microsoft Sans Serif", 9.5F), ForeColor = c };
            p.Controls.AddRange(new Control[] { tl, vl });
        }

        private static Button _Btn(string t, int x, int y, Color c)
        {
            var b = new Button { Text = t, Location = new Point(x,y), Size = new Size(110,32),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }

}