using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    /// <summary>
    /// User control for displaying person information with filtering capabilities.
    /// Provides search by Person ID or National No, and displays person details.
    /// </summary>
    public class ctrlPersonCardWithFilter : UserControl
    {
        #region Controls Declaration

        private GroupBox gbFilter;
        private Label lblFindBy;
        private ComboBox cbFilters;
        private TextBox txtFilterValue;
        private Button btnFind;
        private Button btnAddNew;

        private ctrlPersonCard ctrlPersonCard1;

        #endregion

        #region Events

        /// <summary>Event raised when a person is successfully loaded.</summary>
        public event EventHandler<clsPerson> PersonLoaded;

        #endregion

        #region Properties

        /// <summary>Gets the currently selected person's ID.</summary>
        public int PersonID => ctrlPersonCard1.PersonID;

        /// <summary>Gets the currently selected person information.</summary>
        public clsPerson SelectedPersonInfo => ctrlPersonCard1.SelectedPersonInfo;


        /// <summary>Gets or sets the visibility of the filter group box.</summary>
        public bool FilterVisible
        {
            get => gbFilter.Visible;
            set => gbFilter.Visible = value;
        }

        #endregion


        public ctrlPersonCardWithFilter()
        {
            InitializeComponents();
            _SetupEvents();
        }

        #region Initialization

        private void InitializeComponents()
        {
            this.Size = new Size(850, 400);
            this.AutoScroll = true;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);
            this.BackColor = Color.FromArgb(240, 242, 248);

            // Filter GroupBox
            gbFilter = new GroupBox
            {
                Text = "Filter",
                Size = new Size(830, 70),
                Location = new Point(10, 10),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)            };

            lblFindBy = new Label
            {
                Text = "Find By:",
                Location = new Point(20, 30),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };

            cbFilters = new ComboBox
            {
                Location = new Point(85, 27),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };
            cbFilters.Items.AddRange(new object[] { "Person ID", "National No" });
            cbFilters.SelectedIndex = 0;

            txtFilterValue = new TextBox
            {
                Location = new Point(245, 27),
                Size = new Size(180, 25),
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };

            btnFind = new Button
            {
                Text = "Find",
                Location = new Point(440, 23),
                Size = new Size(70, 32),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFind.FlatAppearance.BorderSize = 0;

            btnAddNew = new Button
            {
                Text = "Add New",
                Location = new Point(520, 23),
                Size = new Size(80, 32),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddNew.FlatAppearance.BorderSize = 0;

            gbFilter.Controls.AddRange(new Control[] { lblFindBy, cbFilters, txtFilterValue, btnFind, btnAddNew });

            // Person Card below filter
            ctrlPersonCard1 = new ctrlPersonCard
            {
                Location = new Point(10, 90),
                Size = new Size(830, 300)
            };

            // Add all to the UserControl
            this.Controls.Add(gbFilter);
            this.Controls.Add(ctrlPersonCard1);
        }

        private void _SetupEvents()
        {
            btnFind.Click += btnFind_Click;
            btnAddNew.Click += btnAddNew_Click;
            txtFilterValue.KeyPress += txtFilterValue_KeyPress;
            txtFilterValue.TextChanged += txtFilterValue_TextChanged;
            cbFilters.SelectedIndexChanged += cbFilters_SelectedIndexChanged;
        }

        #endregion

        #region Event Handlers

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Only allow digits when filtering by Person ID
            if (cbFilters.Text == "Person ID")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            // Enable Find button only when there's text
            btnFind.Enabled = !string.IsNullOrWhiteSpace(txtFilterValue.Text);
        }

        private void cbFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear filter value when filter type changes
            txtFilterValue.Clear();
            txtFilterValue.Focus();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string filterValue = txtFilterValue.Text.Trim();

            if (string.IsNullOrEmpty(filterValue))
            {
                clsUtil.ShowError("Please enter a value to search.");
                txtFilterValue.Focus();
                return;
            }

            try
            {
                if (cbFilters.Text == "Person ID")
                {
                    if (!int.TryParse(filterValue, out int personID))
                    {
                        clsUtil.ShowError("Person ID must be a number.");
                        txtFilterValue.Focus();
                        return;
                    }
                    ctrlPersonCard1.LoadPersonInfo(personID);
                }
                else
                {
                    ctrlPersonCard1.LoadPersonInfo(filterValue);
                }

                // Raise event with the loaded person
                if (ctrlPersonCard1.SelectedPersonInfo != null)
                {
                    PersonLoaded?.Invoke(this, ctrlPersonCard1.SelectedPersonInfo);
                }
            }
            catch (Exception ex)
            {
                clsUtil.ShowError($"Error loading person: {ex.Message}");
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            using (var frm = new frmAddEditPerson())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the card with the newly created person
                    if (frm.PersonID > 0)
                    {
                        cbFilters.SelectedIndex = 0;
                        txtFilterValue.Text = frm.PersonID.ToString();
                        btnFind.PerformClick();
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Sets filter criteria and optionally triggers search.</summary>
        public void SetFilter(string value, string filterType, bool autoSearch = true)
        {
            cbFilters.SelectedItem = filterType;
            txtFilterValue.Text = value;
            if (autoSearch && !string.IsNullOrWhiteSpace(value))
            {
                btnFind.PerformClick();
            }
        }

        /// <summary>Loads person information by Person ID.</summary>
        public void LoadPersonInfo(int personID)
        {
            cbFilters.SelectedIndex = 0;
            txtFilterValue.Text = personID.ToString();
            ctrlPersonCard1.LoadPersonInfo(personID);
        }

        /// <summary>Clears the filter and resets the person card.</summary>
        public void Clear()
        {
            txtFilterValue.Clear();
            ctrlPersonCard1.ResetPersonInfo();
            txtFilterValue.Focus();
        }

        #endregion
    }
}