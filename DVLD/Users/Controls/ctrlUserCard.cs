using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class ctrlUserCard : UserControl
    {
        #region Controls Declaration
        private GroupBox gbUserInformation;
        private Label lblUserIDTitle, lblUserID;
        private Label lblUserNameTitle, lblUserName;
        private Label lblIsActiveTitle, lblIsActive;

        private ctrlPersonCard ctrlPersonCard1;

        private int _UserID = -1;
        private clsUser _User;
        private clsPerson _Person;
        #endregion

        #region Properties
        public int UserID
        {
            get { return _UserID; }
            set 
            { 
                _UserID = value;
                if (_UserID > 0)
                    LoadUserInfo(_UserID);
            }
        }

        public clsUser SelectedUser => _User;
        public clsPerson SelectedPerson => _Person;
        #endregion

        public ctrlUserCard()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(830, 300);
            this.Font = new Font("Microsoft Sans Serif", 9F);
            this.AutoScroll = true;

            // GroupBox
            gbUserInformation = new GroupBox
            {
                Text = "User Information",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular)
            };

            // User Info Labels
            int y = 30;
            _CreateLabelPair("User ID", 20, y, out lblUserIDTitle, out lblUserID);
            y += 40;

            _CreateLabelPair("User Name", 20, y, out lblUserNameTitle, out lblUserName);
            y += 40;

            _CreateLabelPair("Is Active", 20, y, out lblIsActiveTitle, out lblIsActive);

            // Person Card
            ctrlPersonCard1 = new ctrlPersonCard
            {
                Location = new Point(380, 25),
                Size = new Size(420, 240)
            };

            // Add Controls
            gbUserInformation.Controls.AddRange(new Control[] 
            { 
                lblUserIDTitle, lblUserID,
                lblUserNameTitle, lblUserName,
                lblIsActiveTitle, lblIsActive,
                ctrlPersonCard1 
            });

            this.Controls.Add(gbUserInformation);
        }

        private void _CreateLabelPair(string title, int x, int y, 
            out Label titleLabel, out Label valueLabel)
        {
            titleLabel = new Label
            {
                Text = title + ":",
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            valueLabel = new Label
            {
                Text = "[???]",
                Location = new Point(x + 110, y),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 160)
            };
        }

        // ── Main Load Method ─────────────────────────────────────
        public void LoadUserInfo(int userID)
        {
            _UserID = userID;
            _User = clsUser.Find(userID);

            if (_User == null)
            {
                clsUtil.ShowError($"User with ID {userID} not found.");
                return;
            }

            _Person = clsPerson.Find(_User.PersonID);

            // Fill User Info
            lblUserID.Text = _User.UserID.ToString();
            lblUserName.Text = _User.UserName;
            lblIsActive.Text = _User.isActive ? "Yes" : "No";
            lblIsActive.ForeColor = _User.isActive ? Color.Green : Color.Red;

            // Load Person Card
            if (_Person != null && ctrlPersonCard1 != null)
            {
                ctrlPersonCard1.LoadPersonInfo(_Person.ID);
            }
        }

        public void Clear()
        {
            _UserID = -1;
            _User = null;
            _Person = null;

            lblUserID.Text = "[???]";
            lblUserName.Text = "[???]";
            lblIsActive.Text = "[???]";
            lblIsActive.ForeColor = Color.Black;

            ctrlPersonCard1?.ResetPersonInfo();
        }
    }
}