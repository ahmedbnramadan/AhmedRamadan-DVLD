using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmIssueDriverLicenseFirstTime : Form
    {
        #region Controls

        private Label lblTitle;

        private ctrlDrivingLicenseApplicationInfo ctrlApplicationInfo;

        private Label lblNotes;
        private TextBox txtNotes;

        private Button btnIssue;
        private Button btnClose;

        #endregion

        #region Data

        private readonly int _LocalDrivingLicenseApplicationID;

        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplication;

        #endregion

        #region Constructor

        public frmIssueDriverLicenseFirstTime(int LocalDrivingLicenseApplicationID)
        {
            _LocalDrivingLicenseApplicationID =
                LocalDrivingLicenseApplicationID;

            InitializeComponents();

            this.Load += FrmIssueDriverLicenseFirstTime_Load;
        }

        #endregion

        #region Form Events

        private void FrmIssueDriverLicenseFirstTime_Load(object sender, EventArgs e)
        {
            _LoadApplication();
        }

        #endregion

        #region Load Application

        private void _LoadApplication()
        {
            _LocalDrivingLicenseApplication =
                clsLocalDrivingLicenseApplication
                    .FindByLocalDrivingAppID(
                        _LocalDrivingLicenseApplicationID);

            if (_LocalDrivingLicenseApplication == null)
            {
                MessageBox.Show(
                    "Application not found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.Close();

                return;
            }

            if (!_LocalDrivingLicenseApplication.IsAllTestsPassed())
            {
                MessageBox.Show(
                    "The applicant must pass all three required tests before the license can be issued.",
                    "Cannot Issue License",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                this.Close();

                return;
            }

            if (_LocalDrivingLicenseApplication.IsLicenseIssued())
            {
                MessageBox.Show(
                    "The driving license has already been issued.",
                    "License Already Issued",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.Close();

                return;
            }

            ctrlApplicationInfo.LoadApplicationInfo(
                _LocalDrivingLicenseApplicationID);
        }

        #endregion

        #region Initialize Components

        private void InitializeComponents()
        {
            // ---------------------------------------------------------
            // Form
            // ---------------------------------------------------------

            this.Text =
                "Issue Driving License First Time";

            this.Size =
                new Size(900, 650);

            this.StartPosition =
                FormStartPosition.CenterScreen;

            this.FormBorderStyle =
                FormBorderStyle.FixedDialog;

            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.BackColor =
                Color.FromArgb(240, 242, 248);

            this.Font =
                new Font(
                    "Microsoft Sans Serif",
                    9.5F);

            // ---------------------------------------------------------
            // Title
            // ---------------------------------------------------------

            lblTitle = new Label
            {
                Text =
                    "Issue Driving License First Time",

                Font =
                    new Font(
                        "Arial",
                        18F,
                        FontStyle.Bold),

                ForeColor =
                    clsGlobal.PrimaryRed,

                AutoSize = true,

                Location =
                    new Point(250, 20)
            };

            // ---------------------------------------------------------
            // Driving License Application Information
            // ---------------------------------------------------------

            ctrlApplicationInfo =
                new ctrlDrivingLicenseApplicationInfo
                {
                    Location =
                        new Point(30, 65),

                    Size =
                        new Size(830, 400)
                };

            // ---------------------------------------------------------
            // Notes Label
            // ---------------------------------------------------------

            lblNotes = new Label
            {
                Text = "Notes:",

                AutoSize = true,

                Font =
                    new Font(
                        "Microsoft Sans Serif",
                        9.5F,
                        FontStyle.Bold),

                Location =
                    new Point(30, 480)
            };

            // ---------------------------------------------------------
            // Notes TextBox
            // ---------------------------------------------------------

            txtNotes = new TextBox
            {
                Location =
                    new Point(30, 505),

                Size =
                    new Size(830, 60),

                Multiline = true,

                ScrollBars =
                    ScrollBars.Vertical,

                MaxLength = 500
            };

            // ---------------------------------------------------------
            // Issue Button
            // ---------------------------------------------------------

            btnIssue = new Button
            {
                Text = "Issue",

                Location =
                    new Point(30, 580),

                Size =
                    new Size(100, 38),

                Font =
                    new Font(
                        "Microsoft Sans Serif",
                        10F,
                        FontStyle.Bold),

                BackColor =
                    clsGlobal.PrimaryRed,

                ForeColor =
                    Color.White,

                FlatStyle =
                    FlatStyle.Flat,

                Cursor =
                    Cursors.Hand
            };

            btnIssue.FlatAppearance.BorderSize = 0;

            btnIssue.Click +=
                BtnIssue_Click;

            // ---------------------------------------------------------
            // Close Button
            // ---------------------------------------------------------

            btnClose = new Button
            {
                Text = "Close",

                Location =
                    new Point(140, 580),

                Size =
                    new Size(100, 38),

                Font =
                    new Font(
                        "Microsoft Sans Serif",
                        10F,
                        FontStyle.Bold),

                BackColor =
                    Color.FromArgb(110, 110, 110),

                ForeColor =
                    Color.White,

                FlatStyle =
                    FlatStyle.Flat,

                Cursor =
                    Cursors.Hand
            };

            btnClose.FlatAppearance.BorderSize = 0;

            btnClose.Click +=
                BtnClose_Click;

            // ---------------------------------------------------------
            // Add Controls
            // ---------------------------------------------------------

            this.Controls.AddRange(
                new Control[]
                {
                    lblTitle,
                    ctrlApplicationInfo,
                    lblNotes,
                    txtNotes,
                    btnIssue,
                    btnClose
                });
        }

        #endregion

        #region Button Events

        private void BtnIssue_Click(
            object sender,
            EventArgs e)
        {
            if (_LocalDrivingLicenseApplication == null)
                return;

            if (!_LocalDrivingLicenseApplication.IsAllTestsPassed())
            {
                MessageBox.Show(
                    "The applicant must pass all three required tests first.",
                    "Cannot Issue License",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (_LocalDrivingLicenseApplication.IsLicenseIssued())
            {
                MessageBox.Show(
                    "The driving license has already been issued.",
                    "License Already Issued",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            int LicenseID =
                _LocalDrivingLicenseApplication
                    .IssueLicenseFrotTheFristTeim(
                        txtNotes.Text.Trim(),
                        clsGlobal.CurrentUserID);

            if (LicenseID == -1)
            {
                MessageBox.Show(
                    "Failed to issue the driving license.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            MessageBox.Show(
                "Driving license issued successfully.\n\n" +
                "License ID = " + LicenseID,
                "License Issued",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.DialogResult =
                DialogResult.OK;

            this.Close();
        }

        private void BtnClose_Click(
            object sender,
            EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}