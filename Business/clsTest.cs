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

        public clsTestAppointment TestAppointmentInfo { get; set; }

        public clsUser CreatedByUserInfo { get; set; }

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

            this.TestAppointmentInfo = clsTestAppointment.Find(
                this.TestAppointmentID
            );

            this.CreatedByUserInfo = clsUser.Find(
                this.CreatedByUserID
            );

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

        
        public static bool GetLastTestByPersonAndTestTypeAndLicenseClass(
            int PersonID,
            int TestTypeID,
            int LicenseClassID,
            ref int TestID,
            ref int TestAppointmentID,
            ref bool TestResult,
            ref string Notes,
            ref int CreatedByUserID,
            ref DateTime AppointmentDate
        )
        {
            return DataAccess.clsTests.GetLastTestByPersonAndTestTypeAndLicenseClass(
                PersonID,
                TestTypeID,
                LicenseClassID,
                ref TestID,
                ref TestAppointmentID,
                ref TestResult,
                ref Notes,
                ref CreatedByUserID,
                ref AppointmentDate
            );
        }

        
    }
}