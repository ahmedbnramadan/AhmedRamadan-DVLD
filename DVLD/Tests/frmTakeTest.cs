using System;
using System.Drawing;
using System.Windows.Forms;
using Business;
using DVLD.Tests.Controls;

namespace DVLD.Tests
{
    public class frmTakeTest : Form
    {
        #region Controls Declaration

        private Label lblTitle;
        private ctrlTakeTest ctrlTakeTest1;
        private Button btnClose;

        #endregion

        #region State

        private readonly int _TestAppointmentID;
        private readonly clsTestType.enTestType _TestType;

        #endregion

        #region Constructor

        public frmTakeTest(int appointmentID, clsTestType.enTestType testType)
        {
            _TestAppointmentID = appointmentID;
            _TestType = testType;

            _InitializeComponents();
            _SetupEvents();
        }

        #endregion

        #region Initialization

        private void _InitializeComponents()
        {
            this.Text = "Take Test";
            this.Size = new Size(1000, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 10F);

            lblTitle = new Label
            {
                Text = "Take Test",
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            ctrlTakeTest1 = new ctrlTakeTest
            {
                Location = new Point(30, 70),
                Size = new Size(950, 470),
                Dock = DockStyle.None
            };

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(410, 555),
                Size = new Size(180, 40),
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                BackColor = clsGlobal.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new Control[] { lblTitle, ctrlTakeTest1, btnClose });
        }

        private void _SetupEvents()
        {
            this.Load += frmTakeTest_Load;
            btnClose.Click += (s, e) => this.Close();
            ctrlTakeTest1.OnTestSaved += CtrlTakeTest1_OnTestSaved;
        }

        #endregion

        #region Event Handlers

        private void frmTakeTest_Load(object sender, EventArgs e)
        {
            if (_TestAppointmentID <= 0)
            {
                clsUtil.ShowError("Invalid Test Appointment ID.", "Error");
                this.Close();
                return;
            }

            ctrlTakeTest1.SetCurrentUser(clsGlobal.CurrentUserID);
            ctrlTakeTest1.LoadTestAppointment(_TestAppointmentID, _TestType);
        }

        private void CtrlTakeTest1_OnTestSaved(object sender, EventArgs e)
        {
            IsSaved = true;
        }

        #endregion

        #region Public Properties

        /// <summary>True once at least one successful save has happened
        /// during this dialog's lifetime (a fresh save or a notes update).</summary>
        public bool IsSaved { get; private set; }

        #endregion
    }
}