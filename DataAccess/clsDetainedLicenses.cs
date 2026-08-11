using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsDetainedLicenses
    {
        public static string LastErrorMessage = "";

        public static int AddNewDetainedLicense(
            int LicenseID,
            DateTime DetainDate,
            decimal FineFees,
            int CreatedByUserID,
            bool IsReleased,
            DateTime? ReleaseDate,
            int? ReleasedByUserID,
            int? ReleaseApplicationID
            )
        {
            int DetainID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO detainedlicenses 
                            (licenseid, detaindate, finefees, createdbyuserid, 
                             isreleased, releasedate, releasedbyuserid, releaseapplicationid)
                            VALUES 
                            (@LicenseID, @DetainDate, @FineFees, @CreatedByUserID, 
                             @IsReleased, @ReleaseDate, @ReleasedByUserID, @ReleaseApplicationID);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@DetainDate", DetainDate);
                    command.Parameters.AddWithValue("@FineFees", FineFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    command.Parameters.AddWithValue("@IsReleased", IsReleased);
                    
                    if (ReleaseDate.HasValue)
                        command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate.Value);
                    else
                        command.Parameters.AddWithValue("@ReleaseDate", DBNull.Value);
                    
                    if (ReleasedByUserID.HasValue)
                        command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID.Value);
                    else
                        command.Parameters.AddWithValue("@ReleasedByUserID", DBNull.Value);
                    
                    if (ReleaseApplicationID.HasValue)
                        command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID.Value);
                    else
                        command.Parameters.AddWithValue("@ReleaseApplicationID", DBNull.Value);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            DetainID = InsertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding detained license: " + ex.Message;
                    }
                }
            }
            return DetainID;
        }

        // Simplified AddNewDetainedLicense for new detention
        public static int AddNewDetainedLicense(
            int LicenseID,
            decimal FineFees,
            int CreatedByUserID
            )
        {
            return AddNewDetainedLicense(
                LicenseID,
                DateTime.Now,  // Current date for detain
                FineFees,
                CreatedByUserID,
                false,         // Not released by default
                null,          // No release date
                null,          // No released by user
                null           // No release application
                );
        }

        public static bool IsDetainedLicenseExist(int DetainID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM detainedlicenses WHERE detainid = @DetainID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking detained license existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsLicenseDetained(int LicenseID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 FROM detainedlicenses 
                                WHERE licenseid = @LicenseID 
                                AND isreleased = 0";

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
                        LastErrorMessage = "Error checking if license is detained: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsLicenseDetained(int LicenseID, ref int DetainID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT detainid FROM detainedlicenses 
                                WHERE licenseid = @LicenseID 
                                AND isreleased = 0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            isFound = true;
                            DetainID = (int)result;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if license is detained: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteDetainedLicense(int DetainID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM detainedlicenses WHERE detainid = @DetainID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting detained license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool ReleaseDetainedLicense(
            int DetainID,
            int ReleasedByUserID,
            int ReleaseApplicationID
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE detainedlicenses
                                SET isreleased = 1,
                                    releasedate = @ReleaseDate,
                                    releasedbyuserid = @ReleasedByUserID,
                                    releaseapplicationid = @ReleaseApplicationID
                                WHERE detainid = @DetainID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);
                    command.Parameters.AddWithValue("@ReleaseDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
                    command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error releasing detained license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool ReleaseLicense(int LicenseID, int ReleasedByUserID, int ReleaseApplicationID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE detainedlicenses
                                SET isreleased = 1,
                                    releasedate = @ReleaseDate,
                                    releasedbyuserid = @ReleasedByUserID,
                                    releaseapplicationid = @ReleaseApplicationID
                                WHERE licenseid = @LicenseID AND isreleased = 0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@ReleaseDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
                    command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error releasing license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetDetainedLicenseByID(
            int DetainID,
            ref int LicenseID,
            ref DateTime DetainDate,
            ref decimal FineFees,
            ref int CreatedByUserID,
            ref bool IsReleased,
            ref DateTime? ReleaseDate,
            ref int? ReleasedByUserID,
            ref int? ReleaseApplicationID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM detainedlicenses WHERE detainid = @DetainID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                LicenseID = (int)reader["licenseid"];
                                DetainDate = (DateTime)reader["detaindate"];
                                FineFees = (decimal)reader["finefees"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                                IsReleased = (bool)reader["isreleased"];
                                
                                ReleaseDate = reader["releasedate"] != DBNull.Value 
                                    ? (DateTime?)reader["releasedate"] 
                                    : null;
                                
                                ReleasedByUserID = reader["releasedbyuserid"] != DBNull.Value 
                                    ? (int?)reader["releasedbyuserid"] 
                                    : null;
                                
                                ReleaseApplicationID = reader["releaseapplicationid"] != DBNull.Value 
                                    ? (int?)reader["releaseapplicationid"] 
                                    : null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting detained license by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetDetainedLicenseByLicenseID(
            int LicenseID,
            ref int DetainID,
            ref DateTime DetainDate,
            ref decimal FineFees,
            ref int CreatedByUserID,
            ref bool IsReleased,
            ref DateTime? ReleaseDate,
            ref int? ReleasedByUserID,
            ref int? ReleaseApplicationID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM detainedlicenses 
                                WHERE licenseid = @LicenseID AND isreleased = 0";

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

                                DetainID = (int)reader["detainid"];
                                DetainDate = (DateTime)reader["detaindate"];
                                FineFees = (decimal)reader["finefees"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                                IsReleased = (bool)reader["isreleased"];
                                
                                ReleaseDate = reader["releasedate"] != DBNull.Value 
                                    ? (DateTime?)reader["releasedate"] 
                                    : null;
                                
                                ReleasedByUserID = reader["releasedbyuserid"] != DBNull.Value 
                                    ? (int?)reader["releasedbyuserid"] 
                                    : null;
                                
                                ReleaseApplicationID = reader["releaseapplicationid"] != DBNull.Value 
                                    ? (int?)reader["releaseapplicationid"] 
                                    : null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting detained license by license ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetAllDetainedLicenses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT dl.*, 
                                        l.licenseclass,
                                        l.licenseid,
                                        l.issuereason,
                                        lc.classname,
                                        d.driverid,
                                        p.firstname + ' ' + p.lastname as drivername,
                                        p.nationalno,
                                        u1.username as detainedbyusername,
                                        u2.username as releasedbyusername
                                FROM detainedlicenses dl
                                INNER JOIN licenses l ON dl.licenseid = l.licenseid
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                INNER JOIN drivers d ON l.driverid = d.driverid
                                INNER JOIN people p ON d.personid = p.personid
                                INNER JOIN users u1 ON dl.createdbyuserid = u1.userid
                                LEFT JOIN users u2 ON dl.releasedbyuserid = u2.userid
                                ORDER BY dl.detaindate DESC";

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
                        LastErrorMessage = "Error getting all detained licenses: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetDetainedLicensesByDriverID(int DriverID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT dl.*, 
                                        l.licenseclass,
                                        l.licenseid,
                                        lc.classname,
                                        u1.username as detainedbyusername,
                                        u2.username as releasedbyusername
                                FROM detainedlicenses dl
                                INNER JOIN licenses l ON dl.licenseid = l.licenseid
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                INNER JOIN drivers d ON l.driverid = d.driverid
                                INNER JOIN users u1 ON dl.createdbyuserid = u1.userid
                                LEFT JOIN users u2 ON dl.releasedbyuserid = u2.userid
                                WHERE d.driverid = @DriverID
                                ORDER BY dl.detaindate DESC";

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
                        LastErrorMessage = "Error getting detained licenses by driver: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetActiveDetainedLicenses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT dl.*, 
                                        l.licenseclass,
                                        l.licenseid,
                                        lc.classname,
                                        d.driverid,
                                        p.firstname + ' ' + p.lastname as drivername,
                                        p.nationalno,
                                        u.username as detainedbyusername
                                FROM detainedlicenses dl
                                INNER JOIN licenses l ON dl.licenseid = l.licenseid
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                INNER JOIN drivers d ON l.driverid = d.driverid
                                INNER JOIN people p ON d.personid = p.personid
                                INNER JOIN users u ON dl.createdbyuserid = u.userid
                                WHERE dl.isreleased = 0
                                ORDER BY dl.detaindate DESC";

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
                        LastErrorMessage = "Error getting active detained licenses: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static DataTable GetReleasedDetainedLicenses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT dl.*, 
                                        l.licenseclass,
                                        l.licenseid,
                                        lc.classname,
                                        d.driverid,
                                        p.firstname + ' ' + p.lastname as drivername,
                                        p.nationalno,
                                        u1.username as detainedbyusername,
                                        u2.username as releasedbyusername
                                FROM detainedlicenses dl
                                INNER JOIN licenses l ON dl.licenseid = l.licenseid
                                INNER JOIN licenseclasses lc ON l.licenseclass = lc.licenseclassid
                                INNER JOIN drivers d ON l.driverid = d.driverid
                                INNER JOIN people p ON d.personid = p.personid
                                INNER JOIN users u1 ON dl.createdbyuserid = u1.userid
                                INNER JOIN users u2 ON dl.releasedbyuserid = u2.userid
                                WHERE dl.isreleased = 1
                                ORDER BY dl.releasedate DESC";

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
                        LastErrorMessage = "Error getting released detained licenses: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateDetainedLicense(
            int DetainID,
            int LicenseID,
            DateTime DetainDate,
            decimal FineFees,
            int CreatedByUserID,
            bool IsReleased,
            DateTime? ReleaseDate,
            int? ReleasedByUserID,
            int? ReleaseApplicationID
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE detainedlicenses
                SET 
                    licenseid = @LicenseID,
                    detaindate = @DetainDate,
                    finefees = @FineFees,
                    createdbyuserid = @CreatedByUserID,
                    isreleased = @IsReleased,
                    releasedate = @ReleaseDate,
                    releasedbyuserid = @ReleasedByUserID,
                    releaseapplicationid = @ReleaseApplicationID
                WHERE detainid = @DetainID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DetainID", DetainID);
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    command.Parameters.AddWithValue("@DetainDate", DetainDate);
                    command.Parameters.AddWithValue("@FineFees", FineFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    command.Parameters.AddWithValue("@IsReleased", IsReleased);
                    
                    if (ReleaseDate.HasValue)
                        command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate.Value);
                    else
                        command.Parameters.AddWithValue("@ReleaseDate", DBNull.Value);
                    
                    if (ReleasedByUserID.HasValue)
                        command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID.Value);
                    else
                        command.Parameters.AddWithValue("@ReleasedByUserID", DBNull.Value);
                    
                    if (ReleaseApplicationID.HasValue)
                        command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID.Value);
                    else
                        command.Parameters.AddWithValue("@ReleaseApplicationID", DBNull.Value);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating detained license: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get total fines collected
        public static decimal GetTotalFinesCollected()
        {
            decimal total = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT ISNULL(SUM(finefees), 0) 
                                FROM detainedlicenses 
                                WHERE isreleased = 1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && decimal.TryParse(result.ToString(), out decimal totalValue))
                        {
                            total = totalValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting total fines: " + ex.Message;
                    }
                }
            }
            return total;
        }

        // Get detained licenses count
        public static int GetDetainedLicensesCount(bool IsReleased)
        {
            int count = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM detainedlicenses WHERE isreleased = @IsReleased";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IsReleased", IsReleased);

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
                        LastErrorMessage = "Error getting detained licenses count: " + ex.Message;
                    }
                }
            }
            return count;
        }
    }
}