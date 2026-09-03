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
                    _LicenseClassInfo = clsLicenseClass.Find(this.LicenseClassID);
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

        private bool _ValidateAddNew()
        {
            if (DoesPersonHaveActiveApplication(
                this.ApplicantPersonID,
                this.LicenseClassID))
            {
                return false;
            }

            if (clsLicense.DoesPersonHaveActiveLicense(
                this.ApplicantPersonID,
                this.LicenseClassID))
            {
                return false;
            }

            return true;
        }

        public override bool Save()
        {
            if (Mode == enMode.AddNew)
            {
                if (!_ValidateAddNew())
                {
                    return false;
                }
            }

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
                    return false;

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

        // DoesPassTestType
        public static bool DoesPassTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            return DataAccess.clsLocalDrivingLicenseApplications.DoesPassTestType(
                LocalDrivingLicenseApplicationID, 
                TestTypeID
            );
        }

        // DoesAttendTestType
        public static bool DoesAttendTestType(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            return DataAccess.clsLocalDrivingLicenseApplications.DoesAttendTestType(
                LocalDrivingLicenseApplicationID, 
                TestTypeID
            );
        }

        // TotalTrialsPerTest
        public static int TotalTrialsPerTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            return DataAccess.clsLocalDrivingLicenseApplications.TotalTrialsPerTest(
                LocalDrivingLicenseApplicationID, 
                TestTypeID
            );
        }

        // IsThereAnActiveScheduledTest
        public static bool IsThereAnActiveScheduledTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            return DataAccess.clsLocalDrivingLicenseApplications.IsThereAnActiveScheduledTest(
                LocalDrivingLicenseApplicationID, 
                TestTypeID
            );
        }

        // Get Active Test Appointment ID
        public static int GetActiveTestAppointmentID(int LocalDrivingLicenseApplicationID)
        {
            return DataAccess.clsLocalDrivingLicenseApplications.GetActiveTestAppointmentID(
                LocalDrivingLicenseApplicationID
            );
        }

        // IsAllTestsPassed
        public bool IsAllTestsPassed()
        {
            return (this.GetPassedTestCount() >= 3);
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

        public static bool DoesPersonHaveActiveLicense(int PersonID, int LicenseClassID)
        {
            return clsLicense.GetActiveLicenseIDByPersonID(PersonID, LicenseClassID) != -1;
        }

        public bool DoesPersonHaveActiveApplication()
        {
            return DoesPersonHaveActiveApplication(
                this.ApplicantPersonID,
                this.LicenseClassID);
        }

        public bool DoesPersonHaveActiveLicense()
        {
            return clsLicense.DoesPersonHaveActiveLicense(
                this.ApplicantPersonID,
                this.LicenseClassID);
        }

        public int IssueLicenseFrotTheFristTeim(string Notes, int CreatedByUserID)
        {
            // The application must be a saved local driving license application.
            if (this.LocalDrivingLicenseApplicationID <= 0 || this.ApplicationID <= 0)
                return -1;

            // This method is only for issuing a brand-new local driving license.
            if (this.ApplicationTypeID != 1)
                return -1;

            // The application must still be new and all required tests must be passed.
            if (this.ApplicationStatus != clsApplication.enApplicationStatus.New)
                return -1;

            if (!this.IsAllTestsPassed())
                return -1;

            // Do not issue another active license for the same person and license class.
            if (this.DoesPersonHaveActiveLicense())
                return -1;

            // The license class must exist and have a valid validity period.
            clsLicenseClass LicenseClass = this.LicenseClassInfo;

            if (LicenseClass == null || LicenseClass.DefaultValidityLength <= 0)
                return -1;

            // A person becomes a driver when their first license is issued.
            clsDriver Driver = clsDriver.FindByPersonID(this.ApplicantPersonID);

            if (Driver == null)
            {
                Driver = new clsDriver();

                Driver.PersonID = this.ApplicantPersonID;
                Driver.CreatedByUserID = CreatedByUserID;

                if (!Driver.Save())
                    return -1;
            }

            // Create the new license.
            DateTime IssueDate = DateTime.Now;

            clsLicense License = new clsLicense();

            License.ApplicationID = this.ApplicationID;
            License.DriverID = Driver.ID;
            License.LicenseClassID = this.LicenseClassID;
            License.IssueDate = IssueDate;
            License.ExpirationDate =
                IssueDate.AddYears(LicenseClass.DefaultValidityLength);
            License.Notes = Notes;
            License.PaidFees = LicenseClass.Fees;
            License.IsActive = true;
            License.IssueReason = clsLicense.enIssueReason.FirstTime;
            License.CreatedByUserID = CreatedByUserID;

            // The application must only be completed after the license is saved.
            if (!License.Save())
                return -1;

            // Complete the application.
            if (!this.SetComplete())
                return -1;

            // Keep the current object synchronized with the database.
            this.ApplicationStatus =
                clsApplication.enApplicationStatus.Completed;

            this.LastStatusDate = DateTime.Now;

            return License.ID;
        }



    }
}