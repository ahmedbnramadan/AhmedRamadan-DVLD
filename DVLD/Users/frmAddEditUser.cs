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
            this.Size            = new Size(990, 750);
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
            btnNext.Click += btnNext_Click;

            // ── Save button (only shown on tab 2) ───────────────────
            btnSave = new Button
            {
                Text      = "Save",
                Location  = new Point(810, 625),
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
            if (!_isEditMode)
            {
                // Add mode: initialize form for new user
                _InitializeForAddMode();
                return;
            }

            // Edit mode: load existing user data
            try
            {
                _user = clsUser.Find(_userID);

                if (_user == null)
                {
                    clsUtil.ShowError($"User with ID {_userID} not found.", "User Not Found");
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                _person = clsPerson.Find(_user.PersonID);
                // Load person into the ctrlPersonCardWithFilter
                if (_person != null)
                {
                    ctrlPersonCardWithFilter1.LoadPersonInfo(_person.ID);
                }

                // Populate login info fields
                lblUserID.Text    = _user.UserID.ToString();
                txtUserName.Text  = _user.UserName;
                chkIsActive.Checked = _user.IsActive;

                // Disable person filter in edit mode since we're editing existing user
                ctrlPersonCardWithFilter1.FilterVisible = false;

                // Set focus to username field for quick editing
                txtUserName.Focus();
            }
            catch (Exception ex)
            {
                clsUtil.ShowError($"Failed to load user data: {ex.Message}", "Load Error");
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

        }

        private void _InitializeForAddMode()
        {
            // Reset all fields for adding a new user
            lblUserID.Text = "New";
            txtUserName.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            chkIsActive.Checked = true;

            // Ensure filter is visible in add mode
            ctrlPersonCardWithFilter1.FilterVisible = true;

            // Set focus to the filter for quick person lookup
            ctrlPersonCardWithFilter1.FocusOnFilter();
        }

        private void CtrlPersonCardWithFilter1_PersonLoaded(object sender, clsPerson person)
        {
            _person = person;
        }

        // ── Validation ───────────────────────────────────────────────────────

        /// <summary>
        /// Validates that a person is selected and checks if a user already exists for that person.
        /// If validation fails, shows appropriate warning and resets to Personal Info tab.
        /// Returns true if validation failed, false if validation passed.
        /// </summary>
        private bool _ValidatePersonSelectionAndUserExistence()
        {
            // Check if person is selected
            if (_person == null || _person.ID == -1)
            {
                clsUtil.ShowWarning("Please find and select a person first.", "No Person Selected");
                tcMain.SelectedTab = tpPersonalInfo;
                ctrlPersonCardWithFilter1.FocusOnFilter();
                return true;
            }

            // Check if user already exists for this person (only in Add mode)
            if (!_isEditMode && clsUser.IsExistsByPersonID(_person.ID))
            {
                clsUtil.ShowWarning(
                    $"The person '{_person.FullName}' already has a user account.\n\nPlease select a different person.",
                    "User Already Exists");
                tcMain.SelectedTab = tpPersonalInfo;
                ctrlPersonCardWithFilter1.Clear();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates all login information fields before saving.
        /// Returns true if all validations pass, false otherwise.
        /// </summary>
        private bool _ValidateLoginInfo()
        {
            // Validate username
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                clsUtil.ShowWarning("Username is required.", "Validation Error");
                txtUserName.BackColor = clsGlobal.InputError;
                txtUserName.Focus();
                return false;
            }
            txtUserName.BackColor = clsGlobal.InputValid;

            // Check if user already exists for this person (only in Add mode)
            if (!_isEditMode && clsUser.IsExistsByPersonID(_person.ID))
            {
                if (clsUser.IsExists(txtUserName.Text.Trim()))
                {
                    clsUtil.ShowWarning(
                        $"The person '{_person.FullName}' already has a user account.\n\nPlease select a different person.",
                        "User Already Exists");
                    tcMain.SelectedTab = tpPersonalInfo;
                    ctrlPersonCardWithFilter1.Clear();
                    return true;
                }

                return false;
            }

            // In Add mode a password is required; in Edit mode it's optional
            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                clsUtil.ShowWarning("Password is required for new users.", "Validation Error");
                txtPassword.BackColor = clsGlobal.InputError;
                txtPassword.Focus();
                return false;
            }
            txtPassword.BackColor = clsGlobal.InputValid;

            // Validate password confirmation only if password is entered
            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    clsUtil.ShowWarning("Passwords do not match.", "Validation Error");
                    txtConfirmPassword.BackColor = clsGlobal.InputError;
                    txtConfirmPassword.Focus();
                    return false;
                }
                txtConfirmPassword.BackColor = clsGlobal.InputValid;

                // Optional: Validate password strength
                if (!_IsPasswordStrongEnough(txtPassword.Text))
                {
                    clsUtil.ShowWarning(
                        "Password must be at least 6 characters long.\n\nFor better security, consider using a mix of letters, numbers, and symbols.",
                        "Weak Password");
                    txtPassword.BackColor = Color.FromArgb(255, 200, 100); // Warning color
                    // Continue anyway, just warn the user
                }
            }

            return true;
        }


        /// <summary>
        /// Checks if the password meets minimum security requirements.
        /// </summary>
        private bool _IsPasswordStrongEnough(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;

            // At least one letter and one digit recommended
            bool hasLetter = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            return hasLetter && hasDigit;
        }


        // ── Events ───────────────────────────────────────────────────────────

        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool onLoginTab = tcMain.SelectedTab == tpLoginInfo;
            btnNext.Visible = !onLoginTab;
            btnSave.Visible =  onLoginTab;

            // Set appropriate focus when switching tabs
            if (onLoginTab)
            {
                txtUserName.Focus();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Validate person selection and check for existing user
            if (_ValidatePersonSelectionAndUserExistence())
            {
                return; // Validation failed, stay on current tab
            }

            // All validations passed, navigate to Login Info tab
            tcMain.SelectedTab = tpLoginInfo;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Final validation before saving
            if (!_ValidateLoginInfo())
            {
                return; // Validation failed
            }

            try
            {
                // Prepare user object
                _user ??= new clsUser();
                _user.PersonID  = _person.ID;
                _user.UserName  = txtUserName.Text.Trim();
                _user.IsActive  = chkIsActive.Checked;

                // Only set password if it's provided (required in Add mode, optional in Edit mode)
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    _user.Password = txtPassword.Text;   // hashing should happen in Business layer
                }
                // Save the user
                if (_user.Save())
                {
                    lblUserID.Text = _user.UserID.ToString();
                    clsUtil.ShowSuccess("User saved successfully!", "Saved");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    clsUtil.ShowError("Failed to save user. Please check the data and try again.", "Save Failed");
                }
            }
            catch (Exception ex)
            {
                clsUtil.ShowError($"An error occurred while saving: {ex.Message}", "Save Error");
            }
        }


    }
}