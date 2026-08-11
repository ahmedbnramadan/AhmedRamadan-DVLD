using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsCountry
    {
        public int CountryID {get; set; }
        public string CountryName {get; set; }

        public clsCountry()
        {
            CountryID = -1;
            CountryName = "";
        }

        private clsCountry (int CountryID, string CountryName)
        {
            this.CountryID = CountryID;
            this.CountryName = CountryName;
        }

        public static clsCountry Find (int CountryID)
        {
            string CountryName = "";

            if(DataAccess.clsCountries.GetCountryByID(CountryID, ref CountryName))
            return new clsCountry(CountryID, CountryName);
            else return null;
        }

        public static clsCountry Find (string CountryName)
        {
            int CountryID = -1;

            if(DataAccess.clsCountries.GetCountryByName(ref CountryID, CountryName))
            return new clsCountry(CountryID, CountryName);
            else return null;
        }

        public static DataTable GetAllCountries()
        {
            return DataAccess.clsCountries.GetAllCountries();
        }
    }
} // dotnet build