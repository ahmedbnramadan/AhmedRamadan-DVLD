using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListLocalDrivingLicenseApplications : Form
    {
        #region Controls
        private Label            lblTitle;
        private Label            lblFilterBy;
        private ComboBox         cbFilterBy;
        private TextBox          txtFilter;
        private DataGridView     dgv;
        private Button           btnAddNew, btnClose;
        private Label            lblCount;
        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxShowDetails, ctxAddNew, ctxEdit, ctxDelete;
        #endregion

        private DataTable _full;

        public frmListLocalDrivingLicenseApplications()
        {
            _Build();
            _LoadData();
        }

        private void _Build()
        {
            this.Text = "Local Driving License Applications";
            this.Size = new Size(1200, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Local Driving License Applications",
                Font = new Font("Arial", 18F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(330, 18) };

            lblFilterBy = new Label { Text = "Filter By:", AutoSize = true, Location = new Point(30, 65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };

            cbFilterBy = new ComboBox { Location = new Point(110, 62), Size = new Size(160, 23),
                DropDownStyle = ComboBoxStyle.DropDownList, Cursor = Cursors.Hand };
            cbFilterBy.Items.AddRange(new object[] {
                "None","Application ID","Person ID","National No.","Full Name","License Class","Status" });
            cbFilterBy.SelectedIndex = 0;
            cbFilterBy.SelectedIndexChanged += (s, e) => {
                txtFilter.Visible = cbFilterBy.SelectedIndex > 0;
                txtFilter.Clear();
                if (!txtFilter.Visible) _BindGrid(_full);
            };

            txtFilter = new TextBox { Location = new Point(280, 62), Size = new Size(220, 23), Visible = false };
            txtFilter.TextChanged += (s, e) => _Filter();

            // Context menu
            ctxMenu = new ContextMenuStrip { Font = new Font("Microsoft Sans Serif", 9.5F) };
            ctxShowDetails = new ToolStripMenuItem("👤  Show Details");
            ctxAddNew      = new ToolStripMenuItem("➕  Add New");
            ctxEdit        = new ToolStripMenuItem("✏️  Edit");
            ctxDelete      = new ToolStripMenuItem("🗑  Delete");
            ctxShowDetails.Click += (s, e) => _ShowDetails();
            ctxAddNew.Click      += (s, e) => _OpenAddNew();
            ctxEdit.Click        += (s, e) => _OpenEdit();
            ctxDelete.Click      += (s, e) => _Delete();
            ctxMenu.Items.AddRange(new ToolStripItem[] {
                ctxShowDetails, new ToolStripSeparator(), ctxAddNew, ctxEdit, ctxDelete });

            dgv = new DataGridView {
                Location = new Point(20, 100), Size = new Size(1150, 490),
                ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 34, RowTemplate = { Height = 28 },
                BorderStyle = BorderStyle.None, BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220, 225, 235),
                ContextMenuStrip = ctxMenu, Cursor = Cursors.Hand
            };
            _StyleGrid(dgv);
            dgv.CellDoubleClick  += (s, e) => { if (e.RowIndex >= 0) _ShowDetails(); };
            dgv.MouseDown        += (s, e) => {
                if (e.Button == MouseButtons.Right) {
                    var h = dgv.HitTest(e.X, e.Y);
                    if (h.RowIndex >= 0) dgv.Rows[h.RowIndex].Selected = true;
                }
            };
            dgv.SelectionChanged += (s, e) => {
                bool ok = dgv.SelectedRows.Count > 0;
                ctxShowDetails.Enabled = ok; ctxEdit.Enabled = ok; ctxDelete.Enabled = ok;
            };

            lblCount = new Label { Text = "Records: 0", AutoSize = true,
                Location = new Point(20, 602), ForeColor = Color.Gray };

            btnAddNew = _Btn("➕  Add New", 925, 600, Color.FromArgb(0, 120, 215));
            btnAddNew.Click += (s, e) => _OpenAddNew();
            btnClose  = _Btn("✖  Close",  1090, 600, Color.FromArgb(192, 50, 50));
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblFilterBy, cbFilterBy, txtFilter, dgv, lblCount, btnAddNew, btnClose });
        }

        private void _LoadData()
        {
            _full = clsLocalDrivingLicenseApplication.GetAllLocalDrivingLicenseApplications();
            _BindGrid(_full);
        }

        private void _BindGrid(DataTable dt)
        {
            dgv.DataSource  = dt;
            lblCount.Text   = $"Records: {dt.Rows.Count}";
            _Rename("localdrivinglicenseapplicationid", "App ID");
            _Rename("classname",                 "License Class");
            _Rename("nationalno",                       "National No.");
            _Rename("fullname",                         "Full Name");
            _Rename("applicationdate",                  "Date");
            _Rename("passedtestscount",                 "Passed Tests");
            _Rename("status",                           "Status");
        }

        private void _Filter()
        {
            if (_full == null) return;
            string col = cbFilterBy.Text, val = txtFilter.Text.Trim();
            if (col == "None" || string.IsNullOrEmpty(val)) { _BindGrid(_full); return; }
            string dbCol = col switch {
                "Application ID" => "localdrivinglicenseapplicationid",
                "National No."   => "nationalno",
                "Full Name"      => "fullname",
                "License Class"  => "classname",
                "Status"         => "status",
                _                => null
            };
            if (dbCol == null) return;
            try {
                var dv = new DataView(_full);
                dv.RowFilter = $"CONVERT([{dbCol}], System.String) LIKE '%{val}%'";
                _BindGrid(dv.ToTable());
            } catch { }
        }

        private int _SelectedID()
        {
            if (dgv.SelectedRows.Count == 0) return -1;
            var c = dgv.SelectedRows[0].Cells["LocalDrivingLicenseApplicationID"];
            return c?.Value == null ? -1 : Convert.ToInt32(c.Value);
        }

        private void _ShowDetails()
        {
            int id = _SelectedID(); if (id < 0) return;
            new frmShowLocalDrivingLicenseApplicationInfo(id).ShowDialog();
        }

        private void _OpenAddNew()
        {
            new frmAddEditNewLocalDrivingLicenseApplication().ShowDialog();
            _LoadData();
        }

        private void _OpenEdit()
        {
            int id = _SelectedID(); if (id < 0) return;
            new frmAddEditNewLocalDrivingLicenseApplication(id).ShowDialog();
            _LoadData();
        }

        private void _Delete()
        {
            int id = _SelectedID(); if (id < 0) return;
            if (!clsUtil.ConfirmDelete("this application")) return;
            clsLocalDrivingLicenseApplication Application = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppID(id);
            if (Application.Delete()) _LoadData();
            else clsUtil.ShowError("Cannot delete — application may have linked records.");
        }

        private void _Rename(string d, string h) { if (dgv.Columns.Contains(d)) dgv.Columns[d].HeaderText = h; }
        private static void _StyleGrid(DataGridView g)
        {
            g.ColumnHeadersDefaultCellStyle.BackColor = clsGlobal.GridHeaderBack;
            g.ColumnHeadersDefaultCellStyle.ForeColor = clsGlobal.GridHeaderFore;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold);
            g.EnableHeadersVisualStyles = false;
            g.DefaultCellStyle.SelectionBackColor = clsGlobal.GridSelectionBack;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
        }

        private static Button _Btn(string t, int x, int y, Color c)
        {
            var b = new Button { Text = t, Location = new Point(x, y), Size = new Size(150, 34),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }
}