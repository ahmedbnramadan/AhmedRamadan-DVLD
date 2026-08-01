using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsTests
    {
        public static string LastErrorMessage = "";

        public static int AddNewTest(
            int TestAppointmentID,
            bool TestResult,
            string Notes,
            int CreatedByUserID
            )
        {
            int TestID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO tests 
                            (testappointmentid, testresult, notes, createdbyuserid)
                            VALUES 
                            (@TestAppointmentID, @TestResult, @Notes, @CreatedByUserID);
                            UPDATE testappointments
                            SET islocked = 1 where testappointmentid = @TestAppointmentID

                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                    command.Parameters.AddWithValue("@TestResult", TestResult);
                    
                    if (string.IsNullOrEmpty(Notes))
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Notes", Notes);
                    
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            TestID = InsertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding test: " + ex.Message;
                    }
                }
            }
            return TestID;
        }

        public static bool IsTestExist(int TestID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM tests WHERE testid = @TestID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestID", TestID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking test existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsTestTaken(int TestAppointmentID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM tests WHERE testappointmentid = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if test is taken: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetTestResultByAppointmentID(int TestAppointmentID, ref bool TestResult)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT testresult FROM tests WHERE testappointmentid = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            isFound = true;
                            TestResult = (bool)result;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test result: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteTest(int TestID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM tests WHERE testid = @TestID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestID", TestID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting test: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool DeleteTestByAppointmentID(int TestAppointmentID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM tests WHERE testappointmentid = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting test by appointment ID: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetTestByID(
            int TestID,
            ref int TestAppointmentID,
            ref bool TestResult,
            ref string Notes,
            ref int CreatedByUserID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM tests WHERE testid = @TestID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestID", TestID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                TestAppointmentID = (int)reader["testappointmentid"];
                                TestResult = (bool)reader["testresult"];
                                Notes = reader["notes"] != DBNull.Value ? (string)reader["notes"] : "";
                                CreatedByUserID = (int)reader["createdbyuserid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetTestByAppointmentID(
            int TestAppointmentID,
            ref int TestID,
            ref bool TestResult,
            ref string Notes,
            ref int CreatedByUserID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM tests WHERE testappointmentid = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                TestID = (int)reader["testid"];
                                TestResult = (bool)reader["testresult"];
                                Notes = reader["notes"] != DBNull.Value ? (string)reader["notes"] : "";
                                CreatedByUserID = (int)reader["createdbyuserid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test by appointment ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetTestsByAppointmentID(int TestAppointmentID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT t.*, 
                                        u.username as createdbyusername
                                FROM tests t
                                INNER JOIN users u ON t.createdbyuserid = u.userid
                                WHERE t.testappointmentid = @TestAppointmentID
                                ORDER BY t.testid DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting tests by appointment ID: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetTestsByLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT t.*, 
                                        ta.testtypeid,
                                        tt.testtypetitle,
                                        u.username as createdbyusername,
                                        ta.appointmentdate
                                FROM tests t
                                INNER JOIN testappointments ta ON t.testappointmentid = ta.testappointmentid
                                INNER JOIN testtypes tt ON ta.testtypeid = tt.testtypeid
                                INNER JOIN users u ON t.createdbyuserid = u.userid
                                WHERE ta.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID
                                ORDER BY ta.testtypeid, t.testid DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting tests by application: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetAllTests()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT t.*, 
                                        ta.testtypeid,
                                        tt.testtypetitle,
                                        ta.localdrivinglicenseapplicationid,
                                        ta.appointmentdate,
                                        u.username as createdbyusername,
                                        a.applicantpersonid,
                                        p.firstname + ' ' + p.lastname as applicantname
                                FROM tests t
                                INNER JOIN testappointments ta ON t.testappointmentid = ta.testappointmentid
                                INNER JOIN testtypes tt ON ta.testtypeid = tt.testtypeid
                                INNER JOIN users u ON t.createdbyuserid = u.userid
                                INNER JOIN localdrivinglicenseapplications lda ON ta.localdrivinglicenseapplicationid = lda.localdrivinglicenseapplicationid
                                INNER JOIN applications a ON lda.applicationid = a.applicationid
                                INNER JOIN people p ON a.applicantpersonid = p.personid
                                ORDER BY t.testid DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting all tests: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetTestsByDateRange(DateTime StartDate, DateTime EndDate)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT t.*, 
                                        ta.testtypeid,
                                        tt.testtypetitle,
                                        ta.localdrivinglicenseapplicationid,
                                        ta.appointmentdate,
                                        u.username as createdbyusername
                                FROM tests t
                                INNER JOIN testappointments ta ON t.testappointmentid = ta.testappointmentid
                                INNER JOIN testtypes tt ON ta.testtypeid = tt.testtypeid
                                INNER JOIN users u ON t.createdbyuserid = u.userid
                                WHERE ta.appointmentdate BETWEEN @StartDate AND @EndDate
                                ORDER BY ta.appointmentdate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", StartDate);
                    command.Parameters.AddWithValue("@EndDate", EndDate);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting tests by date range: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateTest(
            int TestID,
            int TestAppointmentID,
            bool TestResult,
            string Notes,
            int CreatedByUserID
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE tests
                SET 
                    testappointmentid = @TestAppointmentID,
                    testresult = @TestResult,
                    notes = @Notes,
                    createdbyuserid = @CreatedByUserID
                WHERE testid = @TestID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestID", TestID);
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                    command.Parameters.AddWithValue("@TestResult", TestResult);
                    
                    if (string.IsNullOrEmpty(Notes))
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Notes", Notes);
                    
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating test: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Update test result only
        public static bool UpdateTestResult(int TestID, bool NewResult, string NewNotes)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE tests
                SET testresult = @TestResult,
                    notes = @Notes
                WHERE testid = @TestID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestID", TestID);
                    command.Parameters.AddWithValue("@TestResult", NewResult);
                    
                    if (string.IsNullOrEmpty(NewNotes))
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Notes", NewNotes);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating test result: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get pass/fail statistics for a test type
        public static void GetTestTypeStatistics(int TestTypeID, ref int TotalTests, ref int PassedTests, ref int FailedTests)
        {
            TotalTests = 0;
            PassedTests = 0;
            FailedTests = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
                                    COUNT(*) as TotalTests,
                                    SUM(CASE WHEN testresult = 1 THEN 1 ELSE 0 END) as PassedTests,
                                    SUM(CASE WHEN testresult = 0 THEN 1 ELSE 0 END) as FailedTests
                                FROM tests t
                                INNER JOIN testappointments ta ON t.testappointmentid = ta.testappointmentid
                                WHERE ta.testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TotalTests = reader["TotalTests"] != DBNull.Value ? Convert.ToInt32(reader["TotalTests"]) : 0;
                                PassedTests = reader["PassedTests"] != DBNull.Value ? Convert.ToInt32(reader["PassedTests"]) : 0;
                                FailedTests = reader["FailedTests"] != DBNull.Value ? Convert.ToInt32(reader["FailedTests"]) : 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test type statistics: " + ex.Message;
                    }
                }
            }
        }

        // Get pass rate for a test type
        public static decimal GetTestTypePassRate(int TestTypeID)
        {
            decimal passRate = 0;
            LastErrorMessage = "";

            int totalTests = 0, passedTests = 0, failedTests = 0;
            GetTestTypeStatistics(TestTypeID, ref totalTests, ref passedTests, ref failedTests);

            if (totalTests > 0)
            {
                passRate = (decimal)passedTests / totalTests * 100;
            }

            return passRate;
        }

        // Get latest test result for an application and test type
        public static bool GetLatestTestResult(int LocalDrivingLicenseApplicationID, int TestTypeID, ref bool TestResult, ref DateTime TestDate)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TOP 1 t.testresult, ta.appointmentdate
                                FROM tests t
                                INNER JOIN testappointments ta ON t.testappointmentid = ta.testappointmentid
                                WHERE ta.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID 
                                AND ta.testtypeid = @TestTypeID
                                ORDER BY t.testid DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                TestResult = (bool)reader["testresult"];
                                TestDate = (DateTime)reader["appointmentdate"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting latest test result: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        // Count tests for a specific application and test type
        public static int CountTestsFor(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            int count = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT COUNT(*)
                                FROM tests t
                                INNER JOIN testappointments ta ON t.testappointmentid = ta.testappointmentid
                                WHERE ta.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID 
                                AND ta.testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int countValue))
                        {
                            count = countValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error counting tests: " + ex.Message;
                    }
                }
            }
            return count;
        }

        // Check if a test type is passed for an application
        public static bool IsTestTypePassed(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            bool isPassed = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TOP 1 t.testresult
                                FROM tests t
                                INNER JOIN testappointments ta ON t.testappointmentid = ta.testappointmentid
                                WHERE ta.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID 
                                AND ta.testtypeid = @TestTypeID
                                ORDER BY t.testid DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            isPassed = (bool)result;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if test type is passed: " + ex.Message;
                    }
                }
            }
            return isPassed;
        }

        
        // Get the last test by person and test type and license class
        public static bool GetLastTestByPersonAndTestTypeAndLicenseClass(
            int PersonID,
            int TestTypeID,
            int LicenseClassID,
            ref int TestID,
            ref int TestAppointmentID,
            ref bool TestResult,
            ref string Notes,
            ref int CreatedByUserID,
            ref DateTime AppointmentDate)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TOP 1 t.TestID, t.TestAppointmentID, t.TestResult, t.Notes, t.CreatedByUserID, ta.AppointmentDate
                                FROM Tests t
                                INNER JOIN TestAppointments ta ON t.TestAppointmentID = ta.TestAppointmentID
                                INNER JOIN LocalDrivingLicenseApplications lda ON ta.LocalDrivingLicenseApplicationID = lda.LocalDrivingLicenseApplicationID
                                INNER JOIN Applications a ON lda.ApplicationID = a.ApplicationID
                                WHERE a.ApplicantPersonID = @PersonID
                                AND ta.TestTypeID = @TestTypeID
                                AND lda.LicenseClassID = @LicenseClassID
                                ORDER BY ta.AppointmentDate DESC, t.TestID DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                TestID = (int)reader["TestID"];
                                TestAppointmentID = (int)reader["TestAppointmentID"];
                                TestResult = (bool)reader["TestResult"];
                                Notes = reader["Notes"] != DBNull.Value ? (string)reader["Notes"] : "";
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                                AppointmentDate = (DateTime)reader["AppointmentDate"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting last test by person and test type and license class: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        
    }
}