using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsInternationalLicense
    {
        public enum enMode 
        { 
            AddNew = 0, 
            Update = 1 
        };

        public enMode Mode = enMode.AddNew;

        public int InternationalLicenseID { get; set; }
        
        public int ApplicationID { get; set; }
        
        public int DriverID { get; set; }
        
        public int IssuedUsingLocalLicenseID { get; set; }
        
        public DateTime IssueDate { get; set; }
        
        public DateTime ExpirationDate { get; set; }
        
        public bool IsActive { get; set; }
        
        public int CreatedByUserID { get; set; }

        public clsApplication ApplicationInfo { get; set; }
        
        public clsDriver DriverInfo { get; set; }
        
        public clsUser CreatedByUserInfo { get; set; }
        
        public clsLicense LocalLicenseInfo { get; set; }

        public clsInternationalLicense()
        {
            this.InternationalLicenseID = -1;
            this.ApplicationID = -1;
            this.DriverID = -1;
            this.IssuedUsingLocalLicenseID = -1;
            this.IssueDate = DateTime.Now;
            this.ExpirationDate = DateTime.Now;
            this.IsActive = true;
            this.CreatedByUserID = -1;

            this.Mode = enMode.AddNew;
        }

        private clsInternationalLicense(
            int InternationalLicenseID,
            int ApplicationID,
            int DriverID,
            int IssuedUsingLocalLicenseID,
            DateTime IssueDate,
            DateTime ExpirationDate,
            bool IsActive,
            int CreatedByUserID
        )
        {
            this.InternationalLicenseID = InternationalLicenseID;
            this.ApplicationID = ApplicationID;
            this.DriverID = DriverID;
            this.IssuedUsingLocalLicenseID = IssuedUsingLocalLicenseID;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.IsActive = IsActive;
            this.CreatedByUserID = CreatedByUserID;

            this.ApplicationInfo = clsApplication.FindBaseApplication(
                this.ApplicationID
            );

            this.DriverInfo = clsDriver.Find(
                this.DriverID
            );

            this.CreatedByUserInfo = clsUser.Find(
                this.CreatedByUserID
            );

            this.LocalLicenseInfo = clsLicense.Find(
                this.IssuedUsingLocalLicenseID
            );

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.InternationalLicenseID = DataAccess.clsInternationalLicenses.AddNewInternationalLicense(
                this.ApplicationID,
                this.DriverID,
                this.IssuedUsingLocalLicenseID,
                this.IssueDate,
                this.ExpirationDate,
                this.IsActive,
                this.CreatedByUserID
            );

            return (this.InternationalLicenseID != -1);
        }

        private bool _Update()
        {
            return DataAccess.clsInternationalLicenses.UpdateInternationalLicense(
                this.InternationalLicenseID,
                this.ApplicationID,
                this.DriverID,
                this.IssuedUsingLocalLicenseID,
                this.IssueDate,
                this.ExpirationDate,
                this.IsActive,
                this.CreatedByUserID
            );
        }

        public static clsInternationalLicense Find(int InternationalLicenseID)
        {
            int ApplicationID = -1;
            int DriverID = -1;
            int IssuedUsingLocalLicenseID = -1;
            DateTime IssueDate = DateTime.Now;
            DateTime ExpirationDate = DateTime.Now;
            bool IsActive = true;
            int CreatedByUserID = -1;

            if (DataAccess.clsInternationalLicenses.GetInternationalLicenseByID(
                    InternationalLicenseID,
                    ref ApplicationID,
                    ref DriverID,
                    ref IssuedUsingLocalLicenseID,
                    ref IssueDate,
                    ref ExpirationDate,
                    ref IsActive,
                    ref CreatedByUserID
                ))
            {
                return new clsInternationalLicense(
                    InternationalLicenseID,
                    ApplicationID,
                    DriverID,
                    IssuedUsingLocalLicenseID,
                    IssueDate,
                    ExpirationDate,
                    IsActive,
                    CreatedByUserID
                );
            }
            else
            {
                return null;
            }
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNew())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    break;

                case enMode.Update:
                    return _Update();
            }

            return false;
        }

        public static DataTable GetAllInternationalLicenses()
        {
            return DataAccess.clsInternationalLicenses.GetAllInternationalLicenses();
        }

        public static DataTable GetDriverInternationalLicenses(int DriverID)
        {
            return DataAccess.clsInternationalLicenses.GetInternationalLicensesByDriverID(
                DriverID
            );
        }

        public static bool IsDriverHaveActiveInternationalLicense(int DriverID)
        {
            return DataAccess.clsInternationalLicenses.IsDriverHaveActiveInternationalLicense(
                DriverID
            );
        }

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            return DataAccess.clsInternationalLicenses.GetActiveInternationalLicenseIDByDriverID(
                DriverID
            );
        }
    }
}