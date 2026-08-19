using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmAddEditUser : Form
    {
        #region Controls Declaration

        private Label       lblTitle;

        // ── Tab Control ──────────────────────────────────────────────
        private TabControl  tcMain;
        private TabPage     tpPersonalInfo;
        private TabPage     tpLoginInfo;

        // ── Tab 1: Personal Info ─────────────────────────────────────
        // Filter strip
        private Label       lblFindBy;
        private ComboBox    cbFindBy;
        private TextBox     txtFindValue;
        private Button      btnFindPerson;
        private Button      btnOpenFindDialog;  // opens frmFindPerson dialog

        // Person info display (read-only card inside tab 1)
        private Panel       pnlPersonCard;
        private Label       lblPersonIDTitle,    lblPersonID;
        private Label       lblNameTitle,        lblFullName;
        private Label       lblNationalNoTitle,  lblNationalNo;
        private Label       lblGenderTitle,      lblGender;
        private Label       lblDOBTitle,         lblDOB;
        private Label       lblPhoneTitle,       lblPhone;
        private Label       lblEmailTitle,       lblEmail;
        private Label       lblCountryTitle,     lblCountry;
        private Label       lblAddressTitle,     lblAddress;
        private PictureBox  pbPersonImage;
        private LinkLabel   llEditPersonInfo;

        // ── Tab 2: Login Info ────────────────────────────────────────
        private Label       lblUserIDTitle,      lblUserID;
        private Label       lblUserNameTitle;
        private TextBox     txtUserName;
        private Label       lblPasswordTitle;
        private TextBox     txtPassword;
        private Label       lblConfirmPassTitle;
        private TextBox     txtConfirmPassword;
        private CheckBox    chkIsActive;

        // ── Bottom buttons (outside tabs) ────────────────────────────
        private Button      btnNext;
        private Button      btnSave;
        private Button      btnClose;

        #endregion

        #region State

        private readonly int _userID;
        private clsUser      _user;
        private clsPerson    _person;

        private bool _isEditMode => _userID > 0;

        #endregion

        /// <summary>Gets the ID of the user after successful save.</summary>
        // public int UserID => _user?.ID ?? _userID;

        // ── Constructors ────────────────────────────────────────────────────

        public frmAddEditUser(int userID = 0)
        {
            _userID = userID;
            _InitializeComponents();
            _LoadData();
        }

        // ── Form Build ──────────────────────────────────────────────────────

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text            = _isEditMode ? "Update User" : "Add New User";
            this.Size            = new Size(990, 700);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;
            this.Font            = new Font("Microsoft Sans Serif", 9.5F);

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text      = _isEditMode ? "Update User" : "Add New User",
                Font      = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize  = true,
                Location  = new Point(360, 18)
            };

            // ── Tab Control ──────────────────────────────────────────
            tcMain = new TabControl
            {
                Location  = new Point(20, 65),
                Size      = new Size(940, 550),
                Font      = new Font("Microsoft Sans Serif", 10F)
            };

            tpPersonalInfo = new TabPage("  Personal Info  ");
            tpLoginInfo    = new TabPage("  Login Info  ");

            tcMain.TabPages.Add(tpPersonalInfo);
            tcMain.TabPages.Add(tpLoginInfo);
            tcMain.SelectedIndexChanged += tcMain_SelectedIndexChanged;

            _BuildPersonalInfoTab();
            _BuildLoginInfoTab();

            // ── Next button (only shown on tab 1) ───────────────────
            btnNext = new Button
            {
                Text      = "Next  →",
                Location  = new Point(810, 625),
                Size      = new Size(150, 38),
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += (s, e) => tcMain.SelectedTab = tpLoginInfo;

            // ── Save button (only shown on tab 2) ───────────────────
            btnSave = new Button
            {
                Text      = "💾  Save",
                Location  = new Point(650, 625),
                Size      = new Size(150, 38),
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Visible   = false
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += btnSave_Click;

            // ── Close button (always visible) ────────────────────────
            btnClose = new Button
            {
                Text      = "✖  Close",
                Location  = new Point(20, 625),
                Size      = new Size(150, 38),
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(192, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            // ── Add to form ───────────────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                lblTitle, tcMain, btnNext, btnSave, btnClose
            });
        }

        // ── Tab 1: Personal Info ─────────────────────────────────────────────

        private void _BuildPersonalInfoTab()
        {
            tpPersonalInfo.BackColor = Color.White;
            tpPersonalInfo.Padding   = new Padding(10);

            // ── Filter strip ─────────────────────────────────────────
            var pnlFilter = new Panel
            {
                Location  = new Point(10, 10),
                Size      = new Size(900, 55),
                BackColor = Color.FromArgb(245, 247, 252),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblFindBy = new Label
            {
                Text     = "Find By:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 16)
            };

            cbFindBy = new ComboBox
            {
                Location      = new Point(90, 13),
                Size          = new Size(160, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor        = Cursors.Hand
            };
            cbFindBy.Items.AddRange(new object[] { "National No.", "Person ID" });
            cbFindBy.SelectedIndex = 0;
            cbFindBy.SelectedIndexChanged += (s, e) => txtFindValue.Clear();

            txtFindValue = new TextBox
            {
                Location = new Point(260, 13),
                Size     = new Size(280, 23),
                Font     = new Font("Microsoft Sans Serif", 9.5F)
            };
            txtFindValue.KeyPress += txtFindValue_KeyPress;
            txtFindValue.KeyDown  += (s, e) => { if (e.KeyCode == Keys.Enter) btnFindPerson.PerformClick(); };

            btnFindPerson = _MakeIconButton("🔍", 550, 10, 40, Color.FromArgb(0, 120, 215));
            btnFindPerson.Click += btnFindPerson_Click;

            btnOpenFindDialog = _MakeIconButton("👤", 598, 10, 40, Color.FromArgb(80, 130, 80));
            btnOpenFindDialog.Click += btnOpenFindDialog_Click;

            pnlFilter.Controls.AddRange(new Control[]
            {
                lblFindBy, cbFindBy, txtFindValue, btnFindPerson, btnOpenFindDialog
            });

            // ── Person Info Card ─────────────────────────────────────
            pnlPersonCard = new Panel
            {
                Location    = new Point(10, 75),
                Size        = new Size(900, 430),
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Left column info rows
            int y = 25; const int step = 42;
            const int tx = 20, vx = 165;

            _MakeCardRow(pnlPersonCard, "Person ID:",   tx, vx, y, out lblPersonIDTitle,   out lblPersonID,   Color.SteelBlue);        y += step;
            _MakeCardRow(pnlPersonCard, "Name:",        tx, vx, y, out lblNameTitle,       out lblFullName,   Color.FromArgb(30,80,160)); y += step;
            _MakeCardRow(pnlPersonCard, "National No:", tx, vx, y, out lblNationalNoTitle, out lblNationalNo, Color.Black);             y += step;
            _MakeCardRow(pnlPersonCard, "Gender:",      tx, vx, y, out lblGenderTitle,     out lblGender,     Color.Black);             y += step;
            _MakeCardRow(pnlPersonCard, "Date of Birth:", tx, vx, y, out lblDOBTitle,      out lblDOB,        Color.Black);             y += step;
            _MakeCardRow(pnlPersonCard, "Phone:",       tx, vx, y, out lblPhoneTitle,      out lblPhone,      Color.Black);             y += step;
            _MakeCardRow(pnlPersonCard, "Email:",       tx, vx, y, out lblEmailTitle,      out lblEmail,      Color.FromArgb(0,102,204)); y += step;
            _MakeCardRow(pnlPersonCard, "Country:",     tx, vx, y, out lblCountryTitle,    out lblCountry,    Color.Black);             y += step;
            _MakeCardRow(pnlPersonCard, "Address:",     tx, vx, y, out lblAddressTitle,    out lblAddress,    Color.DimGray);

            // Photo
            pbPersonImage = new PictureBox
            {
                Location    = new Point(700, 25),
                Size        = new Size(175, 190),
                SizeMode    = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor   = Color.FromArgb(235, 237, 244)
            };

            // Edit Person link
            llEditPersonInfo = new LinkLabel
            {
                Text      = "✏  Edit Person Info",
                AutoSize  = true,
                Location  = new Point(700, 225),
                Font      = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = Color.SteelBlue,
                Visible   = false
            };
            llEditPersonInfo.LinkClicked += llEditPersonInfo_LinkClicked;

            pnlPersonCard.Controls.AddRange(new Control[]
            {
                pbPersonImage, llEditPersonInfo
            });

            _SetPersonCardVisible(false);   // blank until someone is found

            tpPersonalInfo.Controls.AddRange(new Control[]
            {
                pnlFilter, pnlPersonCard
            });
        }

        // ── Tab 2: Login Info ────────────────────────────────────────────────

        private void _BuildLoginInfoTab()
        {
            tpLoginInfo.BackColor = Color.White;
            tpLoginInfo.Padding   = new Padding(10);

            var pnlLogin = new Panel
            {
                Location    = new Point(10, 10),
                Size        = new Size(900, 480),
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            int y = 40; const int step = 55;
            const int lx = 130, fx = 295, fw = 260;

            // User ID (read-only)
            lblUserIDTitle = _MakeBoldLabel("User ID:", lx, y);
            lblUserID      = new Label
            {
                Text      = "???",
                Location  = new Point(fx, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                ForeColor = Color.SteelBlue
            };
            y += step;

            // Username
            lblUserNameTitle = _MakeBoldLabel("UserName:", lx, y);
            txtUserName      = _MakeLoginBox(fx, y - 3, fw);
            y += step;

            // Password
            lblPasswordTitle = _MakeBoldLabel("Password:", lx, y);
            txtPassword      = _MakeLoginBox(fx, y - 3, fw);
            txtPassword.PasswordChar = '●';
            y += step;

            // Confirm Password
            lblConfirmPassTitle = _MakeBoldLabel("Confirm Password:", lx, y);
            txtConfirmPassword  = _MakeLoginBox(fx, y - 3, fw);
            txtConfirmPassword.PasswordChar = '●';
            y += step;

            // Is Active
            chkIsActive = new CheckBox
            {
                Text     = "Is Active",
                Location = new Point(fx, y),
                Font     = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Checked  = true,
                AutoSize = true,
                Cursor   = Cursors.Hand
            };

            pnlLogin.Controls.AddRange(new Control[]
            {
                lblUserIDTitle,    lblUserID,
                lblUserNameTitle,  txtUserName,
                lblPasswordTitle,  txtPassword,
                lblConfirmPassTitle, txtConfirmPassword,
                chkIsActive
            });

            tpLoginInfo.Controls.Add(pnlLogin);
        }

        // ── Factories ────────────────────────────────────────────────────────

        private static void _MakeCardRow(
            Panel parent, string titleText,
            int tx, int vx, int y,
            out Label titleLbl, out Label valueLbl,
            Color valueColor)
        {
            titleLbl = new Label
            {
                Text      = titleText,
                Location  = new Point(tx, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 90)
            };
            valueLbl = new Label
            {
                Text      = "—",
                Location  = new Point(vx, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 9.5F),
                ForeColor = valueColor
            };
            var sep = new Panel
            {
                Location  = new Point(tx, y + 20),
                Size      = new Size(660, 1),
                BackColor = Color.FromArgb(230, 232, 240)
            };
            parent.Controls.AddRange(new Control[] { titleLbl, valueLbl, sep });
        }

        private static Label _MakeBoldLabel(string text, int x, int y)
            => new Label
            {
                Text      = text,
                Location  = new Point(x, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 70)
            };

        private static TextBox _MakeLoginBox(int x, int y, int width)
            => new TextBox
            {
                Location = new Point(x, y),
                Size     = new Size(width, 26),
                Font     = new Font("Microsoft Sans Serif", 10F)
            };

        private static Button _MakeIconButton(string text, int x, int y, int width, Color back)
        {
            var btn = new Button
            {
                Text      = text,
                Location  = new Point(x, y),
                Size      = new Size(width, 30),
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Microsoft Sans Serif", 10F),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ── Data helpers ─────────────────────────────────────────────────────

        private void _LoadData()
        {
            if (!_isEditMode) return;

            _user = clsUser.Find(_userID);
            if (_user == null) return;

            _person = clsPerson.Find(_user.PersonID);
            if (_person != null) _FillPersonCard();

            // Login info
            lblUserID.Text    = _user.UserID.ToString();
            txtUserName.Text  = _user.UserName;
            chkIsActive.Checked = _user.IsActive;
        }

        private void _FillPersonCard()
        {
            if (_person == null) return;

            lblPersonID.Text   = _person.ID.ToString();
            lblFullName.Text   = _person.FullName;
            lblNationalNo.Text = _person.NationalNo;
            lblGender.Text     = clsFormat.Gender(_person.Gender);
            lblDOB.Text        = clsFormat.DateLong(_person.DateOfBirth);
            lblPhone.Text      = string.IsNullOrWhiteSpace(_person.Phone)   ? "—" : _person.Phone;
            lblEmail.Text      = string.IsNullOrWhiteSpace(_person.Email)   ? "—" : _person.Email;
            lblCountry.Text    = _person.CountryName;
            lblAddress.Text    = string.IsNullOrWhiteSpace(_person.Address) ? "—" : _person.Address;

            clsUtil.LoadPersonImage(pbPersonImage, _person.ImagePath);

            llEditPersonInfo.Visible = true;
            _SetPersonCardVisible(true);
        }

        private void _SetPersonCardVisible(bool visible)
        {
            // Toggle all row labels and separators inside pnlPersonCard
            foreach (Control c in pnlPersonCard.Controls)
                if (c != pbPersonImage && c != llEditPersonInfo)
                    c.Visible = visible;

            pbPersonImage.Visible    = visible;
            llEditPersonInfo.Visible = visible;
        }

        private void _ClearPersonCard()
        {
            _person = null;
            _SetPersonCardVisible(false);
        }

        // ── Validation ───────────────────────────────────────────────────────

        private bool _ValidateLoginInfo()
        {
            if (_person == null)
            {
                clsUtil.ShowWarning("Please find and select a person first.", "No Person Selected");
                tcMain.SelectedTab = tpPersonalInfo;
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                clsUtil.ShowWarning("Username is required.");
                txtUserName.BackColor = clsGlobal.InputError;
                return false;
            }
            txtUserName.BackColor = clsGlobal.InputValid;

            // In Add mode a password is required; in Edit mode it's optional
            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                clsUtil.ShowWarning("Password is required.");
                txtPassword.BackColor = clsGlobal.InputError;
                return false;
            }
            txtPassword.BackColor = clsGlobal.InputValid;

            if (!string.IsNullOrWhiteSpace(txtPassword.Text) &&
                txtPassword.Text != txtConfirmPassword.Text)
            {
                clsUtil.ShowWarning("Passwords do not match.");
                txtConfirmPassword.BackColor = clsGlobal.InputError;
                return false;
            }
            txtConfirmPassword.BackColor = clsGlobal.InputValid;

            return true;
        }

        // ── Events ───────────────────────────────────────────────────────────

        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool onLoginTab = tcMain.SelectedTab == tpLoginInfo;
            btnNext.Visible = !onLoginTab;
            btnSave.Visible =  onLoginTab;
        }

        private void txtFindValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Person ID: digits only
            if (cbFindBy.SelectedIndex == 1 &&
                !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void btnFindPerson_Click(object sender, EventArgs e)
        {
            string value = txtFindValue.Text.Trim();
            if (string.IsNullOrEmpty(value))
            {
                clsUtil.ShowWarning("Please enter a value to search.");
                return;
            }

            clsPerson found = cbFindBy.SelectedIndex == 0
                ? clsPerson.Find(value)                        // National No.
                : clsPerson.Find(int.Parse(value));            // Person ID

            if (found == null)
            {
                clsUtil.ShowWarning("No person found with the given value.", "Not Found");
                _ClearPersonCard();
                return;
            }

            _person = found;
            _FillPersonCard();
        }

        private void btnOpenFindDialog_Click(object sender, EventArgs e)
        {
            // Reuse the dedicated find dialog from the People module
            var dlg = new frmFindPerson();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _person = clsPerson.Find(dlg.SelectedPersonID);
            if (_person != null) _FillPersonCard();
        }

        private void llEditPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_person == null) return;

            var frm = new frmAddEditPerson(_person.ID);
            frm.DataBack += (s, personID) => { _person = clsPerson.Find(_person.ID); _FillPersonCard(); };
            frm.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!_ValidateLoginInfo()) return;

            _user ??= new clsUser();

            _user.PersonID  = _person.ID;
            _user.UserName  = txtUserName.Text.Trim();
            _user.IsActive  = chkIsActive.Checked;

            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                _user.Password = txtPassword.Text;   // hashing should happen in Business layer

            if (_user.Save())
            {
                lblUserID.Text = _user.UserID.ToString();
                clsUtil.ShowInfo("User saved successfully.", "Saved");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                clsUtil.ShowError("Failed to save user. Please try again.");
            }
        }
    }
}