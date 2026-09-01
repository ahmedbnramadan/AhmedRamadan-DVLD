using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsTestAppointments
    {
        public static string LastErrorMessage = "";

        public static int AddNewTestAppointment(
            int TestTypeID,
            int LocalDrivingLicenseApplicationID,
            DateTime AppointmentDate,
            decimal PaidFees,
            int CreatedByUserID,
            bool IsLocked,
            int? RetakeTestApplicationID
            )
        {
            int TestAppointmentID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO testappointments 
                            (testtypeid, localdrivinglicenseapplicationid, appointmentdate, 
                             paidfees, createdbyuserid, islocked, retaketestapplicationid)
                            VALUES 
                            (@TestTypeID, @LocalDrivingLicenseApplicationID, @AppointmentDate, 
                             @PaidFees, @CreatedByUserID, @IsLocked, @RetakeTestApplicationID);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    command.Parameters.AddWithValue("@IsLocked", IsLocked);

                    if (RetakeTestApplicationID.HasValue)
                        command.Parameters.AddWithValue("@RetakeTestApplicationID", RetakeTestApplicationID.Value);
                    else
                        command.Parameters.AddWithValue("@RetakeTestApplicationID", DBNull.Value);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            TestAppointmentID = InsertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding test appointment: " + ex.Message;
                    }
                }
            }
            return TestAppointmentID;
        }

        // Simplified AddNewTestAppointment for new appointments
        public static int AddNewTestAppointment(
            int TestTypeID,
            int LocalDrivingLicenseApplicationID,
            DateTime AppointmentDate,
            decimal PaidFees,
            int CreatedByUserID
            )
        {
            return AddNewTestAppointment(
                TestTypeID,
                LocalDrivingLicenseApplicationID,
                AppointmentDate,
                PaidFees,
                CreatedByUserID,
                false,  // Not locked by default
                null    // No retake application
                );
        }

        public static bool IsTestAppointmentExist(int TestAppointmentID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM testappointments WHERE testappointmentid = @TestAppointmentID";

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
                        LastErrorMessage = "Error checking test appointment existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsAppointmentLocked(int TestAppointmentID)
        {
            bool isLocked = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT islocked FROM testappointments WHERE testappointmentid = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            isLocked = (bool)result;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if appointment is locked: " + ex.Message;
                    }
                }
            }
            return isLocked;
        }

        public static bool DoesAppointmentExistForTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 FROM testappointments 
                                WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID 
                                AND testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking appointment existence for test: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetActiveAppointmentForTest(
            int LocalDrivingLicenseApplicationID,
            int TestTypeID,
            ref int TestAppointmentID,
            ref DateTime AppointmentDate,
            ref decimal PaidFees,
            ref int CreatedByUserID,
            ref bool IsLocked,
            ref int? RetakeTestApplicationID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TOP 1 * FROM testappointments 
                                WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID 
                                AND testtypeid = @TestTypeID
                                AND islocked = 0
                                AND appointmentdate > GETDATE()
                                ORDER BY appointmentdate";

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

                                TestAppointmentID = (int)reader["testappointmentid"];
                                AppointmentDate = (DateTime)reader["appointmentdate"];
                                PaidFees = (decimal)reader["paidfees"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                                IsLocked = (bool)reader["islocked"];

                                RetakeTestApplicationID = reader["retaketestapplicationid"] != DBNull.Value
                                    ? (int?)reader["retaketestapplicationid"]
                                    : null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting active appointment for test: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteTestAppointment(int TestAppointmentID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM testappointments WHERE testappointmentid = @TestAppointmentID";

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
                        LastErrorMessage = "Error deleting test appointment: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool LockAppointment(int TestAppointmentID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE testappointments
                                SET islocked = 1
                                WHERE testappointmentid = @TestAppointmentID";

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
                        LastErrorMessage = "Error locking appointment: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetTestAppointmentByID(
            int TestAppointmentID,
            ref int TestTypeID,
            ref int LocalDrivingLicenseApplicationID,
            ref DateTime AppointmentDate,
            ref decimal PaidFees,
            ref int CreatedByUserID,
            ref bool IsLocked,
            ref int? RetakeTestApplicationID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM testappointments WHERE testappointmentid = @TestAppointmentID";

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

                                TestTypeID = (int)reader["testtypeid"];
                                LocalDrivingLicenseApplicationID = (int)reader["localdrivinglicenseapplicationid"];
                                AppointmentDate = (DateTime)reader["appointmentdate"];
                                PaidFees = (decimal)reader["paidfees"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                                IsLocked = (bool)reader["islocked"];

                                RetakeTestApplicationID = reader["retaketestapplicationid"] != DBNull.Value
                                    ? (int?)reader["retaketestapplicationid"]
                                    : null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test appointment by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetLastTestAppointment(
            ref int TestAppointmentID,
            int TestTypeID,
            int LocalDrivingLicenseApplicationID,
            ref DateTime AppointmentDate,
            ref decimal PaidFees,
            ref int CreatedByUserID,
            ref bool IsLocked,
            ref int? RetakeTestApplicationID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TOP 1 * FROM testappointments
                                WHERE TestTypeID = @TestTypeID
                                AND LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                ORDER by testappointmentid DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                TestAppointmentID                   = (int)reader["testappointmentid"];
                                AppointmentDate                     = (DateTime)reader["appointmentdate"];
                                PaidFees                            = (decimal)reader["paidfees"];
                                CreatedByUserID                     = (int)reader["createdbyuserid"];
                                IsLocked                            = (bool)reader["islocked"];

                                RetakeTestApplicationID = reader["retaketestapplicationid"] != DBNull.Value
                                    ? (int?)reader["retaketestapplicationid"]
                                    : null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test appointment by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static int GetTestID(int TestAppointmentID)
        {
            int TestID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT TestID
                                FROM Tests
                                WHERE TestAppointmentID = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            TestID = (int)result;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting Test ID: " + ex.Message;
                    }
                }
            }

            return TestID;
        }



        public static DataTable GetAppointmentsForTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT ta.testappointmentid as TestAppointmentID,
                                        ta.testtypeid,
                                        ta.localdrivinglicenseapplicationid,
                                        ta.appointmentdate,
                                        ta.paidfees,
                                        ta.createdbyuserid,
                                        ta.islocked,
                                        ta.retaketestapplicationid,
                                        tt.testtypetitle,
                                        u.username as createdbyusername
                                FROM testappointments ta
                                INNER JOIN testtypes tt ON ta.testtypeid = tt.testtypeid
                                INNER JOIN users u ON ta.createdbyuserid = u.userid
                                WHERE ta.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID 
                                AND ta.testtypeid = @TestTypeID
                                ORDER BY ta.appointmentdate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

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
                        LastErrorMessage = "Error getting appointments for test: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetUpcomingAppointmentsForTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT ta.*, 
                                        tt.testtypetitle,
                                        u.username as createdbyusername
                                FROM testappointments ta
                                INNER JOIN testtypes tt ON ta.testtypeid = tt.testtypeid
                                INNER JOIN users u ON ta.createdbyuserid = u.userid
                                WHERE ta.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID 
                                AND ta.testtypeid = @TestTypeID
                                AND ta.islocked = 0
                                AND ta.appointmentdate > GETDATE()
                                ORDER BY ta.appointmentdate";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

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
                        LastErrorMessage = "Error getting upcoming appointments: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetAllTestAppointments()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM TestAppointments_View
                                ORDER BY appointmentdate DESC";

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
                        LastErrorMessage = "Error getting all test appointments: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateTestAppointment(
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
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE testappointments
                SET 
                    testtypeid = @TestTypeID,
                    localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID,
                    appointmentdate = @AppointmentDate,
                    paidfees = @PaidFees,
                    createdbyuserid = @CreatedByUserID,
                    islocked = @IsLocked,
                    retaketestapplicationid = @RetakeTestApplicationID
                WHERE testappointmentid = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    command.Parameters.AddWithValue("@IsLocked", IsLocked);

                    if (RetakeTestApplicationID.HasValue)
                        command.Parameters.AddWithValue("@RetakeTestApplicationID", RetakeTestApplicationID.Value);
                    else
                        command.Parameters.AddWithValue("@RetakeTestApplicationID", DBNull.Value);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating test appointment: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Update appointment date only
        public static bool UpdateAppointmentDate(int TestAppointmentID, DateTime NewAppointmentDate)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE testappointments
                SET appointmentdate = @AppointmentDate
                WHERE testappointmentid = @TestAppointmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
                    command.Parameters.AddWithValue("@AppointmentDate", NewAppointmentDate);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating appointment date: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get count of appointments for a test
        public static int GetAppointmentsCountForTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            int count = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT COUNT(*) 
                                FROM testappointments 
                                WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID 
                                AND testtypeid = @TestTypeID";

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
                        LastErrorMessage = "Error getting appointments count: " + ex.Message;
                    }
                }
            }
            return count;
        }

        // Get retake count for a test
        public static int GetRetakeCountForTest(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            int count = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT COUNT(*) 
                                FROM testappointments 
                                WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID 
                                AND testtypeid = @TestTypeID
                                AND retaketestapplicationid IS NOT NULL";

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
                        LastErrorMessage = "Error getting retake count: " + ex.Message;
                    }
                }
            }
            return count;
        }
    }
}