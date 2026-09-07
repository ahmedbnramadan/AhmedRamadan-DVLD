using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListInternationalApplications : Form
    {
        private Label lblTitle;
        private Label lblFilterBy;
        private ComboBox cbFilterBy;
        private TextBox txtFilter;

        private DataGridView dgv;
        private Label lblCount;

        private Button btnAddNew;
        private Button btnClose;

        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxShowPersonDetails;
        private ToolStripMenuItem ctxShowLicenseDetails;
        private ToolStripMenuItem ctxShowPersonLicenseHistory;

        private DataTable _full;

        public frmListInternationalApplications()
        {
            _Build();
            _LoadData();
        }

        #region Form

        private void _Build()
        {
            this.Text = "International Driving License Applications";
            this.Size = new Size(1200, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            // Title
            lblTitle = new Label
            {
                Text = "International License Applications",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(370, 18)
            };

            // Filter label
            lblFilterBy = new Label
            {
                Text = "Filter By:",
                AutoSize = true,
                Location = new Point(30, 65),
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold)
            };

            // Filter combo box
            cbFilterBy = new ComboBox
            {
                Location = new Point(110, 62),
                Size = new Size(180, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor = Cursors.Hand
            };

            cbFilterBy.Items.AddRange(new object[]
            {
                "None",
                "International License ID",
                "Application ID",
                "Driver ID",
                "Local License ID",
                "Is Active"
            });

            cbFilterBy.SelectedIndex = 0;

            cbFilterBy.SelectedIndexChanged +=
                cbFilterBy_SelectedIndexChanged;

            // Filter textbox
            txtFilter = new TextBox
            {
                Location = new Point(300, 62),
                Size = new Size(220, 23),
                Visible = false
            };

            txtFilter.TextChanged +=
                txtFilter_TextChanged;

            // Context menu
            _BuildContextMenu();

            // DataGridView
            dgv = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(1150, 490),

                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,

                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,

                MultiSelect = false,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.None,

                ColumnHeadersHeight = 34,
                RowTemplate = { Height = 28 },

                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220, 225, 235),

                ContextMenuStrip = ctxMenu,
                Cursor = Cursors.Hand
            };

            _StyleGrid(dgv);

            dgv.CellDoubleClick +=
                dgv_CellDoubleClick;

            dgv.MouseDown +=
                dgv_MouseDown;

            dgv.SelectionChanged +=
                dgv_SelectionChanged;

            // Record count
            lblCount = new Label
            {
                Text = "Records: 0",
                AutoSize = true,
                Location = new Point(20, 602),
                ForeColor = Color.Gray
            };

            // Add New button
            btnAddNew = _CreateButton(
                "Add New",
                860,
                590,
                Color.FromArgb(0, 120, 215));

            btnAddNew.Click +=
                btnAddNew_Click;

            // Close button
            btnClose = _CreateButton(
                "✖  Close",
                1025,
                590,
                Color.FromArgb(192, 50, 50));

            btnClose.Click +=
                btnClose_Click;

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblFilterBy,
                cbFilterBy,
                txtFilter,
                dgv,
                lblCount,
                btnAddNew,
                btnClose
            });
        }

        #endregion

        #region Context Menu

        private void _BuildContextMenu()
        {
            ctxMenu = new ContextMenuStrip
            {
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F)
            };

            ctxShowPersonDetails =
                new ToolStripMenuItem(
                    "Show Person Details");

            ctxShowLicenseDetails =
                new ToolStripMenuItem(
                    "Show License Details");

            ctxShowPersonLicenseHistory =
                new ToolStripMenuItem(
                    "Show Person License History");

            ctxShowPersonDetails.Click +=
                ctxShowPersonDetails_Click;

            ctxShowLicenseDetails.Click +=
                ctxShowLicenseDetails_Click;

            ctxShowPersonLicenseHistory.Click +=
                ctxShowPersonLicenseHistory_Click;

            ctxMenu.Items.AddRange(
                new ToolStripItem[]
                {
                    ctxShowPersonDetails,
                    ctxShowLicenseDetails,
                    ctxShowPersonLicenseHistory
                });
        }

        #endregion

        #region Data

        private void _LoadData()
        {
            _full =
                clsInternationalLicense
                .GetAllInternationalLicenses();

            _BindGrid(_full);
        }

        private void _BindGrid(DataTable dt)
        {
            dgv.DataSource = dt;

            lblCount.Text =
                "Records: " + dt.Rows.Count;

            // Visible columns
            _Rename(
                "InternationalLicenseID",
                "License ID");

            _Rename(
                "ApplicationID",
                "Application ID");

            _Rename(
                "DriverID",
                "Driver ID");

            _Rename(
                "IssuedUsingLocalLicenseID",
                "L.License ID");

            _Rename(
                "IssueDate",
                "Issue Date");

            _Rename(
                "ExpirationDate",
                "Expiration Date");

            _Rename(
                "IsActive",
                "Is Active");

            // Set widths
            _SetWidth(
                "InternationalLicenseID",
                145);

            _SetWidth(
                "ApplicationID",
                145);

            _SetWidth(
                "DriverID",
                145);

            _SetWidth(
                "IssuedUsingLocalLicenseID",
                145);

            _SetWidth(
                "IssueDate",
                205);

            _SetWidth(
                "ExpirationDate",
                205);

            _SetWidth(
                "IsActive",
                100);

            // Center visible columns
            _CenterColumn(
                "InternationalLicenseID");

            _CenterColumn(
                "ApplicationID");

            _CenterColumn(
                "DriverID");

            _CenterColumn(
                "IssuedUsingLocalLicenseID");

            _CenterColumn(
                "IssueDate");

            _CenterColumn(
                "ExpirationDate");

            _CenterColumn(
                "IsActive");
        }

        #endregion

        #region Filtering

        private void cbFilterBy_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            bool showFilter =
                cbFilterBy.SelectedIndex > 0;

            txtFilter.Visible = showFilter;
            txtFilter.Clear();

            if (showFilter)
            {
                txtFilter.Focus();
            }
            else
            {
                _BindGrid(_full);
            }
        }

        private void txtFilter_TextChanged(
            object sender,
            EventArgs e)
        {
            _Filter();
        }

        private void _Filter()
        {
            if (_full == null)
                return;

            string value =
                txtFilter.Text.Trim();

            if (cbFilterBy.SelectedIndex == 0 ||
                string.IsNullOrEmpty(value))
            {
                _BindGrid(_full);
                return;
            }

            string columnName = null;

            switch (cbFilterBy.Text)
            {
                case "International License ID":
                    columnName =
                        "InternationalLicenseID";
                    break;

                case "Application ID":
                    columnName =
                        "ApplicationID";
                    break;

                case "Driver ID":
                    columnName =
                        "DriverID";
                    break;

                case "Local License ID":
                    columnName =
                        "IssuedUsingLocalLicenseID";
                    break;

                case "Is Active":
                    columnName =
                        "IsActive";
                    break;
            }

            if (columnName == null)
                return;

            try
            {
                DataView dv =
                    new DataView(_full);

                string safeValue =
                    value.Replace("'", "''");

                dv.RowFilter =
                    "CONVERT([" +
                    columnName +
                    "], System.String) LIKE '%" +
                    safeValue +
                    "%'";

                _BindGrid(dv.ToTable());
            }
            catch
            {
                // Ignore invalid filter input.
            }
        }

        #endregion

        #region Grid Events

        private void dgv_MouseDown(
            object sender,
            MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            DataGridView.HitTestInfo hit =
                dgv.HitTest(e.X, e.Y);

            if (hit.RowIndex >= 0)
            {
                dgv.ClearSelection();
                dgv.Rows[hit.RowIndex].Selected = true;
            }
        }

        private void dgv_SelectionChanged(
            object sender,
            EventArgs e)
        {
            bool hasSelection =
                dgv.SelectedRows.Count > 0;

            ctxShowPersonDetails.Enabled =
                hasSelection;

            ctxShowLicenseDetails.Enabled =
                hasSelection;

            ctxShowPersonLicenseHistory.Enabled =
                hasSelection;
        }

        private void dgv_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            _ShowLicenseDetails();
        }

        #endregion

        #region Context Actions

        private void ctxShowPersonDetails_Click(
            object sender,
            EventArgs e)
        {
            _ShowPersonDetails();
        }

        private void ctxShowLicenseDetails_Click(
            object sender,
            EventArgs e)
        {
            _ShowLicenseDetails();
        }

        private void ctxShowPersonLicenseHistory_Click(
            object sender,
            EventArgs e)
        {
            _ShowPersonLicenseHistory();
        }

        private void _ShowPersonDetails()
        {
            if (dgv.SelectedRows.Count == 0)
                return;

            DataGridViewRow row =
                dgv.SelectedRows[0];

            if (!dgv.Columns.Contains("DriverID"))
                return;

            object value =
                row.Cells["DriverID"].Value;

            if (value == null ||
                value == DBNull.Value)
                return;

            int driverID;

            if (!int.TryParse(
                value.ToString(),
                out driverID))
                return;

            clsDriver driver =
                clsDriver.Find(driverID);

            if (driver == null)
                return;

            new frmShowPersonInfo(
                driver.PersonID).ShowDialog();
        }

        private void _ShowLicenseDetails()
        {
            if (dgv.SelectedRows.Count == 0)
                return;

            DataGridViewRow row =
                dgv.SelectedRows[0];

            if (!dgv.Columns.Contains(
                "InternationalLicenseID"))
                return;

            object value =
                row.Cells[
                    "InternationalLicenseID"].Value;

            if (value == null ||
                value == DBNull.Value)
                return;

            int internationalLicenseID;

            if (!int.TryParse(
                value.ToString(),
                out internationalLicenseID))
                return;

            new frmShowInternationalLicenseInfo(
                internationalLicenseID).ShowDialog();
        }

        private void _ShowPersonLicenseHistory()
        {
            if (dgv.SelectedRows.Count == 0)
                return;

            DataGridViewRow row =
                dgv.SelectedRows[0];

            if (!dgv.Columns.Contains("DriverID"))
                return;

            object value =
                row.Cells["DriverID"].Value;

            if (value == null ||
                value == DBNull.Value)
                return;

            int driverID;

            if (!int.TryParse(
                value.ToString(),
                out driverID))
                return;

            clsDriver driver =
                clsDriver.Find(driverID);

            if (driver == null)
                return;

            new frmShowPersonLicenseHistory(
                driver.PersonID).ShowDialog();
        }

        #endregion

        #region Buttons

        private void btnAddNew_Click(
            object sender,
            EventArgs e)
        {
            new frmAddNewInternationalLicense()
                .ShowDialog();

            _LoadData();
        }

        private void btnClose_Click(
            object sender,
            EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Grid Helpers

        private void _Rename(
            string columnName,
            string headerText)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]
                    .HeaderText = headerText;
            }
        }

        private void _Hide(
            string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]
                    .Visible = false;
            }
        }

        private void _SetWidth(
            string columnName,
            int width)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]
                    .Width = width;
            }
        }

        private void _CenterColumn(
            string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]
                    .DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private static void _StyleGrid(
            DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle
                .BackColor =
                clsGlobal.GridHeaderBack;

            grid.ColumnHeadersDefaultCellStyle
                .ForeColor =
                clsGlobal.GridHeaderFore;

            grid.ColumnHeadersDefaultCellStyle
                .Font =
                new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold);

            grid.EnableHeadersVisualStyles =
                false;

            grid.DefaultCellStyle
                .SelectionBackColor =
                clsGlobal.GridSelectionBack;

            grid.DefaultCellStyle
                .SelectionForeColor =
                Color.White;

            grid.AlternatingRowsDefaultCellStyle
                .BackColor =
                Color.FromArgb(
                    245,
                    248,
                    255);
        }

        private static Button _CreateButton(
            string text,
            int x,
            int y,
            Color backColor)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(150, 34),

                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold),

                BackColor = backColor,
                ForeColor = Color.White,

                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;

            return button;
        }

        #endregion
    }
}