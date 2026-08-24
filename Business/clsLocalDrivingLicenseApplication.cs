using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsLocalDrivingLicenseApplication : clsApplication
    {
        public new enum enMode 
        { 
            AddNew = 0, 
            Update = 1 
        };

        public new enMode Mode = enMode.AddNew;

        public int LocalDrivingLicenseApplicationID { get; set; }
        
        public int LicenseClassID { get; set; }
        private clsLicenseClass _LicenseClassInfo;
        public clsLicenseClass LicenseClassInfo
        {
            get
            {
                if (_LicenseClassInfo == null)
                    _LicenseClassInfo = clsLicenceClass.Find(this.LicenseClassID);
                return _LicenseClassInfo;
            }
        }

        public string PersonFullName
        {
            get
            {
                return base.PersonInfo.FullName;
            }
        }

        public clsLocalDrivingLicenseApplication()
        {
            this.LocalDrivingLicenseApplicationID = -1;
            this.LicenseClassID = -1;
            
            this.Mode = enMode.AddNew;
        }

        private clsLocalDrivingLicenseApplication(
            int LocalDrivingLicenseApplicationID,
            int ApplicationID,
            int ApplicantPersonID,
            DateTime ApplicationDate,
            int ApplicationTypeID,
            clsApplication.enApplicationStatus ApplicationStatus,
            DateTime LastStatusDate,
            decimal PaidFees,
            int CreatedByUserID,
            int LicenseClassID
        )
        {
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            this.ApplicationID          = ApplicationID;
            this.ApplicantPersonID      = ApplicantPersonID;
            this.ApplicationDate        = ApplicationDate;
            this.ApplicationTypeID      = ApplicationTypeID;
            this.ApplicationStatus      = ApplicationStatus;
            this.LastStatusDate         = LastStatusDate;
            this.PaidFees               = PaidFees;
            this.CreatedByUserID        = CreatedByUserID;
            this.LicenseClassID         = LicenseClassID;

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.LocalDrivingLicenseApplicationID = DataAccess.clsLocalDrivingLicenseApplications.AddNewLocalDrivingLicenseApplication(
                this.ApplicationID,
                this.LicenseClassID
            );

            return (this.LocalDrivingLicenseApplicationID != -1);
        }

        private bool _Update()
        {
            return DataAccess.clsLocalDrivingLicenseApplications.UpdateLocalDrivingLicenseApplication(
                this.LocalDrivingLicenseApplicationID,
                this.ApplicationID,
                this.LicenseClassID
            );
        }

        public static clsLocalDrivingLicenseApplication FindByLocalDrivingAppID(int LocalDrivingLicenseApplicationID)
        {
            int ApplicationID = -1;
            int LicenseClassID = -1;

            if (DataAccess.clsLocalDrivingLicenseApplications.GetLocalDrivingLicenseApplicationByID(
                    LocalDrivingLicenseApplicationID,
                    ref ApplicationID,
                    ref LicenseClassID
                ))
            {
                clsApplication BaseApplication = clsApplication.FindBaseApplication(
                    ApplicationID
                );

                return new clsLocalDrivingLicenseApplication(
                    LocalDrivingLicenseApplicationID,
                    BaseApplication.ApplicationID,
                    BaseApplication.ApplicantPersonID,
                    BaseApplication.ApplicationDate,
                    BaseApplication.ApplicationTypeID,
                    BaseApplication.ApplicationStatus,
                    BaseApplication.LastStatusDate,
                    BaseApplication.PaidFees,
                    BaseApplication.CreatedByUserID,
                    LicenseClassID
                );
            }
            else
            {
                return null;
            }
        }

        public static clsLocalDrivingLicenseApplication FindByApplicationID(int ApplicationID)
        {
            int LocalDrivingLicenseApplicationID = -1;
            int LicenseClassID = -1;

            if (DataAccess.clsLocalDrivingLicenseApplications.GetLocalDrivingLicenseApplicationByApplicationID(
                    ApplicationID,
                    ref LocalDrivingLicenseApplicationID,
                    ref LicenseClassID
                ))
            {
                clsApplication BaseApplication = clsApplication.FindBaseApplication(
                    ApplicationID
                );

                return new clsLocalDrivingLicenseApplication(
                    LocalDrivingLicenseApplicationID,
                    BaseApplication.ApplicationID,
                    BaseApplication.ApplicantPersonID,
                    BaseApplication.ApplicationDate,
                    BaseApplication.ApplicationTypeID,
                    BaseApplication.ApplicationStatus,
                    BaseApplication.LastStatusDate,
                    BaseApplication.PaidFees,
                    BaseApplication.CreatedByUserID,
                    LicenseClassID
                );
            }
            else
            {
                return null;
            }
        }

        public override bool Save()
        {
            base.Mode = (clsApplication.enMode)this.Mode;
            if (!base.Save())
            {
                return false;
            }

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNew())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _Update();
            }

            return false;
        }

        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            return DataAccess.clsLocalDrivingLicenseApplications.GetAllLocalDrivingLicenseApplications();
        }

        public override bool Delete()
        {
            bool IsLocalAppDeleted = DataAccess.clsLocalDrivingLicenseApplications.DeleteLocalDrivingLicenseApplication(
                this.LocalDrivingLicenseApplicationID
            );

            if (!IsLocalAppDeleted)
            {
                return false;
            }

            return base.Delete();
        }

        public static bool DoesPersonHaveActiveApplication(int PersonID, int LicenseClassID)
        {
            return DataAccess.clsLocalDrivingLicenseApplications.DoesPersonHaveActiveApplication(
                PersonID, 
                LicenseClassID
            );
        }

        public byte GetPassedTestCount()
        {
            return clsTest.GetPassedTestCount(
                this.LocalDrivingLicenseApplicationID
            );
        }

        public static bool IsTestPassed(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            return clsTest.IsPassed(
                LocalDrivingLicenseApplicationID, 
                TestTypeID
            );
        }

        public bool IsLicenseIssued()
        {
            return (this.GetActiveLicenseID() != -1);
        }

        public int GetActiveLicenseID()
        {
            return clsLicense.GetActiveLicenseIDByPersonID(
                this.ApplicantPersonID, 
                this.LicenseClassID
            );
        }
    }
}