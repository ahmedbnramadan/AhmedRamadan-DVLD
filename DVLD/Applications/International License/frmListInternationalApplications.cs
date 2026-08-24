using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListInternationalApplications : Form
    {
        private Label            lblTitle;
        private Label            lblFilterBy; private ComboBox cbFilterBy; private TextBox txtFilter;
        private DataGridView     dgv;
        private Button           btnAddNew, btnClose;
        private Label            lblCount;
        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxShowDetails, ctxAddNew;
        private DataTable        _full;

        public frmListInternationalApplications() { _Build(); _LoadData(); }

        private void _Build()
        {
            this.Text = "International Driving License Applications"; this.Size = new Size(1200, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "International License Applications",
                Font = new Font("Arial", 18F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(370, 18) };

            lblFilterBy = new Label { Text = "Filter By:", AutoSize = true, Location = new Point(30,65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            cbFilterBy = new ComboBox { Location = new Point(110,62), Size = new Size(160,23),
                DropDownStyle = ComboBoxStyle.DropDownList, Cursor = Cursors.Hand };
            cbFilterBy.Items.AddRange(new object[] { "None","License ID","Driver Name","National No.","Status" });
            cbFilterBy.SelectedIndex = 0;
            cbFilterBy.SelectedIndexChanged += (s, e) => {
                txtFilter.Visible = cbFilterBy.SelectedIndex > 0;
                txtFilter.Clear();
                if (!txtFilter.Visible) _BindGrid(_full);
            };
            txtFilter = new TextBox { Location = new Point(280,62), Size = new Size(220,23), Visible = false };
            txtFilter.TextChanged += (s, e) => _Filter();

            ctxMenu       = new ContextMenuStrip { Font = new Font("Microsoft Sans Serif", 9.5F) };
            ctxShowDetails = new ToolStripMenuItem("👤  Show Details");
            ctxAddNew      = new ToolStripMenuItem("➕  New International License");
            ctxShowDetails.Click += (s, e) => _ShowDetails();
            ctxAddNew.Click      += (s, e) => _OpenAddNew();
            ctxMenu.Items.AddRange(new ToolStripItem[] { ctxShowDetails, new ToolStripSeparator(), ctxAddNew });

            dgv = new DataGridView {
                Location = new Point(20,100), Size = new Size(1150,490),
                ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 34, RowTemplate = { Height = 28 },
                BorderStyle = BorderStyle.None, BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220,225,235), ContextMenuStrip = ctxMenu, Cursor = Cursors.Hand
            };
            _StyleGrid(dgv);
            dgv.CellDoubleClick  += (s, e) => { if (e.RowIndex >= 0) _ShowDetails(); };
            dgv.MouseDown        += (s, e) => { if (e.Button == MouseButtons.Right) { var h = dgv.HitTest(e.X,e.Y); if (h.RowIndex >= 0) dgv.Rows[h.RowIndex].Selected = true; } };
            dgv.SelectionChanged += (s, e) => { ctxShowDetails.Enabled = dgv.SelectedRows.Count > 0; };

            lblCount  = new Label { Text = "Records: 0", AutoSize = true, Location = new Point(20,602), ForeColor = Color.Gray };

            btnAddNew = _Btn("➕  Add New", 920, 600, Color.FromArgb(0,120,215));
            btnClose  = _Btn("✖  Close",  1075, 600, Color.FromArgb(192,50,50));
            btnAddNew.Click += (s, e) => _OpenAddNew();
            btnClose.Click  += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, lblFilterBy, cbFilterBy, txtFilter, dgv, lblCount, btnAddNew, btnClose });
        }

        private void _LoadData()
        {
            _full = clsInternationalLicense.GetAllInternationalLicenses();
            _BindGrid(_full);
        }

        private void _BindGrid(DataTable dt)
        {
            dgv.DataSource = dt;
            lblCount.Text  = $"Records: {dt.Rows.Count}";
            _Rename("InternationalLicenseID", "Intl. License ID");
            _Rename("LocalLicenseID",         "Local License ID");
            _Rename("DriverName",             "Driver");
            _Rename("NationalNo",             "National No.");
            _Rename("ApplicationDate",        "App. Date");
            _Rename("IssueDate",              "Issue Date");
            _Rename("ExpirationDate",         "Expiry Date");
            _Rename("IsActive",               "Active");
            _Rename("PaidFees",               "Fees (US)");
            _Hide("DriverID"); _Hide("CreatedByUserID");
        }

        private void _Filter()
        {
            if (_full == null) return;
            string col = cbFilterBy.Text, val = txtFilter.Text.Trim();
            if (col == "None" || string.IsNullOrEmpty(val)) { _BindGrid(_full); return; }
            string dbCol = col switch {
                "License ID"   => "InternationalLicenseID",
                "Driver Name"  => "DriverName",
                "National No." => "NationalNo",
                "Status"       => "IsActive",
                _              => null
            };
            if (dbCol == null) return;
            try { var dv = new DataView(_full); dv.RowFilter = $"CONVERT([{dbCol}], System.String) LIKE '%{val}%'"; _BindGrid(dv.ToTable()); } catch { }
        }

        private void _ShowDetails()
        {
            if (dgv.SelectedRows.Count == 0) return;
            var c = dgv.SelectedRows[0].Cells["DriverID"];
            if (c?.Value != null) new frmShowPersonInfo(Convert.ToInt32(c.Value)).ShowDialog();
        }

        private void _OpenAddNew()
        {
            new frmAddNewInternationalLicense().ShowDialog();
            _LoadData();
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
        private static Button _Btn(string t, int x, int y, Color c)
        {
            var b = new Button { Text = t, Location = new Point(x,y), Size = new Size(150,34),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }

}