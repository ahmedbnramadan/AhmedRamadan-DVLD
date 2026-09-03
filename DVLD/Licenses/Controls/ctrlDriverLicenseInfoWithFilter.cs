using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    /// <summary>
    /// Wraps ctrlDriverLicenseInfo with a numeric-only "Find by License ID"
    /// bar. Same shape as ctrlPersonCardWithFilter / ctrlUserCardWithFilter.
    /// </summary>
    public class ctrlDriverLicenseInfoWithFilter : UserControl
    {
        #region Controls Declaration

        private GroupBox gbFilter;
        private Label    lblFindBy;
        private TextBox  txtLicenseID;
        private Button   btnFind;
        private Button   btnClear;

        private ctrlDriverLicenseInfo ctrlDriverLicenseInfo1;

        #endregion

        #region State

        private clsLicense _license;

        #endregion

        #region Events

        /// <summary>Raised after a Find. Passes null if no license matched.</summary>
        public event EventHandler<clsLicense> LicenseLoaded;

        #endregion

        #region Properties

        public int LicenseID => ctrlDriverLicenseInfo1.LicenseID;

        public clsLicense SelectedLicenseInfo => _license;

        #endregion

        public ctrlDriverLicenseInfoWithFilter()
        {
            _InitializeComponents();
            _SetupEvents();
        }

        #region Build

        private void _InitializeComponents()
        {
            this.Size       = new Size(850, 430);
            this.AutoScroll = true;
            this.Font       = new Font("Microsoft Sans Serif", 9.5F);
            this.BackColor  = Color.FromArgb(240, 242, 248);

            gbFilter = new GroupBox
            {
                Text     = "Filter",
                Location = new Point(10, 10),
                Size     = new Size(830, 65),
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            lblFindBy = new Label
            {
                Text     = "License ID:",
                Location = new Point(20, 28),
                AutoSize = true,
                Font     = new Font("Microsoft Sans Serif", 9.5F)
            };

            txtLicenseID = new TextBox
            {
                Location  = new Point(120, 25),
                Size      = new Size(160, 25),
                Font      = new Font("Microsoft Sans Serif", 9.5F),
                MaxLength = 10
            };

            btnFind = new Button
            {
                Text      = "Find",
                Location  = new Point(295, 23),
                Size      = new Size(80, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                Enabled   = false               // only enabled once something is typed
            };
            btnFind.FlatAppearance.BorderSize = 0;

            btnClear = new Button
            {
                Text      = "Clear",
                Location  = new Point(385, 23),
                Size      = new Size(80, 30),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Microsoft Sans Serif", 9F),
                Cursor    = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;

            gbFilter.Controls.AddRange(new Control[] { lblFindBy, txtLicenseID, btnFind, btnClear });

            ctrlDriverLicenseInfo1 = new ctrlDriverLicenseInfo
            {
                Location = new Point(10, 90)
            };

            this.Controls.Add(gbFilter);
            this.Controls.Add(ctrlDriverLicenseInfo1);
        }

        private void _SetupEvents()
        {
            btnFind.Click            += btnFind_Click;
            btnClear.Click           += (s, e) => Clear();
            txtLicenseID.KeyPress    += txtLicenseID_KeyPress;
            txtLicenseID.KeyDown     += txtLicenseID_KeyDown;
            txtLicenseID.TextChanged += txtLicenseID_TextChanged;

            // Best-effort auto-focus the moment this control actually becomes
            // visible on screen. See the note under "focus" below for why
            // this alone isn't 100% reliable and what to pair it with.
            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible) FocusOnFilter();
            };
        }

        #endregion

        #region Input restriction / validation

        // Digits only — same guard style as frmDetainLicense.txtLicenseID
        // and ctrlPersonCardWithFilter.txtFilterValue_KeyPress.
        private void txtLicenseID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // Enter = click Find.
        private void txtLicenseID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;   // stops the Windows "ding"
                btnFind.PerformClick();
            }
        }

        private void txtLicenseID_TextChanged(object sender, EventArgs e)
        {
            btnFind.Enabled = !string.IsNullOrWhiteSpace(txtLicenseID.Text);
        }

        #endregion

        #region Find / Clear

        private void btnFind_Click(object sender, EventArgs e)
        {
            string value = txtLicenseID.Text.Trim();

            if (string.IsNullOrEmpty(value))
            {
                clsUtil.ShowWarning("Please enter a License ID to search.");
                txtLicenseID.Focus();
                return;
            }

            // KeyPress already blocks non-digits, but never trust client-side
            // filtering as your only line of defense — paste, IME input, and
            // programmatic Text assignment can all skip KeyPress entirely.
            if (!int.TryParse(value, out int licenseID) || licenseID <= 0)
            {
                clsUtil.ShowWarning("License ID must be a valid positive number.");
                txtLicenseID.BackColor = clsGlobal.InputError;
                txtLicenseID.Focus();
                return;
            }

            txtLicenseID.BackColor = clsGlobal.InputValid;

            ctrlDriverLicenseInfo1.LoadLicenseInfo(licenseID);
            _license = ctrlDriverLicenseInfo1.SelectedLicenseInfo;

            LicenseLoaded?.Invoke(this, _license);
        }

        public void Clear()
        {
            txtLicenseID.Clear();
            txtLicenseID.BackColor = clsGlobal.InputValid;
            ctrlDriverLicenseInfo1.ResetLicenseInfo();
            _license = null;
            txtLicenseID.Focus();
        }

        #endregion

        #region Public API

        /// <summary>Shows/hides + enables/disables the whole filter bar.
        /// Use this when a hosting form wants to lock the control to one
        /// already-known license (e.g. an edit screen that passed the ID
        /// in via the constructor).</summary>
        public void EnableFilter(bool filterEnabled)
        {
            gbFilter.Visible = filterEnabled;
            gbFilter.Enabled = filterEnabled;

            if (filterEnabled)
                FocusOnFilter();
        }

        public void FocusOnFilter()
        {
            if (gbFilter.Visible)
                txtLicenseID.Focus();
        }

        /// <summary>Loads a specific license directly and hides the filter bar
        /// — for callers who already know the ID and don't want the user
        /// typing it in again.</summary>
        public void LoadLicenseInfo(int licenseID)
        {
            EnableFilter(false);
            txtLicenseID.Text = licenseID.ToString();

            ctrlDriverLicenseInfo1.LoadLicenseInfo(licenseID);
            _license = ctrlDriverLicenseInfo1.SelectedLicenseInfo;

            LicenseLoaded?.Invoke(this, _license);
        }

        #endregion
    }
}