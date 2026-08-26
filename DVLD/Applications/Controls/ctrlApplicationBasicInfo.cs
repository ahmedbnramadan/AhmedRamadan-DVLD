using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class ctrlApplicationBasicInfo : UserControl
    {
        #region Controls Declaration

        private GroupBox gbApplicationInformation;

        private Label lblApplicationIDTitle, lblApplicationID;
        private Label lblStatusTitle, lblStatus;
        private Label lblFeesTitle, lblFees;
        private Label lblTypeTitle, lblType;
        private Label lblApplicantTitle, lblApplicant;

        private Label lblApplicationDateTitle, lblApplicationDate;
        private Label lblStatusDateTitle, lblStatusDate;
        private Label lblCreatedByTitle, lblCreatedBy;

        private LinkLabel llViewPersonInfo;

        private int _ApplicationID = -1;
        private clsApplication _Application;

        #endregion

        #region Properties

        public int ApplicationID
        {
            get { return _ApplicationID; }
        }

        public clsApplication SelectedApplicationInfo
        {
            get { return _Application; }
        }

        #endregion

        #region Constructor

        public ctrlApplicationBasicInfo()
        {
            InitializeComponents();
            _SetupEvents();
        }

        #endregion

        #region Events

        private void _SetupEvents()
        {
            llViewPersonInfo.LinkClicked += LlViewPersonInfo_LinkClicked;
        }

        private void LlViewPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_Application == null || _Application.ApplicantPersonID <= 0)
            {
                return;
            }

            using (var frm = new frmShowPersonInfo(_Application.ApplicantPersonID))
            {
                frm.ShowDialog();
            }
        }

        #endregion

        #region Initialize Components

        private void InitializeComponents()
        {
            // UserControl
            this.Size = new Size(830, 300);
            this.Font = new Font( "Microsoft Sans Serif", 9F);

            // GroupBox
            gbApplicationInformation = new GroupBox
            {
                Text = "Application Information",
                Dock = DockStyle.Fill,
                Font = new Font( "Microsoft Sans Serif", 10F, FontStyle.Regular)
            };

            // ---------------------------------------------------------
            // Column 1
            // ---------------------------------------------------------

            lblApplicationIDTitle = new Label
            {
                Text = "Application ID:",
                Location = new Point(20, 40),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblApplicationID = new Label
            {
                Text = "[???]",
                Location = new Point(135, 40),
                AutoSize = true,
                ForeColor = Color.Red
            };

            lblStatusTitle = new Label
            {
                Text = "Status:",
                Location = new Point(20, 80),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblStatus = new Label
            {
                Text = "[???]",
                Location = new Point(135, 80),
                AutoSize = true
            };

            lblFeesTitle = new Label
            {
                Text = "Fees:",
                Location = new Point(20, 120),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblFees = new Label
            {
                Text = "[???]",
                Location = new Point(135, 120),
                AutoSize = true
            };

            lblTypeTitle = new Label
            {
                Text = "Type:",
                Location = new Point(20, 160),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblType = new Label
            {
                Text = "[???]",
                Location = new Point(135, 160),
                AutoSize = true
            };

            lblApplicantTitle = new Label
            {
                Text = "Applicant:",
                Location = new Point(20, 200),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblApplicant = new Label
            {
                Text = "[???]",
                Location = new Point(135, 200),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            // ---------------------------------------------------------
            // Column 2
            // ---------------------------------------------------------

            lblApplicationDateTitle = new Label
            {
                Text = "Application Date:",
                Location = new Point(450, 80),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblApplicationDate = new Label
            {
                Text = "[???]",
                Location = new Point(580, 80),
                AutoSize = true
            };

            lblStatusDateTitle = new Label
            {
                Text = "Status Date:",
                Location = new Point(450, 120),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblStatusDate = new Label
            {
                Text = "[???]",
                Location = new Point(580, 120),
                AutoSize = true
            };

            lblCreatedByTitle = new Label
            {
                Text = "Created By:",
                Location = new Point(450, 160),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblCreatedBy = new Label
            {
                Text = "[???]",
                Location = new Point(580, 160),
                AutoSize = true
            };

            // View Person Info link
            llViewPersonInfo = new LinkLabel
            {
                Text = "View Person Info",
                Location = new Point(350, 200),
                AutoSize = true,
                Visible = false,
                LinkColor = Color.SteelBlue,
                Font = new Font(
                    this.Font,
                    FontStyle.Underline)
            };

            // ---------------------------------------------------------
            // Add controls
            // ---------------------------------------------------------

            gbApplicationInformation.Controls.AddRange(
                new Control[]
                {
                    lblApplicationIDTitle,
                    lblApplicationID,

                    lblStatusTitle,
                    lblStatus,

                    lblFeesTitle,
                    lblFees,

                    lblTypeTitle,
                    lblType,

                    lblApplicantTitle,
                    lblApplicant,

                    lblApplicationDateTitle,
                    lblApplicationDate,

                    lblStatusDateTitle,
                    lblStatusDate,

                    lblCreatedByTitle,
                    lblCreatedBy,

                    llViewPersonInfo
                });

            this.Controls.Add(gbApplicationInformation);
        }

        #endregion

        #region Load Application

        public void LoadApplicationInfo(int ApplicationID)
        {
            _Application =
                clsApplication.FindBaseApplication(ApplicationID);

            if (_Application == null)
            {
                ResetApplicationInfo();

                MessageBox.Show(
                    "No Application with ID = " + ApplicationID.ToString(), "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            _FillApplicationData();
        }

        public void LoadApplicationInfo(clsApplication Application)
        {
            if (Application == null)
            {
                ResetApplicationInfo();
                return;
            }

            _Application = Application;

            _FillApplicationData();
        }

        #endregion

        #region Fill Data

        private void _FillApplicationData()
        {
            _ApplicationID          = _Application.ApplicationID;

            lblApplicationID.Text   = _Application.ApplicationID.ToString();

            lblStatus.Text          = _GetApplicationStatusText(_Application.ApplicationStatus);

            lblFees.Text            = _Application.PaidFees.ToString("0.00");

            // Application Type
            if (_Application.ApplicationTypeInfo != null)
            {
                lblType.Text = _Application.ApplicationTypeInfo.Title;
            }
            else
            {
                lblType.Text = "[Unknown]";
            }

            // Applicant
            if (_Application.PersonInfo != null)
            {
                lblApplicant.Text   = _Application.PersonInfo.FullName;
            }
            else
            {
                lblApplicant.Text   = "[Unknown]";
            }

            // Dates
            lblApplicationDate.Text = _Application.ApplicationDate.ToShortDateString();

            lblStatusDate.Text      = _Application.LastStatusDate.ToShortDateString();

            // Created By
            if (_Application.UserInfo != null)
            {
                lblCreatedBy.Text = _Application.UserInfo.UserName;
            }
            else
            {
                lblCreatedBy.Text = "[Unknown]";
            }

            // Show link only when applicant exists
            llViewPersonInfo.Visible = (_Application.PersonInfo != null);
        }

        #endregion

        #region Helpers

        private string _GetApplicationStatusText(
            clsApplication.enApplicationStatus Status)
        {
            switch (Status)
            {
                case clsApplication.enApplicationStatus.New:
                    return "New";

                case clsApplication.enApplicationStatus.Cancelled:
                    return "Cancelled";

                case clsApplication.enApplicationStatus.Completed:
                    return "Completed";

                default:
                    return "[Unknown]";
            }
        }

        #endregion

        #region Reset

        public void ResetApplicationInfo()
        {
            _ApplicationID = -1;
            _Application = null;

            lblApplicationID.Text   = "[???]";
            lblStatus.Text          = "[???]";
            lblFees.Text            = "[???]";
            lblType.Text            = "[???]";
            lblApplicant.Text       = "[???]";

            lblApplicationDate.Text = "[???]";
            lblStatusDate.Text      = "[???]";
            lblCreatedBy.Text       = "[???]";

            llViewPersonInfo.Visible = false;
        }

        #endregion
    }
}