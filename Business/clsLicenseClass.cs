using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsLicenseClass
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short MinimumAllowedAge { get; set; }
        public short DefaultValidityLength { get; set; }
        public decimal Fees { get; set; }

        public clsLicenseClass()
        {
            this.ID = -1;
            this.Name = "";
            this.Description = "";
            this.MinimumAllowedAge = -1;
            this.DefaultValidityLength = -1;
            this.Fees = -1;

            this.Mode = enMode.AddNew;
        }

        private clsLicenseClass(int ID, string Name, string Description, short MinimumAllowedAge, short DefaultValidityLength, decimal Fees)
        {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.MinimumAllowedAge = MinimumAllowedAge;
            this.DefaultValidityLength = DefaultValidityLength;
            this.Fees = Fees;

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.Name = this.Name.Trim();
            this.Description = this.Description.Trim();

            this.ID = DataAccess.clsLicenseClasses.AddNewLicenseClass(this.ID, this.Name, this.Description, this.MinimumAllowedAge,
            this.DefaultValidityLength, this.Fees);
            return (this.ID != -1);
        }

        private bool _Update()
        {
            this.Name = this.Name.Trim();
            this.Description = this.Description.Trim();


            return (DataAccess.clsLicenseClasses.UpdateLicenseClass(this.ID, this.Name, this.Description, this.MinimumAllowedAge,
            this.DefaultValidityLength, this.Fees));
        }

        public static clsLicenseClass Find(int ID)
        {
            string Name = "";
            string Description = "";
            short MinimumAllowedAge = -1;
            short DefaultValidityLength = -1;
            decimal Fees = -1;

            if (DataAccess.clsLicenseClasses.GetLicenseClassByID(ID, ref Name, ref Description, ref MinimumAllowedAge, ref DefaultValidityLength, ref Fees))
                return new clsLicenseClass(ID, Name, Description, MinimumAllowedAge, DefaultValidityLength, Fees);
            else return null;
        }

        public static clsLicenseClass Find(string Name)
        {
            Name = Name.Trim();

            int ID = -1;
            string Description = "";
            short MinimumAllowedAge = -1;
            short DefaultValidityLength = -1;
            decimal Fees = -1;

            if (DataAccess.clsLicenseClasses.GetLicenseClassByName(ref ID, Name, ref Description, ref MinimumAllowedAge, ref DefaultValidityLength, ref Fees))
                return new clsLicenseClass(ID, Name, Description, MinimumAllowedAge, DefaultValidityLength, Fees);
            else return null;
        }

        public static bool IsExist(int ID)
        {
            return DataAccess.clsLicenseClasses.IsLicenseClassExist(ID);
        }

        public static bool IsExist(string Name)
        {
            Name = Name.Trim();
            return DataAccess.clsLicenseClasses.IsLicenseClassExist(Name.Trim());
        }

        // Make GetAllLicenseClasses
        public static DataTable GetAllLicenseClasses()
        {
            return DataAccess.clsLicenseClasses.GetAllLicenseClasses();
        }

        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(this.Name) || this.MinimumAllowedAge < 16 || this.DefaultValidityLength < 0 || this.Fees < 0) return false;

            switch (Mode)
            {
                case enMode.AddNew:
                    if (clsLicenseClass.IsExist(this.Name))
                    {
                        return false;
                    }
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

    }
}