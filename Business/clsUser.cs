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
        public string PassWord { get; set; }
        public bool isActive { get; set; }

        public clsPerson PersonInfo;

        public clsUser()
        {
            this.UserID = -1;
            // the teacher didn't make personid
            this.PersonID = -1;
            this.UserName = "";
            this.PassWord = "";
            this.isActive = true;
        }

        private clsUser(
            int UserID,
            int PersonID,
            string UserName,
            string PassWord,
            bool isActive
        )
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.PassWord = PassWord;
            this.isActive = isActive;

            this.PersonInfo = clsPerson.Find(PersonID);

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.UserID = DataAccess.clsUsers.AddNewUser(
                this.PersonID,
                this.UserName,
                this.PassWord,
                this.isActive
            );

            return (this.UserID != -1);
        }

        private bool _Update()
        {
            return (DataAccess.clsUsers.UpdateUser(this.UserID, this.PersonID, this.UserName, this.PassWord, this.isActive));
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            return DataAccess.clsUsers.ChangePassword(UserID, NewPassword);
        }

        public static clsUser Find(int UserID)
        {
            int PersonID = -1;
            string UserName = "";
            string PassWord = "";
            bool isActive = true;

            if (DataAccess.clsUsers.GetUserByID(
                UserID,
                ref PersonID,
                ref UserName,
                ref PassWord,
                ref isActive))
                return new clsUser(UserID, PersonID, UserName, PassWord, isActive);

            else return null;
        }

        public static clsUser Find(string UserName)
        {
            int UserID = -1;
            int PersonID = -1;
            string PassWord = "";
            bool isActive = true;

            if (DataAccess.clsUsers.GetUserInfoByUserName(
                ref UserID,
                ref PersonID,
                UserName,
                ref PassWord,
                ref isActive))
                return new clsUser(UserID, PersonID, UserName, PassWord, isActive);

            else return null;
        }

        public static clsUser FindByPersonID(int PersonID)
        {
            int UserID = -1;
            string UserName = "";
            string PassWord = "";
            bool isActive = true;

            if (DataAccess.clsUsers.GetUserInfoByPersonID(
                ref UserID,
                PersonID,
                ref UserName,
                ref PassWord,
                ref isActive))
                return new clsUser(UserID, PersonID, UserName, PassWord, isActive);

            else return null;
        }

        public static clsUser FindByUserNameAndPassWord(string UserName, string PassWord)
        {
            int UserID = -1;
            int PersonID = -1;
            bool isActive = true;

            if (DataAccess.clsUsers.GetUserInfoByUserNameAndPassword(
                ref UserID,
                ref PersonID,
                UserName,
                PassWord,
                ref isActive))
                return new clsUser(UserID, PersonID, UserName, PassWord, isActive);

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

        public static bool IsExists(string UserName, string PassWord)
        {
            return (DataAccess.clsUsers.IsUserExist(UserName, PassWord));
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