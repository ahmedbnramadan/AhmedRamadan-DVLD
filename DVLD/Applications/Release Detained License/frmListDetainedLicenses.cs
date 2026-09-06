using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListDetainedLicenses : Form
    {
        #region Controls

        private Label lblTitle;

        private Label lblFilterBy;
        private ComboBox cbFilterBy;
        private TextBox txtFilter;

        private DataGridView dgv;
        private Button btnClose;
        private Label lblCount;

        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxShowPersonDetails;
        private ToolStripMenuItem ctxShowLicenseDetails;
        private ToolStripMenuItem ctxShowPersonLicenseHistory;
        private ToolStripMenuItem ctxRelease;

        #endregion

        #region Constants

        private const int FormWidth = 1200;
        private const int FormHeight = 680;

        private const int GridMargin = 20;
        private const int GridTop = 100;
        private const int BottomAreaHeight = 60;

        #endregion

        #region Column Setup

        // Same declarative pattern as frmListTestTypes / frmListApplicationTypes:
        // one array describes every column, one loop applies it. Adding or
        // reordering a column means editing this table, not writing a new method.
        private struct ColumnSetup
        {
            public string DataField;
            public string Header;
            public DataGridViewContentAlignment Alignment;
            public int Width;
            public string Format;

            public ColumnSetup(string dataField, string header,
                DataGridViewContentAlignment alignment, int width, string format = null)
            {
                DataField = dataField;
                Header = header;
                Alignment = alignment;
                Width = width;
                Format = format;
            }
        }

        // Data field names must match columns returned by
        // clsDetainedLicense.GetAllDetainedLicenses() (see the corrected
        // DataAccess query - dl.* + the joined extras).
        private static readonly ColumnSetup[] ColumnLayout =
        {
            new ColumnSetup("DetainID",             "D ID",            DataGridViewContentAlignment.MiddleCenter, 60),
            new ColumnSetup("LicenseID",            "L ID",            DataGridViewContentAlignment.MiddleCenter, 60),
            new ColumnSetup("DetainDate",           "D Date",          DataGridViewContentAlignment.MiddleCenter, 140, "g"),
            new ColumnSetup("IsReleased",           "Is Released",     DataGridViewContentAlignment.MiddleCenter, 90),
            new ColumnSetup("FineFees",             "Fine Fees",       DataGridViewContentAlignment.MiddleRight,  100, "N2"),
            new ColumnSetup("ReleaseDate",          "Release Date",    DataGridViewContentAlignment.MiddleCenter, 140, "g"),
            new ColumnSetup("NationalNo",           "N.No.",           DataGridViewContentAlignment.MiddleCenter, 100),
            new ColumnSetup("DriverName",           "Full Name",       DataGridViewContentAlignment.MiddleLeft,   220),
            new ColumnSetup("ReleaseApplicationID", "Release App.ID",  DataGridViewContentAlignment.MiddleCenter, 110)
        };

        #endregion

        #region State

        private DataTable _full;

        #endregion

        public frmListDetainedLicenses()
        {
            _Build();
            _LoadData();
        }

        #region Build

        private void _Build()
        {
            _InitializeForm();
            _InitializeTitle();
            _InitializeFilter();
            _InitializeContextMenu();
            _InitializeGrid();
            _InitializeFooter();

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblFilterBy, cbFilterBy, txtFilter,
                dgv,
                lblCount,
                btnClose
            });
        }

        private void _InitializeForm()
        {
            Text = "List Detained Licenses";
            Size = new Size(FormWidth, FormHeight);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = new Font("Microsoft Sans Serif", 9.5F);
        }

        private void _InitializeTitle()
        {
            lblTitle = new Label
            {
                Text = "List Detained Licenses",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                Dock = DockStyle.Top,
                Height = 55,
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private void _InitializeFilter()
        {
            lblFilterBy = new Label
            {
                Text = "Filter By:",
                AutoSize = true,
                Location = new Point(GridMargin, 65),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold)
            };

            cbFilterBy = new ComboBox
            {
                Location = new Point(110, 62),
                Size = new Size(170, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor = Cursors.Hand
            };
            cbFilterBy.Items.AddRange(new object[]
            {
                "None", "Detain ID", "License ID", "National No.", "Full Name", "Status"
            });
            cbFilterBy.SelectedIndex = 0;
            cbFilterBy.SelectedIndexChanged += CbFilterBy_SelectedIndexChanged;

            txtFilter = new TextBox
            {
                Location = new Point(290, 62),
                Size = new Size(220, 23),
                Visible = false
            };
            txtFilter.TextChanged += (s, e) => _Filter();
        }

        private void _InitializeContextMenu()
        {
            ctxMenu = new ContextMenuStrip
            {
                Font = new Font("Microsoft Sans Serif", 9.5F)
            };

            ctxShowPersonDetails        = new ToolStripMenuItem("Show Person Details");
            ctxShowLicenseDetails       = new ToolStripMenuItem("Show License Details");
            ctxShowPersonLicenseHistory = new ToolStripMenuItem("Show Person License History");
            ctxRelease                  = new ToolStripMenuItem("Release Detained License");

            ctxShowPersonDetails.Click        += (s, e) => _ShowPersonDetails();
            ctxShowLicenseDetails.Click       += (s, e) => _ShowLicenseDetails();
            ctxShowPersonLicenseHistory.Click += (s, e) => _ShowPersonLicenseHistory();
            ctxRelease.Click                  += (s, e) => _ReleaseSelected();

            // Recompute right as the menu is about to open, not only on
            // SelectionChanged - the same defensive pattern used for
            // ctxScheduleTest in frmListLocalDrivingLicenseApplications.
            ctxMenu.Opening += (s, e) => _UpdateContextMenuState();

            ctxMenu.Items.AddRange(new ToolStripItem[]
            {
                ctxShowPersonDetails,
                ctxShowLicenseDetails,
                ctxShowPersonLicenseHistory,
                new ToolStripSeparator(),
                ctxRelease
            });
        }

        private void _InitializeGrid()
        {
            dgv = new DataGridView
            {
                Location = new Point(GridMargin, GridTop),
                Size = new Size(FormWidth - (GridMargin * 2) - 15, FormHeight - GridTop - BottomAreaHeight - 40),

                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,

                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,

                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,

                ColumnHeadersHeight = 34,
                RowTemplate = new DataGridViewRow { Height = 28 },

                BorderStyle = BorderStyle.FixedSingle,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220, 225, 235),

                ContextMenuStrip = ctxMenu,
                Cursor = Cursors.Hand,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false
            };

            _StyleGrid(dgv);

            dgv.CellDoubleClick  += (s, e) => { if (e.RowIndex >= 0) _ShowLicenseDetails(); };
            dgv.MouseDown        += Dgv_MouseDown;
            dgv.SelectionChanged += (s, e) => _UpdateContextMenuState();
            dgv.CellFormatting   += Dgv_CellFormatting;
        }

        private void _InitializeFooter()
        {
            lblCount = new Label
            {
                Text = "# Records: 0",
                AutoSize = true,
                Location = new Point(GridMargin, FormHeight - 100),
                ForeColor = Color.Gray
            };

            btnClose = new Button
            {
                Text = "✖  Close",
                Size = new Size(150, 36),
                Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(192, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Location = new Point(FormWidth - 170 - GridMargin, FormHeight - 100)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Close();
        }

        #endregion

        #region Load / Bind

        private void _LoadData()
        {
            // NOTE: this relies on the corrected clsDetainedLicenses.GetAllDetainedLicenses()
            // (the trailing-comma SQL bug must be fixed in the DataAccess layer
            // first, or this call throws a SqlException before it ever reaches here).
            _full = clsDetainedLicense.GetAllDetainedLicenses();
            _BindGrid(_full);
        }

        private void _BindGrid(DataTable dt)
        {
            dgv.DataSource = dt;
            lblCount.Text = "# Records: " + dt.Rows.Count;
            _ConfigureColumns();
        }

        private void _ConfigureColumns()
        {
            for (int i = 0; i < ColumnLayout.Length; i++)
                _ApplyColumn(ColumnLayout[i], i);

            // Hide anything the query returns that we didn't explicitly lay out
            // (licenseclass, issuereason, classname, driverid, createdbyuserid,
            // releasedbyuserid, detainedbyusername, releasedbyusername).
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                bool isLaidOut = Array.Exists(ColumnLayout, c => c.DataField == col.Name);
                if (!isLaidOut)
                    col.Visible = false;
            }
        }

        private void _ApplyColumn(ColumnSetup setup, int displayIndex)
        {
            if (!dgv.Columns.Contains(setup.DataField))
                return;

            DataGridViewColumn column = dgv.Columns[setup.DataField];
            column.HeaderText = setup.Header;
            column.DisplayIndex = displayIndex;
            column.Width = setup.Width;
            column.DefaultCellStyle.Alignment = setup.Alignment;

            if (!string.IsNullOrEmpty(setup.Format))
                column.DefaultCellStyle.Format = setup.Format;
        }

        // Renders IsReleased as a checkbox column instead of "True"/"False" text,
        // matching the screenshot. DataGridView auto-generates a text column for
        // bool by default unless we swap it - simplest fix is via formatting +
        // ValueType, but the cleanest is just replacing the column type here.
        private bool _checkboxColumnApplied;
        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!_checkboxColumnApplied && dgv.Columns.Contains("IsReleased"))
            {
                if (!(dgv.Columns["IsReleased"] is DataGridViewCheckBoxColumn))
                {
                    int index = dgv.Columns["IsReleased"].Index;
                    var checkCol = new DataGridViewCheckBoxColumn
                    {
                        Name = "IsReleased",
                        DataPropertyName = "IsReleased",
                        HeaderText = "Is Released",
                        Width = 90,
                        ReadOnly = true
                    };
                    dgv.Columns.RemoveAt(index);
                    dgv.Columns.Insert(index, checkCol);
                    checkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                _checkboxColumnApplied = true;
            }
        }

        #endregion

        #region Filter

        private void CbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasFilter = cbFilterBy.SelectedIndex > 0;
            txtFilter.Visible = hasFilter;
            txtFilter.Clear();

            if (!hasFilter)
                _BindGrid(_full);
            else
                txtFilter.Focus();
        }

        private void _Filter()
        {
            if (_full == null) return;

            string col = cbFilterBy.Text;
            string value = txtFilter.Text.Trim();

            if (col == "None" || string.IsNullOrEmpty(value))
            {
                _BindGrid(_full);
                return;
            }

            string dbCol = col switch
            {
                "Detain ID"    => "DetainID",
                "License ID"   => "LicenseID",
                "National No." => "NationalNo",
                "Full Name"    => "DriverName",
                "Status"       => "IsReleased",
                _              => null
            };

            if (dbCol == null) return;

            try
            {
                DataView dv = new DataView(_full);

                if (dbCol == "IsReleased")
                {
                    // "Released" / "Active" as user-friendly status words,
                    // mapped onto the underlying bit column.
                    if (value.Equals("released", StringComparison.OrdinalIgnoreCase))
                        dv.RowFilter = "IsReleased = true";
                    else if (value.Equals("active", StringComparison.OrdinalIgnoreCase))
                        dv.RowFilter = "IsReleased = false";
                    else
                        dv.RowFilter = "1 = 0"; // unrecognized status text -> no matches
                }
                else
                {
                    dv.RowFilter = $"CONVERT([{dbCol}], System.String) LIKE '%{value.Replace("'", "''")}%'";
                }

                _BindGrid(dv.ToTable());
            }
            catch { /* ignore invalid filter expressions while typing */ }
        }

        #endregion

        #region Selection

        private int _SelectedDetainID() => _SelectedInt("DetainID");
        private int _SelectedLicenseID() => _SelectedInt("LicenseID");

        private int _SelectedInt(string columnName)
        {
            if (dgv.SelectedRows.Count == 0) return -1;
            if (!dgv.Columns.Contains(columnName)) return -1;

            object value = dgv.SelectedRows[0].Cells[columnName].Value;
            if (value == null || value == DBNull.Value) return -1;

            return int.TryParse(value.ToString(), out int id) ? id : -1;
        }

        private bool _SelectedIsReleased()
        {
            if (dgv.SelectedRows.Count == 0) return true; // fail safe: treat as released -> disable action
            object value = dgv.SelectedRows[0].Cells["IsReleased"]?.Value;
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        // Resolves the person behind the selected row's license, since the
        // grid itself only carries NationalNo / DriverName / DriverID for
        // display, not PersonID.
        private int _SelectedPersonID()
        {
            int licenseID = _SelectedLicenseID();
            if (licenseID <= 0) return -1;

            clsLicense license = clsLicense.Find(licenseID);
            return license?.DriverInfo?.PersonID ?? -1;
        }

        #endregion

        #region Context Menu State

        // Single source of truth for which actions are currently valid -
        // recomputed from the grid's live selection every time, never
        // toggled by hand from multiple event handlers.
        private void _UpdateContextMenuState()
        {
            bool hasSelection = dgv.SelectedRows.Count > 0;

            ctxShowPersonDetails.Enabled        = hasSelection;
            ctxShowLicenseDetails.Enabled       = hasSelection;
            ctxShowPersonLicenseHistory.Enabled = hasSelection;

            // Can only release a row that is currently NOT released.
            ctxRelease.Enabled = hasSelection && !_SelectedIsReleased();
        }

        #endregion

        #region Actions

        private void _ShowPersonDetails()
        {
            int personID = _SelectedPersonID();
            if (personID <= 0)
            {
                clsUtil.ShowWarning("Could not resolve the person for this license.");
                return;
            }
            new frmShowPersonInfo(personID).ShowDialog();
        }

        private void _ShowLicenseDetails()
        {
            int licenseID = _SelectedLicenseID();
            if (licenseID <= 0) return;
            new frmShowLicenseInfo(licenseID).ShowDialog();
        }

        private void _ShowPersonLicenseHistory()
        {
            int personID = _SelectedPersonID();
            if (personID <= 0)
            {
                clsUtil.ShowWarning("Could not resolve the person for this license.");
                return;
            }
            new frmShowPersonLicenseHistory(personID).ShowDialog();
        }

        private void _ReleaseSelected()
        {
            int licenseID = _SelectedLicenseID();

            if (licenseID <= 0)
            {
                clsUtil.ShowWarning("Please select a valid detained license.");
                return;
            }

            // Defensive re-check: the menu item's Enabled state already
            // reflects this, but never trust that alone at the moment of
            // action - the underlying data could have changed since the
            // grid was last bound (e.g. released from another list/session).
            if (clsDetainedLicense.FindByLicenseID(licenseID)?.IsReleased != false)
            {
                clsUtil.ShowWarning("This license is not currently detained (or was already released).");
                _LoadData();
                return;
            }

            // Locked mode: the release form already knows exactly which
            // license to work with, so the filter inside it is disabled -
            // the user can't accidentally release a different license.
            using (var frm = new frmReleaseDetainedLicense(licenseID))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    _LoadData();
                }
            }
        }

        #endregion

        #region Grid Events

        private void Dgv_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var hit = dgv.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0) return;

            dgv.ClearSelection();
            dgv.Rows[hit.RowIndex].Selected = true;
        }

        #endregion

        #region Grid Formatting

        private static void _StyleGrid(DataGridView g)
        {
            g.ColumnHeadersDefaultCellStyle.BackColor = clsGlobal.GridHeaderBack;
            g.ColumnHeadersDefaultCellStyle.ForeColor = clsGlobal.GridHeaderFore;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.EnableHeadersVisualStyles = false;

            g.DefaultCellStyle.SelectionBackColor = clsGlobal.GridSelectionBack;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
            g.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);

            g.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        }

        #endregion
    }
}