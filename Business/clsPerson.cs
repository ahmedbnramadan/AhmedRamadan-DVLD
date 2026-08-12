using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsPerson
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public string NationalNo { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public short Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int NationalityCountryID { get; set; }
        public string ImagePath { get; set; }


        public string FullName => FirstName + " " + SecondName + " " + ThirdName + " " + LastName;
        public clsCountry CountryInfo;



        // if someone outside make a new clsPerson it gives him default values
        public clsPerson()
        {
            this.ID = -1;
            this.NationalNo = "";
            this.FirstName = "";
            this.SecondName = "";
            this.ThirdName = "";
            this.LastName = "";
            this.DateOfBirth = DateTime.Now;
            this.Gender = 0;
            this.Address = "";
            this.Phone = "";
            this.Email = "";
            this.NationalityCountryID = -1;
            this.ImagePath = "";
        }

        // if you make a new clsPerson you give the values, that change all parameters here
        private clsPerson(int ID,
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
            this.ID = ID;
            this.NationalNo = NationalNO;
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.ThirdName = ThirdName;
            this.LastName = LastName;
            this.DateOfBirth = DateOfBirth;
            this.Gender = Gender;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.NationalityCountryID = NationalityCountryID;
            this.ImagePath = ImagePath;
            this.CountryInfo = clsCountry.Find(NationalityCountryID);

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.ID = DataAccess.clsPeople.AddNewPerson(
                this.NationalNo,
                this.FirstName,
                this.SecondName,
                this.ThirdName,
                this.LastName,
                this.DateOfBirth,
                this.Gender,
                this.Address,
                this.Phone,
                this.Email,
                this.NationalityCountryID,
                this.ImagePath);

            return (this.ID != -1);
        }

        private bool _Update()
        {
            return (DataAccess.clsPeople.UpdatePerson(this.ID, this.NationalNo, this.FirstName,this.SecondName,
            this.ThirdName, this.LastName, this.DateOfBirth, this.Gender, this.Address, this.Phone,
            this.Email, this.NationalityCountryID, this.ImagePath));

        }

        public static clsPerson Find(int ID)
        {
            string NationalNo = "";
            string FirstName = "";
            string SecondName = "";
            string ThirdName = "";
            string LastName = "";
            DateTime DateofBirth = DateTime.Now;
            short Gender = 0;
            string Address = "";
            string Phone = "";
            string Email = "";
            int NationalityCountryID = -1;
            string ImagePath = "";

            if (DataAccess.clsPeople.GetPersonByID(ID,
            ref NationalNo,
            ref FirstName,
            ref SecondName,
            ref ThirdName,
            ref LastName,
            ref DateofBirth,
            ref Gender,
            ref Address,
            ref Phone,
            ref Email,
            ref NationalityCountryID,
            ref ImagePath))
            {
                return new clsPerson(ID, NationalNo, FirstName, SecondName, ThirdName, LastName, DateofBirth,
                Gender, Address, Phone, Email, NationalityCountryID, ImagePath);

            }
            else
            {
                return null;
            }
        }

        public static clsPerson Find(string NationalNo)
        {
            int ID = -1;
            string FirstName = "";
            string SecondName = "";
            string ThirdName = "";
            string LastName = "";
            DateTime DateofBirth = DateTime.Now;
            short Gender = 0;
            string Address = "";
            string Phone = "";
            string Email = "";
            int NationalityCountryID = -1;
            string ImagePath = "";

            if (DataAccess.clsPeople.GetPersonByNationalNo(
            ref ID,
            NationalNo,
            ref FirstName,
            ref SecondName,
            ref ThirdName,
            ref LastName,
            ref DateofBirth,
            ref Gender,
            ref Address,
            ref Phone,
            ref Email,
            ref NationalityCountryID,
            ref ImagePath))
            {
                return new clsPerson(ID, NationalNo, FirstName, SecondName, ThirdName, LastName, DateofBirth,
                Gender, Address, Phone, Email, NationalityCountryID, ImagePath);

            }
            else
            {
                return null;
            }
        }

        public static bool Delete(int ID)
        {
            return (DataAccess.clsPeople.DeletePerson(ID));
        }

        public static bool IsExists(int ID)
        {
            return (DataAccess.clsPeople.IsPersonExist(ID));
        }

        public static bool IsExists(string NationalNo)
        {
            return (DataAccess.clsPeople.IsPersonExist(NationalNo));
        }

        public static DataTable GetAllPeople()
        {
            return DataAccess.clsPeople.GetAllPeople();
        }


        public bool Save()
        {
            if (this.DateOfBirth > DateTime.Now.AddYears(-18))
            {
                return false;
            }

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNew())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    break;

                case enMode.Update:
                    return _Update();
            }

            return false;
        }


        public string CountryName
        {
            get
            {
                clsCountry Country = clsCountry.Find(this.NationalityCountryID);

                return (Country != null) ? Country.CountryName : "[Unknown]";

            }
        }


    }
} //dotnet build