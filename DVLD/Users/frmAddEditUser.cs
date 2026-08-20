using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public enum enMode { AddNew = 0, Update = 1 }
    public class frmAddEditUser : Form
    {
        #region Controls Declaration

        private Label       lblTitle;

        // ── Tab Control ──────────────────────────────────────────────
        private TabControl  tcMain;
        private TabPage     tpPersonalInfo;
        private TabPage     tpLoginInfo;

        // Use ctrlPersonCardWithFilter instead of manual filter + card
        private ctrlPersonCardWithFilter ctrlPersonCardWithFilter1;
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
        private enMode       _mode;


        private bool _isEditMode => _mode == enMode.Update;

        #endregion

        /// <summary>Gets the ID of the user after successful save.</summary>
        // public int UserID => _user?.ID ?? _userID;

        // ── Constructors ────────────────────────────────────────────────────

        /// <summary>Opens the form in Add-New mode.</summary>
        public frmAddEditUser()
        {
            _userID = -1;
            _mode = enMode.AddNew;
            _InitializeComponents();
            _LoadData();
        }

        /// <summary>Opens the form in Update mode for the given user.</summary>
        public frmAddEditUser(int userID)
        {
            _userID = userID;
            _mode = enMode.Update;
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

            // ── Use ctrlPersonCardWithFilter control ─────────────────────────
            ctrlPersonCardWithFilter1 = new ctrlPersonCardWithFilter
            {
                Location = new Point(10, 10),
                Size     = new Size(900, 430),
                Font     = new Font("Microsoft Sans Serif", 9.5F)
            };

            // Handle person loaded event
            ctrlPersonCardWithFilter1.PersonLoaded += CtrlPersonCardWithFilter1_PersonLoaded;

            // Hide filter in update mode (when loading existing user)
            if (_isEditMode)
            {
                ctrlPersonCardWithFilter1.FilterVisible = false;
            }

            tpPersonalInfo.Controls.Add(ctrlPersonCardWithFilter1);
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

        // ── Data helpers ─────────────────────────────────────────────────────

        private void _LoadData()
        {
            if (!_isEditMode) return;

            _user = clsUser.Find(_userID);
            if (_user == null) return;

            _person = clsPerson.Find(_user.PersonID);

            // Load person into the ctrlPersonCardWithFilter
            if (_person != null)
            {
                ctrlPersonCardWithFilter1.LoadPersonInfo(_person.ID);
            }
            // Login info
            lblUserID.Text    = _user.UserID.ToString();
            txtUserName.Text  = _user.UserName;
            chkIsActive.Checked = _user.IsActive;
        }

        private void CtrlPersonCardWithFilter1_PersonLoaded(object sender, clsPerson person)
        {
            _person = person;
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