using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmChangePassword : Form
    {
        private Label   lblTitle;
        private Label   lblCurrent, lblNew, lblConfirm;
        private TextBox txtCurrent, txtNew, txtConfirm;
        private Button  btnSave, btnClose;

        public frmChangePassword()
        {
            this.Text = "Change Password"; this.Size = new Size(430, 310);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Change Password",
                Font = new Font("Arial", 15F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(110, 18) };

            int y = 75; const int lx = 30, tx = 190, tw = 190, s = 50;
            lblCurrent = _L("Current Password:", lx, y);
            txtCurrent = _T(tx, y-3, tw); txtCurrent.PasswordChar = '●'; y += s;
            lblNew     = _L("New Password:", lx, y);
            txtNew     = _T(tx, y-3, tw); txtNew.PasswordChar = '●'; y += s;
            lblConfirm = _L("Confirm New:", lx, y);
            txtConfirm = _T(tx, y-3, tw); txtConfirm.PasswordChar = '●';

            btnSave  = _Btn("💾  Save",  220, 232, Color.FromArgb(0,120,215));
            btnClose = _Btn("✖  Close", 310, 232, Color.FromArgb(192,50,50));
            btnSave.Click  += _Save;
            btnClose.Click += (s2, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblCurrent, txtCurrent, lblNew, txtNew, lblConfirm, txtConfirm, btnSave, btnClose });
        }

        private void _Save(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrent.Text) ||
                string.IsNullOrWhiteSpace(txtNew.Text))
            { clsUtil.ShowWarning("All fields are required."); return; }

            if (txtNew.Text != txtConfirm.Text)
            { clsUtil.ShowWarning("New passwords do not match."); txtConfirm.BackColor = clsGlobal.InputError; return; }
            txtConfirm.BackColor = clsGlobal.InputValid;

            var user = clsUser.Find(clsGlobal.CurrentUserID);
            if (user == null) { clsUtil.ShowError("Current user not found."); return; }

            if (user.PassWord != txtCurrent.Text)
            { clsUtil.ShowWarning("Current password is incorrect."); txtCurrent.BackColor = clsGlobal.InputError; return; }
            txtCurrent.BackColor = clsGlobal.InputValid;

            user.PassWord = txtNew.Text;
            if (user.Save())
            { clsUtil.ShowInfo("Password changed successfully."); this.Close(); }
            else clsUtil.ShowError("Failed to change password.");
        }

        private static Label _L(string t, int x, int y)
            => new Label { Text = t, Location = new Point(x, y), AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };

        private static TextBox _T(int x, int y, int w)
            => new TextBox { Location = new Point(x, y), Size = new Size(w, 23) };

        private static Button _Btn(string t, int x, int y, Color c)
        {
            var b = new Button { Text = t, Location = new Point(x, y), Size = new Size(95, 32),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }
}