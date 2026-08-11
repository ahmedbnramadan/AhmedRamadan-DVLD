using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListDrivers : Form
    {
        private Label            lblTitle;
        private Label            lblFilterBy; private ComboBox cbFilterBy; private TextBox txtFilter;
        private DataGridView     dgv;
        private Button           btnClose;
        private Label            lblCount;
        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxShowDetails, ctxSendEmail, ctxPhoneCall;
        private DataTable _full;

        public frmListDrivers() { _Build(); _LoadData(); }

        private void _Build()
        {
            this.Text = "Manage Drivers"; this.Size = new Size(1200, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Manage Drivers",
                Font = new Font("Arial", 18F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(480, 18) };

            lblFilterBy = new Label { Text = "Filter By:", AutoSize = true, Location = new Point(30,65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            cbFilterBy = new ComboBox { Location = new Point(110,62), Size = new Size(160,23),
                DropDownStyle = ComboBoxStyle.DropDownList, Cursor = Cursors.Hand };
            cbFilterBy.Items.AddRange(new object[] { "None","Driver ID","National No.","Full Name","License Class" });
            cbFilterBy.SelectedIndex = 0;
            cbFilterBy.SelectedIndexChanged += (s, e) => {
                txtFilter.Visible = cbFilterBy.SelectedIndex > 0;
                txtFilter.Clear();
                if (!txtFilter.Visible) _BindGrid(_full);
            };
            txtFilter = new TextBox { Location = new Point(280,62), Size = new Size(220,23), Visible = false };
            txtFilter.TextChanged += (s, e) => _Filter();

            ctxMenu = new ContextMenuStrip { Font = new Font("Microsoft Sans Serif", 9.5F) };
            ctxShowDetails = new ToolStripMenuItem("👤  Show Details");
            ctxSendEmail   = new ToolStripMenuItem("📧  Send Email");
            ctxPhoneCall   = new ToolStripMenuItem("📞  Phone Call");
            ctxShowDetails.Click += (s, e) => _ShowDetails();
            ctxSendEmail.Click   += (s, e) => {
                if (dgv.SelectedRows.Count == 0) return;
                clsUtil.SendEmail(dgv.SelectedRows[0].Cells["Email"]?.Value?.ToString() ?? "");
            };
            ctxPhoneCall.Click += (s, e) => {
                if (dgv.SelectedRows.Count == 0) return;
                clsUtil.MakePhoneCall(dgv.SelectedRows[0].Cells["Phone"]?.Value?.ToString() ?? "");
            };
            ctxMenu.Items.AddRange(new ToolStripItem[] { ctxShowDetails, new ToolStripSeparator(), ctxSendEmail, ctxPhoneCall });

            dgv = new DataGridView {
                Location = new Point(20,100), Size = new Size(1150,490),
                ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 34, RowTemplate = { Height = 28 },
                BorderStyle = BorderStyle.None, BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220,225,235), ContextMenuStrip = ctxMenu, Cursor = Cursors.Hand
            };
            _StyleGrid(dgv);
            dgv.CellDoubleClick  += (s, e) => { if (e.RowIndex >= 0) _ShowDetails(); };
            dgv.MouseDown        += (s, e) => {
                if (e.Button == MouseButtons.Right) { var h = dgv.HitTest(e.X,e.Y); if (h.RowIndex >= 0) dgv.Rows[h.RowIndex].Selected = true; }
            };
            dgv.SelectionChanged += (s, e) => { bool ok = dgv.SelectedRows.Count > 0; ctxShowDetails.Enabled = ok; ctxSendEmail.Enabled = ok; ctxPhoneCall.Enabled = ok; };

            lblCount = new Label { Text = "Records: 0", AutoSize = true, Location = new Point(20,602), ForeColor = Color.Gray };

            btnClose = new Button { Text = "✖  Close", Location = new Point(1075,600), Size = new Size(95,34),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(192,50,50), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, lblFilterBy, cbFilterBy, txtFilter, dgv, lblCount, btnClose });
        }

        private void _LoadData()
        {
            _full = clsDriver.ViewAllDrivers();
            _BindGrid(_full);
        }

        private void _BindGrid(DataTable dt)
        {
            dgv.DataSource = dt;
            lblCount.Text  = $"Records: {dt.Rows.Count}";
            _Rename("DriverID",     "Driver ID");
            _Rename("FullName",     "Full Name");
            _Rename("NationalNo",   "National No.");
            _Rename("DateOfBirth",  "Date of Birth");
            _Rename("Nationality",  "Nationality");
            _Hide("PersonID"); _Hide("ImagePath"); _Hide("Address");
        }

        private void _Filter()
        {
            if (_full == null) return;
            string col = cbFilterBy.Text, val = txtFilter.Text.Trim();
            if (col == "None" || string.IsNullOrEmpty(val)) { _BindGrid(_full); return; }
            string dbCol = col switch {
                "Driver ID"    => "DriverID",
                "National No." => "NationalNo",
                "Full Name"    => "FullName",
                "License Class"=> "LicenseClassName",
                _              => null
            };
            if (dbCol == null) return;
            try { var dv = new DataView(_full); dv.RowFilter = $"CONVERT([{dbCol}], System.String) LIKE '%{val}%'"; _BindGrid(dv.ToTable()); } catch { }
        }

        private void _ShowDetails()
        {
            if (dgv.SelectedRows.Count == 0) return;
            int pid = Convert.ToInt32(dgv.SelectedRows[0].Cells["PersonID"]?.Value ?? -1);
            if (pid > 0) new frmShowPersonInfo(pid).ShowDialog();
        }

        private void _Rename(string d, string h) { if (dgv.Columns.Contains(d)) dgv.Columns[d].HeaderText = h; }
        private void _Hide(string n)              { if (dgv.Columns.Contains(n)) dgv.Columns[n].Visible = false; }

        private static void _StyleGrid(DataGridView g)
        {
            g.ColumnHeadersDefaultCellStyle.BackColor = clsGlobal.GridHeaderBack;
            g.ColumnHeadersDefaultCellStyle.ForeColor = clsGlobal.GridHeaderFore;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold);
            g.EnableHeadersVisualStyles = false;
            g.DefaultCellStyle.SelectionBackColor = clsGlobal.GridSelectionBack;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245,248,255);
        }
    }
}