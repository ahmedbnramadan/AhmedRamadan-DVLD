using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;
using DVLD.Tests;

namespace DVLD
{
    /// make the availavle optioins only theat is availvle, others read-only/////////////
    public class frmListLocalDrivingLicenseApplications : Form
    {
        #region Controls

        private Label lblTitle;
        private Label lblFilterBy;
        private ComboBox cbFilterBy;
        private TextBox txtFilter;

        private DataGridView dgv;
        private Button btnAddNew;
        private Button btnClose;
        private Label lblCount;

        private ContextMenuStrip ctxMenu;

        private ToolStripMenuItem ctxShowDetails;
        private ToolStripMenuItem ctxEdit;
        private ToolStripMenuItem ctxDelete;
        private ToolStripMenuItem ctxCancel;

        private ToolStripMenuItem ctxScheduleTest;
        private ToolStripMenuItem ctxVisionTest;
        private ToolStripMenuItem ctxWrittenTest;
        private ToolStripMenuItem ctxPracticalTest;

        private ToolStripMenuItem ctxIssueDrivingLicense;
        private ToolStripMenuItem ctxShowLicense;
        private ToolStripMenuItem ctxShowPersonLicenseHistory;

        #endregion

        #region Data

        private DataTable _full;

        #endregion

        #region Constructor

        public frmListLocalDrivingLicenseApplications()
        {
            _Build();
            _LoadData();
        }

        #endregion

        #region Build UI

        private void _Build()
        {
            this.Text = "Local Driving License Applications";
            this.Size = new Size(1200, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            // ---------------------------------------------------------
            // Title
            // ---------------------------------------------------------

            lblTitle = new Label
            {
                Text = "Local Driving License Applications",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(380, 18)
            };

            // ---------------------------------------------------------
            // Filter
            // ---------------------------------------------------------

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

            cbFilterBy = new ComboBox
            {
                Location = new Point(110, 62),
                Size = new Size(160, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor = Cursors.Hand
            };

            cbFilterBy.Items.AddRange(new object[]
            {
                "None",
                "Application ID",
                "Person ID",
                "National No.",
                "Full Name",
                "License Class",
                "Status"
            });

            cbFilterBy.SelectedIndex = 0;

            cbFilterBy.SelectedIndexChanged +=
                CbFilterBy_SelectedIndexChanged;

            txtFilter = new TextBox
            {
                Location = new Point(280, 62),
                Size = new Size(220, 23),
                Visible = false
            };

            txtFilter.TextChanged +=
                TxtFilter_TextChanged;

            // ---------------------------------------------------------
            // Context Menu
            // ---------------------------------------------------------

            _BuildContextMenu();

            // ---------------------------------------------------------
            // Grid
            // ---------------------------------------------------------

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

                AutoGenerateColumns = true,
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
                Dgv_CellDoubleClick;

            dgv.MouseDown +=
                Dgv_MouseDown;

            dgv.SelectionChanged +=
                Dgv_SelectionChanged;

            // ---------------------------------------------------------
            // Count
            // ---------------------------------------------------------

            lblCount = new Label
            {
                Text = "Records: 0",
                AutoSize = true,
                Location = new Point(20, 602),
                ForeColor = Color.Gray
            };

            // ---------------------------------------------------------
            // Buttons
            // ---------------------------------------------------------

            btnAddNew =
                _Btn( "Add New", 860, 590, Color.FromArgb(0, 120, 215));

            btnAddNew.Click +=
                BtnAddNew_Click;

            btnClose =
                _Btn( "✖  Close", 1025, 590, Color.FromArgb(192, 50, 50));

            btnClose.Click +=
                BtnClose_Click;

            // ---------------------------------------------------------
            // Add controls
            // ---------------------------------------------------------

            this.Controls.AddRange(
                new Control[]
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

        private void _BuildContextMenu()
        {
            ctxMenu = new ContextMenuStrip
            {
                Font = new Font(
                    "Microsoft Sans Serif",
                    9.5F)
            };

            // ---------------------------------------------------------
            // Application actions
            // ---------------------------------------------------------

            ctxShowDetails =
                new ToolStripMenuItem(
                    "Show Application Details");

            ctxEdit =
                new ToolStripMenuItem(
                    "Edit Application");

            ctxDelete =
                new ToolStripMenuItem(
                    "Delete Application");

            ctxCancel =
                new ToolStripMenuItem(
                    "Cancel Application");

            // ---------------------------------------------------------
            // Schedule Test
            // ---------------------------------------------------------

            ctxScheduleTest =
                new ToolStripMenuItem(
                    "Schedule Test");

            ctxVisionTest =
                new ToolStripMenuItem(
                    "Vision Test");

            ctxWrittenTest =
                new ToolStripMenuItem(
                    "Written Test");

            ctxPracticalTest =
                new ToolStripMenuItem(
                    "Practical Test");

            ctxScheduleTest.DropDownItems.Add(
                ctxVisionTest);

            ctxScheduleTest.DropDownItems.Add(
                ctxWrittenTest);

            ctxScheduleTest.DropDownItems.Add(
                ctxPracticalTest);

            // Recompute which test types are actually schedulable every
            // time this submenu is about to open, rather than trying to
            // keep it in sync proactively from elsewhere (e.g. selection
            // change), since passing a test can happen in another dialog
            // while this row stays selected.
            ctxScheduleTest.DropDownOpening += CtxScheduleTest_DropDownOpening;

            // ---------------------------------------------------------
            // License
            // ---------------------------------------------------------

            ctxIssueDrivingLicense =
                new ToolStripMenuItem(
                    "Issue Driving License (First Time)");

            ctxShowLicense =
                new ToolStripMenuItem(
                    "Show License");

            ctxShowPersonLicenseHistory =
                new ToolStripMenuItem(
                    "Show Person License History");

            // ---------------------------------------------------------
            // Events
            // ---------------------------------------------------------

            ctxShowDetails.Click            += CtxShowDetails_Click;

            ctxEdit.Click                   += CtxEdit_Click;

            ctxDelete.Click                 += CtxDelete_Click;

            ctxCancel.Click                 += CtxCancel_Click;

            ctxVisionTest.Click             += CtxVisionTest_Click;

            ctxWrittenTest.Click            += CtxWrittenTest_Click;

            ctxPracticalTest.Click          += CtxPracticalTest_Click;

            ctxIssueDrivingLicense.Click    += CtxIssueDrivingLicense_Click;

            ctxShowLicense.Click            += CtxShowLicense_Click;

            ctxShowPersonLicenseHistory.Click += CtxShowPersonLicenseHistory_Click;

            // ---------------------------------------------------------
            // Menu items
            // ---------------------------------------------------------

            ctxMenu.Items.Add(ctxShowDetails);

            ctxMenu.Items.Add(new ToolStripSeparator());

            ctxMenu.Items.Add(ctxEdit);
            ctxMenu.Items.Add(ctxDelete);
            ctxMenu.Items.Add(ctxCancel);

            ctxMenu.Items.Add(new ToolStripSeparator());

            ctxMenu.Items.Add(ctxScheduleTest);

            ctxMenu.Items.Add(new ToolStripSeparator());

            ctxMenu.Items.Add(ctxIssueDrivingLicense);
            ctxMenu.Items.Add(ctxShowLicense);
            ctxMenu.Items.Add(ctxShowPersonLicenseHistory);
        }

        #endregion

        #region Load Data

        private void _LoadData()
        {
            _full =
                clsLocalDrivingLicenseApplication
                    .GetAllLocalDrivingLicenseApplications();

            _BindGrid(_full);
        }

        private void _BindGrid(DataTable dt)
        {
            dgv.DataSource = dt;

            lblCount.Text = "Records: " + dt.Rows.Count;

            _Rename("localdrivinglicenseapplicationid","App ID");

            _Rename("personid","Person ID");

            _Rename("classname", "License Class");

            _Rename("nationalno", "National No.");

            _Rename("fullname", "Full Name");

            _Rename("applicationdate", "Date");

            _Rename("passedtestscount", "Passed Tests");

            _Rename("status", "Status");

            _SetColumnWidths();
        }

        private void _SetColumnWidths()
        {
            _SetWidth("localdrivinglicenseapplicationid", 80);

            _SetWidth("personid", 80);

            _SetWidth("classname", 250);

            _SetWidth("nationalno", 140);

            _SetWidth("fullname", 250);

            _SetWidth("applicationdate", 140);

            _SetWidth("passedtestscount", 115);

            _SetWidth("status", 110);

            // Center small/value columns.
            _SetAlignment("localdrivinglicenseapplicationid", DataGridViewContentAlignment.MiddleCenter);

            _SetAlignment("personid", DataGridViewContentAlignment.MiddleCenter);

            _SetAlignment("applicationdate", DataGridViewContentAlignment.MiddleCenter);

            _SetAlignment("passedtestscount", DataGridViewContentAlignment.MiddleCenter);

            _SetAlignment("status", DataGridViewContentAlignment.MiddleCenter);
        }

        private void _SetWidth(string columnName, int width)
        {
            if (dgv.Columns.Contains(columnName))
                dgv.Columns[columnName].Width = width;
        }

        private void _SetAlignment(string columnName, DataGridViewContentAlignment alignment)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]
                    .DefaultCellStyle
                    .Alignment = alignment;
            }
        }

        #endregion

        #region Filter

        private void CbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool showFilter =
                cbFilterBy.SelectedIndex > 0;

            txtFilter.Visible = showFilter;
            txtFilter.Clear();

            if (!showFilter)
                _BindGrid(_full);

            if (showFilter)
                txtFilter.Focus();
        }

        private void TxtFilter_TextChanged(object sender, EventArgs e)
        {
            _Filter();
        }

        private void _Filter()
        {
            if (_full == null)
                return;

            string filterType =
                cbFilterBy.Text.Trim();

            string value =
                txtFilter.Text.Trim();

            if (filterType == "None")
            {
                _BindGrid(_full);
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                _BindGrid(_full);
                return;
            }

            switch (filterType)
            {
                case "Application ID":
                    _FilterNumeric(
                        "localdrivinglicenseapplicationid",
                        value);
                    break;

                case "Person ID":
                    _FilterNumeric(
                        "personid",
                        value);
                    break;

                case "National No.":
                    _FilterString(
                        "nationalno",
                        value);
                    break;

                case "Full Name":
                    _FilterString(
                        "fullname",
                        value);
                    break;

                case "License Class":
                    _FilterString(
                        "classname",
                        value);
                    break;

                case "Status":
                    _FilterString(
                        "status",
                        value);
                    break;

                default:
                    _BindGrid(_full);
                    break;
            }
        }

        private void _FilterNumeric(
            string columnName,
            string value)
        {
            int number;

            if (!int.TryParse(value, out number))
            {
                // Numeric filters should only accept numbers.
                _BindGrid(
                    _full.Clone());

                return;
            }

            if (!_full.Columns.Contains(columnName))
            {
                _BindGrid(_full);
                return;
            }

            DataView dv =
                new DataView(_full);

            dv.RowFilter =
                string.Format(
                    "[{0}] = {1}",
                    columnName,
                    number);

            _BindGrid(dv.ToTable());
        }

        private void _FilterString(
            string columnName,
            string value)
        {
            if (!_full.Columns.Contains(columnName))
            {
                _BindGrid(_full);
                return;
            }

            DataView dv =
                new DataView(_full);

            string escapedValue =
                value
                    .Replace("'", "''")
                    .Replace("[", "[[]")
                    .Replace("%", "[%")
                    .Replace("*", "[*");

            dv.RowFilter =
                string.Format(
                    "CONVERT([{0}], System.String) LIKE '%{1}%'",
                    columnName,
                    escapedValue);

            _BindGrid(dv.ToTable());
        }

        #endregion

        #region Selection

        private int _SelectedID()
        {
            if (dgv.SelectedRows.Count == 0)
                return -1;

            DataGridViewRow row =
                dgv.SelectedRows[0];

            string columnName =
                "localdrivinglicenseapplicationid";

            if (!dgv.Columns.Contains(columnName))
                return -1;

            object value =
                row.Cells[columnName].Value;

            if (value == null ||
                value == DBNull.Value)
            {
                return -1;
            }

            int id;

            return int.TryParse(
                value.ToString(),
                out id)
                ? id
                : -1;
        }

        private int _SelectedPersonID()
        {
            if (dgv.SelectedRows.Count == 0)
                return -1;

            if (!dgv.Columns.Contains("personid"))
                return -1;

            object value =
                dgv.SelectedRows[0]
                   .Cells["personid"]
                   .Value;

            if (value == null ||
                value == DBNull.Value)
            {
                return -1;
            }

            int id;

            return int.TryParse(
                value.ToString(),
                out id)
                ? id
                : -1;
        }

        #endregion

        #region Context Menu Actions

        private void CtxShowDetails_Click( object sender, EventArgs e)
        {
            _ShowDetails();
        }

        private void CtxEdit_Click(object sender, EventArgs e)
        {
            _OpenEdit();
        }

        private void CtxDelete_Click(object sender, EventArgs e)
        {
            _Delete();
        }

        private void CtxCancel_Click(object sender, EventArgs e)
        {
            _CancelApplication();
        }

        private void CtxVisionTest_Click(object sender, EventArgs e)
        {
            _ScheduleTest(clsTestType.enTestType.Vision);
        }

        private void CtxWrittenTest_Click(object sender, EventArgs e)
        {
            _ScheduleTest(clsTestType.enTestType.Written);
        }

        private void CtxPracticalTest_Click(object sender, EventArgs e)
        {
            _ScheduleTest(clsTestType.enTestType.Practical);
        }

        private void CtxIssueDrivingLicense_Click(object sender, EventArgs e)
        {
            _IssueDrivingLicense();
        }

        private void CtxShowLicense_Click(object sender, EventArgs e)
        {
            _ShowLicense();
        }

        private void CtxShowPersonLicenseHistory_Click(object sender, EventArgs e)
        {
            _ShowPersonLicenseHistory();
        }

        private void CtxScheduleTest_DropDownOpening(object sender, EventArgs e)
        {
            int id = _SelectedID();

            if (id < 0)
            {
                ctxVisionTest.Enabled       = false;
                ctxWrittenTest.Enabled      = false;
                ctxPracticalTest.Enabled    = false;
                return;
            }

            ctxVisionTest.Enabled =
                _CanScheduleTestType(id, (int)clsTestType.enTestType.Vision);

            ctxWrittenTest.Enabled =
                _CanScheduleTestType(id, (int)clsTestType.enTestType.Written);

            ctxPracticalTest.Enabled =
                _CanScheduleTestType(id, (int)clsTestType.enTestType.Practical);
        }

        #endregion

        #region Application Actions

        private void _ShowDetails()
        {
            int id = _SelectedID();

            if (id < 0)
                return;

            new frmShowLocalDrivingLicenseApplicationInfo(id)
                .ShowDialog();
        }

        private void _OpenEdit()
        {
            int id = _SelectedID();

            if (id < 0)
                return;

            new frmAddEditNewLocalDrivingLicenseApplication(id)
                .ShowDialog();

            _LoadData();
        }

        private void _Delete()
        {
            int id = _SelectedID();

            if (id < 0)
                return;

            if (!clsUtil.ConfirmDelete(
                    "this application"))
            {
                return;
            }

            clsLocalDrivingLicenseApplication application =
                clsLocalDrivingLicenseApplication
                    .FindByLocalDrivingAppID(id);

            if (application == null)
            {
                clsUtil.ShowError("Application not found.");

                return;
            }

            if (application.Delete())
            {
                _LoadData();
            }
            else
            {
                clsUtil.ShowError("Cannot delete this application.\n\n" +
                    "It may have linked records.");
            }
        }

        private void _CancelApplication()
        {
            int id = _SelectedID();

            if (id < 0)
                return;

            clsLocalDrivingLicenseApplication application =
                clsLocalDrivingLicenseApplication
                    .FindByLocalDrivingAppID(id);

            if (application == null)
            {
                clsUtil.ShowError(
                    "Application not found.");

                return;
            }

            if (application.ApplicationStatus ==
                clsApplication.enApplicationStatus.Cancelled)
            {
                MessageBox.Show(
                    "This application is already cancelled.",
                    "Cancel Application",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            if (application.ApplicationStatus ==
                clsApplication.enApplicationStatus.Completed)
            {
                MessageBox.Show(
                    "A completed application cannot be cancelled.",
                    "Cancel Application",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult result =
                MessageBox.Show(
                    "Are you sure you want to cancel this application?",
                    "Cancel Application",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            if (application.Cancel())
            {
                _LoadData();

                MessageBox.Show(
                    "Application cancelled successfully.",
                    "Cancel Application",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                clsUtil.ShowError(
                    "Failed to cancel the application.");
            }
        }

        private void _OpenAddNew()
        {
            new frmAddEditNewLocalDrivingLicenseApplication()
                .ShowDialog();

            _LoadData();
        }

        #endregion

        #region Test Scheduling

        // A test type can be scheduled only if:
        //   1. It hasn't already been passed (no point rescheduling a passed test), and
        //   2. Every test type before it in the sequence (Vision -> Written -> Practical)
        //      has already been passed.
        //
        // NOTE: this rule is also implemented separately in
        // ctrlScheduleTest._ValidateTestSequence() and
        // frmListTestAppointments._ValidateTestSequence(). Having it a third
        // time here is a maintenance risk - if DVLD ever changes the required
        // test order, all three places must be updated together. Worth pulling
        // into a single static method (e.g.
        // clsLocalDrivingLicenseApplication.CanScheduleTestType) in the
        // Business layer the next time this file is touched.
        private static bool _CanScheduleTestType(int localDrivingLicenseApplicationID, int testTypeID)
        {
            if (clsLocalDrivingLicenseApplication.IsTestPassed(
                    localDrivingLicenseApplicationID, testTypeID))
            {
                return false;
            }

            for (int previousTestID = 1; previousTestID < testTypeID; previousTestID++)
            {
                if (!clsLocalDrivingLicenseApplication.IsTestPassed(
                        localDrivingLicenseApplicationID, previousTestID))
                {
                    return false;
                }
            }

            return true;
        }

        private void _ScheduleTest(clsTestType.enTestType testType)
        {
            int id = _SelectedID();

            if (id < 0)
                return;

            clsLocalDrivingLicenseApplication application =
                clsLocalDrivingLicenseApplication
                    .FindByLocalDrivingAppID(id);

            if (application == null)
            {
                clsUtil.ShowError(
                    "Application not found.");

                return;
            }

            if (application.IsAllTestsPassed())
            {
                MessageBox.Show(
                    "All three tests have already been passed.",
                    "Schedule Test",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            // Defensive re-check: the menu item should already be disabled
            // for this case (see CtxScheduleTest_DropDownOpening), so this
            // path should be unreachable through the UI - but we don't
            // trust that blindly here.
            if (!_CanScheduleTestType(id, (int)testType))
            {
                clsUtil.ShowWarning(
                    $"You cannot schedule the {testType} test yet.");

                return;
            }

            using (frmListTestAppointments frm =
                new frmListTestAppointments(id, testType))
            {
                frm.ShowDialog();
            }

            // Refresh so columns like "Passed Tests" reflect anything that
            // happened while the appointments list / schedule dialog was open.
            _LoadData();
        }

        #endregion

        private void _IssueDrivingLicense()
        {
            int id = _SelectedID();

            if (id < 0)
                return;

            clsLocalDrivingLicenseApplication application =
                clsLocalDrivingLicenseApplication
                    .FindByLocalDrivingAppID(id);

            if (application == null)
            {
                clsUtil.ShowError(
                    "Application not found.");

                return;
            }

            if (!application.IsAllTestsPassed())
            {
                MessageBox.Show(
                    "The applicant must pass all three tests first.",
                    "Issue Driving License",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (application.IsLicenseIssued())
            {
                MessageBox.Show(
                    "A driving license has already been issued for this application.",
                    "Issue Driving License",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            MessageBox.Show(
                "The first-time Issue Driving License form is not available " +
                "in the current GitHub branch.",
                "Issue Driving License",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void _ShowLicense()
        {
            int id = _SelectedID();

            if (id < 0)
                return;

            clsLocalDrivingLicenseApplication application =
                clsLocalDrivingLicenseApplication
                    .FindByLocalDrivingAppID(id);

            if (application == null)
            {
                clsUtil.ShowError(
                    "Application not found.");

                return;
            }

            int licenseID =
                application.GetActiveLicenseID();

            if (licenseID == -1)
            {
                MessageBox.Show(
                    "No active driving license exists for this application.",
                    "Show License",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            MessageBox.Show(
                "License ID: " + licenseID +
                "\n\n" +
                "The Show License form is not available in the current branch.",
                "Show License",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void _ShowPersonLicenseHistory()
        {
            int personID = _SelectedPersonID();

            if (personID < 0)
            {
                MessageBox.Show(
                    "Person ID is not available in the current application list.",
                    "License History",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            MessageBox.Show(
                "The Person License History form is not available in the current branch.",
                "License History",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #region Grid Events

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

            DataGridView.HitTestInfo h =
                dgv.HitTest(e.X, e.Y);

            if (h.RowIndex >= 0)
            {
                dgv.ClearSelection();
                dgv.Rows[h.RowIndex].Selected = true;
            }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection =
                dgv.SelectedRows.Count > 0;

            ctxShowDetails.Enabled =
                hasSelection;

            ctxEdit.Enabled =
                hasSelection;

            ctxDelete.Enabled =
                hasSelection;

            ctxCancel.Enabled =
                hasSelection;

            ctxScheduleTest.Enabled =
                hasSelection;

            ctxIssueDrivingLicense.Enabled =
                hasSelection;

            ctxShowLicense.Enabled =
                hasSelection;

            ctxShowPersonLicenseHistory.Enabled =
                hasSelection;
        }

        #endregion

        #region Button Events

        private void BtnAddNew_Click(object sender, EventArgs e)
        {_OpenAddNew();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Helpers

        private void _Rename(string columnName ,string headerText)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName]
                    .HeaderText = headerText;
            }
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

            g.EnableHeadersVisualStyles = false;

            g.DefaultCellStyle.SelectionBackColor =
                clsGlobal.GridSelectionBack;

            g.DefaultCellStyle.SelectionForeColor =
                Color.White;

            g.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 248, 255);
        }

        private static Button _Btn(
            string text,
            int x,
            int y,
            Color color)
        {
            Button button =
                new Button
                {
                    Text = text,
                    Location = new Point(x, y),
                    Size = new Size(150, 34),

                    Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),

                    BackColor = color,
                    ForeColor = Color.White,

                    FlatStyle =
                        FlatStyle.Flat,

                    Cursor =
                        Cursors.Hand
                };

            button.FlatAppearance.BorderSize = 0;

            return button;
        }

        #endregion
    }
}