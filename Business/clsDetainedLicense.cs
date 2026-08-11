using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsDetainedLicense
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public int LicenseID { get; set; }
        public DateTime DetainDate { get; set; }
        public decimal FineFees { get; set; }
        public int CreatedByUserID { get; set; }
        public bool IsReleased { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? ReleasedByUserID { get; set; }
        public int? ReleaseApplicationID { get; set; }

        // كائنات الربط (Composition)
        public clsLicense LicenseInfo { get; set; }
        public clsUser CreatedByUserInfo { get; set; }
        public clsUser ReleasedByUserInfo { get; set; }

        public clsDetainedLicense()
        {
            this.ID = -1;
            this.LicenseID = -1;
            this.DetainDate = DateTime.Now;
            this.FineFees = 0;
            this.CreatedByUserID = -1;
            this.IsReleased = false;
            this.ReleaseDate = null;
            this.ReleasedByUserID = null;
            this.ReleaseApplicationID = null;

            Mode = enMode.AddNew;
        }

        private clsDetainedLicense(int DetainID, 
        int LicenseID, 
        DateTime DetainDate,
        decimal FineFees, 
        int CreatedByUserID, 
        bool IsReleased,
        DateTime? ReleaseDate, 
        int? ReleasedByUserID, 
        int? ReleaseApplicationID)
        {
            this.ID = DetainID;
            this.LicenseID = LicenseID;
            this.DetainDate = DetainDate;
            this.FineFees = FineFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsReleased = IsReleased;
            this.ReleaseDate = ReleaseDate;
            this.ReleasedByUserID = ReleasedByUserID;
            this.ReleaseApplicationID = ReleaseApplicationID;

            // ربط الكائنات
            this.LicenseInfo = clsLicense.Find(this.LicenseID);
            this.CreatedByUserInfo = clsUser.Find(this.CreatedByUserID);
            
            if (this.IsReleased && this.ReleasedByUserID.HasValue)
                this.ReleasedByUserInfo = clsUser.Find(this.ReleasedByUserID.Value);

            Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            // منطق الأعمال: لا يمكن سحب رخصة هي بالفعل مسحوبة ونشطة
            if (IsLicenseDetained(this.LicenseID))
                return false;

            this.ID = DataAccess.clsDetainedLicenses.AddNewDetainedLicense(
                this.LicenseID,
                this.DetainDate,   
                this.FineFees,
                this.CreatedByUserID,
                this.IsReleased,
                this.ReleaseDate,
                this.ReleasedByUserID,
                this.ReleaseApplicationID);

            return (this.ID != -1);
        }

        private bool _Update()
        {
            return DataAccess.clsDetainedLicenses.UpdateDetainedLicense(
                this.ID,
                this.LicenseID,
                this.DetainDate,
                this.FineFees,
                this.CreatedByUserID,
                this.IsReleased,
                this.ReleaseDate,
                this.ReleasedByUserID,
                this.ReleaseApplicationID);
        }

        public static clsDetainedLicense Find(int DetainID)
        {
            int LicenseID = -1, 
            CreatedByUserID = -1;
            decimal FineFees = 0; 
            DateTime DetainDate = DateTime.Now;
            bool IsReleased = false;
            DateTime? ReleaseDate = null;
            int? ReleasedByUserID = null,
            ReleaseApplicationID = null;

            if (DataAccess.clsDetainedLicenses.GetDetainedLicenseByID(DetainID,
                ref LicenseID, 
                ref DetainDate, 
                ref FineFees, 
                ref CreatedByUserID,
                ref IsReleased, 
                ref ReleaseDate, 
                ref ReleasedByUserID, 
                ref ReleaseApplicationID))
            {
                return new clsDetainedLicense(DetainID, 
                LicenseID, 
                DetainDate, 
                FineFees, 
                CreatedByUserID, 
                IsReleased, ReleaseDate, 
                ReleasedByUserID, 
                ReleaseApplicationID);
            }
            return null;
        }

        public static clsDetainedLicense FindByLicenseID(int LicenseID)
        {
            int DetainID = -1, 
            CreatedByUserID = -1;
            decimal FineFees = 0; 
            DateTime DetainDate = DateTime.Now;
            bool IsReleased = false;
            DateTime? ReleaseDate = null; 
            int? ReleasedByUserID = null, 
            ReleaseApplicationID = null;

            if (DataAccess.clsDetainedLicenses.GetDetainedLicenseByLicenseID(LicenseID,
                ref DetainID, 
                ref DetainDate, 
                ref FineFees, 
                ref CreatedByUserID,
                ref IsReleased, 
                ref ReleaseDate, 
                ref ReleasedByUserID, 
                ref ReleaseApplicationID))
            {
                return new clsDetainedLicense(DetainID, 
                LicenseID, 
                DetainDate, 
                FineFees, 
                CreatedByUserID, 
                IsReleased, 
                ReleaseDate, 
                ReleasedByUserID, 
                ReleaseApplicationID);
            }
            return null;
        }

        public bool Save()
        {
            if (this.FineFees < 0) return false;

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

        public static bool IsLicenseDetained(int LicenseID)
        {
            return DataAccess.clsDetainedLicenses.IsLicenseDetained(LicenseID);
        }

        public bool Release(int ReleasedByUserID, int ReleaseApplicationID)
        {
            return DataAccess.clsDetainedLicenses.ReleaseDetainedLicense(this.ID, 
                ReleasedByUserID, ReleaseApplicationID);
        }

        public static DataTable GetAllDetainedLicenses()
        {
            return DataAccess.clsDetainedLicenses.GetAllDetainedLicenses();
        }
        
        // دوال إحصائية مفيدة لشاشة الـ Dashboard
        public static decimal GetTotalFines() => DataAccess.clsDetainedLicenses.GetTotalFinesCollected();
    }
}