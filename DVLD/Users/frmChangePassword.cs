using System;
using System.Drawing;
using System.Windows.Forms;
using Business;
using DVLD.Users.Controls;

namespace DVLD
{
    public class frmChangePassword : Form
    {
        #region Controls Declaration

        // Header
        private Label lblTitle;

        // User Card (no filter)
        private ctrlUserCard ctrlUserCard1;

        // Password Change Section
        private GroupBox grpChangePassword;
        private Label lblCurrent, lblNew, lblConfirm;
        private TextBox txtCurrent, txtNew, txtConfirm;
        private Button btnSave, btnClose;

        #endregion

        #region State

        private int _currentUserID = -1;
        private clsUser _currentUser = null;

        #endregion

        public frmChangePassword(int userID)
        {
            _currentUserID = userID;

            this.Text = "Change Password";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            _InitializeComponents();
            _LoadCurrentUser();
        }

        private void _InitializeComponents()
        {
            // ── Title ────────────────────────────────────────────────
            lblTitle = new Label
            {
                Text = "Change Password",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 50, 50),
                AutoSize = true,
                Location = new Point(30, 15)
            };

            // ── User Card (display only, no filter) ──────────────────
            ctrlUserCard1 = new ctrlUserCard
            {
                Location = new Point(15, 55),
                Size = new Size(860, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // ── Change Password Group ───────────────────────────────
            grpChangePassword = new GroupBox
            {
                Text = "Change Password",
                Location = new Point(15, 515),
                Size = new Size(860, 130),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                Enabled = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            int y = 28;
            const int lx = 25, tx = 160, tw = 240, s = 36;

            lblCurrent = _L("Current Password:", lx, y);
            txtCurrent = _T(tx, y, tw);
            txtCurrent.UseSystemPasswordChar = true;
            y += s;

            lblNew = _L("New Password:", lx, y);
            txtNew = _T(tx, y, tw);
            txtNew.UseSystemPasswordChar = true;
            y += s;

            lblConfirm = _L("Confirm Password:", lx, y);
            txtConfirm = _T(tx, y, tw);
            txtConfirm.UseSystemPasswordChar = true;

            btnSave = _Btn("Save Changes", 480, 25, Color.FromArgb(0, 120, 215));
            btnClose = _Btn("✖  Close", 630, 25, Color.FromArgb(192, 50, 50));

            btnSave.Click += BtnSave_Click;
            btnClose.Click += (s2, e) => this.Close();

            grpChangePassword.Controls.AddRange(new Control[] {
                lblCurrent, txtCurrent, lblNew, txtNew, lblConfirm, txtConfirm, btnSave, btnClose
            });

            // ── Add all to form ─────────────────────────────────────
            this.Controls.AddRange(new Control[] {
                lblTitle, ctrlUserCard1, grpChangePassword
            });
        }

        #region Event Handlers

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _SavePassword();
        }

        #endregion

        #region Methods

        private void _LoadCurrentUser()
        {
            if (_currentUserID == -1)
            {
                clsUtil.ShowError("No user ID provided. Access denied.");
                this.Close();
                return;
            }

            _currentUser = clsUser.Find(_currentUserID);

            if (_currentUser == null)
            {
                clsUtil.ShowError($"User with ID '{_currentUserID}' was not found.");
                this.Close();
                return;
            }

            // Load the current user's card
            ctrlUserCard1.UserID = _currentUserID;

            // Enable password section since we have the current user loaded
            grpChangePassword.Enabled = true;
            grpChangePassword.ForeColor = Color.FromArgb(0, 120, 215);
            txtCurrent.Focus();
        }

        private void _SavePassword()
        {
            // Validate that we have a loaded user
            if (_currentUser == null)
            {
                clsUtil.ShowWarning("User information not loaded.");
                return;
            }

            // Validate current password field
            if (string.IsNullOrWhiteSpace(txtCurrent.Text))
            {
                clsUtil.ShowWarning("Current password is required.");
                txtCurrent.BackColor = clsGlobal.InputError;
                txtCurrent.Focus();
                return;
            }
            txtCurrent.BackColor = Color.White;

            // Validate new password field
            if (string.IsNullOrWhiteSpace(txtNew.Text))
            {
                clsUtil.ShowWarning("New password is required.");
                txtNew.BackColor = clsGlobal.InputError;
                txtNew.Focus();
                return;
            }

            // Password strength validation
            if (txtNew.Text.Length < 6)
            {
                clsUtil.ShowWarning("New password must be at least 6 characters long.");
                txtNew.BackColor = clsGlobal.InputError;
                txtNew.Focus();
                return;
            }
            txtNew.BackColor = Color.White;

            // Validate confirm password field
            if (string.IsNullOrWhiteSpace(txtConfirm.Text))
            {
                clsUtil.ShowWarning("Please confirm your new password.");
                txtConfirm.BackColor = clsGlobal.InputError;
                txtConfirm.Focus();
                return;
            }

            // Check if passwords match
            if (txtNew.Text != txtConfirm.Text)
            {
                clsUtil.ShowWarning("New password and confirmation do not match.");
                txtConfirm.BackColor = clsGlobal.InputError;
                txtConfirm.Focus();
                return;
            }
            txtConfirm.BackColor = Color.White;

            // Verify current password
            if (_currentUser.Password != txtCurrent.Text)
            {
                clsUtil.ShowWarning("Current password is incorrect.");
                txtCurrent.BackColor = clsGlobal.InputError;
                txtCurrent.Focus();
                return;
            }

            // Check if new password is same as current
            if (txtNew.Text == txtCurrent.Text)
            {
                clsUtil.ShowWarning("New password must be different from the current password.");
                txtNew.BackColor = clsGlobal.InputError;
                txtNew.Focus();
                return;
            }

            // Change password using business layer
            if (clsUser.ChangePassword(_currentUserID, txtNew.Text))
            {
                clsUtil.ShowInfo("Password changed successfully.", "Success");
                _ClearFields();
                grpChangePassword.Enabled = false;
                grpChangePassword.ForeColor = Color.Gray;
                this.Close();
            }
            else
            {
                clsUtil.ShowError("Failed to change password. Please try again.", "Error");
            }
        }

        private void _ClearFields()
        {
            txtCurrent.Clear();
            txtNew.Clear();
            txtConfirm.Clear();
            txtCurrent.BackColor = Color.White;
            txtNew.BackColor = Color.White;
            txtConfirm.BackColor = Color.White;
        }

        #endregion

        #region Helper Methods

        private static Label _L(string t, int x, int y)
            => new Label
            {
                Text = t,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

        private static TextBox _T(int x, int y, int w)
            => new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 30),
                Font = new Font("Microsoft Sans Serif", 9.5F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

        private static Button _Btn(string t, int x, int y, Color c)
        {
            var b = new Button
            {
                Text = t,
                Location = new Point(x, y),
                Size = new Size(140, 38),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = c,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        #endregion
    }
}