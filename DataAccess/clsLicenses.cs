using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsLicenses
    {
        public static string LastErrorMessage = "";

        // Issue Reason Constants
        public const short IssueReasonFirstTime = 1;
        public const short IssueReasonRenew = 2;
        public const short IssueReasonReplacementForDamaged = 3;
        public const short IssueReasonReplacementForLost = 4;

        public static int AddNewLicense(
            int ApplicationID,
            int DriverID,
            int LicenseClass,
            DateTime IssueDate,
            DateTime ExpirationDate,
            string Notes,
            decimal PaidFees,
            bool IsActive,
            short IssueReason,
            int CreatedByUserID
            )
        {
            int LicenseID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO licenses 
                            (applicationid, driverid, licenseclass, issuedate, 
                             expirationdate, notes, paidfees, isactive, issuereason, createdbyuserid)
                            VALUES 
                            (@ApplicationID, @DriverID, @LicenseClass, @IssueDate, 
                             @ExpirationDate, @Notes, @PaidFees, @IsActive, @IssueReason, @CreatedByUserID);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", LicenseClass);
                    command.Parameters.AddWithValue("@IssueDate", IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);

                    if (string.IsNullOrEmpty(Notes))
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Notes", Notes);

                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    command.Parameters.AddWithValue("@IssueReason", IssueReason);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            LicenseID = InsertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding license: " + ex.Message;
                    }
                }
            }
            return LicenseID;
        }

        // Simplified AddNewLicense with current issue date
        public static int AddNewLicense(
            int ApplicationID,
            int DriverID,
            int LicenseClass,
            DateTime ExpirationDate,
            decimal PaidFees,
            short IssueReason,
            int CreatedByUserID
            )
        {
            return AddNewLicense(
                ApplicationID,
                DriverID,
                LicenseClass,
                DateTime.Now,  // Current date for issue
                ExpirationDate,
                null,          // No notes
                PaidFees,
                true,          // Active by default
                IssueReason,
                CreatedByUserID
                );
        }

        public static bool IsLicenseExist(int LicenseID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM licenses WHERE licenseid = @LicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking license existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsDriverHaveActiveLicense(int DriverID, int LicenseClass)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 FROM licenses 
                                WHERE driverid = @DriverID 
                                AND licenseclass = @LicenseClass
                                AND isactive = 1
                                AND (expirationdate > GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", LicenseClass);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking active license: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteLicense(int LicenseID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM licenses WHERE licenseid = @LicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool DeactivateLicense(int LicenseID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE licenses
                                SET isactive = 0
                                WHERE licenseid = @LicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deactivating license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool ActivateLicense(int LicenseID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE licenses
                                SET isactive = 1
                                WHERE licenseid = @LicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error activating license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetLicenseByID(
            int LicenseID,
            ref int ApplicationID,
            ref int DriverID,
            ref int LicenseClass,
            ref DateTime IssueDate,
            ref DateTime ExpirationDate,
            ref string Notes,
            ref decimal PaidFees,
            ref bool IsActive,
            ref short IssueReason,
            ref int CreatedByUserID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM licenses WHERE licenseid = @LicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

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
                                LicenseClass = (int)reader["licenseclass"];
                                IssueDate = (DateTime)reader["issuedate"];
                                ExpirationDate = (DateTime)reader["expirationdate"];
                                Notes = reader["notes"] != DBNull.Value ? (string)reader["notes"] : "";
                                PaidFees = (decimal)reader["paidfees"];
                                IsActive = (bool)reader["isactive"];
                                IssueReason = (short)reader["issuereason"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting license by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetLicenseByApplicationID(
            ref int LicenseID,
            int ApplicationID,
            ref int DriverID,
            ref int LicenseClass,
            ref DateTime IssueDate,
            ref DateTime ExpirationDate,
            ref string Notes,
            ref decimal PaidFees,
            ref bool IsActive,
            ref short IssueReason,
            ref int CreatedByUserID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM licenses WHERE applicationid = @ApplicationID";

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

                                LicenseID = (int)reader["licenseid"];
                                DriverID = (int)reader["driverid"];
                                LicenseClass = (int)reader["licenseclass"];
                                IssueDate = (DateTime)reader["issuedate"];
                                ExpirationDate = (DateTime)reader["expirationdate"];
                                Notes = reader["notes"] != DBNull.Value ? (string)reader["notes"] : "";
                                PaidFees = (decimal)reader["paidfees"];
                                IsActive = (bool)reader["isactive"];
                                IssueReason = (short)reader["issuereason"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting license by application ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetLicensesByDriverID(int DriverID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT l.*, 
                                        lc.classname,
                                        lc.classfees,
                                        (SELECT COUNT(*) FROM licenses WHERE driverid = l.driverid AND isactive = 1) as activecount
                                FROM licenses l
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                WHERE l.driverid = @DriverID
                                ORDER BY l.issuedate DESC";

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
                        LastErrorMessage = "Error getting licenses by driver: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetActiveLicensesByDriverID(int DriverID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT l.*, 
                                        lc.classname,
                                        lc.classfees
                                FROM licenses l
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                WHERE l.driverid = @DriverID 
                                AND l.isactive = 1
                                AND (l.expirationdate > GETDATE())
                                ORDER BY l.issuedate DESC";

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
                        LastErrorMessage = "Error getting active licenses: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
        {
            int LicenseID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT Licenses.LicenseID 
                         FROM Licenses 
                         INNER JOIN Drivers ON Licenses.DriverID = Drivers.DriverID
                         WHERE Drivers.PersonID = @PersonID 
                         AND Licenses.LicenseClass = @LicenseClassID 
                         AND Licenses.IsActive = 1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open(); object result = command.ExecuteScalar();
                        if (result != null) LicenseID = (int)result;
                    }
                    catch { return -1; }
                }
            }
            return LicenseID;
        }

        public static DataTable GetAllLicenses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT l.*, 
                                        lc.classname,
                                        p.firstname + ' ' + p.lastname as drivername,
                                        p.nationalno,
                                        u.username as createdbyusername
                                FROM licenses l
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                INNER JOIN drivers d ON l.driverid = d.driverid
                                INNER JOIN people p ON d.personid = p.personid
                                INNER JOIN users u ON l.createdbyuserid = u.userid
                                ORDER BY l.issuedate DESC";

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
                        LastErrorMessage = "Error getting all licenses: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateLicense(
            int LicenseID,
            int ApplicationID,
            int DriverID,
            int LicenseClass,
            DateTime IssueDate,
            DateTime ExpirationDate,
            string Notes,
            decimal PaidFees,
            bool IsActive,
            short IssueReason,
            int CreatedByUserID
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE licenses
                SET 
                    applicationid = @ApplicationID,
                    driverid = @DriverID,
                    licenseclass = @LicenseClass,
                    issuedate = @IssueDate,
                    expirationdate = @ExpirationDate,
                    notes = @Notes,
                    paidfees = @PaidFees,
                    isactive = @IsActive,
                    issuereason = @IssueReason,
                    createdbyuserid = @CreatedByUserID
                WHERE licenseid = @LicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", LicenseClass);
                    command.Parameters.AddWithValue("@IssueDate", IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);

                    if (string.IsNullOrEmpty(Notes))
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Notes", Notes);

                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    command.Parameters.AddWithValue("@IssueReason", IssueReason);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Extend license expiration date
        public static bool ExtendLicense(int LicenseID, int AdditionalYears)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE licenses
                                SET expirationdate = DATEADD(YEAR, @AdditionalYears, expirationdate)
                                WHERE licenseid = @LicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@AdditionalYears", AdditionalYears);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error extending license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get active licenses count
        public static int GetActiveLicensesCount()
        {
            int count = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT COUNT(*) 
                                FROM licenses 
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
                        LastErrorMessage = "Error getting active licenses count: " + ex.Message;
                    }
                }
            }
            return count;
        }

        // Get expired licenses
        public static DataTable GetExpiredLicenses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT l.*, 
                                        lc.classname,
                                        p.firstname + ' ' + p.lastname as drivername
                                FROM licenses l
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                INNER JOIN drivers d ON l.driverid = d.driverid
                                INNER JOIN people p ON d.personid = p.personid
                                WHERE l.expirationdate < GETDATE()
                                ORDER BY l.expirationdate";

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
                        LastErrorMessage = "Error getting expired licenses: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        // Get license by driver and class
        public static bool GetActiveLicenseByDriverAndClass(
            int DriverID,
            int LicenseClass,
            ref int LicenseID,
            ref int ApplicationID,
            ref DateTime IssueDate,
            ref DateTime ExpirationDate,
            ref string Notes,
            ref decimal PaidFees,
            ref short IssueReason,
            ref int CreatedByUserID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM licenses 
                                WHERE driverid = @DriverID 
                                AND licenseclass = @LicenseClass
                                AND isactive = 1
                                AND expirationdate > GETDATE()";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", LicenseClass);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                LicenseID = (int)reader["licenseid"];
                                ApplicationID = (int)reader["applicationid"];
                                IssueDate = (DateTime)reader["issuedate"];
                                ExpirationDate = (DateTime)reader["expirationdate"];
                                Notes = reader["notes"] != DBNull.Value ? (string)reader["notes"] : "";
                                PaidFees = (decimal)reader["paidfees"];
                                IssueReason = (short)reader["issuereason"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting license by driver and class: " + ex.Message;
                    }
                }
            }
            return isFound;
        }
    }
}