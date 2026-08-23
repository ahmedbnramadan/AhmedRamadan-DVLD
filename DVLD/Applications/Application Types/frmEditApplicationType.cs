using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmEditApplicationType : Form
    {
        private Label    lblTitle, lblIDTitle, lblID, lblTitleLbl, lblFees;
        private TextBox  txtTitle, txtFees;
        private Button   btnSave, btnClose;

        private readonly int          _id;
        private clsApplicationType    _appType;

        public frmEditApplicationType(int id)
        {
            _id = id;
            _Build();
            _LoadData();
        }

        private void _Build()
        {
            this.Text = "Edit Application Type";
            this.Size = new Size(480, 310);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Edit Application Type",
                Font = new Font("Arial", 15F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(130, 18) };

            lblIDTitle = new Label { Text = "Type ID:", AutoSize = true, Location = new Point(40, 75),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            lblID = new Label { Text =  _id.ToString(), AutoSize = true,
                Location = new Point(170, 75), ForeColor = Color.SteelBlue,
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };

            lblTitleLbl = new Label { Text = "Title:", AutoSize = true, Location = new Point(40, 118),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            txtTitle = new TextBox { Location = new Point(170, 115), Size = new Size(260, 23) };

            lblFees = new Label { Text = "Fees (US):", AutoSize = true, Location = new Point(40, 160),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            txtFees = new TextBox { Location = new Point(170, 157), Size = new Size(120, 23) };

            btnSave  = _Btn("Save",  185, 215, Color.FromArgb(0, 120, 215));
            btnClose = _Btn("Close", 315, 215, Color.FromArgb(192, 50, 50));
            btnSave.Click  += _Save;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblIDTitle, lblID, lblTitleLbl, txtTitle, lblFees, txtFees, btnSave, btnClose
            });
        }

        private void _LoadData()
        {
            _appType = clsApplicationType.Find(_id);
            if (_appType == null) { clsUtil.ShowError("Type not found."); this.Close(); return; }
            txtTitle.Text = _appType.Title;
            txtFees.Text  = _appType.Fees.ToString("F2");
        }

        private void _Save(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            { clsUtil.ShowWarning("Title is required."); txtTitle.BackColor = clsGlobal.InputError; return; }
            txtTitle.BackColor = clsGlobal.InputValid;

            if (!decimal.TryParse(txtFees.Text, out decimal fees) || fees < 0)
            { clsUtil.ShowWarning("Enter a valid fees amount."); txtFees.BackColor = clsGlobal.InputError; return; }
            txtFees.BackColor = clsGlobal.InputValid;

            _appType = clsApplicationType.Find(_id);

            _appType.Title = txtTitle.Text.Trim();
            _appType.Fees      = fees;

            if (_appType.Save())
            { clsUtil.ShowInfo("Saved successfully."); this.DialogResult = DialogResult.OK; this.Close(); }
            else
                clsUtil.ShowError("Save failed.");
        }

        private static Button _Btn(string t, int x, int y, Color c)
        {
            var b = new Button { Text = t, Location = new Point(x, y), Size = new Size(120, 34),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }
}