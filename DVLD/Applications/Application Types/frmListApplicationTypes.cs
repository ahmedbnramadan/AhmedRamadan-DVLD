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

        private Label lblTitle;
        private DataGridView dgv;
        private Button btnClose;
        private Label lblCount;

        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxEdit;

        #endregion

        #region Constants

        private const int FormWidth = 950;
        private const int FormHeight = 620;

        private const int GridMargin = 20;
        private const int GridTop = 70;
        private const int BottomAreaHeight = 60;

        #endregion

        #region Column Setup

        // Declarative column configuration: field name in the DataTable,
        // header to display, alignment, width (ignored when filling),
        // whether it should fill the remaining space, and an optional
        // display format. Applied in a single loop instead of separate
        // rename / width methods per column.
        private struct ColumnSetup
        {
            public string DataField;
            public string Header;
            public DataGridViewContentAlignment Alignment;
            public int Width;
            public bool Fill;
            public string Format;

            public ColumnSetup(
                string dataField,
                string header,
                DataGridViewContentAlignment alignment,
                int width,
                bool fill = false,
                string format = null)
            {
                DataField = dataField;
                Header = header;
                Alignment = alignment;
                Width = width;
                Fill = fill;
                Format = format;
            }
        }

        // Order in this array = display order (ID, Title, Fees).
        // Data field names must match the columns returned by
        // clsApplicationType.GetAllApplicationTypes().
        private static readonly ColumnSetup[] ColumnLayout =
        {
            new ColumnSetup(
                "ApplicationTypeID",
                "ID",
                DataGridViewContentAlignment.MiddleCenter,
                width: 60),

            new ColumnSetup(
                "ApplicationTypeTitle",
                "Title",
                DataGridViewContentAlignment.MiddleLeft,
                width: 0,
                fill: true),

            new ColumnSetup(
                "ApplicationFees",
                "Fees (US)",
                DataGridViewContentAlignment.MiddleLeft,
                width: 120,
                format: "N2")
        };

        #endregion

        public frmListApplicationTypes()
        {
            _Build();
            _Load();
        }

        #region Build

        private void _Build()
        {
            _InitializeForm();
            _InitializeTitle();
            _InitializeContextMenu();
            _InitializeGrid();
            _InitializeFooter();

            Controls.AddRange(new Control[]
            {
                lblTitle,
                dgv,
                lblCount,
                btnClose
            });
        }

        private void _InitializeForm()
        {
            Text = "Manage Application Types";

            Size = new Size(FormWidth, FormHeight);

            StartPosition = FormStartPosition.CenterScreen;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            BackColor = Color.White;

            Font = new Font(
                "Microsoft Sans Serif",
                9.5F);
        }

        private void _InitializeTitle()
        {
            lblTitle = new Label
            {
                Text = "Application Types",

                Font = new Font(
                    "Arial",
                    18F,
                    FontStyle.Bold),

                ForeColor = clsGlobal.PrimaryRed,

                Dock = DockStyle.Top,
                Height = 55,

                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private void _InitializeContextMenu()
        {
            ctxMenu = new ContextMenuStrip
            {
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F)
            };

            ctxEdit = new ToolStripMenuItem("Edit");

            ctxEdit.Click += (s, e) =>
            {
                _OpenAddEdit(_SelectedID());
            };

            ctxMenu.Items.Add(ctxEdit);
        }

        private void _InitializeGrid()
        {
            dgv = new DataGridView
            {
                Location = new Point(
                    GridMargin,
                    GridTop),

                Size = new Size(
                    ClientSize.Width - (GridMargin * 2),
                    ClientSize.Height -
                    GridTop -
                    BottomAreaHeight),

                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Bottom |
                    AnchorStyles.Left |
                    AnchorStyles.Right,

                ReadOnly = true,

                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,

                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,

                MultiSelect = false,

                AutoGenerateColumns = true,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,

                ColumnHeadersHeight = 34,

                RowTemplate = new DataGridViewRow
                {
                    Height = 30
                },

                BorderStyle = BorderStyle.FixedSingle,

                BackgroundColor = Color.White,

                ContextMenuStrip = ctxMenu,

                Cursor = Cursors.Hand,

                RowHeadersVisible = false,

                EnableHeadersVisualStyles = false
            };

            _StyleGrid(dgv);

            dgv.CellDoubleClick += _GridCellDoubleClick;
            dgv.MouseDown += _GridMouseDown;
            dgv.SelectionChanged += _GridSelectionChanged;
            dgv.KeyDown += _GridKeyDown;
        }

        private void _InitializeFooter()
        {
            lblCount = new Label
            {
                Text = "Records: 0",

                AutoSize = true,

                Location = new Point(
                    GridMargin,
                    ClientSize.Height - 43),

                Anchor =
                    AnchorStyles.Bottom |
                    AnchorStyles.Left,

                ForeColor = Color.Gray
            };

            btnClose = _CreateButton(
                "✖  Close",
                Color.FromArgb(192, 50, 50));

            btnClose.Anchor =
                AnchorStyles.Bottom |
                AnchorStyles.Right;

            btnClose.Location = new Point(
                ClientSize.Width -
                btnClose.Width -
                GridMargin,

                ClientSize.Height -
                btnClose.Height -
                18);

            btnClose.Click += (s, e) => Close();
        }

        #endregion

        #region Load

        private void _Load()
        {
            dgv.DataSource =
                clsApplicationType.GetAllApplicationTypes();

            _ConfigureColumns();

            lblCount.Text =
                "Records: " + dgv.Rows.Count;

            ctxEdit.Enabled =
                dgv.SelectedRows.Count > 0;
        }

        // Applies header text, alignment, width/fill and format for every
        // column in one pass, driven by ColumnLayout.
        private void _ConfigureColumns()
        {
            for (int i = 0; i < ColumnLayout.Length; i++)
            {
                _ApplyColumn(ColumnLayout[i], i);
            }
        }

        private void _ApplyColumn(ColumnSetup setup, int displayIndex)
        {
            if (!dgv.Columns.Contains(setup.DataField))
                return;

            DataGridViewColumn column =
                dgv.Columns[setup.DataField];

            column.HeaderText = setup.Header;
            column.DisplayIndex = displayIndex;

            column.DefaultCellStyle.Alignment =
                setup.Alignment;

            if (setup.Fill)
            {
                column.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                column.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;

                column.Width = setup.Width;
            }

            if (!string.IsNullOrEmpty(setup.Format))
            {
                column.DefaultCellStyle.Format =
                    setup.Format;
            }
        }

        #endregion

        #region Selection / Editing

        private int _SelectedID()
        {
            if (dgv.SelectedRows.Count == 0)
                return -1;

            const string idField = "ApplicationTypeID";

            if (!dgv.Columns.Contains(idField))
                return -1;

            object value =
                dgv.SelectedRows[0]
                   .Cells[idField]
                   .Value;

            if (value == null ||
                value == DBNull.Value)
            {
                return -1;
            }

            int id;

            if (!int.TryParse(
                    value.ToString(),
                    out id))
            {
                return -1;
            }

            return id > 0 ? id : -1;
        }

        private void _OpenAddEdit(int id)
        {
            if (id <= 0)
            {
                MessageBox.Show(
                    "Please select a valid application type.",
                    "Edit Application Type",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            using (frmEditApplicationType frm =
                new frmEditApplicationType(id))
            {
                frm.ShowDialog();
            }

            // Refresh the list after editing.
            _Load();
        }

        #endregion

        #region Grid Events

        private void _GridCellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int id = _SelectedID();

            if (id > 0)
                _OpenAddEdit(id);
        }

        private void _GridSelectionChanged(
            object sender,
            EventArgs e)
        {
            ctxEdit.Enabled =
                _SelectedID() > 0;
        }

        private void _GridMouseDown(
            object sender,
            MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            DataGridView.HitTestInfo hit =
                dgv.HitTest(e.X, e.Y);

            if (hit.RowIndex < 0)
                return;

            dgv.ClearSelection();

            dgv.Rows[hit.RowIndex].Selected = true;

            dgv.CurrentCell =
                dgv.Rows[hit.RowIndex].Cells[0];
        }

        private void _GridKeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            int id = _SelectedID();

            if (id <= 0)
                return;

            e.Handled = true;
            e.SuppressKeyPress = true;

            _OpenAddEdit(id);
        }

        #endregion

        #region Grid Formatting

        private static void _StyleGrid(
            DataGridView g)
        {
            g.ColumnHeadersDefaultCellStyle.BackColor =
                clsGlobal.GridHeaderBack;

            g.ColumnHeadersDefaultCellStyle.ForeColor =
                clsGlobal.GridHeaderFore;

            g.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold);

            g.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            g.EnableHeadersVisualStyles = false;

            g.DefaultCellStyle.SelectionBackColor =
                clsGlobal.GridSelectionBack;

            g.DefaultCellStyle.SelectionForeColor =
                Color.White;

            g.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            g.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(
                    245,
                    248,
                    255);

            g.DefaultCellStyle.Padding =
                new Padding(5, 0, 5, 0);

            // Vertical lines between columns.
            g.CellBorderStyle =
                DataGridViewCellBorderStyle.Single;

            g.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.Single;
        }

        #endregion

        #region Controls

        private static Button _CreateButton(
            string text,
            Color backColor)
        {
            Button b = new Button
            {
                Text = text,

                Size = new Size(
                    150,
                    36),

                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold),

                BackColor = backColor,
                ForeColor = Color.White,

                FlatStyle =
                    FlatStyle.Flat,

                Cursor = Cursors.Hand,

                UseVisualStyleBackColor = false
            };

            b.FlatAppearance.BorderSize = 0;

            return b;
        }

        #endregion
    }
}