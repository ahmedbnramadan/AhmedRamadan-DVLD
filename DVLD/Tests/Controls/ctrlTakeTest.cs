using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD.Tests.Controls
{
    /// <summary>
    /// Records the result of a test appointment. Once a result has been
    /// saved (clsTest already exists for this appointment), the Pass/Fail
    /// choice becomes permanently read-only — matching the DB-level lock
    /// that clsTests.AddNewTest applies to the appointment. Notes remain
    /// editable indefinitely.
    /// </summary>
    public class ctrlTakeTest : UserControl
    {
        #region Controls Declaration

        private GroupBox gbTakeTest;
        private Label lblTitle;

        private Label lblDLAppIDTitle, lblDLAppID;
        private Label lblLicenseClassTitle, lblLicenseClass;
        private Label lblNameTitle, lblName;
        private Label lblTrialTitle, lblTrial;
        private Label lblDateTitle, lblDate;
        private Label lblFeesTitle, lblFees;
        private Label lblTestIDTitle, lblTestID;

        private GroupBox gbResult;
        private RadioButton rbPass, rbFail;
        private Label lblResultLockedMessage;

        private Label lblNotesTitle;
        private TextBox txtNotes;

        private Button btnSave;

        #endregion

        #region Fields

        private int _TestAppointmentID = -1;
        private clsTestAppointment _Appointment;
        private clsTestType.enTestType _TestType;
        private int _TestID = -1;
        private clsTest _Test;
        private int _CreatedByUserID = -1;

        #endregion

        #region Properties

        public int TestAppointmentID => _TestAppointmentID;
        public int TestID => _TestID;

        #endregion

        #region Events

        /// <summary>Raised after a successful save (either the first save,
        /// or a later notes-only update).</summary>
        public event EventHandler OnTestSaved;

        #endregion

        #region Constructor

        public ctrlTakeTest()
        {
            InitializeComponent();
            SetupEvents();
            ResetControls();
        }

        #endregion

        #region Initialize Components

        private void InitializeComponent()
        {
            this.Size = new Size(950, 470);
            this.Font = new Font("Microsoft Sans Serif", 10F);
            this.Dock = DockStyle.Fill;

            gbTakeTest = new GroupBox
            {
                Text = "Take Test",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                Padding = new Padding(20)
            };

            lblTitle = new Label
            {
                Text = "Take Test",
                Location = new Point(560, 25),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Italic),
                ForeColor = Color.SteelBlue
            };

            // ── Left column: read-only appointment facts ──────────────
            lblDLAppIDTitle = _Bold("DL App ID:", 30, 80);
            lblDLAppID      = _Value("[???]", 160, 80, Color.DarkBlue);

            lblLicenseClassTitle = _Bold("License Class:", 30, 120);
            lblLicenseClass      = _Value("[???]", 160, 120, Color.Black);

            lblNameTitle = _Bold("Name:", 30, 160);
            lblName      = _Value("[???]", 160, 160, Color.DarkGreen);

            lblTrialTitle = _Bold("Trial:", 30, 200);
            lblTrial      = _Value("[???]", 160, 200, Color.Red);

            lblDateTitle = _Bold("Date:", 30, 240);
            lblDate      = _Value("[???]", 160, 240, Color.Black);

            lblFeesTitle = _Bold("Fees:", 30, 280);
            lblFees      = _Value("[???]", 160, 280, Color.Brown);

            lblTestIDTitle = _Bold("Test ID:", 30, 320);
            lblTestID      = _Value("[???]", 160, 320, Color.SteelBlue);

            // ── Right side: Result ─────────────────────────────────────
            gbResult = new GroupBox
            {
                Text = "Result",
                Location = new Point(450, 80),
                Size = new Size(420, 150),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold)
            };

            rbPass = new RadioButton
            {
                Text = "Pass",
                Location = new Point(30, 35),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10F)
            };

            rbFail = new RadioButton
            {
                Text = "Fail",
                Location = new Point(30, 70),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10F)
            };

            lblResultLockedMessage = new Label
            {
                Text = "This result was already saved and can no longer be changed.",
                Location = new Point(20, 105),
                Size = new Size(380, 35),
                Font = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Italic),
                ForeColor = Color.Firebrick,
                Visible = false
            };

            gbResult.Controls.AddRange(new Control[] { rbPass, rbFail, lblResultLockedMessage });

            // ── Notes ───────────────────────────────────────────────────
            lblNotesTitle = _Bold("Notes:", 450, 250);

            txtNotes = new TextBox
            {
                Location = new Point(450, 275),
                Size = new Size(420, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };

            // ── Save ────────────────────────────────────────────────────
            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(400, 400),
                Size = new Size(150, 40),
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;

            gbTakeTest.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblDLAppIDTitle, lblDLAppID,
                lblLicenseClassTitle, lblLicenseClass,
                lblNameTitle, lblName,
                lblTrialTitle, lblTrial,
                lblDateTitle, lblDate,
                lblFeesTitle, lblFees,
                lblTestIDTitle, lblTestID,
                gbResult,
                lblNotesTitle, txtNotes,
                btnSave
            });

            this.Controls.Add(gbTakeTest);
        }

        private void SetupEvents()
        {
            btnSave.Click += BtnSave_Click;
        }

        private static Label _Bold(string text, int x, int y)
            => new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

        private static Label _Value(string text, int x, int y, Color color)
            => new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = color
            };

        #endregion

        #region Public Methods

        public void SetCurrentUser(int userID) => _CreatedByUserID = userID;

        /// <summary>
        /// Loads the appointment and any existing test result.
        /// <paramref name="testType"/> is accepted to match the caller's
        /// context, but the loaded appointment's own TestTypeID is treated
        /// as the source of truth for display — never trust a value the
        /// caller merely *thinks* is correct when the database can tell you.
        /// </summary>
        public void LoadTestAppointment(int testAppointmentID, clsTestType.enTestType testType)
        {
            if (testAppointmentID <= 0)
            {
                ResetControls();
                return;
            }

            _Appointment = clsTestAppointment.Find(testAppointmentID);

            if (_Appointment == null)
            {
                ResetControls();
                clsUtil.ShowError($"No test appointment found with ID = {testAppointmentID}.");
                return;
            }

            _TestAppointmentID = testAppointmentID;
            _TestType = _Appointment.TestTypeID;

            lblTitle.Text = $"{_TestType} Test";

            _FillAppointmentData();

            _Test = clsTest.FindByAppointmentID(testAppointmentID);

            if (_Test != null)
            {
                // Already taken - lock the result, keep notes editable.
                _TestID = _Test.TestID;
                lblTestID.Text = _TestID.ToString();

                rbPass.Checked = _Test.TestResult;
                rbFail.Checked = !_Test.TestResult;

                txtNotes.Text = _Test.Notes;

                _LockResultControls();
                btnSave.Enabled = true;
            }
            else if (_Appointment.IsLocked)
            {
                // Defensive: shouldn't happen (AddNewTest locks the
                // appointment in the same statement that creates the Test
                // row), but if the two ever get out of sync, don't let the
                // user create a second, conflicting result.
                lblTestID.Text = "[Unknown]";
                rbPass.Checked = false;
                rbFail.Checked = false;
                txtNotes.Text = string.Empty;

                _LockResultControls();
                btnSave.Enabled = false;

                clsUtil.ShowWarning(
                    "This appointment is marked locked but no result record was found. " +
                    "Please contact support before proceeding.");
            }
            else
            {
                // Not taken yet - free to choose.
                _TestID = -1;
                lblTestID.Text = "New";

                rbPass.Checked = false;
                rbFail.Checked = false;
                txtNotes.Text = string.Empty;

                _UnlockResultControls();
                btnSave.Enabled = true;
            }
        }

        public void ResetControls()
        {
            _TestAppointmentID = -1;
            _Appointment = null;
            _Test = null;
            _TestID = -1;

            lblTitle.Text = "Take Test";
            lblDLAppID.Text = "[???]";
            lblLicenseClass.Text = "[???]";
            lblName.Text = "[???]";
            lblTrial.Text = "[???]";
            lblDate.Text = "[???]";
            lblFees.Text = "[???]";
            lblTestID.Text = "[???]";

            rbPass.Checked = false;
            rbFail.Checked = false;
            txtNotes.Text = string.Empty;

            _UnlockResultControls();
            btnSave.Enabled = true;
        }

        #endregion

        #region Private Helpers

        private void _FillAppointmentData()
        {
            var app = _Appointment.LocalDrivingLicenseApplicationInfo;

            lblDLAppID.Text = _Appointment.LocalDrivingLicenseApplicationID.ToString();

            lblLicenseClass.Text = app?.LicenseClassInfo != null
                ? $"{app.LicenseClassInfo.Name} ({app.LicenseClassInfo.ID})"
                : "[Unknown]";

            lblName.Text = app?.PersonFullName ?? "[Unknown]";

            int trialNumber = clsLocalDrivingLicenseApplication.TotalTrialsPerTest(
                _Appointment.LocalDrivingLicenseApplicationID, (int)_TestType);
            lblTrial.Text = trialNumber.ToString();

            lblDate.Text = _Appointment.AppointmentDate.ToString("dd/MMM/yyyy");
            lblFees.Text = _Appointment.PaidFees.ToString("0.00");
        }

        private void _LockResultControls()
        {
            rbPass.Enabled = false;
            rbFail.Enabled = false;
            lblResultLockedMessage.Visible = true;
        }

        private void _UnlockResultControls()
        {
            rbPass.Enabled = true;
            rbFail.Enabled = true;
            lblResultLockedMessage.Visible = false;
        }

        private bool _ValidateInputs()
        {
            // A choice is only required the first time - once _Test exists,
            // the radios are disabled and already hold the saved value.
            if (_Test == null && !rbPass.Checked && !rbFail.Checked)
            {
                clsUtil.ShowWarning("Please select Pass or Fail before saving.");
                return false;
            }
            return true;
        }

        #endregion

        #region Event Handlers

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_TestAppointmentID <= 0)
            {
                clsUtil.ShowWarning("No test appointment loaded.");
                return;
            }

            if (!_ValidateInputs()) return;

            string confirmMessage = (_Test == null)
                ? "Are you sure you want to save this test result?\n" +
                  "Once saved, the Pass/Fail result cannot be changed."
                : "Are you sure you want to save the changes to the notes?";

            if (MessageBox.Show(confirmMessage, "Confirm Save",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            _Test ??= new clsTest
            {
                TestAppointmentID = _TestAppointmentID,
                CreatedByUserID = _CreatedByUserID
            };

            // On a first save this sets the permanent result. On every
            // later save this just re-writes the same value the radios
            // already hold (they're disabled, so it can't have changed) -
            // the actual immutability guarantee comes from clsTest.Mode:
            // Save() routes to _Update(), never _AddNew(), once TestID exists.
            _Test.TestResult = rbPass.Checked;
            _Test.Notes = txtNotes.Text.Trim();

            if (_Test.Save())
            {
                _TestID = _Test.TestID;
                lblTestID.Text = _TestID.ToString();

                _LockResultControls();

                clsUtil.ShowSuccess("Test result saved successfully.");

                OnTestSaved?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                clsUtil.ShowError("Failed to save the test result. Please try again.");
            }
        }

        #endregion
    }
}