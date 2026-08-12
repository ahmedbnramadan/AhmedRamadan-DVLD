using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Business;

namespace DVLD
{
    public class frmMain : Form
    {
        // Main form controls
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
            // Main form settings
            Text = "DVLD - System Dashboard";
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;

            BackColor = Color.White;
            ForeColor = Color.Black;

            // Build the menu and status bar
            _InitializeMainMenu();
            _InitializeStatusBar();

            MainMenuStrip = _msMain;
        }

        // Main menu
        private void _InitializeMainMenu()
        {
            _msMain = new MenuStrip();

            // Light menu appearance
            _msMain.BackColor = Color.White;
            _msMain.ForeColor = Color.FromArgb(45, 45, 48);

            _msMain.ImageScalingSize = new Size(24, 24);

            // Main menu items
            _menuApplications = new ToolStripMenuItem("Applications");
            _menuApplications.Image = LoadIcon("Applications 64.png");

            _menuPeople = new ToolStripMenuItem("People");
            _menuPeople.Image = LoadIcon("People 64.png");
            _menuPeople.Click += _OpenPeopleForm;

            _menuDrivers = new ToolStripMenuItem("Drivers");
            _menuDrivers.Image = LoadIcon("Drivers 64.png");
            _menuDrivers.Click += _OpenDriversForm;

            _menuUsers = new ToolStripMenuItem("Users");
            _menuUsers.Image = LoadIcon("Users 2 64.png");
            _menuUsers.Click += _OpenUsersForm;

            _menuAccountSettings = new ToolStripMenuItem("Account Setting");
            _menuAccountSettings.Image = LoadIcon("account_settings 64.png");

            // Applications menu
            ToolStripMenuItem menuDrivingLicensesServices =
                new ToolStripMenuItem("Driving Licenses Services");

            ToolStripMenuItem menuManageApplications =
                new ToolStripMenuItem("Manage Applications");

            ToolStripMenuItem menuDetainLicenses =
                new ToolStripMenuItem("Detain Licenses");

            ToolStripMenuItem menuManageApplicationTypes =
                new ToolStripMenuItem("Manage Application Types");

            ToolStripMenuItem menuManageTestTypes =
                new ToolStripMenuItem("Manage Test Types");

            // Applications icons
            menuDrivingLicensesServices.Image =
                LoadIcon("Driver License 48.png");

            menuManageApplications.Image =
                LoadIcon("Manage Applications 32.png");

            menuDetainLicenses.Image =
                LoadIcon("Detain 32.png");

            menuManageApplicationTypes.Image =
                LoadIcon("Application Types 64.png");

            menuManageTestTypes.Image =
                LoadIcon("Test Type 64.png");

            _menuApplications.DropDownItems.Add(menuDrivingLicensesServices);
            _menuApplications.DropDownItems.Add(menuManageApplications);
            _menuApplications.DropDownItems.Add(menuDetainLicenses);
            _menuApplications.DropDownItems.Add(menuManageApplicationTypes);
            _menuApplications.DropDownItems.Add(menuManageTestTypes);

            // Application forms
            menuManageApplicationTypes.Click += _ManageApplicationTypes;
            menuManageTestTypes.Click += _ManageTestTypes;

            // Driving Licenses Services
            ToolStripMenuItem menuNewDrivingLicense =
                new ToolStripMenuItem("New Driving License");

            ToolStripMenuItem menuRenewDrivingLicense =
                new ToolStripMenuItem("Renew Driving License");

            ToolStripMenuItem menuReplacementForLostorDamagedLicense =
                new ToolStripMenuItem("Replacement For Lost or Damaged License");

            ToolStripMenuItem menuReleaseDetainedDrivingLicense =
                new ToolStripMenuItem("Release Detained Driving License");

            ToolStripMenuItem menuRetakeTest =
                new ToolStripMenuItem("Retake Test");

            // Driving Licenses Services icons
            menuNewDrivingLicense.Image =
                LoadIcon("New Driving License 32.png");

            menuRenewDrivingLicense.Image =
                LoadIcon("Renew Driving License 32.png");

            menuReplacementForLostorDamagedLicense.Image =
                LoadIcon("Damaged Driving License 32.png");

            menuReleaseDetainedDrivingLicense.Image =
                LoadIcon("Release Detained License 32.png");

            menuRetakeTest.Image =
                LoadIcon("Retake Test 32.png");

            menuDrivingLicensesServices.DropDownItems.Add(menuNewDrivingLicense);
            menuDrivingLicensesServices.DropDownItems.Add(menuRenewDrivingLicense);
            menuDrivingLicensesServices.DropDownItems.Add(
                menuReplacementForLostorDamagedLicense);
            menuDrivingLicensesServices.DropDownItems.Add(
                menuReleaseDetainedDrivingLicense);
            menuDrivingLicensesServices.DropDownItems.Add(menuRetakeTest);

            // Driving License Services forms
            menuRenewDrivingLicense.Click += _RenewDrivingLicense;
            menuReplacementForLostorDamagedLicense.Click +=
                _ReplacementForLostorDamagedLicense;
            menuReleaseDetainedDrivingLicense.Click +=
                _ReleaseDetainedDrivingLicense;
            menuRetakeTest.Click += _RetakeTest;

            // Manage Applications
            ToolStripMenuItem menuLocalDrivingLicenseApplications =
                new ToolStripMenuItem("Local Driving License Applications");

            ToolStripMenuItem menuInternationalDrivingLicenseApplications =
                new ToolStripMenuItem(
                    "International Driving License Applications");

            menuLocalDrivingLicenseApplications.Image =
                LoadIcon("Local 32.png");

            menuInternationalDrivingLicenseApplications.Image =
                LoadIcon("International 32.png");

            menuManageApplications.DropDownItems.Add(
                menuLocalDrivingLicenseApplications);
            menuManageApplications.DropDownItems.Add(
                menuInternationalDrivingLicenseApplications);

            // Manage Applications forms
            menuLocalDrivingLicenseApplications.Click +=
                _LocalDrivingLicenseApplications;

            menuInternationalDrivingLicenseApplications.Click +=
                _InternationalDrivingLicenseApplications;

            // Detained Licenses
            ToolStripMenuItem menuManageDetainedLicenses =
                new ToolStripMenuItem("Manage Detained Licenses");

            ToolStripMenuItem menuDetainLicense =
                new ToolStripMenuItem("Detain License");

            ToolStripMenuItem menuReleaseDetainedLicense =
                new ToolStripMenuItem("Release Detained License");

            menuManageDetainedLicenses.Image =
                LoadIcon("Detained Driving License 32.png");

            menuDetainLicense.Image =
                LoadIcon("Detain 32.png");

            menuReleaseDetainedLicense.Image =
                LoadIcon("Release Detained License 32.png");

            menuDetainLicenses.DropDownItems.Add(
                menuManageDetainedLicenses);
            menuDetainLicenses.DropDownItems.Add(menuDetainLicense);
            menuDetainLicenses.DropDownItems.Add(
                menuReleaseDetainedLicense);

            // Detained Licenses forms
            menuManageDetainedLicenses.Click +=
                _ManageDetainedLicenses;

            menuDetainLicense.Click +=
                _DetainLicense;

            menuReleaseDetainedLicense.Click +=
                _ReleaseDetainedLicense;

            // New Driving License
            ToolStripMenuItem menuLocalLicense =
                new ToolStripMenuItem("Local License");

            ToolStripMenuItem menuInternationalLicense =
                new ToolStripMenuItem("International License");

            menuLocalLicense.Image =
                LoadIcon("Local 32.png");

            menuInternationalLicense.Image =
                LoadIcon("International 32.png");

            menuNewDrivingLicense.DropDownItems.Add(menuLocalLicense);
            menuNewDrivingLicense.DropDownItems.Add(
                menuInternationalLicense);

            menuLocalLicense.Click += _LocalLicense;
            menuInternationalLicense.Click += _InternationalLicense;

            // Account Settings menu
            ToolStripMenuItem menuCurrentUserInfo =
                new ToolStripMenuItem("Current User Info");

            ToolStripMenuItem menuChangePassword =
                new ToolStripMenuItem("Change Password");

            ToolStripMenuItem menuLogOut =
                new ToolStripMenuItem("Log Out");

            menuCurrentUserInfo.Image =
                LoadIcon("User 32 -2.png");

            menuChangePassword.Image =
                LoadIcon("Password 32.png");

            menuLogOut.Image =
                LoadIcon("Sign Out 32.png");

            menuCurrentUserInfo.Click += _CurrentUserInfo;
            menuChangePassword.Click += _ChangePassword;
            menuLogOut.Click += _LogOut;

            _menuAccountSettings.DropDownItems.Add(
                menuCurrentUserInfo);
            _menuAccountSettings.DropDownItems.Add(
                menuChangePassword);
            _menuAccountSettings.DropDownItems.Add(menuLogOut);

            // Add main menu items
            _msMain.Items.Add(_menuApplications);
            _msMain.Items.Add(_menuPeople);
            _msMain.Items.Add(_menuDrivers);
            _msMain.Items.Add(_menuUsers);
            _msMain.Items.Add(_menuAccountSettings);

            Controls.Add(_msMain);
        }

        // Status bar
        private void _InitializeStatusBar()
        {
            _ssFooter = new StatusStrip();

            _ssFooter.BackColor = Color.WhiteSmoke;
            _ssFooter.ForeColor = Color.FromArgb(45, 45, 48);

            _lblCurrentUser =
                new ToolStripStatusLabel(
                    "User: Admin");

            _ssFooter.Items.Add(_lblCurrentUser);

            Controls.Add(_ssFooter);
        }

        // Loads an icon from the application's Icons folder.
        private Image LoadIcon(string fileName)
        {
            string path = Path.Combine(
                Application.StartupPath,
                "Icons",
                fileName);

            return Image.FromFile(path);
        }

        // Form navigation

        // Opens an MDI child only once.
        private void OpenMDIForm<T>() where T : Form, new()
        {
            foreach (Form frm in MdiChildren)
            {
                if (frm is T)
                {
                    frm.BringToFront();
                    frm.Focus();
                    return;
                }
            }

            T newForm = new T
            {
                MdiParent = this,
                WindowState = FormWindowState.Maximized
            };

            newForm.Show();
        }

        // Opens a parameterized MDI child only once.
        private void OpenMDIForm<T>(
            Func<T> formFactory) where T : Form
        {
            foreach (Form frm in MdiChildren)
            {
                if (frm is T)
                {
                    frm.BringToFront();
                    frm.Focus();
                    return;
                }
            }

            T newForm = formFactory();

            newForm.MdiParent = this;
            newForm.WindowState = FormWindowState.Maximized;
            newForm.Show();
        }

        // Form opening handlers

        private void _OpenPeopleForm(object sender, EventArgs e)
        {
            OpenMDIForm<frmListPeople>();
        }

        private void _OpenDriversForm(object sender, EventArgs e)
        {
            OpenMDIForm<frmListDrivers>();
        }

        private void _OpenUsersForm(object sender, EventArgs e)
        {
            OpenMDIForm<frmListUsers>();
        }

        // Applications - Direct Forms

        private void _ManageApplicationTypes(
            object sender,
            EventArgs e)
        {
            new frmListApplicationTypes().ShowDialog();
        }

        private void _ManageTestTypes(
            object sender,
            EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }

        // Driving Licenses Services

        private void _RenewDrivingLicense(
            object sender,
            EventArgs e)
        {
            new frmRenewDrivingLicense().ShowDialog();
        }

        private void _ReplacementForLostorDamagedLicense(
            object sender,
            EventArgs e)
        {
            new frmReplaceLicense().ShowDialog();
        }

        private void _ReleaseDetainedDrivingLicense(
            object sender,
            EventArgs e)
        {
            new frmReleaseDetainedLicense().ShowDialog();
        }

        private void _RetakeTest(
            object sender,
            EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }

        // New Driving License

        private void _LocalLicense(
            object sender,
            EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }

        private void _InternationalLicense(
            object sender,
            EventArgs e)
        {
            new frmAddNewInternationalLicense().ShowDialog();
        }

        // Manage Applications

        private void _LocalDrivingLicenseApplications(
            object sender,
            EventArgs e)
        {
            OpenMDIForm<frmListLocalDrivingLicenseApplications>();
        }

        private void _InternationalDrivingLicenseApplications(
            object sender,
            EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }

        // Detained Licenses

        private void _ManageDetainedLicenses(
            object sender,
            EventArgs e)
        {
            new frmListDetainedLicenses().ShowDialog();
        }

        private void _DetainLicense(
            object sender,
            EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }

        private void _ReleaseDetainedLicense(
            object sender,
            EventArgs e)
        {
            new frmReleaseDetainedLicense().ShowDialog();
        }

        // Account Settings

        private void _CurrentUserInfo(
            object sender,
            EventArgs e)
        {
            new frmShowUserInfo(
                clsGlobal.CurrentUserID).ShowDialog();
        }

        private void _ChangePassword(
            object sender,
            EventArgs e)
        {
            new frmChangePassword().ShowDialog();
        }

        private void _LogOut(
            object sender,
            EventArgs e)
        {
            if (MessageBox.Show(
                    "Are you sure you want to log out?",
                    "Log Out",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                Hide();

                if (new frmLogin().ShowDialog()
                    == DialogResult.OK)
                {
                    Show();
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }
}