using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmDetainLicense : Form
    {
        #region Controls

        private Label lblTitle;

        private ctrlDriverLicenseInfoWithFilter ctrlDriverLicenseInfoWithFilter1;

        private GroupBox gbDetainInfo;
        private Label lblDetainIDTitle, lblDetainID;
        private Label lblLicenseIDTitle, lblLicenseIDValue;
        private Label lblDetainDateTitle, lblDetainDateValue;
        private Label lblCreatedByTitle, lblCreatedByValue;
        private Label lblFineFeesTitle;
        private TextBox txtFineFees;
        private ErrorProvider errorProvider1;

        private LinkLabel lnkShowLicensesHistory;
        private LinkLabel lnkShowLicenseInfo;

        private Button btnDetain;
        private Button btnClose;

        #endregion

        #region State

        // The single license currently loaded through the filter, or null.
        // Every check in this form is based off this object, never off
        // control text alone.
        private clsLicense _License;
        private clsDetainedLicense _NewDetainedLicense;

        #endregion

        public frmDetainLicense()
        {
            _InitializeComponents();
            _SetupEvents();
        }

        #region Build

        private void _InitializeComponents()
        {
            this.Text = "Detain License";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label
            {
                Text = "Detain License",
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(340, 18)
            };

            // Reuse the existing filter + driver-license-info card instead
            // of rebuilding all those labels by hand.
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

            btnDetain = _CreateButton("🌐  Detain", 640, gbDetainInfo.Bottom + 12, Color.FromArgb(0, 140, 60));
            btnDetain.Enabled = false; // nothing loaded yet - nothing to detain

            btnClose = _CreateButton("✖  Close", 770, gbDetainInfo.Bottom + 12, Color.FromArgb(192, 50, 50));

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                ctrlDriverLicenseInfoWithFilter1,
                gbDetainInfo,
                lnkShowLicensesHistory,
                lnkShowLicenseInfo,
                btnDetain,
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
                Size = new Size(850, 150),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            const int leftX = 20, leftValX = 150;
            const int rightX = 430, rightValX = 560;
            const int step = 35;
            int y = 35;

            lblDetainIDTitle = _Bold("Detain ID:", leftX, y);
            lblDetainID = _Value("[???]", leftValX, y, Color.SteelBlue);

            lblLicenseIDTitle = _Bold("License ID:", rightX, y);
            lblLicenseIDValue = _Value("[???]", rightValX, y, Color.Black);
            y += step;

            lblDetainDateTitle = _Bold("Detain Date:", leftX, y);
            lblDetainDateValue = _Value("[???]", leftValX, y, Color.Black);

            lblCreatedByTitle = _Bold("Created By:", rightX, y);
            lblCreatedByValue = _Value("[???]", rightValX, y, Color.Black);
            y += step;

            lblFineFeesTitle = _Bold("Fine Fees:", leftX, y);
            txtFineFees = new TextBox
            {
                Location = new Point(leftValX, y - 3),
                Size = new Size(150, 23),
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };

            errorProvider1 = new ErrorProvider { ContainerControl = this };

            gbDetainInfo.Controls.AddRange(new Control[]
            {
                lblDetainIDTitle,   lblDetainID,
                lblLicenseIDTitle,  lblLicenseIDValue,
                lblDetainDateTitle, lblDetainDateValue,
                lblCreatedByTitle,  lblCreatedByValue,
                lblFineFeesTitle,   txtFineFees
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
            txtFineFees.TextChanged += (s, e) => _UpdateDetainButtonState();

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

            btnDetain.Click += _Detain;
            btnClose.Click += (s, e) => this.Close();
        }

        // Fired by ctrlDriverLicenseInfoWithFilter after a Find. This is the
        // ONLY place _License gets assigned - everything else just reads it.
        private void _LicenseLoaded(object sender, clsLicense license)
        {
            _ResetDetainState();

            if (license == null)
                return; // "not found" message already shown by the control

            if (!license.IsActive)
            {
                clsUtil.ShowWarning(
                    "This license is inactive and cannot be detained.",
                    "Cannot Detain License");
                return;
            }

            if (license.IsDetained)
            {
                clsUtil.ShowWarning(
                    "This license is already detained.",
                    "Cannot Detain License");
                return;
            }

            if (license.DriverInfo == null)
            {
                clsUtil.ShowWarning(
                    "The driver associated with this license could not be found.",
                    "Cannot Detain License");
                return;
            }

            _License = license;

            lblLicenseIDValue.Text  = license.ID.ToString();
            lblDetainDateValue.Text = clsFormat.DateShort(DateTime.Now);
            lblCreatedByValue.Text  = clsGlobal.CurrentUsername;

            lnkShowLicensesHistory.Enabled = true;
            lnkShowLicenseInfo.Enabled     = true;

            txtFineFees.Focus();
            _UpdateDetainButtonState();
        }

        private void _ResetDetainState()
        {
            _License = null;
            _NewDetainedLicense = null;

            lblDetainID.Text        = "[???]";
            lblLicenseIDValue.Text  = "[???]";
            lblDetainDateValue.Text = "[???]";
            lblCreatedByValue.Text  = "[???]";

            txtFineFees.Clear();
            txtFineFees.Enabled = true;
            errorProvider1.Clear();

            lnkShowLicensesHistory.Enabled = false;
            lnkShowLicenseInfo.Enabled     = false;

            btnDetain.Enabled = false;
        }

        // Single source of truth for whether Detain is clickable.
        // Recomputed from current state - never toggled by hand elsewhere.
        private void _UpdateDetainButtonState()
        {
            btnDetain.Enabled =
                _License != null &&
                _License.IsActive &&
                !_License.IsDetained &&
                !string.IsNullOrWhiteSpace(txtFineFees.Text);
        }

        #endregion

        #region Validation

        private bool _ValidateFineFees(out decimal fees)
        {
            fees = 0;
            errorProvider1.SetError(txtFineFees, string.Empty);

            if (string.IsNullOrWhiteSpace(txtFineFees.Text))
            {
                errorProvider1.SetError(txtFineFees, "Fine fees are required.");
                txtFineFees.Focus();
                return false;
            }

            if (!decimal.TryParse(txtFineFees.Text.Trim(), out fees) || fees <= 0)
            {
                errorProvider1.SetError(txtFineFees, "Enter a valid amount greater than zero.");
                txtFineFees.Focus();
                return false;
            }

            return true;
        }

        #endregion

        #region Detain Action

        private void _Detain(object sender, EventArgs e)
        {
            // Re-validate everything, even though the button being enabled
            // already implied all of this. The Enabled state is a UX hint,
            // not a security guarantee - state can drift between the last
            // update and this click (e.g. via the inner filter's own
            // "Clear" button, which does not raise LicenseLoaded).
            if (_License == null)
            {
                clsUtil.ShowWarning("Find a valid license first.");
                return;
            }

            if (!_License.IsActive)
            {
                clsUtil.ShowWarning("This license is inactive and cannot be detained.");
                _UpdateDetainButtonState();
                return;
            }

            if (clsDetainedLicense.IsLicenseDetained(_License.ID))
            {
                clsUtil.ShowWarning("This license is already detained.");
                _UpdateDetainButtonState();
                return;
            }

            if (!_ValidateFineFees(out decimal fineFees))
                return;

            if (!clsUtil.ConfirmDelete("detain this license"))
                return;

            var detained = new clsDetainedLicense
            {
                LicenseID       = _License.ID,
                DetainDate      = DateTime.Now,
                FineFees        = fineFees,
                CreatedByUserID = clsGlobal.CurrentUserID
            };

            if (detained.Save())
            {
                _NewDetainedLicense = detained;

                lblDetainID.Text = detained.ID.ToString();

                clsUtil.ShowSuccess("License detained successfully.", "Detained");

                // Lock the form after success - a license can't be detained
                // twice, and the fee just recorded is now permanent.
                ctrlDriverLicenseInfoWithFilter1.EnableFilter(false);
                txtFineFees.Enabled = false;
                btnDetain.Enabled = false;

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                clsUtil.ShowError("Failed to detain the license. Please try again.");
            }
        }

        #endregion
    }
}