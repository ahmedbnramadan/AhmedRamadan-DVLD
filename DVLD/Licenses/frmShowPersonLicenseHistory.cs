using System;
using System.Drawing;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    /// <summary>
    /// Shows a person's full driving-license history (Local + International).
    /// Purely a composition of two already-existing, independently reusable
    /// controls - no license/person logic is duplicated here:
    ///
    ///   - ctrlPersonCardWithFilter : finds/displays a person.
    ///   - ctrlDriverLicenses       : shows that person's licenses.
    ///
    /// Two ways to open it:
    ///
    ///   new frmShowPersonLicenseHistory().ShowDialog();
    ///       "Search mode" - filter is visible and focused; the caller
    ///       doesn't know which person yet, the user looks one up.
    ///
    ///   new frmShowPersonLicenseHistory(personID).ShowDialog();
    ///       "Locked mode" - filter is hidden and the given person is
    ///       loaded immediately. Use this whenever the caller already
    ///       knows exactly who they want (e.g. a selected grid row), so
    ///       the user can't switch to someone else's records mid-view.
    /// </summary>
    public class frmShowPersonLicenseHistory : Form
    {
        #region Controls

        private Label lblTitle;
        private ctrlPersonCardWithFilter ctrlPersonCardWithFilter1;
        private ctrlDriverLicenses ctrlDriverLicenses1;
        private Button btnClose;

        #endregion

        #region State

        // Any value <= 0 means "no specific person - let the user search".
        // Matches the -1 sentinel convention used everywhere else in DVLD
        // (see clsPerson(), clsApplication(), etc).
        private readonly int _personID;

        #endregion

        // ── Constructors ────────────────────────────────────────────────────

        /// <summary>Opens in search mode.</summary>
        public frmShowPersonLicenseHistory() : this(-1) { }

        /// <summary>
        /// Opens locked to a specific person.
        /// </summary>
        /// <param name="personID">
        /// The Person ID whose license history to display. Any value
        /// &lt;= 0 falls back to search mode instead of failing.
        /// </param>
        public frmShowPersonLicenseHistory(int personID)
        {
            _personID = personID;

            _InitializeComponents();
            _SetupEvents();
        }

        // ── Build ────────────────────────────────────────────────────────────

        private void _InitializeComponents()
        {
            this.Text = "License History";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            lblTitle = new Label
            {
                Text = "License History",
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(330, 15)
            };

            // Keeps its own default size (850x400) - set that in one place
            // only, inside the control itself, so we never fight it here.
            ctrlPersonCardWithFilter1 = new ctrlPersonCardWithFilter
            {
                Location = new Point(30, 65)
            };
            ctrlPersonCardWithFilter1.PersonLoaded += CtrlPersonCardWithFilter1_PersonLoaded;

            // Keeps its own default size (850x270).
            ctrlDriverLicenses1 = new ctrlDriverLicenses
            {
                Location = new Point(30, ctrlPersonCardWithFilter1.Bottom + 15)
            };

            btnClose = new Button
            {
                Text = "✖  Close",
                Size = new Size(150, 38),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = clsGlobal.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Location = new Point(
                ctrlDriverLicenses1.Right - btnClose.Width,
                ctrlDriverLicenses1.Bottom + 20);
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                ctrlPersonCardWithFilter1,
                ctrlDriverLicenses1,
                btnClose
            });

            // Size the form to what we actually built rather than guessing
            // pixel counts up front - stays correct even if either child
            // control's own default size changes later.
            this.ClientSize = new Size(
                ctrlDriverLicenses1.Right + 30,
                btnClose.Bottom + 20);
        }

        private void _SetupEvents()
        {
            this.Load += FrmShowPersonLicenseHistory_Load;
            this.Shown += FrmShowPersonLicenseHistory_Shown;
        }

        // ── Load / Shown ─────────────────────────────────────────────────────

        // Deliberately NOT done in the constructor: the form has no window
        // handle yet at that point, so a Close() call here (e.g. "person
        // not found") would not reliably stop a subsequent ShowDialog()
        // from displaying an empty form afterwards. Same reasoning
        // frmEditTestType / frmShowPersonInfo already use elsewhere in
        // this project - keep it consistent.
        private void FrmShowPersonLicenseHistory_Load(object sender, EventArgs e)
        {
            bool locked = _personID > 0;

            ctrlPersonCardWithFilter1.FilterVisible = !locked;

            if (!locked)
            {
                ctrlDriverLicenses1.Clear();
                return;
            }

            // IMPORTANT: LoadPersonInfo() only fills the card - unlike the
            // Find button's click handler, it does NOT raise PersonLoaded.
            // If we relied on that event here, the license panel below
            // would silently stay empty for every caller that opens this
            // form pre-locked to a person - i.e. for the entire "locked
            // mode" half of this feature. So we push the ID down ourselves.
            ctrlPersonCardWithFilter1.LoadPersonInfo(_personID);

            clsPerson person = ctrlPersonCardWithFilter1.SelectedPersonInfo;

            if (person == null)
            {
                // The card control already showed its own error message
                // box for this case - nothing useful left to display.
                // Never trust a caller-supplied ID blindly; re-verify here
                // instead of assuming it was valid.
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            ctrlDriverLicenses1.LoadInfoByPersonID(person.ID);
        }

        private void FrmShowPersonLicenseHistory_Shown(object sender, EventArgs e)
        {
            // Only steal focus for the filter when there is a filter to use -
            // it's hidden entirely in locked mode. Focusing here (Shown),
            // not in Load, matches the rest of the project: the control
            // isn't reliably focusable before the form has actually painted.
            if (_personID <= 0)
            {
                ctrlPersonCardWithFilter1.FocusOnFilter();
            }
        }

        // ── Events ───────────────────────────────────────────────────────────

        private void CtrlPersonCardWithFilter1_PersonLoaded(object sender, clsPerson person)
        {
            if (person == null)
            {
                ctrlDriverLicenses1.Clear();
                return;
            }

            ctrlDriverLicenses1.LoadInfoByPersonID(person.ID);
        }
    }
}   