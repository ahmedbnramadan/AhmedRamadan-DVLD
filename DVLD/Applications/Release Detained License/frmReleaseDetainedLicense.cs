using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmReleaseDetainedLicense : Form
    {
        private Label   lblTitle;
        private TextBox txtDetainID;
        private Button  btnFind, btnRelease, btnClose;
        private Panel   pnlCard;
        private Label   lblDetainIDv, lblLicIDv, lblDriverv, lblReasonv, lblDetainDatev;

        private clsDetainedLicense _detained;

        public frmReleaseDetainedLicense() { _Build(); }

        private void _Build()
        {
            this.Text = "Release Detained License"; this.Size = new Size(560, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Release Detained License",
                Font = new Font("Arial", 16F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(140, 18) };

            var lbl = new Label { Text = "Detain Record ID:", AutoSize = true, Location = new Point(30,65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            txtDetainID = new TextBox { Location = new Point(175,62), Size = new Size(130,23) };
            txtDetainID.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };

            btnFind = new Button { Text = "🔍 Find", Location = new Point(315,61), Size = new Size(80,26),
                BackColor = Color.FromArgb(0,120,215), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFind.FlatAppearance.BorderSize = 0;
            btnFind.Click += _Find;

            pnlCard = new Panel { Location = new Point(20,100), Size = new Size(510,180),
                BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Visible = false };

            int y = 15; const int s = 33, lx = 15, vx = 150;
            _R(pnlCard, "Detain ID:",   lx, vx, y, out _, out lblDetainIDv,   Color.SteelBlue);          y += s;
            _R(pnlCard, "License ID:",  lx, vx, y, out _, out lblLicIDv,      Color.Black);               y += s;
            _R(pnlCard, "Driver:",      lx, vx, y, out _, out lblDriverv,     Color.FromArgb(30,80,160)); y += s;
            _R(pnlCard, "Detain Date:", lx, vx, y, out _, out lblDetainDatev, Color.Black);               y += s;
            _R(pnlCard, "Reason:",      lx, vx, y, out _, out lblReasonv,     Color.DarkRed);

            btnRelease = _Btn("✔  Release", 340, 300, Color.FromArgb(0,140,60));
            btnClose   = _Btn("✖  Close",  450, 300, Color.FromArgb(192,50,50));
            btnRelease.Click += _Release;
            btnClose.Click   += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, lbl, txtDetainID, btnFind, pnlCard, btnRelease, btnClose });
        }

        private void _Find(object sender, EventArgs e)
        {
            if (!int.TryParse(txtDetainID.Text, out int id)) { clsUtil.ShowWarning("Enter a valid ID."); return; }
            _detained = clsDetainedLicense.Find(id);
            if (_detained == null) { clsUtil.ShowWarning("Record not found.", "Not Found"); pnlCard.Visible = false; return; }
            lblDetainIDv.Text   = _detained.ID.ToString();
            lblLicIDv.Text      = _detained.LicenseID.ToString();
            lblDriverv.Text     = clsPerson.Find(clsLicense.Find(_detained.LicenseID)?.DriverID ?? -1)?.FullName ?? "—";
            lblDetainDatev.Text = clsFormat.DateShort(_detained.DetainDate);
            // lblReasonv.Text     = _detained.DetainReason;
            pnlCard.Visible     = true;
        }

        private void _Release(object sender, EventArgs e)
        {
            if (_detained == null) { clsUtil.ShowWarning("Find a detained record first."); return; }
            if (!clsUtil.ConfirmDelete("release this detained license")) return;
            _detained.ReleaseDate      = DateTime.Now;
            _detained.ReleasedByUserID = clsGlobal.CurrentUserID;
            if (_detained.Release(_detained.ID,_detained.ReleaseApplicationID ?? -1)) { clsUtil.ShowInfo("License released successfully."); this.DialogResult = DialogResult.OK; this.Close(); }
            else clsUtil.ShowError("Failed to release license.");
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