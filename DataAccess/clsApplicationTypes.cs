using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsApplicationTypes
    {
        public static string LastErrorMessage = "";

        public static int AddNewApplicationType(
            int ApplicationTypeID,
            string ApplicationTypeTitle,
            decimal ApplicationFees
            )
        {
            int insertedID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO applicationtypes 
                            (applicationtypeid, applicationtypetitle, applicationfees)
                            VALUES (@ApplicationTypeID, @ApplicationTypeTitle, @ApplicationFees);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                    command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            insertedID = InsertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding application type: " + ex.Message;
                    }
                }
            }
            return insertedID;
        }

        public static bool IsApplicationTypeExist(int ApplicationTypeID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM applicationtypes WHERE applicationtypeid = @ApplicationTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking application type existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsApplicationTypeExist(string ApplicationTypeTitle)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM applicationtypes WHERE applicationtypetitle = @ApplicationTypeTitle";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking application type title existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteApplicationType(int ApplicationTypeID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM applicationtypes WHERE applicationtypeid = @ApplicationTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting application type: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetApplicationTypeByID(
            int ApplicationTypeID,
            ref string ApplicationTypeTitle,
            ref decimal ApplicationFees
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM applicationtypes WHERE applicationtypeid = @ApplicationTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                ApplicationTypeTitle = (string)reader["applicationtypetitle"];
                                ApplicationFees = (decimal)reader["applicationfees"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting application type by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetApplicationTypeByTitle(
            ref int ApplicationTypeID,
            string ApplicationTypeTitle,
            ref decimal ApplicationFees
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM applicationtypes WHERE applicationtypetitle = @ApplicationTypeTitle";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                ApplicationTypeID = (int)reader["applicationtypeid"];
                                ApplicationFees = (decimal)reader["applicationfees"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting application type by title: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetAllApplicationTypes()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM applicationtypes ORDER BY applicationtypetitle";

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
                        LastErrorMessage = "Error getting all application types: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateApplicationType(
            int ApplicationTypeID,
            string ApplicationTypeTitle,
            decimal ApplicationFees
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE applicationtypes
                SET 
                    applicationtypetitle = @ApplicationTypeTitle,
                    applicationfees = @ApplicationFees
                WHERE applicationtypeid = @ApplicationTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationTypeTitle", ApplicationTypeTitle);
                    command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating application type: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool UpdateApplicationFees(
            int ApplicationTypeID,
            string ApplicationTypeTitle,
            decimal ApplicationFees
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE applicationtypes
                SET 
                    applicationfees = @ApplicationFees";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationFees", ApplicationFees);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating application fees: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get application fees by ID
        public static decimal GetApplicationFees(int ApplicationTypeID)
        {
            decimal fees = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT applicationfees FROM applicationtypes WHERE applicationtypeid = @ApplicationTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && decimal.TryParse(result.ToString(), out decimal feesValue))
                        {
                            fees = feesValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting application fees: " + ex.Message;
                    }
                }
            }
            return fees;
        }


    }
}