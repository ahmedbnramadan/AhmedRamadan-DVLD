using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListApplicationTypes : Form
    {
        #region Controls
        private Label            lblTitle;
        private DataGridView     dgv;
        private Button           btnAddNew, btnClose;
        private Label            lblCount;
        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxEdit, ctxAddNew;
        #endregion

        public frmListApplicationTypes()
        {
            _Build();
            _Load();
        }

        private void _Build()
        {
            this.Text = "Manage Application Types";
            this.Size = new Size(700, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label
            {
                Text = "Application Types",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(220, 18)
            };

            // Context menu
            ctxMenu  = new ContextMenuStrip { Font = new Font("Microsoft Sans Serif", 9.5F) };
            ctxAddNew = new ToolStripMenuItem("➕  Add New");
            ctxEdit   = new ToolStripMenuItem("✏️  Edit");
            // ctxDelete = new ToolStripMenuItem("🗑  Delete");
            ctxAddNew.Click += (s, e) => _OpenAddEdit(-1);
            ctxEdit.Click   += (s, e) => _OpenAddEdit(_SelectedID());
            // ctxDelete.Click += (s, e) => _Delete();
            ctxMenu.Items.AddRange(new ToolStripItem[] { ctxAddNew, ctxEdit });

            dgv = new DataGridView
            {
                Location = new Point(20, 70), Size = new Size(645, 360),
                ReadOnly = true, AllowUserToAddRows = false,// AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 34, RowTemplate = { Height = 28 },
                BorderStyle = BorderStyle.None, BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220, 225, 235),
                ContextMenuStrip = ctxMenu, Cursor = Cursors.Hand
            };
            _StyleGrid(dgv);
            dgv.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) _OpenAddEdit(_SelectedID()); };
            dgv.MouseDown       += _GridMouseDown;
            dgv.SelectionChanged += (s, e) => { bool ok = dgv.SelectedRows.Count > 0; ctxEdit.Enabled = ok;/*ctxDelete.Enabled = ok; */  };

            lblCount = new Label { Text = "Records: 0", AutoSize = true, Location = new Point(20, 440), ForeColor = Color.Gray };

            btnAddNew = _Btn("➕  Add New", 380, 438, Color.FromArgb(0, 120, 215));
            btnAddNew.Click += (s, e) => _OpenAddEdit(-1);

            btnClose = _Btn("✖  Close", 545, 438, Color.FromArgb(192, 50, 50));
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, dgv, lblCount, btnAddNew, btnClose });
        }

        private void _Load()
        {
            dgv.DataSource = clsApplicationType.GetAllApplicationTypes();
            lblCount.Text  = $"Records: {dgv.Rows.Count}";
            _RenameCol("ApplicationTypeID",    "ID");
            _RenameCol("ApplicationTypeTitle", "Title");
            _RenameCol("ApplicationFees",      "Fees (JD)");
        }

        private int _SelectedID()
        {
            if (dgv.SelectedRows.Count == 0) return -1;
            return Convert.ToInt32(dgv.SelectedRows[0].Cells["ApplicationTypeID"].Value);
        }

        private void _OpenAddEdit(int id)
        {
            new frmAddEditApplicationType(id).ShowDialog();
            _Load();
        }

        // private void _Delete()
        // {
        //     int id = _SelectedID();
        //     if (id == -1) return;
        //     if (!clsUtil.ConfirmDelete("this application type")) return;
        //     if (clsApplicationType.Delete(id))
        //         _Load();
        //     else
        //         clsUtil.ShowError("Cannot delete — it may be linked to existing applications.");
        // }

        private void _GridMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var hit = dgv.HitTest(e.X, e.Y);
            if (hit.RowIndex >= 0) dgv.Rows[hit.RowIndex].Selected = true;
        }

        private void _RenameCol(string data, string display)
        { if (dgv.Columns.Contains(data)) dgv.Columns[data].HeaderText = display; }

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

        private static Button _Btn(string text, int x, int y, Color back)
        {
            var b = new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(150, 34),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = back, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }
    }
}