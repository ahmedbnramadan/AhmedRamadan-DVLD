using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsApplication
    {
        public enum enMode { AddNew = 0, Update = 1 };

        public enum enApplicationStatus
        {
            New = 1,
            Cancelled = 2,
            Completed = 3
        };

        public enMode Mode = enMode.AddNew;

        public int ApplicationID { get; set; }
        public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }
        public enApplicationStatus ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }

        private clsPerson _PersonInfo;
        private clsUser _UserInfo;
        private clsApplicationType _ApplicaitonTypeInfo;

        public clsPerson PersonInfo
        {
            get
            {
                if (_PersonInfo == null)
                    _PersonInfo = clsPerson.Find(this.ApplicantPersonID);
                return _PersonInfo;
            }
        }

        public clsUser UserInfo
        {
            get
            {
                if (_UserInfo == null)
                    _UserInfo = clsUser.Find(this.CreatedByUserID);
                return _UserInfo;
            }
        }

        public clsApplicationType ApplicationTypeInfo
        {
            get
            {
                if (_ApplicaitonTypeInfo == null)
                    _ApplicaitonTypeInfo = clsApplicationType.Find(this.ApplicationTypeID);
                return _ApplicaitonTypeInfo;
            }
        }
        public clsApplication()
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this.ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = -1;
            this.ApplicationStatus = enApplicationStatus.New;
            this.LastStatusDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;

            this.Mode = enMode.AddNew;
        }

        private clsApplication(
            int ApplicationID,
            int ApplicantPersonID,
            DateTime ApplicationDate,
            int ApplicationTypeID,
            enApplicationStatus ApplicationStatus,
            DateTime LastStatusDate,
            decimal PaidFees,
            int CreatedByUserID)
        {
            this.ApplicationID     = ApplicationID;
            this.ApplicantPersonID = ApplicantPersonID;
            this.ApplicationDate   = ApplicationDate;
            this.ApplicationTypeID = ApplicationTypeID;
            this.ApplicationStatus = ApplicationStatus;
            this.LastStatusDate    = LastStatusDate;
            this.PaidFees          = PaidFees;
            this.CreatedByUserID   = CreatedByUserID;


            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.ApplicationID = DataAccess.clsApplications.AddNewApplication(
                this.ApplicantPersonID,
                this.ApplicationDate,
                this.ApplicationTypeID,
                (byte)this.ApplicationStatus,
                this.LastStatusDate,
                this.PaidFees,
                this.CreatedByUserID);

            return (this.ApplicationID != -1);
        }

        private bool _Update()
        {
            return DataAccess.clsApplications.UpdateApplication(
                this.ApplicationID,
                this.ApplicantPersonID,
                this.ApplicationDate,
                this.ApplicationTypeID,
                (byte)this.ApplicationStatus,
                this.LastStatusDate,
                this.PaidFees,
                this.CreatedByUserID);
        }

        public static clsApplication FindBaseApplication(int ApplicationID)
        {
            int ApplicantPersonID = -1;
            DateTime ApplicationDate = DateTime.Now;
            int ApplicationTypeID = -1;
            short Status = 1;
            DateTime LastStatusDate = DateTime.Now;
            decimal PaidFees = 0;
            int CreatedByUserID = -1;

            if (DataAccess.clsApplications.GetApplicationByID(
                    ApplicationID,
                    ref ApplicantPersonID,
                    ref ApplicationDate,
                    ref ApplicationTypeID,
                    ref Status,
                    ref LastStatusDate,
                    ref PaidFees,
                    ref CreatedByUserID))
            {
                return new clsApplication(
                    ApplicationID,
                    ApplicantPersonID,
                    ApplicationDate,
                    ApplicationTypeID,
                    (enApplicationStatus)Status,
                    LastStatusDate,
                    PaidFees,
                    CreatedByUserID);
            }
            else
            {
                return null;
            }
        }

        public virtual bool Save()
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

        public virtual bool Delete()
        {
            return DataAccess.clsApplications.DeleteApplication(this.ApplicationID);
        }

        public bool Cancel()
        {
            return DataAccess.clsApplications.CancelApplication(this.ApplicationID);
        }

        public bool SetComplete()
        {
            return DataAccess.clsApplications.SetApplicationComplete(this.ApplicationID);
        }

        public static DataTable GetAllApplications()
        {
            return DataAccess.clsApplications.GetAllApplications();
        }

        public static bool IsApplicationExist(int ApplicationID)
        {
            return DataAccess.clsApplications.IsApplicationExist(ApplicationID);
        }

        public static bool DoesPerosnHaveActiveApplication(int PersonID)
        {
            return DataAccess.clsApplications.DoesPersonHaveActiveApplication(PersonID);
        }

        public static bool DoesPerosnHaveActiveApplication(int PersonID, int ApplicationTypeID)
        {
            return DataAccess.clsApplications.DoesPersonHaveActiveApplication(PersonID, ApplicationTypeID);
        }

        public static int GetActiveApplicationIDForLicenseClass(int PersonID, int ApplicationTypeID, int LicenseClassID)
        {
            return DataAccess.clsApplications.GetActiveApplicationIDForLicenseClass(
                PersonID, 
                ApplicationTypeID, 
                LicenseClassID);
        }
    }
}