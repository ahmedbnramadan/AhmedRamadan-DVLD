using System;
using System.Windows.Forms;

namespace DVLD
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show login; only open Main if login succeeded
            frmLogin login = new frmLogin();
            if (login.ShowDialog() == DialogResult.OK)
                Application.Run(new frmMain());

        }
    }
}