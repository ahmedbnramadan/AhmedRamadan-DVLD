using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsLicense
    {

        public enum enMode { AddNew = 0, Update = 1 };
        enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public int ApplicationID { get; set; }
        public int DriverID { get; set; }
        public int LicenseClassID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsActive { get; set; }
        public short IssueReason { get; set; }
        public int CreatedByUserID { get; set; }

        public clsDriver DriverInfo;
        public clsLicenseClass LicenseClassInfo;
        public clsApplication ApplicationInfo;

        public clsLicense()
        {
            this.ID = -1;
            this.ApplicationID = -1;
            this.DriverID = -1;
            this.LicenseClassID = -1;
            this.IssueDate = DateTime.Now;
            this.ExpirationDate = DateTime.Now;
            this.Notes = "";
            this.PaidFees = -1;
            this.IsActive = true;
            this.IssueReason = -1;
            this.CreatedByUserID = -1;

            Mode = enMode.AddNew;
        }

        private clsLicense(
            int ID,
            int ApplicationID,
            int DriverID,
            int LicenseClass,
            DateTime IssueDate,
            DateTime ExpirationDate,
            string Notes,
            decimal PaidFees,
            bool IsActive,
            short IssueReason,
            int CreatedByUserID
        )
        {
            this.ID = ID;
            this.ApplicationID = ApplicationID;
            this.DriverID = DriverID;
            this.LicenseClassID = LicenseClass;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.Notes = Notes;
            this.PaidFees = PaidFees;
            this.IsActive = IsActive;
            this.IssueReason = IssueReason;
            this.CreatedByUserID = CreatedByUserID;

            this.DriverInfo = clsDriver.Find(this.DriverID);
            this.ApplicationInfo = clsApplication.FindBaseApplication(this.ApplicationID);
            this.LicenseClassInfo = clsLicenseClass.Find(this.LicenseClassID);

            this.Mode = enMode.Update;
        }


        private bool _AddNew()
        {
            this.ID = DataAccess.clsLicenses.AddNewLicense(
                this.ApplicationID,
                this.DriverID,
                this.LicenseClassID,
                this.IssueDate,
                this.ExpirationDate,
                this.Notes,
                this.PaidFees,
                this.IsActive,
                this.IssueReason,
                this.CreatedByUserID
            );

            return (this.ID != -1);
        }

        private bool _Update()
        {
            return (DataAccess.clsLicenses.UpdateLicense(
                this.ID,
                this.ApplicationID,
                this.DriverID,
                this.LicenseClassID,
                this.IssueDate,
                this.ExpirationDate,
                this.Notes,
                this.PaidFees,
                this.IsActive,
                this.IssueReason,
                this.CreatedByUserID
            ));
        }

        public static bool IsLicenseExist(int LicenseID)
        {
            return DataAccess.clsLicenses.IsLicenseExist(LicenseID);
        }

        public static clsLicense Find(int ID)
        {
            int ApplicationID = -1;
            int DriverID = -1;
            int LicenseClassID = -1;
            DateTime IssueDate = DateTime.Now;
            DateTime ExpirationDate = DateTime.Now;
            string Notes = "";
            decimal PaidFees = 0;
            bool IsActive = true;
            short IssueReason = -1;
            int CreatedByUserID = -1;

            if (DataAccess.clsLicenses.GetLicenseByID(ID,
                ref ApplicationID,
                ref DriverID,
                ref LicenseClassID,
                ref IssueDate,
                ref ExpirationDate,
                ref Notes,
                ref PaidFees,
                ref IsActive,
                ref IssueReason,
                ref CreatedByUserID))
            {
                return new clsLicense(
                    ID,
                    ApplicationID,
                    DriverID,
                    LicenseClassID,
                    IssueDate,
                    ExpirationDate,
                    Notes,
                    PaidFees,
                    IsActive,
                    IssueReason,
                    CreatedByUserID
                );
            }

            return null;
        }

        public static clsLicense FindLicenseByApplicationID(int ApplicationID)
        {
            int ID = -1;
            int DriverID = -1;
            int LicenseClassID = -1;
            DateTime IssueDate = DateTime.Now;
            DateTime ExpirationDate = DateTime.Now;
            string Notes = "";
            decimal PaidFees = 0;
            bool IsActive = true;
            short IssueReason = 1;
            int CreatedByUserID = -1;

            if (DataAccess.clsLicenses.GetLicenseByApplicationID(ref ID,
                ApplicationID,
                ref DriverID,
                ref LicenseClassID,
                ref IssueDate,
                ref ExpirationDate,
                ref Notes,
                ref PaidFees,
                ref IsActive,
                ref IssueReason,
                ref CreatedByUserID))
            {
                return new clsLicense(
                    ID,
                    ApplicationID,
                    DriverID,
                    LicenseClassID,
                    IssueDate,
                    ExpirationDate,
                    Notes,
                    PaidFees,
                    IsActive,
                    IssueReason,
                    CreatedByUserID
                );
            }

            return null;
        }

        public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
        {
            return DataAccess.clsLicenses.GetActiveLicenseIDByPersonID(PersonID, LicenseClassID);
        }

        public static DataTable ViewDriverLicenses(int DriverID)
        {
            if (DriverID <= 0) return null;
            return DataAccess.clsLicenses.GetLicensesByDriverID(DriverID);
        }

        public bool IsExpired()
        {
            return (this.ExpirationDate < DateTime.Now);
        }

        public static DataTable GetActiveLicenseIDByDriverID(int DriverID)
        {
            return DataAccess.clsLicenses.GetActiveLicensesByDriverID(DriverID);
        }

        public static bool ActivateLicense(int LicenseID)
        {
            return DataAccess.clsLicenses.ActivateLicense(LicenseID);
        }

        public static bool Deactivate(int LicenseID)
        {
            return DataAccess.clsLicenses.DeactivateLicense(LicenseID);
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




    }
}