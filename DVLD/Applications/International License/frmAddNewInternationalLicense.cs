using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmAddNewInternationalLicense : Form
    {
        private Label      lblTitle;
        private TextBox    txtLicenseID;
        private Button     btnFind, btnSave, btnClose;
        private Panel      pnlCard;
        private Label      lblLicIDv, lblDriverv, lblClassv, lblIssuev, lblExpiryv;
        private Label      lblDateT; private DateTimePicker dtpDate;
        private Label      lblFeesT; private TextBox txtFees;

        private clsLicense _localLicense;

        public frmAddNewInternationalLicense() { _Build(); }

        private void _Build()
        {
            this.Text = "New International License"; this.Size = new Size(570, 430);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "New International License",
                Font = new Font("Arial", 16F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(140, 18) };

            var lbl = new Label { Text = "Local License ID:", AutoSize = true, Location = new Point(30,65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            txtLicenseID = new TextBox { Location = new Point(160,62), Size = new Size(130,23) };
            txtLicenseID.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };

            btnFind = new Button { Text = "🔍 Find", Location = new Point(300,61), Size = new Size(80,26),
                BackColor = Color.FromArgb(0,120,215), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFind.FlatAppearance.BorderSize = 0; btnFind.Click += _Find;

            pnlCard = new Panel { Location = new Point(20,100), Size = new Size(520,145),
                BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Visible = false };

            int y = 15; const int s = 28, lx = 15, vx = 150;
            _R(pnlCard, "License ID:", lx, vx, y, out _, out lblLicIDv,  Color.SteelBlue);          y += s;
            _R(pnlCard, "Driver:",     lx, vx, y, out _, out lblDriverv, Color.FromArgb(30,80,160)); y += s;
            _R(pnlCard, "Class:",      lx, vx, y, out _, out lblClassv,  Color.Black);               y += s;
            _R(pnlCard, "Issued:",     lx, vx, y, out _, out lblIssuev,  Color.Black);               y += s;
            _R(pnlCard, "Expires:",    lx, vx, y, out _, out lblExpiryv, Color.DarkRed);

            lblDateT = new Label { Text = "Application Date:", AutoSize = true, Location = new Point(30,260),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            dtpDate = new DateTimePicker { Location = new Point(170,257), Size = new Size(190,24),
                Format = DateTimePickerFormat.Short, Value = DateTime.Now };

            lblFeesT = new Label { Text = "Fees (US):", AutoSize = true, Location = new Point(30,305),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            txtFees = new TextBox { Location = new Point(170,302), Size = new Size(100,23),
                Text = clsApplicationType.Find(6)?.Fees.ToString("F2") ?? "50.00",
                ReadOnly = true, BackColor = Color.FromArgb(245,247,252) };

            btnSave  = _Btn("💾  Issue",  340, 350, Color.FromArgb(0,140,60));
            btnClose = _Btn("✖  Close",  455, 350, Color.FromArgb(192,50,50));
            btnSave.Click  += _Save;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lbl, txtLicenseID, btnFind, pnlCard,
                lblDateT, dtpDate, lblFeesT, txtFees, btnSave, btnClose });
        }

        private void _Find(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLicenseID.Text, out int id)) { clsUtil.ShowWarning("Enter a valid License ID."); return; }
            _localLicense = clsLicense.Find(id);
            if (_localLicense == null) { clsUtil.ShowWarning("License not found."); pnlCard.Visible = false; return; }
            if (!_localLicense.IsActive) { clsUtil.ShowWarning("License is not active."); pnlCard.Visible = false; return; }
            lblLicIDv.Text  = _localLicense.ID.ToString();
            lblDriverv.Text = clsPerson.Find(_localLicense.DriverID)?.FullName ?? "—";
            lblClassv.Text  = clsLicenseClass.Find(_localLicense.LicenseClassID)?.Name ?? "—";
            lblIssuev.Text  = clsFormat.DateShort(_localLicense.IssueDate);
            lblExpiryv.Text = clsFormat.DateShort(_localLicense.ExpirationDate);
            pnlCard.Visible = true;
        }

        private void _Save(object sender, EventArgs e)
        {
            if (_localLicense == null) { clsUtil.ShowWarning("Find a local license first."); return; }
            var intl = new clsInternationalLicense
            {
                DriverID             = _localLicense.DriverID,
                // LocalLicenseID       = _localLicense.ID.
                // ApplicationDate      = dtpDate.Value,
                // IssueDate            = DateTime.Now,
                ExpirationDate       = DateTime.Now.AddYears(1),
                IsActive             = true,
                CreatedByUserID      = clsGlobal.CurrentUserID,
                // PaidFees             = decimal.TryParse(txtFees.Text, out decimal f) ? f : 50
            };
            if (intl.Save()) { clsUtil.ShowInfo($"International License issued. ID: {intl.InternationalLicenseID}"); this.DialogResult = DialogResult.OK; this.Close(); }
            else clsUtil.ShowError("Failed to issue international license.");
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