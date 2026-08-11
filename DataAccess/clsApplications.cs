using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsApplications
    {
        public static string LastErrorMessage = "";

        // Application Status Constants
        public const short StatusNew = 1;
        public const short StatusCancelled = 2;
        public const short StatusCompleted = 3;

        public static int AddNewApplication(
            int ApplicantPersonID,
            DateTime ApplicationDate,
            int ApplicationTypeID,
            short ApplicationStatus,
            DateTime LastStatusDate,
            decimal PaidFees,
            int CreatedByUserID
            )
        {
            int ApplicationID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO applications 
                            (applicantpersonid, applicationdate, applicationtypeid, 
                             applicationstatus, laststatusdate, paidfees, createdbyuserid)
                            VALUES 
                            (@ApplicantPersonID, @ApplicationDate, @ApplicationTypeID, 
                             @ApplicationStatus, @LastStatusDate, @PaidFees, @CreatedByUserID);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                    command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                    command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            ApplicationID = InsertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding application: " + ex.Message;
                    }
                }
            }
            return ApplicationID;
        }

        // Simplified AddNewApplication with current dates
        public static int AddNewApplication(
            int ApplicantPersonID,
            int ApplicationTypeID,
            decimal PaidFees,
            int CreatedByUserID
            )
        {
            return AddNewApplication(
                ApplicantPersonID,
                DateTime.Now,  // Current date for application
                ApplicationTypeID,
                StatusNew,     // Default status = New
                DateTime.Now,  // Current date for last status
                PaidFees,
                CreatedByUserID
                );
        }

        public static bool IsApplicationExist(int ApplicationID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM applications WHERE applicationid = @ApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking application existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsPersonHaveApplicationOfType(int PersonID, int ApplicationTypeID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 FROM applications 
                                WHERE applicantpersonid = @ApplicantPersonID 
                                AND applicationtypeid = @ApplicationTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking person application type: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsPersonHaveActiveApplication(int PersonID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 FROM applications 
                                WHERE applicantpersonid = @ApplicantPersonID 
                                AND applicationstatus = @ApplicationStatus";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
                    command.Parameters.AddWithValue("@ApplicationStatus", StatusNew);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking active application: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteApplication(int ApplicationID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM applications WHERE applicationid = @ApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting application: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool CancelApplication(int ApplicationID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE applications
                                SET applicationstatus = @ApplicationStatus,
                                    laststatusdate = @LastStatusDate
                                WHERE applicationid = @ApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@ApplicationStatus", StatusCancelled);
                    command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error cancelling application: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool SetApplicationComplete(int ApplicationID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE applications
                                SET applicationstatus = @ApplicationStatus,
                                    laststatusdate = @LastStatusDate
                                WHERE applicationid = @ApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@ApplicationStatus", StatusCompleted);
                    command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error completing application: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetApplicationByID(
            int ApplicationID,
            ref int ApplicantPersonID,
            ref DateTime ApplicationDate,
            ref int ApplicationTypeID,
            ref short ApplicationStatus,
            ref DateTime LastStatusDate,
            ref decimal PaidFees,
            ref int CreatedByUserID
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM applications WHERE applicationid = @ApplicationID";

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

                                ApplicantPersonID = (int)reader["applicantpersonid"];
                                ApplicationDate = (DateTime)reader["applicationdate"];
                                ApplicationTypeID = (int)reader["applicationtypeid"];
                                ApplicationStatus = (short)reader["applicationstatus"];
                                LastStatusDate = (DateTime)reader["laststatusdate"];
                                PaidFees = (decimal)reader["paidfees"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting application by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetApplicationsByPersonID(int PersonID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT a.*, 
                                        ap.applicationtypetitle,
                                        p.firstname + ' ' + p.lastname as applicantname
                                FROM applications a
                                INNER JOIN applicationtypes ap ON a.applicationtypeid = ap.applicationtypeid
                                INNER JOIN people p ON a.applicantpersonid = p.personid
                                WHERE a.applicantpersonid = @ApplicantPersonID
                                ORDER BY a.applicationdate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);

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

        public static DataTable GetApplicationsByType(int ApplicationTypeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT a.*, 
                                        ap.applicationtypetitle,
                                        p.firstname + ' ' + p.lastname as applicantname
                                FROM applications a
                                INNER JOIN applicationtypes ap ON a.applicationtypeid = ap.applicationtypeid
                                INNER JOIN people p ON a.applicantpersonid = p.personid
                                WHERE a.applicationtypeid = @ApplicationTypeID
                                ORDER BY a.applicationdate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

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
                        LastErrorMessage = "Error getting applications by type: " + ex.Message;
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

        public static int GetActiveApplicationIDForLicenseClass(int PersonID, int ApplicationTypeID, int LicenseClassID)
        {
            int ApplicationID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT a.ApplicationID 
                         FROM Applications a
                         INNER JOIN LocalDrivingLicenseApplications l ON a.ApplicationID = l.ApplicationID
                         WHERE a.ApplicantPersonID = @PersonID 
                         AND a.ApplicationTypeID = @ApplicationTypeID 
                         AND l.LicenseClassID = @LicenseClassID 
                         AND a.ApplicationStatus = 1"; // 1 = New

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open(); object result = command.ExecuteScalar();
                        if (result != null) ApplicationID = (int)result;
                    }
                    catch { return -1; }
                }
            }
            return ApplicationID;
        }

        public static DataTable GetAllApplications()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT a.*, 
                                        ap.applicationtypetitle,
                                        p.firstname + ' ' + p.lastname as applicantname,
                                        u.username as createdbyusername
                                FROM applications a
                                INNER JOIN applicationtypes ap ON a.applicationtypeid = ap.applicationtypeid
                                INNER JOIN people p ON a.applicantpersonid = p.personid
                                INNER JOIN users u ON a.createdbyuserid = u.userid
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
                        LastErrorMessage = "Error getting all applications: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateApplication(
            int ApplicationID,
            int ApplicantPersonID,
            DateTime ApplicationDate,
            int ApplicationTypeID,
            short ApplicationStatus,
            DateTime LastStatusDate,
            decimal PaidFees,
            int CreatedByUserID
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE applications
                SET 
                    applicantpersonid = @ApplicantPersonID,
                    applicationdate = @ApplicationDate,
                    applicationtypeid = @ApplicationTypeID,
                    applicationstatus = @ApplicationStatus,
                    laststatusdate = @LastStatusDate,
                    paidfees = @PaidFees,
                    createdbyuserid = @CreatedByUserID
                WHERE applicationid = @ApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                    command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                    command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
                    command.Parameters.AddWithValue("@PaidFees", PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating application: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Update application status only
        public static bool UpdateApplicationStatus(int ApplicationID, short NewStatus)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE applications
                SET 
                    applicationstatus = @ApplicationStatus,
                    laststatusdate = @LastStatusDate
                WHERE applicationid = @ApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                    command.Parameters.AddWithValue("@ApplicationStatus", NewStatus);
                    command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating application status: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get total fees collected by application type
        public static decimal GetTotalFeesByApplicationType(int ApplicationTypeID)
        {
            decimal totalFees = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT ISNULL(SUM(paidfees), 0) 
                                FROM applications 
                                WHERE applicationtypeid = @ApplicationTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

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

        // Get applications count by status
        public static int GetApplicationsCountByStatus(short Status)
        {
            int count = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM applications WHERE applicationstatus = @ApplicationStatus";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationStatus", Status);

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
                        LastErrorMessage = "Error getting applications count: " + ex.Message;
                    }
                }
            }
            return count;
        }
    }
}