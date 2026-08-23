using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmEditTestType : Form
    {
        private Label lblTitle;
        private Label lblIDTitle, lblID;
        private Label lblTitleLbl, lblDescription, lblFees;

        private TextBox txtTitle;
        private TextBox txtDescription;
        private TextBox txtFees;

        private Button btnSave, btnClose;

        private readonly int _id;
        private clsTestType _testType;

        public frmEditTestType(int id)
        {
            _id = id;

            _Build();
            _LoadData();
        }

        private void _Build()
        {
            this.Text = "Edit Test Type";
            this.Size = new Size(560, 440);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label
            {
                Text = "Edit Test Type",
                Font = new Font("Arial", 15F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(180, 18)
            };

            lblIDTitle = new Label
            {
                Text = "Test Type ID:",
                AutoSize = true,
                Location = new Point(40, 75),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            lblID = new Label
            {
                Text = _id.ToString(),
                AutoSize = true,
                Location = new Point(190, 75),
                ForeColor = Color.SteelBlue,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            lblTitleLbl = new Label
            {
                Text = "Title:",
                AutoSize = true,
                Location = new Point(40, 118),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            txtTitle = new TextBox
            {
                Location = new Point(190, 115),
                Size = new Size(320, 23)
            };

            lblDescription = new Label
            {
                Text = "Description:",
                AutoSize = true,
                Location = new Point(40, 160),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            txtDescription = new TextBox
            {
                Location = new Point(190, 157),
                Size = new Size(320, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            lblFees = new Label
            {
                Text = "Fees (JD):",
                AutoSize = true,
                Location = new Point(40, 260),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            txtFees = new TextBox
            {
                Location = new Point(190, 257),
                Size = new Size(120, 23)
            };

            btnSave = _Btn("Save", 230, 345, Color.FromArgb(0, 120, 215));
            btnClose = _Btn("Close", 360, 345, Color.FromArgb(192, 50, 50));

            btnSave.Click += _Save;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblIDTitle,
                lblID,
                lblTitleLbl,
                txtTitle,
                lblDescription,
                txtDescription,
                lblFees,
                txtFees,
                btnSave,
                btnClose
            });
        }

        private void _LoadData()
        {
            _testType = clsTestType.Find(_id);

            if (_testType == null)
            {
                clsUtil.ShowError("Test type not found.");
                this.Close();
                return;
            }

            txtTitle.Text = _testType.Title;
            txtDescription.Text = _testType.Description;
            txtFees.Text = _testType.Fees.ToString("F2");
        }

        private void _Save(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                clsUtil.ShowWarning("Title is required.");

                txtTitle.BackColor = clsGlobal.InputError;
                txtTitle.Focus();

                return;
            }

            txtTitle.BackColor = clsGlobal.InputValid;

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                clsUtil.ShowWarning("Description is required.");

                txtDescription.BackColor = clsGlobal.InputError;
                txtDescription.Focus();

                return;
            }

            txtDescription.BackColor = clsGlobal.InputValid;


            if (!decimal.TryParse(txtFees.Text, out decimal fees) || fees < 0)
            {
                clsUtil.ShowWarning("Enter a valid fees amount.");


                txtFees.BackColor = clsGlobal.InputError;
                txtFees.Focus();

                return;
            }

            txtFees.BackColor = clsGlobal.InputValid;

            string newTitle = txtTitle.Text.Trim();

            clsTestType existing = clsTestType.Find(newTitle);

            if (existing != null && existing.ID != _id)
            {
                clsUtil.ShowWarning("Another test type already uses this title.");

                txtTitle.BackColor = clsGlobal.InputError;
                txtTitle.Focus();

                return;
            }

            _testType.Title = newTitle;
            _testType.Description = txtDescription.Text.Trim();
            _testType.Fees = fees;

            if (_testType.Save())
            {
                clsUtil.ShowInfo("Saved successfully.");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                clsUtil.ShowError("Save failed.");
            }
        }
        private static Button _Btn(string text, int x, int y, Color back)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(120, 34),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),

                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            b.FlatAppearance.BorderSize = 0;

            return b;
        }
    }
}