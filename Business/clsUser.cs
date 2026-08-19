using System;
using System.Data;
using DataAccess;

namespace Business
{

    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;


        public int UserID { get; set; }
        public int PersonID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        private clsPerson _PersonInfo;


        public clsPerson PersonInfo
        {
            get
            {
                if (_PersonInfo == null)
                    _PersonInfo = clsPerson.Find(this.PersonID);
                return _PersonInfo;
            }
        }
        public clsUser()
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = "";
            this.Password = "";
            this.IsActive = true;
        }

        private clsUser(
            int UserID,
            int PersonID,
            string UserName,
            string Password,
            bool IsActive
        )
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.Password = Password;
            this.IsActive = IsActive;

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.UserID = DataAccess.clsUsers.AddNewUser(
                this.PersonID,
                this.UserName,
                this.Password,
                this.IsActive
            );

            return (this.UserID != -1);
        }

        private bool _Update()
        {
            return (DataAccess.clsUsers.UpdateUser(this.UserID, this.PersonID, this.UserName, this.Password, this.IsActive));
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            return DataAccess.clsUsers.ChangePassword(UserID, NewPassword);
        }

        public static clsUser Find(int UserID)
        {
            int PersonID = -1;
            string UserName = "";
            string Password = "";
            bool IsActive = true;

            if (DataAccess.clsUsers.GetUserByID(
                UserID,
                ref PersonID,
                ref UserName,
                ref Password,
                ref IsActive))
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);

            else return null;
        }

        public static clsUser Find(string UserName)
        {
            int UserID = -1;
            int PersonID = -1;
            string Password = "";
            bool IsActive = true;

            if (DataAccess.clsUsers.GetUserInfoByUserName(
                ref UserID,
                ref PersonID,
                UserName,
                ref Password,
                ref IsActive))
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);

            else return null;
        }

        public static clsUser FindByPersonID(int PersonID)
        {
            int UserID = -1;
            string UserName = "";
            string Password = "";
            bool IsActive = true;

            if (DataAccess.clsUsers.GetUserInfoByPersonID(
                ref UserID,
                PersonID,
                ref UserName,
                ref Password,
                ref IsActive))
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);

            else return null;
        }

        public static clsUser FindByUserNameAndPassWord(string UserName, string Password)
        {
            int UserID = -1;
            int PersonID = -1;
            bool IsActive = true;

            if (DataAccess.clsUsers.GetUserInfoByUserNameAndPassword(
                ref UserID,
                ref PersonID,
                UserName,
                Password,
                ref IsActive))
                return new clsUser(UserID, PersonID, UserName, Password, IsActive);

            else return null;
        }

        public static bool Delete(int UserID)
        {
            return (DataAccess.clsUsers.DeleteUser(UserID));
        }

        public static bool IsExists(int UserID)
        {
            return (DataAccess.clsUsers.IsUserExist(UserID));
        }

        public static bool IsExists(string UserName, string Password)
        {
            return (DataAccess.clsUsers.IsUserExist(UserName, Password));
        }

        public static bool IsExists(string UserName)
        {
            return (DataAccess.clsUsers.IsUserNameExist(UserName));
        }

        public static bool IsExistsByPersonID(int PersonID)
        {
            return (DataAccess.clsUsers.IsUserExistForPersonID(PersonID));
        }

        public static DataTable GetAllUsers()
        {
            return DataAccess.clsUsers.GetAllUsers();
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