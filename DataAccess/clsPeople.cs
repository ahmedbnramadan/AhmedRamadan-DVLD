using System;
using System.Data;
using Microsoft.Data.SqlClient;


namespace DataAccess
{
    public class clsPeople
    {
        public static string LastErrorMessage = "";

        public static int AddNewPerson(
            string NationalNO,
            string FirstName,
            string SecondName,
            string ThirdName,
            string LastName,
            DateTime DateofBirth,
            short Gender,
            string Address,
            string Phone,
            string Email,
            int NationalityCountryID,
            string ImagePath)
        {
            int PersonID = -1;
            LastErrorMessage = "";

            // this way the connection will close automaticlly.
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = @"INSERT INTO people (nationalno, firstname, secondname, thirdname, lastname, 
                dateofbirth, gender, address, phone, email, nationalitycountryid, imagepath)
                VALUES (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, 
                @DateofBirth, @Gender, @Address, @Phone, @Email, 
                @NationalityCountryID, @ImagePath);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNO);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? DBNull.Value : (object)ThirdName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@DateofBirth", DateofBirth);
                    command.Parameters.AddWithValue("@Gender", Gender);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email);
                    command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? DBNull.Value : (object)ImagePath);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            PersonID = InsertedID;
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error adding person: " + ex.Message;
                    }
                }

            }
            return PersonID;

        }

        public static bool IsPersonExist(int ID)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM people WHERE personid = @ID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);


                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error is person exsist: " + ex.Message;
                    }
                }

            }
            return isFound;
        }

        public static bool IsPersonExist(string NationalNO)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT 1 FROM people WHERE nationalno = @NationalNO";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNO", NationalNO);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        isFound = (result != null);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error checking if person exists by National No: " + ex.Message;
                    }
                }
            }
            return isFound;
        }

        public static bool DeletePerson(int ID)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "DELETE FROM people WHERE personid = @ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);

                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error deleting person: " + ex.Message;
                    }

                }

            }
            return (rowAffected > 0);
        }

        public static bool GetPersonByID(
            int ID,
            ref string NationalNO,
            ref string FirstName,
            ref string SecondName,
            ref string ThirdName,
            ref string LastName,
            ref DateTime DateofBirth,
            ref short Gender,
            ref string Address,
            ref string Phone,
            ref string Email,
            ref int NationalityCountryID,
            ref string ImagePath)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT * FROM people WHERE personid = @ID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                NationalNO = (string)reader["nationalno"];
                                FirstName = (string)reader["firstname"];
                                SecondName = (string)reader["secondname"];
                                ThirdName = (reader["thirdname"] != DBNull.Value) ? (string)reader["thirdname"] : "";
                                LastName = (string)reader["lastname"];
                                DateofBirth = (DateTime)reader["dateofbirth"];
                                Gender = Convert.ToInt16(reader["gender"]);
                                Address = (string)reader["address"];
                                Phone = (string)reader["phone"];
                                Email = (reader["email"] != DBNull.Value) ? (string)reader["email"] : "";
                                NationalityCountryID = (int)reader["nationalitycountryid"];
                                ImagePath = (reader["imagepath"] != DBNull.Value) ? (string)reader["imagepath"] : "";

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error get person by id: " + ex.Message;
                    }
                }
            }

            return isFound;
        }

        public static bool GetPersonByNationalNo(
            ref int PersonID,
            string NationalNO,
            ref string FirstName,
            ref string SecondName,
            ref string ThirdName,
            ref string LastName,
            ref DateTime DateofBirth,
            ref short Gender,
            ref string Address,
            ref string Phone,
            ref string Email,
            ref int NationalityCountryID,
            ref string ImagePath)
        {
            bool isFound = false;
            LastErrorMessage = "";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT * FROM people WHERE nationalno = @NationalNO";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNO", NationalNO);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                PersonID = (int)reader["personid"];
                                FirstName = (string)reader["firstname"];
                                SecondName = (string)reader["secondname"];
                                ThirdName = (reader["thirdname"] != DBNull.Value) ? (string)reader["thirdname"] : "";
                                LastName = (string)reader["lastname"];
                                DateofBirth = (DateTime)reader["dateofbirth"];
                                Gender = Convert.ToInt16(reader["gender"]);
                                Address = (string)reader["address"];
                                Phone = (string)reader["phone"];
                                Email = (reader["email"] != DBNull.Value) ? (string)reader["email"] : "";
                                NationalityCountryID = (int)reader["nationalitycountryid"];
                                ImagePath = (reader["imagepath"] != DBNull.Value) ? (string)reader["imagepath"] : "";

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error getting person by nationalno : " + ex.Message;
                    }
                }
            }

            return isFound;
        }

        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT * FROM people";
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
                        LastErrorMessage = "Error showing people : " + ex.Message;
                    }
                }
            }
            return dt;
        }

        public static bool UpdatePerson(int ID,
            string NationalNO,
            string FirstName,
            string SecondName,
            string ThirdName,
            string LastName,
            DateTime DateofBirth,
            short Gender,
            string Address,
            string Phone,
            string Email,
            int NationalityCountryID,
            string ImagePath)
        {
            LastErrorMessage = "";
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                UPDATE people
                SET
                nationalno = @NationalNO,
                firstname = @FirstName,
                secondname = @SecondName,
                thirdname = @ThirdName,
                lastname = @LastName,
                dateofbirth = @DateofBirth,
                gender = @Gender,
                address = @Address,
                phone = @Phone,
                email = @Email,
                nationalitycountryid = @NationalityCountryID,
                imagepath = @ImagePath
                WHERE personid = @ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@NationalNO", NationalNO);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? DBNull.Value : (object)ThirdName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@DateofBirth", DateofBirth);
                    command.Parameters.AddWithValue("@Gender", Gender);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email);
                    command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? DBNull.Value : (object)ImagePath);

                    try
                    {
                        connection.Open();

                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = "Error Updating Person: " + ex.Message;
                    }
                }
            }
            return (rowAffected > 0);
        }

    }
}