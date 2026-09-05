using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmReplaceLostOrDamagedLicense : Form
    {
        #region Controls
        private Label lblTitle;

        private RadioButton rbLostLicense;
        private RadioButton rbDamagedLicense;

        private ctrlDriverLicenseInfoWithFilter ctrlDriverLicenseInfoWithFilter1;

        private GroupBox gbReplacementApplicationInfo;

        private Label lblRLApplicationIDTitle;
        private Label lblRLApplicationID;

        private Label lblApplicationDateTitle;
        private Label lblApplicationDate;

        private Label lblApplicationFeesTitle;
        private Label lblApplicationFees;

        private Label lblReplacedLicenseIDTitle;
        private Label lblReplacedLicenseID;

        private Label lblOldLicenseIDTitle;
        private Label lblOldLicenseID;

        private Label lblCreatedByTitle;
        private Label lblCreatedBy;

        private LinkLabel lnkShowLicensesHistory;
        private LinkLabel lnkShowNewLicenseInfo;

        private Button btnIssueReplacement;
        private Button btnClose;

        #endregion

        #region Data

        private clsLicense _OldLicense;
        private clsLicense _NewLicense;

        private int _ReplacementApplicationID = -1;

        private DateTime _ApplicationDate;

        private decimal _ApplicationFees;

        private clsLicense.enIssueReason _ReplacementReason =
            clsLicense.enIssueReason.LostReplacement;

        #endregion

        #region Constructor

        public frmReplaceLostOrDamagedLicense()
        {
            InitializeComponents();

            _InitializeApplicationInfo();

            _SetupEvents();
        }

        #endregion

        #region Form Initialization

        private void InitializeComponents()
        {
            // =========================================================
            // Form
            // =========================================================

            this.Text = "Replace Lost or Damaged Driving License";
            this.Size = new Size(940, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            // =========================================================
            // Title
            // =========================================================

            lblTitle = new Label
            {
                Text = "Replace Lost or Damaged Driving License",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(220, 15)
            };

            // =========================================================
            // Replacement Reason
            // =========================================================

            GroupBox gbReplacementReason = new GroupBox
            {
                Text = "Replacement Reason",
                Location = new Point(30, 55),
                Size = new Size(850, 65),
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold)
            };

            rbLostLicense = new RadioButton
            {
                Text = "Lost License",
                Location = new Point(25, 27),
                AutoSize = true,
                Checked = true,
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F)
            };

            rbDamagedLicense = new RadioButton
            {
                Text = "Damaged License",
                Location = new Point(180, 27),
                AutoSize = true,
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F)
            };

            gbReplacementReason.Controls.AddRange(
                new Control[]
                {
                    rbLostLicense,
                    rbDamagedLicense
                });

            // =========================================================
            // Driver License Filter + Information
            // =========================================================

            ctrlDriverLicenseInfoWithFilter1 =
                new ctrlDriverLicenseInfoWithFilter
                {
                    Location = new Point(30, 130),
                    Size = new Size(850, 430)
                };

            // =========================================================
            // Replacement Application Information
            // =========================================================

            gbReplacementApplicationInfo = new GroupBox
            {
                Text = "Replacement Application Info",
                Location = new Point(30, 565),
                Size = new Size(850, 125),
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold)
            };

            // ---------------------------------------------------------
            // Left Column
            // ---------------------------------------------------------

            _AddInfoLabel(
                gbReplacementApplicationInfo,
                "RL Application ID:",
                20,
                30,
                out lblRLApplicationIDTitle,
                out lblRLApplicationID);

            _AddInfoLabel(
                gbReplacementApplicationInfo,
                "Application Date:",
                20,
                60,
                out lblApplicationDateTitle,
                out lblApplicationDate);

            _AddInfoLabel(
                gbReplacementApplicationInfo,
                "Application Fees:",
                20,
                90,
                out lblApplicationFeesTitle,
                out lblApplicationFees);

            // ---------------------------------------------------------
            // Right Column
            // ---------------------------------------------------------

            _AddInfoLabel(
                gbReplacementApplicationInfo,
                "Replaced License ID:",
                430,
                30,
                out lblReplacedLicenseIDTitle,
                out lblReplacedLicenseID);

            _AddInfoLabel(
                gbReplacementApplicationInfo,
                "Old License ID:",
                430,
                60,
                out lblOldLicenseIDTitle,
                out lblOldLicenseID);

            _AddInfoLabel(
                gbReplacementApplicationInfo,
                "Created By:",
                430,
                90,
                out lblCreatedByTitle,
                out lblCreatedBy);

            // =========================================================
            // Buttons
            // =========================================================

            btnIssueReplacement = _CreateButton(
                "Issue Replacement",
                755,
                705,
                clsGlobal.PrimaryRed);

            btnIssueReplacement.Enabled = false;

            btnClose = _CreateButton(
                "Close",
                630,
                705,
                Color.FromArgb(110, 110, 110));

            // =========================================================
            // Links
            // =========================================================

            lnkShowLicensesHistory = new LinkLabel
            {
                Text = "Show Licenses History ",
                AutoSize = true,
                Location = new Point(30, 715),
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Underline),
                Enabled = false,
                TabStop = false
            };

            lnkShowNewLicenseInfo = new LinkLabel
            {
                Text = "Show New License Info ",
                AutoSize = true,
                Location = new Point(190, 715),
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Underline),
                Enabled = false,
                TabStop = false
            };

            // =========================================================
            // Add Controls
            // =========================================================

            this.Controls.AddRange(
                new Control[]
                {
                    lblTitle,

                    gbReplacementReason,

                    ctrlDriverLicenseInfoWithFilter1,

                    gbReplacementApplicationInfo,

                    btnIssueReplacement,
                    btnClose,

                    lnkShowLicensesHistory,
                    lnkShowNewLicenseInfo
                });
        }

        #endregion

        #region UI Helpers

        private static void _AddInfoLabel(
            Control parent,
            string title,
            int x,
            int y,
            out Label titleLabel,
            out Label valueLabel)
        {
            titleLabel = new Label
            {
                Text = title,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font(
                    "Microsoft Sans Serif",
                    9F,
                    FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 80)
            };

            valueLabel = new Label
            {
                Text = "[???]",
                Location = new Point(x + 145, y),
                AutoSize = true,
                Font = new Font(
                    "Microsoft Sans Serif",
                    9F),
                ForeColor = Color.FromArgb(30, 80, 160)
            };

            parent.Controls.Add(titleLabel);
            parent.Controls.Add(valueLabel);
        }

        private static Button _CreateButton(
            string text,
            int x,
            int y,
            Color backColor)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(115, 35),
                Font = new Font(
                    "Microsoft Sans Serif",
                    10F,
                    FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;

            return button;
        }

        #endregion

        #region Initialization

        private void _InitializeApplicationInfo()
        {
            _ApplicationDate = DateTime.Now;

            _UpdateApplicationFees();

            lblRLApplicationID.Text =
                "[Will be created after replacement]";

            lblApplicationDate.Text =
                clsFormat.DateShort(_ApplicationDate);

            lblReplacedLicenseID.Text =
                "[Will be created after replacement]";

            lblOldLicenseID.Text =
                "[???]";

            lblCreatedBy.Text =
                clsGlobal.CurrentUsername;
        }

        private void _UpdateApplicationFees()
        {
            int applicationTypeID =
                (_ReplacementReason ==
                    clsLicense.enIssueReason.LostReplacement)
                    ? 3
                    : 4;

            clsApplicationType applicationType =
                clsApplicationType.Find(applicationTypeID);

            _ApplicationFees =
                applicationType == null
                    ? 0
                    : applicationType.Fees;

            lblApplicationFees.Text =
                _ApplicationFees.ToString("0.00");
        }

        #endregion

        #region Events

        private void _SetupEvents()
        {
            ctrlDriverLicenseInfoWithFilter1.LicenseLoaded +=
                _LicenseLoaded;

            rbLostLicense.CheckedChanged +=
                _ReplacementReasonChanged;

            rbDamagedLicense.CheckedChanged +=
                _ReplacementReasonChanged;

            btnIssueReplacement.Click +=
                _IssueReplacement;

            btnClose.Click +=
                _Close;

            lnkShowLicensesHistory.LinkClicked +=
                _ShowLicensesHistory;

            lnkShowNewLicenseInfo.LinkClicked +=
                _ShowNewLicenseInfo;
        }

        #endregion

        #region Replacement Reason

        private void _ReplacementReasonChanged(
            object sender,
            EventArgs e)
        {
            // Do nothing while the form is locked
            // after a successful replacement.
            if (!btnIssueReplacement.Enabled &&
                _NewLicense != null)
            {
                return;
            }

            if (rbLostLicense.Checked)
            {
                _ReplacementReason =
                    clsLicense.enIssueReason.LostReplacement;
            }
            else if (rbDamagedLicense.Checked)
            {
                _ReplacementReason =
                    clsLicense.enIssueReason.DamagedReplacement;
            }

            _UpdateApplicationFees();
        }

        #endregion

        #region License Validation

        private void _LicenseLoaded(
            object sender,
            clsLicense license)
        {
            _ResetReplacementState();

            // ---------------------------------------------------------
            // License not found
            // ---------------------------------------------------------

            if (license == null)
                return;

            // ---------------------------------------------------------
            // License must be active
            // ---------------------------------------------------------

            if (!license.IsActive)
            {
                clsUtil.ShowWarning(
                    "This license is inactive and cannot be replaced.",
                    "Cannot Replace License");

                return;
            }

            // ---------------------------------------------------------
            // Driver must exist
            // ---------------------------------------------------------

            if (license.DriverInfo == null)
            {
                clsUtil.ShowWarning(
                    "The driver associated with this license could not be found.",
                    "Cannot Replace License");

                return;
            }

            // ---------------------------------------------------------
            // License class must exist
            // ---------------------------------------------------------

            if (license.LicenseClassInfo == null)
            {
                clsUtil.ShowWarning(
                    "The license class associated with this license could not be found.",
                    "Cannot Replace License");

                return;
            }

            // ---------------------------------------------------------
            // Application type must exist
            // ---------------------------------------------------------

            int applicationTypeID =
                (_ReplacementReason ==
                    clsLicense.enIssueReason.LostReplacement)
                    ? 3
                    : 4;

            if (clsApplicationType.Find(applicationTypeID) == null)
            {
                clsUtil.ShowError(
                    "The replacement application type is not configured.",
                    "Cannot Replace License");

                return;
            }

            // ---------------------------------------------------------
            // Valid license
            // ---------------------------------------------------------

            _OldLicense = license;

            // ---------------------------------------------------------
            // Fill information
            // ---------------------------------------------------------

            lblOldLicenseID.Text =
                license.ID.ToString();

            // ---------------------------------------------------------
            // Enable actions
            // ---------------------------------------------------------

            btnIssueReplacement.Enabled = true;

            lnkShowLicensesHistory.Enabled = true;
        }

        #endregion

        #region Reset

        private void _ResetReplacementState()
        {
            _OldLicense = null;
            _NewLicense = null;

            _ReplacementApplicationID = -1;

            // ---------------------------------------------------------
            // Reset displayed information
            // ---------------------------------------------------------

            lblRLApplicationID.Text =
                "[Will be created after replacement]";

            lblReplacedLicenseID.Text =
                "[Will be created after replacement]";

            lblOldLicenseID.Text =
                "[???]";

            // ---------------------------------------------------------
            // Disable controls
            // ---------------------------------------------------------

            btnIssueReplacement.Enabled = false;

            lnkShowLicensesHistory.Enabled = false;
            lnkShowNewLicenseInfo.Enabled = false;
        }

        #endregion

        #region Issue Replacement

        private void _IssueReplacement(
            object sender,
            EventArgs e)
        {
            // ---------------------------------------------------------
            // Defensive validation
            // ---------------------------------------------------------

            if (_OldLicense == null)
            {
                clsUtil.ShowWarning(
                    "Find a valid active license first.",
                    "Cannot Replace License");

                return;
            }

            // ---------------------------------------------------------
            // Active validation
            // ---------------------------------------------------------

            if (!_OldLicense.IsActive)
            {
                clsUtil.ShowWarning(
                    "This license is inactive and cannot be replaced.",
                    "Cannot Replace License");

                btnIssueReplacement.Enabled = false;

                return;
            }

            // ---------------------------------------------------------
            // Driver validation
            // ---------------------------------------------------------

            if (_OldLicense.DriverInfo == null)
            {
                clsUtil.ShowWarning(
                    "The driver associated with this license could not be found.",
                    "Cannot Replace License");

                btnIssueReplacement.Enabled = false;

                return;
            }

            // ---------------------------------------------------------
            // License class validation
            // ---------------------------------------------------------

            if (_OldLicense.LicenseClassInfo == null)
            {
                clsUtil.ShowWarning(
                    "The license class associated with this license could not be found.",
                    "Cannot Replace License");

                btnIssueReplacement.Enabled = false;

                return;
            }

            // ---------------------------------------------------------
            // Determine application type
            // ---------------------------------------------------------

            int applicationTypeID =
                (_ReplacementReason ==
                    clsLicense.enIssueReason.LostReplacement)
                    ? 3
                    : 4;

            clsApplicationType applicationType =
                clsApplicationType.Find(applicationTypeID);

            if (applicationType == null)
            {
                clsUtil.ShowError(
                    "The replacement application type is not configured.",
                    "Cannot Replace License");

                return;
            }

            // ---------------------------------------------------------
            // Replace through Business layer
            // ---------------------------------------------------------

            clsLicense newLicense =
                _OldLicense.Replace(
                    _ReplacementReason,
                    clsGlobal.CurrentUserID);

            if (newLicense == null)
            {
                clsUtil.ShowError(
                    "Failed to issue the replacement driving license.",
                    "Replace License");

                return;
            }

            // ---------------------------------------------------------
            // Replacement succeeded
            // ---------------------------------------------------------

            _NewLicense = newLicense;

            _ReplacementApplicationID =
                newLicense.ApplicationID;

            _ApplicationDate =
                newLicense.ApplicationInfo == null
                    ? DateTime.Now
                    : newLicense.ApplicationInfo.ApplicationDate;

            // ---------------------------------------------------------
            // Update UI
            // ---------------------------------------------------------

            lblRLApplicationID.Text =
                _ReplacementApplicationID.ToString();

            lblApplicationDate.Text =
                clsFormat.DateShort(_ApplicationDate);

            lblReplacedLicenseID.Text =
                _NewLicense.ID.ToString();

            lblOldLicenseID.Text =
                _OldLicense.ID.ToString();

            lblCreatedBy.Text =
                clsGlobal.CurrentUsername;

            // ---------------------------------------------------------
            // IMPORTANT:
            // Lock the form after successful replacement.
            //
            // The old license has now been deactivated by the
            // Business layer, and the new license is active.
            // ---------------------------------------------------------

            btnIssueReplacement.Enabled = false;

            rbLostLicense.Enabled = false;
            rbDamagedLicense.Enabled = false;

            ctrlDriverLicenseInfoWithFilter1.EnableFilter(false);

            lnkShowNewLicenseInfo.Enabled = true;

            clsUtil.ShowInfo(
                "The driving license has been replaced successfully.\n\n" +
                "New License ID = " +
                _NewLicense.ID,
                "License Replaced");
        }

        #endregion

        #region Links

        private void _ShowLicensesHistory(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            if (_OldLicense == null)
                return;

            // Keep this exactly like the Renew form until
            // your existing License History form is connected.

            MessageBox.Show(
                "The License History form is not available yet.",
                "License History",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void _ShowNewLicenseInfo(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            if (_NewLicense == null)
                return;

            using (frmShowLicenseInfo frm =
                new frmShowLicenseInfo(_NewLicense.ID))
            {
                frm.ShowDialog();
            }
        }

        #endregion

        #region Close

        private void _Close(
            object sender,
            EventArgs e)
        {
            this.Close();
        }

        #endregion

    }

}