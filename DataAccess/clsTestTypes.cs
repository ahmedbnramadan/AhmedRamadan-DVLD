using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsTestTypes
    {
        public static string LastErrorMessage = "";

        // Test Type Constants
        public const int TestTypeVision = 1;
        public const int TestTypeWritten = 2;
        public const int TestTypePractical = 3;

        public static int AddNewTestType(
            int TestTypeID,
            string TestTypeTitle,
            string TestTypeDescription,
            decimal TestTypeFees
            )
        {
            int insertedID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO testtypes 
                            (testtypeid, testtypetitle, testtypedescription, testtypefees)
                            VALUES 
                            (@TestTypeID, @TestTypeTitle, @TestTypeDescription, @TestTypeFees);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@TestTypeTitle", TestTypeTitle);
                    command.Parameters.AddWithValue("@TestTypeDescription", TestTypeDescription);
                    command.Parameters.AddWithValue("@TestTypeFees", TestTypeFees);

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
                        LastErrorMessage = "Error adding test type: " + ex.Message;
                    }
                }
            }
            return insertedID;
        }

        public static bool IsTestTypeExist(int TestTypeID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM testtypes WHERE testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking test type existence by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsTestTypeExist(string TestTypeTitle)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM testtypes WHERE testtypetitle = @TestTypeTitle";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeTitle", TestTypeTitle);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking test type existence by title: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteTestType(int TestTypeID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM testtypes WHERE testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting test type: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetTestTypeByID(
            int TestTypeID,
            ref string TestTypeTitle,
            ref string TestTypeDescription,
            ref decimal TestTypeFees
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM testtypes WHERE testtypeid = @TestTypeID";

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
                                isFound = true;

                                TestTypeTitle = (string)reader["testtypetitle"];
                                TestTypeDescription = (string)reader["testtypedescription"];
                                TestTypeFees = (decimal)reader["testtypefees"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test type by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetTestTypeByTitle(
            string TestTypeTitle,
            ref int TestTypeID,
            ref string TestTypeDescription,
            ref decimal TestTypeFees
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM testtypes WHERE testtypetitle = @TestTypeTitle";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeTitle", TestTypeTitle);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                TestTypeID = (int)reader["testtypeid"];
                                TestTypeDescription = (string)reader["testtypedescription"];
                                TestTypeFees = (decimal)reader["testtypefees"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test type by title: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetAllTestTypes()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM testtypes ORDER BY testtypeid";

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
                        LastErrorMessage = "Error getting all test types: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateTestType(
            int TestTypeID,
            string TestTypeTitle,
            string TestTypeDescription,
            decimal TestTypeFees
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE testtypes
                SET 
                    testtypetitle = @TestTypeTitle,
                    testtypedescription = @TestTypeDescription,
                    testtypefees = @TestTypeFees
                WHERE testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@TestTypeTitle", TestTypeTitle);
                    command.Parameters.AddWithValue("@TestTypeDescription", TestTypeDescription);
                    command.Parameters.AddWithValue("@TestTypeFees", TestTypeFees);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating test type: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get test fees by ID
        public static decimal GetTestFees(int TestTypeID)
        {
            decimal fees = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT testtypefees FROM testtypes WHERE testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

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
                        LastErrorMessage = "Error getting test fees: " + ex.Message;
                    }
                }
            }
            return fees;
        }

        // Get test title by ID
        public static string GetTestTitle(int TestTypeID)
        {
            string title = "";
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT testtypetitle FROM testtypes WHERE testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            title = result.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test title: " + ex.Message;
                    }
                }
            }
            return title;
        }

        // Get test description by ID
        public static string GetTestDescription(int TestTypeID)
        {
            string description = "";
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT testtypedescription FROM testtypes WHERE testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            description = result.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting test description: " + ex.Message;
                    }
                }
            }
            return description;
        }

        // Update test fees only
        public static bool UpdateTestFees(int TestTypeID, decimal NewFees)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE testtypes
                SET testtypefees = @TestTypeFees
                WHERE testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@TestTypeFees", NewFees);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating test fees: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Update test title only
        public static bool UpdateTestTitle(int TestTypeID, string NewTitle)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE testtypes
                SET testtypetitle = @TestTypeTitle
                WHERE testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@TestTypeTitle", NewTitle);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating test title: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Update test description only
        public static bool UpdateTestDescription(int TestTypeID, string NewDescription)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE testtypes
                SET testtypedescription = @TestTypeDescription
                WHERE testtypeid = @TestTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@TestTypeDescription", NewDescription);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating test description: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }
    }
}