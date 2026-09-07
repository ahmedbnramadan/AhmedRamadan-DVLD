using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmReleaseDetainedLicense : Form
    {
        #region Controls

        private Label lblTitle;

        private ctrlDriverLicenseInfoWithFilter ctrlDriverLicenseInfoWithFilter1;

        private GroupBox gbDetainInfo;
        private Label lblDetainIDTitle, lblDetainID;
        private Label lblLicenseIDTitle, lblLicenseIDValue;
        private Label lblDetainDateTitle, lblDetainDateValue;
        private Label lblCreatedByTitle, lblCreatedByValue;
        private Label lblApplicationFeesTitle, lblApplicationFeesValue;
        private Label lblFineFeesTitle, lblFineFeesValue;
        private Label lblTotalFeesTitle, lblTotalFeesValue;
        private Label lblApplicationIDTitle, lblApplicationIDValue;

        private LinkLabel lnkShowLicensesHistory;
        private LinkLabel lnkShowLicenseInfo;

        private Button btnRelease;
        private Button btnClose;

        #endregion

        #region State

        // The two objects everything else is derived from. Set ONLY in
        // _LicenseLoaded - every other method just reads them.
        private clsLicense _License;
        private clsDetainedLicense _DetainedLicense;

        // Fees are pulled from ApplicationTypeID = 5
        // ("Release Detained Driving License") - see SQL_Sever_inserting_script.
        private const int ReleaseApplicationTypeID = 5;
        private decimal _ApplicationFees;

        // <= 0 means "no specific license - let the user search" (same
        // sentinel convention as _personID in frmShowPersonLicenseHistory).
        private readonly int _presetLicenseID;

        #endregion

        // ── Constructors ─────────────────────────────────────────────────────

        /// <summary>Opens in search mode - the user finds the license themselves.</summary>
        public frmReleaseDetainedLicense() : this(-1) { }

        /// <summary>
        /// Opens locked to a specific license (e.g. from a "Release" context-menu
        /// action on a grid row that already knows the License ID). The filter
        /// is disabled so the user can't switch to a different license mid-flow.
        /// </summary>
        /// <param name="licenseID">
        /// The License ID to load and lock onto. Any value &lt;= 0 falls back
        /// to search mode instead of failing.
        /// </param>
        public frmReleaseDetainedLicense(int licenseID)
        {
            _presetLicenseID = licenseID;

            _InitializeComponents();
            _SetupEvents();

            // Deferred to Load (not done here): if the preset license turns
            // out to be invalid, _LicenseLoaded's validation will want to
            // close this form (see _CloseIfLocked) - and Close() called from
            // inside a constructor does not reliably stop a queued
            // ShowDialog() from still displaying an empty window. Same
            // pattern already used in frmShowPersonLicenseHistory,
            // frmEditTestType, and frmShowPersonInfo.
            this.Load += frmReleaseDetainedLicense_Load;
        }

        #region Build

        private void _InitializeComponents()
        {
            this.Text = "Release Detained License";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label
            {
                Text = "Release Detained License",
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(280, 18)
            };

            ctrlDriverLicenseInfoWithFilter1 = new ctrlDriverLicenseInfoWithFilter
            {
                Location = new Point(30, 70),
                Size = new Size(850, 430)
            };

            _BuildDetainInfoGroup();

            lnkShowLicensesHistory = new LinkLabel
            {
                Text = "Show Licenses History",
                AutoSize = true,
                Location = new Point(30, gbDetainInfo.Bottom + 18),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Underline),
                LinkColor = Color.SteelBlue,
                Enabled = false,
                TabStop = false
            };

            lnkShowLicenseInfo = new LinkLabel
            {
                Text = "Show License Info",
                AutoSize = true,
                Location = new Point(220, gbDetainInfo.Bottom + 18),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Underline),
                LinkColor = Color.SteelBlue,
                Enabled = false,
                TabStop = false
            };

            btnRelease = _CreateButton("✔  Release", 640, gbDetainInfo.Bottom + 12, Color.FromArgb(0, 140, 60));
            btnRelease.Enabled = false;

            btnClose = _CreateButton("✖  Close", 770, gbDetainInfo.Bottom + 12, Color.FromArgb(192, 50, 50));

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                ctrlDriverLicenseInfoWithFilter1,
                gbDetainInfo,
                lnkShowLicensesHistory,
                lnkShowLicenseInfo,
                btnRelease,
                btnClose
            });

            this.ClientSize = new Size(910, btnClose.Bottom + 25);
        }

        private void _BuildDetainInfoGroup()
        {
            gbDetainInfo = new GroupBox
            {
                Text = "Detain Info",
                Location = new Point(30, ctrlDriverLicenseInfoWithFilter1.Bottom + 15),
                Size = new Size(850, 180),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            const int leftX = 20, leftValX = 165;
            const int rightX = 430, rightValX = 570;
            const int step = 35;
            int y = 35;

            lblDetainIDTitle = _Bold("Detain ID:", leftX, y);
            lblDetainID = _Value("[???]", leftValX, y, Color.SteelBlue);

            lblLicenseIDTitle = _Bold("License ID:", rightX, y);
            lblLicenseIDValue = _Value("[???]", rightValX, y, Color.Black);
            y += step;

            lblDetainDateTitle = _Bold("Detain Date:", leftX, y);
            lblDetainDateValue = _Value("[??/??/????]", leftValX, y, Color.Black);

            lblCreatedByTitle = _Bold("Created By:", rightX, y);
            lblCreatedByValue = _Value("[????]", rightValX, y, Color.Black);
            y += step;

            lblApplicationFeesTitle = _Bold("Application Fees:", leftX, y);
            lblApplicationFeesValue = _Value("[$$$$]", leftValX, y, Color.Brown);

            lblFineFeesTitle = _Bold("Fine Fees:", rightX, y);
            lblFineFeesValue = _Value("[$$$$]", rightValX, y, Color.Brown);
            y += step;

            lblTotalFeesTitle = _Bold("Total Fees:", leftX, y);
            lblTotalFeesValue = _Value("[$$$$]", leftValX, y, Color.DarkRed);

            lblApplicationIDTitle = _Bold("Application ID:", rightX, y);
            lblApplicationIDValue = _Value("[????]", rightValX, y, Color.Black);

            gbDetainInfo.Controls.AddRange(new Control[]
            {
                lblDetainIDTitle,        lblDetainID,
                lblLicenseIDTitle,       lblLicenseIDValue,
                lblDetainDateTitle,      lblDetainDateValue,
                lblCreatedByTitle,       lblCreatedByValue,
                lblApplicationFeesTitle, lblApplicationFeesValue,
                lblFineFeesTitle,        lblFineFeesValue,
                lblTotalFeesTitle,       lblTotalFeesValue,
                lblApplicationIDTitle,   lblApplicationIDValue
            });
        }

        private static Label _Bold(string text, int x, int y) => new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(60, 60, 70)
        };

        private static Label _Value(string text, int x, int y, Color color) => new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Microsoft Sans Serif", 9.5F),
            ForeColor = color
        };

        private static Button _CreateButton(string text, int x, int y, Color color)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(115, 36),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        #endregion

        #region Events

        private void _SetupEvents()
        {
            ctrlDriverLicenseInfoWithFilter1.LicenseLoaded += _LicenseLoaded;

            lnkShowLicensesHistory.LinkClicked += (s, e) =>
            {
                if (_License?.DriverInfo == null) return;
                new frmShowPersonLicenseHistory(_License.DriverInfo.PersonID).ShowDialog();
            };

            lnkShowLicenseInfo.LinkClicked += (s, e) =>
            {
                if (_License == null) return;
                new frmShowLicenseInfo(_License.ID).ShowDialog();
            };

            btnRelease.Click += _Release;
            btnClose.Click += (s, e) => this.Close();
        }

        #endregion

        #region Form Events

        private void frmReleaseDetainedLicense_Load(object sender, EventArgs e)
        {
            if (_presetLicenseID <= 0)
                return; // search mode - filter stays enabled, nothing to preload

            // LoadLicenseInfo internally calls EnableFilter(false) AND raises
            // LicenseLoaded, so this runs through the exact same validation
            // path as a manual search - there is no second code path here.
            ctrlDriverLicenseInfoWithFilter1.LoadLicenseInfo(_presetLicenseID);
        }

        #endregion

        #region License Loading / Validation

        private void _LicenseLoaded(object sender, clsLicense license)
        {
            _ResetReleaseState();

            if (license == null)
            {
                // "not found" message already shown by the filter control.
                _CloseIfLocked();
                return;
            }

            // A license that isn't detained at all can't be released -
            // check this via clsDetainedLicense, not license.IsActive,
            // since IsActive/IsDetained are two different concerns.
            clsDetainedLicense detained = clsDetainedLicense.FindByLicenseID(license.ID);

            if (detained == null || detained.IsReleased)
            {
                clsUtil.ShowWarning(
                    "This license is not currently detained.",
                    "Cannot Release License");

                _CloseIfLocked();
                return;
            }

            clsApplicationType releaseType = clsApplicationType.Find(ReleaseApplicationTypeID);

            if (releaseType == null)
            {
                clsUtil.ShowError(
                    "The release application type is not configured.",
                    "Cannot Release License");

                _CloseIfLocked();
                return;
            }

            _License = license;
            _DetainedLicense = detained;
            _ApplicationFees = releaseType.Fees;

            lblDetainID.Text        = detained.ID.ToString();
            lblLicenseIDValue.Text  = license.ID.ToString();
            lblDetainDateValue.Text = clsFormat.DateShort(detained.DetainDate);
            lblCreatedByValue.Text  = detained.CreatedByUserInfo?.UserName ?? "[Unknown]";

            lblApplicationFeesValue.Text = _ApplicationFees.ToString("0.00");
            lblFineFeesValue.Text        = detained.FineFees.ToString("0.00");
            lblTotalFeesValue.Text       = (_ApplicationFees + detained.FineFees).ToString("0.00");
            lblApplicationIDValue.Text   = "[Will be created after release]";

            lnkShowLicensesHistory.Enabled = true;
            lnkShowLicenseInfo.Enabled     = true;

            _UpdateReleaseButtonState();
        }

        private void _ResetReleaseState()
        {
            _License = null;
            _DetainedLicense = null;
            _ApplicationFees = 0;

            lblDetainID.Text             = "[???]";
            lblLicenseIDValue.Text       = "[???]";
            lblDetainDateValue.Text      = "[??/??/????]";
            lblCreatedByValue.Text       = "[????]";
            lblApplicationFeesValue.Text = "[$$$$]";
            lblFineFeesValue.Text        = "[$$$$]";
            lblTotalFeesValue.Text       = "[$$$$]";
            lblApplicationIDValue.Text   = "[????]";

            lnkShowLicensesHistory.Enabled = false;
            lnkShowLicenseInfo.Enabled     = false;

            btnRelease.Enabled = false;
        }

        // In locked mode there is no filter left for the user to try a
        // different ID with, so a validation failure means "this dialog
        // cannot do anything useful" - close it rather than leaving a dead
        // form open. In search mode this does nothing; the filter stays live
        // and the user can just search again.
        private void _CloseIfLocked()
        {
            if (_presetLicenseID <= 0)
                return;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Single source of truth for the Release button's enabled state.
        private void _UpdateReleaseButtonState()
        {
            btnRelease.Enabled =
                _License != null &&
                _DetainedLicense != null &&
                !_DetainedLicense.IsReleased;
        }

        #endregion

        #region Release Action

        private void _Release(object sender, EventArgs e)
        {
            if (_detained == null) { clsUtil.ShowWarning("Find a detained record first."); return; }
            if (_detained.IsReleased) { clsUtil.ShowWarning("This license is already released."); return; }
            if (!clsUtil.ConfirmDelete("release this detained license")) return;

            clsLicense license = _detained.LicenseInfo;   // already exposed as a lazy property
            if (license == null)
            {
                clsUtil.ShowError("The license associated with this record could not be found.");
                return;
            }

            int applicationID = -1;
            if (license.ReleaseDetained(clsGlobal.CurrentUserID, ref applicationID))
            {
                clsUtil.ShowInfo("License released successfully.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                clsUtil.ShowError("Failed to release license.");
            }
        }

        #endregion
    }
}