using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVLD
{
    public class frmShowLocalDrivingLicenseApplicationInfo : Form
    {
        #region Controls Declaration

        private Label lblTitle;
        private ctrlDrivingLicenseApplicationInfo ctrlApplicationInfo;
        private Button btnClose;

        #endregion

        #region State

        private readonly int _LocalDrivingLicenseApplicationID;

        #endregion

        #region Constructor

        public frmShowLocalDrivingLicenseApplicationInfo(
            int LocalDrivingLicenseApplicationID)
        {
            _LocalDrivingLicenseApplicationID =
                LocalDrivingLicenseApplicationID;

            InitializeComponents();

            this.Load += FrmShowLocalDrivingLicenseApplicationInfo_Load;
        }

        #endregion

        #region Form Events

        private void FrmShowLocalDrivingLicenseApplicationInfo_Load(
            object sender,
            EventArgs e)
        {
            ctrlApplicationInfo.LoadApplicationInfo(
                _LocalDrivingLicenseApplicationID);
        }

        #endregion

        #region Initialize Components

        private void InitializeComponents()
        {
            // Setup Form
            this.Text = "Local Driving License Application Information";
            this.Size = new Size(900, 570);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            // Title
            lblTitle = new Label
            {
                Text        = "Local Driving License Application Information",
                Font        = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor   = clsGlobal.PrimaryRed,
                AutoSize    = true,
                Location    = new Point(170, 20)
            };

            // Driving License Application Information Control
            ctrlApplicationInfo = new ctrlDrivingLicenseApplicationInfo
                {
                    Location = new Point(30, 65),
                    Size = new Size(830, 400)
                };

            // Close Button
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(700, 480),
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
                    ctrlApplicationInfo,
                    btnClose
                });
        }

        #endregion

        #region Button Events

        private void BtnClose_Click(
            object sender,
            EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}