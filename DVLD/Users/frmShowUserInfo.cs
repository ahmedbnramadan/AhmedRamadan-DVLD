using System;
using System.Drawing;
using System.Windows.Forms;
using DVLD.Users.Controls;

namespace DVLD
{
    public class frmShowUserInfo : Form
    {
        private ctrlUserCard ctrlUserCard1;
        private Button btnClose;

        public frmShowUserInfo(int userID)
        {
            InitializeComponent();

            ctrlUserCard1.UserID = userID;
        }

        private void InitializeComponent()
        {
            this.Text = "User Information";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            ctrlUserCard1 = new ctrlUserCard
            {
                Location = new Point(0, 0)
            };

            btnClose = new Button
            {
                Text = "Close",
                Size = new Size(100, 32),
                Font = new Font("Microsoft Sans Serif", 9F)
            };

            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(ctrlUserCard1);
            this.Controls.Add(btnClose);

            const int bottomPadding = 15;

            // Center the button horizontally under the card, with fixed spacing below it.
            btnClose.Location = new Point(
                ctrlUserCard1.Right - btnClose.Width - 30,
                ctrlUserCard1.Bottom + 15
            );

            this.ClientSize = new Size(
                ctrlUserCard1.Width,
                btnClose.Bottom + bottomPadding
            );
        }
    }
}