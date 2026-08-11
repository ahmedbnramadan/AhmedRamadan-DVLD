using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListDetainedLicenses : Form
    {
        private Label            lblTitle;
        private Label            lblFilterBy; private ComboBox cbFilterBy; private TextBox txtFilter;
        private DataGridView     dgv;
        private Button           btnRelease, btnClose;
        private Label            lblCount;
        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxRelease, ctxShowDetails;
        private DataTable        _full;

        public frmListDetainedLicenses() { _Build(); _LoadData(); }

        private void _Build()
        {
            this.Text = "Detained Licenses"; this.Size = new Size(1200, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label { Text = "Detained Licenses",
                Font = new Font("Arial", 18F, FontStyle.Bold), ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true, Location = new Point(490, 18) };

            lblFilterBy = new Label { Text = "Filter By:", AutoSize = true, Location = new Point(30,65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold) };
            cbFilterBy = new ComboBox { Location = new Point(110,62), Size = new Size(160,23),
                DropDownStyle = ComboBoxStyle.DropDownList, Cursor = Cursors.Hand };
            cbFilterBy.Items.AddRange(new object[] { "None","Detain ID","License ID","Driver Name","Reason" });
            cbFilterBy.SelectedIndex = 0;
            cbFilterBy.SelectedIndexChanged += (s, e) => {
                txtFilter.Visible = cbFilterBy.SelectedIndex > 0;
                txtFilter.Clear();
                if (!txtFilter.Visible) _BindGrid(_full);
            };
            txtFilter = new TextBox { Location = new Point(280,62), Size = new Size(220,23), Visible = false };
            txtFilter.TextChanged += (s, e) => _Filter();

            ctxMenu       = new ContextMenuStrip { Font = new Font("Microsoft Sans Serif", 9.5F) };
            ctxShowDetails = new ToolStripMenuItem("👤  Show License Details");
            ctxRelease    = new ToolStripMenuItem("✔  Release License");
            ctxShowDetails.Click += (s, e) => _ShowDetails();
            ctxRelease.Click     += (s, e) => _Release();
            ctxMenu.Items.AddRange(new ToolStripItem[] { ctxShowDetails, new ToolStripSeparator(), ctxRelease });

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
            dgv.SelectionChanged += (s, e) => { bool ok = dgv.SelectedRows.Count > 0; ctxRelease.Enabled = ok; ctxShowDetails.Enabled = ok; };

            lblCount  = new Label { Text = "Records: 0", AutoSize = true, Location = new Point(20,602), ForeColor = Color.Gray };

            btnRelease = _Btn("✔  Release",  920, 600, Color.FromArgb(0,140,60));
            btnClose   = _Btn("✖  Close",   1075, 600, Color.FromArgb(192,50,50));
            btnRelease.Click += (s, e) => _Release();
            btnClose.Click   += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, lblFilterBy, cbFilterBy, txtFilter, dgv, lblCount, btnRelease, btnClose });
        }

        private void _LoadData()
        {
            _full = clsDetainedLicense.GetAllDetainedLicenses();
            _BindGrid(_full);
        }

        private void _BindGrid(DataTable dt)
        {
            dgv.DataSource = dt;
            lblCount.Text  = $"Records: {dt.Rows.Count}";
            _Rename("DetainID",          "Detain ID");
            _Rename("LicenseID",         "License ID");
            _Rename("DriverName",        "Driver");
            _Rename("DetainDate",        "Detain Date");
            _Rename("DetainReason",      "Reason");
            _Rename("ReleaseDate",       "Release Date");
            _Rename("IsReleased",        "Released");
            _Hide("CreatedByUserID"); _Hide("ReleasedByUserID");
        }

        private void _Filter()
        {
            if (_full == null) return;
            string col = cbFilterBy.Text, val = txtFilter.Text.Trim();
            if (col == "None" || string.IsNullOrEmpty(val)) { _BindGrid(_full); return; }
            string dbCol = col switch {
                "Detain ID"   => "DetainID",
                "License ID"  => "LicenseID",
                "Driver Name" => "DriverName",
                "Reason"      => "DetainReason",
                _             => null
            };
            if (dbCol == null) return;
            try { var dv = new DataView(_full); dv.RowFilter = $"CONVERT([{dbCol}], System.String) LIKE '%{val}%'"; _BindGrid(dv.ToTable()); } catch { }
        }

        private int _SelectedDetainID()
        {
            if (dgv.SelectedRows.Count == 0) return -1;
            var c = dgv.SelectedRows[0].Cells["DetainID"];
            return c?.Value == null ? -1 : Convert.ToInt32(c.Value);
        }

        private void _ShowDetails()
        {
            if (dgv.SelectedRows.Count == 0) return;
            var licCell = dgv.SelectedRows[0].Cells["LicenseID"];
            if (licCell?.Value == null) return;
            int licID = Convert.ToInt32(licCell.Value);
            var lic = clsLicense.Find(licID);
            if (lic != null) new frmShowPersonInfo(lic.DriverID).ShowDialog();
        }

        private void _Release()
        {
            int id = _SelectedDetainID(); if (id < 0) return;
            var detained = clsDetainedLicense.Find(id);
            if (detained == null) return;
            if (detained.IsReleased) { clsUtil.ShowWarning("This license is already released."); return; }
            if (!clsUtil.ConfirmDelete("release this detained license")) return;
            detained.ReleaseDate      = DateTime.Now;
            detained.ReleasedByUserID = clsGlobal.CurrentUserID;
            if (detained.Release(detained.ID, detained.ReleaseApplicationID ?? -1 )) { clsUtil.ShowInfo("License released successfully."); _LoadData(); }
            else clsUtil.ShowError("Failed to release license.");
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