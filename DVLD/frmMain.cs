using System;
using System.Drawing;
using System.Windows.Forms;
using Business; // طبقة البزنس التي ربطناها

namespace DVLD
{
    // حذفنا partial لأنك ستكتب كل شيء هنا
    public class frmMain : Form
    {
        // --- العظام: تعريف المكونات التي ستظهر على الشاشة ---
        private MenuStrip _msMain;
        private ToolStripMenuItem _menuApplications;
        private ToolStripMenuItem _menuPeople;
        private ToolStripMenuItem _menuDrivers;
        private ToolStripMenuItem _menuUsers;
        private ToolStripMenuItem _menuAccountSettings;
        private StatusStrip _ssFooter;
        private ToolStripStatusLabel _lblCurrentUser;

        public frmMain()
        {
            // 1. إعدادات النافذة الأم
            this.Text = "DVLD - System Dashboard";
            this.IsMdiContainer = true; // ضروري جداً لفتح النوافذ الأخرى بداخله
            this.WindowState = FormWindowState.Maximized;
            this.MainMenuStrip = _msMain;

            // 2. استدعاء دوال البناء اليدوي
            _InitializeMainMenu();
            _InitializeStatusBar();

            // foreach (Control control in this.Controls)
            // {
            //     // إذا وجدنا الكائن المسؤول عن مساحة العمل الداخلية
            //     if (control is MdiClient)
            //     {
            //         control.BackColor = Color.Black; // اجعلها سوداء
            //         break; // توقف عن البحث بمجرد إيجاده
            //     }
            // }
        }

        // --- الرسم اليدوي: شريط القوائم ---
        private void _InitializeMainMenu()
        {
            _msMain = new MenuStrip();

            // _msMain.BackColor = Color.FromArgb(45, 45, 48); // لون رمادي غامق جداً (Dark Theme)
            // _msMain.ForeColor = Color.White; // الخط أبيض ليظهر فوق الغامق


            // main menu

            _menuApplications = new ToolStripMenuItem("Applications");

            _menuPeople = new ToolStripMenuItem("People");
            _menuPeople.Click += new EventHandler(_OpenPeopleForm);

            _menuDrivers = new ToolStripMenuItem("Drivers");
            _menuDrivers.Click += new EventHandler(_OpenDriversForm);

            _menuUsers = new ToolStripMenuItem("Users");
            _menuUsers.Click += new EventHandler(_OpenUsersForm);

            _menuAccountSettings = new ToolStripMenuItem("Account Setting");



            // Applicatins menu:
            ToolStripMenuItem menuDrivingLicensesServices = new ToolStripMenuItem("Driving Licenses Services");
            ToolStripMenuItem menuManageApllications = new ToolStripMenuItem("Manage Apllications");
            ToolStripMenuItem menuDetainLicenses = new ToolStripMenuItem("Detain Licenses");
            ToolStripMenuItem menuManageApplicationTypes = new ToolStripMenuItem("Manage Application Types");
            ToolStripMenuItem menuMenageTestTypes = new ToolStripMenuItem("Menage Test Types");

            _menuApplications.DropDownItems.Add(menuDrivingLicensesServices);
            _menuApplications.DropDownItems.Add(menuManageApllications);
            _menuApplications.DropDownItems.Add(menuDetainLicenses);
            _menuApplications.DropDownItems.Add(menuManageApplicationTypes);
            _menuApplications.DropDownItems.Add(menuMenageTestTypes);

            // Application Forms:
            menuManageApplicationTypes.Click += new EventHandler(_ManageApplicationTypes);
            menuMenageTestTypes.Click += new EventHandler(_MenageTestTypes);


            // Applications::DrivingLicensesServicesMenu:
            ToolStripMenuItem menuNewDrivingLicense = new ToolStripMenuItem("New Driving License");
            ToolStripMenuItem menuRenewDrivingLicense = new ToolStripMenuItem("Renew Driving License");
            ToolStripMenuItem menuReplacementForLostorDamagedLicense = new ToolStripMenuItem("Replacement For Lost or Damaged License");
            ToolStripMenuItem menuReleaseDetainedDrivingLicense = new ToolStripMenuItem("Release Detained Driving License");
            ToolStripMenuItem menuRetakeTest = new ToolStripMenuItem("Retake Test");

            menuDrivingLicensesServices.DropDownItems.Add(menuNewDrivingLicense);
            menuDrivingLicensesServices.DropDownItems.Add(menuRenewDrivingLicense);
            menuDrivingLicensesServices.DropDownItems.Add(menuReplacementForLostorDamagedLicense);
            menuDrivingLicensesServices.DropDownItems.Add(menuReleaseDetainedDrivingLicense);
            menuDrivingLicensesServices.DropDownItems.Add(menuRetakeTest);

            // Applications::DrivingLicensesServicesForms:
            menuRenewDrivingLicense.Click += new EventHandler(_RenewDrivingLicense);
            menuReplacementForLostorDamagedLicense.Click += new EventHandler(_ReplacementForLostorDamagedLicense);
            menuReleaseDetainedDrivingLicense.Click += new EventHandler(_ReleaseDetainedDrivingLicense);
            menuRetakeTest.Click += new EventHandler(_RetakeTest);

            // Applications::ManageApplications:
            ToolStripMenuItem menuLocalDrivingLicenseApplications = new ToolStripMenuItem("Local Driving License Applications");
            ToolStripMenuItem menuInternationalDrivingLicenseApplications = new ToolStripMenuItem("International Driving License Applications");

            menuManageApllications.DropDownItems.Add(menuLocalDrivingLicenseApplications);
            menuManageApllications.DropDownItems.Add(menuInternationalDrivingLicenseApplications);

            // Applications::ManageApplicationsForms:
            menuLocalDrivingLicenseApplications.Click += new EventHandler(_LocalDrivingLicenseApplications);
            menuInternationalDrivingLicenseApplications.Click += new EventHandler(_InternationalDrivingLicenseApplications);

            // Applications::DetainLicenses:
            ToolStripMenuItem menuManageDetainedLicenses = new ToolStripMenuItem("Manage Detained Licenses");
            ToolStripMenuItem menuDetainLicense = new ToolStripMenuItem("Detain License");
            ToolStripMenuItem menuReleaseDetainedLicense = new ToolStripMenuItem("Release Detained License");

            menuDetainLicenses.DropDownItems.Add(menuManageDetainedLicenses);
            menuDetainLicenses.DropDownItems.Add(menuDetainLicense);
            menuDetainLicenses.DropDownItems.Add(menuReleaseDetainedLicense);

            // Applications::DetainLicenses Forms:
            menuManageDetainedLicenses.Click += new EventHandler(_ManageDetainedLicenses);
            menuDetainLicense.Click += new EventHandler(_DetainLicense);
            menuReleaseDetainedLicense.Click += new EventHandler(_ReleaseDetainedLicense);



            // Applications::DrivingLicensesServicesMenu::NewDrivingLicense:
            ToolStripMenuItem menuLocalLicense = new ToolStripMenuItem("Local License");
            ToolStripMenuItem menuInternationalLicense = new ToolStripMenuItem("International License");

            menuNewDrivingLicense.DropDownItems.Add(menuLocalLicense);
            menuNewDrivingLicense.DropDownItems.Add(menuInternationalLicense);

            menuLocalLicense.Click += new EventHandler(_LocalLicense);
            menuInternationalLicense.Click += new EventHandler(_InternationalLicense);



            // Account setting menu:
            ToolStripMenuItem menuCuurentUserInfo = new ToolStripMenuItem("Current User Info");
            ToolStripMenuItem menuChangePassword = new ToolStripMenuItem("Change Password");
            ToolStripMenuItem menuLogOut = new ToolStripMenuItem("Log Out");

            menuCuurentUserInfo.Click += new EventHandler(_CurrnetUserInfo);
            menuChangePassword.Click += new EventHandler(_ChangePassword);
            menuLogOut.Click += new EventHandler(_LogOut);

            _menuAccountSettings.DropDownItems.Add(menuCuurentUserInfo);
            _menuAccountSettings.DropDownItems.Add(menuChangePassword);
            _menuAccountSettings.DropDownItems.Add(menuLogOut);


            // Main Form Menu
            _msMain.Items.Add(_menuApplications);
            _msMain.Items.Add(_menuPeople);
            _msMain.Items.Add(_menuDrivers);
            _msMain.Items.Add(_menuUsers);
            _msMain.Items.Add(_menuAccountSettings);

            this.Controls.Add(_msMain);
        }

        // --- الرسم اليدوي: شريط الحالة في الأسفل ---
        private void _InitializeStatusBar()
        {
            _ssFooter = new StatusStrip();
            _lblCurrentUser = new ToolStripStatusLabel("User: Admin");

            _ssFooter.Items.Add(_lblCurrentUser);
            this.Controls.Add(_ssFooter);
        }

        // --- منطق الربط مع الملفات الأخرى ---

        // Main menu Froms:
        private void _OpenPeopleForm(object sender, EventArgs e)
        {
            Form frmTemp = Application.OpenForms["frmListPeople"];

            if (frmTemp != null)
            {
                frmTemp.BringToFront();
                frmTemp.Focus();
            }

            else
            {
                frmListPeople frm = new frmListPeople();
                frm.Name = "frmListPeople";
                frm.MdiParent = this;
                frm.WindowState = FormWindowState.Maximized;
                frm.Show();
            }
        }

        private void _OpenDriversForm(object sender, EventArgs e)
        {
            frmListDrivers frm2 = new frmListDrivers();
            frm2.Name = "frmListDrivers"; frm2.MdiParent = this;
            frm2.WindowState = FormWindowState.Maximized; frm2.Show();
        }

        private void _OpenUsersForm(object senter, EventArgs e)
        {
            Form frmTemp = Application.OpenForms["frmListUsers"];

            if (frmTemp != null)
            {
                frmTemp.BringToFront();
                frmTemp.Focus();
            }

            else
            {
                frmListUsers frm = new frmListUsers();
                frm.Name = "frmListUsers";
                frm.MdiParent = this;
                frm.WindowState = FormWindowState.Maximized;
                frm.Show();
            }
        }

        // MainMenu::Applications Driect Forms:

        private void _ManageApplicationTypes(object sender, EventArgs e)
        {
            new frmListApplicationTypes().ShowDialog();
        }

        private void _MenageTestTypes(object sender, EventArgs e)
        {
            MessageBox.Show("Comming Soon");
        }

        // Applications::DrivingLicensesServices Direct Forms:

        private void _RenewDrivingLicense(object sender, EventArgs e)
        {
            new frmRenewDrivingLicense().ShowDialog();
        }

        private void _ReplacementForLostorDamagedLicense(object sender, EventArgs e)
        {
            new frmReplaceLicense().ShowDialog();
        }

        private void _ReleaseDetainedDrivingLicense(object sender, EventArgs e)
        {
            new frmReleaseDetainedLicense().ShowDialog();
        }

        private void _RetakeTest(object sender, EventArgs e)
        {
            MessageBox.Show("Comming Soon");
        }

        // Applications::DrivingLicensesServicesMenu::NewDrivingLicense Direct Forms:

        private void _LocalLicense(object sender, EventArgs e)
        {
            new frmAddNewLocalDrivingLicenseApplication().ShowDialog();
        }

        private void _InternationalLicense(object sender, EventArgs e)
        {
            new frmAddNewInternationalLicense().ShowDialog();
        }

        // Applications::DrivingLicensesServicesMenu::ManageApplications Forms:
        private void _LocalDrivingLicenseApplications(object sender, EventArgs e)
        {
            Form t1 = Application.OpenForms["frmListLocalApps"];
            if (t1 != null) { t1.BringToFront(); }
            else
            {
                var f = new frmListLocalDrivingLicenseApplications();
                f.Name = "frmListLocalApps"; f.MdiParent = this;
                f.WindowState = FormWindowState.Maximized; f.Show();
            }
        }

        private void _InternationalDrivingLicenseApplications(object sender, EventArgs e)
        {
            MessageBox.Show("Comming Soon");
        }

        // Applications::DrivingLicensesServicesMenu::DetainedLicenses Forms:
        private void _ManageDetainedLicenses(object sender, EventArgs e)
        {
            new frmListDetainedLicenses().ShowDialog();
        }

        private void _DetainLicense(object sender, EventArgs e)
        {
            MessageBox.Show("Comming Soon");
        }

        private void _ReleaseDetainedLicense(object sender, EventArgs e)
        {
            new frmReleaseDetainedLicense().ShowDialog();
        }



        // Account Settings Menu:
        private void _CurrnetUserInfo(object sender, EventArgs e)
        {
            new frmShowUserInfo(clsGlobal.CurrentUserID).ShowDialog();
        }

        private void _ChangePassword(object sender, EventArgs e)
        {
            new frmChangePassword().ShowDialog();
        }

        private void _LogOut(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Log Out",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                if (new frmLogin().ShowDialog() == DialogResult.OK)
                    this.Show();
                else
                    Application.Exit();
            }


        }
    }


}