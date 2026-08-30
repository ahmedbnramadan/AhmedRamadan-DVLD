using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class ctrlDrivingLicenseApplicationInfo : UserControl
    {
        #region Controls Declaration

        private GroupBox gbDrivingLicenseApplicationInformation;

        private Label lblDLAppIDTitle, lblDLAppID;
        private Label lblAppliedForTitle, lblAppliedFor;
        private Label lblPassedTestsTitle, lblPassedTests;

        private LinkLabel llShowLicenseInfo;

        private ctrlApplicationBasicInfo ctrlApplicationBasicInfo;

        private int _LocalDrivingLicenseApplicationID = -1;
        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplication;
        private clsLicense _License;

        #endregion

        #region Properties

        public int LocalDrivingLicenseApplicationID
        {
            get { return _LocalDrivingLicenseApplicationID; }
        }

        public clsLocalDrivingLicenseApplication SelectedLocalDrivingLicenseApplication
        {
            get { return _LocalDrivingLicenseApplication; }
        }

        public clsLicense SelectedLicenseInfo
        {
            get { return _License; }
        }

        #endregion

        #region Events

        public event EventHandler<int> ShowLicenseInfo;

        #endregion

        #region Constructor

        public ctrlDrivingLicenseApplicationInfo()
        {
            InitializeComponents();
            _SetupEvents();
        }

        #endregion

        #region Events Setup

        private void _SetupEvents()
        {
            llShowLicenseInfo.LinkClicked += LlShowLicenseInfo_LinkClicked;
        }

        private void LlShowLicenseInfo_LinkClicked(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            if (_License == null || _License.ID <= 0)
                return;

            if (ShowLicenseInfo != null)
                ShowLicenseInfo(this, _License.ID);
        }

        #endregion

        #region Initialize Components

        private void InitializeComponents()
        {
            // Setup UserControl
            this.Size = new Size(830, 400);
            this.Font = new Font("Microsoft Sans Serif", 9F);

            // Driving License Application Information
            gbDrivingLicenseApplicationInformation = new GroupBox
            {
                Text = "Driving License Application Information",
                Location = new Point(0, 0),
                Size = new Size(830, 130),
                Font = new Font(
                    "Microsoft Sans Serif",
                    10F,
                    FontStyle.Regular)
            };

            // DL Application ID
            lblDLAppIDTitle = new Label
            {
                Text = "DL App ID:",
                Location = new Point(20, 40),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblDLAppID = new Label
            {
                Text = "[???]",
                Location = new Point(135, 40),
                AutoSize = true,
                ForeColor = Color.Red
            };

            // Applied For
            lblAppliedForTitle = new Label
            {
                Text = "Applied For:",
                Location = new Point(20, 80),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblAppliedFor = new Label
            {
                Text = "[???]",
                Location = new Point(135, 80),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            // Passed Tests
            lblPassedTestsTitle = new Label
            {
                Text = "Passed Tests:",
                Location = new Point(450, 40),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblPassedTests = new Label
            {
                Text = "[???]",
                Location = new Point(580, 40),
                AutoSize = true
            };

            // Show License Info
            llShowLicenseInfo = new LinkLabel
            {
                Text = "Show License Info",
                Location = new Point(450, 80),
                AutoSize = true,
                Visible = false,
                LinkColor = Color.SteelBlue,
                Font = new Font(this.Font, FontStyle.Underline)
            };

            gbDrivingLicenseApplicationInformation.Controls.AddRange(
                new Control[]
                {
                    lblDLAppIDTitle,
                    lblDLAppID,

                    lblAppliedForTitle,
                    lblAppliedFor,

                    lblPassedTestsTitle,
                    lblPassedTests,

                    llShowLicenseInfo
                });

            // Base Application Information
            ctrlApplicationBasicInfo = new ctrlApplicationBasicInfo
            {
                Location = new Point(0, 150),
                Size = new Size(830, 250)
            };

            this.Controls.Add(gbDrivingLicenseApplicationInformation);
            this.Controls.Add(ctrlApplicationBasicInfo);
        }

        #endregion

        #region Load Application Info

        public void LoadApplicationInfo(
            int LocalDrivingLicenseApplicationID)
        {
            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppID(LocalDrivingLicenseApplicationID);

            if (_LocalDrivingLicenseApplication == null)
            {
                ResetApplicationInfo();

                MessageBox.Show(
                    "No Local Driving License Application with ID = "
                    + LocalDrivingLicenseApplicationID.ToString(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            _FillApplicationData();
        }

        public void LoadApplicationInfo(clsLocalDrivingLicenseApplication Application)
        {
            if (Application == null)
            {
                ResetApplicationInfo();
                return;
            }

            _LocalDrivingLicenseApplication = Application;

            _FillApplicationData();
        }

        #endregion

        #region Fill Data

        private void _FillApplicationData()
        {
            _LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID;

            lblDLAppID.Text = _LocalDrivingLicenseApplicationID.ToString();

            lblAppliedFor.Text = (_LocalDrivingLicenseApplication.LicenseClassInfo != null)
                ? _LocalDrivingLicenseApplication.LicenseClassInfo.Name
                : "[Unknown]";

            lblPassedTests.Text =
                _LocalDrivingLicenseApplication
                .GetPassedTestCount()
                .ToString() + " / 3";

            _License = null;

            if (_LocalDrivingLicenseApplication.IsLicenseIssued())
            {
                int LicenseID =
                    _LocalDrivingLicenseApplication.GetActiveLicenseID();

                if (LicenseID != -1)
                    _License = clsLicense.Find(LicenseID);
            }

            llShowLicenseInfo.Visible =
                (_License != null);

            // Load the Base Application Card
            ctrlApplicationBasicInfo.LoadApplicationInfo(
                _LocalDrivingLicenseApplication);
        }

        #endregion

        #region Reset

        public void ResetApplicationInfo()
        {
            _LocalDrivingLicenseApplicationID = -1;
            _LocalDrivingLicenseApplication = null;
            _License = null;

            lblDLAppID.Text     = "[???]";
            lblAppliedFor.Text  = "[???]";
            lblPassedTests.Text = "[???]";

            llShowLicenseInfo.Visible = false;

            ctrlApplicationBasicInfo.ResetApplicationInfo();
        }

        #endregion
    }
}