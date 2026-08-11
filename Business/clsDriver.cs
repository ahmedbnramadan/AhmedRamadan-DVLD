using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsDriver
    {

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public int PersonID { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDate { get; set; }

        public clsPerson PersonInfo;
        public clsUser CreatedByUserInfo;

        public clsDriver()
        {
            this.ID = -1;
            this.PersonID = -1;
            this.CreatedByUserID = -1;
            this.CreatedDate = DateTime.Now;

            this.Mode = enMode.AddNew;

        }

        private clsDriver(int ID, int PersonID, int CreatedByUserID, DateTime CreatedDate)
        {
            this.ID = ID;
            this.PersonID = PersonID;
            this.CreatedByUserID = CreatedByUserID;
            this.CreatedDate = CreatedDate;

            this.PersonInfo = clsPerson.Find(this.PersonID);
            this.CreatedByUserInfo = clsUser.Find(this.CreatedByUserID);

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.ID = DataAccess.clsDrivers.AddNewDriver(
                this.PersonID,
                this.CreatedByUserID,
                this.CreatedDate
            );
            return (this.ID != -1);
        }

        private bool _Update()
        {
            return (DataAccess.clsDrivers.UpdateDriver(this.ID,
            this.PersonID,
            this.CreatedByUserID,
            this.CreatedDate));
        }

        public static bool IsExist(int ID)
        {
            return (DataAccess.clsDrivers.IsDriverExist(ID));
        }

        public static clsDriver Find(int ID)
        {
            int PersonID = -1;
            int CreatedByUserID = -1;
            DateTime CreatedDate = DateTime.Now;

            if (DataAccess.clsDrivers.GetDriverByID(ID,
             ref PersonID,
             ref CreatedByUserID,
             ref CreatedDate))
            {
                return new clsDriver(
                    ID,
                    PersonID,
                    CreatedByUserID,
                    CreatedDate
                );
            }

            return null;
        }

        public static clsDriver FindByPersonID(int PersonID)
        {
            int ID = -1;
            int CreatedByUserID = -1;
            DateTime CreatedDate = DateTime.Now;

            if (DataAccess.clsDrivers.GetDriverByPersonID(ref ID,
             PersonID,
             ref CreatedByUserID,
             ref CreatedDate))
            {
                return new clsDriver(ID, PersonID, CreatedByUserID, CreatedDate);
            }
            return null;
        }


        public static DataTable ViewAllDrivers()
        {
            return DataAccess.clsDrivers.GetAllDrivers();
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

    }
}