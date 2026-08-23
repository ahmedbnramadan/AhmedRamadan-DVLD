using System;
using System.Data;
using Microsoft.Data.SqlClient;


namespace DataAccess
{
    public class clsUsers
    {
        public static string LastErrorMessage = "";

        public static int AddNewUser(
            int PersonID,
            string UserName,
            string Password,
            bool IsActive
            )
        {
            int UserID = -1;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = @"INSERT INTO users 
                            (personid, username, password, isactive)
                            VALUES (@PersonID, @UserName, @Password, @IsActive);
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@IsActive", IsActive);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            UserID = InsertedID;
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding user: " + ex.Message;
                    }
                }

            }
            return UserID;

        }

        public static bool IsUserExist(int UserID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM users WHERE userid = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@UserID", UserID);
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error is user exsist: " + ex.Message;
                    }
                }

            }
            return isFound;
        }

        public static bool IsUserExist(string UserName, string Password)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM users WHERE username = @UserName AND password = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error is user exsist: " + ex.Message;
                    }
                }
            }
            return isFound;

        }

        public static bool IsUserNameExist(string UserName)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM users WHERE username = @UserName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error is user exsist: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool IsUserExistForPersonID(int PersonID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM users WHERE personid = @PersonID";

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
                        LastErrorMessage = "Error is user exsist: " + ex.Message;
                    }
                }

            }
            return isFound;
        }


        public static bool DeleteUser(int UserID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "DELETE FROM users WHERE userid = @UserID";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting user: " + ex.Message;
                    }
                }


            }
            return (rowAffected > 0);
        }

        public static bool GetUserByID(
            int UserID,
            ref int PersonID,
            ref string UserName,
            ref string Password,
            ref bool IsActive)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT * FROM users WHERE userid = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                PersonID = (int)reader["personid"];
                                UserName = (string)reader["username"];
                                Password = (string)reader["password"];
                                IsActive = (bool)reader["isactive"];

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting user by id: " + ex.Message;
                    }
                }
            }

            return isFound;
        }

        public static bool GetUserInfoByUserName(ref int UserID, ref int PersonID, string UserName, ref string Password, ref bool IsActive)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT * FROM users WHERE username = @UserName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                UserID = (int)reader["userid"];
                                PersonID = (int)reader["personid"];
                                Password = (string)reader["password"];
                                IsActive = (bool)reader["isactive"];

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting user by id: " + ex.Message;
                    }
                }
            }

            return isFound;
        }

        public static bool GetUserInfoByPersonID(ref int UserID, int PersonID, ref string UserName, ref string Password, ref bool IsActive)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM users WHERE personid = @PersonID";

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

                                UserID = (int)reader["userid"];
                                UserName = (string)reader["username"];
                                Password = (string)reader["password"];
                                IsActive = (bool)reader["isactive"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error in GetUserInfoByPersonID: " + ex.Message;
                        isFound = false;
                    }
                }
            }
            return isFound;
        }
        public static bool GetUserInfoByUserNameAndPassword(ref int UserID, ref int PersonID, string UserName, string Password, ref bool IsActive)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT * FROM users WHERE username = @UserName and password = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                UserID = (int)reader["userid"];
                                PersonID = (int)reader["personid"];
                                IsActive = (bool)reader["isactive"];

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error in GetUserInfoByUserNameAndPassword: " + ex.Message;
                    }
                }
            }

            return isFound;
        }



        public static DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = @"SELECT users.UserID, users.PersonID, 
                                FullName = CONCAT(People.FirstName, ' ', People.SecondName, ' ', People.ThirdName, ' ', People.LastName),
                                users.UserName, users.IsActive, People.Phone, People.Email
                                FROM users 
                                INNER JOIN People ON users.PersonID = People.PersonID";
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
                        LastErrorMessage = "Error showing users : " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdateUser(
            int UserID,
            int PersonID,
            string UserName,
            string Password,
            bool IsActive)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE users
                SET
                username = @UserName,
                password = @Password,
                isactive = @IsActive
                WHERE userid = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@IsActive", IsActive);

                    try
                    {
                        connection.Open();

                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error Updating User: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            int rowAffected = 0;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                // استعلام بسيط ومباشر لتحديث كلمة المرور فقط
                string query = "UPDATE users SET password = @NewPassword WHERE userid = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@NewPassword", NewPassword);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error changing password: " + ex.Message;
                    }
                }
            }

            return (rowAffected > 0);
        }

    }
}