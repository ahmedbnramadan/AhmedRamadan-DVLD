using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmAddNewLocalDrivingLicenseApplication : Form
    {
        #region Controls
        private Label    lblTitle;
        private TabControl tc;
        private TabPage  tpPerson, tpAppInfo;

        // Tab1 - Person
        private Label    lblFindBy; private ComboBox cbFindBy;
        private TextBox  txtFindVal; private Button btnFind, btnOpenDialog;
        private Panel    pnlPerson;
        private Label    lblPIDt, lblPID, lblNamet, lblName, lblNNot, lblNNo;
        private Label    lblGent, lblGen, lblDOBt, lblDOB, lblPhonet, lblPhone;
        private Label    lblEmailt, lblEmail, lblCountryt, lblCountry;
        private PictureBox pbImg;

        // Tab2 - Application Info
        private Label    lblAppIDt, lblAppID, lblDatet, lblApplFees;
        private DateTimePicker dtpDate;
        private Label    lblLicClass; private ComboBox cbLicClass;
        private Label    lblFees; private TextBox txtFees;
        private CheckBox chkPassedVision;

        private Button   btnNext, btnSave, btnClose;
        #endregion

        private int              _appID;
        private clsPerson        _person;
        private bool _isEdit => _appID > 0;

        public frmAddNewLocalDrivingLicenseApplication(int appID = -1)
        {
            _appID = appID;
            _Build();
            _LoadLicenseClasses();
            if (_isEdit) _LoadAppData();
        }

        private void _Build()
        {
            this.Text = _isEdit ? "Edit Application" : "New Local Driving License Application";
            this.Size = new Size(980, 660); this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = _isEdit ? "Edit Application" : "New Driving License Application",
                Font = new Font("Arial", 16F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(250, 15) };

            tc = new TabControl { Location = new Point(15, 55), Size = new Size(940, 530),
                Font = new Font("Microsoft Sans Serif", 10F) };
            tpPerson  = new TabPage("  Personal Info  ");
            tpAppInfo = new TabPage("  Application Info  ");
            tc.TabPages.Add(tpPerson); tc.TabPages.Add(tpAppInfo);
            tc.SelectedIndexChanged += (s, e) => {
                btnNext.Visible = tc.SelectedTab == tpPerson;
                btnSave.Visible = tc.SelectedTab == tpAppInfo;
            };

            _BuildPersonTab();
            _BuildAppInfoTab();

            btnNext  = _Btn("Next  →", 780, 595, Color.FromArgb(0,120,215));
            btnSave  = _Btn("💾  Save", 620, 595, Color.FromArgb(0,140,60)); btnSave.Visible = false;
            btnClose = _Btn("✖  Close", 15, 595, Color.FromArgb(192,50,50));
            btnNext.Click  += (s, e) => { if (_person == null) { clsUtil.ShowWarning("Find a person first."); return; } tc.SelectedTab = tpAppInfo; };
            btnSave.Click  += _Save;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, tc, btnNext, btnSave, btnClose });
        }

        private void _BuildPersonTab()
        {
            tpPerson.BackColor = Color.White;

            // Filter strip
            var pnlF = new Panel { Location = new Point(10,10), Size = new Size(905,50),
                BackColor = Color.FromArgb(245,247,252), BorderStyle = BorderStyle.FixedSingle };

            lblFindBy = new Label { Text = "Find By:", AutoSize = true, Location = new Point(15,14),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            cbFindBy = new ComboBox { Location = new Point(85,11), Size = new Size(150,23), DropDownStyle = ComboBoxStyle.DropDownList };
            cbFindBy.Items.AddRange(new object[] { "National No.", "Person ID" });
            cbFindBy.SelectedIndex = 0;
            cbFindBy.SelectedIndexChanged += (s, e) => txtFindVal.Clear();

            txtFindVal = new TextBox { Location = new Point(245,11), Size = new Size(260,23) };
            txtFindVal.KeyPress += (s, e) => {
                if (cbFindBy.SelectedIndex == 1 && !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };
            txtFindVal.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnFind.PerformClick(); };

            btnFind       = _SmallBtn("🔍", 515, 9, Color.FromArgb(0,120,215));
            btnOpenDialog = _SmallBtn("👤", 560, 9, Color.FromArgb(80,130,80));
            btnFind.Click       += _FindPerson;
            btnOpenDialog.Click += (s, e) => {
                var dlg = new frmFindPerson();
                if (dlg.ShowDialog() == DialogResult.OK) { _person = clsPerson.Find(dlg.SelectedPersonID); _FillCard(); }
            };
            pnlF.Controls.AddRange(new Control[] { lblFindBy, cbFindBy, txtFindVal, btnFind, btnOpenDialog });

            // Card
            pnlPerson = new Panel { Location = new Point(10,70), Size = new Size(905,420),
                BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            int y = 22; const int s = 42, tx = 20, vx = 160;
            _Row(pnlPerson, "Person ID:", tx, vx, y, out lblPIDt, out lblPID, Color.SteelBlue);          y += s;
            _Row(pnlPerson, "Name:", tx, vx, y, out lblNamet, out lblName, Color.FromArgb(30,80,160));   y += s;
            _Row(pnlPerson, "National No:", tx, vx, y, out lblNNot, out lblNNo, Color.Black);             y += s;
            _Row(pnlPerson, "Gender:", tx, vx, y, out lblGent, out lblGen, Color.Black);                  y += s;
            _Row(pnlPerson, "Date of Birth:", tx, vx, y, out lblDOBt, out lblDOB, Color.Black);           y += s;
            _Row(pnlPerson, "Phone:", tx, vx, y, out lblPhonet, out lblPhone, Color.Black);               y += s;
            _Row(pnlPerson, "Email:", tx, vx, y, out lblEmailt, out lblEmail, Color.FromArgb(0,102,204)); y += s;
            _Row(pnlPerson, "Country:", tx, vx, y, out lblCountryt, out lblCountry, Color.Black);

            pbImg = new PictureBox { Location = new Point(710,20), Size = new Size(170,190),
                SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(235,237,244) };
            pnlPerson.Controls.Add(pbImg);
            _SetCardVisible(false);

            tpPerson.Controls.AddRange(new Control[] { pnlF, pnlPerson });
        }

        private void _BuildAppInfoTab()
        {
            tpAppInfo.BackColor = Color.White;
            var pnl = new Panel { Location = new Point(10,10), Size = new Size(905,480),
                BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            int y = 30; const int s = 55, lx = 130, vx = 310, vw = 250;

            lblAppIDt = _BoldLbl("Application ID:", lx, y);
            lblAppID  = new Label { Text = _isEdit ? _appID.ToString() : "Auto", AutoSize = true,
                Location = new Point(vx, y), ForeColor = Color.SteelBlue,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold) };
            y += s;

            lblDatet = _BoldLbl("Application Date:", lx, y);
            dtpDate  = new DateTimePicker { Location = new Point(vx, y-3), Size = new Size(vw, 24),
                Format = DateTimePickerFormat.Short, Value = DateTime.Now };
            y += s;

            lblLicClass = _BoldLbl("License Class:", lx, y);
            cbLicClass  = new ComboBox { Location = new Point(vx, y-3), Size = new Size(vw, 24),
                DropDownStyle = ComboBoxStyle.DropDownList, Cursor = Cursors.Hand };
            cbLicClass.SelectedIndexChanged += _UpdateFees;
            y += s;

            lblFees = _BoldLbl("Application Fees:", lx, y);
            txtFees = new TextBox { Location = new Point(vx, y-3), Size = new Size(vw, 24),
                ReadOnly = true, BackColor = Color.FromArgb(245,247,252) };
            y += s;

            chkPassedVision = new CheckBox { Text = "Passed Vision Test", Location = new Point(vx, y),
                AutoSize = true, Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand };

            pnl.Controls.AddRange(new Control[] {
                lblAppIDt, lblAppID, lblDatet, dtpDate,
                lblLicClass, cbLicClass, lblFees, txtFees, chkPassedVision });

            tpAppInfo.Controls.Add(pnl);
        }

        private void _LoadLicenseClasses()
        {
            cbLicClass.DataSource    = clsLicenseClass.GetAllLicenseClasses();
            cbLicClass.DisplayMember = "ClassName";
            cbLicClass.ValueMember   = "LicenseClassID";
        }

        private void _LoadAppData()
        {
            var app = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppID(_appID);
            if (app == null) { clsUtil.ShowError("Application not found."); this.Close(); return; }
            _person = clsPerson.Find(app.ApplicantPersonID);
            if (_person != null) _FillCard();
            dtpDate.Value = app.ApplicationDate;
            cbLicClass.SelectedValue = app.LicenseClassID;
        }

        private void _FindPerson(object sender, EventArgs e)
        {
            string val = txtFindVal.Text.Trim();
            if (string.IsNullOrEmpty(val)) { clsUtil.ShowWarning("Enter a search value."); return; }
            _person = cbFindBy.SelectedIndex == 0
                ? clsPerson.Find(val)
                : clsPerson.Find(int.Parse(val));
            if (_person == null) { clsUtil.ShowWarning("Person not found.", "Not Found"); _SetCardVisible(false); return; }
            _FillCard();
        }

        private void _FillCard()
        {
            if (_person == null) return;
            lblPID.Text     = _person.ID.ToString();
            lblName.Text    = _person.FullName;
            lblNNo.Text     = _person.NationalNo;
            lblGen.Text     = clsFormat.Gender(_person.Gender);
            lblDOB.Text     = clsFormat.DateLong(_person.DateOfBirth);
            lblPhone.Text   = string.IsNullOrWhiteSpace(_person.Phone) ? "—" : _person.Phone;
            lblEmail.Text   = string.IsNullOrWhiteSpace(_person.Email) ? "—" : _person.Email;
            lblCountry.Text = _person.CountryName;
            clsUtil.LoadPersonImage(pbImg, _person.ImagePath);
            _SetCardVisible(true);
        }

        private void _SetCardVisible(bool v)
        {
            foreach (Control c in pnlPerson.Controls)
                if (c != pbImg) c.Visible = v;
            pbImg.Visible = v;
        }

        private void _UpdateFees(object sender, EventArgs e)
        {
            if (cbLicClass.SelectedValue == null) return;
            var lc = clsLicenseClass.Find(Convert.ToInt32(cbLicClass.SelectedValue));
            txtFees.Text = lc != null ? lc.Fees.ToString("F2") : "0.00";
        }

        private void _Save(object sender, EventArgs e)
        {
            if (_person == null)
            { clsUtil.ShowWarning("Select a person first."); tc.SelectedTab = tpPerson; return; }
            if (cbLicClass.SelectedValue == null)
            { clsUtil.ShowWarning("Select a license class."); return; }

            var app = _isEdit
                ? clsLocalDrivingLicenseApplication.FindByLocalDrivingAppID(_appID) ?? new clsLocalDrivingLicenseApplication()
                : new clsLocalDrivingLicenseApplication();

            app.ApplicantPersonID          = _person.ID;
            app.LicenseClassID    = Convert.ToInt32(cbLicClass.SelectedValue);
            app.ApplicationDate   = dtpDate.Value;
            app.CreatedByUserID   = clsGlobal.CurrentUserID;
            app.PaidFees          = decimal.TryParse(txtFees.Text, out decimal f) ? f : 0;

            if (app.Save())
            {
                _appID = app.LocalDrivingLicenseApplicationID;
                lblAppID.Text = _appID.ToString();
                clsUtil.ShowInfo("Application saved successfully.", "Saved");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else clsUtil.ShowError("Failed to save application.");
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static void _Row(Panel p, string title, int tx, int vx, int y,
            out Label tl, out Label vl, Color vc)
        {
            tl = new Label { Text = title, Location = new Point(tx, y), AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(80,80,90) };
            vl = new Label { Text = "—", Location = new Point(vx, y), AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F), ForeColor = vc };
            var sep = new Panel { Location = new Point(tx, y+20), Size = new Size(680,1),
                BackColor = Color.FromArgb(230,232,240) };
            p.Controls.AddRange(new Control[] { tl, vl, sep });
        }

        private static Label _BoldLbl(string t, int x, int y)
            => new Label { Text = t, Location = new Point(x,y), AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(60,60,70) };

        private static Button _Btn(string t, int x, int y, Color c)
        {
            var b = new Button { Text = t, Location = new Point(x,y), Size = new Size(155,36),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }

        private static Button _SmallBtn(string t, int x, int y, Color c)
        {
            var b = new Button { Text = t, Location = new Point(x,y), Size = new Size(38,28),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }
}