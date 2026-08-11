using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{

    public class frmDetainLicense : Form
    {
        private Label   lblTitle, lblLicIDt, lblReasonT;
        private TextBox txtLicenseID, txtReason;
        private Button  btnFind, btnSave, btnClose;
        private Panel   pnlCard;
        private Label   lblLicID, lblDriver, lblClass, lblIssue, lblExpiry, lblStatus;
        private Label   lblLicIDv, lblDriverv, lblClassv, lblIssuev, lblExpiryv, lblStatusv;

        private clsLicense _license;

        public frmDetainLicense() { _Build(); }

        private void _Build()
        {
            this.Text = "Detain License"; this.Size = new Size(580, 490);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Detain Driving License",
                Font = new Font("Arial", 16F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(150, 18) };

            var lblID = new Label { Text = "License ID:", AutoSize = true, Location = new Point(30,65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            txtLicenseID = new TextBox { Location = new Point(140,62), Size = new Size(150,23) };
            txtLicenseID.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };

            btnFind = new Button { Text = "🔍 Find", Location = new Point(300,61), Size = new Size(80,26),
                BackColor = Color.FromArgb(0,120,215), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFind.FlatAppearance.BorderSize = 0;
            btnFind.Click += _Find;

            pnlCard = new Panel { Location = new Point(20,100), Size = new Size(530,210),
                BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Visible = false };

            int y = 15; const int s = 35, lx = 15, vx = 160;
            _R(pnlCard, "License ID:", lx, vx, y, out lblLicID,  out lblLicIDv,  Color.SteelBlue);      y += s;
            _R(pnlCard, "Driver:",     lx, vx, y, out lblDriver,  out lblDriverv, Color.FromArgb(30,80,160)); y += s;
            _R(pnlCard, "Class:",      lx, vx, y, out lblClass,   out lblClassv,  Color.Black);           y += s;
            _R(pnlCard, "Issue Date:", lx, vx, y, out lblIssue,   out lblIssuev,  Color.Black);           y += s;
            _R(pnlCard, "Expiry:",     lx, vx, y, out lblExpiry,  out lblExpiryv, Color.Black);           y += s;
            _R(pnlCard, "Status:",     lx, vx, y, out lblStatus,  out lblStatusv, Color.DarkGreen);

            lblReasonT = new Label { Text = "Detain Reason:", AutoSize = true, Location = new Point(30,325),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            txtReason = new TextBox { Location = new Point(30,347), Size = new Size(510,70),
                Multiline = true, ScrollBars = ScrollBars.Vertical };

            btnSave  = _Btn("💾  Save",  360, 432, Color.FromArgb(0,140,60));
            btnClose = _Btn("✖  Close", 465, 432, Color.FromArgb(192,50,50));
            btnSave.Click  += _Save;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblID, txtLicenseID, btnFind, pnlCard,
                lblReasonT, txtReason, btnSave, btnClose });
        }

        private void _Find(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLicenseID.Text, out int id)) { clsUtil.ShowWarning("Enter a valid License ID."); return; }
            _license = clsLicense.Find(id);
            if (_license == null) { clsUtil.ShowWarning("License not found.", "Not Found"); pnlCard.Visible = false; return; }
            lblLicIDv.Text  = _license.ID.ToString();
            lblDriverv.Text = clsPerson.Find(_license.DriverID)?.FullName ?? "—";
            lblClassv.Text  = clsLicenseClass.Find(_license.LicenseClassID)?.Name ?? "—";
            lblIssuev.Text  = clsFormat.DateShort(_license.IssueDate);
            lblExpiryv.Text = clsFormat.DateShort(_license.ExpirationDate);
            lblStatusv.Text = _license.IsActive ? "Active" : "Inactive";
            pnlCard.Visible = true;
        }

        private void _Save(object sender, EventArgs e)
        {
            if (_license == null) { clsUtil.ShowWarning("Find a license first."); return; }
            if (string.IsNullOrWhiteSpace(txtReason.Text)) { clsUtil.ShowWarning("Reason is required."); return; }

            var detained = new clsDetainedLicense
            {
                LicenseID       = _license.ID,
                DetainDate      = DateTime.Now,
                // DetainReason    = txtReason.Text.Trim(),
                CreatedByUserID = clsGlobal.CurrentUserID
            };

            if (detained.Save()) { clsUtil.ShowInfo("License detained successfully."); this.DialogResult = DialogResult.OK; this.Close(); }
            else clsUtil.ShowError("Failed to detain license.");
        }

        private static void _R(Panel p, string t, int tx, int vx, int y, out Label tl, out Label vl, Color c)
        {
            tl = new Label { Text = t, Location = new Point(tx,y), AutoSize = true, Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(80,80,90) };
            vl = new Label { Text = "—", Location = new Point(vx,y), AutoSize = true, Font = new Font("Microsoft Sans Serif", 9.5F), ForeColor = c };
            p.Controls.AddRange(new Control[] { tl, vl });
        }

        private static Button _Btn(string t, int x, int y, Color c)
        {
            var b = new Button { Text = t, Location = new Point(x,y), Size = new Size(100,32),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }


}