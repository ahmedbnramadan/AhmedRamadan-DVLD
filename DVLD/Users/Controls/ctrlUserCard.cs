using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD.Users.Controls
{
    /// <summary>
    /// User control that displays comprehensive user information including person details.
    /// </summary>
    public class ctrlUserCard : UserControl
    {
        #region Controls Declaration
        private GroupBox gbUserInformation;
        private Label lblUserIDTitle, lblUserID;
        private Label lblUserNameTitle, lblUserName;
        private Label lblIsActiveTitle, lblIsActive;

        private ctrlPersonCard ctrlPersonCard1;

        private int _userID = -1;
        private clsUser _user;
        private clsPerson _person;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the user ID to display. Setting this property loads the user information.
        /// </summary>
        public int UserID
        {
            get => _userID;
            set 
            { 
                _userID = value;
                if (_userID > 0)
                    LoadUserInfo(_userID);
            }
        }

        /// <summary>
        /// Gets the currently selected user object.
        /// </summary>
        public clsUser SelectedUser => _user;

        /// <summary>
        /// Gets the currently selected person object.
        /// </summary>
        public clsPerson SelectedPerson => _person;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ctrlUserCard"/> class.
        /// </summary>
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
                Text = "N/A",
                Location = new Point(x + 110, y),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 160)
            };
        }

        /// <summary>
        /// Loads and displays user information for the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID to load.</param>
        public void LoadUserInfo(int userID)
        {
            _userID = userID;
            _user = clsUser.Find(userID);

            if (_user == null)
            {
                clsUtil.ShowError($"User with ID {userID} not found.");
                return;
            }

            _person = clsPerson.Find(_user.PersonID);

            // Fill User Info
            lblUserID.Text = _user.UserID.ToString();
            lblUserName.Text = _user.UserName;
            lblIsActive.Text = _user.IsActive ? "Yes" : "No";
            lblIsActive.ForeColor = _user.IsActive ? Color.Green : Color.Red;

            // Load Person Card
            if (_person != null && ctrlPersonCard1 != null)
            {
                ctrlPersonCard1.LoadPersonInfo(_person.ID);
            }
        }

        /// <summary>
        /// Clears all displayed user information and resets the control to its default state.
        /// </summary>
        public void Clear()
        {
            _userID = -1;
            _user = null;
            _person = null;

            lblUserID.Text = "N/A";
            lblUserName.Text = "N/A";
            lblIsActive.Text = "N/A";
            lblIsActive.ForeColor = Color.Black;

            ctrlPersonCard1?.ResetPersonInfo();
        }
    }
}