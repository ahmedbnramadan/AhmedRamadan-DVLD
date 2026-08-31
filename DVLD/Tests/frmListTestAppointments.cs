using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD.Tests
{
    public class frmListTestAppointments : Form
    {
        #region Controls Declaration

        private Label lblTitle;
        private ctrlDrivingLicenseApplicationInfo ctrlDrivingLicenseApplicationInfo1;
        private DataGridView dgv;
        private Button btnAddNew;
        private Button btnClose;
        private Label lblCount;

        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxEdit;
        private ToolStripMenuItem ctxTakeTest;

        #endregion

        #region State

        private readonly int _LocalDrivingLicenseApplicationID;
        private readonly clsTestType.enTestType _TestType;
        private DataTable _dt;

        #endregion

        #region Constructors

        public frmListTestAppointments(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestType)
        {
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestType = TestType;

            _InitializeComponents();
            _SetupEvents();
        }

        #endregion

        #region Initialization

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text = "Test Appointments";
            this.FormBorderStyle    = FormBorderStyle.FixedDialog;
            this.ClientSize         = new Size(1050, 760);
            this.StartPosition      = FormStartPosition.CenterScreen;
            this.FormBorderStyle    = FormBorderStyle.FixedDialog;
            this.MaximizeBox        = false;
            this.MinimizeBox        = false;
            this.BackColor          = Color.FromArgb(240, 242, 248);
            this.Font               = new Font("Microsoft Sans Serif", 10F);

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text = "Test Appointments",
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            // ── Context Menu (built BEFORE the grid, since the grid needs it) ──
            ctxMenu = new ContextMenuStrip();

            ctxEdit = new ToolStripMenuItem
            {
                Text = "Edit"
            };

            ctxTakeTest = new ToolStripMenuItem
            {
                Text = "Take Test"
            };

            ctxMenu.Items.AddRange(new ToolStripItem[] { ctxEdit, ctxTakeTest });

            // ── Driving License Application Info Control ─────────────
            ctrlDrivingLicenseApplicationInfo1 = new ctrlDrivingLicenseApplicationInfo
            {
                Location = new Point(30, 60),
                Size = new Size(990, 400),
                Dock = DockStyle.None
            };

            // ── DataGridView ─────────────────────────────────────────
            dgv = new DataGridView
            {
                Location = new Point(30, 470),
                Size = new Size(990, 200),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 34,
                RowTemplate = { Height = 28 },
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(220, 225, 235),
                ContextMenuStrip = ctxMenu,   // ← now valid, ctxMenu already exists
                Cursor = Cursors.Hand,
                ReadOnly = true
            };

            _ConfigureDataGridViewColumns();

            // ── Count label ──────────────────────────────────────────
            lblCount = new Label
            {
                Text = "Records: 0",
                AutoSize = true,
                Location = new Point(30, 685),
                ForeColor = Color.Gray,
                Font = new Font("Microsoft Sans Serif", 9F)
            };

            // ── Add New button ───────────────────────────────────────
            btnAddNew = new Button
            {
                Text = "Schedule Test",
                Location = new Point(30, 705),
                Size = new Size(180, 40),
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                BackColor = clsGlobal.PrimaryBlue,  
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddNew.FlatAppearance.BorderSize = 0;

            // ── Close button ─────────────────────────────────────────
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(835, 705),
                Size = new Size(180, 40),
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                BackColor = clsGlobal.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;

            // ── Add to form ───────────────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                ctrlDrivingLicenseApplicationInfo1,
                dgv,
                lblCount,
                btnClose,
                btnAddNew
            });
        }

        private void _ConfigureDataGridViewColumns()
        {
            dgv.Columns.Clear();

            // Test Appointment ID
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "testappointmentid",
                HeaderText = "Appointment ID",
                FillWeight = 100,
                SortMode = DataGridViewColumnSortMode.Automatic
            });

            // Test Type
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "testtypetitle",
                HeaderText = "Test Type",
                FillWeight = 120,
                SortMode = DataGridViewColumnSortMode.Automatic
            });

            // Appointment Date
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "appointmentdate",
                HeaderText = "Appointment Date",
                FillWeight = 140,
                SortMode = DataGridViewColumnSortMode.Automatic,
                DefaultCellStyle = { Format = "dd/MMM/yyyy" }
            });

            // Paid Fees
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "paidfees",
                HeaderText = "Paid Fees",
                FillWeight = 100,
                SortMode = DataGridViewColumnSortMode.Automatic,
                DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // Created By
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "createdbyusername",
                HeaderText = "Created By",
                FillWeight = 130,
                SortMode = DataGridViewColumnSortMode.Automatic
            });

            // Is Locked
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "islocked",
                HeaderText = "Locked",
                FillWeight = 80,
                SortMode = DataGridViewColumnSortMode.Automatic,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Retake Application ID
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "retaketestapplicationid",
                HeaderText = "Retake App ID",
                FillWeight = 100,
                SortMode = DataGridViewColumnSortMode.Automatic,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
        }

        private void _SetupEvents()
        {
            this.Load           += frmListTestAppointments_Load;
            btnClose.Click      += btnClose_Click;
            btnAddNew.Click     += btnAddNew_Click;
            ctxEdit.Click       += ctxEdit_Click;
            ctxTakeTest.Click   += ctxTakeTest_Click;
            dgv.CellDoubleClick += Dgv_CellDoubleClick;

            // Keep the context menu's Enabled state in sync with the current
            // selection, not just whatever it happened to be at load time.
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        #endregion

        #region Event Handlers

        private void frmListTestAppointments_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            _HandleScheduleTest();
        }

        private void ctxEdit_Click(object sender, EventArgs e)
        {
            _HandleEditTest();
        }

        private void ctxTakeTest_Click(object sender, EventArgs e)
        {
            _HandleTakeTest();
        }

        private void Dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _HandleEditTest();
            }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            _UpdateContextMenuAvailability();
        }

        #endregion

        #region Private Methods

        private void _LoadData()
        {
            if (_LocalDrivingLicenseApplicationID <= 0)
            {
                clsUtil.ShowError("Invalid Application ID.", "Error");
                this.Close();
                return;
            }

            try
            {
                // Load application info
                ctrlDrivingLicenseApplicationInfo1.LoadApplicationInfo(_LocalDrivingLicenseApplicationID);

                // Load test appointments for this application and test type
                _dt = clsTestAppointment.GetApplicationAppointmentsPerTestType(
                    _LocalDrivingLicenseApplicationID,
                    (int)_TestType
                );

                dgv.DataSource = _dt;
                lblCount.Text = $"Records: {_dt.Rows.Count}";

                _UpdateContextMenuAvailability();
            }
            catch (Exception ex)
            {
                clsUtil.ShowError($"Failed to load test appointments.\n{ex.Message}", "Error");
            }
        }

        private void _UpdateContextMenuAvailability()
        {
            bool hasSelection = dgv.SelectedRows.Count > 0;
            ctxEdit.Enabled = hasSelection;
            ctxTakeTest.Enabled = hasSelection;
        }

        private int _GetSelectedTestAppointmentID()
        {
            if (dgv.SelectedRows.Count > 0)
            {
                if (int.TryParse(dgv.SelectedRows[0].Cells["testappointmentid"].Value?.ToString(), out int id))
                {
                    return id;
                }
            }
            return -1;
        }

        private void _HandleScheduleTest()
        {
            // Validate: Check if person has active application
            var localApp = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppID(_LocalDrivingLicenseApplicationID);
            if (localApp == null)
            {
                clsUtil.ShowError("Application not found.", "Error");
                return;
            }

            // Check if person already has an active license of this class
            if (localApp.DoesPersonHaveActiveLicense())
            {
                clsUtil.ShowError("Person already has an active license of this class.", "Validation Error");
                return;
            }

            // Check if there's already an active appointment for this test type
            if (clsLocalDrivingLicenseApplication.IsThereAnActiveScheduledTest(_LocalDrivingLicenseApplicationID, (int)_TestType))
            {
                clsUtil.ShowError($"There is already an active scheduled appointment for {_TestType} test.", "Validation Error");
                return;
            }

            // Validate test sequence - check if previous tests are passed
            if (!_ValidateTestSequence())
            {
                return;
            }

            // Check if already taken this test
            if (clsLocalDrivingLicenseApplication.DoesAttendTestType(_LocalDrivingLicenseApplicationID, (int)_TestType))
            {
                clsUtil.ShowError($"You have already taken the {_TestType} test before.", "Validation Error");
                return;
            }

            // Open schedule test form
            using (var frm = new frmScheduleTest(_LocalDrivingLicenseApplicationID, _TestType))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    IsSaved = true;          // ← now actually gets set
                    _LoadData();             // Refresh data
                    clsUtil.ShowSuccess("Test appointment scheduled successfully.", "Success");
                }
            }
        }

        private bool _ValidateTestSequence()
        {
            // Tests must be taken in order: Vision -> Written -> Practical
            // Can't take a test without passing the previous one

            switch (_TestType)
            {
                case clsTestType.enTestType.Vision:
                    // Vision test can always be taken first
                    return true;

                case clsTestType.enTestType.Written:
                    // Must pass Vision test first
                    if (!clsLocalDrivingLicenseApplication.IsTestPassed(_LocalDrivingLicenseApplicationID, (int)clsTestType.enTestType.Vision))
                    {
                        clsUtil.ShowError("You must pass the Vision test before taking the Written test.", "Validation Error");
                        return false;
                    }
                    return true;

                case clsTestType.enTestType.Practical:
                    // Must pass Vision and Written tests first
                    if (!clsLocalDrivingLicenseApplication.IsTestPassed(_LocalDrivingLicenseApplicationID, (int)clsTestType.enTestType.Vision))
                    {
                        clsUtil.ShowError("You must pass the Vision test before taking the Practical test.", "Validation Error");
                        return false;
                    }
                    if (!clsLocalDrivingLicenseApplication.IsTestPassed(_LocalDrivingLicenseApplicationID, (int)clsTestType.enTestType.Written))
                    {
                        clsUtil.ShowError("You must pass the Written test before taking the Practical test.", "Validation Error");
                        return false;
                    }
                    return true;

                default:
                    return false;
            }
        }

        private void _HandleEditTest()
        {
            int testAppointmentID = _GetSelectedTestAppointmentID();
            if (testAppointmentID <= 0)
            {
                clsUtil.ShowError("Please select a test appointment to edit.", "Error");
                return;
            }

            // Check if appointment is locked
            if (clsTestAppointment.IsAppointmentLocked(testAppointmentID))
            {
                clsUtil.ShowError("This appointment is locked and cannot be edited.", "Error");
                return;
            }

            // For now, show coming soon message
            // TODO: Implement edit functionality when frmEditTestAppointment is created
            clsUtil.ShowMessage("Edit Test Appointment feature is coming soon.", "Coming Soon");
        }

        private void _HandleTakeTest()
        {
            int testAppointmentID = _GetSelectedTestAppointmentID();
            if (testAppointmentID <= 0)
            {
                clsUtil.ShowError("Please select a test appointment to take.", "Error");
                return;
            }

            if (clsTestAppointment.IsAppointmentLocked(testAppointmentID))
            {
                clsUtil.ShowError("This appointment is locked - the test has already been taken.", "Error");
                return;
            }

            using (var frm = new frmTakeTest(testAppointmentID, _TestType))
            {
                frm.ShowDialog();
            }

            // Always refresh, regardless of DialogResult - the "Locked" column
            // (and any future result column) needs to reflect what just happened,
            // and the user might have saved without formally "closing OK".
            _LoadData();
        }

        #endregion

        #region Public Properties

        public bool IsSaved { get; private set; }

        #endregion
    }
}