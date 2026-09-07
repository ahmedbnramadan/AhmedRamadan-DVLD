using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD
{
    public class frmShowInternationalLicenseInfo : Form
    {
        private Label lblTitle;
        private ctrlInternationalLicenseInfo ctrlLicenseInfo;
        private Button btnClose;

        private readonly int _internationalLicenseID;

        public frmShowInternationalLicenseInfo(int internationalLicenseID)
        {
            _internationalLicenseID = internationalLicenseID;

            _InitializeComponents();

            this.Load += (s, e) => ctrlLicenseInfo.LoadLicenseInfo(_internationalLicenseID);
        }

        private void _InitializeComponents()
        {
            this.Text = "Driver International License Info";
            this.Size = new Size(760, 390);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label
            {
                Text = "Driver International License Info",
                Font = new Font("Arial", 16F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(150, 18)
            };

            ctrlLicenseInfo = new ctrlInternationalLicenseInfo
            {
                Location = new Point(20, 65),
                Size = new Size(700, 220)
            };

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(560, 300),
                Size = new Size(150, 36),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = clsGlobal.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, ctrlLicenseInfo, btnClose });
        }
    }
}