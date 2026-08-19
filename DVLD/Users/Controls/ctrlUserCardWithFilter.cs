using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class ctrlUserCardWithFilter : UserControl
    {
        #region Controls Declaration
        private GroupBox grpFilter;
        private Label lblFilterBy;
        private ComboBox cmbFilterBy;
        private TextBox txtFilterValue;
        private Button btnFind;
        private Button btnClear;

        private Panel pnlCard;
        private PictureBox pbUserImage;

        private Label lblUserIDTitle, lblUserID;
        private Label lblUserNameTitle, lblUserName;
        private Label lblGenderTitle, lblGender;
        private Label lblIsActiveTitle, lblIsActive;

        private LinkLabel llEditUser;
        #endregion

        #region State
        private clsUser _user;
        private clsPerson _person;
        #endregion

        // Event when user is found successfully
        public event Action<int> OnUserSelected;

        // ── Constructor ─────────────────────────────────────────────────────
        public ctrlUserCardWithFilter()
        {
            _InitializeComponents();
        }

        private void _InitializeComponents()
        {
            this.Size = new Size(820, 380);
            this.BackColor = Color.FromArgb(240, 242, 248);

            // ==================== Filter Section ====================
            grpFilter = new GroupBox
            {
                Text = "Filter",
                Location = new Point(10, 10),
                Size = new Size(800, 70),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            lblFilterBy = new Label
            {
                Text = "Filter By:",
                Location = new Point(20, 30),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };

            cmbFilterBy = new ComboBox
            {
                Location = new Point(100, 27),
                Size = new Size(130, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterBy.Items.AddRange(new string[] { "User ID", "UserName" });
            cmbFilterBy.SelectedIndex = 0;

            txtFilterValue = new TextBox
            {
                Location = new Point(250, 27),
                Size = new Size(250, 25),
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };

            btnFind = new Button
            {
                Text = "Find",
                Location = new Point(520, 26),
                Size = new Size(80, 28),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFind.FlatAppearance.BorderSize = 0;
            btnFind.Click += btnFind_Click;

            btnClear = new Button
            {
                Text = "Clear",
                Location = new Point(610, 26),
                Size = new Size(80, 28),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9F),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += (s, e) => Clear();

            grpFilter.Controls.AddRange(new Control[] { lblFilterBy, cmbFilterBy, txtFilterValue, btnFind, btnClear });

            // ==================== User Card ====================
            pnlCard = new Panel
            {
                Location = new Point(10, 95),
                Size = new Size(800, 270),
                BackColor = Color.White,
                Visible = false
            };

            pnlCard.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(210, 215, 225), 2))
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            };

            // Image
            pbUserImage = new PictureBox
            {
                Location = new Point(620, 30),
                Size = new Size(150, 180),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(235, 237, 244)
            };

            // Info Rows
            int rowY = 30;
            const int rowStep = 45;
            const int titleX = 30;
            const int valueX = 160;

            _MakeInfoRow("User ID:", titleX, valueX, rowY, out lblUserIDTitle, out lblUserID, Color.SteelBlue);
            rowY += rowStep;

            _MakeInfoRow("UserName:", titleX, valueX, rowY, out lblUserNameTitle, out lblUserName, Color.FromArgb(30, 80, 160));
            rowY += rowStep;

            _MakeInfoRow("Gender:", titleX, valueX, rowY, out lblGenderTitle, out lblGender, Color.FromArgb(30, 80, 160));
            rowY += rowStep;

            _MakeInfoRow("Is Active:", titleX, valueX, rowY, out lblIsActiveTitle, out lblIsActive, Color.Black);

            // Edit Link
            llEditUser = new LinkLabel
            {
                Text = "✏ Edit User",
                Location = new Point(620, 230),
                AutoSize = true,
                LinkColor = Color.SteelBlue,
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };
            llEditUser.LinkClicked += llEditUser_LinkClicked;

            pnlCard.Controls.AddRange(new Control[] 
            { 
                pbUserImage, 
                lblUserIDTitle, lblUserID,
                lblUserNameTitle, lblUserName,
                lblGenderTitle, lblGender,
                lblIsActiveTitle, lblIsActive,
                llEditUser 
            });

            // Add to UserControl
            this.Controls.Add(grpFilter);
            this.Controls.Add(pnlCard);
        }

        private void _MakeInfoRow(string titleText, int titleX, int valueX, int y,
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

            pnlCard.Controls.Add(titleLabel);
            pnlCard.Controls.Add(valueLabel);
        }

        // ── Find Button ─────────────────────────────────────────────────────
        private void btnFind_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilterValue.Text))
            {
                clsUtil.ShowError("Please enter a value to search.");
                return;
            }

            if (cmbFilterBy.Text == "User ID")
            {
                if (!int.TryParse(txtFilterValue.Text.Trim(), out int userID))
                {
                    clsUtil.ShowError("User ID must be a number.");
                    return;
                }
                _user = clsUser.Find(userID);
            }
            else // UserName
            {
                _user = clsUser.Find(txtFilterValue.Text.Trim());
            }

            if (_user == null)
            {
                clsUtil.ShowError("User not found.");
                Clear();
                return;
            }

            _person = clsPerson.Find(_user.PersonID);
            _FillUserCard();
            OnUserSelected?.Invoke(_user.UserID);
        }

        private void _FillUserCard()
        {
            pnlCard.Visible = true;

            lblUserID.Text = _user.UserID.ToString();
            lblUserName.Text = _user.UserName;
            lblGender.Text = (_person.Gender == 0) ? "Male" : "Female";
            lblIsActive.Text = _user.IsActive ? "Yes" : "No";
            lblIsActive.ForeColor = _user.IsActive ? Color.Green : Color.Red;

            clsUtil.LoadPersonImage(pbUserImage, _person.ImagePath);
        }

        // ── Public Methods ─────────────────────────────────────────────────────
        public void Clear()
        {
            txtFilterValue.Clear();
            pnlCard.Visible = false;
            _user = null;
            _person = null;
        }

        public clsUser SelectedUser => _user;

        private void llEditUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_user == null) return;

            new frmAddEditUser(_user.UserID).ShowDialog();
            // Refresh after edit
            btnFind_Click(null, null);
        }
    }
}