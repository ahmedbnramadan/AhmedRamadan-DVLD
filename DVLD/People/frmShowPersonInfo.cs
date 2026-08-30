using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    /// <summary>
    /// Read-only view of a single person's full details.
    /// Opened by double-clicking a row in frmListPeople, or via "Show Details".
    /// </summary>
    public class frmShowPersonInfo : Form
    {
        #region Controls Declaration

        private Label lblTitle;
        private ctrlPersonCard ctrlPerson;

        // Action links
        private LinkLabel llSendEmail;
        private LinkLabel llPhoneCall;

        private Button btnClose;

        #endregion

        #region State

        private readonly int _personID;
        private clsPerson _person;

        #endregion

        // ── Constructors ────────────────────────────────────────────────────

        public frmShowPersonInfo(int personID)
        {
            // Single data-access hit, resolved up front.
            _person = clsPerson.Find(personID);
            _personID = personID;

            _InitializeComponents();

            // Defer loading to the Load event: the handle exists by then,
            // so Close() on "not found" is safe (no ObjectDisposedException
            // for callers doing new frmShowPersonInfo(id).ShowDialog()).
            this.Load += delegate { _LoadPerson(); };
        }

        public frmShowPersonInfo(string nationalNo)
        {
            // Single data-access hit — no second Find() inside the load method.
            _person = clsPerson.Find(nationalNo);
            _personID = (_person != null) ? _person.ID : -1;

            _InitializeComponents();

            this.Load += delegate { _LoadPerson(); };
        }

        // ── Form Build ──────────────────────────────────────────────────────

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text = "Person Details";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 248);
            this.Font = new Font("Microsoft Sans Serif", 9.5F);

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text = "Person Information",
                Font = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            // ── Reusable Person Card Control ─────────────────────────
            ctrlPerson = new ctrlPersonCard
            {
                Location = new Point(30, 70),
                Size = new Size(830, 300)
            };

            // ── Action links (below card) ────────────────────────────
            llSendEmail = new LinkLabel
            {
                Text = "Send Email",
                AutoSize = true,
                Location = new Point(30, 390),
                Font = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = clsGlobal.LinkBlue
            };
            llSendEmail.LinkClicked += delegate
            {
                if (_person != null)
                    clsUtil.SendEmail(_person.Email);
            };

            llPhoneCall = new LinkLabel
            {
                Text = "Phone Call",
                AutoSize = true,
                Location = new Point(120, 390),
                Font = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = clsGlobal.LinkBlue
            };
            llPhoneCall.LinkClicked += delegate
            {
                if (_person != null)
                    clsUtil.MakePhoneCall(_person.Phone);
            };

            // ── Close button ─────────────────────────────────────────
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(680, 385),
                Size = new Size(165, 38),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = clsGlobal.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += delegate { this.Close(); };

            // ── Add to form ───────────────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                ctrlPerson,
                llSendEmail, llPhoneCall,
                btnClose
            });
        }

        // ── Data ─────────────────────────────────────────────────────────────

        private void _LoadPerson()
        {
            if (_person == null)
            {
                clsUtil.ShowError("No person found with ID = " + _personID + ".");
                this.Close();
                return;
            }

            // Load data into the reusable ctrlPersonCard.
            // (If you have a LoadPersonInfo(nationalNo) overload you prefer
            //  for the NationalNo path, load _person into the card directly
            //  instead — the data is already here, no need to re-query.)
            ctrlPerson.LoadPersonInfo(_personID);

            // Hide action links if data is missing.
            // Note: .NET 3.5 has no string.IsNullOrWhiteSpace — use IsNullOrEmpty.
            llSendEmail.Visible = !string.IsNullOrWhiteSpace(_person.Email);
            llPhoneCall.Visible = !string.IsNullOrWhiteSpace(_person.Phone);
        }
    }
}