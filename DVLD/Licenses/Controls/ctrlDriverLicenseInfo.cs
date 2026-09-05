using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class ctrlDriverLicenseInfo : UserControl
    {
        #region Controls Declaration

        private GroupBox gbDriverLicenseInfo;

        private Label lblLicenseIDTitle, lblLicenseID;
        private Label lblClassTitle, lblClass;
        private Label lblDriverIDTitle, lblDriverID;
        private Label lblNationalNoTitle, lblNationalNo;
        private Label lblNameTitle, lblName;
        private Label lblGenderTitle, lblGender;
        private Label lblDateOfBirthTitle, lblDateOfBirth;

        private Label lblIssueDateTitle, lblIssueDate;
        private Label lblExpirationDateTitle, lblExpirationDate;
        private Label lblIssueReasonTitle, lblIssueReason;
        private Label lblIsActiveTitle, lblIsActive;
        private Label lblIsDetainedTitle, lblIsDetained;

        private Label lblNotesTitle;
        private Label lblNotes;

        private PictureBox pbPersonImage;

        private int _LicenseID = -1;
        private clsLicense _License;

        #endregion

        #region Properties

        public int LicenseID
        {
            get { return _LicenseID; }
        }

        public clsLicense SelectedLicenseInfo
        {
            get { return _License; }
        }

        #endregion

        #region Constructor

        public ctrlDriverLicenseInfo()
        {
            InitializeComponents();
        }

        #endregion

        #region Initialize Components

        private void InitializeComponents()
        {
            // UserControl
            this.Size = new Size(830, 340);
            this.Font = new Font("Microsoft Sans Serif", 9F);

            // Driver License Information
            gbDriverLicenseInfo = new GroupBox
            {
                Text = "Driver License Info",
                Dock = DockStyle.Fill,
                Font = new Font(
                    "Microsoft Sans Serif",
                    10F,
                    FontStyle.Regular)
            };

            // ---------------------------------------------------------
            // Column 1 - Driver Information
            // ---------------------------------------------------------

            lblClassTitle = new Label
            {
                Text = "Class:",
                Location = new Point(20, 40),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblClass = new Label
            {
                Text = "[???]",
                Location = new Point(135, 40),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            lblNameTitle = new Label
            {
                Text = "Name:",
                Location = new Point(20, 75),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblName = new Label
            {
                Text = "[???]",
                Location = new Point(135, 75),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            lblLicenseIDTitle = new Label
            {
                Text = "License ID:",
                Location = new Point(20, 110),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblLicenseID = new Label
            {
                Text = "[???]",
                Location = new Point(135, 110),
                AutoSize = true,
                ForeColor = Color.Red
            };

            lblNationalNoTitle = new Label
            {
                Text = "National No:",
                Location = new Point(20, 145),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblNationalNo = new Label
            {
                Text = "[???]",
                Location = new Point(135, 145),
                AutoSize = true
            };

            lblGenderTitle = new Label
            {
                Text = "Gender:",
                Location = new Point(20, 180),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblGender = new Label
            {
                Text = "[???]",
                Location = new Point(135, 180),
                AutoSize = true
            };

            lblIssueDateTitle = new Label
            {
                Text = "Issue Date:",
                Location = new Point(20, 215),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblIssueDate = new Label
            {
                Text = "[???]",
                Location = new Point(135, 215),
                AutoSize = true
            };

            lblIssueReasonTitle = new Label
            {
                Text = "Issue Reason:",
                Location = new Point(20, 250),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblIssueReason = new Label
            {
                Text = "[???]",
                Location = new Point(135, 250),
                AutoSize = true
            };

            // ---------------------------------------------------------
            // Column 2 - License Information
            // ---------------------------------------------------------

            lblIsActiveTitle = new Label
            {
                Text = "Is Active:",
                Location = new Point(350, 40),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblIsActive = new Label
            {
                Text = "[???]",
                Location = new Point(470, 40),
                AutoSize = true
            };

            lblDateOfBirthTitle = new Label
            {
                Text = "Date Of Birth:",
                Location = new Point(350, 75),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblDateOfBirth = new Label
            {
                Text = "[???]",
                Location = new Point(470, 75),
                AutoSize = true
            };

            lblDriverIDTitle = new Label
            {
                Text = "Driver ID:",
                Location = new Point(350, 110),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblDriverID = new Label
            {
                Text = "[???]",
                Location = new Point(470, 110),
                AutoSize = true
            };

            lblExpirationDateTitle = new Label
            {
                Text = "Expiration Date:",
                Location = new Point(350, 145),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblExpirationDate = new Label
            {
                Text = "[???]",
                Location = new Point(470, 145),
                AutoSize = true
            };

            lblIsDetainedTitle = new Label
            {
                Text = "Is Detained:",
                Location = new Point(350, 180),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblIsDetained = new Label
            {
                Text = "[???]",
                Location = new Point(470, 180),
                AutoSize = true
            };

            // ---------------------------------------------------------
            // Driver Picture - Right Side
            // ---------------------------------------------------------

            pbPersonImage = new PictureBox
            {
                Location = new Point(665, 35),
                Size = new Size(140, 170),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            // ---------------------------------------------------------
            // Notes
            // ---------------------------------------------------------

            lblNotesTitle = new Label
            {
                Text = "Notes:",
                Location = new Point(350, 215),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblNotes = new Label
            {
                Text = "[???]",
                Location = new Point(350, 240),
                Size = new Size(455, 65),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5),
                BackColor = Color.White
            };

            // ---------------------------------------------------------
            // Add Controls
            // ---------------------------------------------------------

            gbDriverLicenseInfo.Controls.AddRange(
                new Control[]
                {
                    lblClassTitle,
                    lblClass,

                    lblNameTitle,
                    lblName,

                    lblLicenseIDTitle,
                    lblLicenseID,

                    lblNationalNoTitle,
                    lblNationalNo,

                    lblGenderTitle,
                    lblGender,

                    lblIssueDateTitle,
                    lblIssueDate,

                    lblIssueReasonTitle,
                    lblIssueReason,

                    lblIsActiveTitle,
                    lblIsActive,

                    lblDateOfBirthTitle,
                    lblDateOfBirth,

                    lblDriverIDTitle,
                    lblDriverID,

                    lblExpirationDateTitle,
                    lblExpirationDate,

                    lblIsDetainedTitle,
                    lblIsDetained,

                    pbPersonImage,

                    lblNotesTitle,
                    lblNotes
                });

            this.Controls.Add(gbDriverLicenseInfo);
        }

        #endregion

        #region Load License Info

        public void LoadLicenseInfo(int LicenseID)
        {
            _License = clsLicense.Find(LicenseID);

            if (_License == null)
            {
                ResetLicenseInfo();

                clsUtil.ShowError($"No License with ID = {LicenseID}.");

                return;
            }

            _FillLicenseData();
        }

        public void LoadLicenseInfo(clsLicense License)
        {
            if (License == null)
            {
                ResetLicenseInfo();
                return;
            }

            _License = License;

            _FillLicenseData();
        }

        #endregion

        #region Fill Data

        private void _FillLicenseData()
        {
            _LicenseID =
                _License.ID;

            lblLicenseID.Text =
                _License.ID.ToString();

            // License Class
            if (_License.LicenseClassInfo != null)
            {
                lblClass.Text =
                    _License.LicenseClassInfo.Name;
            }
            else
            {
                lblClass.Text = "[Unknown]";
            }

            // Driver
            if (_License.DriverInfo != null)
            {
                lblDriverID.Text =
                    _License.DriverInfo.ID.ToString();

                if (_License.DriverInfo.PersonInfo != null)
                {
                    clsPerson Person =
                        _License.DriverInfo.PersonInfo;

                    lblName.Text =
                        Person.FullName;

                    lblNationalNo.Text =
                        Person.NationalNo;

                    lblGender.Text =
                        Person.Gender == 0
                            ? "Male"
                            : "Female";

                    lblDateOfBirth.Text =
                        Person.DateOfBirth.ToShortDateString();

                    _LoadPersonImage(Person);
                }
                else
                {
                    lblName.Text = "[Unknown]";
                    lblNationalNo.Text = "[Unknown]";
                    lblGender.Text = "[Unknown]";
                    lblDateOfBirth.Text = "[Unknown]";

                    _ClearPersonImage();
                }
            }
            else
            {
                lblDriverID.Text = "[Unknown]";
                lblName.Text = "[Unknown]";
                lblNationalNo.Text = "[Unknown]";
                lblGender.Text = "[Unknown]";
                lblDateOfBirth.Text = "[Unknown]";

                _ClearPersonImage();
            }

            // License Information
            lblIssueDate.Text =
                _License.IssueDate.ToShortDateString();

            lblExpirationDate.Text =
                _License.ExpirationDate.ToShortDateString();

            lblIssueReason.Text =
                _License.IssueReasonText;

            lblIsActive.Text =
                _License.IsActive
                    ? "Yes"
                    : "No";

            lblIsDetained.Text =
                _License.IsDetained
                    ? "Yes"
                    : "No";

            lblNotes.Text =
                string.IsNullOrWhiteSpace(_License.Notes)
                    ? "No Notes"
                    : _License.Notes;
        }

        #endregion

        #region Image

        private void _LoadPersonImage(clsPerson Person)
        {
            clsGlobal.CreateImagesFolderIfDoesNotExist();

            string fallbackPath =
                clsGlobal.GetDefaultPersonImagePath(
                    Person.Gender);

            Image defaultImage = null;

            if (File.Exists(fallbackPath))
            {
                try
                {
                    using (MemoryStream ms =
                        new MemoryStream(
                            File.ReadAllBytes(fallbackPath)))
                    {
                        defaultImage =
                            Image.FromStream(ms);
                    }
                }
                catch
                {
                    defaultImage = null;
                }
            }

            clsUtil.LoadPersonImage(
                pbPersonImage,
                Person.ImagePath,
                defaultImage);
        }

        private void _ClearPersonImage()
        {
            if (pbPersonImage.Image != null)
            {
                Image oldImage =
                    pbPersonImage.Image;

                pbPersonImage.Image = null;

                oldImage.Dispose();
            }
        }

        #endregion

        #region Reset

        public void ResetLicenseInfo()
        {
            _LicenseID = -1;
            _License = null;

            lblClass.Text = "[???]";
            lblName.Text = "[???]";
            lblLicenseID.Text = "[???]";
            lblNationalNo.Text = "[???]";
            lblGender.Text = "[???]";
            lblIssueDate.Text = "[???]";
            lblIssueReason.Text = "[???]";

            lblIsActive.Text = "[???]";
            lblDateOfBirth.Text = "[???]";
            lblDriverID.Text = "[???]";
            lblExpirationDate.Text = "[???]";
            lblIsDetained.Text = "[???]";

            lblNotes.Text = "[???]";

            _ClearPersonImage();
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ClearPersonImage();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}