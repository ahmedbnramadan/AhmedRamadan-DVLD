using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListTestTypes : Form
    {
        #region Controls
        private Label lblTitle;
        private DataGridView dgv;
        private Button btnClose;
        private Label lblCount;
        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxEdit;
        #endregion

        public frmListTestTypes()
        {
            _Build();
            _Load();
        }

        private void _Build()
        {
            this.Text = "Manage Test Types";
            this.Size = new Size(700, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label
            {
                Text = "Test Types",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(240, 18)
            };
            // Context menu
            ctxMenu = new ContextMenuStrip
            {
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };

            ctxEdit = new ToolStripMenuItem("Edit");
            ctxEdit.Click += (s, e) => _OpenEdit(_SelectedID());

            ctxMenu.Items.AddRange(new ToolStripItem[] { ctxEdit });

            dgv = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(645, 360),

                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,

                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,

                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,

                ColumnHeadersHeight = 34,
                RowTemplate = { Height = 28 },

                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220, 225, 235),

                ContextMenuStrip = ctxMenu,
                Cursor = Cursors.Hand
            };

            _StyleGrid(dgv);

            dgv.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    _OpenEdit(_SelectedID());
            };

            dgv.MouseDown += _GridMouseDown;

            dgv.SelectionChanged += (s, e) =>
            {
                bool hasSelection = dgv.SelectedRows.Count > 0;
                ctxEdit.Enabled = hasSelection;
            };

            lblCount = new Label
            {
                Text = "Records: 0",
                AutoSize = true,
                Location = new Point(20, 440),
                ForeColor = Color.Gray
            };

            btnClose = _Btn("✖  Close", 525, 438, Color.FromArgb(192, 50, 50));


            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                dgv,
                lblCount,
                btnClose
            });
        }

        private void _Load()
        {
            dgv.DataSource = clsTestType.GetAllTestTypes();

            lblCount.Text = $"Records: {dgv.Rows.Count}";

            _RenameCol("ID", "ID");
            _RenameCol("Title", "Title");
            _RenameCol("Description", "Description");
            _RenameCol("Fees", "Fees (JD)");

            // ID
            if (dgv.Columns.Contains("ID"))
            {
                dgv.Columns["ID"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;

                dgv.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns["ID"].Width = 80;
            }

            // Fees
            if (dgv.Columns.Contains("Fees"))
            {
                dgv.Columns["Fees"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns["Fees"].Width = 150;
                dgv.Columns["Fees"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["Fees"].DefaultCellStyle.Format = "N2";
            }

            // Description takes the remaining space
            if (dgv.Columns.Contains("Title"))
            {
                dgv.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private int _SelectedID()
        {
            if (dgv.SelectedRows.Count == 0) return -1;
            return Convert.ToInt32(dgv.SelectedRows[0].Cells["ID"].Value);
        }

        private void _OpenEdit(int id)
        {
            new frmEditTestType(id).ShowDialog();
            _Load();

        }

        private void _GridMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var hit = dgv.HitTest(e.X, e.Y);
            if (hit.RowIndex >= 0) dgv.Rows[hit.RowIndex].Selected = true;
        }

        private void _RenameCol(string data, string display)
        {
            if (dgv.Columns.Contains(data))
                dgv.Columns[data].HeaderText = display;
        }

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
                Text = text,
                Location = new Point(x, y),
                Size = new Size(150, 34),
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