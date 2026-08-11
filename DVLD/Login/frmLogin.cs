using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Business;

namespace DVLD
{
    public class frmLogin : Form
    {
        #region Controls
        private Panel pnlLeft, pnlRight;
        private Label lblAppTitle, lblVersion;
        private Label lblLoginTitle;
        private Label lblUsername, lblPassword;
        private TextBox txtUsername, txtPassword;
        private CheckBox chkRememberMe;
        private Button btnLogin;
        private Label lblError;
        #endregion

        private static readonly string _settingsFile = Path.Combine(Application.StartupPath, "remember.cfg");

        public frmLogin()
        {
            _Build();
            _LoadRemembered();
        }

        private void _Build()
        {
            this.Text = "DVLD – Login";
            this.Size = new Size(780, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = btnLogin;   // set after button created below

            // ── Left dark panel ──────────────────────────────────────
            pnlLeft = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(310, 480),
                BackColor = Color.FromArgb(18, 18, 18)
            };

            lblAppTitle = new Label
            {
                Text = "WELCOME TO\nDRIVING & VEHICLE\nLICENSE DEPARTMENT\n(DVLD) SYSTEM",
                Font = new Font("Arial", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 160),
                Size = new Size(270, 160)
            };

            lblVersion = new Label
            {
                Text = "Version " + clsGlobal.AppVersion,
                Font = new Font("Arial", 9F),
                ForeColor = Color.Silver,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 390),
                Size = new Size(270, 25)
            };

            pnlLeft.Controls.AddRange(new Control[] { lblAppTitle, lblVersion });

            // ── Right white panel ─────────────────────────────────────
            pnlRight = new Panel
            {
                Location = new Point(310, 0),
                Size = new Size(470, 480),
                BackColor = Color.White
            };

            lblLoginTitle = new Label
            {
                Text = "Login to your account",
                Font = new Font("Arial", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                AutoSize = true,
                Location = new Point(80, 70)
            };

            lblUsername = new Label { Text = "Username:", AutoSize = true, Location = new Point(80, 140), Font = new Font("MS Sans Serif", 10F, FontStyle.Bold) };
            txtUsername = new TextBox { Location = new Point(195, 137), Size = new Size(200, 26), Font = new Font("MS Sans Serif", 10F) };

            lblPassword = new Label { Text = "Password:", AutoSize = true, Location = new Point(80, 195), Font = new Font("MS Sans Serif", 10F, FontStyle.Bold) };
            txtPassword = new TextBox { Location = new Point(195, 192), Size = new Size(200, 26), Font = new Font("MS Sans Serif", 10F), PasswordChar = '●' };

            chkRememberMe = new CheckBox
            {
                Text = "Remember Me",
                Location = new Point(195, 230),
                AutoSize = true,
                Font = new Font("MS Sans Serif", 9F),
                Cursor = Cursors.Hand
            };

            lblError = new Label
            {
                Text = "Invalid username or password.",
                ForeColor = Color.Crimson,
                Font = new Font("MS Sans Serif", 8.5F),
                AutoSize = true,
                Location = new Point(195, 262),
                Visible = false
            };

            btnLogin = new Button
            {
                Text = "🔑  Login",
                Location = new Point(195, 290),
                Size = new Size(200, 40),
                Font = new Font("Arial", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(18, 18, 18),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += btnLogin_Click;

            pnlRight.Controls.AddRange(new Control[]
            {
                lblLoginTitle, lblUsername, txtUsername,
                lblPassword, txtPassword,
                chkRememberMe, lblError, btnLogin
            });

            this.AcceptButton = btnLogin;
            this.Controls.AddRange(new Control[] { pnlLeft, pnlRight });
        }

        // ── Remember Me ──────────────────────────────────────────────────────

        private void _LoadRemembered()
        {
            if (File.Exists(_settingsFile))
            {
                txtUsername.Text = File.ReadAllText(_settingsFile).Trim();
                chkRememberMe.Checked = !string.IsNullOrEmpty(txtUsername.Text);
                if (chkRememberMe.Checked) txtPassword.Focus();
            }
        }

        private void _SaveRemembered()
        {
            File.WriteAllText(_settingsFile, chkRememberMe.Checked ? txtUsername.Text.Trim() : "");
        }

        // ── Login ─────────────────────────────────────────────────────────────

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Please enter username and password.";
                lblError.Visible = true;
                return;
            }

            clsUser user = clsUser.Find(txtUsername.Text.Trim());

            if (user == null || !user.isActive ||
                user.PassWord != txtPassword.Text)   // replace with hash compare if hashed
            {
                lblError.Text = "Invalid username or password.";
                lblError.Visible = true;
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            // Store globally
            clsGlobal.CurrentUserID = user.UserID;
            clsGlobal.CurrentUsername = user.UserName;

            _SaveRemembered();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}