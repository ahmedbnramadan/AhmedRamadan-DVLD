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

        public clsTestType.enTestType TestTypeID { get; set; }
        
        public int TestTypeID { get; set; }
        
        public int LocalDrivingLicenseApplicationID { get; set; }
        
        public DateTime AppointmentDate { get; set; }
        
        public decimal PaidFees { get; set; }
        
        public int CreatedByUserID { get; set; }
        
        public bool IsLocked { get; set; }
        
        public int? RetakeTestApplicationID { get; set; }

        private clsApplication _RetakeTestApplicationInfo { get; set; }

        private clsTestType _TestTypeInfo { get; set; }
        
        private clsUser _CreatedByUserInfo { get; set; }
        
        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplicationInfo { get; set; }

        
        public clsApplication RetakeTestApplicationInfo
        {
            get
            {
                if (_RetakeTestApplicationInfo == null)
                    _RetakeTestApplicationInfo = clsApplication.Find(this.RetakeTestApplicationID);
                return _RetakeTestApplicationInfo;
            }
        }
        
        public clsTestType TestTypeInfo
        {
            get
            {
                if (_TestTypeInfo == null)
                    _TestTypeInfo = clsTestType.Find(this.TestTypeID);
                return _TestTypeInfo;
            }
        }

        public clsUser CreatedByUserInfo
        {
            get
            {
                if (_CreatedByUserInfo == null)
                    _CreatedByUserInfo = clsUser.Find(this.CreatedByUserID);
                return _CreatedByUserInfo;
            }
        }

        public clsLocalDrivingLicenseApplication LocalDrivingLicenseApplicationInfo
        {
            get
            {
                if (_LocalDrivingLicenseApplicationInfo == null)
                    _LocalDrivingLicenseApplicationInfo = clsLocalDrivingLicenseApplication.Find(this.LocalDrivingLicenseApplicationID);
                return _LocalDrivingLicenseApplicationInfo;
            }
        }

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


        public static DataTable GetApplicationAppointmentsPerTestType(
            int LocalDrivingLicenseApplicationID,
            clsTestType.enTestType TestType
        )
        {
            return DataAccess.clsTestAppointments.GetAppointmentsForTest(
                LocalDrivingLicenseApplicationID,
                (int)TestType
            );
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

        public static clsTestAppointment GetLastTestAppointment(
            int LocalDrivingLicenseApplicationID,
            clsTestType.enTestType TestType
        )
        {
            int testAppointmentID = -1;
            DateTime appointmentDate = DateTime.MinValue;
            decimal paidFees = 0;
            int createdByUserID = -1;
            bool isLocked = false;
            int? retakeTestApplicationID = null;

            if (DataAccess.clsTestAppointments.GetLastTestAppointment(
                ref testAppointmentID,
                (int)TestType,
                LocalDrivingLicenseApplicationID,
                ref appointmentDate,
                ref paidFees,
                ref createdByUserID,
                ref isLocked,
                ref retakeTestApplicationID
            ))
            {
                return new clsTestAppointment(
                    testAppointmentID,
                    LocalDrivingLicenseApplicationID,
                    (int)TestType,
                    appointmentDate,
                    paidFees,
                    createdByUserID,
                    isLocked,
                    retakeTestApplicationID
                );
            }

            return null;
        }

        public bool Lock()
        {
            return DataAccess.clsTestAppointments.LockAppointment(
                this.TestAppointmentID
            );
        }

        private int _GetTestID()
        {
            return clsTestAppointment.GetTestID(this.TestAppointmentID);
        }
        
    }
}