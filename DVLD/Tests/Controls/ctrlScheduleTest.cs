using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD.Tests.Controls
{
    public class ctrlScheduleTest : UserControl
    {
        #region Enums

        /// <summary>
        /// Whether this control is about to insert a brand-new appointment
        /// or update one that is already on file for this application.
        /// This drives clsTestAppointment's own enMode when Save() is called.
        /// </summary>
        public enum enMode
        {
            AddNew = 0,
            Update = 1
        }

        /// <summary>
        /// Whether the applicant is taking this test type for the first time,
        /// or retaking it after a previous failed attempt.
        /// </summary>
        public enum enCreationMode
        {
            FirstTime = 0,
            RetakeTest = 1
        }

        #endregion

        #region Controls Declaration

        private GroupBox gbScheduleTest;
        private Label lblTitle;
        private Label lblStatusMessage;

        // Test Type Selection
        private Label lblTestTypeTitle;
        private ComboBox cbTestType;

        // DL App ID
        private Label lblDLAppIDTitle;
        private Label lblDLAppID;

        // License Class
        private Label lblLicenseClassTitle;
        private Label lblLicenseClass;

        // Applicant Name
        private Label lblNameTitle;
        private Label lblName;

        // Trial Number
        private Label lblTrialTitle;
        private Label lblTrial;

        // Appointment Date
        private Label lblDateTitle;
        private DateTimePicker dtpAppointmentDate;

        // Fees
        private Label lblFeesTitle;
        private Label lblFees;

        // Payment Section
        private GroupBox gbPayment;
        private CheckBox chkPayApplicationFees;
        private Label lblApplicationFeesTitle;
        private Label lblApplicationFees;
        private Label lblTotalFeesTitle;
        private Label lblTotalFees;

        // Retake Info Section (shown only when CreationMode = RetakeTest)
        private GroupBox gbRetakeTestInfo;
        private Label lblRetakeApplicationIDTitle;
        private Label lblRetakeApplicationID;
        private Label lblRetakeFeesTitle;
        private Label lblRetakeFees;

        // Save Button
        private Button btnSave;

        // Error Provider
        private ErrorProvider errorProvider1;

        #endregion

        #region Fields

        private int _LocalDrivingLicenseApplicationID = -1;
        private clsLocalDrivingLicenseApplication _Application;

        private clsTestType.enTestType _TestType = clsTestType.enTestType.Vision;
        private int _TestTypeID = 1;
        private clsTestType _TestTypeInfo;

        private decimal _ApplicationFees = 0;
        private decimal _TestFees = 0;
        private decimal _RetakeTestFees = 0;
        private int _CreatedByUserID = 1; // Should be set from login session

        private enMode _Mode = enMode.AddNew;
        private enCreationMode _CreationMode = enCreationMode.FirstTime;
        private clsTestAppointment _ExistingAppointment;
        private bool _IsLocked = false;
        private int _TestAppointmentID = -1;
        private bool _CanScheduleCurrentTest = true;

        // Suppresses handler side-effects while we are populating controls programmatically,
        // so we don't re-run the same logic twice or fight our own initial values.
        private bool _isPopulatingControls = false;

        #endregion

        #region Properties

        public int LocalDrivingLicenseApplicationID
        {
            get { return _LocalDrivingLicenseApplicationID; }
            set { _InitializeControl(value); }
        }

        public clsTestType.enTestType TestType
        {
            get { return _TestType; }
            set
            {
                if (_TestType != value)
                {
                    _TestType = value;
                    _TestTypeID = (int)value;
                    _UpdateTestTypeSpecifics();
                }
            }
        }

        public int TestTypeID
        {
            get { return _TestTypeID; }
            set
            {
                if (_TestTypeID != value)
                {
                    _TestTypeID = value;
                    _TestType = (clsTestType.enTestType)value;
                    _UpdateTestTypeSpecifics();
                }
            }
        }

        public int CreatedByUserID
        {
            get { return _CreatedByUserID; }
            set { _CreatedByUserID = value; }
        }

        public bool IsValid
        {
            get { return ValidateChildren(ValidationConstraints.Enabled); }
        }

        /// <summary>Whether we are about to insert a new appointment or update an existing one.</summary>
        public enMode Mode
        {
            get { return _Mode; }
        }

        /// <summary>Whether the applicant is taking this test for the first time or retaking it.</summary>
        public enCreationMode CreationMode
        {
            get { return _CreationMode; }
        }

        public bool IsLocked
        {
            get { return _IsLocked; }
        }

        public int TestAppointmentID
        {
            get { return _TestAppointmentID; }
        }

        #endregion

        #region Events

        public event EventHandler OnSaveClicked;
        public event EventHandler OnTestTypeChanged;

        #endregion

        #region Constructor

        public ctrlScheduleTest()
        {
            InitializeComponent();
            SetupEvents();
            ResetControls();
        }

        #endregion

        #region Initialize Components

        private void InitializeComponent()
        {
            this.Size = new Size(950, 650);
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
            this.Dock = DockStyle.Fill;

            gbScheduleTest = new GroupBox
            {
                Text = "Schedule Test",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                Padding = new Padding(20)
            };

            lblTitle = new Label
            {
                Text = "Schedule Test - Vision Test",
                Location = new Point(600, 25),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Italic),
                ForeColor = Color.SteelBlue,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Status message straight under the title
            lblStatusMessage = new Label
            {
                Text = string.Empty,
                Location = new Point(600, 50),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                ForeColor = Color.Red,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                MaximumSize = new Size(300, 0),
                Visible = false
            };

            lblTestTypeTitle = new Label
            {
                Text = "Test Type:",
                Location = new Point(30, 35),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            cbTestType = new ComboBox
            {
                Location = new Point(140, 32),
                Size = new Size(220, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft Sans Serif", 9F)
            };

            lblDLAppIDTitle = new Label
            {
                Text = "DL App ID:",
                Location = new Point(30, 80),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblDLAppID = new Label
            {
                Text = "[???]",
                Location = new Point(140, 80),
                AutoSize = true,
                ForeColor = Color.DarkBlue,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblLicenseClassTitle = new Label
            {
                Text = "License Class:",
                Location = new Point(30, 125),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblLicenseClass = new Label
            {
                Text = "[???]",
                Location = new Point(140, 125),
                AutoSize = true
            };

            lblNameTitle = new Label
            {
                Text = "Name:",
                Location = new Point(30, 170),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblName = new Label
            {
                Text = "[???]",
                Location = new Point(140, 170),
                AutoSize = true,
                ForeColor = Color.DarkGreen
            };

            lblTrialTitle = new Label
            {
                Text = "Trial:",
                Location = new Point(30, 215),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblTrial = new Label
            {
                Text = "[???]",
                Location = new Point(140, 215),
                AutoSize = true,
                ForeColor = Color.Red
            };

            lblDateTitle = new Label
            {
                Text = "Date:",
                Location = new Point(30, 260),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            dtpAppointmentDate = new DateTimePicker
            {
                Location = new Point(140, 255),
                Size = new Size(250, 25),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Now.AddDays(1),
                Font = new Font("Microsoft Sans Serif", 9F)
            };

            lblFeesTitle = new Label
            {
                Text = "Test Fees:",
                Location = new Point(30, 305),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblFees = new Label
            {
                Text = "[???]",
                Location = new Point(140, 305),
                AutoSize = true,
                ForeColor = Color.Brown
            };

            gbPayment = new GroupBox
            {
                Text = "Payment Details",
                Location = new Point(450, 80),
                Size = new Size(420, 220),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold)
            };

            chkPayApplicationFees = new CheckBox
            {
                Text = "Pay Application Fees",
                Location = new Point(20, 35),
                AutoSize = true,
                Checked = true
            };

            lblApplicationFeesTitle = new Label
            {
                Text = "Application Fees:",
                Location = new Point(40, 70),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular)
            };

            lblApplicationFees = new Label
            {
                Text = "0.00",
                Location = new Point(180, 70),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                ForeColor = Color.Green
            };

            lblTotalFeesTitle = new Label
            {
                Text = "Total Fees:",
                Location = new Point(40, 110),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold)
            };

            lblTotalFees = new Label
            {
                Text = "0.00",
                Location = new Point(180, 110),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };

            // Retake Info GroupBox - only visible when CreationMode = RetakeTest
            gbRetakeTestInfo = new GroupBox
            {
                Text = "Retake Test Information",
                Location = new Point(450, 320),
                Size = new Size(420, 130),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Visible = false
            };

            lblRetakeApplicationIDTitle = new Label
            {
                Text = "Retake Base Application ID:",
                Location = new Point(20, 35),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular)
            };

            lblRetakeApplicationID = new Label
            {
                Text = "N/A",
                Location = new Point(230, 35),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                ForeColor = Color.Gray
            };

            lblRetakeFeesTitle = new Label
            {
                Text = "Retake Fees:",
                Location = new Point(20, 70),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular)
            };

            lblRetakeFees = new Label
            {
                Text = "0.00",
                Location = new Point(230, 70),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                ForeColor = Color.Brown
            };

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(140, 560),
                Size = new Size(150, 40),
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom
            };
            btnSave.FlatAppearance.BorderSize = 0;

            errorProvider1 = new ErrorProvider();

            gbRetakeTestInfo.Controls.AddRange(new Control[]
            {
                lblRetakeApplicationIDTitle,
                lblRetakeApplicationID,
                lblRetakeFeesTitle,
                lblRetakeFees
            });

            gbPayment.Controls.AddRange(new Control[]
            {
                chkPayApplicationFees,
                lblApplicationFeesTitle,
                lblApplicationFees,
                lblTotalFeesTitle,
                lblTotalFees
            });

            gbScheduleTest.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblStatusMessage,
                lblTestTypeTitle,
                cbTestType,
                lblDLAppIDTitle,
                lblDLAppID,
                lblLicenseClassTitle,
                lblLicenseClass,
                lblNameTitle,
                lblName,
                lblTrialTitle,
                lblTrial,
                lblDateTitle,
                dtpAppointmentDate,
                lblFeesTitle,
                lblFees,
                gbPayment,
                gbRetakeTestInfo,
                btnSave
            });

            this.Controls.Add(gbScheduleTest);
        }

        private void SetupEvents()
        {
            cbTestType.SelectedIndexChanged      += CbTestType_SelectedIndexChanged;
            chkPayApplicationFees.CheckedChanged += ChkPayApplicationFees_CheckedChanged;
            btnSave.Click                        += BtnSave_Click;
            dtpAppointmentDate.ValueChanged       += DtpAppointmentDate_ValueChanged;
        }

        #endregion

        #region Event Handlers

        private void CbTestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isPopulatingControls) return;

            // The dropdown is only ever enabled in AddNew mode (see _SetControlsReadOnly),
            // so we don't need to re-check Add-vs-Update here - just refresh fees/validation.
            TestTypeID = cbTestType.SelectedIndex + 1; // setter runs _UpdateTestTypeSpecifics
            _ValidateTestSequence();
            _RefreshControlsEnabledState();
            OnTestTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ChkPayApplicationFees_CheckedChanged(object sender, EventArgs e)
        {
            if (_isPopulatingControls) return;
            CalculateTotalFees();
        }

        private void DtpAppointmentDate_ValueChanged(object sender, EventArgs e)
        {
            if (_isPopulatingControls) return;
            _ValidateAppointmentDate();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_IsLocked)
            {
                MessageBox.Show(
                    "Cannot save: Application is locked.",
                    "Locked",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_ValidateInputs())
            {
                if (_SaveTestAppointment())
                {
                    MessageBox.Show(
                        "Test appointment scheduled successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    OnSaveClicked?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show(
                        "Failed to schedule test appointment.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Private Methods - Load / Initialize

        private void _InitializeControl(int ldlaID)
        {
            _isPopulatingControls = true;
            try
            {
                if (ldlaID <= 0)
                {
                    ResetControls();
                    return;
                }

                _Application = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppID(ldlaID);
                if (_Application == null)
                {
                    ResetControls();
                    _ShowStatusMessage($"No application found with ID = {ldlaID}", isError: true);
                    MessageBox.Show(
                        $"No application found with ID = {ldlaID}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                _LocalDrivingLicenseApplicationID = ldlaID;

                int activeAppointmentID = clsLocalDrivingLicenseApplication.GetActiveTestAppointmentID(ldlaID);

                if (activeAppointmentID == -1)
                {
                    // Nothing active at all - we're adding a new appointment.
                    // Default the selection to the first test type not yet passed.
                    _Mode = enMode.AddNew;
                    _ExistingAppointment = null;
                    _TestAppointmentID = -1;
                    _IsLocked = false;

                    _TestTypeID = _DetermineDefaultTestTypeID();
                    _TestType = (clsTestType.enTestType)_TestTypeID;

                    dtpAppointmentDate.MinDate = DateTime.Now.AddDays(1);
                    dtpAppointmentDate.Value = DateTime.Now.AddDays(1);
                }
                else
                {
                    // There's exactly one active appointment for this application -
                    // load it, and the test type comes from the record itself, not a guess.
                    _Mode = enMode.Update;
                    _ExistingAppointment = clsTestAppointment.Find(activeAppointmentID);
                    _TestAppointmentID = activeAppointmentID;

                    if (_ExistingAppointment != null)
                    {
                        _TestTypeID = (int)_ExistingAppointment.TestTypeID;
                        _TestType = _ExistingAppointment.TestTypeID;
                        _IsLocked = _ExistingAppointment.IsLocked;

                        DateTime lowestAllowedDate = _ExistingAppointment.AppointmentDate < DateTime.Now
                            ? DateTime.Now
                            : _ExistingAppointment.AppointmentDate;

                        dtpAppointmentDate.MinDate = lowestAllowedDate;
                        dtpAppointmentDate.Value = _ExistingAppointment.AppointmentDate;
                    }
                }

                _FillApplicationData();
                _UpdateTestTypeSpecifics();
                EnableAvailableTestsOnly();
            }
            finally
            {
                _isPopulatingControls = false;
            }

            _ValidateTestSequence();
            _SetControlsReadOnly(_IsLocked);
        }

        /// <summary>
        /// Picks the first test type the applicant hasn't passed yet
        /// (Vision, then Written, then Practical) as the initial selection
        /// when adding a brand-new appointment.
        /// </summary>
        private int _DetermineDefaultTestTypeID()
        {
            if (!clsTest.IsPassed(_LocalDrivingLicenseApplicationID, (int)clsTestType.enTestType.Vision))
            {
                return (int)clsTestType.enTestType.Vision;
            }

            if (!clsTest.IsPassed(_LocalDrivingLicenseApplicationID, (int)clsTestType.enTestType.Written))
            {
                return (int)clsTestType.enTestType.Written;
            }

            return (int)clsTestType.enTestType.Practical;
        }

        #endregion

        #region Private Methods - Business Logic

        /// <summary>
        /// A test type is a retake if the applicant already has at least one prior
        /// trial on record for it; otherwise it's a first-time attempt.
        /// </summary>
        private void _DetermineCreationMode()
        {
            if (_LocalDrivingLicenseApplicationID <= 0)
            {
                _CreationMode = enCreationMode.FirstTime;
                return;
            }

            int previousTrials = clsLocalDrivingLicenseApplication.TotalTrialsPerTest(
                _LocalDrivingLicenseApplicationID, _TestTypeID);

            _CreationMode = previousTrials > 0 ? enCreationMode.RetakeTest : enCreationMode.FirstTime;
        }

        private void _UpdateTestTypeSpecifics()
        {
            lblTitle.Text = $"Schedule Test - {_TestType} Test";

            _DetermineCreationMode();

            _TestTypeInfo = clsTestType.Find(_TestTypeID);
            if (_TestTypeInfo != null)
            {
                _TestFees = _TestTypeInfo.Fees;
                lblFees.Text = _TestFees.ToString("0.00");
            }
            else
            {
                _TestFees = 0;
                lblFees.Text = "[???]";
            }

            if (_LocalDrivingLicenseApplicationID > 0)
            {
                int trialNumber = clsLocalDrivingLicenseApplication.TotalTrialsPerTest(
                    _LocalDrivingLicenseApplicationID, _TestTypeID);
                lblTrial.Text = trialNumber.ToString();
            }

            _UpdateRetakeInfo();
            CalculateTotalFees();
        }

        private void _UpdateRetakeInfo()
        {
            if (_CreationMode == enCreationMode.RetakeTest && _Application != null)
            {
                _RetakeTestFees = _Application.ApplicationTypeInfo?.Fees ?? 0;
                lblRetakeFees.Text = _RetakeTestFees.ToString("0.00");

                lblRetakeApplicationID.Text =
                    (_Mode == enMode.Update && _ExistingAppointment?.RetakeTestApplicationID != null)
                        ? _ExistingAppointment.RetakeTestApplicationID.Value.ToString()
                        : "[Created on save]";

                gbRetakeTestInfo.Visible = true;
            }
            else
            {
                _RetakeTestFees = 0;
                lblRetakeFees.Text = "0.00";
                lblRetakeApplicationID.Text = "N/A";
                gbRetakeTestInfo.Visible = false;
            }
        }

        private void CalculateTotalFees()
        {
            decimal total = _TestFees;

            if (chkPayApplicationFees.Checked && _Application != null)
            {
                _ApplicationFees = _Application.ApplicationTypeInfo?.Fees ?? 0;
                lblApplicationFees.Text = _ApplicationFees.ToString("0.00");
                total += _ApplicationFees;
            }
            else
            {
                _ApplicationFees = 0;
                lblApplicationFees.Text = "0.00";
            }

            if (_CreationMode == enCreationMode.RetakeTest)
            {
                total += _RetakeTestFees;
            }

            lblTotalFees.Text = total.ToString("0.00");
        }

        /// <summary>
        /// Can't take test N without having passed every test before it,
        /// and can't re-schedule a test that has already been passed.
        /// </summary>
        private void _ValidateTestSequence()
        {
            bool canSchedule = true;
            string message = string.Empty;

            for (int testID = 1; testID < _TestTypeID; testID++)
            {
                if (!clsTest.IsPassed(_LocalDrivingLicenseApplicationID, testID))
                {
                    canSchedule = false;
                    string previousTestName = ((clsTestType.enTestType)testID).ToString();
                    message = $"Cannot schedule, {previousTestName} test should be passed first.";
                    break;
                }
            }

            if (canSchedule && clsTest.IsPassed(_LocalDrivingLicenseApplicationID, _TestTypeID))
            {
                canSchedule = false;
                message = $"Cannot schedule, {_TestType} test has already been passed.";
            }

            _CanScheduleCurrentTest = canSchedule;
            _ShowStatusMessage(message, isError: !canSchedule);
        }

        /// <summary>
        /// A test type counts as a duplicate only if it isn't the very appointment
        /// we are currently editing - otherwise updating a record would always flag
        /// itself as a conflict.
        /// </summary>
        private bool _IsDuplicateAppointmentForTestType(int testTypeID)
        {
            if (_Mode == enMode.Update
                && _ExistingAppointment != null
                && testTypeID == (int)_ExistingAppointment.TestTypeID)
            {
                return false;
            }

            return clsLocalDrivingLicenseApplication.IsThereAnActiveScheduledTest(
                _LocalDrivingLicenseApplicationID, testTypeID);
        }

        private void _ShowStatusMessage(string message, bool isError)
        {
            lblStatusMessage.Text = message;
            lblStatusMessage.ForeColor = isError ? Color.Red : Color.Green;
            lblStatusMessage.Visible = !string.IsNullOrEmpty(message);
        }

        /// <summary>
        /// Single place that decides what the user is allowed to touch.
        /// A locked application always wins over everything else.
        /// </summary>
        private void _RefreshControlsEnabledState()
        {
            bool canEdit = !_IsLocked;
            bool canSave = canEdit && _CanScheduleCurrentTest;

            dtpAppointmentDate.Enabled = canEdit;
            chkPayApplicationFees.Enabled = canEdit;
            btnSave.Enabled = canSave;

            if (_IsLocked)
            {
                _ShowStatusMessage("Application is locked. View only.", isError: true);
            }
        }

        private void _SetControlsReadOnly(bool isLocked)
        {
            _IsLocked = isLocked;

            // You can only pick a test type while adding a brand-new appointment -
            // once one is active (Update mode), the test type comes from that record.
            cbTestType.Enabled = (_Mode == enMode.AddNew) && !isLocked;

            _RefreshControlsEnabledState();
        }

        private void _ValidateAppointmentDate()
        {
            if (dtpAppointmentDate.Value.Date < DateTime.Now.Date)
            {
                errorProvider1.SetError(dtpAppointmentDate, "Appointment date cannot be in the past");
            }
            else
            {
                errorProvider1.SetError(dtpAppointmentDate, string.Empty);
            }
        }

        private bool _ValidateInputs()
        {
            bool isValid = true;
            errorProvider1.Clear();

            if (_LocalDrivingLicenseApplicationID <= 0)
            {
                errorProvider1.SetError(lblDLAppID, "Invalid Application ID");
                isValid = false;
            }

            if (_TestTypeID <= 0)
            {
                errorProvider1.SetError(cbTestType, "Please select a test type");
                isValid = false;
            }

            if (dtpAppointmentDate.Value.Date < DateTime.Now.Date)
            {
                errorProvider1.SetError(dtpAppointmentDate, "Appointment date cannot be in the past");
                isValid = false;
            }

            if (_TestFees <= 0)
            {
                errorProvider1.SetError(lblFees, "Invalid test fees");
                isValid = false;
            }

            if (!_CanScheduleCurrentTest)
            {
                errorProvider1.SetError(cbTestType, "This test cannot be scheduled yet.");
                isValid = false;
            }

            if (_IsDuplicateAppointmentForTestType(_TestTypeID))
            {
                errorProvider1.SetError(cbTestType, "An active appointment already exists for this test type.");
                isValid = false;
            }

            return isValid;
        }

        private bool _SaveTestAppointment()
        {
            try
            {
                if (_Mode == enMode.AddNew)
                {
                    DataTable existingAppointments = clsTestAppointment.GetApplicationAppointmentsPerTestType(
                        _LocalDrivingLicenseApplicationID, _TestTypeID);

                    if (existingAppointments != null && existingAppointments.Rows.Count > 0)
                    {
                        DialogResult result = MessageBox.Show(
                            "An appointment already exists for this test type. Do you want to create a new one?",
                            "Confirmation",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                        {
                            return false;
                        }
                    }
                }

                clsTestAppointment appointment = (_Mode == enMode.Update && _ExistingAppointment != null)
                    ? _ExistingAppointment
                    : new clsTestAppointment();

                appointment.Mode = (_Mode == enMode.Update)
                    ? clsTestAppointment.enMode.Update
                    : clsTestAppointment.enMode.AddNew;

                appointment.TestTypeID = (clsTestType.enTestType)_TestTypeID;
                appointment.LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationID;
                appointment.AppointmentDate = dtpAppointmentDate.Value;
                appointment.PaidFees = decimal.Parse(lblTotalFees.Text);
                appointment.CreatedByUserID = _CreatedByUserID;
                appointment.IsLocked = false;

                if (_CreationMode == enCreationMode.RetakeTest)
                {
                    appointment.RetakeTestApplicationID = _EnsureRetakeBaseApplicationID();
                }
                else
                {
                    appointment.RetakeTestApplicationID = null;
                }

                bool saved = appointment.Save();

                if (saved && _Mode == enMode.AddNew)
                {
                    _TestAppointmentID = appointment.TestAppointmentID;
                    _ExistingAppointment = appointment;
                    _Mode = enMode.Update;
                }

                return saved;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving test appointment: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// A retake should not duplicate the whole LDLA - it just needs its own
        /// lightweight base application record, linked via RetakeTestApplicationID.
        /// If one is already linked (editing an existing retake), we reuse it.
        /// </summary>
        private int? _EnsureRetakeBaseApplicationID()
        {
            if (_Mode == enMode.Update && _ExistingAppointment?.RetakeTestApplicationID != null)
            {
                return _ExistingAppointment.RetakeTestApplicationID;
            }

            // I don't have clsApplication's real constructor/Save API in front of me,
            // so I'm not going to invent it. See my note in chat about this.
            throw new NotImplementedException(
                "Creating a new base application for a retake needs clsApplication's actual members - please confirm them.");
        }

        #endregion

        #region Public Methods

        public void LoadApplicationInfo(int applicationID)
        {
            LocalDrivingLicenseApplicationID = applicationID;
        }

        public void LoadApplicationInfo(clsLocalDrivingLicenseApplication application)
        {
            if (application == null)
            {
                ResetControls();
                return;
            }

            LocalDrivingLicenseApplicationID = application.LocalDrivingLicenseApplicationID;
        }

        public void ResetControls()
        {
            _isPopulatingControls = true;

            _LocalDrivingLicenseApplicationID = -1;
            _Application = null;
            _TestTypeID = 1;
            _TestType = clsTestType.enTestType.Vision;
            _TestFees = 0;
            _ApplicationFees = 0;
            _RetakeTestFees = 0;
            _Mode = enMode.AddNew;
            _CreationMode = enCreationMode.FirstTime;
            _ExistingAppointment = null;
            _IsLocked = false;
            _TestAppointmentID = -1;
            _CanScheduleCurrentTest = true;

            lblDLAppID.Text = "[???]";
            lblLicenseClass.Text = "[???]";
            lblName.Text = "[???]";
            lblTrial.Text = "[???]";
            lblFees.Text = "[???]";
            lblApplicationFees.Text = "0.00";
            lblTotalFees.Text = "0.00";
            lblRetakeApplicationID.Text = "N/A";
            lblRetakeFees.Text = "0.00";
            gbRetakeTestInfo.Visible = false;

            dtpAppointmentDate.MinDate = DateTime.Now.AddDays(1);
            dtpAppointmentDate.Value = DateTime.Now.AddDays(1);

            cbTestType.Items.Clear();
            cbTestType.Items.Add("Vision Test");
            cbTestType.SelectedIndex = 0;

            chkPayApplicationFees.Checked = true;

            _ShowStatusMessage(string.Empty, isError: false);
            errorProvider1.Clear();

            _isPopulatingControls = false;

            cbTestType.Enabled = true; // fresh control, no LDLA loaded yet - always AddNew
            _RefreshControlsEnabledState();
        }

        public void SetCurrentUser(int userID)
        {
            _CreatedByUserID = userID;
        }

        /// <summary>
        /// Rebuilds the test type list so only tests the applicant is actually
        /// eligible for are shown (e.g. Written only appears once Vision is passed).
        /// </summary>
        public void EnableAvailableTestsOnly()
        {
            bool wasPopulating = _isPopulatingControls;
            _isPopulatingControls = true;

            cbTestType.Items.Clear();
            cbTestType.Items.Add("Vision Test");

            bool visionPassed = clsTest.IsPassed(_LocalDrivingLicenseApplicationID, (int)clsTestType.enTestType.Vision);
            if (visionPassed)
            {
                cbTestType.Items.Add("Written Test");
            }

            bool writtenPassed = visionPassed
                && clsTest.IsPassed(_LocalDrivingLicenseApplicationID, (int)clsTestType.enTestType.Written);
            if (writtenPassed)
            {
                cbTestType.Items.Add("Practical Test");
            }

            int indexToSelect = _TestTypeID - 1;
            if (indexToSelect < 0 || indexToSelect >= cbTestType.Items.Count)
            {
                indexToSelect = 0;
            }

            cbTestType.SelectedIndex = indexToSelect;

            _isPopulatingControls = wasPopulating;
        }

        #endregion

        #region Helper Methods

        private void _FillApplicationData()
        {
            if (_Application == null) return;

            lblDLAppID.Text = _Application.LocalDrivingLicenseApplicationID.ToString();

            lblLicenseClass.Text = _Application.LicenseClassInfo != null
                ? $"{_Application.LicenseClassInfo.Name} ({_Application.LicenseClassInfo.ID})"
                : "[Unknown]";

            lblName.Text = _Application.PersonInfo != null
                ? _Application.PersonInfo.FullName
                : "[Unknown]";

            if (_Application.ApplicationTypeInfo != null)
            {
                _ApplicationFees = _Application.ApplicationTypeInfo.Fees;
            }
        }

        #endregion
    }
}