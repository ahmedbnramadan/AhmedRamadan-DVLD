using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    /// <summary>
    /// Read-only history of every license (past and present) a driver
    /// has held. Backed by clsLicense.GetDriverLicenses(DriverID).
    /// </summary>
    public class frmShowPersonLicenseHistory : Form
    {
        #region Controls

        private Label lblTitle;
        private DataGridView dgv;
        private Label lblCount;
        private Button btnClose;

        #endregion

        #region Column Setup

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

        // Field names must match the aliases/columns returned by
        // clsLicense.GetDriverLicenses() -> clsLicenses.GetLicensesByDriverID().
        private static readonly ColumnSetup[] ColumnLayout =
        {
            new ColumnSetup("licenseid", "License ID",
                DataGridViewContentAlignment.MiddleCenter, width: 90),

            new ColumnSetup("classname", "Class",
                DataGridViewContentAlignment.MiddleLeft, width: 0, fill: true),

            new ColumnSetup("issuedate", "Issue Date",
                DataGridViewContentAlignment.MiddleCenter, width: 110, format: "d"),

            new ColumnSetup("expirationdate", "Expiration",
                DataGridViewContentAlignment.MiddleCenter, width: 110, format: "d"),

            new ColumnSetup("issuereason", "Reason",
                DataGridViewContentAlignment.MiddleCenter, width: 140),

            new ColumnSetup("paidfees", "Fees",
                DataGridViewContentAlignment.MiddleRight, width: 90, format: "N2"),

            new ColumnSetup("isactive", "Active",
                DataGridViewContentAlignment.MiddleCenter, width: 70)
        };

        #endregion

        private readonly int _driverID;

        public frmShowPersonLicenseHistory(int driverID)
        {
            _driverID = driverID;
            _Build();
            _LoadData();
        }

        #region Build

        private void _Build()
        {
            this.Text = "Driving License History";
            this.Size = new Size(900, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label
            {
                Text = "Driving License History",
                Font = new Font("Arial", 16F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };

            dgv = new DataGridView
            {
                Location = new Point(20, 65),
                Size = new Size(840, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                         AnchorStyles.Left | AnchorStyles.Right,

                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,

                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,

                ColumnHeadersHeight = 34,
                RowTemplate = new DataGridViewRow { Height = 28 },

                BorderStyle = BorderStyle.FixedSingle,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false
            };

            dgv.ColumnHeadersDefaultCellStyle.BackColor = clsGlobal.GridHeaderBack;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = clsGlobal.GridHeaderFore;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
            dgv.CellFormatting += Dgv_CellFormatting;

            lblCount = new Label
            {
                Text = "Records: 0",
                AutoSize = true,
                Location = new Point(20, ClientSize.Height - 43),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
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
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Location = new Point(
                ClientSize.Width - btnClose.Width - 20,
                ClientSize.Height - btnClose.Height - 18);
            btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { lblTitle, dgv, lblCount, btnClose });
        }

        #endregion

        #region Load

        private void _LoadData()
        {
            if (_driverID <= 0)
            {
                clsUtil.ShowError("Invalid driver.");
                Close();
                return;
            }

            DataTable dt = clsLicense.GetDriverLicenses(_driverID);

            if (dt == null)
            {
                dt = new DataTable();
            }

            dgv.DataSource = dt;
            lblCount.Text = "Records: " + dt.Rows.Count;

            _ConfigureColumns();
        }

        private void _ConfigureColumns()
        {
            for (int i = 0; i < ColumnLayout.Length; i++)
                _ApplyColumn(ColumnLayout[i], i);

            // Everything else the query brought back (applicationid,
            // driverid, notes, createdbyuserid, classfees, activecount)
            // is internal plumbing, not history the user asked to see.
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                bool isDisplayed = Array.Exists(ColumnLayout, c => c.DataField == col.Name);
                if (!isDisplayed)
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
            column.DefaultCellStyle.Alignment = setup.Alignment;

            if (setup.Fill)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                column.Width = setup.Width;
            }

            if (!string.IsNullOrEmpty(setup.Format))
                column.DefaultCellStyle.Format = setup.Format;
        }

        #endregion

        #region Formatting

        // issuereason and isactive come back from the database as raw
        // numbers/bits. CellFormatting lets us show human-readable text
        // without mutating the underlying DataTable.
        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0) return;

            string columnName = dgv.Columns[e.ColumnIndex].Name;

            if (columnName == "issuereason")
            {
                short reason = Convert.ToInt16(e.Value);
                e.Value = _IssueReasonText(reason);
                e.FormattingApplied = true;
            }
            else if (columnName == "isactive")
            {
                bool isActive = Convert.ToBoolean(e.Value);
                e.Value = isActive ? "Yes" : "No";
                e.FormattingApplied = true;
            }
        }

        private static string _IssueReasonText(short reason)
        {
            switch ((clsLicense.enIssueReason)reason)
            {
                case clsLicense.enIssueReason.FirstTime: return "First Time";
                case clsLicense.enIssueReason.Renew: return "Renew";
                case clsLicense.enIssueReason.DamagedReplacement: return "Damaged Replacement";
                case clsLicense.enIssueReason.LostReplacement: return "Lost Replacement";
                default: return "Unknown";
            }
        }

        #endregion
    }
}