using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsApplicationType
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public string Title { get; set; }
        public decimal Fees { get; set; }

        public clsApplicationType()
        {
            this.ID = -1;
            this.Title = "";
            this.Fees = -1;

            this.Mode = enMode.AddNew;
        }

        private clsApplicationType(int ID, string Title, decimal Fees)
        {
            this.ID = ID;
            this.Title = Title;
            this.Fees = Fees;

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.Title = this.Title.Trim();

            this.ID = DataAccess.clsApplicationTypes.AddNewApplicationType(this.ID, this.Title, this.Fees);
            return (this.ID != -1);
        }

        private bool _Update()
        {
            this.Title = this.Title.Trim();

            return (DataAccess.clsApplicationTypes.UpdateApplicationType(this.ID, this.Title, this.Fees));
        }
        
        public static clsApplicationType Find(int ApplicationID)
        {
            string Title = "";
            decimal Fees = -1;

            if (DataAccess.clsApplicationTypes.GetApplicationTypeByID(ApplicationID, ref Title, ref Fees))
            {
                return new clsApplicationType(ApplicationID, Title, Fees);

            }
            else return null;
        }

        public static clsApplicationType Find(string ApplicationTitle)
        {
            int ID = -1;
            decimal Fees = -1;

            ApplicationTitle = ApplicationTitle.Trim();

            if (DataAccess.clsApplicationTypes.GetApplicationTypeByTitle(ref ID, ApplicationTitle, ref Fees))
            {
                return new clsApplicationType(ID, ApplicationTitle, Fees);
            }
            else return null;
        }


        public static decimal GetApplicationFees(int ApplicationTypeID)
        {
            if (ApplicationTypeID <= 0) return -1;
            return DataAccess.clsApplicationTypes.GetApplicationFees(ApplicationTypeID);
        }

        public static DataTable GetAllApplicationTypes()
        {
            return DataAccess.clsApplicationTypes.GetAllApplicationTypes();
        }

        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(this.Title) || this.Fees < 0) return false;

            switch (Mode)
            {

                case enMode.AddNew:
                    if (_AddNew())
                    {
                        this.Mode = enMode.Update;
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