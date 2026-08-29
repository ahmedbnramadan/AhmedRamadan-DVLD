using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class clsScheduleTest : UserControl
    {
        #region Controls Declaration

        private GroupBox gbScheduleTest;
        private Label lblTitle;

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
        private CheckBox chkUseTestAppointmentID;
        private Label lblTestAppointmentIDTitle;
        private Label lblTestAppointmentID;

        // Save Button
        private Button btnSave;

        // Error Provider
        private ErrorProvider errorProvider1;

        #endregion

        #region Fields

        private int _LocalDrivingLicenseApplicationID = -1;
        private clsLocalDrivingLicenseApplication _Application;

        private clsTestType.enTestType _TestType;
        private int _TestTypeID;
        private clsTestType _TestTypeInfo;

        private decimal _ApplicationFees = 0;
        private decimal _TestFees = 0;
        private int _CreatedByUserID = 1; // Should be set from login session

        #endregion

        #region Properties

        public int LocalDrivingLicenseApplicationID
        {
            get { return _LocalDrivingLicenseApplicationID; }
            set
            {
                _LocalDrivingLicenseApplicationID = value;
                LoadApplicationInfo(value);
            }
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

                    switch (_TestTypeID)
                    {
                        case 1:
                            _TestType = clsTestType.enTestType.Vision;
                            break;
                        case 2:
                            _TestType = clsTestType.enTestType.Written;
                            break;
                        case 3:
                            _TestType = clsTestType.enTestType.Practical;
                            break;
                        default:
                            _TestType = clsTestType.enTestType.Vision;
                            break;
                    }

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

        #endregion

        #region Events

        public event EventHandler OnSaveClicked;
        public event EventHandler OnTestTypeChanged;

        #endregion

        #region Constructor

        public clsScheduleTest()
        {
            InitializeComponent();
            SetupEvents();
        }

        #endregion

        #region Initialize Components

        private void InitializeComponent()
        {
            // UserControl - taller than other controls (600 vs 300)
            this.Size = new Size(900, 600);
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
            this.Dock = DockStyle.Fill;

            // Main GroupBox
            gbScheduleTest = new GroupBox
            {
                Text = "Schedule Test",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                Padding = new Padding(20)
            };

            // Title Label (Upper Right Corner)
            lblTitle = new Label
            {
                Text = "Schedule Test - Vision Test",
                Location = new Point(600, 25),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Italic),
                ForeColor = Color.SteelBlue,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Test Type Selection
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
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft Sans Serif", 9F)
            };
            cbTestType.Items.Add("Vision Test");
            cbTestType.Items.Add("Written Test");
            cbTestType.Items.Add("Practical Test");
            cbTestType.SelectedIndex = 0;

            // DL App ID
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

            // License Class
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

            // Applicant Name
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

            // Trial Number
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

            // Appointment Date
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

            // Fees
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

            // Payment GroupBox
            gbPayment = new GroupBox
            {
                Text = "Payment Details",
                Location = new Point(450, 80),
                Size = new Size(400, 280),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold)
            };

            // Application Fees Checkbox
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
                Font = new Font(new FontFamily("Microsoft Sans Serif"), 9F, FontStyle.Regular)
            };

            lblApplicationFees = new Label
            {
                Text = "0.00",
                Location = new Point(180, 70),
                AutoSize = true,
                Font = new Font(new FontFamily("Microsoft Sans Serif"), 9F, FontStyle.Bold),
                ForeColor = Color.Green
            };

            // Total Fees
            lblTotalFeesTitle = new Label
            {
                Text = "Total Fees:",
                Location = new Point(40, 110),
                AutoSize = true,
                Font = new Font(new FontFamily("Microsoft Sans Serif"), 10F, FontStyle.Bold)
            };

            lblTotalFees = new Label
            {
                Text = "0.00",
                Location = new Point(180, 110),
                AutoSize = true,
                Font = new Font(new FontFamily("Microsoft Sans Serif"), 11F, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };

            // Test Appointment ID Checkbox
            chkUseTestAppointmentID = new CheckBox
            {
                Text = "Use Existing Test Appointment ID",
                Location = new Point(20, 155),
                AutoSize = true
            };

            lblTestAppointmentIDTitle = new Label
            {
                Text = "Test Appointment ID:",
                Location = new Point(40, 195),
                AutoSize = true,
                Font = new Font(new FontFamily("Microsoft Sans Serif"), 9F, FontStyle.Regular)
            };

            lblTestAppointmentID = new Label
            {
                Text = "[Not Set]",
                Location = new Point(220, 195),
                AutoSize = true,
                Font = new Font(new FontFamily("Microsoft Sans Serif"), 9F, FontStyle.Bold),
                ForeColor = Color.Gray
            };

            // Save Button
            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(140, 530),
                Size = new Size(150, 40),
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom
            };
            btnSave.FlatAppearance.BorderSize = 0;

            // Error Provider
            errorProvider1 = new ErrorProvider();

            // Add controls to Payment GroupBox
            gbPayment.Controls.AddRange(new Control[]
            {
                chkPayApplicationFees,
                lblApplicationFeesTitle,
                lblApplicationFees,
                lblTotalFeesTitle,
                lblTotalFees,
                chkUseTestAppointmentID,
                lblTestAppointmentIDTitle,
                lblTestAppointmentID
            });

            // Add all controls to Main GroupBox
            gbScheduleTest.Controls.AddRange(new Control[]
            {
                lblTitle,
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
                btnSave
            });

            this.Controls.Add(gbScheduleTest);
        }

        private void SetupEvents()
        {
            cbTestType.SelectedIndexChanged += CbTestType_SelectedIndexChanged;
            chkPayApplicationFees.CheckedChanged += ChkPayApplicationFees_CheckedChanged;
            chkUseTestAppointmentID.CheckedChanged += ChkUseTestAppointmentID_CheckedChanged;
            btnSave.Click += BtnSave_Click;
            dtpAppointmentDate.ValueChanged += DtpAppointmentDate_ValueChanged;
        }

        #endregion

        #region Event Handlers

        private void CbTestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TestTypeID = cbTestType.SelectedIndex + 1;
            OnTestTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ChkPayApplicationFees_CheckedChanged(object sender, EventArgs e)
        {
            CalculateTotalFees();
        }

        private void ChkUseTestAppointmentID_CheckedChanged(object sender, EventArgs e)
        {
            lblTestAppointmentID.Enabled = chkUseTestAppointmentID.Checked;
        }

        private void DtpAppointmentDate_ValueChanged(object sender, EventArgs e)
        {
            // Validate date is not in the past
            if (dtpAppointmentDate.Value.Date < DateTime.Now.Date)
            {
                errorProvider1.SetError(dtpAppointmentDate, "Appointment date cannot be in the past");
            }
            else
            {
                errorProvider1.SetError(dtpAppointmentDate, "");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                if (SaveTestAppointment())
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

        #region Private Methods

        private void _UpdateTestTypeSpecifics()
        {
            // Update title based on test type
            string testTypeName = _TestType.ToString();
            lblTitle.Text = $"Schedule Test - {testTypeName} Test";

            // Update combo box selection
            cbTestType.SelectedIndex = _TestTypeID - 1;

            // Load test fees
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

            CalculateTotalFees();
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

            lblTotalFees.Text = total.ToString("0.00");
        }

        private bool ValidateInputs()
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

            return isValid;
        }

        private bool SaveTestAppointment()
        {
            try
            {
                // Check if appointment already exists for this test type
                DataTable existingAppointments = clsTestAppointment.GetApplicationAppointmentsPerTestType(
                    _LocalDrivingLicenseApplicationID,
                    _TestTypeID);

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

                // Create new test appointment
                clsTestAppointment appointment = new clsTestAppointment
                {
                    TestTypeID = _TestTypeID,
                    LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationID,
                    AppointmentDate = dtpAppointmentDate.Value,
                    PaidFees = decimal.Parse(lblTotalFees.Text),
                    CreatedByUserID = _CreatedByUserID,
                    IsLocked = false,
                    Mode = clsTestAppointment.enMode.AddNew
                };

                if (chkUseTestAppointmentID.Checked && !string.IsNullOrEmpty(lblTestAppointmentID.Text))
                {
                    if (int.TryParse(lblTestAppointmentID.Text, out int retakeAppID))
                    {
                        appointment.RetakeTestApplicationID = retakeAppID;
                    }
                }

                return appointment.Save();
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

        #endregion

        #region Public Methods

        public void LoadApplicationInfo(int applicationID)
        {
            _LocalDrivingLicenseApplicationID = applicationID;
            _Application = clsLocalDrivingLicenseApplication.Find(applicationID);

            if (_Application == null)
            {
                ResetControls();
                MessageBox.Show(
                    $"No application found with ID = {applicationID}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            FillApplicationData();
            _UpdateTestTypeSpecifics();
        }

        public void LoadApplicationInfo(clsLocalDrivingLicenseApplication application)
        {
            if (application == null)
            {
                ResetControls();
                return;
            }

            _Application = application;
            _LocalDrivingLicenseApplicationID = application.LocalDrivingLicenseApplicationID;

            FillApplicationData();
            _UpdateTestTypeSpecifics();
        }

        public void ResetControls()
        {
            _LocalDrivingLicenseApplicationID = -1;
            _Application = null;
            _TestTypeID = 1;
            _TestType = clsTestType.enTestType.Vision;
            _TestFees = 0;
            _ApplicationFees = 0;

            lblDLAppID.Text = "[???]";
            lblLicenseClass.Text = "[???]";
            lblName.Text = "[???]";
            lblTrial.Text = "[???]";
            lblFees.Text = "[???]";
            lblApplicationFees.Text = "0.00";
            lblTotalFees.Text = "0.00";
            lblTestAppointmentID.Text = "[Not Set]";

            dtpAppointmentDate.Value = DateTime.Now.AddDays(1);
            cbTestType.SelectedIndex = 0;
            chkPayApplicationFees.Checked = true;
            chkUseTestAppointmentID.Checked = false;

            errorProvider1.Clear();
        }

        public void SetCurrentUser(int userID)
        {
            _CreatedByUserID = userID;
        }

        #endregion

        #region Helper Methods

        private void FillApplicationData()
        {
            if (_Application == null) return;

            lblDLAppID.Text = _Application.LocalDrivingLicenseApplicationID.ToString();

            // License Class
            if (_Application.LicenseClassInfo != null)
            {
                lblLicenseClass.Text = $"{_Application.LicenseClassInfo.ClassName} ({_Application.LicenseClassInfo.ClassID})";
            }
            else
            {
                lblLicenseClass.Text = "[Unknown]";
            }

            // Applicant Name
            if (_Application.PersonInfo != null)
            {
                lblName.Text = _Application.PersonInfo.FullName;
            }
            else
            {
                lblName.Text = "[Unknown]";
            }

            // Trial Number
            lblTrial.Text = _Application.TrialNumber.ToString();

            // Application Fees
            if (_Application.ApplicationTypeInfo != null)
            {
                _ApplicationFees = _Application.ApplicationTypeInfo.Fees;
            }
        }

        #endregion
    }
}
