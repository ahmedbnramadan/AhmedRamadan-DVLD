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

        public static int GetCountryIDByName(string CountryName)
        {
            int ID = -1;

            if (DataAccess.clsCountries.GetCountryByName(ref ID, CountryName))
            {
                return ID;
            }

            return -1;
        }

        public static string GetCountryNameByID(int CountryID)
        {
            string Name = "";

            if (DataAccess.clsCountries.GetCountryByID(CountryID,ref  Name))
            {
                return Name;
            }
            
            return null;
        }

        public static int GetDefaultCountryIDBySystemLocale()
        {
            try
            {
                System.Globalization.RegionInfo region = System.Globalization.RegionInfo.CurrentRegion;
                string countryName = region.EnglishName;

                // Try to find country by its English name from system locale
                int id = GetCountryIDByName(countryName);

                if (id != -1)
                    return id;

                // If exact match fails, try common variations
                // Some systems might have different naming conventions
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        public static DataTable GetAllCountries()
        {
            return DataAccess.clsCountries.GetAllCountries();
        }
    }
} 