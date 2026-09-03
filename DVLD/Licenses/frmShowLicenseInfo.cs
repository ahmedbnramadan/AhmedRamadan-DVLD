using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD
{
    /// <summary>
    /// Read-only view of a single driver license's details.
    /// Displays license info using ctrlDriverLicenseInfo and provides a Close button.
    /// </summary>
    public class frmShowLicenseInfo : Form
    {
        #region Controls Declaration

        private Label lblTitle;
        private ctrlDriverLicenseInfo ctrlLicenseInfo;
        private Button btnClose;

        #endregion

        #region State

        private readonly int _licenseID;

        #endregion

        #region Constructor

        public frmShowLicenseInfo(int licenseID)
        {
            _licenseID = licenseID;

            InitializeComponents();

            this.Load += FrmShowLicenseInfo_Load;
        }

        #endregion

        #region Form Events

        private void FrmShowLicenseInfo_Load(object sender, EventArgs e)
        {
            ctrlLicenseInfo.LoadLicenseInfo(_licenseID);
        }

        #endregion

        #region Initialize Components

        private void InitializeComponents()
        {
            // Setup Form
            this.Text = "Driver License Information";
            this.Size = new Size(900, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            // Title
            lblTitle = new Label
            {
                Text = "Driver License Information",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            // Driver License Information Control
            ctrlLicenseInfo = new ctrlDriverLicenseInfo
            {
                Location = new Point(30, 70),
                Size = new Size(830, 350)
            };

            // Close Button
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(700, 440),
                Size = new Size(160, 38),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = clsGlobal.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += BtnClose_Click;

            // Add Controls
            this.Controls.AddRange(
                new Control[]
                {
                    lblTitle,
                    ctrlLicenseInfo,
                    btnClose
                });
        }

        #endregion

        #region Button Events

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}