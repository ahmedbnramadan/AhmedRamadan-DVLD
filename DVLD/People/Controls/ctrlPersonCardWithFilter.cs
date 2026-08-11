using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
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

        // [Edited by Assistant] - Added PersonLoaded event
        public event EventHandler<clsPerson> PersonLoaded;

        public int PersonID => ctrlPersonCard1.PersonID;
        public clsPerson SelectedPersonInfo => ctrlPersonCard1.SelectedPersonInfo;

        public ctrlPersonCardWithFilter()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(850, 400);
            this.AutoScroll = true;

            // 1. GroupBox للفلتر
            gbFilter = new GroupBox
            {
                Text = "Filter",
                Size = new Size(830, 70),
                Location = new Point(10, 10)
            };

            lblFindBy = new Label { Text = "Find By:", Location = new Point(20, 30), AutoSize = true, Font = new Font("Arial", 9, FontStyle.Bold) };

            cbFilters = new ComboBox
            {
                Location = new Point(85, 27),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbFilters.Items.AddRange(new object[] { "Person ID", "National No" });
            cbFilters.SelectedIndex = 0;

            txtFilterValue = new TextBox { Location = new Point(245, 27), Size = new Size(180, 25) };
            txtFilterValue.KeyPress += txtFilterValue_KeyPress;

            btnFind = new Button
            {
                Text = "Find",
                Location = new Point(440, 23),
                Size = new Size(70, 32),
                BackColor = Color.White
            };
            btnFind.Click += btnFind_Click;

            btnAddNew = new Button
            {
                Text = "Add New",
                Location = new Point(520, 23),
                Size = new Size(80, 32),
                BackColor = Color.White
            };
            btnAddNew.Click += (s, e) => { /* ارفع حدث هنا لفتح فورم الإضافة */ };

            gbFilter.Controls.AddRange(new Control[] { lblFindBy, cbFilters, txtFilterValue, btnFind, btnAddNew });

            // 2. إضافة بطاقة الشخص أسفل الفلتر
            ctrlPersonCard1 = new ctrlPersonCard
            {
                Location = new Point(10, 90)
            };

            // إضافة الكل للـ User Control
            this.Controls.Add(gbFilter);
            this.Controls.Add(ctrlPersonCard1);
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilters.Text == "Person ID")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        // [Edited by Assistant] - Modified to raise PersonLoaded event after successful load
        private void btnFind_Click(object sender, EventArgs e)
        {
            string filterValue = txtFilterValue.Text.Trim();
            if (string.IsNullOrEmpty(filterValue))
                return;

            try
            {
                if (cbFilters.Text == "Person ID")
                    ctrlPersonCard1.LoadPersonInfo(int.Parse(filterValue));
                else
                    ctrlPersonCard1.LoadPersonInfo(filterValue);

                // Raise event with the loaded person
                PersonLoaded?.Invoke(this, ctrlPersonCard1.SelectedPersonInfo);
            }
            catch (Exception ex)
            {
                clsUtil.ShowError($"Error loading person: {ex.Message}");
            }
        }

        // [Edited by Assistant] - Added SetFilter method
        public void SetFilter(string value, string filterType)
        {
            cbFilters.SelectedItem = filterType;
            txtFilterValue.Text = value;
            btnFind.PerformClick();
        }

        public void LoadPersonInfo(int PersonID)
        {
            cbFilters.SelectedIndex = 0;
            txtFilterValue.Text = PersonID.ToString();
            ctrlPersonCard1.LoadPersonInfo(PersonID);
        }
    }
}