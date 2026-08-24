using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsLocalDrivingLicenseApplications
    {
        public static string LastErrorMessage = "";

        public static int AddNewLocalDrivingLicenseApplication(
            int ApplicationID,
            int LicenseClassID
            )
        {
            int LocalDrivingLicenseApplicationID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO localdrivinglicenseapplications 
                            (applicationid, licenseclassid)
                            VALUES 
                            (@ApplicationID, @LicenseClassID);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            LocalDrivingLicenseApplicationID = InsertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding local driving license application: " + ex.Message;
                    }
                }
            }
            return LocalDrivingLicenseApplicationID;
        }

        public static bool IsLocalDrivingLicenseApplicationExist(int LocalDrivingLicenseApplicationID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM localdrivinglicenseapplications WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking local driving license application existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DoesPersonHaveActiveApplication(int PersonID, int LicenseClassID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 
                                FROM localdrivinglicenseapplications ldla
                                INNER JOIN applications a ON ldla.applicationid = a.applicationid
                                WHERE a.applicantpersonid = @PersonID 
                                AND ldla.licenseclassid = @LicenseClassID
                                AND a.applicationstatus = 1"; // Status 1 = New

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if person has active application: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DoesPersonHaveActiveApplication(int PersonID, int LicenseClassID, ref int LocalDrivingLicenseApplicationID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT ldla.localdrivinglicenseapplicationid
                                FROM localdrivinglicenseapplications ldla
                                INNER JOIN applications a ON ldla.applicationid = a.applicationid
                                WHERE a.applicantpersonid = @PersonID 
                                AND ldla.licenseclassid = @LicenseClassID
                                AND a.applicationstatus = 1"; // Status 1 = New

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            isFound = true;
                            LocalDrivingLicenseApplicationID = (int)result;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if person has active application: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DoesPersonHaveAnyActiveApplication(int PersonID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 
                                FROM localdrivinglicenseapplications ldla
                                INNER JOIN applications a ON ldla.applicationid = a.applicationid
                                WHERE a.applicantpersonid = @PersonID 
                                AND a.applicationstatus = 1"; // Status 1 = New

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if person has any active application: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM localdrivinglicenseapplications WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting local driving license application: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        #region Finding Methodd
        // <summary> GetLocalDrivingLicenseApplicationByID,
        // GetLocalDrivingLicenseApplicationByApplicationID,
        // GetAllLocalDrivingLicenseApplications
        // GetLocalDrivingLicenseApplicationsByPersonID
        // GetLocalDrivingLicenseApplicationsByStatus
        // GetLocalDrivingLicenseApplicationsByLicenseClass <summary>

        public static bool GetLocalDrivingLicenseApplicationByID(
            int LocalDrivingLicenseApplicationID,
            ref int ApplicationID,
            ref int LicenseClassID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM localdrivinglicenseapplications WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                ApplicationID = (int)reader["applicationid"];
                                LicenseClassID = (int)reader["licenseclassid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting local driving license application by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetLocalDrivingLicenseApplicationByApplicationID(
            int ApplicationID,
            ref int LocalDrivingLicenseApplicationID,
            ref int LicenseClassID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM localdrivinglicenseapplications WHERE applicationid = @ApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                LocalDrivingLicenseApplicationID = (int)reader["localdrivinglicenseapplicationid"];
                                LicenseClassID = (int)reader["licenseclassid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting local driving license application by application ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM localdrivinglicenseapplications_view   
                                ORDER BY a.applicationdate DESC";

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
                        LastErrorMessage = "Error getting all local driving license applications: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetLocalDrivingLicenseApplicationsByPersonID(int PersonID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
                                    ldla.localdrivinglicenseapplicationid,
                                    ldla.licenseclassid,
                                    lc.classname,
                                    lc.classfees,
                                    a.applicationid,
                                    a.applicationdate,
                                    a.applicationstatus,
                                    a.laststatusdate,
                                    a.paidfees,
                                    CASE 
                                        WHEN a.applicationstatus = 1 THEN 'New'
                                        WHEN a.applicationstatus = 2 THEN 'Cancelled'
                                        WHEN a.applicationstatus = 3 THEN 'Completed'
                                    END as statusname,
                                    (SELECT COUNT(*) FROM tests t 
                                     INNER JOIN testappointments ta ON t.testappointmentid = ta.testappointmentid
                                     WHERE ta.localdrivinglicenseapplicationid = ldla.localdrivinglicenseapplicationid
                                     AND t.testresult = 1) as passedtestscount,
                                    (SELECT licenseid FROM licenses l 
                                     INNER JOIN applications app ON l.applicationid = app.applicationid
                                     WHERE app.applicationid = a.applicationid) as issuedlicenseid
                                FROM localdrivinglicenseapplications ldla
                                INNER JOIN licenseclasses lc ON ldla.licenseclassid = lc.licenseclassid
                                INNER JOIN applications a ON ldla.applicationid = a.applicationid
                                WHERE a.applicantpersonid = @PersonID
                                ORDER BY a.applicationdate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

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
                        LastErrorMessage = "Error getting applications by person: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetLocalDrivingLicenseApplicationsByStatus(short Status)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
                                    ldla.localdrivinglicenseapplicationid,
                                    ldla.licenseclassid,
                                    lc.classname,
                                    a.applicationid,
                                    a.applicantpersonid,
                                    a.applicationdate,
                                    a.applicationstatus,
                                    a.paidfees,
                                    p.firstname + ' ' + p.lastname as applicantname,
                                    p.nationalno,
                                    (SELECT COUNT(*) FROM tests t 
                                     INNER JOIN testappointments ta ON t.testappointmentid = ta.testappointmentid
                                     WHERE ta.localdrivinglicenseapplicationid = ldla.localdrivinglicenseapplicationid
                                     AND t.testresult = 1) as passedtestscount
                                FROM localdrivinglicenseapplications ldla
                                INNER JOIN licenseclasses lc ON ldla.licenseclassid = lc.licenseclassid
                                INNER JOIN applications a ON ldla.applicationid = a.applicationid
                                INNER JOIN people p ON a.applicantpersonid = p.personid
                                WHERE a.applicationstatus = @ApplicationStatus
                                ORDER BY a.applicationdate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationStatus", Status);

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
                        LastErrorMessage = "Error getting applications by status: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetLocalDrivingLicenseApplicationsByLicenseClass(int LicenseClassID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
                                    ldla.localdrivinglicenseapplicationid,
                                    lc.classname,
                                    a.applicationid,
                                    a.applicantpersonid,
                                    a.applicationdate,
                                    a.applicationstatus,
                                    a.paidfees,
                                    p.firstname + ' ' + p.lastname as applicantname,
                                    p.nationalno
                                FROM localdrivinglicenseapplications ldla
                                INNER JOIN licenseclasses lc ON ldla.licenseclassid = lc.licenseclassid
                                INNER JOIN applications a ON ldla.applicationid = a.applicationid
                                INNER JOIN people p ON a.applicantpersonid = p.personid
                                WHERE ldla.licenseclassid = @LicenseClassID
                                ORDER BY a.applicationdate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

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
                        LastErrorMessage = "Error getting applications by license class: " + ex.Message;
                    }
                }
            }
            return dt;
        }
        #endregion

        public static bool UpdateLocalDrivingLicenseApplication(
            int LocalDrivingLicenseApplicationID,
            int ApplicationID,
            int LicenseClassID
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE localdrivinglicenseapplications
                SET 
                    applicationid = @ApplicationID,
                    licenseclassid = @LicenseClassID
                WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating local driving license application: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get ApplicationID by LocalDrivingLicenseApplicationID
        public static int GetApplicationID(int LocalDrivingLicenseApplicationID)
        {
            int ApplicationID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT applicationid FROM localdrivinglicenseapplications WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int ID))
                        {
                            ApplicationID = ID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting application ID: " + ex.Message;
                    }
                }
            }
            return ApplicationID;
        }

        // Get LicenseClassID by LocalDrivingLicenseApplicationID
        public static int GetLicenseClassID(int LocalDrivingLicenseApplicationID)
        {
            int LicenseClassID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT licenseclassid FROM localdrivinglicenseapplications WHERE localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int ID))
                        {
                            LicenseClassID = ID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting license class ID: " + ex.Message;
                    }
                }
            }
            return LicenseClassID;
        }

        // Get PersonID by LocalDrivingLicenseApplicationID
        public static int GetPersonID(int LocalDrivingLicenseApplicationID)
        {
            int PersonID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT a.applicantpersonid
                                FROM localdrivinglicenseapplications ldla
                                INNER JOIN applications a ON ldla.applicationid = a.applicationid
                                WHERE ldla.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int ID))
                        {
                            PersonID = ID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting person ID: " + ex.Message;
                    }
                }
            }
            return PersonID;
        }

        // Get total fees for an application (application fees + test fees)
        public static decimal GetTotalFees(int LocalDrivingLicenseApplicationID)
        {
            decimal totalFees = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
                                    (SELECT a.paidfees FROM applications a 
                                     INNER JOIN localdrivinglicenseapplications ldla ON a.applicationid = ldla.applicationid
                                     WHERE ldla.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID) +
                                    ISNULL((SELECT SUM(ta.paidfees) 
                                           FROM testappointments ta 
                                           WHERE ta.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID), 0) as TotalFees";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && decimal.TryParse(result.ToString(), out decimal fees))
                        {
                            totalFees = fees;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting total fees: " + ex.Message;
                    }
                }
            }
            return totalFees;
        }

        // Get number of trials for a specific test type
        public static int GetTestTrialsCount(int LocalDrivingLicenseApplicationID, int TestTypeID)
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
                        if (result != null && int.TryParse(result.ToString(), out int trials))
                        {
                            count = trials;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test trials count: " + ex.Message;
                    }
                }
            }
            return count;
        }

        // Check if all tests are passed
        public static bool IsAllTestsPassed(int LocalDrivingLicenseApplicationID)
        {
            bool allPassed = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
                                    CASE 
                                        WHEN COUNT(*) = 3 AND SUM(CASE WHEN t.testresult = 1 THEN 1 ELSE 0 END) = 3 THEN 1
                                        ELSE 0
                                    END as AllPassed
                                FROM testappointments ta
                                LEFT JOIN tests t ON ta.testappointmentid = t.testappointmentid
                                WHERE ta.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            allPassed = (bool)result;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if all tests passed: " + ex.Message;
                    }
                }
            }
            return allPassed;
        }

        // Get application status text
        public static string GetStatusText(int LocalDrivingLicenseApplicationID)
        {
            string statusText = "";
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT a.applicationstatus
                                FROM localdrivinglicenseapplications ldla
                                INNER JOIN applications a ON ldla.applicationid = a.applicationid
                                WHERE ldla.localdrivinglicenseapplicationid = @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            short status = (short)result;
                            switch (status)
                            {
                                case 1:
                                    statusText = "New";
                                    break;
                                case 2:
                                    statusText = "Cancelled";
                                    break;
                                case 3:
                                    statusText = "Completed";
                                    break;
                                default:
                                    statusText = "Unknown";
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting status text: " + ex.Message;
                    }
                }
            }
            return statusText;
        }

        // Get total applications count
        public static int GetTotalApplicationsCount()
        {
            int count = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM localdrivinglicenseapplications";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int resultCount))
                        {
                            count = resultCount;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting total applications count: " + ex.Message;
                    }
                }
            }
            return count;
        }
    }
}