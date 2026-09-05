using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmRenewLocalDrivingLicenseApplication : Form
    {
        #region Controls

        private Label lblTitle;

        private ctrlDriverLicenseInfoWithFilter ctrlDriverLicenseInfoWithFilter1;

        private GroupBox gbApplicationNewLicenseInfo;

        private Label lblRLApplicationIDTitle;
        private Label lblRLApplicationID;

        private Label lblApplicationDateTitle;
        private Label lblApplicationDate;

        private Label lblIssueDateTitle;
        private Label lblIssueDate;

        private Label lblApplicationFeesTitle;
        private Label lblApplicationFees;

        private Label lblLicenseFeesTitle;
        private Label lblLicenseFees;

        private Label lblRenewedLicenseIDTitle;
        private Label lblRenewedLicenseID;

        private Label lblOldLicenseIDTitle;
        private Label lblOldLicenseID;

        private Label lblExpirationDateTitle;
        private Label lblExpirationDate;

        private Label lblCreatedByTitle;
        private Label lblCreatedBy;

        private Label lblTotalFeesTitle;
        private Label lblTotalFees;

        private Label lblNotesTitle;
        private TextBox txtNotes;

        private LinkLabel lnkShowLicensesHistory;
        private LinkLabel lnkShowNewLicenseInfo;

        private Button btnRenew;
        private Button btnClose;

        #endregion

        #region Data

        private clsLicense _OldLicense;
        private clsLicense _NewLicense;

        private int _RenewApplicationID = -1;

        private DateTime _ApplicationDate;
        private DateTime _IssueDate;
        private DateTime _ExpirationDate;

        private decimal _ApplicationFees;
        private decimal _LicenseFees;

        #endregion

        #region Constructor

        public frmRenewLocalDrivingLicenseApplication()
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

            this.Text = "Renew Local Driving License";
            this.Size = new Size(940, 875);
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
                Text = "Renew Local Driving License",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(285, 18)
            };

            // =========================================================
            // Driver License Filter + Information
            // =========================================================

            ctrlDriverLicenseInfoWithFilter1 =
                new ctrlDriverLicenseInfoWithFilter
                {
                    Location = new Point(30, 58),
                    Size = new Size(850, 430)
                };

            // =========================================================
            // Application New License Info
            // =========================================================

            gbApplicationNewLicenseInfo = new GroupBox
            {
                Text = "Application New License Info",
                Location = new Point(30, 500),
                Size = new Size(850, 180),
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold)
            };

            // -------------------------
            // Left Column
            // -------------------------

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "RL Application ID:",
                20,
                32,
                out lblRLApplicationIDTitle,
                out lblRLApplicationID);

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "Application Date:",
                20,
                62,
                out lblApplicationDateTitle,
                out lblApplicationDate);

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "Issue Date:",
                20,
                92,
                out lblIssueDateTitle,
                out lblIssueDate);

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "Application Fees:",
                20,
                122,
                out lblApplicationFeesTitle,
                out lblApplicationFees);

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "License Fees:",
                20,
                152,
                out lblLicenseFeesTitle,
                out lblLicenseFees);

            // -------------------------
            // Right Column
            // -------------------------

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "Renewed License ID:",
                430,
                32,
                out lblRenewedLicenseIDTitle,
                out lblRenewedLicenseID);

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "Old License ID:",
                430,
                62,
                out lblOldLicenseIDTitle,
                out lblOldLicenseID);

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "Expiration Date:",
                430,
                92,
                out lblExpirationDateTitle,
                out lblExpirationDate);

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "Created By:",
                430,
                122,
                out lblCreatedByTitle,
                out lblCreatedBy);

            _AddInfoLabel(
                gbApplicationNewLicenseInfo,
                "Total Fees:",
                430,
                152,
                out lblTotalFeesTitle,
                out lblTotalFees);

            // =========================================================
            // Notes
            // =========================================================

            lblNotesTitle = new Label
            {
                Text = "Notes:",
                AutoSize = true,
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold),
                Location = new Point(30, 695)
            };

            txtNotes = new TextBox
            {
                Location = new Point(30, 720),
                Size = new Size(850, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                MaxLength = 500,
                Enabled = false
            };

            // =========================================================
            // Buttons
            // =========================================================

            btnRenew = _CreateButton(
                "Renew",
                772,
                790,
                clsGlobal.PrimaryRed);

            btnRenew.Enabled = false;

            btnClose = _CreateButton(
                "Close",
                650,
                790,
                Color.FromArgb(110, 110, 110));

            // =========================================================
            // Links
            // =========================================================

            lnkShowLicensesHistory = new LinkLabel
            {
                Text = "Show Licenses History ",
                AutoSize = true,
                Location = new Point(30, 800),
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
                Location = new Point(180, 800),
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

                    ctrlDriverLicenseInfoWithFilter1,

                    gbApplicationNewLicenseInfo,

                    lblNotesTitle,
                    txtNotes,

                    btnRenew,
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
                Size = new Size(110, 35),
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
            _IssueDate = DateTime.Now;

            clsApplicationType applicationType =
                clsApplicationType.Find(2);

            _ApplicationFees =
                applicationType == null
                    ? 0
                    : applicationType.Fees;

            lblRLApplicationID.Text =
                "[Will be created after renewal]";

            lblApplicationDate.Text =
                clsFormat.DateShort(_ApplicationDate);

            lblIssueDate.Text =
                clsFormat.DateShort(_IssueDate);

            lblApplicationFees.Text =
                _ApplicationFees.ToString("0.00");

            lblRenewedLicenseID.Text =
                "[Will be created after renewal]";

            lblOldLicenseID.Text = "[???]";

            lblExpirationDate.Text = "[???]";

            lblCreatedBy.Text =
                clsGlobal.CurrentUsername;

            lblLicenseFees.Text = "[???]";

            lblTotalFees.Text = "[???]";
        }

        #endregion

        #region Events

        private void _SetupEvents()
        {
            ctrlDriverLicenseInfoWithFilter1.LicenseLoaded +=
                _LicenseLoaded;

            btnRenew.Click +=
                _Renew;

            btnClose.Click +=
                _Close;

            lnkShowLicensesHistory.LinkClicked +=
                _ShowLicensesHistory;

            lnkShowNewLicenseInfo.LinkClicked +=
                _ShowNewLicenseInfo;
        }

        #endregion

        #region License Validation

        private void _LicenseLoaded(
            object sender,
            clsLicense license)
        {
            _ResetRenewalState();

            // ---------------------------------------------------------
            // No valid license
            // ---------------------------------------------------------

            if (license == null)
                return;

            // ---------------------------------------------------------
            // License must be active
            // ---------------------------------------------------------

            if (!license.IsActive)
            {
                clsUtil.ShowWarning(
                    "This license is inactive and cannot be renewed.",
                    "Cannot Renew License");

                return;
            }

            // ---------------------------------------------------------
            // License must be expired
            // ---------------------------------------------------------

            if (!license.IsExpired())
            {
                clsUtil.ShowWarning(
                    "You cannot renew this license because it has not expired yet.",
                    "Cannot Renew License");

                return;
            }

            // ---------------------------------------------------------
            // Valid license
            // ---------------------------------------------------------

            _OldLicense = license;

            _ExpirationDate =
                DateTime.Now.AddYears(
                    license.LicenseClassInfo
                        .DefaultValidityLength);

            _LicenseFees =
                license.LicenseClassInfo.Fees;

            // ---------------------------------------------------------
            // Fill information
            // ---------------------------------------------------------

            lblOldLicenseID.Text =
                license.ID.ToString();

            lblExpirationDate.Text =
                clsFormat.DateShort(_ExpirationDate);

            lblLicenseFees.Text =
                _LicenseFees.ToString("0.00");

            lblTotalFees.Text =
                (_ApplicationFees + _LicenseFees)
                .ToString("0.00");

            // ---------------------------------------------------------
            // Notes are copied from old license
            // ---------------------------------------------------------

            txtNotes.Enabled = true;

            txtNotes.Text =
                license.Notes ?? string.Empty;

            // ---------------------------------------------------------
            // Enable actions
            // ---------------------------------------------------------

            btnRenew.Enabled = true;

            // History is available only when
            // we have a valid license.
            lnkShowLicensesHistory.Enabled = true;
        }

        #endregion

        #region Reset

        private void _ResetRenewalState()
        {
            _OldLicense = null;
            _NewLicense = null;

            _RenewApplicationID = -1;

            _LicenseFees = 0;

            _ExpirationDate =
                DateTime.MinValue;

            // ---------------------------------------------------------
            // Reset displayed information
            // ---------------------------------------------------------

            lblRenewedLicenseID.Text =
                "[Will be created after renewal]";

            lblOldLicenseID.Text =
                "[???]";

            lblExpirationDate.Text =
                "[???]";

            lblLicenseFees.Text =
                "[???]";

            lblTotalFees.Text =
                "[???]";

            // ---------------------------------------------------------
            // Disable controls
            // ---------------------------------------------------------

            txtNotes.Enabled = false;
            txtNotes.Text = string.Empty;

            btnRenew.Enabled = false;

            // IMPORTANT:
            // No valid license = no license history.
            lnkShowLicensesHistory.Enabled = false;

            // New license doesn't exist yet.
            lnkShowNewLicenseInfo.Enabled = false;
        }

        #endregion

        #region Renewal

        private void _Renew(
            object sender,
            EventArgs e)
        {
            // ---------------------------------------------------------
            // Defensive validation
            // ---------------------------------------------------------

            if (_OldLicense == null)
            {
                clsUtil.ShowWarning(
                    "Find a valid expired license first.",
                    "Cannot Renew License");

                return;
            }

            // ---------------------------------------------------------
            // Active validation
            // ---------------------------------------------------------

            if (!_OldLicense.IsActive)
            {
                clsUtil.ShowWarning(
                    "This license is inactive and cannot be renewed.",
                    "Cannot Renew License");

                btnRenew.Enabled = false;

                return;
            }

            // ---------------------------------------------------------
            // Expiration validation
            // ---------------------------------------------------------

            if (!_OldLicense.IsExpired())
            {
                clsUtil.ShowWarning(
                    "You cannot renew this license because it has not expired yet.",
                    "Cannot Renew License");

                btnRenew.Enabled = false;

                return;
            }

            // ---------------------------------------------------------
            // Renew through Business layer
            // ---------------------------------------------------------

            clsLicense newLicense =
                _OldLicense.Renew(
                    txtNotes.Text.Trim(),
                    clsGlobal.CurrentUserID);

            if (newLicense == null)
            {
                clsUtil.ShowError(
                    "Failed to renew the driving license.",
                    "Renew License");

                return;
            }

            // ---------------------------------------------------------
            // Renewal succeeded
            // ---------------------------------------------------------

            _NewLicense = newLicense;

            _RenewApplicationID =
                newLicense.ApplicationID;

            _IssueDate =
                newLicense.IssueDate;

            _ExpirationDate =
                newLicense.ExpirationDate;

            // ---------------------------------------------------------
            // Update UI
            // ---------------------------------------------------------

            lblRLApplicationID.Text =
                _RenewApplicationID.ToString();

            lblIssueDate.Text =
                clsFormat.DateShort(_IssueDate);

            lblRenewedLicenseID.Text =
                _NewLicense.ID.ToString();

            lblExpirationDate.Text =
                clsFormat.DateShort(_ExpirationDate);

            lblTotalFees.Text =
                (_ApplicationFees + _LicenseFees)
                .ToString("0.00");

            // ---------------------------------------------------------
            // Lock editing after successful renewal
            // ---------------------------------------------------------

            txtNotes.Enabled = false;

            btnRenew.Enabled = false;

            // New license now exists.
            lnkShowNewLicenseInfo.Enabled = true;

            clsUtil.ShowInfo(
                "The driving license has been renewed successfully.\n\n" +
                "New License ID = " +
                _NewLicense.ID,
                "License Renewed");
        }

        #endregion

        #region Links

        private void _ShowLicensesHistory(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            // No valid license = nothing to show.
            if (_OldLicense == null)
                return;

            // Put your existing license-history form here.
            //
            // Example:
            //
            // using (frmShowLicenseHistory frm =
            //        new frmShowLicenseHistory(_OldLicense.DriverID))
            // {
            //     frm.ShowDialog();
            // }

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
            // New license only exists after successful renewal.
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