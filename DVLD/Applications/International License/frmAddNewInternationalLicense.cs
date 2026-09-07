using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmAddNewInternationalLicense : Form
    {
        #region Controls

        private Label lblTitle;

        private Label lblFilterTitle;
        private TextBox txtLicenseID;
        private Button btnFind;

        private ctrlDriverLicenseInfo ctrlDriverLicenseInfo1;

        private GroupBox gbApplicationInfo;
        private Label lblILAppIDTitle, lblILAppID;
        private Label lblAppDateTitle, lblAppDate;
        private Label lblILLicenseIDTitle, lblILLicenseID;
        private Label lblLocalLicenseIDTitle, lblLocalLicenseID;
        private Label lblIssueDateTitle, lblIssueDate;
        private Label lblExpirationDateTitle, lblExpirationDate;
        private Label lblFeesTitle, lblFees;
        private Label lblCreatedByTitle, lblCreatedBy;

        private LinkLabel lnkShowLicenseInfo;
        private LinkLabel lnkShowLicensesHistory;

        private Button btnIssue;
        private Button btnClose;

        #endregion

        #region State

        private clsLicense _localLicense;
        private clsInternationalLicense _issuedLicense;

        #endregion

        public frmAddNewInternationalLicense()
        {
            _Build();
        }

        #region Build

        private void _Build()
        {
            this.Text = "International License Application";
            this.Size = new Size(950, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label
            {
                Text = "International License Application",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(220, 18)
            };

            // ── Filter ──────────────────────────────────────────────
            lblFilterTitle = new Label
            {
                Text = "Local License ID:",
                AutoSize = true,
                Location = new Point(30, 65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            txtLicenseID = new TextBox
            {
                Location = new Point(170, 62),
                Size = new Size(130, 23)
            };
            txtLicenseID.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            btnFind = new Button
            {
                Text = "🔍 Find",
                Location = new Point(310, 61),
                Size = new Size(80, 26),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnFind.FlatAppearance.BorderSize = 0;
            btnFind.Click += _Find;

            // ── Driver License Info (reused control) ────────────────
            ctrlDriverLicenseInfo1 = new ctrlDriverLicenseInfo
            {
                Location = new Point(20, 100)
            };

            // ── Application Info ─────────────────────────────────────
            gbApplicationInfo = new GroupBox
            {
                Text = "Application Info",
                Location = new Point(20, ctrlDriverLicenseInfo1.Bottom + 15),
                Size = new Size(890, 170),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            int y1 = 30, y2 = 65, y3 = 100, y4 = 135;
            const int lx1 = 20, vx1 = 170, lx2 = 460, vx2 = 610;

            _AddInfoLabel(gbApplicationInfo, "I.L.Application ID:", lx1, vx1, y1, out lblILAppIDTitle, out lblILAppID);
            _AddInfoLabel(gbApplicationInfo, "Application Date:",   lx1, vx1, y2, out lblAppDateTitle, out lblAppDate);
            _AddInfoLabel(gbApplicationInfo, "I.L.License ID:",     lx1, vx1, y3, out lblILLicenseIDTitle, out lblILLicenseID);
            _AddInfoLabel(gbApplicationInfo, "Local License ID:",   lx1, vx1, y4, out lblLocalLicenseIDTitle, out lblLocalLicenseID);

            _AddInfoLabel(gbApplicationInfo, "Issue Date:",         lx2, vx2, y1, out lblIssueDateTitle, out lblIssueDate);
            _AddInfoLabel(gbApplicationInfo, "Expiration Date:",    lx2, vx2, y2, out lblExpirationDateTitle, out lblExpirationDate);
            _AddInfoLabel(gbApplicationInfo, "Fees:",               lx2, vx2, y3, out lblFeesTitle, out lblFees);
            _AddInfoLabel(gbApplicationInfo, "Created By:",         lx2, vx2, y4, out lblCreatedByTitle, out lblCreatedBy);

            // ── Links ─────────────────────────────────────────────────
            lnkShowLicenseInfo = new LinkLabel
            {
                Text = "Show License Info",
                AutoSize = true,
                Location = new Point(20, gbApplicationInfo.Bottom + 15),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Underline),
                LinkColor = Color.SteelBlue,
                Enabled = false,
                TabStop = false
            };

            lnkShowLicensesHistory = new LinkLabel
            {
                Text = "Show Licenses History",
                AutoSize = true,
                Location = new Point(180, gbApplicationInfo.Bottom + 15),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Underline),
                LinkColor = Color.SteelBlue,
                Enabled = false,
                TabStop = false
            };

            // ── Buttons ───────────────────────────────────────────────
            btnIssue = _Btn("Issue", 620, gbApplicationInfo.Bottom + 10, Color.FromArgb(0, 140, 60));
            btnIssue.Enabled = false;
            btnIssue.Click += _Issue;

            btnClose = _Btn("✖  Close", 750, gbApplicationInfo.Bottom + 10, Color.FromArgb(192, 50, 50));
            btnClose.Click += (s, e) => this.Close();

            lnkShowLicenseInfo.LinkClicked += (s, e) =>
            {
                if (_issuedLicense == null) return;
                new frmShowInternationalLicenseInfo(_issuedLicense.InternationalLicenseID).ShowDialog();
            };

            lnkShowLicensesHistory.LinkClicked += (s, e) =>
            {
                if (_localLicense?.DriverInfo == null) return;
                new frmShowPersonLicenseHistory(_localLicense.DriverInfo.PersonID).ShowDialog();
            };

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblFilterTitle, txtLicenseID, btnFind,
                ctrlDriverLicenseInfo1,
                gbApplicationInfo,
                lnkShowLicenseInfo, lnkShowLicensesHistory,
                btnIssue, btnClose
            });
        }

        private static void _AddInfoLabel(Control parent, string title, int lx, int vx, int y,
            out Label titleLabel, out Label valueLabel)
        {
            titleLabel = new Label
            {
                Text = title,
                Location = new Point(lx, y),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 80)
            };
            valueLabel = new Label
            {
                Text = "[???]",
                Location = new Point(vx, y),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9F),
                ForeColor = Color.FromArgb(30, 80, 160)
            };
            parent.Controls.Add(titleLabel);
            parent.Controls.Add(valueLabel);
        }

        private static Button _Btn(string text, int x, int y, Color back)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(110, 34),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        #endregion

        #region Find

        private void _Find(object sender, EventArgs e)
        {
            _ResetApplicationPreview();
            btnIssue.Enabled = false;
            lnkShowLicenseInfo.Enabled = false;
            lnkShowLicensesHistory.Enabled = false;
            _issuedLicense = null;

            if (!int.TryParse(txtLicenseID.Text, out int id))
            {
                clsUtil.ShowWarning("Enter a valid License ID.");
                return;
            }

            _localLicense = clsLicense.Find(id);
            if (_localLicense == null)
            {
                clsUtil.ShowWarning("License not found.", "Not Found");
                ctrlDriverLicenseInfo1.ResetLicenseInfo();
                return;
            }

            ctrlDriverLicenseInfo1.LoadLicenseInfo(_localLicense);

            // Client-side pre-checks purely so the user gets fast feedback
            // and doesn't waste a click on "Issue" — clsInternationalLicense
            // .IssueNew() re-validates every one of these itself, since the
            // form is never the last line of defense.
            if (!_localLicense.IsActive)
            {
                clsUtil.ShowWarning("This license is not active."); return;
            }
            if (_localLicense.IsExpired())
            {
                clsUtil.ShowWarning("This license has expired."); return;
            }
            if (_localLicense.IsDetained)
            {
                clsUtil.ShowWarning("This license is currently detained."); return;
            }
            if (_localLicense.LicenseClassID != clsInternationalLicense.EligibleLicenseClassID_Ordinary)
            {
                clsUtil.ShowWarning(
                    "Only an ordinary (Class 3) driving license is eligible for an international license.\n\n" +
                    $"This license is: {_localLicense.LicenseClassInfo?.Name ?? "[Unknown class]"}",
                    "Not Eligible");
                return;
            }
            if (clsInternationalLicense.IsDriverHaveActiveInternationalLicense(_localLicense.DriverID))
            {
                clsUtil.ShowWarning("This driver already has an active international license.");
                return;
            }
            if (clsInternationalLicense.IsLicenseIssuedAsInternational(_localLicense.ID))
            {
                clsUtil.ShowWarning("An international license was already issued using this local license.");
                return;
            }

            // All checks passed — show a live preview of what will be created.
            clsApplicationType appType =
                clsApplicationType.Find(clsInternationalLicense.ApplicationTypeID_NewInternational);

            lblILAppID.Text        = "[Will be created after issuing]";
            lblAppDate.Text        = clsFormat.DateShort(DateTime.Now);
            lblILLicenseID.Text    = "[Will be created after issuing]";
            lblLocalLicenseID.Text = _localLicense.ID.ToString();
            lblIssueDate.Text      = clsFormat.DateShort(DateTime.Now);
            lblExpirationDate.Text = clsFormat.DateShort(DateTime.Now.AddYears(1));
            lblFees.Text           = (appType?.Fees ?? 0).ToString("0.00");
            lblCreatedBy.Text      = clsGlobal.CurrentUsername;

            btnIssue.Enabled = true;
            lnkShowLicensesHistory.Enabled = true;
        }

        private void _ResetApplicationPreview()
        {
            lblILAppID.Text = lblAppDate.Text = lblILLicenseID.Text = lblLocalLicenseID.Text =
            lblIssueDate.Text = lblExpirationDate.Text = lblFees.Text = lblCreatedBy.Text = "[???]";
        }

        #endregion

        #region Issue

        private void _Issue(object sender, EventArgs e)
        {
            if (_localLicense == null)
            {
                clsUtil.ShowWarning("Find a local license first.");
                return;
            }

            if (MessageBox.Show(
                    "Are you sure you want to issue this international license?",
                    "Confirm Issue",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            clsInternationalLicense intl = clsInternationalLicense.IssueNew(
                _localLicense.ID, clsGlobal.CurrentUserID, out string errorMessage);

            if (intl == null)
            {
                clsUtil.ShowError(errorMessage, "Cannot Issue License");
                return;
            }

            _issuedLicense = intl;

            lblILAppID.Text        = intl.ApplicationID.ToString();
            lblILLicenseID.Text    = intl.InternationalLicenseID.ToString();
            lblIssueDate.Text      = clsFormat.DateShort(intl.IssueDate);
            lblExpirationDate.Text = clsFormat.DateShort(intl.ExpirationDate);

            // Lock the form after a successful issue — same convention as
            // frmReplaceLostOrDamagedLicense / frmRenewLocalDrivingLicenseApplication.
            btnIssue.Enabled = false;
            txtLicenseID.Enabled = false;
            btnFind.Enabled = false;
            lnkShowLicenseInfo.Enabled = true;

            clsUtil.ShowInfo(
                $"International License issued successfully.\n\nID = {intl.InternationalLicenseID}",
                "License Issued");
        }

        #endregion
    }
}