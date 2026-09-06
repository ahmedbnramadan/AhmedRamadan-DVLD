using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    /// <summary>
    /// Displays the Local and International licenses of a driver.
    ///
    /// This control contains ONLY:
    ///     - Local licenses tab
    ///     - International licenses tab
    ///
    /// The parent form is responsible for displaying
    /// ctrlPersonCardWithFilter.
    /// </summary>
    public class ctrlDriverLicenses : UserControl
    {
        #region Controls

        private GroupBox gbLicenses;

        private TabControl tcLicenses;
        private TabPage tpLocal;
        private TabPage tpInternational;

        private DataGridView dgvLocal;
        private DataGridView dgvInternational;

        private Label lblLocalRecords;
        private Label lblInternationalRecords;

        private ContextMenuStrip ctxLocalLicense;
        private ToolStripMenuItem ctxShowLicense;

        #endregion

        #region Fields

        private int _DriverID = -1;

        #endregion

        #region Properties

        public int DriverID
        {
            get
            {
                return _DriverID;
            }
        }

        #endregion

        #region Constructors

        public ctrlDriverLicenses()
        {
            _InitializeComponents();
            _InitializeEvents();
        }

        public ctrlDriverLicenses(int DriverID)
        {
            _InitializeComponents();
            _InitializeEvents();

            LoadInfo(DriverID);
        }

        #endregion

        #region Initialization

        private void _InitializeComponents()
        {
            this.Size = new Size(850, 270);
            this.BackColor = Color.White;
            this.Font = new Font(
                "Microsoft Sans Serif",
                9.5F);

            // =====================================================
            // GroupBox
            // =====================================================

            gbLicenses = new GroupBox
            {
                Text = "Driver Licenses",
                Location = new Point(0, 0),
                Size = new Size(
                    this.Width - 5,
                    this.Height - 5),

                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Bottom |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            // =====================================================
            // TabControl
            // =====================================================

            tcLicenses = new TabControl
            {
                Location = new Point(10, 22),

                Size = new Size(
                    gbLicenses.Width - 20,
                    gbLicenses.Height - 55),

                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Bottom |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            // =====================================================
            // Local Tab
            // =====================================================

            tpLocal = new TabPage
            {
                Text = "Local",
                Padding = new Padding(5)
            };

            dgvLocal = _CreateGrid();

            dgvLocal.Dock =
                DockStyle.Fill;

            _CreateLocalContextMenu();

            dgvLocal.ContextMenuStrip =
                ctxLocalLicense;

            tpLocal.Controls.Add(
                dgvLocal);

            // =====================================================
            // International Tab
            // =====================================================

            tpInternational = new TabPage
            {
                Text = "International",
                Padding = new Padding(5)
            };

            dgvInternational = _CreateGrid();

            dgvInternational.Dock =
                DockStyle.Fill;

            tpInternational.Controls.Add(
                dgvInternational);

            // =====================================================
            // Tabs
            // =====================================================

            tcLicenses.TabPages.Add(
                tpLocal);

            tcLicenses.TabPages.Add(
                tpInternational);

            // =====================================================
            // Records Labels
            // =====================================================

            lblLocalRecords = new Label
            {
                Text = "Records: 0",
                AutoSize = true,
                Location = new Point(
                    15,
                    gbLicenses.Height - 28),

                Anchor =
                    AnchorStyles.Bottom |
                    AnchorStyles.Left,

                ForeColor = Color.Gray
            };

            lblInternationalRecords = new Label
            {
                Text = "Records: 0",
                AutoSize = true,
                Location = new Point(
                    15,
                    gbLicenses.Height - 28),

                Anchor =
                    AnchorStyles.Bottom |
                    AnchorStyles.Left,

                ForeColor = Color.Gray,
                Visible = false
            };

            // =====================================================
            // Add Controls
            // =====================================================

            gbLicenses.Controls.Add(
                tcLicenses);

            gbLicenses.Controls.Add(
                lblLocalRecords);

            gbLicenses.Controls.Add(
                lblInternationalRecords);

            this.Controls.Add(
                gbLicenses);
        }

        private DataGridView _CreateGrid()
        {
            DataGridView dgv =
                new DataGridView
                {
                    ReadOnly = true,

                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToResizeRows = false,

                    RowHeadersVisible = false,

                    SelectionMode =
                        DataGridViewSelectionMode.FullRowSelect,

                    MultiSelect = false,

                    AutoGenerateColumns = true,

                    AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill,

                    ColumnHeadersHeight = 32,

                    RowTemplate =
                        new DataGridViewRow
                        {
                            Height = 27
                        },

                    BackgroundColor = Color.White,

                    BorderStyle =
                        BorderStyle.FixedSingle,

                    GridColor =
                        Color.FromArgb(
                            220,
                            225,
                            235),

                    EnableHeadersVisualStyles = false,

                    Cursor = Cursors.Hand
                };

            dgv.ColumnHeadersDefaultCellStyle.BackColor =
                clsGlobal.GridHeaderBack;

            dgv.ColumnHeadersDefaultCellStyle.ForeColor =
                clsGlobal.GridHeaderFore;

            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Microsoft Sans Serif",
                    9.5F,
                    FontStyle.Bold);

            dgv.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgv.DefaultCellStyle.Font =
                new Font(
                    "Microsoft Sans Serif",
                    9F);

            dgv.DefaultCellStyle.SelectionBackColor =
                clsGlobal.GridSelectionBack;

            dgv.DefaultCellStyle.SelectionForeColor =
                Color.White;

            dgv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(
                    245,
                    248,
                    255);

            return dgv;
        }

        private void _InitializeEvents()
        {
            tcLicenses.SelectedIndexChanged +=
                TcLicenses_SelectedIndexChanged;

            dgvLocal.CellMouseDown +=
                DgvLocal_CellMouseDown;

            dgvLocal.CellFormatting +=
                DgvLocal_CellFormatting;

            dgvInternational.CellFormatting +=
                DgvInternational_CellFormatting;
        }

        #endregion

        #region Context Menu

        private void _CreateLocalContextMenu()
        {
            ctxLocalLicense =
                new ContextMenuStrip();

            ctxShowLicense =
                new ToolStripMenuItem(
                    "Show License");

            ctxShowLicense.Click +=
                CtxShowLicense_Click;

            ctxLocalLicense.Items.Add(
                ctxShowLicense);

            ctxLocalLicense.Opening +=
                CtxLocalLicense_Opening;
        }

        private void DgvLocal_CellMouseDown(
            object sender,
            DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            if (e.RowIndex < 0)
                return;

            dgvLocal.ClearSelection();

            dgvLocal.Rows[e.RowIndex].Selected =
                true;

            if (e.ColumnIndex >= 0)
            {
                dgvLocal.CurrentCell =
                    dgvLocal.Rows[e.RowIndex]
                            .Cells[e.ColumnIndex];
            }
        }

        private void CtxLocalLicense_Opening(
            object sender,
            System.ComponentModel.CancelEventArgs e)
        {
            if (_GetSelectedLocalLicenseID() <= 0)
            {
                e.Cancel = true;
            }
        }

        private void CtxShowLicense_Click(
            object sender,
            EventArgs e)
        {
            int LicenseID =
                _GetSelectedLocalLicenseID();

            if (LicenseID <= 0)
            {
                clsUtil.ShowError(
                    "Please select a valid license.");

                return;
            }

            clsLicense License =
                clsLicense.Find(LicenseID);

            if (License == null)
            {
                clsUtil.ShowError(
                    "The selected license was not found.");

                return;
            }

            frmShowLicenseInfo frm =
                new frmShowLicenseInfo(
                    LicenseID);

            frm.ShowDialog();
        }

        private int _GetSelectedLocalLicenseID()
        {
            if (dgvLocal.SelectedRows.Count == 0)
                return -1;

            if (!dgvLocal.Columns.Contains(
                    "licenseid"))
            {
                return -1;
            }

            object Value =
                dgvLocal.SelectedRows[0]
                        .Cells["licenseid"]
                        .Value;

            if (Value == null ||
                Value == DBNull.Value)
            {
                return -1;
            }

            int LicenseID;

            if (!int.TryParse(
                    Value.ToString(),
                    out LicenseID))
            {
                return -1;
            }

            return LicenseID > 0
                ? LicenseID
                : -1;
        }

        #endregion

        #region Load Information

        /// <summary>
        /// Loads the licenses using Driver ID.
        /// </summary>
        public bool LoadInfo(int DriverID)
        {
            if (DriverID <= 0)
            {
                Clear();

                clsUtil.ShowError(
                    "Invalid Driver ID.");

                return false;
            }

            clsDriver Driver =
                clsDriver.Find(DriverID);

            if (Driver == null)
            {
                Clear();

                clsUtil.ShowError(
                    "Driver with ID " +
                    DriverID +
                    " was not found.");

                return false;
            }

            _DriverID =
                Driver.ID;

            _LoadLicenses();

            return true;
        }

        /// <summary>
        /// Loads the licenses using Person ID.
        ///
        /// The method first checks whether the person exists,
        /// then checks whether that person is registered as
        /// a driver. If not, nothing is loaded.
        /// </summary>
        public bool LoadInfoByPersonID(int PersonID)
        {
            if (PersonID <= 0)
            {
                Clear();

                clsUtil.ShowError(
                    "Invalid Person ID.");

                return false;
            }

            clsPerson Person =
                clsPerson.Find(PersonID);

            if (Person == null)
            {
                Clear();

                clsUtil.ShowError(
                    "Person with ID " +
                    PersonID +
                    " was not found.");

                return false;
            }

            clsDriver Driver =
                clsDriver.FindByPersonID(
                    PersonID);

            if (Driver == null)
            {
                Clear();

                clsUtil.ShowWarning(
                    "This person is not registered as a driver.");

                return false;
            }

            // The person is a driver.
            // Use the real Driver ID to load the licenses.
            _DriverID =
                Driver.ID;

            _LoadLicenses();

            return true;
        }

        /// <summary>
        /// Reloads the licenses of the currently loaded driver.
        /// </summary>
        public void RefreshLicenses()
        {
            if (_DriverID <= 0)
                return;

            _LoadLicenses();
        }

        /// <summary>
        /// Clears the current driver and both lists.
        /// </summary>
        public void Clear()
        {
            _DriverID = -1;

            dgvLocal.DataSource = null;
            dgvInternational.DataSource = null;

            lblLocalRecords.Text =
                "Records: 0";

            lblInternationalRecords.Text =
                "Records: 0";
        }

        #endregion

        #region Load Licenses

        private void _LoadLicenses()
        {
            if (_DriverID <= 0)
            {
                Clear();
                return;
            }

            _LoadLocalLicenses();
            _LoadInternationalLicenses();
        }

        private void _LoadLocalLicenses()
        {
            DataTable dt =
                clsLicense.GetDriverLicenses(
                    _DriverID);

            if (dt == null)
                dt = new DataTable();

            dgvLocal.DataSource =
                dt;

            lblLocalRecords.Text =
                "Records: " +
                dt.Rows.Count;

            _ConfigureLocalColumns();
        }

        private void _LoadInternationalLicenses()
        {
            DataTable dt =
                clsInternationalLicense
                    .GetDriverInternationalLicenses(
                        _DriverID);

            if (dt == null)
                dt = new DataTable();

            dgvInternational.DataSource =
                dt;

            lblInternationalRecords.Text =
                "Records: " +
                dt.Rows.Count;

            _ConfigureInternationalColumns();
        }

        #endregion

        #region Local Columns

        private void _ConfigureLocalColumns()
        {
            _HideColumns(dgvLocal);

            _ConfigureColumn(
                dgvLocal,
                "licenseid",
                "Lic ID",
                11,
                DataGridViewContentAlignment.MiddleCenter);

            _ConfigureColumn(
                dgvLocal,
                "applicationid",
                "App ID",
                11,
                DataGridViewContentAlignment.MiddleCenter);

            _ConfigureColumn(
                dgvLocal,
                "classname",
                "Class Name",
                30,
                DataGridViewContentAlignment.MiddleLeft);

            _ConfigureColumn(
                dgvLocal,
                "issuedate",
                "Issue Date",
                17,
                DataGridViewContentAlignment.MiddleCenter,
                "d");

            _ConfigureColumn(
                dgvLocal,
                "expirationdate",
                "Expiration Date",
                18,
                DataGridViewContentAlignment.MiddleCenter,
                "d");

            _ConfigureColumn(
                dgvLocal,
                "isactive",
                "Is Active",
                13,
                DataGridViewContentAlignment.MiddleCenter);
        }

        private void DgvLocal_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.Value == null ||
                e.Value == DBNull.Value)
                return;

            if (dgvLocal.Columns[
                    e.ColumnIndex].Name ==
                "isactive")
            {
                bool IsActive =
                    Convert.ToBoolean(
                        e.Value);

                e.Value =
                    IsActive
                        ? "Yes"
                        : "No";

                e.FormattingApplied =
                    true;
            }
        }

        #endregion

        #region International Columns

        private void _ConfigureInternationalColumns()
        {
            _HideColumns(
                dgvInternational);

            _ConfigureColumn(
                dgvInternational,
                "internationallicenseid",
                "Int. Lic ID",
                18,
                DataGridViewContentAlignment.MiddleCenter);

            _ConfigureColumn(
                dgvInternational,
                "applicationid",
                "App ID",
                13,
                DataGridViewContentAlignment.MiddleCenter);

            _ConfigureColumn(
                dgvInternational,
                "issuedusinglocallicenseid",
                "Local Lic ID",
                16,
                DataGridViewContentAlignment.MiddleCenter);

            _ConfigureColumn(
                dgvInternational,
                "issuedate",
                "Issue Date",
                17,
                DataGridViewContentAlignment.MiddleCenter,
                "d");

            _ConfigureColumn(
                dgvInternational,
                "expirationdate",
                "Expiration Date",
                23,
                DataGridViewContentAlignment.MiddleCenter,
                "d");

            _ConfigureColumn(
                dgvInternational,
                "isactive",
                "Is Active",
                13,
                DataGridViewContentAlignment.MiddleCenter);
        }

        private void DgvInternational_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.Value == null ||
                e.Value == DBNull.Value)
                return;

            if (dgvInternational.Columns[
                    e.ColumnIndex].Name ==
                "isactive")
            {
                bool IsActive =
                    Convert.ToBoolean(
                        e.Value);

                e.Value =
                    IsActive
                        ? "Yes"
                        : "No";

                e.FormattingApplied =
                    true;
            }
        }

        #endregion

        #region Grid Helpers

        private void _HideColumns(
            DataGridView dgv)
        {
            foreach (
                DataGridViewColumn Column
                in dgv.Columns)
            {
                Column.Visible = false;
            }
        }

        private void _ConfigureColumn(
            DataGridView dgv,
            string ColumnName,
            string HeaderText,
            float FillWeight,
            DataGridViewContentAlignment Alignment,
            string Format = null)
        {
            if (!dgv.Columns.Contains(
                    ColumnName))
            {
                return;
            }

            DataGridViewColumn Column =
                dgv.Columns[ColumnName];

            Column.Visible = true;

            Column.HeaderText =
                HeaderText;

            Column.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;

            Column.FillWeight =
                FillWeight;

            Column.DefaultCellStyle.Alignment =
                Alignment;

            if (!string.IsNullOrEmpty(
                    Format))
            {
                Column.DefaultCellStyle.Format =
                    Format;
            }
        }

        #endregion

        #region Tabs

        private void TcLicenses_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            bool IsLocal =
                tcLicenses.SelectedTab ==
                tpLocal;

            lblLocalRecords.Visible =
                IsLocal;

            lblInternationalRecords.Visible =
                !IsLocal;
        }

        #endregion
    }
}