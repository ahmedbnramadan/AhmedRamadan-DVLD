// using System;
// using System.Data;
// using System.Drawing;
// using System.Windows.Forms;
// using Business;
// using System.IO;

// namespace DVLD
// {
//     public class ctrlAddNewPersonNew : UserControl
//     {
//         // إضافة Event لإعلام الفورم الأب بانتهاء عملية الحفظ
//         public delegate void DataBackEventHandler(object sender, int PersonID);
//         public event DataBackEventHandler OnSaveComplete;

//         #region Controls Declaration
//         private Label lblTitle, lblPersonIDTitle, lblPersonID;
//         private Label lblFirstName, lblSecondName, lblThirdName, lblLastName;
//         private Label lblNationalNo, lblDateOfBirth, lblGender, lblPhone, lblEmail, lblAddress, lblCountry;
//         private TextBox txtFirstName, txtSecondName, txtThirdName, txtLastName;
//         private TextBox txtNationalNo, txtPhone, txtEmail, txtAddress;
//         private DateTimePicker dtpDateOfBirth;
//         private ComboBox cbCountry;
//         private GroupBox gbGender;
//         private RadioButton rbMale, rbFemale;
//         private PictureBox pbPersonImage;
//         private LinkLabel llSetImage, llRemoveImage;
//         private Button btnSave, btnClose;

//         private enum enMode { AddNew = 0, Update = 1 };
//         private enMode _Mode = enMode.AddNew;
//         private int _PersonID = -1;
//         private clsPerson _Person;
//         #endregion

//         public ctrlAddNewPersonNew()
//         {
//             InitializeComponents();
//         }

//         // دالة لتحميل البيانات في حالة التعديل (Reusability)
//         public void LoadPersonData(int PersonID)
//         {
//             _PersonID = PersonID;
//             if (_PersonID == -1)
//             {
//                 _Mode = enMode.AddNew;
//                 _Person = new clsPerson();
//                 _ResetDefaultValues();
//                 return;
//             }

//             _Person = clsPerson.Find(_PersonID);
//             _Mode = enMode.Update;

//             if (_Person == null)
//             {
//                 MessageBox.Show("No Person with ID = " + _PersonID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                 return;
//             }

//             _FillFieldsWithData();
//         }

//         private void _ResetDefaultValues()
//         {
//             _LoadCountries();
//             lblTitle.Text = "Add New Person";
//             lblPersonID.Text = "N/A";
//             txtFirstName.Text = "";
//             txtSecondName.Text = "";
//             txtThirdName.Text = "";
//             txtLastName.Text = "";
//             txtNationalNo.Text = "";
//             txtEmail.Text = "";
//             txtPhone.Text = "";
//             txtAddress.Text = "";
//             dtpDateOfBirth.MaxDate = DateTime.Now.AddYears(-18); // السن القانوني
//             cbCountry.SelectedIndex = cbCountry.FindString("Egypt");
//             rbMale.Checked = true;
//             pbPersonImage.Image = null; // أو صورة افتراضية
//         }

//         private void _LoadCountries()
//         {
//             DataTable dtCountries = clsCountry.GetAllCountries();
//             cbCountry.DataSource = dtCountries;
//             cbCountry.DisplayMember = "CountryName";
//             cbCountry.ValueMember = "CountryID";
//         }

//         private void _FillFieldsWithData()
//         {
//             lblTitle.Text = "Edit Person";
//             lblPersonID.Text = _Person.ID.ToString();
//             txtFirstName.Text = _Person.FirstName;
//             txtSecondName.Text = _Person.SecondName;
//             txtThirdName.Text = _Person.ThirdName;
//             txtLastName.Text = _Person.LastName;
//             txtNationalNo.Text = _Person.NationalNo;
//             txtEmail.Text = _Person.Email;
//             txtPhone.Text = _Person.Phone;
//             txtAddress.Text = _Person.Address;
//             dtpDateOfBirth.Value = _Person.DateOfBirth;
//             cbCountry.SelectedValue = _Person.NationalityCountryID;

//             if (_Person.Gender == 0) rbMale.Checked = true; else rbFemale.Checked = true;

//             if (_Person.ImagePath != "" && File.Exists(_Person.ImagePath))
//                 pbPersonImage.ImageLocation = _Person.ImagePath;
//         }

//         private void btnSave_Click(object sender, EventArgs e)
//         {
//             if (!this.ValidateChildren()) // تفعيل التحقق من المدخلات
//             {
//                 MessageBox.Show("Some fields are not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                 return;
//             }

//             _Person.FirstName = txtFirstName.Text.Trim();
//             _Person.SecondName = txtSecondName.Text.Trim();
//             _Person.ThirdName = txtThirdName.Text.Trim();
//             _Person.LastName = txtLastName.Text.Trim();
//             _Person.NationalNo = txtNationalNo.Text.Trim();
//             _Person.Email = txtEmail.Text.Trim();
//             _Person.Phone = txtPhone.Text.Trim();
//             _Person.Address = txtAddress.Text.Trim();
//             _Person.DateOfBirth = dtpDateOfBirth.Value;
//             _Person.NationalityCountryID = (int)cbCountry.SelectedValue;
//             _Person.Gender = (short)(rbMale.Checked ? 0 : 1);
//             _Person.ImagePath = pbPersonImage.ImageLocation ?? "";

//             if (_Person.Save())
//             {
//                 lblTitle.Text = "Edit Person";
//                 _Mode = enMode.Update;
//                 _PersonID = _Person.ID;
//                 lblPersonID.Text = _PersonID.ToString();
                
//                 MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
//                 // استدعاء الـ Event لإبلاغ الفورم الأب
//                 OnSaveComplete?.Invoke(this, _PersonID);
//             }
//             else
//                 MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//         }

//         // ... (بقية كود الـ InitializeComponents كما هو مع ربط الأحداث)
//     }
// }