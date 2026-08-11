using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsTestAppointment
    {
        public enum enMode 
        { 
            AddNew = 0, 
            Update = 1 
        };

        public enMode Mode = enMode.AddNew;

        public int TestAppointmentID { get; set; }
        
        public int TestTypeID { get; set; }
        
        public int LocalDrivingLicenseApplicationID { get; set; }
        
        public DateTime AppointmentDate { get; set; }
        
        public decimal PaidFees { get; set; }
        
        public int CreatedByUserID { get; set; }
        
        public bool IsLocked { get; set; }
        
        public int? RetakeTestApplicationID { get; set; }

        public clsTestType TestTypeInfo { get; set; }
        
        public clsUser CreatedByUserInfo { get; set; }
        
        public clsLocalDrivingLicenseApplication LocalDrivingLicenseApplicationInfo { get; set; }

        public clsTestAppointment()
        {
            this.TestAppointmentID = -1;
            this.TestTypeID = -1;
            this.LocalDrivingLicenseApplicationID = -1;
            this.AppointmentDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;
            this.IsLocked = false;
            this.RetakeTestApplicationID = null;

            this.Mode = enMode.AddNew;
        }

        private clsTestAppointment(
            int TestAppointmentID,
            int TestTypeID,
            int LocalDrivingLicenseApplicationID,
            DateTime AppointmentDate,
            decimal PaidFees,
            int CreatedByUserID,
            bool IsLocked,
            int? RetakeTestApplicationID
        )
        {
            this.TestAppointmentID = TestAppointmentID;
            this.TestTypeID = TestTypeID;
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            this.AppointmentDate = AppointmentDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsLocked = IsLocked;
            this.RetakeTestApplicationID = RetakeTestApplicationID;

            this.TestTypeInfo = clsTestType.Find(
                this.TestTypeID
            );

            this.CreatedByUserInfo = clsUser.Find(
                this.CreatedByUserID
            );

            this.LocalDrivingLicenseApplicationInfo = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppID(
                this.LocalDrivingLicenseApplicationID
            );

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.TestAppointmentID = DataAccess.clsTestAppointments.AddNewTestAppointment(
                this.TestTypeID,
                this.LocalDrivingLicenseApplicationID,
                this.AppointmentDate,
                this.PaidFees,
                this.CreatedByUserID,
                this.IsLocked,
                this.RetakeTestApplicationID
            );

            return (this.TestAppointmentID != -1);
        }

        private bool _Update()
        {
            return DataAccess.clsTestAppointments.UpdateTestAppointment(
                this.TestAppointmentID,
                this.TestTypeID,
                this.LocalDrivingLicenseApplicationID,
                this.AppointmentDate,
                this.PaidFees,
                this.CreatedByUserID,
                this.IsLocked,
                this.RetakeTestApplicationID
            );
        }

        public static clsTestAppointment Find(int TestAppointmentID)
        {
            int TestTypeID = -1;
            int LocalDrivingLicenseApplicationID = -1;
            DateTime AppointmentDate = DateTime.Now;
            decimal PaidFees = 0;
            int CreatedByUserID = -1;
            bool IsLocked = false;
            int? RetakeTestApplicationID = null;

            if (DataAccess.clsTestAppointments.GetTestAppointmentByID(
                    TestAppointmentID,
                    ref TestTypeID,
                    ref LocalDrivingLicenseApplicationID,
                    ref AppointmentDate,
                    ref PaidFees,
                    ref CreatedByUserID,
                    ref IsLocked,
                    ref RetakeTestApplicationID
                ))
            {
                return new clsTestAppointment(
                    TestAppointmentID,
                    TestTypeID,
                    LocalDrivingLicenseApplicationID,
                    AppointmentDate,
                    PaidFees,
                    CreatedByUserID,
                    IsLocked,
                    RetakeTestApplicationID
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

        public static DataTable GetAllTestAppointments()
        {
            return DataAccess.clsTestAppointments.GetAllTestAppointments();
        }

        public static DataTable GetApplicationAppointmentsPerTestType(
            int LocalDrivingLicenseApplicationID, 
            int TestTypeID
        )
        {
            return DataAccess.clsTestAppointments.GetAppointmentsForTest(
                LocalDrivingLicenseApplicationID, 
                TestTypeID
            );
        }

        public bool Lock()
        {
            return DataAccess.clsTestAppointments.LockAppointment(
                this.TestAppointmentID
            );
        }
    }
}