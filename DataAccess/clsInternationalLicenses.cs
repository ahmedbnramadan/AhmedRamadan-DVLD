using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsInternationalLicenses
    {
        public static string LastErrorMessage = "";

        public static int AddNewInternationalLicense(
            int ApplicationID,
            int DriverID,
            int IssuedUsingLocalLicenseID,
            DateTime IssueDate,
            DateTime ExpirationDate,
            bool IsActive,
            int CreatedByUserID
            )
        {
            int InternationalLicenseID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO internationallicenses 
                            (applicationid, driverid, issuedusinglocallicenseid, issuedate, 
                             expirationdate, isactive, createdbyuserid)
                            VALUES 
                            (@ApplicationID, @DriverID, @IssuedUsingLocalLicenseID, @IssueDate, 
                             @ExpirationDate, @IsActive, @CreatedByUserID);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                    command.Parameters.AddWithValue("@IssueDate", IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            InternationalLicenseID = InsertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding international license: " + ex.Message;
                    }
                }
            }
            return InternationalLicenseID;
        }

        // Simplified AddNewInternationalLicense with current issue date
        public static int AddNewInternationalLicense(
            int ApplicationID,
            int DriverID,
            int IssuedUsingLocalLicenseID,
            DateTime ExpirationDate,
            int CreatedByUserID
            )
        {
            return AddNewInternationalLicense(
                ApplicationID,
                DriverID,
                IssuedUsingLocalLicenseID,
                DateTime.Now,  // Current date for issue
                ExpirationDate,
                true,          // Active by default
                CreatedByUserID
                );
        }

        public static bool IsInternationalLicenseExist(int InternationalLicenseID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM internationallicenses WHERE internationallicenseid = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking international license existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsDriverHaveActiveInternationalLicense(int DriverID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 FROM internationallicenses 
                                WHERE driverid = @DriverID 
                                AND isactive = 1
                                AND (expirationdate > GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking active international license: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsLicenseIssuedAsInternational(int LocalLicenseID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 FROM internationallicenses 
                                WHERE issuedusinglocallicenseid = @LocalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalLicenseID", LocalLicenseID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if license issued as international: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            int LicenseID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT internationallicenseid 
                                FROM internationallicenses 
                                WHERE driverid = @DriverID 
                                AND isactive = 1
                                AND (expirationdate > GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int ID))
                        {
                            LicenseID = ID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting active international license ID: " + ex.Message;
                    }
                }
            }
            return LicenseID;
        }

        public static bool DeleteInternationalLicense(int InternationalLicenseID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM internationallicenses WHERE internationallicenseid = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting international license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool DeactivateInternationalLicense(int InternationalLicenseID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE internationallicenses
                                SET isactive = 0
                                WHERE internationallicenseid = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deactivating international license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool ActivateInternationalLicense(int InternationalLicenseID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE internationallicenses
                                SET isactive = 1
                                WHERE internationallicenseid = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error activating international license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetInternationalLicenseByID(
            int InternationalLicenseID,
            ref int ApplicationID,
            ref int DriverID,
            ref int IssuedUsingLocalLicenseID,
            ref DateTime IssueDate,
            ref DateTime ExpirationDate,
            ref bool IsActive,
            ref int CreatedByUserID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM internationallicenses WHERE internationallicenseid = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                ApplicationID = (int)reader["applicationid"];
                                DriverID = (int)reader["driverid"];
                                IssuedUsingLocalLicenseID = (int)reader["issuedusinglocallicenseid"];
                                IssueDate = (DateTime)reader["issuedate"];
                                ExpirationDate = (DateTime)reader["expirationdate"];
                                IsActive = (bool)reader["isactive"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting international license by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetInternationalLicenseByApplicationID(
            int ApplicationID,
            ref int InternationalLicenseID,
            ref int DriverID,
            ref int IssuedUsingLocalLicenseID,
            ref DateTime IssueDate,
            ref DateTime ExpirationDate,
            ref bool IsActive,
            ref int CreatedByUserID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM internationallicenses WHERE applicationid = @ApplicationID";

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

                                InternationalLicenseID = (int)reader["internationallicenseid"];
                                DriverID = (int)reader["driverid"];
                                IssuedUsingLocalLicenseID = (int)reader["issuedusinglocallicenseid"];
                                IssueDate = (DateTime)reader["issuedate"];
                                ExpirationDate = (DateTime)reader["expirationdate"];
                                IsActive = (bool)reader["isactive"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting international license by application ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetInternationalLicensesByDriverID(int DriverID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT il.*, 
                                        l.licenseclass,
                                        lc.classname,
                                        l.issuedate as localissuedate,
                                        l.expirationdate as localexpirationdate
                                FROM internationallicenses il
                                INNER JOIN licenses l ON il.issuedusinglocallicenseid = l.licenseid
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                WHERE il.driverid = @DriverID
                                ORDER BY il.issuedate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);

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
                        LastErrorMessage = "Error getting international licenses by driver: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetActiveInternationalLicensesByDriverID(int DriverID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT il.*, 
                                        l.licenseclass,
                                        lc.classname,
                                        l.issuedate as localissuedate,
                                        l.expirationdate as localexpirationdate
                                FROM internationallicenses il
                                INNER JOIN licenses l ON il.issuedusinglocallicenseid = l.licenseid
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                WHERE il.driverid = @DriverID 
                                AND il.isactive = 1
                                AND (il.expirationdate > GETDATE())
                                ORDER BY il.issuedate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);

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
                        LastErrorMessage = "Error getting active international licenses: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetAllInternationalLicenses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT il.*, 
                                        l.licenseclass,
                                        lc.classname,
                                        d.driverid,
                                        p.firstname + ' ' + p.lastname as drivername,
                                        p.nationalno,
                                        u.username as createdbyusername
                                FROM internationallicenses il
                                INNER JOIN licenses l ON il.issuedusinglocallicenseid = l.licenseid
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                INNER JOIN drivers d ON il.driverid = d.driverid
                                INNER JOIN people p ON d.personid = p.personid
                                INNER JOIN users u ON il.createdbyuserid = u.userid
                                ORDER BY il.issuedate DESC";

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
                        LastErrorMessage = "Error getting all international licenses: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateInternationalLicense(
            int InternationalLicenseID,
            int ApplicationID,
            int DriverID,
            int IssuedUsingLocalLicenseID,
            DateTime IssueDate,
            DateTime ExpirationDate,
            bool IsActive,
            int CreatedByUserID
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE internationallicenses
                SET 
                    applicationid = @ApplicationID,
                    driverid = @DriverID,
                    issuedusinglocallicenseid = @IssuedUsingLocalLicenseID,
                    issuedate = @IssueDate,
                    expirationdate = @ExpirationDate,
                    isactive = @IsActive,
                    createdbyuserid = @CreatedByUserID
                WHERE internationallicenseid = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                    command.Parameters.AddWithValue("@IssueDate", IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating international license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Extend international license expiration date
        public static bool ExtendInternationalLicense(int InternationalLicenseID, int AdditionalYears)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE internationallicenses
                                SET expirationdate = DATEADD(YEAR, @AdditionalYears, expirationdate)
                                WHERE internationallicenseid = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);
                    command.Parameters.AddWithValue("@AdditionalYears", AdditionalYears);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error extending international license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get active international licenses count
        public static int GetActiveInternationalLicensesCount()
        {
            int count = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT COUNT(*) 
                                FROM internationallicenses 
                                WHERE isactive = 1 
                                AND expirationdate > GETDATE()";

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
                        LastErrorMessage = "Error getting active international licenses count: " + ex.Message;
                    }
                }
            }
            return count;
        }

        // Get expired international licenses
        public static DataTable GetExpiredInternationalLicenses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT il.*, 
                                        lc.classname,
                                        p.firstname + ' ' + p.lastname as drivername
                                FROM internationallicenses il
                                INNER JOIN licenses l ON il.issuedusinglocallicenseid = l.licenseid
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                INNER JOIN drivers d ON il.driverid = d.driverid
                                INNER JOIN people p ON d.personid = p.personid
                                WHERE il.expirationdate < GETDATE()
                                ORDER BY il.expirationdate";

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
                        LastErrorMessage = "Error getting expired international licenses: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        // Get international license by local license ID
        public static bool GetInternationalLicenseByLocalLicenseID(
            int LocalLicenseID,
            ref int InternationalLicenseID,
            ref int ApplicationID,
            ref int DriverID,
            ref DateTime IssueDate,
            ref DateTime ExpirationDate,
            ref bool IsActive,
            ref int CreatedByUserID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM internationallicenses WHERE issuedusinglocallicenseid = @LocalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalLicenseID", LocalLicenseID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                InternationalLicenseID = (int)reader["internationallicenseid"];
                                ApplicationID = (int)reader["applicationid"];
                                DriverID = (int)reader["driverid"];
                                IssueDate = (DateTime)reader["issuedate"];
                                ExpirationDate = (DateTime)reader["expirationdate"];
                                IsActive = (bool)reader["isactive"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting international license by local license ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        // Get driver ID by international license ID
        public static int GetDriverIDByInternationalLicenseID(int InternationalLicenseID)
        {
            int DriverID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT driverid FROM internationallicenses WHERE internationallicenseid = @InternationalLicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int ID))
                        {
                            DriverID = ID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting driver ID: " + ex.Message;
                    }
                }
            }
            return DriverID;
        }
    }
}