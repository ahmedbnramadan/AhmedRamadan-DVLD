using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccess
{
    public class clsCountries
    {
        public static string LastErrorMessage = "";

        public static int AddNewCountry(string CountryName)
        {
            int CountryID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO countries 
                                (countryname)
                                VALUES (@CountryName);
                                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CountryName", CountryName);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            CountryID = InsertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding country: " + ex.Message;
                    }
                }
            }
            return CountryID;
        }

        public static bool IsCountryExist(int CountryID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM countries WHERE countryid = @CountryID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CountryID", CountryID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking country existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsCountryExist(string CountryName)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM countries WHERE countryname = @CountryName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CountryName", CountryName);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking country existence: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeleteCountry(int CountryID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM countries WHERE countryid = @CountryID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CountryID", CountryID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting country: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool GetCountryByID(int CountryID, ref string CountryName)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM countries WHERE countryid = @CountryID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CountryID", CountryID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                CountryName = (string)reader["countryname"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting country by ID: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool GetCountryByName(ref int CountryID, string CountryName)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM countries WHERE countryname = @CountryName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CountryName", CountryName);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                CountryID = (int)reader["countryid"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting country by name: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static DataTable GetAllCountries()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM countries ORDER BY countryname";

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
                        LastErrorMessage = "Error getting all countries: " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateCountry(int CountryID, string CountryName)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE countries
                SET countryname = @CountryName
                WHERE countryid = @CountryID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CountryID", CountryID);
                    command.Parameters.AddWithValue("@CountryName", CountryName);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error updating country: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }
    }
}