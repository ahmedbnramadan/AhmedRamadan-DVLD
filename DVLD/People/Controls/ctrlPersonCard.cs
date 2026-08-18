using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;
using System.IO;

namespace DVLD
{
    public class ctrlPersonCard : UserControl
    {
        #region Controls Declaration
        private GroupBox gbPersonInformation;
        private Label lblPersonIDTitle, lblPersonID;
        private Label lblNameTitle, lblFullName;
        private Label lblNationalNoTitle, lblNationalNo;
        private Label lblGenderTitle, lblGender;
        private Label lblEmailTitle, lblEmail;
        private Label lblAddressTitle, lblAddress;
        private Label lblDateOfBirthTitle, lblDateOfBirth;
        private Label lblPhoneTitle, lblPhone;
        private Label lblCountryTitle, lblCountry;
        private PictureBox pbPersonImage;
        private LinkLabel llEditPersonInfo;

        private int _PersonID = -1;
        private clsPerson _Person;
        #endregion

        public int PersonID
        {
            get { return _PersonID; }
        }

        public clsPerson SelectedPersonInfo
        {
            get { return _Person; }
        }

        public ctrlPersonCard()
        {
            InitializeComponents();
            _SetupEvents();
        }

        private void _SetupEvents()
        {
            llEditPersonInfo.LinkClicked += LlEditPersonInfo_LinkClicked;
        }

        private void LlEditPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_PersonID == -1 || _Person == null)
                return;

            using (var frm = new frmAddEditPerson(_PersonID))
            {
                frm.DataBack += (s, personID) => LoadPersonInfo(personID);
                frm.ShowDialog();
            }
        }

        private void InitializeComponents()
        {
            // Setup UserControl
            this.Size = new Size(830, 300);
            this.Font = new Font("Microsoft Sans Serif", 9F);

            gbPersonInformation = new GroupBox
            {
                Text = "Person Information",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular),
                Location = new Point(0, 0)
            };

            // Column 1 Labels
            lblPersonIDTitle = new Label { Text = "Person ID:", Location = new Point(20, 40), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblPersonID = new Label { Text = "[???]", Location = new Point(120, 40), AutoSize = true, ForeColor = Color.Red };

            lblNameTitle = new Label { Text = "Name:", Location = new Point(20, 80), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblFullName = new Label { Text = "[???]", Location = new Point(120, 80), AutoSize = true, ForeColor = Color.DarkBlue };

            lblNationalNoTitle = new Label { Text = "National No:", Location = new Point(20, 120), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblNationalNo = new Label { Text = "[???]", Location = new Point(120, 120), AutoSize = true };

            lblGenderTitle = new Label { Text = "Gender:", Location = new Point(20, 160), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblGender = new Label { Text = "[???]", Location = new Point(120, 160), AutoSize = true };

            lblEmailTitle = new Label { Text = "Email:", Location = new Point(20, 200), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblEmail = new Label { Text = "[???]", Location = new Point(120, 200), AutoSize = true };

            lblAddressTitle = new Label { Text = "Address:", Location = new Point(20, 240), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblAddress = new Label { Text = "[???]", Location = new Point(120, 240), AutoSize = true };

            // Column 2 Labels
            lblDateOfBirthTitle = new Label { Text = "Date Of Birth:", Location = new Point(350, 120), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblDateOfBirth = new Label { Text = "[???]", Location = new Point(470, 120), AutoSize = true };

            lblPhoneTitle = new Label { Text = "Phone:", Location = new Point(350, 160), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblPhone = new Label { Text = "[???]", Location = new Point(470, 160), AutoSize = true };

            lblCountryTitle = new Label { Text = "Country:", Location = new Point(350, 200), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblCountry = new Label { Text = "[???]", Location = new Point(470, 200), AutoSize = true };

            // Image & Link
            pbPersonImage = new PictureBox
            {
                Size = new Size(160, 160),
                Location = new Point(650, 50),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.WhiteSmoke
            };

            llEditPersonInfo = new LinkLabel
            {
                Text = "Edit Person Info",
                Location = new Point(680, 220),
                AutoSize = true,
                Visible = false, // يظهر فقط عند تحميل بيانات شخص
                LinkColor = Color.SteelBlue,
                Font = new Font(this.Font, FontStyle.Underline)
            };

            // Adding controls to GroupBox
            gbPersonInformation.Controls.AddRange(new Control[] {
                lblPersonIDTitle, lblPersonID, lblNameTitle, lblFullName,
                lblNationalNoTitle, lblNationalNo, lblGenderTitle, lblGender,
                lblEmailTitle, lblEmail, lblAddressTitle, lblAddress,
                lblDateOfBirthTitle, lblDateOfBirth, lblPhoneTitle, lblPhone,
                lblCountryTitle, lblCountry, pbPersonImage, llEditPersonInfo
            });

            this.Controls.Add(gbPersonInformation);
        }

        public void LoadPersonInfo(int PersonID)
        {
            _Person = clsPerson.Find(PersonID);
            if (_Person == null)
            {
                ResetPersonInfo();
                MessageBox.Show("No Person with ID = " + PersonID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillPersonData();
        }

        public void LoadPersonInfo(string NationalNo)
        {
            _Person = clsPerson.Find(NationalNo);
            if (_Person == null)
            {
                ResetPersonInfo();
                MessageBox.Show("No Person with National No = " + NationalNo, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillPersonData();
        }

        private void _FillPersonData()
        {
            _PersonID = _Person.ID;
            lblPersonID.Text = _Person.ID.ToString();
            lblFullName.Text = _Person.FirstName + " " + _Person.SecondName + " " + _Person.ThirdName + " " + _Person.LastName;
            lblNationalNo.Text = _Person.NationalNo;
            lblGender.Text = (_Person.Gender == 0) ? "Male" : "Female";
            lblEmail.Text = _Person.Email;
            lblPhone.Text = _Person.Phone;
            lblAddress.Text = _Person.Address;
            lblDateOfBirth.Text = _Person.DateOfBirth.ToShortDateString();
            lblCountry.Text = clsCountry.Find(_Person.NationalityCountryID).CountryName;

            _LoadPersonImage();

            llEditPersonInfo.Visible = true;
        }

        private void _LoadPersonImage()
        {
            // 1. Ensure the images folder exists
            clsGlobal.CreateImagesFolderIfDoesNotExist();

            // 2. Determine the default image based on gender
            string fallbackPath = clsGlobal.GetDefaultPersonImagePath(_Person.Gender);

            // 3. Load the default image without locking the file
            Image defaultImage = null;
            if (File.Exists(fallbackPath))
            {
                try
                {
                    using (var ms = new MemoryStream(File.ReadAllBytes(fallbackPath)))
                        defaultImage = Image.FromStream(ms);
                }
                catch 
                {
                    // If loading fails, keep defaultImage as null
                }
            }

            // 4. Use the professional clsUtil.LoadPersonImage method
            clsUtil.LoadPersonImage(pbPersonImage, _Person.ImagePath, defaultImage);
        }

        public void ResetPersonInfo()
        {
            _PersonID = -1;
            lblPersonID.Text = "[???]";
            lblFullName.Text = "[???]";
            lblNationalNo.Text = "[???]";
            lblGender.Text = "[???]";
            lblEmail.Text = "[???]";
            lblPhone.Text = "[???]";
            lblAddress.Text = "[???]";
            lblDateOfBirth.Text = "[???]";
            lblCountry.Text = "[???]";
            pbPersonImage.Image = null;
            llEditPersonInfo.Visible = false;
        }
    }
}