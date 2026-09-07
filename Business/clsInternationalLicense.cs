using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsInternationalLicense
    {
        public enum enMode 
        { 
            AddNew = 0, 
            Update = 1 
        };

        public enMode Mode = enMode.AddNew;

        public int InternationalLicenseID { get; set; }
        
        public int ApplicationID { get; set; }
        
        public int DriverID { get; set; }
        
        public int IssuedUsingLocalLicenseID { get; set; }
        
        public DateTime IssueDate { get; set; }
        
        public DateTime ExpirationDate { get; set; }
        
        public bool IsActive { get; set; }
        
        public int CreatedByUserID { get; set; }

        public clsApplication ApplicationInfo { get; set; }
        
        public clsDriver DriverInfo { get; set; }
        
        public clsUser CreatedByUserInfo { get; set; }
        
        public clsLicense LocalLicenseInfo { get; set; }

        // ── Business rule constants ──────────────────────────────────
        //
        // The ApplicationTypeID for "New International License", per the
        // seeded applicationtypes table. NOTE: clsApplicationType
        // .enApplicationType.NewInternationalDrivingLicense incorrectly
        // lists this as 5 (which is actually "Release Detained Driving
        // License" in the seed script). Always trust the DB seed script
        // over a stale enum — that enum should be fixed/removed, but until
        // it is, this constant is the single source of truth used here.
        public const int ApplicationTypeID_NewInternational = 6;

        // The only LicenseClassID eligible to be the basis of an
        // international license, per the seeded licenseclasses table
        // ("Class 3 - Ordinary driving license"). Like the constant
        // above, this is tied to seed data, not a law of nature — if the
        // DB seed ever changes class IDs, update this too.
        public const int EligibleLicenseClassID_Ordinary = 3;

        public clsInternationalLicense()
        {
            this.InternationalLicenseID = -1;
            this.ApplicationID = -1;
            this.DriverID = -1;
            this.IssuedUsingLocalLicenseID = -1;
            this.IssueDate = DateTime.Now;
            this.ExpirationDate = DateTime.Now.AddYears(1);
            this.IsActive = true;
            this.CreatedByUserID = -1;

            this.Mode = enMode.AddNew;
        }

        private clsInternationalLicense(
            int InternationalLicenseID,
            int ApplicationID,
            int DriverID,
            int IssuedUsingLocalLicenseID,
            DateTime IssueDate,
            DateTime ExpirationDate,
            bool IsActive,
            int CreatedByUserID
        )
        {
            this.InternationalLicenseID = InternationalLicenseID;
            this.ApplicationID = ApplicationID;
            this.DriverID = DriverID;
            this.IssuedUsingLocalLicenseID = IssuedUsingLocalLicenseID;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.IsActive = IsActive;
            this.CreatedByUserID = CreatedByUserID;

            this.ApplicationInfo = clsApplication.FindBaseApplication(
                this.ApplicationID
            );

            this.DriverInfo = clsDriver.Find(
                this.DriverID
            );

            this.CreatedByUserInfo = clsUser.Find(
                this.CreatedByUserID
            );

            this.LocalLicenseInfo = clsLicense.Find(
                this.IssuedUsingLocalLicenseID
            );

            this.Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.InternationalLicenseID = DataAccess.clsInternationalLicenses.AddNewInternationalLicense(
                this.ApplicationID,
                this.DriverID,
                this.IssuedUsingLocalLicenseID,
                this.IssueDate,
                this.ExpirationDate,
                this.IsActive,
                this.CreatedByUserID
            );

            return (this.InternationalLicenseID != -1);
        }

        private bool _Update()
        {
            return DataAccess.clsInternationalLicenses.UpdateInternationalLicense(
                this.InternationalLicenseID,
                this.ApplicationID,
                this.DriverID,
                this.IssuedUsingLocalLicenseID,
                this.IssueDate,
                this.ExpirationDate,
                this.IsActive,
                this.CreatedByUserID
            );
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

        // ── Find ──────────────────────────────────────────────────────

        public static clsInternationalLicense Find(int InternationalLicenseID)
        {
            int ApplicationID = -1;
            int DriverID = -1;
            int IssuedUsingLocalLicenseID = -1;
            DateTime IssueDate = DateTime.Now;
            DateTime ExpirationDate = DateTime.Now;
            bool IsActive = true;
            int CreatedByUserID = -1;

            if (DataAccess.clsInternationalLicenses.GetInternationalLicenseByID(
                    InternationalLicenseID,
                    ref ApplicationID,
                    ref DriverID,
                    ref IssuedUsingLocalLicenseID,
                    ref IssueDate,
                    ref ExpirationDate,
                    ref IsActive,
                    ref CreatedByUserID
                ))
            {
                return new clsInternationalLicense(
                    InternationalLicenseID,
                    ApplicationID,
                    DriverID,
                    IssuedUsingLocalLicenseID,
                    IssueDate,
                    ExpirationDate,
                    IsActive,
                    CreatedByUserID
                );
            }
            else
            {
                return null;
            }
        }

        // NEW — wraps DataAccess.GetInternationalLicenseByApplicationID,
        // which existed but was never exposed to the Business layer.
        public static clsInternationalLicense FindByApplicationID(int ApplicationID)
        {
            int InternationalLicenseID = -1;
            int DriverID = -1;
            int IssuedUsingLocalLicenseID = -1;
            DateTime IssueDate = DateTime.Now;
            DateTime ExpirationDate = DateTime.Now;
            bool IsActive = true;
            int CreatedByUserID = -1;

            if (DataAccess.clsInternationalLicenses.GetInternationalLicenseByApplicationID(
                    ApplicationID,
                    ref InternationalLicenseID,
                    ref DriverID,
                    ref IssuedUsingLocalLicenseID,
                    ref IssueDate,
                    ref ExpirationDate,
                    ref IsActive,
                    ref CreatedByUserID
                ))
            {
                return new clsInternationalLicense(
                    InternationalLicenseID,
                    ApplicationID,
                    DriverID,
                    IssuedUsingLocalLicenseID,
                    IssueDate,
                    ExpirationDate,
                    IsActive,
                    CreatedByUserID
                );
            }
            else
            {
                return null;
            }
        }

        // NEW — wraps DataAccess.GetInternationalLicenseByLocalLicenseID.
        public static clsInternationalLicense FindByLocalLicenseID(int LocalLicenseID)
        {
            int InternationalLicenseID = -1;
            int ApplicationID = -1;
            int DriverID = -1;
            DateTime IssueDate = DateTime.Now;
            DateTime ExpirationDate = DateTime.Now;
            bool IsActive = true;
            int CreatedByUserID = -1;

            if (DataAccess.clsInternationalLicenses.GetInternationalLicenseByLocalLicenseID(
                    LocalLicenseID,
                    ref InternationalLicenseID,
                    ref ApplicationID,
                    ref DriverID,
                    ref IssueDate,
                    ref ExpirationDate,
                    ref IsActive,
                    ref CreatedByUserID
                ))
            {
                return new clsInternationalLicense(
                    InternationalLicenseID,
                    ApplicationID,
                    DriverID,
                    LocalLicenseID,
                    IssueDate,
                    ExpirationDate,
                    IsActive,
                    CreatedByUserID
                );
            }
            else
            {
                return null;
            }
        }

        // ── Existence / state checks ─────────────────────────────────

        // NEW — wraps DataAccess.IsInternationalLicenseExist.
        public static bool IsInternationalLicenseExist(int InternationalLicenseID)
        {
            return DataAccess.clsInternationalLicenses.IsInternationalLicenseExist(
                InternationalLicenseID
            );
        }

        public static bool IsDriverHaveActiveInternationalLicense(int DriverID)
        {
            return DataAccess.clsInternationalLicenses.IsDriverHaveActiveInternationalLicense(
                DriverID
            );
        }

        // NEW — wraps DataAccess.IsLicenseIssuedAsInternational. This was
        // present in DataAccess but never reachable from the Business
        // layer, which meant nothing could enforce "a local license can
        // only be the basis of ONE international license."
        public static bool IsLicenseIssuedAsInternational(int LocalLicenseID)
        {
            return DataAccess.clsInternationalLicenses.IsLicenseIssuedAsInternational(
                LocalLicenseID
            );
        }

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            return DataAccess.clsInternationalLicenses.GetActiveInternationalLicenseIDByDriverID(
                DriverID
            );
        }

        // NEW — wraps DataAccess.GetDriverIDByInternationalLicenseID.
        public static int GetDriverIDByInternationalLicenseID(int InternationalLicenseID)
        {
            return DataAccess.clsInternationalLicenses.GetDriverIDByInternationalLicenseID(
                InternationalLicenseID
            );
        }

        // ── Listing ───────────────────────────────────────────────────

        public static DataTable GetAllInternationalLicenses()
        {
            return DataAccess.clsInternationalLicenses.GetAllInternationalLicenses();
        }

        public static DataTable GetDriverInternationalLicenses(int DriverID)
        {
            return DataAccess.clsInternationalLicenses.GetInternationalLicensesByDriverID(
                DriverID
            );
        }

        // NEW — wraps DataAccess.GetActiveInternationalLicensesByDriverID.
        public static DataTable GetActiveDriverInternationalLicenses(int DriverID)
        {
            return DataAccess.clsInternationalLicenses.GetActiveInternationalLicensesByDriverID(
                DriverID
            );
        }

        // NEW — wraps DataAccess.GetExpiredInternationalLicenses.
        public static DataTable GetExpiredInternationalLicenses()
        {
            return DataAccess.clsInternationalLicenses.GetExpiredInternationalLicenses();
        }

        // NEW — wraps DataAccess.GetActiveInternationalLicensesCount.
        public static int GetActiveInternationalLicensesCount()
        {
            return DataAccess.clsInternationalLicenses.GetActiveInternationalLicensesCount();
        }

        // ── Lifecycle actions ─────────────────────────────────────────

        // NEW — wraps DataAccess.DeleteInternationalLicense.
        public bool Delete()
        {
            return DataAccess.clsInternationalLicenses.DeleteInternationalLicense(
                this.InternationalLicenseID
            );
        }

        // NEW — wraps DataAccess.DeactivateInternationalLicense and keeps
        // the in-memory object's IsActive flag consistent with the DB.
        public bool Deactivate()
        {
            if (!DataAccess.clsInternationalLicenses.DeactivateInternationalLicense(
                    this.InternationalLicenseID))
            {
                return false;
            }

            this.IsActive = false;
            return true;
        }

        // NEW — wraps DataAccess.ActivateInternationalLicense.
        public bool Activate()
        {
            if (!DataAccess.clsInternationalLicenses.ActivateInternationalLicense(
                    this.InternationalLicenseID))
            {
                return false;
            }

            this.IsActive = true;
            return true;
        }

        // NEW — wraps DataAccess.ExtendInternationalLicense.
        public bool Extend(int AdditionalYears)
        {
            if (AdditionalYears <= 0)
                return false;

            if (!DataAccess.clsInternationalLicenses.ExtendInternationalLicense(
                    this.InternationalLicenseID, AdditionalYears))
            {
                return false;
            }

            this.ExpirationDate = this.ExpirationDate.AddYears(AdditionalYears);
            return true;
        }

        // ═══════════════════════════════════════════════════════════════
        // IssueNew — the entire "issue an international license" workflow
        // lives here, not in the form. This mirrors clsLicense.Renew()
        // and clsLicense.Replace(): validate business rules, create and
        // save the parent Application row FIRST, then create the derived
        // record using that Application's real ID.
        //
        // Rules enforced (in order, cheapest/most-obvious first):
        //   1. Local license must exist, be active, unexpired, undetained.
        //   2. Local license's class must be the eligible one (Class 3 —
        //      Ordinary). Motorcycle/commercial/agricultural/bus/truck are
        //      NOT eligible for an international license.
        //   3. Driver must not already hold an active international license.
        //   4. This specific local license must not already have been used
        //      to issue an international license before.
        //   5. The "New International License" application type must be
        //      configured in applicationtypes.
        // ═══════════════════════════════════════════════════════════════
        public static clsInternationalLicense IssueNew(
            int localLicenseID, int createdByUserID, out string errorMessage)
        {
            errorMessage = "";

            clsLicense localLicense = clsLicense.Find(localLicenseID);
            if (localLicense == null)
            {
                errorMessage = "Local license not found.";
                return null;
            }

            if (!localLicense.IsActive)
            {
                errorMessage = "This local license is not active.";
                return null;
            }

            if (localLicense.IsExpired())
            {
                errorMessage = "This local license has expired.";
                return null;
            }

            if (localLicense.IsDetained)
            {
                errorMessage = "This local license is currently detained.";
                return null;
            }

            if (localLicense.LicenseClassID != EligibleLicenseClassID_Ordinary)
            {
                string className = localLicense.LicenseClassInfo != null
                    ? localLicense.LicenseClassInfo.Name
                    : "this class";

                errorMessage =
                    "Only an ordinary (Class 3) driving license is eligible for an " +
                    "international license. This license is " + className + ".";
                return null;
            }

            if (localLicense.DriverInfo == null)
            {
                errorMessage = "The driver associated with this license could not be found.";
                return null;
            }

            if (IsDriverHaveActiveInternationalLicense(localLicense.DriverID))
            {
                errorMessage = "This driver already has an active international license.";
                return null;
            }

            if (IsLicenseIssuedAsInternational(localLicenseID))
            {
                errorMessage =
                    "An international license has already been issued using this local license.";
                return null;
            }

            clsApplicationType appType = clsApplicationType.Find(ApplicationTypeID_NewInternational);
            if (appType == null)
            {
                errorMessage = "The 'New International License' application type is not configured.";
                return null;
            }

            // Step 1: the Application record MUST exist first — it's the
            // parent row the internationallicenses.applicationid FK points to.
            clsApplication application = new clsApplication
            {
                ApplicantPersonID = localLicense.DriverInfo.PersonID,
                ApplicationTypeID = ApplicationTypeID_NewInternational,
                ApplicationDate   = DateTime.Now,
                ApplicationStatus = clsApplication.enApplicationStatus.Completed,
                LastStatusDate    = DateTime.Now,
                PaidFees          = appType.Fees,
                CreatedByUserID   = createdByUserID
            };

            if (!application.Save())
            {
                errorMessage = "Failed to create the application record.";
                return null;
            }

            // Step 2: now it's safe to create the international license,
            // using the Application's real, saved ApplicationID.
            clsInternationalLicense intl = new clsInternationalLicense
            {
                ApplicationID             = application.ApplicationID,
                DriverID                  = localLicense.DriverID,
                IssuedUsingLocalLicenseID = localLicense.ID,
                IssueDate                 = DateTime.Now,
                ExpirationDate            = DateTime.Now.AddYears(1),
                IsActive                  = true,
                CreatedByUserID           = createdByUserID
            };

            if (!intl.Save())
            {
                errorMessage = "Failed to create the international license record.";
                return null;
            }

            return intl;
        }
    }
}