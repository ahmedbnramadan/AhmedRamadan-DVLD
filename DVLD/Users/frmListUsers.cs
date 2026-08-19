using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;
using System.IO;

namespace DVLD
{
    public class frmListUsers : Form
    {
        #region Controls Declaration

        private Label lblTitle;
        private PictureBox pbHeader;

        private Label lblFilterBy;
        private ComboBox cbFilterBy;
        private TextBox txtFilterValue;


        private DataGridView dgvUsers;
        private Button btnAddNew;
        private Button btnClose;

        
        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem ctxShowDetails;
        private ToolStripMenuItem ctxAddNew;
        private ToolStripMenuItem ctxEdit;
        private ToolStripMenuItem ctxDelete;
        private ToolStripMenuItem ctxSendEmail;
        private ToolStripMenuItem ctxPhoneCall;


        private Label lblRecordCount;


        #endregion

        #region State
        private DataTable _fullTable = null;
        #endregion

        // ── Constructor ─────────────────────────────────────────────────────

        public frmListUsers()
        {
            _InitializeComponents();
            _LoadUsers();
        }

        // ── Form Build ──────────────────────────────────────────────────────

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text               = "Manage Users";
            this.Size               = new Size(1260, 700);
            this.StartPosition      = FormStartPosition.CenterScreen;
            this.FormBorderStyle    = FormBorderStyle.FixedDialog;
            this.MaximizeBox        = false;
            this.BackColor          = Color.White;
            this.Font               = new Font("Microsoft Sans Serif", 9.5F);

            // ── Header image (group-of-users icon area) ─────────────
            pbHeader = new PictureBox
            {
                Location    = new Point(540, 15),
                Size        = new Size(90, 75),
                SizeMode    = PictureBoxSizeMode.Zoom,
                BackColor   = Color.Transparent
                // Assign pbHeader.Image from your resources if available
            };

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text        = "Mange Users",
                Font        = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor   = clsGlobal.PrimaryRed,
                AutoSize    = true,
                Location    = new Point(490, 95)
            };

            // ── Filter row ───────────────────────────────────────────
            lblFilterBy = new Label
            {
                Text     = "Filter By",
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

            cbFilterBy.Items.AddRange( new object[]
            {
                "None", "User ID", "Person ID", "Name", "UserName", "is Active"
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
                Text = "➕ Add New User",
                Location = new Point(1075, 130),
                Size = new Size(155, 36),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddNew.FlatAppearance.BorderSize = 0;
            btnAddNew.Click += btnAddNew_Click;

            // ── Context menu ─────────────────────────────────────────
            ctxMenu = new ContextMenuStrip();
            ctxMenu.Font = new Font("Microsoft Sans Serif", 9.5F);

            ctxShowDetails = new ToolStripMenuItem("👤  Show Details");
            ctxAddNew      = new ToolStripMenuItem("➕  Add New User");
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

            ctxMenu.Items.AddRange(new ToolStripItem[] {
                ctxShowDetails,
                new ToolStripSeparator(),
                ctxAddNew, ctxEdit, ctxDelete,
                new ToolStripSeparator(),
                ctxSendEmail, ctxPhoneCall
            });

            // ── DataGridView ─────────────────────────────────────────
            dgvUsers = new DataGridView
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
            _StyleGrid(dgvUsers);

            dgvUsers.CellDoubleClick          += dgvUsers_CellDoubleClick;
            dgvUsers.MouseDown                += dgvUsers_MouseDown;
            dgvUsers.SelectionChanged         += dgvUsers_SelectionChanged;


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
            this.Controls.AddRange(new Control[] {
                pbHeader, lblTitle,
                lblFilterBy, cbFilterBy, txtFilterValue,
                btnAddNew,
                dgvUsers,
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
        private void _LoadUsers()
        {
            _fullTable = clsUser.GetAllUsers();
            _BindGrid(_fullTable);
        }

        private void _BindGrid(DataTable dt)
        {
            dgvUsers.DataSource = dt;
            lblRecordCount.Text  = $"Records: {dt.Rows.Count}";

            // Friendly column headers (in case the SP uses raw column names)
            _RenameColumnIfExists("UserID", "User ID");
            _RenameColumnIfExists("PersonID", "Person ID");
            _RenameColumnIfExists("Name", "Full Name");
            _RenameColumnIfExists("UserName", "UserName");
            _RenameColumnIfExists("IsActive", "is Active");

            // Hide internal columns the user doesn't need to see
            _HideColumnIfExists("Password");
        }

        private void _RenameColumnIfExists(string dataName, string displayName)
        {
            if (dgvUsers.Columns.Contains(dataName))
                dgvUsers.Columns[dataName].HeaderText = displayName;
        }

        private void _HideColumnIfExists(string name)
        {
            if (dgvUsers.Columns.Contains(name))
                dgvUsers.Columns[name].Visible = false;
        }


        // ── Filter logic ─────────────────────────────────────────────────────

        private void _ApplyFilter()
        {
            if(_fullTable == null) return;
            string col = cbFilterBy.Text;
            string Value = txtFilterValue.Text.Trim();

            if (col == "None" || string.IsNullOrEmpty(Value))
            {
                _BindGrid(_fullTable);
                return;
            }

            // Map display name → DataTable column name
            string dbCol = col switch
            {
                "User ID"       => "UserID",
                "Person ID"     => "PersonID",
                "Name"          => "Name",
                "UserName"      => "UserName",
                "is Active"     => "IsActive",
                _               => null        
            };

            if (dbCol == null) return;

            try
            {
                DataView dv   = new DataView(_fullTable);
                dv.RowFilter  = $"CONVERT([{dbCol}], System.String) LIKE '%{Value}%'";
                _BindGrid(dv.ToTable());
            }
            catch { /* ignore invalid filter expressions while user is still typing */ }
            
        }

        // ── Row helpers ──────────────────────────────────────────────────────

        private int _SelectedUserID()
        {
            if (dgvUsers.SelectedRows.Count == 0) return -1;

            var cell = dgvUsers.SelectedRows[0].Cells["UserID"];
            if (cell?.Value == null) return -1;

            return Convert.ToInt32(cell.Value);
        }

        private string _SelectedEmail()
        {
            if (dgvUsers.SelectedRows.Count == 0) return string.Empty;
            var cell = dgvUsers.SelectedRows[0].Cells["Email"];
            return cell?.Value?.ToString() ?? string.Empty;
        }

        private string _SelectedPhone()
        {
            if (dgvUsers.SelectedRows.Count == 0) return string.Empty;
            var cell = dgvUsers.SelectedRows[0].Cells["Phone"];
            return cell?.Value?.ToString() ?? string.Empty;
        }

        // ── Actions ──────────────────────────────────────────────────────────

        private void _ShowDetails()
        {
            int id = _SelectedUserID();
            if(id == -1) return;

            new frmShowUserInfo(id).ShowDialog();
        }

        private void _OpenAddNew()
        {
            new frmAddEditUser().ShowDialog();
            _LoadUsers();     // refresh after dialog closes
        }

        private void _OpenEdit()
        {
            int id = _SelectedUserID();
            if (id == -1) return;

            new frmAddEditUser(id).ShowDialog();
            _LoadUsers();
        }

        private void _DeleteSelected()
        {
            int id = _SelectedUserID();
            if (id == -1) return;

            if (!clsUtil.ConfirmDelete("this user")) return;

            if (clsUser.Delete(id))
            {
                clsUtil.ShowInfo("user deleted successfully.", "Deleted");
                _LoadUsers();
            }
            else
            {
                clsUtil.ShowError("Could not delete this user.\n" +
                                  "They may be linked to existing records.");
            }
        }

        private void _SendEmail()
        {
            string email = _SelectedEmail();
            if (string.IsNullOrWhiteSpace(email))
            { clsUtil.ShowWarning("No email address on record for this user."); return; }
            clsUtil.SendEmail(email);
        }

        private void _PhoneCall()
        {
            string phone = _SelectedPhone();
            if (string.IsNullOrWhiteSpace(phone))
            { clsUtil.ShowWarning("No phone number on record for this user."); return; }
            clsUtil.MakePhoneCall(phone);
        }

        // ── Events ───────────────────────────────────────────────────────────
        
        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasFilter = cbFilterBy.SelectedIndex > 0;
            txtFilterValue.Visible = hasFilter;
            txtFilterValue.Text = string.Empty;

            if (!hasFilter)
                _BindGrid(_fullTable);   // restore full list immediately
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
            => _ApplyFilter();

        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            _ShowDetails();
        }

        private void dgvUsers_MouseDown(object sender, MouseEventArgs e)
        {
            // Right-click: select the row under the cursor before showing the menu
            if (e.Button == MouseButtons.Right)
            {
                var hit = dgvUsers.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                    dgvUsers.Rows[hit.RowIndex].Selected = true;
            }
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            // Enable / disable row-specific menu items
            bool hasSelection      = dgvUsers.SelectedRows.Count > 0;
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