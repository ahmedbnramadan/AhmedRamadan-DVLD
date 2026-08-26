using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmAddEditNewLocalDrivingLicenseApplication : Form
    {
        #region Controls Declaration

        private Label       lblTitle;

        // ── Tab Control ──────────────────────────────────────────────
        private TabControl  tcMain;
        private TabPage     tpPersonalInfo;
        private TabPage     tpAppInfo;

        private ctrlPersonCardWithFilter ctrlPersonCardWithFilter1;

        // ── Tab 2: Application Info ──────────────────────────────────
        private Label       lblAppIDTitle,     lblAppID;
        private Label       lblAppDateTitle;
        private DateTimePicker dtpApplicationDate;
        private Label       lblLicClassTitle;
        private ComboBox    cbLicClass;
        private Label       lblFeesTitle;
        private TextBox     txtFees;
        private Label       lblCreatedByTitle, lblCreatedBy;

        // ── Bottom buttons (outside tabs) ────────────────────────────
        private Button      btnNext;
        private Button      btnSave;
        private Button      btnClose;

        #endregion

        #region State

        private readonly int _appID;
        private clsLocalDrivingLicenseApplication _application;
        private clsPerson    _person;
        private enMode       _mode;

        private bool _isEditMode => _mode == enMode.Update;

        #endregion

        // ── Constructors ────────────────────────────────────────────────────

        /// <summary>Opens the form in Add-New mode.</summary>
        public frmAddEditNewLocalDrivingLicenseApplication()
        {
            _appID = -1;
            _mode  = enMode.AddNew;
            _InitializeComponents();
            _LoadLicenseClasses();
            _LoadData();
        }

        /// <summary>Opens the form in Update mode for the given application.</summary>
        public frmAddEditNewLocalDrivingLicenseApplication(int appID)
        {
            _appID = appID;
            _mode  = enMode.Update;
            _InitializeComponents();
            _LoadLicenseClasses();
            _LoadData();
        }

        // ── Form Build ──────────────────────────────────────────────────────

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text            = _isEditMode ? "Edit Application" : "New Local Driving License Application";
            this.Size            = new Size(990, 750);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;
            this.Font            = new Font("Microsoft Sans Serif", 9.5F);

            this.Shown += frmAddEditNewLocalDrivingLicenseApplication_Shown;

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text      = _isEditMode ? "Edit Application" : "New Driving License Application",
                Font      = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize  = true,
                Location  = new Point(300, 18)
            };

            // ── Tab Control ──────────────────────────────────────────
            tcMain = new TabControl
            {
                Location  = new Point(20, 65),
                Size      = new Size(940, 550),
                Font      = new Font("Microsoft Sans Serif", 10F)
            };

            tpPersonalInfo = new TabPage("  Personal Info  ");
            tpAppInfo      = new TabPage("  Application Info  ");

            tcMain.TabPages.Add(tpPersonalInfo);
            tcMain.TabPages.Add(tpAppInfo);
            tcMain.SelectedIndexChanged += tcMain_SelectedIndexChanged;
            tcMain.Selecting            += tcMain_Selecting;

            _BuildPersonalInfoTab();
            _BuildAppInfoTab();

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

            // Hide filter in update mode (when loading existing application)
            if (_isEditMode)
            {
                ctrlPersonCardWithFilter1.FilterVisible = false;
            }

            tpPersonalInfo.Controls.Add(ctrlPersonCardWithFilter1);
        }

        // ── Tab 2: Application Info ────────────────────────────────────────────

        private void _BuildAppInfoTab()
        {
            tpAppInfo.BackColor = Color.White;
            tpAppInfo.Padding   = new Padding(10);

            var pnlAppInfo = new Panel
            {
                Location    = new Point(10, 10),
                Size        = new Size(900, 480),
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            int y = 40; const int step = 55;
            const int lx = 130, fx = 295, fw = 260;

            // Application ID (read-only)
            lblAppIDTitle = _MakeBoldLabel("Application ID:", lx, y);
            lblAppID      = new Label
            {
                Text      = "???",
                Location  = new Point(fx, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                ForeColor = Color.SteelBlue
            };
            y += step;

            // Application Date
            lblAppDateTitle    = _MakeBoldLabel("Application Date:", lx, y);
            dtpApplicationDate = new DateTimePicker
            {
                Location = new Point(fx, y - 3),
                Size     = new Size(fw, 26),
                Font     = new Font("Microsoft Sans Serif", 10F),
                Format   = DateTimePickerFormat.Short
            };
            y += step;

            // License Class
            lblLicClassTitle = _MakeBoldLabel("License Class:", lx, y);
            cbLicClass        = new ComboBox
            {
                Location      = new Point(fx, y - 3),
                Size          = new Size(fw, 26),
                Font          = new Font("Microsoft Sans Serif", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor        = Cursors.Hand
            };
            cbLicClass.SelectedIndexChanged += cbLicClass_SelectedIndexChanged;
            y += step;

            // Application Fees (read-only, derived from License Class)
            lblFeesTitle = _MakeBoldLabel("Application Fees:", lx, y);
            txtFees      = _MakeInputBox(fx, y - 3, fw);
            txtFees.ReadOnly  = true;
            txtFees.BackColor = Color.FromArgb(245, 247, 252);
            y += step;

            // Created By (read-only)
            lblCreatedByTitle = _MakeBoldLabel("Created By:", lx, y);
            lblCreatedBy      = new Label
            {
                Text      = "???",
                Location  = new Point(fx, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                ForeColor = Color.SteelBlue
            };

            pnlAppInfo.Controls.AddRange(new Control[]
            {
                lblAppIDTitle,     lblAppID,
                lblAppDateTitle,   dtpApplicationDate,
                lblLicClassTitle,  cbLicClass,
                lblFeesTitle,      txtFees,
                lblCreatedByTitle, lblCreatedBy
            });

            tpAppInfo.Controls.Add(pnlAppInfo);
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

        private static TextBox _MakeInputBox(int x, int y, int width)
            => new TextBox
            {
                Location = new Point(x, y),
                Size     = new Size(width, 26),
                Font     = new Font("Microsoft Sans Serif", 10F)
            };

        // ── Data helpers ─────────────────────────────────────────────────────

        private void _LoadLicenseClasses()
        {
            cbLicClass.DataSource    = clsLicenseClass.GetAllLicenseClasses();
            cbLicClass.DisplayMember = "ClassName";
            cbLicClass.ValueMember   = "LicenseClassID";
        }

        private void _LoadData()
        {
            if (!_isEditMode)
            {
                // Add mode: initialize form for new application
                _InitializeForAddMode();
                return;
            }

            // Edit mode: load existing application data
            try
            {
                _application = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppID(_appID);

                if (_application == null)
                {
                    clsUtil.ShowError($"Application with ID {_appID} not found.", "Application Not Found");
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                _person = clsPerson.Find(_application.ApplicantPersonID);
                // Load person into the ctrlPersonCardWithFilter
                if (_person != null)
                {
                    ctrlPersonCardWithFilter1.LoadPersonInfo(_person.ID);
                }

                // Populate application info fields
                lblAppID.Text            = _application.LocalDrivingLicenseApplicationID.ToString();
                dtpApplicationDate.Value = _application.ApplicationDate;
                cbLicClass.SelectedValue = _application.LicenseClassID;
                txtFees.Text             = _application.PaidFees.ToString("F2");

                var creator = clsUser.Find(_application.CreatedByUserID);
                lblCreatedBy.Text = creator != null ? creator.UserName : _application.CreatedByUserID.ToString();

                // Disable person filter in edit mode since we're editing existing application
                ctrlPersonCardWithFilter1.FilterVisible = false;
            }
            catch (Exception ex)
            {
                clsUtil.ShowError($"Failed to load application data: {ex.Message}", "Load Error");
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void _InitializeForAddMode()
        {
            // Reset all fields for adding a new application
            lblAppID.Text            = "New";
            dtpApplicationDate.Value = DateTime.Now;
            if (cbLicClass.Items.Count > 0) cbLicClass.SelectedIndex = 0;
            txtFees.Clear();

            var creator = clsUser.Find(clsGlobal.CurrentUserID);
            lblCreatedBy.Text = creator != null ? creator.UserName : "Current User";

            // Ensure filter is visible in add mode
            ctrlPersonCardWithFilter1.FilterVisible = true;

        }

        private void CtrlPersonCardWithFilter1_PersonLoaded(object sender, clsPerson person)
        {
            _person = person;
        }

        // ── Validation ───────────────────────────────────────────────────────

        /// <summary>
        /// Validates that a person is selected.
        /// If validation fails, shows appropriate warning and resets to Personal Info tab.
        /// Returns true if validation failed, false if validation passed.
        /// </summary>
        private bool _ValidatePersonSelection()
        {
            if (_person == null || _person.ID == -1)
            {
                clsUtil.ShowWarning("Please find and select a person first.", "No Person Selected");
                tcMain.SelectedTab = tpPersonalInfo;
                ctrlPersonCardWithFilter1.FocusOnFilter();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates all application information fields before saving.
        /// Returns true if all validations pass, false otherwise.
        /// </summary>
        private bool _ValidateAppInfo()
        {
            // Validate license class
            if (cbLicClass.SelectedValue == null)
            {
                clsUtil.ShowWarning("Please select a license class.", "Validation Error");
                cbLicClass.BackColor = clsGlobal.InputError;
                cbLicClass.Focus();
                return false;
            }
            cbLicClass.BackColor = clsGlobal.InputValid;

            // Validate application date
            if (dtpApplicationDate.Value.Date > DateTime.Now.Date)
            {
                clsUtil.ShowWarning("Application date cannot be in the future.", "Validation Error");
                dtpApplicationDate.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// check if person already has an active application of that type of license of the same type
        /// Returns true if all validations pass, false otherwise with the wrong message
        /// </summary>
        private bool _ValidateApplicationRules()
        {
            if (!_isEditMode)
            {
                if (_application.DoesPersonHaveActiveApplication())
                {
                    clsUtil.ShowWarning(
                        "This person already has an active application for this license class.",
                        "Application Exists");

                    return false;
                }

                if (_application.DoesPersonHaveActiveLicense())
                {
                    clsUtil.ShowWarning(
                        "This person already has an active license for this license class.",
                        "License Exists");

                    return false;
                }
            }

            return true;
        }

        // ── Events ───────────────────────────────────────────────────────────

        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool onAppInfoTab = tcMain.SelectedTab == tpAppInfo;
            btnNext.Visible = !onAppInfoTab;
            btnSave.Visible =  onAppInfoTab;
        }

        private void frmAddEditNewLocalDrivingLicenseApplication_Shown(object sender, EventArgs e)
        {
            if (!_isEditMode)
            {
                ctrlPersonCardWithFilter1.FocusOnFilter();
            }
        }

        private void tcMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // Block jumping to Application Info (via tab header or code) until a person is selected
            if (e.TabPage == tpAppInfo && (_person == null || _person.ID == -1))
            {
                e.Cancel = true;
                clsUtil.ShowWarning("Please find and select a person first.", "No Person Selected");
                ctrlPersonCardWithFilter1.FocusOnFilter();
            }
        }

        private void cbLicClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLicClass.SelectedValue == null) return;
            var licenseClass = clsLicenseClass.Find(Convert.ToInt32(cbLicClass.SelectedValue));
            txtFees.Text = licenseClass != null ? licenseClass.Fees.ToString("F2") : "0.00";
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Validate person selection
            if (_ValidatePersonSelection())
            {
                return; // Validation failed, stay on current tab
            }

            // All validations passed, navigate to Application Info tab
            tcMain.SelectedTab = tpAppInfo;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Final validation before saving
            if (!_ValidateAppInfo())
            {
                return; // Validation failed
            }

            try
            {
                // Prepare application object
                _application ??= new clsLocalDrivingLicenseApplication();
                _application.ApplicantPersonID = _person.ID;
                _application.ApplicationTypeID = (int)clsApplicationType.enApplicationType.NewLocalDrivingLicense;
                _application.LicenseClassID    = Convert.ToInt32(cbLicClass.SelectedValue);
                _application.ApplicationDate   = dtpApplicationDate.Value;
                _application.PaidFees          = decimal.TryParse(txtFees.Text, out decimal fees) ? fees : 0;

                // Only set creator on first save
                if (!_isEditMode)
                {
                    _application.CreatedByUserID = clsGlobal.CurrentUserID;
                }

                if (!_ValidateApplicationRules())
                {
                    return;
                }

                // Save the application
                if (_application.Save())
                {
                    lblAppID.Text = _application.LocalDrivingLicenseApplicationID.ToString();
                    clsUtil.ShowSuccess("Application saved successfully!", "Saved");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    clsUtil.ShowError("Failed to save application. Please check the data and try again.", "Save Failed");
                }
            }
            catch (Exception ex)
            {
                clsUtil.ShowError($"An error occurred while saving: {ex.Message}", "Save Error");
            }
        }

    }
}