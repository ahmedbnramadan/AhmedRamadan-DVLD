using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmShowUserInfo : Form
    {
        #region Controls Declaration
        private Label lblTitle;
        private Panel pnlCard;

        private Label lblUserIDTitle, lblUserID;
        private Label lblUserNameTitle, lblUserName;
        private Label lblGenderTitle, lblGender;
        private Label lblIsActiveTitle, lblIsActive;

        private PictureBox pbUserImage;

        private LinkLabel llEdit;
        private LinkLabel llSendEmail;
        private LinkLabel llPhoneCall;
        private Button btnClose;
        #endregion

        #region State
        private readonly int _userID;
        private clsUser _user;
        private clsPerson _person;   // لأن Gender و الصورة و Email و Phone في clsPerson
        #endregion

        // ── Constructor ─────────────────────────────────────────────────────
        public frmShowUserInfo(int userID)
        {
            _userID = userID;
            _InitializeComponents();
            _LoadUser();
        }

        private void _InitializeComponents()
        {
            this.Text = "User Details";
            this.Size = new Size(860, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            // Title
            lblTitle = new Label
            {
                Text = "User Information",
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(280, 20)
            };

            // User Image
            pbUserImage = new PictureBox
            {
                Location = new Point(650, 65),
                Size = new Size(165, 190),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(235, 237, 244)
            };

            // Main Card
            pnlCard = new Panel
            {
                Location = new Point(30, 65),
                Size = new Size(600, 455),
                BackColor = Color.White
            };

            pnlCard.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(210, 215, 225), 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            };

            // Info Rows
            int rowY = 25;
            const int rowStep = 47;
            const int titleX = 20;
            const int valueX = 160;

            _MakeInfoRow(pnlCard, "User ID:", titleX, valueX, rowY, out lblUserIDTitle, out lblUserID, Color.SteelBlue);
            rowY += rowStep;

            _MakeInfoRow(pnlCard, "UserName:", titleX, valueX, rowY, out lblUserNameTitle, out lblUserName, Color.FromArgb(30, 80, 160));
            rowY += rowStep;

            _MakeInfoRow(pnlCard, "Gender:", titleX, valueX, rowY, out lblGenderTitle, out lblGender, Color.FromArgb(30, 80, 160));
            rowY += rowStep;

            _MakeInfoRow(pnlCard, "Is Active:", titleX, valueX, rowY, out lblIsActiveTitle, out lblIsActive, Color.Black);

            // Action Links
            llEdit = new LinkLabel
            {
                Text = "✏ Edit User",
                Location = new Point(650, 265),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = Color.SteelBlue
            };
            llEdit.LinkClicked += llEdit_LinkClicked;

            llSendEmail = new LinkLabel
            {
                Text = "📧 Send Email",
                Location = new Point(650, 295),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = Color.SteelBlue
            };
            llSendEmail.LinkClicked += (s, e) => clsUtil.SendEmail(_person?.Email);

            llPhoneCall = new LinkLabel
            {
                Text = "📞 Phone Call",
                Location = new Point(650, 325),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = Color.SteelBlue
            };
            llPhoneCall.LinkClicked += (s, e) => clsUtil.MakePhoneCall(_person?.Phone);

            // Close Button
            btnClose = new Button
            {
                Text = "✖ Close",
                Location = new Point(650, 490),
                Size = new Size(165, 38),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(192, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] 
            { 
                lblTitle, pbUserImage, pnlCard,
                llEdit, llSendEmail, llPhoneCall, btnClose 
            });
        }

        private static void _MakeInfoRow(Panel parent, string titleText, int titleX, int valueX, int y,
            out Label titleLabel, out Label valueLabel, Color valueColor)
        {
            titleLabel = new Label
            {
                Text = titleText,
                Location = new Point(titleX, y),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 90)
            };

            valueLabel = new Label
            {
                Text = "—",
                Location = new Point(valueX, y),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F),
                ForeColor = valueColor
            };

            var separator = new Panel
            {
                Location = new Point(titleX, y + 22),
                Size = new Size(560, 1),
                BackColor = Color.FromArgb(230, 232, 240)
            };

            parent.Controls.AddRange(new Control[] { titleLabel, valueLabel, separator });
        }

        // ── Load User Data ─────────────────────────────────────────────────────
        private void _LoadUser()
        {
            _user = clsUser.Find(_userID);

            if (_user == null)
            {
                clsUtil.ShowError($"No user found with ID = {_userID}.");
                this.Close();
                return;
            }

            // Load associated Person (Gender, Image, Email, Phone موجودة هنا)
            _person = clsPerson.Find(_user.PersonID);

            if (_person == null)
            {
                clsUtil.ShowError("Unable to load person information for this user.");
                this.Close();
                return;
            }

            // Fill Data
            lblUserID.Text = _user.UserID.ToString();
            lblUserName.Text = _user.UserName;

            // Gender
            lblGender.Text = (_person.Gender == 0) ? "Male" : "Female";

            // Is Active
            lblIsActive.Text = _user.isActive ? "Yes" : "No";
            lblIsActive.ForeColor = _user.isActive ? Color.Green : Color.Red;

            // Load Image
            clsUtil.LoadPersonImage(pbUserImage, _person.ImagePath);

            // Show/Hide Links
            llSendEmail.Visible = !string.IsNullOrWhiteSpace(_person.Email);
            llPhoneCall.Visible = !string.IsNullOrWhiteSpace(_person.Phone);
        }

        private void llEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //new frmAddEditUser(_userID).ShowDialog();
            _LoadUser(); // Refresh after edit
        }
    }
}