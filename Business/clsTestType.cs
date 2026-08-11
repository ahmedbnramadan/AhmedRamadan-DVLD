using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsTestType
    {
        public enum enMode 
        { 
            AddNew = 0, 
            Update = 1 
        };

        public enMode Mode = enMode.Update;

        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Fees { get; set; }

        public enum enTestType 
        { 
            Vision = 1, 
            Written = 2, 
            Practical = 3 
        };

        public clsTestType()
        {
            this.ID = -1;
            this.Title = "";
            this.Description = "";
            this.Fees = 0;

            this.Mode = enMode.AddNew;
        }

        private clsTestType(
            int ID, 
            string Title, 
            string Description, 
            decimal Fees
        )
        {
            this.ID = ID;
            this.Title = Title;
            this.Description = Description;
            this.Fees = Fees;

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            // منع تكرار العنوان
            if (clsTestType.IsTestTypeExist(this.Title))
            {
                return false;
            }

            this.ID = DataAccess.clsTestTypes.AddNewTestType(
                this.ID, 
                this.Title, 
                this.Description, 
                this.Fees
            );

            return (this.ID != -1);
        }

        private bool _Update()
        {
            return DataAccess.clsTestTypes.UpdateTestType(
                this.ID, 
                this.Title, 
                this.Description, 
                this.Fees
            );
        }

        public static clsTestType Find(int ID)
        {
            string Title = "";
            string Description = "";
            decimal Fees = 0;

            if (DataAccess.clsTestTypes.GetTestTypeByID(
                ID, 
                ref Title, 
                ref Description, 
                ref Fees
            ))
            {
                return new clsTestType(
                    ID, 
                    Title, 
                    Description, 
                    Fees
                );
            }
            else
            {
                return null;
            }
        }

        public static clsTestType Find(string Title)
        {
            int ID = -1;
            string Description = "";
            decimal Fees = 0;

            if (DataAccess.clsTestTypes.GetTestTypeByTitle(
                Title, 
                ref ID, 
                ref Description, 
                ref Fees
            ))
            {
                return new clsTestType(
                    ID, 
                    Title, 
                    Description, 
                    Fees
                );
            }
            else
            {
                return null;
            }
        }

        public bool Save()
        {
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

        public static DataTable GetAllTestTypes()
        {
            return DataAccess.clsTestTypes.GetAllTestTypes();
        }

        public static bool IsTestTypeExist(int ID)
        {
            return DataAccess.clsTestTypes.IsTestTypeExist(ID);
        }

        public static bool IsTestTypeExist(string Title)
        {
            return DataAccess.clsTestTypes.IsTestTypeExist(Title);
        }

        public static decimal GetFees(int ID)
        {
            return DataAccess.clsTestTypes.GetTestFees(ID);
        }
    }
}