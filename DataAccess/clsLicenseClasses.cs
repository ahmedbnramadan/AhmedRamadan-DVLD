using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsLicenseClasses
    {
        public static string LastErrorMessage = "";

        public static int AddNewLicenseClass(
    int LicenseClassID,
    string ClassName,
    string ClassDescription,
    short MinimumAllowedAge,
    short DefaultValidityLength,
    decimal ClassFees
    )
        {
            int insertedID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = @"INSERT INTO licenseclasses 
                        (licenseclassid, classname, classdescription, minimumallowedage, 
                         defaultvaliditylength, classfees)
                        VALUES 
                        (@LicenseClassID, @ClassName, @ClassDescription, @MinimumAllowedAge, 
                         @DefaultValidityLength, @ClassFees);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                    command.Parameters.AddWithValue("@ClassName", ClassName);
                    command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
                    command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                    command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                    command.Parameters.AddWithValue("@ClassFees", ClassFees);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            insertedID = LicenseClassID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding license class: " + ex.Message;
                    }
                }
            }
            return insertedID;
        }

        public static bool IsLicenseClassExist(int LicenseClassID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM licenseclasses WHERE licenseclassid = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking license class existence by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsLicenseClassExist(string ClassName)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM licenseclasses WHERE classname = @ClassName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", ClassName);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking license class existence by name: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteLicenseClass(int LicenseClassID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM licenseclasses WHERE licenseclassid = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting license class: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetLicenseClassByID(
            int LicenseClassID,
            ref string ClassName,
            ref string ClassDescription,
            ref short MinimumAllowedAge,
            ref short DefaultValidityLength,
            ref decimal ClassFees
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM licenseclasses WHERE licenseclassid = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                ClassName = (string)reader["classname"];
                                ClassDescription = (string)reader["classdescription"];
                                MinimumAllowedAge = (short)reader["minimumallowedage"];
                                DefaultValidityLength = (short)reader["defaultvaliditylength"];
                                ClassFees = (decimal)reader["classfees"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting license class by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetLicenseClassByName(
            ref int LicenseClassID,
            string ClassName,
            ref string ClassDescription,
            ref short MinimumAllowedAge,
            ref short DefaultValidityLength,
            ref decimal ClassFees
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM licenseclasses WHERE classname = @ClassName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", ClassName);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                LicenseClassID = (int)reader["licenseclassid"];
                                ClassDescription = (string)reader["classdescription"];
                                MinimumAllowedAge = (short)reader["minimumallowedage"];
                                DefaultValidityLength = (short)reader["defaultvaliditylength"];
                                ClassFees = (decimal)reader["classfees"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting license class by name: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetAllLicenseClasses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM licenseclasses ORDER BY classname";

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
                        LastErrorMessage = "Error getting all license classes: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateLicenseClass(
            int LicenseClassID,
            string ClassName,
            string ClassDescription,
            short MinimumAllowedAge,
            short DefaultValidityLength,
            decimal ClassFees
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE licenseclasses
                SET 
                    classname = @ClassName,
                    classdescription = @ClassDescription,
                    minimumallowedage = @MinimumAllowedAge,
                    defaultvaliditylength = @DefaultValidityLength,
                    classfees = @ClassFees
                WHERE licenseclassid = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                    command.Parameters.AddWithValue("@ClassName", ClassName);
                    command.Parameters.AddWithValue("@ClassDescription", ClassDescription);
                    command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                    command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                    command.Parameters.AddWithValue("@ClassFees", ClassFees);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating license class: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Get class fees by ID
        public static decimal GetClassFees(int LicenseClassID)
        {
            decimal fees = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT classfees FROM licenseclasses WHERE licenseclassid = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

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
                        LastErrorMessage = "Error getting class fees: " + ex.Message;
                    }
                }
            }
            return fees;
        }

        // Get minimum allowed age by ID
        public static short GetMinimumAllowedAge(int LicenseClassID)
        {
            short age = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT minimumallowedage FROM licenseclasses WHERE licenseclassid = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && short.TryParse(result.ToString(), out short ageValue))
                        {
                            age = ageValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting minimum allowed age: " + ex.Message;
                    }
                }
            }
            return age;
        }

        // Get default validity length by ID
        public static short GetDefaultValidityLength(int LicenseClassID)
        {
            short validity = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT defaultvaliditylength FROM licenseclasses WHERE licenseclassid = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && short.TryParse(result.ToString(), out short validityValue))
                        {
                            validity = validityValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting default validity length: " + ex.Message;
                    }
                }
            }
            return validity;
        }

        // Update class fees only
        public static bool UpdateClassFees(int LicenseClassID, decimal NewFees)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE licenseclasses
                SET classfees = @ClassFees
                WHERE licenseclassid = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                    command.Parameters.AddWithValue("@ClassFees", NewFees);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating class fees: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        // Update minimum allowed age only
        public static bool UpdateMinimumAllowedAge(int LicenseClassID, short NewAge)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE licenseclasses
                SET minimumallowedage = @MinimumAllowedAge
                WHERE licenseclassid = @LicenseClassID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                    command.Parameters.AddWithValue("@MinimumAllowedAge", NewAge);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating minimum allowed age: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }
    }
}