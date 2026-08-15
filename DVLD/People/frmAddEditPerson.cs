using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmAddEditPerson : Form
    {
        #region Controls Declaration

        private Label       lblTitle;
        private Label       lblPersonIDTitle;
        private Label       lblPersonID;

        // Name column headers
        private Label       lblFirstHeader, lblSecondHeader, lblThirdHeader, lblLastHeader;

        // Name row
        private Label       lblName;
        private TextBox     txtFirstName, txtSecondName, txtThirdName, txtLastName;

        // National No + DOB
        private Label       lblNationalNo;
        private TextBox     txtNationalNo;
        private Label       lblNationalNoError;     // inline duplicate warning
        private Label       lblDateOfBirth;
        private DateTimePicker dtpDateOfBirth;

        // Gender + Phone
        private Label       lblGender;
        private RadioButton rbMale, rbFemale;
        private Label       lblPhone;
        private TextBox     txtPhone;

        // Email + Country
        private Label       lblEmail;
        private TextBox     txtEmail;
        private Label       lblCountry;
        private ComboBox    cbCountry;

        // Address
        private Label       lblAddress;
        private TextBox     txtAddress;

        // Photo
        private PictureBox  pbPersonImage;
        private LinkLabel   llSetImage;
        private LinkLabel   llRemoveImage;

        // Buttons
        private Button      btnSave;
        private Button      btnClose;

        #endregion

        #region State

        private int       _personID  = -1;
        private clsPerson _person    = null;
        private string    _imagePath = string.Empty;

        private string    _maleDefaultImage;
        private string    _femaleDefaultImage;

        #endregion

        // ── Events ────────────────────────────────────────────────────────

        public event Action<object, int> DataBack;

        // Constructors ───────────────────────────────────────────────

        /// <summary>Opens the form in Add-New mode.</summary>
        public frmAddEditPerson()
        {
            _personID = -1;
            _InitializeComponents();
            _InitializeDefaultImages();
            _LoadCountries();
            _SetModeAddNew();
        }

        /// <summary>Opens the form in Update mode for the given person.</summary>
        public frmAddEditPerson(int personID)
        {
            _personID = personID;
            _InitializeComponents();
            _InitializeDefaultImages();
            _LoadCountries();
            _LoadPersonData();
            _SetModeUpdate();
        }

        // ── Form Build ──────────────────────────────────────────────────────

        private void _InitializeComponents()
        {
            // ── Form ────────────────────────────────────────────────
            this.Text            = "Add / Edit Person Info.";
            this.Size            = new Size(1010, 570);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Font            = new Font("Microsoft Sans Serif", 9.5F);
            this.BackColor       = Color.White;

            // ── Page title ──────────────────────────────────────────
            lblTitle = new Label
            {
                Text      = "Add New Person",
                Font      = new Font("Arial", 20F, FontStyle.Bold),
                ForeColor = clsGlobal.PrimaryRed,
                AutoSize  = true,
                Location  = new Point(330, 18)
            };

            // ── Person ID ───────────────────────────────────────────
            lblPersonIDTitle = new Label
            {
                Text     = "Person ID :",
                Font     = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 22)
            };
            lblPersonID = new Label
            {
                Text      = "N/A",
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                ForeColor = Color.SteelBlue,
                AutoSize  = true,
                Location  = new Point(160, 22)
            };

            // ── Name column headers ─────────────────────────────────
            lblFirstHeader  = _MakeHeaderLabel("First",  175);
            lblSecondHeader = _MakeHeaderLabel("Second", 325);
            lblThirdHeader  = _MakeHeaderLabel("Third",  475);
            lblLastHeader   = _MakeHeaderLabel("Last",   625);

            // ── Name TextBoxes ──────────────────────────────────────
            lblName = new Label
            {
                Text     = "Name:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 102)
            };
            txtFirstName  = _MakeTextBox(160, 99, 130);
            txtSecondName = _MakeTextBox(310, 99, 130);
            txtThirdName  = _MakeTextBox(460, 99, 130);
            txtLastName   = _MakeTextBox(610, 99, 130);

            // ── National No ─────────────────────────────────────────
            lblNationalNo = new Label
            {
                Text     = "National No:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 142)
            };
            txtNationalNo          = _MakeTextBox(160, 139, 130);
            txtNationalNo.Leave   += txtNationalNo_Leave;

            lblNationalNoError = new Label
            {
                Text      = "⚠ National Number is used for another person!",
                ForeColor = Color.Crimson,
                Font      = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(160, 165),
                Visible   = false
            };

            // ── Date of Birth ───────────────────────────────────────
            lblDateOfBirth = new Label
            {
                Text     = "Date Of Birth:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(390, 142)
            };
            dtpDateOfBirth = new DateTimePicker
            {
                Location = new Point(505, 139),
                Size     = new Size(210, 23),
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Now.AddYears(-20)
            };

            // ── Gender ──────────────────────────────────────────────
            lblGender = new Label
            {
                Text     = "Gender:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 185)
            };
            rbMale = new RadioButton
            {
                Text     = "Male",
                Location = new Point(160, 183),
                AutoSize = true,
                Checked  = true,
                Cursor   = Cursors.Hand
            };
            rbFemale = new RadioButton
            {
                Text     = "Female",
                Location = new Point(255, 183),
                AutoSize = true,
                Cursor   = Cursors.Hand
            };

            rbMale.CheckedChanged += rbGender_CheckedChanged;
            rbFemale.CheckedChanged += rbGender_CheckedChanged;

            // ── Phone ───────────────────────────────────────────────
            lblPhone = new Label
            {
                Text     = "Phone:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(390, 185)
            };
            txtPhone = _MakeTextBox(505, 182, 210);

            // ── Email ───────────────────────────────────────────────
            lblEmail = new Label
            {
                Text     = "Email:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 228)
            };
            txtEmail = _MakeTextBox(160, 225, 200);

            // ── Country ─────────────────────────────────────────────
            lblCountry = new Label
            {
                Text     = "Country:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(390, 228)
            };
            cbCountry = new ComboBox
            {
                Location      = new Point(505, 225),
                Size          = new Size(210, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor        = Cursors.Hand
            };

            // ── Address ─────────────────────────────────────────────
            lblAddress = new Label
            {
                Text     = "Address:",
                Font     = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 271)
            };
            txtAddress = new TextBox
            {
                Location    = new Point(160, 268),
                Size        = new Size(555, 90),
                Multiline   = true,
                ScrollBars  = ScrollBars.Vertical,
                Font        = new Font("Microsoft Sans Serif", 9.5F)
            };

            // ── Photo panel ─────────────────────────────────────────
            pbPersonImage = new PictureBox
            {
                Location    = new Point(762, 90),
                Size        = new Size(185, 185),
                SizeMode    = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor   = Color.FromArgb(240, 242, 246),
                Cursor      = Cursors.Hand
            };
            pbPersonImage.Click += (s, e) => _PickImage();

            llSetImage = new LinkLabel
            {
                Text      = "Set Image",
                AutoSize  = true,
                Location  = new Point(775, 282),
                LinkColor = Color.SteelBlue,
                Font      = new Font("Microsoft Sans Serif", 9.5F)
            };
            llSetImage.LinkClicked += (s, e) => _PickImage();

            llRemoveImage = new LinkLabel
            {
                Text      = "Remove",
                AutoSize  = true,
                Location  = new Point(850, 282),
                LinkColor = Color.Crimson,
                Font      = new Font("Microsoft Sans Serif", 9.5F),
                Visible   = false
            };
            llRemoveImage.LinkClicked += llRemoveImage_LinkClicked;

            // ── Buttons ─────────────────────────────────────────────
            btnSave = _MakeButton("💾  Save",  560, 460, 180, Color.FromArgb(0, 120, 215));
            btnSave.Click += btnSave_Click;

            btnClose = _MakeButton("✖  Close", 755, 460, 180, Color.FromArgb(192, 50, 50));
            btnClose.Click += (s, e) => this.Close();


            this.AcceptButton = btnSave;
            this.CancelButton = btnClose;

            // ── Add all controls ────────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblPersonIDTitle,   lblPersonID,
                lblFirstHeader,     lblSecondHeader,  lblThirdHeader,   lblLastHeader,
                lblName,            txtFirstName,     txtSecondName,    txtThirdName,   txtLastName,
                lblNationalNo,      txtNationalNo,    lblNationalNoError,
                lblDateOfBirth,     dtpDateOfBirth,
                lblGender,          rbMale,           rbFemale,
                lblPhone,           txtPhone,
                lblEmail,           txtEmail,
                lblCountry,         cbCountry,
                lblAddress,         txtAddress,
                pbPersonImage,      llSetImage,       llRemoveImage,
                btnSave,            btnClose
            });
        }

        // ── Factory helpers ─────────────────────────────────────────────────

        private static Label _MakeHeaderLabel(string text, int x)
            => new Label
            {
                Text      = text,
                AutoSize  = true,
                Location  = new Point(x, 78),
                ForeColor = Color.Gray,
                Font      = new Font("Microsoft Sans Serif", 8.5F)
            };

        private static TextBox _MakeTextBox(int x, int y, int width)
            => new TextBox
            {
                Location = new Point(x, y),
                Size     = new Size(width, 23),
                Font     = new Font("Microsoft Sans Serif", 9.5F)
            };

        private static Button _MakeButton(string text, int x, int y, int width, Color back)
        {
            var btn = new Button
            {
                Text      = text,
                Location  = new Point(x, y),
                Size      = new Size(width, 40),
                Font      = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ── Mode helpers ────────────────────────────────────────────────────

        private void _SetModeAddNew()
        {
            lblTitle.Text    = "Add New Person";
            lblPersonID.Text = "N/A";

            _UpdateDefaultPersonImage();
        }

        private void _SetModeUpdate()
        {
            lblTitle.Text    = "Update Person";
            lblPersonID.Text = _personID.ToString();
        }

        // ── Data helpers ────────────────────────────────────────────────────


        private void _InitializeDefaultImages()
        {
             _maleDefaultImage = clsGlobal.DefaultMaleImageFile;
            _femaleDefaultImage = clsGlobal.DefaultFemaleImageFile;
        }

        private void _UpdateDefaultPersonImage()
        {
            if (!string.IsNullOrEmpty(_imagePath))
                return;

            int? gender = rbMale.Checked ? 0 : 1;
            string defaultImagePath = clsGlobal.GetDefaultPersonImagePath(gender);

            Image image = clsUtil.LoadImage(defaultImagePath);

            if (image != null)
                pbPersonImage.Image = image;
        }

        private void _LoadCountries()
        {
            cbCountry.DataSource    = clsCountry.GetAllCountries();
            cbCountry.DisplayMember = "CountryName";
            cbCountry.ValueMember   = "CountryID";
            cbCountry.SelectedIndex = -1;

            int defaultCountryID = -1;

            // Priority 1: User's last selected country (if in Add mode)
            if (_personID == -1 && clsGlobal.LastSelectedCountryID != -1)
            {
                defaultCountryID = clsGlobal.LastSelectedCountryID;
            }
            // Priority 2: Auto-detect from system locale
            else
            {
                defaultCountryID = clsCountry.GetDefaultCountryIDBySystemLocale();
            }

            // Apply the default country if found
            if (defaultCountryID != -1)
            {
                cbCountry.SelectedValue = defaultCountryID;
            }
            
        }

        private void _LoadPersonData()
        {
            _person = clsPerson.Find(_personID);
            if (_person == null) return;

            txtFirstName.Text   = _person.FirstName;
            txtSecondName.Text  = _person.SecondName;
            txtThirdName.Text   = _person.ThirdName;
            txtLastName.Text    = _person.LastName;
            txtNationalNo.Text  = _person.NationalNo;
            dtpDateOfBirth.Value= _person.DateOfBirth;
            txtPhone.Text       = _person.Phone;
            txtEmail.Text       = _person.Email;
            txtAddress.Text     = _person.Address;

            // Select correct country
            if (_person.NationalityCountryID != -1)
                cbCountry.SelectedValue = _person.NationalityCountryID;

            rbMale.Checked   = (_person.Gender == 0);
            rbFemale.Checked = (_person.Gender != 0);

            _imagePath = _person.ImagePath;

            if (!string.IsNullOrEmpty(_imagePath))
            {
                clsUtil.LoadPersonImage(pbPersonImage, _imagePath);
            }
            else
            {
                _UpdateDefaultPersonImage();
            }

            llRemoveImage.Visible = !string.IsNullOrEmpty(_imagePath);
        }

        private void _PickImage()
        {
            string path = clsUtil.PickImagePath();
            if (path == null) return;

            _imagePath            = clsUtil.CopyImageToAppFolder(path);
            llRemoveImage.Visible = true;
            clsUtil.LoadPersonImage(pbPersonImage, _imagePath);
        }

        // ── Validation ──────────────────────────────────────────────────────

        private bool _ValidateInputs()
        {
            bool ok = clsValidation.ValidatePersonForm(
                txtFirstName, txtLastName, txtNationalNo,
                txtPhone, txtEmail, cbCountry, dtpDateOfBirth);

            if (!ok)
                clsUtil.ShowWarning("Please fix the highlighted fields before saving.", "Validation Error");

            return ok;
        }

        // ── Events ──────────────────────────────────────────────────────────

        private void txtNationalNo_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNationalNo.Text)) return;

            clsPerson found       = clsPerson.Find(txtNationalNo.Text.Trim());
            bool      isDuplicate = (found != null && found.ID != _personID);

            lblNationalNoError.Visible = isDuplicate;
            txtNationalNo.BackColor    = isDuplicate
                                         ? clsGlobal.InputError
                                         : clsGlobal.InputValid;
        }

        private void llRemoveImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _imagePath             = string.Empty;
            pbPersonImage.Image    = null;
            llRemoveImage.Visible  = false;

            // Show default gender-based image when photo is removed
            _UpdateDefaultPersonImage();
        }

        private void rbGender_CheckedChanged(object sender, EventArgs e)
        {
            _UpdateDefaultPersonImage();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!_ValidateInputs()) return;

            if (lblNationalNoError.Visible)
            {
                clsUtil.ShowWarning("The National Number is already used. Please enter a unique one.");
                txtNationalNo.Focus();
                return;
            }

            // Build or reuse person object
            _person = (_personID == -1)
                      ? new clsPerson()
                      : clsPerson.Find(_personID) ?? new clsPerson();

            _person.NationalNo           = txtNationalNo.Text.Trim();
            _person.FirstName            = clsFormat.NameCase(txtFirstName.Text);
            _person.SecondName           = clsFormat.NameCase(txtSecondName.Text);
            _person.ThirdName            = clsFormat.NameCase(txtThirdName.Text);
            _person.LastName             = clsFormat.NameCase(txtLastName.Text);
            _person.DateOfBirth          = dtpDateOfBirth.Value;
            _person.Gender               = (short)(rbMale.Checked ? 0 : 1);
            _person.Phone                = clsFormat.Phone(txtPhone.Text);
            _person.Email                = clsFormat.Email(txtEmail.Text);
            _person.Address              = txtAddress.Text.Trim();
            _person.NationalityCountryID = Convert.ToInt32(cbCountry.SelectedValue);
            _person.ImagePath            = _imagePath;

            
            // Remember user's country selection for next time (only in Add mode)
            if (_personID == -1 && cbCountry.SelectedValue != null)
            {
                clsGlobal.LastSelectedCountryID = Convert.ToInt32(cbCountry.SelectedValue);
            }

            if (_person.Save())
            {
                _personID        = _person.ID;
                lblPersonID.Text = _person.ID.ToString();
                lblTitle.Text    = "Update Person";         // switch to Update mode after first save
                clsUtil.ShowInfo("Data Saved Successfully.", "Saved");

                DataBack?.Invoke(this, _person.ID);
                this.Close();

            }
            else
            {
                clsUtil.ShowError("Failed to save data. Please try again.");
            }
        }
    }
}