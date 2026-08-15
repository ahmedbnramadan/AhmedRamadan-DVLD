using System;
using System.Drawing;
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

        private Label      lblTitle;
        private Panel      pnlCard;        // white card that holds all info rows

        // Info labels (title + value pairs)
        private Label lblPersonIDTitle,   lblPersonID;
        private Label lblFullNameTitle,   lblFullName;
        private Label lblNationalNoTitle, lblNationalNo;
        private Label lblGenderTitle,     lblGender;
        private Label lblDOBTitle,        lblDOB;
        private Label lblPhoneTitle,      lblPhone;
        private Label lblEmailTitle,      lblEmail;
        private Label lblCountryTitle,    lblCountry;
        private Label lblAddressTitle,    lblAddress;

        private PictureBox pbPersonImage;

        // Action links
        private LinkLabel llEdit;
        private LinkLabel llSendEmail;
        private LinkLabel llPhoneCall;

        private Button btnClose;

        #endregion

        #region State

        private readonly int _personID;
        private clsPerson    _person;

        #endregion

        // ── Constructor ─────────────────────────────────────────────────────

        public frmShowPersonInfo(int personID)
        {
            _personID = personID;
            _InitializeComponents();
            _LoadPerson();
        }

        // ── Form Build ──────────────────────────────────────────────────────

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text            = "Person Details";
            this.Size            = new Size(860, 600);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.FromArgb(240, 242, 248);
            this.Font            = new Font("Microsoft Sans Serif", 9.5F);

            // ── Page title ───────────────────────────────────────────
            lblTitle = new Label
            {
                Text      = "Person Information",
                Font      = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize  = true,
                Location  = new Point(270, 20)
            };

            // ── Photo ────────────────────────────────────────────────
            pbPersonImage = new PictureBox
            {
                Location    = new Point(650, 65),
                Size        = new Size(165, 190),
                SizeMode    = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor   = Color.FromArgb(235, 237, 244)
            };

            // ── White card panel ─────────────────────────────────────
            pnlCard = new Panel
            {
                Location  = new Point(30, 65),
                Size      = new Size(600, 455),
                BackColor = Color.White
            };
            pnlCard.Paint += (s, e) =>
            {
                // Subtle border
                using (var pen = new Pen(Color.FromArgb(210, 215, 225)))
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            };

            // ── Info rows (inside pnlCard) ───────────────────────────
            int rowY  = 25;
            const int rowStep = 47;
            const int titleX  = 20;
            const int valueX  = 160;

            _MakeInfoRow(pnlCard, "Person ID:",   titleX, valueX, rowY, out lblPersonIDTitle,   out lblPersonID,   Color.SteelBlue);
            rowY += rowStep;
            _MakeInfoRow(pnlCard, "Full Name:",   titleX, valueX, rowY, out lblFullNameTitle,   out lblFullName,   Color.FromArgb(30, 80, 160));
            rowY += rowStep;
            _MakeInfoRow(pnlCard, "National No:", titleX, valueX, rowY, out lblNationalNoTitle, out lblNationalNo, Color.Black);
            rowY += rowStep;
            _MakeInfoRow(pnlCard, "Gender:",      titleX, valueX, rowY, out lblGenderTitle,     out lblGender,     Color.Black);
            rowY += rowStep;
            _MakeInfoRow(pnlCard, "Date of Birth:", titleX, valueX, rowY, out lblDOBTitle,      out lblDOB,        Color.Black);
            rowY += rowStep;
            _MakeInfoRow(pnlCard, "Phone:",       titleX, valueX, rowY, out lblPhoneTitle,      out lblPhone,      Color.Black);
            rowY += rowStep;
            _MakeInfoRow(pnlCard, "Email:",       titleX, valueX, rowY, out lblEmailTitle,      out lblEmail,      Color.FromArgb(0, 102, 204));
            rowY += rowStep;
            _MakeInfoRow(pnlCard, "Country:",     titleX, valueX, rowY, out lblCountryTitle,    out lblCountry,    Color.Black);
            rowY += rowStep;
            _MakeInfoRow(pnlCard, "Address:",     titleX, valueX, rowY, out lblAddressTitle,    out lblAddress,    Color.DimGray);

            // ── Action links (below photo) ────────────────────────────
            llEdit = new LinkLabel
            {
                Text      = "✏  Edit Person",
                AutoSize  = true,
                Location  = new Point(650, 265),
                Font      = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = Color.SteelBlue
            };
            llEdit.LinkClicked += llEdit_LinkClicked;

            llSendEmail = new LinkLabel
            {
                Text      = "📧  Send Email",
                AutoSize  = true,
                Location  = new Point(650, 295),
                Font      = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = Color.SteelBlue
            };
            llSendEmail.LinkClicked += (s, e) => clsUtil.SendEmail(_person?.Email);

            llPhoneCall = new LinkLabel
            {
                Text      = "📞  Phone Call",
                AutoSize  = true,
                Location  = new Point(650, 325),
                Font      = new Font("Microsoft Sans Serif", 9.5F),
                LinkColor = Color.SteelBlue
            };
            llPhoneCall.LinkClicked += (s, e) => clsUtil.MakePhoneCall(_person?.Phone);

            // ── Close button ─────────────────────────────────────────
            btnClose = new Button
            {
                Text      = "✖  Close",
                Location  = new Point(650, 490),
                Size      = new Size(165, 38),
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(192, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            // ── Add to form ───────────────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                pbPersonImage,
                pnlCard,
                llEdit, llSendEmail, llPhoneCall,
                btnClose
            });
        }

        // ── Factory: one info row ────────────────────────────────────────────

        private static void _MakeInfoRow(
            Panel parent,
            string titleText,
            int titleX, int valueX, int y,
            out Label titleLabel, out Label valueLabel,
            Color valueColor)
        {
            titleLabel = new Label
            {
                Text      = titleText,
                Location  = new Point(titleX, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 90)
            };

            valueLabel = new Label
            {
                Text      = "—",
                Location  = new Point(valueX, y),
                AutoSize  = true,
                Font      = new Font("Microsoft Sans Serif", 9.5F),
                ForeColor = valueColor
            };

            // Thin separator line below each row
            var sep = new Panel
            {
                Location  = new Point(titleX, y + 22),
                Size      = new Size(560, 1),
                BackColor = Color.FromArgb(230, 232, 240)
            };

            parent.Controls.AddRange(new Control[] { titleLabel, valueLabel, sep });
        }

        // ── Data ─────────────────────────────────────────────────────────────

        private void _LoadPerson()
        {
            _person = clsPerson.Find(_personID);

            if (_person == null)
            {
                clsUtil.ShowError($"No person found with ID = {_personID}.");
                this.Close();
                return;
            }

            lblPersonID.Text  = _person.ID.ToString();
            lblFullName.Text  = _person.FullName;
            lblNationalNo.Text = _person.NationalNo;
            lblGender.Text    = clsFormat.Gender(_person.Gender);
            lblDOB.Text       = clsFormat.DateShort(_person.DateOfBirth);
            lblPhone.Text     = string.IsNullOrWhiteSpace(_person.Phone) ? "—" : _person.Phone;
            lblEmail.Text     = string.IsNullOrWhiteSpace(_person.Email) ? "—" : _person.Email;
            lblCountry.Text   = _person.CountryName;
            lblAddress.Text   = string.IsNullOrWhiteSpace(_person.Address) ? "—" : _person.Address;

            clsUtil.LoadPersonImage(pbPersonImage, _person.ImagePath);

            // Hide action links if data is missing
            llSendEmail.Visible = !string.IsNullOrWhiteSpace(_person.Email);
            llPhoneCall.Visible = !string.IsNullOrWhiteSpace(_person.Phone);
        }

        // ── Events ───────────────────────────────────────────────────────────

        private void llEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var frm = new frmAddEditPerson(_personID);
            frm.DataBack += (s, personID) => _LoadPerson();
            frm.ShowDialog();
        }
    }
}