using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListDrivers : Form
    {
        private Label lblTitle;

        private Label lblFilterBy;
        private ComboBox cbFilterBy;
        private TextBox txtFilter;

        private DataGridView dgv;

        private Button btnClose;
        private Label lblCount;

        private ContextMenuStrip ctxMenu;

        private ToolStripMenuItem ctxShowDetails;
        private ToolStripMenuItem ctxShowPersonLicenseHistory;

        private DataTable _full;

        public frmListDrivers()
        {
            _Build();
            _LoadData();
        }

        private void _Build()
        {
            // =========================================================
            // Form
            // =========================================================

            this.Text = "Manage Drivers";
            this.Size = new Size(1200, 680);

            this.StartPosition =
                FormStartPosition.CenterScreen;

            this.FormBorderStyle =
                FormBorderStyle.FixedDialog;

            this.MaximizeBox = false;

            this.BackColor =
                Color.White;

            this.Font =
                new Font(
                    "Microsoft Sans Serif",
                    9.5F);

            // =========================================================
            // Title
            // =========================================================

            lblTitle = new Label
            {
                Text = "Manage Drivers",

                Font =
                    new Font(
                        "Arial",
                        18F,
                        FontStyle.Bold),

                ForeColor =
                    clsGlobal.PrimaryRed,

                AutoSize = true,

                Location =
                    new Point(480, 18)
            };

            // =========================================================
            // Filter
            // =========================================================

            lblFilterBy = new Label
            {
                Text = "Filter By:",

                AutoSize = true,

                Location =
                    new Point(30, 65),

                Font =
                    new Font(
                        "Microsoft Sans Serif",
                        9.5F,
                        FontStyle.Bold)
            };

            cbFilterBy = new ComboBox
            {
                Location =
                    new Point(110, 62),

                Size =
                    new Size(160, 23),

                DropDownStyle =
                    ComboBoxStyle.DropDownList,

                Cursor =
                    Cursors.Hand
            };

            cbFilterBy.Items.AddRange(
                new object[]
                {
                    "None",
                    "Driver ID",
                    "Person ID",
                    "National No."
                });

            cbFilterBy.SelectedIndex = 0;

            cbFilterBy.SelectedIndexChanged +=
                CbFilterBy_SelectedIndexChanged;

            txtFilter = new TextBox
            {
                Location =
                    new Point(280, 62),

                Size =
                    new Size(220, 23),

                Visible = false
            };

            txtFilter.TextChanged +=
                TxtFilter_TextChanged;

            // =========================================================
            // Context Menu
            // =========================================================

            ctxMenu =
                new ContextMenuStrip
                {
                    Font =
                        new Font(
                            "Microsoft Sans Serif",
                            9.5F)
                };

            ctxShowDetails =
                new ToolStripMenuItem(
                    "Show Details");

            ctxShowPersonLicenseHistory =
                new ToolStripMenuItem(
                    "Show Person License History");

            ctxShowDetails.Click +=
                CtxShowDetails_Click;

            ctxShowPersonLicenseHistory.Click +=
                ctxShowPersonLicenseHistory_Click;

            ctxMenu.Items.AddRange(
                new ToolStripItem[]
                {
                    ctxShowDetails,
                    new ToolStripSeparator(),
                    ctxShowPersonLicenseHistory,
                });

            // =========================================================
            // Drivers List
            // Same overall dimensions/style as the other list forms.
            // =========================================================

            dgv = new DataGridView
            {
                Location =
                    new Point(20, 100),

                Size =
                    new Size(1150, 490),

                ReadOnly = true,

                AllowUserToAddRows = false,

                AllowUserToDeleteRows = false,

                AllowUserToResizeRows = false,

                RowHeadersVisible = false,

                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,

                MultiSelect = false,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,

                ColumnHeadersHeight = 34,

                RowTemplate =
                {
                    Height = 28
                },

                BorderStyle =
                    BorderStyle.None,

                BackgroundColor =
                    Color.White,

                GridColor =
                    Color.FromArgb(
                        220,
                        225,
                        235),

                ContextMenuStrip =
                    ctxMenu,

                Cursor =
                    Cursors.Hand
            };

            _StyleGrid(dgv);

            dgv.CellDoubleClick +=
                Dgv_CellDoubleClick;

            dgv.MouseDown +=
                Dgv_MouseDown;

            dgv.SelectionChanged +=
                Dgv_SelectionChanged;

            // =========================================================
            // Record Count
            // =========================================================

            lblCount = new Label
            {
                Text = "Records: 0",

                AutoSize = true,

                Location =
                    new Point(20, 602),

                ForeColor =
                    Color.Gray
            };

            // =========================================================
            // Close
            // =========================================================

            btnClose = new Button
            {
                Text = "✖  Close",

                Location =
                    new Point(1075, 600),

                Size =
                    new Size(95, 34),

                Font =
                    new Font(
                        "Microsoft Sans Serif",
                        9.5F,
                        FontStyle.Bold),

                BackColor =
                    Color.FromArgb(
                        192,
                        50,
                        50),

                ForeColor =
                    Color.White,

                FlatStyle =
                    FlatStyle.Flat,

                Cursor =
                    Cursors.Hand
            };

            btnClose.FlatAppearance.BorderSize = 0;

            btnClose.Click +=
                BtnClose_Click;

            // =========================================================
            // Add Controls
            // =========================================================

            this.Controls.AddRange(
                new Control[]
                {
                    lblTitle,
                    lblFilterBy,
                    cbFilterBy,
                    txtFilter,
                    dgv,
                    lblCount,
                    btnClose
                });
        }

        private void _LoadData()
        {
            _full =
                clsDriver.GetAllDrivers();

            _BindGrid(_full);
        }

        private void _BindGrid(DataTable dt)
        {
            dgv.DataSource = dt;

            lblCount.Text =
                "Records: " +
                dt.Rows.Count;

            _Rename(
                "DriverID",
                "Driver ID");

            _Rename(
                "PersonID",
                "Person ID");

            _Rename(
                "NationalNo",
                "National No.");

            _Rename(
                "Date",
                "Date");

            _Rename(
                "ActiveLicense",
                "Active License");

            _Hide(
                "ImagePath");

            _Hide(
                "Address");

            _SetColumnWidths();
        }

        // =============================================================
        // Column sizing
        //
        // IMPORTANT:
        // The grid uses Fill, like the other list forms.
        // FillWeight controls the proportion of the available width.
        // =============================================================

        private void _SetColumnWidths()
        {
            _SetFillWeight(
                "DriverID",
                10);

            _SetFillWeight(
                "PersonID",
                10);

            _SetFillWeight(
                "NationalNo",
                40);

            _SetFillWeight(
                "Date",
                30);

            _SetFillWeight(
                "ActiveLicense",
                10);

            // All five values above total 100.
            // This makes the entire 1150px grid width get used
            // without leaving empty space.

            _SetAlignment(
                "DriverID",
                DataGridViewContentAlignment.MiddleCenter);

            _SetAlignment(
                "PersonID",
                DataGridViewContentAlignment.MiddleCenter);

            _SetAlignment(
                "NationalNo",
                DataGridViewContentAlignment.MiddleCenter);

            _SetAlignment(
                "Date",
                DataGridViewContentAlignment.MiddleCenter);

            _SetAlignment(
                "ActiveLicense",
                DataGridViewContentAlignment.MiddleCenter);
        }

        private void _SetFillWeight(
            string columnName,
            float weight)
        {
            if (!dgv.Columns.Contains(columnName))
                return;

            dgv.Columns[columnName]
                .AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;

            dgv.Columns[columnName]
                .FillWeight =
                weight;
        }

        private void _SetAlignment(
            string columnName,
            DataGridViewContentAlignment alignment)
        {
            if (!dgv.Columns.Contains(columnName))
                return;

            dgv.Columns[columnName]
                .DefaultCellStyle.Alignment =
                alignment;
        }

        // =============================================================
        // Filter
        // =============================================================

        private void CbFilterBy_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            txtFilter.Visible =
                cbFilterBy.SelectedIndex > 0;

            txtFilter.Clear();

            if (!txtFilter.Visible)
                _BindGrid(_full);
        }

        private void TxtFilter_TextChanged(
            object sender,
            EventArgs e)
        {
            _Filter();
        }

        private void _Filter()
        {
            if (_full == null)
                return;

            string columnName = null;

            switch (cbFilterBy.Text)
            {
                case "Driver ID":
                    columnName = "DriverID";
                    break;

                case "Person ID":
                    columnName = "PersonID";
                    break;

                case "National No.":
                    columnName = "NationalNo";
                    break;
            }

            string value =
                txtFilter.Text.Trim();

            if (columnName == null ||
                string.IsNullOrEmpty(value))
            {
                _BindGrid(_full);
                return;
            }

            if (!_full.Columns.Contains(columnName))
                return;

            try
            {
                DataView dv =
                    new DataView(_full);

                string escapedValue =
                    value.Replace(
                        "'",
                        "''");

                dv.RowFilter =
                    string.Format(
                        "CONVERT([{0}], System.String) LIKE '%{1}%'",
                        columnName,
                        escapedValue);

                _BindGrid(
                    dv.ToTable());
            }
            catch
            {
                // Ignore invalid filter expressions.
            }
        }

        // =============================================================
        // Context Menu
        // =============================================================

        private void CtxShowDetails_Click(
            object sender,
            EventArgs e)
        {
            _ShowDetails();
        }

        private void ctxShowPersonLicenseHistory_Click(
            object sender,
            EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
                return;

            if (!dgv.Columns.Contains("DriverID"))
                return;

            object value =
                dgv.SelectedRows[0]
                    .Cells["DriverID"]
                    .Value;

            if (value == null ||
                value == DBNull.Value)
                return;

            int driverID;

            if (!int.TryParse(
                    value.ToString(),
                    out driverID))
            {
                return;
            }

            if (driverID > 0)
            {
                new frmShowPersonLicenseHistory(driverID)
                    .ShowDialog();
            }
        }


        private void _ShowDetails()
        {
            if (dgv.SelectedRows.Count == 0)
                return;

            if (!dgv.Columns.Contains("PersonID"))
                return;

            object value =
                dgv.SelectedRows[0]
                    .Cells["PersonID"]
                    .Value;

            if (value == null ||
                value == DBNull.Value)
                return;

            int personID;

            if (!int.TryParse(
                    value.ToString(),
                    out personID))
            {
                return;
            }

            if (personID > 0)
            {
                new frmShowPersonInfo(personID)
                    .ShowDialog();
            }
        }

        // =============================================================
        // Grid Events
        // =============================================================

        private void Dgv_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                _ShowDetails();
        }

        private void Dgv_MouseDown(
            object sender,
            MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            DataGridView.HitTestInfo hit =
                dgv.HitTest(
                    e.X,
                    e.Y);

            if (hit.RowIndex >= 0)
            {
                dgv.ClearSelection();

                dgv.Rows[
                    hit.RowIndex
                ].Selected = true;
            }
        }

        private void Dgv_SelectionChanged(
            object sender,
            EventArgs e)
        {
            bool enabled =
                dgv.SelectedRows.Count > 0;

            ctxShowDetails.Enabled =
                enabled;

            ctxShowPersonLicenseHistory.Enabled =
                enabled;

        }

        // =============================================================
        // Helpers
        // =============================================================

        private void _Rename(
            string columnName,
            string header)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]
                    .HeaderText =
                    header;
            }
        }

        private void _Hide(
            string columnName)
        {
            if (dgv.Columns.Contains(columnName))
                dgv.Columns[columnName].Visible = false;
        }

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

            g.EnableHeadersVisualStyles =
                false;

            g.DefaultCellStyle.Font =
                new Font(
                    "Microsoft Sans Serif",
                    9F);

            g.DefaultCellStyle.SelectionBackColor =
                clsGlobal.GridSelectionBack;

            g.DefaultCellStyle.SelectionForeColor =
                Color.White;

            g.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(
                    245,
                    248,
                    255);
        }

        private void BtnClose_Click(
            object sender,
            EventArgs e)
        {
            this.Close();
        }
    }

}