using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmListPeople : Form
    {
        #region Controls Declaration

        private Label           lblTitle;
        private PictureBox      pbHeader;

        private Label           lblFilterBy;
        private ComboBox        cbFilterBy;
        private TextBox         txtFilterValue;

        private DataGridView    dgvPeople;
        private Button          btnAddNew;
        private Button          btnClose;

        private ContextMenuStrip    ctxMenu;
        private ToolStripMenuItem   ctxShowDetails;
        private ToolStripMenuItem   ctxAddNew;
        private ToolStripMenuItem   ctxEdit;
        private ToolStripMenuItem   ctxDelete;
        private ToolStripMenuItem   ctxSendEmail;
        private ToolStripMenuItem   ctxPhoneCall;

        private Label           lblRecordCount;

        #endregion

        #region State

        private DataTable _fullTable = null;    // all rows, used for client-side filter

        #endregion

        // ── Constructor ─────────────────────────────────────────────────────

        public frmListPeople()
        {
            _InitializeComponents();
            _LoadPeople();
        }

        // ── Form Build ──────────────────────────────────────────────────────

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text            = "Manage People";
            this.Size            = new Size(1260, 700);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.BackColor       = Color.White;
            this.Font            = new Font("Microsoft Sans Serif", 9.5F);

            // ── Header image (group-of-people icon area) ─────────────
            pbHeader = new PictureBox
            {
                Location  = new Point(540, 15),
                Size      = new Size(90, 75),
                SizeMode  = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                // Assign pbHeader.Image from your resources if available
            };

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text      = "Manage People",
                Font      = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize  = true,
                Location  = new Point(490, 95)
            };

            // ── Filter row ───────────────────────────────────────────
            lblFilterBy = new Label
            {
                Text     = "Filter By:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 140)
            };

            cbFilterBy = new ComboBox
            {
                Location      = new Point(110, 137),
                Size          = new Size(160, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor        = Cursors.Hand
            };
            cbFilterBy.Items.AddRange(new object[]
            {
                "None", "Person ID", "National No.", "First Name",
                "Second Name", "Third Name", "Last Name",
                "Nationality", "Gender", "Phone", "Email"
            });
            cbFilterBy.SelectedIndex = 0;
            cbFilterBy.SelectedIndexChanged += cbFilterBy_SelectedIndexChanged;

            txtFilterValue = new TextBox
            {
                Location = new Point(280, 137),
                Size     = new Size(220, 23),
                Font     = new Font("Microsoft Sans Serif", 9.5F),
                Visible  = false
            };
            txtFilterValue.TextChanged += txtFilterValue_TextChanged;

            // ── Add New button (top-right) ───────────────────────────
            btnAddNew = new Button
            {
                Text      = "Add New Person",
                Location  = new Point(1075, 130),
                Size      = new Size(155, 36),
                Font      = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnAddNew.FlatAppearance.BorderSize = 0;
            btnAddNew.Click += btnAddNew_Click;

            // ── Context menu ─────────────────────────────────────────
            ctxMenu = new ContextMenuStrip();
            ctxMenu.Font = new Font("Microsoft Sans Serif", 9.5F);

            ctxShowDetails = new ToolStripMenuItem("👤  Show Details");
            ctxAddNew      = new ToolStripMenuItem("➕  Add New Person");
            ctxEdit        = new ToolStripMenuItem("✏️  Edit");
            ctxDelete      = new ToolStripMenuItem("🗑  Delete");
            ctxSendEmail   = new ToolStripMenuItem("📧  Send Email");
            ctxPhoneCall   = new ToolStripMenuItem("📞  Phone Call");

            ctxShowDetails.Click += (s, e) => _ShowDetails();
            ctxAddNew.Click      += (s, e) => _OpenAddNew();
            ctxEdit.Click        += (s, e) => _OpenEdit();
            ctxDelete.Click      += (s, e) => _DeleteSelected();
            ctxSendEmail.Click   += (s, e) => _SendEmail();
            ctxPhoneCall.Click   += (s, e) => _PhoneCall();

            ctxMenu.Items.AddRange(new ToolStripItem[]
            {
                ctxShowDetails,
                new ToolStripSeparator(),
                ctxAddNew, ctxEdit, ctxDelete,
                new ToolStripSeparator(),
                ctxSendEmail, ctxPhoneCall
            });

            // ── DataGridView ─────────────────────────────────────────
            dgvPeople = new DataGridView
            {
                Location              = new Point(30, 180),
                Size                  = new Size(1200, 440),
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                ReadOnly              = true,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect           = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight   = 36,
                RowTemplate           = { Height = 28 },
                BorderStyle           = BorderStyle.None,
                BackgroundColor       = Color.White,
                GridColor             = Color.FromArgb(220, 225, 235),
                ContextMenuStrip      = ctxMenu,
                Cursor                = Cursors.Hand
            };
            _StyleGrid(dgvPeople);

            dgvPeople.CellDoubleClick          += dgvPeople_CellDoubleClick;
            dgvPeople.MouseDown                += dgvPeople_MouseDown;
            dgvPeople.SelectionChanged         += dgvPeople_SelectionChanged;

            // ── Record-count label ───────────────────────────────────
            lblRecordCount = new Label
            {
                Text      = "Records: 0",
                AutoSize  = true,
                Location  = new Point(30, 630),
                ForeColor = Color.Gray,
                Font      = new Font("Microsoft Sans Serif", 9F)
            };

            // ── Close button ─────────────────────────────────────────
            btnClose = new Button
            {
                Text      = "✖  Close",
                Location  = new Point(1075, 622),
                Size      = new Size(155, 36),
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(192, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            // ── Add all to form ──────────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                pbHeader, lblTitle,
                lblFilterBy, cbFilterBy, txtFilterValue,
                btnAddNew,
                dgvPeople,
                lblRecordCount,
                btnClose
            });
        }

        // ── Grid styling ────────────────────────────────────────────────────

        private static void _StyleGrid(DataGridView dgv)
        {
            // Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor   = clsGlobal.GridHeaderBack;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor   = clsGlobal.GridHeaderFore;
            dgv.ColumnHeadersDefaultCellStyle.Font        = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment   = DataGridViewContentAlignment.MiddleCenter;
            dgv.EnableHeadersVisualStyles                  = false;

            // Row style
            dgv.DefaultCellStyle.Font          = new Font("Microsoft Sans Serif", 9F);
            dgv.DefaultCellStyle.SelectionBackColor = clsGlobal.GridSelectionBack;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            // Alternate row colour
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
        }

        // ── Data ────────────────────────────────────────────────────────────

        private void _LoadPeople()
        {
            _fullTable = clsPerson.GetAllPeople();
            _BindGrid(_fullTable);
        }

        private void _BindGrid(DataTable dt)
        {
            dgvPeople.DataSource = dt;
            lblRecordCount.Text  = $"Records: {dt.Rows.Count}";

            // Friendly column headers (in case the SP uses raw column names)
            _RenameColumnIfExists("PersonID",            "Person ID");
            _RenameColumnIfExists("NationalNo",          "National No.");
            _RenameColumnIfExists("FirstName",           "First Name");
            _RenameColumnIfExists("SecondName",          "Second Name");
            _RenameColumnIfExists("ThirdName",           "Third Name");
            _RenameColumnIfExists("LastName",            "Last Name");
            _RenameColumnIfExists("Gendor",              "Gender");
            _RenameColumnIfExists("DateOfBirth",         "Date Of Birth");
            _RenameColumnIfExists("countryname",         "Nationality");
            _RenameColumnIfExists("Phone",               "Phone");
            _RenameColumnIfExists("Email",               "Email");

            // Hide internal columns the user doesn't need to see
            _HideColumnIfExists("ImagePath");
            _HideColumnIfExists("Address");

            // Reorder columns: move Email to last position
            _ReorderColumns();

            // Auto-size columns to fit their content
            _AutoSizeColumns();
        }

        private void _RenameColumnIfExists(string dataName, string displayName)
        {
            if (dgvPeople.Columns.Contains(dataName))
                dgvPeople.Columns[dataName].HeaderText = displayName;
        }

        private void _HideColumnIfExists(string name)
        {
            if (dgvPeople.Columns.Contains(name))
                dgvPeople.Columns[name].Visible = false;
        }

        private void _ReorderColumns()
        {
            // Move Email column to the last position
            if (dgvPeople.Columns.Contains("Email"))
            {
                dgvPeople.Columns["Email"].DisplayIndex = dgvPeople.Columns.Count - 1;
            }
        }

        private void _AutoSizeColumns()
        {
            // Start with fixed widths for columns that have predictable content.
            _SetColumnWidth("PersonID", 75);
            _SetColumnWidth("NationalNo", 105);

            _SetColumnWidth("FirstName", 110);
            _SetColumnWidth("SecondName", 110);
            _SetColumnWidth("ThirdName", 110);
            _SetColumnWidth("LastName", 110);

            _SetColumnWidth("Gendor", 75);
            _SetColumnWidth("Gender", 75);

            _SetColumnWidth("DateOfBirth", 105);
            _SetColumnWidth("countryname", 110);
            _SetColumnWidth("Phone", 120);

            // Let Email use whatever space is left.
            if (dgvPeople.Columns.Contains("Email"))
            {
                dgvPeople.Columns["Email"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void _SetColumnWidth(string columnName, int width)
        {
            if (!dgvPeople.Columns.Contains(columnName))
                        return;

            DataGridViewColumn column = dgvPeople.Columns[columnName];

            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = width;
        }

        // ── Filter logic ─────────────────────────────────────────────────────

        private void _ApplyFilter()
        {
            if (_fullTable == null) return;

            string col   = cbFilterBy.Text;
            string value = txtFilterValue.Text.Trim();

            if (col == "None" || string.IsNullOrEmpty(value))
            {
                _BindGrid(_fullTable);
                return;
            }

            // Map display name → DataTable column name
            string dbCol = col switch
            {
                "Person ID"    => "PersonID",
                "National No." => "NationalNo",
                "First Name"   => "FirstName",
                "Second Name"  => "SecondName",
                "Third Name"   => "ThirdName",
                "Last Name"    => "LastName",
                "Nationality"  => "countryname",
                "Gender"       => "Gender",
                "Phone"        => "Phone",
                "Email"        => "Email",
                _              => null
            };

            if (dbCol == null) return;

            try
            {
                DataView dv   = new DataView(_fullTable);
                string escaped = value.Replace("[", "[[]").Replace("%", "[%]").Replace("'", "''");

                // For Person ID, only allow digits
                if (dbCol == "PersonID")
                {
                    // Validate that the filter value contains only digits
                    if (!Regex.IsMatch(value, @"^\d+$"))
                    {
                        // If not all digits, show no results
                        dv.RowFilter = "1 = 0";
                    }
                    else
                    {
                        dv.RowFilter = $"[{dbCol}] = {value}";
                    }
                }
                // For Gender, use exact match to avoid partial matches (e.g., "male" matching "female")
                else if (dbCol == "Gendor")
                {
                    dv.RowFilter = $"[{dbCol}] = '{value}'";
                }
                // For Nationality and other fields, use contains search
                else
                {
                    dv.RowFilter = $"CONVERT([{dbCol}], System.String) LIKE '%{value}%'";
                }
                _BindGrid(dv.ToTable());
            }
            catch { /* ignore invalid filter expressions while user is still typing */ }
        }

        // ── Row helpers ──────────────────────────────────────────────────────

        private int _SelectedPersonID()
        {
            if (dgvPeople.SelectedRows.Count == 0) return -1;

            var cell = dgvPeople.SelectedRows[0].Cells["PersonID"];
            if (cell?.Value == null) return -1;

            return Convert.ToInt32(cell.Value);
        }

        private string _SelectedEmail()
        {
            if (dgvPeople.SelectedRows.Count == 0) return string.Empty;
            var cell = dgvPeople.SelectedRows[0].Cells["Email"];
            return cell?.Value?.ToString() ?? string.Empty;
        }

        private string _SelectedPhone()
        {
            if (dgvPeople.SelectedRows.Count == 0) return string.Empty;
            var cell = dgvPeople.SelectedRows[0].Cells["Phone"];
            return cell?.Value?.ToString() ?? string.Empty;
        }

        // ── Actions ──────────────────────────────────────────────────────────

        private void _ShowDetails()
        {
            int id = _SelectedPersonID();
            if (id == -1) return;

            new frmShowPersonInfo(id).ShowDialog();
        }

        private void _OpenAddNew()
        {
            var frm = new frmAddEditPerson();
            frm.DataBack += (s, personID) => _LoadPeople();
            frm.ShowDialog();
        }

        private void _OpenEdit()
        {
            int id = _SelectedPersonID();
            if (id == -1) return;

            var frm = new frmAddEditPerson(id);
            frm.DataBack += (s, personID) => _LoadPeople();
            frm.ShowDialog();
        }

        private void _DeleteSelected()
        {
            int id = _SelectedPersonID();
            if (id == -1) return;

            if (!clsUtil.ConfirmDelete("this person")) return;

            if (clsPerson.Delete(id))
            {
                clsUtil.ShowInfo("Person deleted successfully.", "Deleted");
                _LoadPeople();
            }
            else
            {
                clsUtil.ShowError("Could not delete this person.\n" +
                                  "They may be linked to existing records.");
            }
        }

        private void _SendEmail()
        {
            string email = _SelectedEmail();
            if (string.IsNullOrWhiteSpace(email))
            { clsUtil.ShowWarning("No email address on record for this person."); return; }
            clsUtil.SendEmail(email);
        }

        private void _PhoneCall()
        {
            string phone = _SelectedPhone();
            if (string.IsNullOrWhiteSpace(phone))
            { clsUtil.ShowWarning("No phone number on record for this person."); return; }
            clsUtil.MakePhoneCall(phone);
        }

        // ── Events ───────────────────────────────────────────────────────────

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasFilter = cbFilterBy.SelectedIndex > 0;
            txtFilterValue.Visible = hasFilter;
            txtFilterValue.Text    = string.Empty;

            if (!hasFilter)
                _BindGrid(_fullTable);   // restore full list immediately
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
            => _ApplyFilter();

        private void dgvPeople_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            _ShowDetails();
        }

        private void dgvPeople_MouseDown(object sender, MouseEventArgs e)
        {
            // Right-click: select the row under the cursor before showing the menu
            if (e.Button == MouseButtons.Right)
            {
                var hit = dgvPeople.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                    dgvPeople.Rows[hit.RowIndex].Selected = true;
            }
        }

        private void dgvPeople_SelectionChanged(object sender, EventArgs e)
        {
            // Enable / disable row-specific menu items
            bool hasSelection = dgvPeople.SelectedRows.Count > 0;
            ctxShowDetails.Enabled = hasSelection;
            ctxEdit.Enabled        = hasSelection;
            ctxDelete.Enabled      = hasSelection;
            ctxSendEmail.Enabled   = hasSelection;
            ctxPhoneCall.Enabled   = hasSelection;
        }

        private void btnAddNew_Click(object sender, EventArgs e)
            => _OpenAddNew();
    }
}