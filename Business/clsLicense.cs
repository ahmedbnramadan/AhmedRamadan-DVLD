using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsLicense
    {

        public enum enMode { AddNew = 0, Update = 1 };
        public enum enIssueReason {FirstTime = 1, Renew = 2, DamagedReplacement = 3, LostReplacement = 4};
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public int ApplicationID { get; set; }
        public int DriverID { get; set; }
        public int LicenseClassID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByUserID { get; set; }
        public enIssueReason IssueReason { get; set; }
        public string IssueReasonText
        {
            get
            {
                return GetIssueReasonText(this.IssueReason);
            }
        }

        private clsDriver _DriverInfo;
        public clsDriver DriverInfo
        {
            get
            {
                if (_DriverInfo == null)
                    _DriverInfo = clsDriver.Find(this.DriverID);
                return _DriverInfo;
            }
        }

        private clsLicenseClass _LicenseClassInfo;
        public clsLicenseClass LicenseClassInfo
        {
            get
            {
                if (_LicenseClassInfo == null)
                    _LicenseClassInfo = clsLicenseClass.Find(this.LicenseClassID);
                return _LicenseClassInfo;
            }
        }

        private clsApplication _ApplicationInfo;
        public clsApplication ApplicationInfo
        {
            get
            {
                if (_ApplicationInfo == null)
                    _ApplicationInfo = clsApplication.FindBaseApplication(this.ApplicationID);
                return _ApplicationInfo;
            }
        }

        private clsDetainedLicense _DetainedLicenseInfo;
        public clsDetainedLicense DetainedLicenseInfo
        {
            get
            {
                if (_DetainedLicenseInfo == null)
                    _DetainedLicenseInfo = clsDetainedLicense.FindByLicenseID(this.ID);
                return _DetainedLicenseInfo;
            }
        }

        public bool IsDetained
        {
            get
            {
                return clsDetainedLicense.IsLicenseDetained(this.ID);
            }
        }

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
            this.IssueReason = enIssueReason.FirstTime;
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
            enIssueReason IssueReason,
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
                (short)this.IssueReason,
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
                (short)this.IssueReason,
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
            short IssueReason = 1;
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
                    (clsLicense.enIssueReason)IssueReason,
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
                    (clsLicense.enIssueReason)IssueReason,
                    CreatedByUserID
                );
            }

            return null;
        }

        public static DataTable GetAllLicenses()
        {
            return DataAccess.clsLicenses.GetAllLicenses();
        }

        public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
        {
            return DataAccess.clsLicenses.GetActiveLicenseIDByPersonID(PersonID, LicenseClassID);
        }

        public static bool DoesPersonHaveActiveLicense(int PersonID, int LicenseClassID)
        {
            return GetActiveLicenseIDByPersonID(PersonID, LicenseClassID) != -1;
        }

        public static DataTable GetDriverLicenses(int DriverID)
        {
            if (DriverID <= 0) return null;
            return DataAccess.clsLicenses.GetLicensesByDriverID(DriverID);
        }

        public bool IsExpired()
        {
            return DateTime.Now > this.ExpirationDate;
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

        private static string GetIssueReasonText(enIssueReason IssueReason)
        {
            switch (IssueReason)
            {
                case enIssueReason.FirstTime:
                    return "First Time";

                case enIssueReason.Renew:
                    return "Renew";

                case enIssueReason.DamagedReplacement:
                    return "Damaged Replacement";

                case enIssueReason.LostReplacement:
                    return "Lost Replacement";

                default:
                    return "First Time";
            }
        }

        public int Detain(decimal FineFees, int CreatedByUserID)
        {
            if (FineFees <= 0)
            return -1;

            clsDetainedLicense DetainedLicense = new clsDetainedLicense();

            DetainedLicense.LicenseID = this.ID;
            DetainedLicense.FineFees = FineFees;
            DetainedLicense.CreatedByUserID = CreatedByUserID;

            if (DetainedLicense.Save())
                return DetainedLicense.ID;

            return -1;
        }

        public bool ReleaseDetained(int ReleasedByUserID, ref int ApplicationID)
        {
            ApplicationID = -1;

            // Get the detained license
            clsDetainedLicense DetainedLicense =
                clsDetainedLicense.FindByLicenseID(this.ID);

            if (DetainedLicense == null || DetainedLicense.IsReleased)
                return false;

            // Create the release application
            clsApplication Application = new clsApplication();

            Application.ApplicantPersonID = this.DriverInfo.PersonID;
            Application.ApplicationTypeID = 5;
            Application.ApplicationDate = DateTime.Now;
            Application.ApplicationStatus = clsApplication.enApplicationStatus.Completed;
            Application.LastStatusDate = DateTime.Now;
            clsApplicationType ApplicationType = clsApplicationType.Find(5);
            if (ApplicationType == null)
                return false;

            Application.PaidFees = ApplicationType.Fees;
            Application.CreatedByUserID = ReleasedByUserID;

            // Save the application
            if (!Application.Save())
                return false;

            // Release the detained license
            if (!DetainedLicense.Release(
                ReleasedByUserID,
                Application.ApplicationID))
            {
                return false;
            }

            ApplicationID = Application.ApplicationID;

            return true;
        }

        public clsLicense Renew(string Notes, int CreatedByUserID)
        {
            clsApplication Application = new clsApplication();

            Application.ApplicantPersonID = this.DriverInfo.PersonID;
            Application.ApplicationTypeID = 2;
            Application.ApplicationDate = DateTime.Now;
            Application.ApplicationStatus = clsApplication.enApplicationStatus.Completed;
            Application.LastStatusDate = DateTime.Now;
            Application.PaidFees = clsApplicationType.Find(2).Fees;
            Application.CreatedByUserID = CreatedByUserID;

            if (!Application.Save())
                return null;

            clsLicense NewLicense = new clsLicense();

            NewLicense.ApplicationID = Application.ApplicationID;
            NewLicense.DriverID = this.DriverID;
            NewLicense.LicenseClassID = this.LicenseClassID;
            NewLicense.IssueDate = DateTime.Now;
            NewLicense.ExpirationDate =
                DateTime.Now.AddYears(this.LicenseClassInfo.DefaultValidityLength);
            NewLicense.Notes = Notes;
            NewLicense.PaidFees = this.LicenseClassInfo.Fees;
            NewLicense.IsActive = true;
            NewLicense.IssueReason = enIssueReason.Renew;
            NewLicense.CreatedByUserID = CreatedByUserID;

            if (!NewLicense.Save())
                return null;

            this.IsActive = false;

            if (!this.Save())
                return null;

            return NewLicense;
        }

        public clsLicense Replace(enIssueReason IssueReason, int CreatedByUserID)
        {
            clsApplication Application = new clsApplication();

            Application.ApplicantPersonID = this.DriverInfo.PersonID;

            int ApplicationTypeID;
            if (IssueReason == enIssueReason.LostReplacement)
                ApplicationTypeID = 3;
            else if (IssueReason == enIssueReason.DamagedReplacement)
                ApplicationTypeID = 4;
            else
                return null;

            Application.ApplicationTypeID = ApplicationTypeID;
            Application.ApplicationDate = DateTime.Now;
            Application.ApplicationStatus = clsApplication.enApplicationStatus.Completed;
            Application.LastStatusDate = DateTime.Now;
            clsApplicationType ApplicationType =
            clsApplicationType.Find(Application.ApplicationTypeID);
            if (ApplicationType == null)
                return null;

            Application.PaidFees = ApplicationType.Fees;
            Application.CreatedByUserID = CreatedByUserID;

            if (!Application.Save())
                return null;

            clsLicense NewLicense = new clsLicense();

            NewLicense.ApplicationID = Application.ApplicationID;
            NewLicense.DriverID = this.DriverID;
            NewLicense.LicenseClassID = this.LicenseClassID;
            NewLicense.IssueDate = DateTime.Now;
            NewLicense.ExpirationDate =
                DateTime.Now.AddYears(this.LicenseClassInfo.DefaultValidityLength);
            NewLicense.Notes = this.Notes;
            NewLicense.PaidFees = this.PaidFees;
            NewLicense.IsActive = true;
            NewLicense.IssueReason = IssueReason;
            NewLicense.CreatedByUserID = CreatedByUserID;

            if (!NewLicense.Save())
                return null;

            this.IsActive = false;

            if (!this.Save())
                return null;

            return NewLicense;
        }



    }
}