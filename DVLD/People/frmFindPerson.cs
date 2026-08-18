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
        private ctrlPersonCardWithFilter _personCardWithFilter = null;

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

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text      = "Find Person",
                Font      = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize  = true,
                Location  = new Point(300, 18)
            };


            // ── Use ctrlPersonCardWithFilter for search and display ─────────────────────────────────────────
            _personCardWithFilter = new ctrlPersonCardWithFilter
            {
                Location = new Point(30, 70),
                Size     = new Size(760, 400)
            };
            _personCardWithFilter.PersonLoaded += _personCardWithFilter_PersonLoaded;

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
                _personCardWithFilter,
                btnSelect, btnClose
            });
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void _personCardWithFilter_PersonLoaded(object sender, clsPerson person)
        {
            if (person != null)
            {
                _foundPerson = person;
                btnSelect.Enabled = true;
            }
            else
            {
                _foundPerson = null;
                btnSelect.Enabled = false;
            }
        }

        private void _ResetResult()
        {
            _foundPerson      = null;
            btnSelect.Enabled = false;
        }

        // ── Events ───────────────────────────────────────────────────────────

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (_foundPerson == null) return;

            SelectedPersonID  = _foundPerson.ID;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}