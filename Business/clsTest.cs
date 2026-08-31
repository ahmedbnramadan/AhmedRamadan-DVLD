using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsTest
    {
        public enum enMode
        {
            AddNew = 0,
            Update = 1
        };

        public enMode Mode = enMode.AddNew;

        public int TestID { get; set; }

        public int TestAppointmentID { get; set; }

        public bool TestResult { get; set; }

        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }

        private clsTestAppointment _TestAppointmentInfo;

        private clsUser _CreatedByUserInfo { get; set; }

        public clsTestAppointment TestAppointmentInfo
        {
            get
            {
                if(_TestAppointmentInfo == null)
                    _TestAppointmentInfo = clsTestAppointment.Find(this.TestAppointmentID);

                return _TestAppointmentInfo;
            }
        }

        public clsUser CreatedByUserInfo
        {
            get
            {
                if(_CreatedByUserInfo == null)
                    _CreatedByUserInfo = clsUser.Find(this.CreatedByUserID);

                return _CreatedByUserInfo;
            }
        }
      
        public clsTest()
        {
            this.TestID = -1;
            this.TestAppointmentID = -1;
            this.TestResult = false;
            this.Notes = "";
            this.CreatedByUserID = -1;

            this.Mode = enMode.AddNew;
        }

        private clsTest(
            int TestID,
            int TestAppointmentID,
            bool TestResult,
            string Notes,
            int CreatedByUserID
        )
        {
            this.TestID = TestID;
            this.TestAppointmentID = TestAppointmentID;
            this.TestResult = TestResult;
            this.Notes = Notes;
            this.CreatedByUserID = CreatedByUserID;

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.TestID = DataAccess.clsTests.AddNewTest(
                this.TestAppointmentID,
                this.TestResult,
                this.Notes,
                this.CreatedByUserID
            );

            return (this.TestID != -1);
        }

        private bool _Update()
        {
            return DataAccess.clsTests.UpdateTest(
                this.TestID,
                this.TestAppointmentID,
                this.TestResult,
                this.Notes,
                this.CreatedByUserID
            );
        }

        public static clsTest Find(int TestID)
        {
            int TestAppointmentID = -1;
            bool TestResult = false;
            string Notes = "";
            int CreatedByUserID = -1;

            if (DataAccess.clsTests.GetTestByID(
                    TestID,
                    ref TestAppointmentID,
                    ref TestResult,
                    ref Notes,
                    ref CreatedByUserID
                ))
            {
                return new clsTest(
                    TestID,
                    TestAppointmentID,
                    TestResult,
                    Notes,
                    CreatedByUserID
                );
            }
            else
            {
                return null;
            }
        }

        public static clsTest FindByAppointmentID(int TestAppointmentID)
        {
            int TestID = -1;
            bool TestResult = false;
            string Notes = "";
            int CreatedByUserID = -1;

            if (DataAccess.clsTests.GetTestByAppointmentID(
                    TestAppointmentID,
                    ref TestID,
                    ref TestResult,
                    ref Notes,
                    ref CreatedByUserID
                ))
            {
                return new clsTest(
                    TestID,
                    TestAppointmentID,
                    TestResult,
                    Notes,
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

        public static DataTable GetAllTests()
        {
            return DataAccess.clsTests.GetAllTests();
        }

        public static byte GetPassedTestCount(int LocalDrivingLicenseApplicationID)
        {
            byte Count = 0;

            // فحص اختبار النظر
            if (DataAccess.clsTests.IsTestTypePassed(
                    LocalDrivingLicenseApplicationID,
                    1
                ))
            {
                Count++;
            }

            // فحص اختبار الكتابة
            if (DataAccess.clsTests.IsTestTypePassed(
                    LocalDrivingLicenseApplicationID,
                    2
                ))
            {
                Count++;
            }

            // فحص اختبار القيادة العملي
            if (DataAccess.clsTests.IsTestTypePassed(
                    LocalDrivingLicenseApplicationID,
                    3
                ))
            {
                Count++;
            }

            return Count;
        }

        public static bool IsPassed(
            int LocalDrivingLicenseApplicationID,
            int TestTypeID
        )
        {
            return DataAccess.clsTests.IsTestTypePassed(
                LocalDrivingLicenseApplicationID,
                TestTypeID
            );
        }

        public static bool IsPassedAllTests(int LocalDrivingLicenseApplicationID)
        {
            return GetPassedTestCount(LocalDrivingLicenseApplicationID) == 3;
        }
        
        public static clsTest GetLastTestByPersonAndTestTypeAndLicenseClass(
            int PersonID,
            int LicenseClassID,
            clsTestType.enTestType TestTypeID)
        {
            int TestID = -1;
            int TestAppointmentID = -1;
            bool TestResult = false;
            string Notes = "";
            int CreatedByUserID = -1;
            DateTime AppointmentDate = DateTime.MinValue;

            if (DataAccess.clsTests.GetLastTestByPersonAndTestTypeAndLicenseClass(
                PersonID,
                (int)TestTypeID,
                LicenseClassID,
                ref TestID,
                ref TestAppointmentID,
                ref TestResult,
                ref Notes,
                ref CreatedByUserID,
                ref AppointmentDate))
            {
                return new clsTest(
                    TestID,
                    TestAppointmentID,
                    TestResult,
                    Notes,
                    CreatedByUserID);
            }

            return null;
        }

        
    }
}