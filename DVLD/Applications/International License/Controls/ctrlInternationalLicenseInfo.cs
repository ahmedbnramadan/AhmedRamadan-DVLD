using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    /// <summary>
    /// Read-only card showing a single international license merged with
    /// the driver's identity — mirrors ctrlDriverLicenseInfo's shape, but
    /// scoped to the fields that matter for an international license.
    /// </summary>
    public class ctrlInternationalLicenseInfo : UserControl
    {
        #region Controls

        private GroupBox gbInfo;

        // Left column — driver identity
        private Label lblNameTitle, lblName;
        private Label lblLicenseIDTitle, lblLicenseID;     // local license ID
        private Label lblNationalNoTitle, lblNationalNo;
        private Label lblGenderTitle, lblGender;
        private Label lblIssueDateTitle, lblIssueDate;     // local license issue date

        // Right column — international license specifics
        private Label lblIntlLicenseIDTitle, lblIntlLicenseID;
        private Label lblApplicationIDTitle, lblApplicationID;
        private Label lblIsActiveTitle, lblIsActive;
        private Label lblDriverIDTitle, lblDriverID;
        private Label lblExpirationDateTitle, lblExpirationDate;

        private PictureBox pbPersonImage;

        private clsInternationalLicense _License;

        #endregion

        public clsInternationalLicense SelectedLicenseInfo => _License;

        public ctrlInternationalLicenseInfo()
        {
            _InitializeComponents();
        }

        private void _InitializeComponents()
        {
            this.Size = new Size(700, 320);
            this.Font = new Font("Microsoft Sans Serif", 9F);

            gbInfo = new GroupBox
            {
                Text = "International License Info",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular)
            };

            // ── Left column ──────────────────────────────────────────
            lblNameTitle = _Bold("Name:", 20, 40);
            lblName      = _Value("[???]", 150, 40, Color.DarkBlue);

            lblLicenseIDTitle = _Bold("License ID :", 20, 75);
            lblLicenseID      = _Value("[???]", 150, 75, Color.SteelBlue);

            lblNationalNoTitle = _Bold("National No:", 20, 110);
            lblNationalNo      = _Value("[???]", 150, 110, Color.Black);

            lblGenderTitle = _Bold("Gender:", 20, 145);
            lblGender      = _Value("[???]", 150, 145, Color.Black);

            lblIssueDateTitle = _Bold("Issue Date:", 20, 180);
            lblIssueDate      = _Value("[???]", 150, 180, Color.Black);

            // ── Right column ─────────────────────────────────────────
            lblIntlLicenseIDTitle = _Bold("Int.License ID:", 300, 40);
            lblIntlLicenseID      = _Value("[???]", 430, 40, Color.Red);

            lblApplicationIDTitle = _Bold("Application ID:", 300, 75);
            lblApplicationID      = _Value("[???]", 430, 75, Color.Black);

            lblIsActiveTitle = _Bold("Is Active?", 300, 110);
            lblIsActive      = _Value("[???]", 430, 110, Color.DarkGreen);

            lblDriverIDTitle = _Bold("Driver ID:", 300, 145);
            lblDriverID      = _Value("[???]", 430, 145, Color.Black);

            lblExpirationDateTitle = _Bold("Expiration Date:", 300, 180);
            lblExpirationDate      = _Value("[???]", 430, 180, Color.DarkRed);

            // ── Photo ─────────────────────────────────────────────────
            pbPersonImage = new PictureBox
            {
                Location = new Point(300, 215),
                Size = new Size(150, 90),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            gbInfo.Controls.AddRange(new Control[]
            {
                lblNameTitle, lblName,
                lblLicenseIDTitle, lblLicenseID,
                lblNationalNoTitle, lblNationalNo,
                lblGenderTitle, lblGender,
                lblIssueDateTitle, lblIssueDate,

                lblIntlLicenseIDTitle, lblIntlLicenseID,
                lblApplicationIDTitle, lblApplicationID,
                lblIsActiveTitle, lblIsActive,
                lblDriverIDTitle, lblDriverID,
                lblExpirationDateTitle, lblExpirationDate,

                pbPersonImage
            });

            this.Controls.Add(gbInfo);
        }

        private Label _Bold(string text, int x, int y)
            => new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };

        private Label _Value(string text, int x, int y, Color c)
            => new Label { Text = text, Location = new Point(x, y), AutoSize = true, ForeColor = c };

        #region Load / Reset

        public void LoadLicenseInfo(int internationalLicenseID)
        {
            _License = clsInternationalLicense.Find(internationalLicenseID);

            if (_License == null)
            {
                ResetLicenseInfo();
                clsUtil.ShowError($"No International License with ID = {internationalLicenseID}.");
                return;
            }

            _FillData();
        }

        public void LoadLicenseInfo(clsInternationalLicense license)
        {
            if (license == null) { ResetLicenseInfo(); return; }
            _License = license;
            _FillData();
        }

        private void _FillData()
        {
            // International-license-specific fields
            lblIntlLicenseID.Text  = _License.InternationalLicenseID.ToString();
            lblApplicationID.Text  = _License.ApplicationID.ToString();
            lblIsActive.Text       = _License.IsActive ? "Yes" : "No";
            lblDriverID.Text       = _License.DriverID.ToString();
            lblExpirationDate.Text = _License.ExpirationDate.ToShortDateString();

            // The "License ID" and "Issue Date" shown here belong to the
            // LOCAL license this was issued from — that's what your
            // teacher's card displays (it identifies the driver via the
            // local license, not the international one).
            clsLicense localLicense = _License.LocalLicenseInfo;
            lblLicenseID.Text = localLicense != null
                ? localLicense.ID.ToString()
                : _License.IssuedUsingLocalLicenseID.ToString();

            lblIssueDate.Text = localLicense != null
                ? localLicense.IssueDate.ToShortDateString()
                : "[Unknown]";

            clsPerson person = _License.DriverInfo?.PersonInfo;

            lblName.Text       = person?.FullName ?? "[Unknown]";
            lblNationalNo.Text = person?.NationalNo ?? "[Unknown]";
            lblGender.Text     = person != null ? (person.Gender == 0 ? "Male" : "Female") : "[Unknown]";

            if (person != null)
                clsUtil.LoadPersonImage(pbPersonImage, person.ImagePath);
            else
                pbPersonImage.Image = null;
        }

        public void ResetLicenseInfo()
        {
            _License = null;

            lblName.Text = lblLicenseID.Text = lblNationalNo.Text = lblGender.Text =
            lblIssueDate.Text = lblIntlLicenseID.Text = lblApplicationID.Text =
            lblIsActive.Text = lblDriverID.Text = lblExpirationDate.Text = "[???]";

            pbPersonImage.Image = null;
        }

        #endregion
    }
}