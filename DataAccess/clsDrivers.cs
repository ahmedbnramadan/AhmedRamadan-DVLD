using System;
using System.Data;
using Microsoft.Data.SqlClient;


namespace DataAccess
{
    public class clsDrivers
    {
        public static string LastErrorMessage = "";

        public static int AddNewDriver(
            int PersonID,
            int CreatedByUserID,
            DateTime CreatedDate
            )
        {
            int DriverID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = @"INSERT INTO drivers 
                            (personid, createdbyuserid , createddate)
                            VALUES (@PersonID, @CreatedByUserID, @CreatedDate);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    command.Parameters.AddWithValue("@CreatedDate", CreatedDate);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            DriverID = InsertedID;
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding Driver: " + ex.Message;
                    }
                }

            }
            return DriverID;

        }

        public static bool IsDriverExist(int DriverID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM drivers WHERE driverid = @DriverID";

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
                        LastErrorMessage = "Error is driver exsists: " + ex.Message;
                    }
                }

            }
            return isFound;
        }

        public static bool DeleteDriver(int DriverID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "DELETE FROM drivers WHERE driverid = @DriverID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting driver: " + ex.Message;
                    }
                }

            }
            return (rowAffected > 0);
        }

        public static bool GetDriverByID(
            int DriverID,
            ref int PersonID,
            ref int CreatedByUserID,
            ref DateTime CreatedDate
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT * FROM drivers WHERE driverid = @DriverID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                PersonID = (int)reader["personid"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                                CreatedDate = (DateTime)reader["createddate"];
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting driver by id: " + ex.Message;
                    }
                }
            }

            return isFound;
        }

        public static bool GetDriverByPersonID(
            ref int DriverID,
            int PersonID,
            ref int CreatedByUserID,
            ref DateTime CreatedDate
            )
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT * FROM drivers WHERE personid = @PersonID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                DriverID = (int)reader["driverid"];
                                CreatedByUserID = (int)reader["createdbyuserid"];
                                CreatedDate = (DateTime)reader["createddate"];
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting driver by person id: " + ex.Message;
                    }
                }
            }

            return isFound;
        }

        


        public static DataTable GetAllDrivers()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT * FROM drivers";
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
                        LastErrorMessage = "Error showing drivers : " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateDriver(
            int DriverID,
            int PersonID,
            int CreatedByUserID,
            DateTime CreatedDate
            )
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE drivers
                SET
                personid = @PersonID,
                createdbyuserid = @CreatedByUserID,
                createddate = @CreatedDate
                WHERE driverid = @DriverID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DriverID", DriverID);
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    command.Parameters.AddWithValue("@CreatedDate", CreatedDate);

                    try
                    {
                        connection.Open();

                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error Updating Driver: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

    }
}