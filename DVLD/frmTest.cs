using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmTest : Form
    {
        #region Controls Declaration

        // Section 1: Simple ctrlPersonCard
        private GroupBox gbSimpleCard;
        private ctrlPersonCard ctrlPersonCardSimple;
        private Button btnLoadSimple;
        private TextBox txtSimplePersonID;

        // Section 2: ctrlPersonCardWithFilter
        private GroupBox gbFilteredCard;
        private ctrlPersonCardWithFilter ctrlPersonCardWithFilter1;

        // Section 3: Add/Edit Person Form
        private GroupBox gbAddEdit;
        private Button btnAddNewPerson;
        private Label lblEditID;
        private TextBox txtEditPersonID;
        private Button btnEditPerson;

        // Common
        private Button btnClose;
        private Label lblTitle;

        #endregion

        public frmTest()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "DVLD - Comprehensive Test Form";
            this.Size = new Size(1150, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            lblTitle = new Label
            {
                Text = "DVLD Controls & Forms Tester",
                Font = new Font("Arial", 16F, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // ==================== SECTION 1: Simple Person Card ====================
            gbSimpleCard = new GroupBox
            {
                Text = "1. Simple ctrlPersonCard (Without Filter)",
                Location = new Point(20, 60),
                Size = new Size(520, 280),
                Font = new Font("Arial", 10F, FontStyle.Bold)
            };

            ctrlPersonCardSimple = new ctrlPersonCard
            {
                Location = new Point(20, 30),
                Size = new Size(480, 180)
            };

            txtSimplePersonID = new TextBox
            {
                Location = new Point(20, 225),
                Size = new Size(120, 25),
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };

            btnLoadSimple = new Button
            {
                Text = "Load Person",
                Location = new Point(150, 223),
                Size = new Size(120, 30),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnLoadSimple.Click += BtnLoadSimple_Click;

            gbSimpleCard.Controls.Add(ctrlPersonCardSimple);
            gbSimpleCard.Controls.Add(txtSimplePersonID);
            gbSimpleCard.Controls.Add(btnLoadSimple);

            // ==================== SECTION 2: Person Card With Filter ====================
            gbFilteredCard = new GroupBox
            {
                Text = "2. ctrlPersonCardWithFilter",
                Location = new Point(560, 60),
                Size = new Size(550, 280),
                Font = new Font("Arial", 10F, FontStyle.Bold)
            };

            ctrlPersonCardWithFilter1 = new ctrlPersonCardWithFilter
            {
                Location = new Point(20, 25)
            };

            // Subscribe to PersonLoaded event
            ctrlPersonCardWithFilter1.PersonLoaded += CtrlPersonCardWithFilter1_PersonLoaded;

            gbFilteredCard.Controls.Add(ctrlPersonCardWithFilter1);

            // ==================== SECTION 3: Add / Edit Person ====================
            gbAddEdit = new GroupBox
            {
                Text = "3. frmAddEditPerson (Add / Update)",
                Location = new Point(20, 360),
                Size = new Size(1090, 180),
                Font = new Font("Arial", 10F, FontStyle.Bold)
            };

            btnAddNewPerson = new Button
            {
                Text = "➕ Add New Person",
                Location = new Point(40, 40),
                Size = new Size(300, 50),
                Font = new Font("Arial", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White
            };
            btnAddNewPerson.Click += BtnAddNewPerson_Click;

            lblEditID = new Label
            {
                Text = "Edit Existing Person (Enter Person ID):",
                Location = new Point(40, 110),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            txtEditPersonID = new TextBox
            {
                Location = new Point(40, 135),
                Size = new Size(150, 28),
                Font = new Font("Microsoft Sans Serif", 10F)
            };
            txtEditPersonID.KeyPress += OnlyNumbers_KeyPress;

            btnEditPerson = new Button
            {
                Text = "✏ Edit Person",
                Location = new Point(210, 133),
                Size = new Size(130, 32),
                Font = new Font("Arial", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 150, 80),
                ForeColor = Color.White
            };
            btnEditPerson.Click += BtnEditPerson_Click;

            gbAddEdit.Controls.Add(btnAddNewPerson);
            gbAddEdit.Controls.Add(lblEditID);
            gbAddEdit.Controls.Add(txtEditPersonID);
            gbAddEdit.Controls.Add(btnEditPerson);

            // Close Button
            btnClose = new Button
            {
                Text = "Close Tester",
                Location = new Point(980, 630),
                Size = new Size(130, 40),
                BackColor = Color.LightCoral,
                ForeColor = Color.White,
                Font = new Font("Arial", 10F, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => this.Close();

            // Add everything to the form
            this.Controls.Add(lblTitle);
            this.Controls.Add(gbSimpleCard);
            this.Controls.Add(gbFilteredCard);
            this.Controls.Add(gbAddEdit);
            this.Controls.Add(btnClose);
        }

        // ====================== Event Handlers ======================

        private void BtnLoadSimple_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSimplePersonID.Text)) return;

            if (int.TryParse(txtSimplePersonID.Text.Trim(), out int id))
            {
                ctrlPersonCardSimple.LoadPersonInfo(id);
            }
            else
            {
                MessageBox.Show("Please enter a valid Person ID.", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddEditPerson frm = new frmAddEditPerson();
            frm.DataBack += (s, personID) => { /* Handle saved person ID if needed */ };
            frm.ShowDialog();
        }

        private void BtnEditPerson_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEditPersonID.Text))
            {
                MessageBox.Show("Please enter Person ID to edit.", "Input Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (int.TryParse(txtEditPersonID.Text.Trim(), out int personID))
            {
                frmAddEditPerson frm = new frmAddEditPerson(personID);
                frm.DataBack += (s, id) => { /* Handle saved person ID if needed */ };
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric Person ID.", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CtrlPersonCardWithFilter1_PersonLoaded(object sender, clsPerson person)
        {
            if (person != null)
            {
                MessageBox.Show($"Person Loaded Successfully!\n\n" +
                                $"ID       : {person.ID}\n" +
                                $"Name     : {person.FullName}\n" +
                                $"National No : {person.NationalNo}", 
                                "Person Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnlyNumbers_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}