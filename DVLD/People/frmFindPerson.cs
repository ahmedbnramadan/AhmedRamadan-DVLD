using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    /// <summary>
    /// A reusable search dialog that lets other forms pick a person by
    /// Person ID or National Number.
    ///
    /// Usage:
    ///   var dlg = new frmFindPerson();
    ///   if (dlg.ShowDialog() == DialogResult.OK)
    ///       int selectedID = dlg.SelectedPersonID;
    /// </summary>
    public class frmFindPerson : Form
    {
        #region Controls Declaration

        private Label      lblTitle;

        // Search bar
        private Label      lblSearchBy;
        private ComboBox   cbSearchBy;
        private TextBox    txtSearchValue;
        private Button     btnFind;

        // Result card panel
        private Panel      pnlResult;
        private Label      lblResultTitle;

        private Label      lblPersonIDTitle,   lblPersonIDValue;
        private Label      lblFullNameTitle,   lblFullNameValue;
        private Label      lblNationalNoTitle, lblNationalNoValue;
        private Label      lblGenderTitle,     lblGenderValue;
        private Label      lblDOBTitle,        lblDOBValue;
        private Label      lblPhoneTitle,      lblPhoneValue;
        private Label      lblEmailTitle,      lblEmailValue;
        private Label      lblCountryTitle,    lblCountryValue;

        private PictureBox pbPersonImage;

        // Buttons
        private Button     btnSelect;
        private Button     btnClose;

        #endregion

        #region Public Result

        /// <summary>The ID of the person the user selected. -1 if none.</summary>
        public int SelectedPersonID { get; private set; } = -1;

        #endregion

        #region State

        private clsPerson _foundPerson = null;

        #endregion

        // ── Constructor ─────────────────────────────────────────────────────

        public frmFindPerson()
        {
            _InitializeComponents();
            _ResetResult();
        }

        // ── Form Build ──────────────────────────────────────────────────────

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text            = "Find Person";
            this.Size            = new Size(830, 560);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.FromArgb(240, 242, 248);
            this.Font            = new Font("Microsoft Sans Serif", 9.5F);
            this.AcceptButton    = null;   // set after btnFind is created

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text      = "Find Person",
                Font      = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize  = true,
                Location  = new Point(300, 18)
            };

            // ── Search bar ───────────────────────────────────────────
            lblSearchBy = new Label
            {
                Text     = "Search By:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 72)
            };

            cbSearchBy = new ComboBox
            {
                Location      = new Point(115, 69),
                Size          = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor        = Cursors.Hand
            };
            cbSearchBy.Items.AddRange(new object[] { "Person ID", "National No." });
            cbSearchBy.SelectedIndex = 0;
            cbSearchBy.SelectedIndexChanged += (s, e) => txtSearchValue.Clear();

            txtSearchValue = new TextBox
            {
                Location = new Point(275, 69),
                Size     = new Size(230, 23),
                Font     = new Font("Microsoft Sans Serif", 9.5F)
            };
            txtSearchValue.KeyPress += txtSearchValue_KeyPress;
            txtSearchValue.KeyDown  += txtSearchValue_KeyDown;

            btnFind = new Button
            {
                Text      = "🔍  Find",
                Location  = new Point(515, 66),
                Size      = new Size(110, 32),
                Font      = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnFind.FlatAppearance.BorderSize = 0;
            btnFind.Click += btnFind_Click;

            this.AcceptButton = btnFind;   // Enter triggers search

            // ── Result panel ─────────────────────────────────────────
            pnlResult = new Panel
            {
                Location  = new Point(30, 115),
                Size      = new Size(760, 340),
                BackColor = Color.White
            };
            pnlResult.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(210, 215, 225)))
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlResult.Width - 1, pnlResult.Height - 1);
            };

            lblResultTitle = new Label
            {
                Text      = "Search result will appear here",
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Italic),
                ForeColor = Color.Silver,
                AutoSize  = true,
                Location  = new Point(250, 150)
            };
            pnlResult.Controls.Add(lblResultTitle);

            // ── Info rows inside pnlResult ────────────────────────────
            int y = 25; const int step = 40;
            const int tx = 20, vx = 180;

            _MakeRow(pnlResult, "Person ID:",    tx, vx, y, out lblPersonIDTitle,   out lblPersonIDValue,   Color.SteelBlue);      y += step;
            _MakeRow(pnlResult, "Full Name:",    tx, vx, y, out lblFullNameTitle,   out lblFullNameValue,   Color.FromArgb(30,80,160)); y += step;
            _MakeRow(pnlResult, "National No.:", tx, vx, y, out lblNationalNoTitle, out lblNationalNoValue, Color.Black);          y += step;
            _MakeRow(pnlResult, "Gender:",       tx, vx, y, out lblGenderTitle,     out lblGenderValue,     Color.Black);          y += step;
            _MakeRow(pnlResult, "Date of Birth:", tx, vx, y, out lblDOBTitle,       out lblDOBValue,        Color.Black);          y += step;
            _MakeRow(pnlResult, "Phone:",        tx, vx, y, out lblPhoneTitle,      out lblPhoneValue,      Color.Black);          y += step;
            _MakeRow(pnlResult, "Email:",        tx, vx, y, out lblEmailTitle,      out lblEmailValue,      Color.FromArgb(0,102,204)); y += step;
            _MakeRow(pnlResult, "Country:",      tx, vx, y, out lblCountryTitle,    out lblCountryValue,    Color.Black);

            // Hide info rows until a person is found
            _SetInfoRowsVisible(false);

            // ── Photo (inside pnlResult) ─────────────────────────────
            pbPersonImage = new PictureBox
            {
                Location    = new Point(570, 20),
                Size        = new Size(170, 190),
                SizeMode    = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor   = Color.FromArgb(235, 237, 244)
            };
            pnlResult.Controls.Add(pbPersonImage);

            // ── Buttons ───────────────────────────────────────────────
            btnSelect = new Button
            {
                Text      = "✔  Select This Person",
                Location  = new Point(480, 475),
                Size      = new Size(185, 38),
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 140, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Enabled   = false          // enabled only when a person is found
            };
            btnSelect.FlatAppearance.BorderSize = 0;
            btnSelect.Click += btnSelect_Click;

            btnClose = new Button
            {
                Text      = "✖  Close",
                Location  = new Point(678, 475),
                Size      = new Size(112, 38),
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(192, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            // ── Add to form ───────────────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblSearchBy, cbSearchBy, txtSearchValue, btnFind,
                pnlResult,
                btnSelect, btnClose
            });
        }

        // ── Factory: one info row ────────────────────────────────────────────

        private static void _MakeRow(
            Panel parent,
            string titleText, int tx, int vx, int y,
            out Label titleLbl, out Label valueLbl,
            Color valueColor)
        {
            titleLbl = new Label
            {
                Text      = titleText,
                Location  = new Point(tx, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 90),
                Visible   = false
            };
            valueLbl = new Label
            {
                Text      = "—",
                Location  = new Point(vx, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 9.5F),
                ForeColor = valueColor,
                Visible   = false
            };
            var sep = new Panel
            {
                Location  = new Point(tx, y + 20),
                Size      = new Size(540, 1),
                BackColor = Color.FromArgb(230, 232, 240),
                Visible   = false,
                Tag       = "sep"
            };
            parent.Controls.AddRange(new Control[] { titleLbl, valueLbl, sep });
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void _SetInfoRowsVisible(bool visible)
        {
            foreach (Control c in pnlResult.Controls)
                if (c != lblResultTitle && c != pbPersonImage)
                    c.Visible = visible;

            pbPersonImage.Visible  = visible;
            lblResultTitle.Visible = !visible;
        }

        private void _ResetResult()
        {
            _foundPerson      = null;
            btnSelect.Enabled = false;
            _SetInfoRowsVisible(false);
            pbPersonImage.Image = null;
        }

        private void _DisplayPerson(clsPerson p)
        {
            if (p == null) { _ResetResult(); return; }

            _foundPerson = p;

            lblPersonIDValue.Text   = p.ID.ToString();
            lblFullNameValue.Text   = p.FullName;
            lblNationalNoValue.Text = p.NationalNo;
            lblGenderValue.Text     = clsFormat.Gender(p.Gender);
            lblDOBValue.Text        = clsFormat.DateLong(p.DateOfBirth);
            lblPhoneValue.Text      = string.IsNullOrWhiteSpace(p.Phone) ? "—" : p.Phone;
            lblEmailValue.Text      = string.IsNullOrWhiteSpace(p.Email) ? "—" : p.Email;
            lblCountryValue.Text    = p.CountryName;

            clsUtil.LoadPersonImage(pbPersonImage, p.ImagePath);

            _SetInfoRowsVisible(true);
            btnSelect.Enabled = true;
        }

        // ── Events ───────────────────────────────────────────────────────────

        private void txtSearchValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Person ID field: digits only
            if (cbSearchBy.SelectedIndex == 0 &&
                !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtSearchValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnFind.PerformClick();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string value = txtSearchValue.Text.Trim();

            if (string.IsNullOrEmpty(value))
            {
                clsUtil.ShowWarning("Please enter a search value.");
                txtSearchValue.Focus();
                return;
            }

            clsPerson found = null;

            if (cbSearchBy.SelectedIndex == 0)          // Person ID
            {
                if (!int.TryParse(value, out int id))
                {
                    clsUtil.ShowWarning("Person ID must be a valid number.");
                    return;
                }
                found = clsPerson.Find(id);
            }
            else                                        // National No.
            {
                found = clsPerson.Find(value);
            }

            if (found == null)
                clsUtil.ShowWarning($"No person found matching the given value.", "Not Found");

            _DisplayPerson(found);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (_foundPerson == null) return;

            SelectedPersonID  = _foundPerson.ID;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}