using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmShowLocalDrivingLicenseApplicationInfo : Form
    {
        private Label      lblTitle;
        private Panel      pnlCard;
        private Label      lblAppIDt,    lblAppIDv;
        private Label      lblPersont,   lblPersonv;
        private Label      lblNNot,      lblNNov;
        private Label      lblClasst,    lblClassv;
        private Label      lblDatet,     lblDatev;
        private Label      lblFeesat,    lblFeesv;
        private Label      lblStatust,   lblStatusv;
        private Label      lblCreatedt,  lblCreatedv;
        private PictureBox pbImg;
        private LinkLabel  llEdit;
        private Button     btnClose;

        private readonly int _appID;

        public frmShowLocalDrivingLicenseApplicationInfo(int appID)
        {
            _appID = appID;
            _Build();
            _LoadData();
        }

        private void _Build()
        {
            this.Text = "Application Details"; this.Size = new Size(820, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.FromArgb(240,242,248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Application Information",
                Font = new Font("Arial", 18F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(220, 18) };

            pbImg = new PictureBox { Location = new Point(620,60), Size = new Size(160,185),
                SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(235,237,244) };

            pnlCard = new Panel { Location = new Point(20,60), Size = new Size(580,415),
                BackColor = Color.White };
            pnlCard.Paint += (s, e) => {
                using (var pen = new Pen(Color.FromArgb(210,215,225)))
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlCard.Width-1, pnlCard.Height-1);
            };

            int y = 22; const int st = 45, tx = 20, vx = 200;
            _Row(pnlCard, "Application ID:",   tx, vx, y, out lblAppIDt,   out lblAppIDv,   Color.SteelBlue);           y += st;
            _Row(pnlCard, "Applicant:",        tx, vx, y, out lblPersont,  out lblPersonv,  Color.FromArgb(30,80,160)); y += st;
            _Row(pnlCard, "National No.:",     tx, vx, y, out lblNNot,     out lblNNov,     Color.Black);               y += st;
            _Row(pnlCard, "License Class:",    tx, vx, y, out lblClasst,   out lblClassv,   Color.Black);               y += st;
            _Row(pnlCard, "Application Date:", tx, vx, y, out lblDatet,    out lblDatev,    Color.Black);               y += st;
            _Row(pnlCard, "Fees Paid:",        tx, vx, y, out lblFeesat,   out lblFeesv,    Color.Black);               y += st;
            _Row(pnlCard, "Status:",           tx, vx, y, out lblStatust,  out lblStatusv,  Color.DarkGreen);           y += st;
            _Row(pnlCard, "Created By:",       tx, vx, y, out lblCreatedt, out lblCreatedv, Color.Gray);

            llEdit = new LinkLabel { Text = "✏  Edit Application", AutoSize = true,
                Location = new Point(620,255), Font = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = Color.SteelBlue };
            llEdit.LinkClicked += (s, e) => {
                new frmAddEditNewLocalDrivingLicenseApplication(_appID).ShowDialog();
                _LoadData();
            };

            btnClose = new Button { Text = "✖  Close", Location = new Point(620,460), Size = new Size(160,36),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(192,50,50), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, pbImg, pnlCard, llEdit, btnClose });
        }

        private void _LoadData()
        {
            var app = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppID(_appID);
            if (app == null) { clsUtil.ShowError("Application not found."); this.Close(); return; }

            var person  = clsPerson.Find(app.ApplicantPersonID);
            var lc      = clsLicenseClass.Find(app.LicenseClassID);
            var creator = clsUser.Find(app.CreatedByUserID);

            lblAppIDv.Text   = app.LocalDrivingLicenseApplicationID.ToString();
            lblPersonv.Text  = person?.FullName  ?? "—";
            lblNNov.Text     = person?.NationalNo ?? "—";
            lblClassv.Text   = lc?.Name     ?? "—";
            lblDatev.Text    = clsFormat.DateLong(app.ApplicationDate);
            lblFeesv.Text    = app.PaidFees.ToString("F2") + " US";
            lblStatusv.Text  = app.ApplicationStatus.ToString();
            lblCreatedv.Text = creator?.UserName ?? "—";

            if (person != null) clsUtil.LoadPersonImage(pbImg, person.ImagePath);
        }

        private static void _Row(Panel p, string t, int tx, int vx, int y,
            out Label tl, out Label vl, Color c)
        {
            tl = new Label { Text = t, Location = new Point(tx,y), AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(80,80,90) };
            vl = new Label { Text = "—", Location = new Point(vx,y), AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F), ForeColor = c };
            var sep = new Panel { Location = new Point(tx,y+22), Size = new Size(540,1),
                BackColor = Color.FromArgb(230,232,240) };
            p.Controls.AddRange(new Control[] { tl, vl, sep });
        }
    }
}