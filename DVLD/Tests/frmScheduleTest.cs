using System;
using System.Drawing;
using System.Windows.Forms;
using Business;
using DVLD.Tests.Controls;

namespace DVLD.Tests
{
    /// <summary>
    /// Form for scheduling a test appointment for a local driving license application.
    /// The embedded ctrlScheduleTest control decides Add/Update mode on its own,
    /// based on whether an active appointment already exists for this application.
    /// </summary>
    public class frmScheduleTest : Form
    {
        #region Controls Declaration

        private Label lblTitle;
        private ctrlScheduleTest ctrlScheduleTest1;
        private Button btnClose;

        #endregion

        #region State

        private readonly int _LocalDrivingLicenseApplicationID;
        private readonly clsTestType.enTestType _TestType;

        // Not wired up yet - ctrlScheduleTest currently has no way to load a
        // specific (non-active) appointment by ID. Kept here so the caller's
        // intent is visible and this can be honored once the control supports it.
        private readonly int _AppointmentID;

        private bool _IsSaved = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new frmScheduleTest for scheduling a test.
        /// </summary>
        /// <param name="LocalDrivingLicenseApplicationID">The Local Driving License Application ID.</param>
        /// <param name="testType">The type of test to schedule.</param>
        /// <param name="AppointmentID">Reserved for future use. Currently ignored - see field comment.</param>
        public frmScheduleTest(int LocalDrivingLicenseApplicationID, clsTestType.enTestType testType, int AppointmentID = -1)
        {
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestType = testType;
            _AppointmentID = AppointmentID;

            _InitializeComponents();
            _SetupEvents();
        }

        #endregion

        #region Initialization

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text = "Schedule Test";
            this.Size = new Size(950, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 10F);

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text = "Schedule Test Appointment",
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            // ── Schedule Test Control ─────────────────────────────────
            ctrlScheduleTest1 = new ctrlScheduleTest
            {
                Location = new Point(30, 70),
                Size = new Size(890, 550),
                Dock = DockStyle.None
            };

            // ── Close button ─────────────────────────────────────────
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(385, 630),
                Size = new Size(180, 40),
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                BackColor = clsGlobal.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;

            // ── Add to form ───────────────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                ctrlScheduleTest1,
                btnClose
            });
        }

        private void _SetupEvents()
        {
            this.Load += frmScheduleTest_Load;
            btnClose.Click += btnClose_Click;
            ctrlScheduleTest1.OnSaveClicked += CtrlScheduleTest1_OnSaveClicked;
        }

        #endregion

        #region Event Handlers

        private void frmScheduleTest_Load(object sender, EventArgs e)
        {
            _LoadAppointmentData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CtrlScheduleTest1_OnSaveClicked(object sender, EventArgs e)
        {
            _IsSaved = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #region Private Methods

        private void _LoadAppointmentData()
        {
            if (_LocalDrivingLicenseApplicationID <= 0)
            {
                clsUtil.ShowError("Invalid Application ID.", "Error");
                this.Close();
                return;
            }

            try
            {
                // Order matters here: TestType is set first so it's already
                // correct if LocalDrivingLicenseApplicationID's setter ends up
                // reading it while resolving AddNew mode.
                ctrlScheduleTest1.TestType = _TestType;
                ctrlScheduleTest1.SetCurrentUser(clsGlobal.CurrentUserID);

                // This setter triggers the control's own _InitializeControl,
                // which looks up the active appointment (if any) for this
                // application and switches itself into Add or Update mode.
                ctrlScheduleTest1.LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationID;
            }
            catch (Exception ex)
            {
                clsUtil.ShowError($"Failed to load appointment data.\n{ex.Message}", "Error");
                this.Close();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether the appointment was successfully saved.
        /// </summary>
        public bool IsSaved => _IsSaved;

        /// <summary>
        /// Gets the test appointment ID after saving (if applicable).
        /// </summary>
        public int TestAppointmentID => ctrlScheduleTest1.TestAppointmentID;

        #endregion
    }
}