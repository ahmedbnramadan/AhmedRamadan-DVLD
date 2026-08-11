using System;
using System.Data;
using Business;

// ============================= dotnet run --project DVLD_ConsoleUI/DVLD_ConsoleUI.csproj =================

namespace ConsoleUI
{
    internal class Program
    {
        private static void Main(string[] args)
        {


            while (true)
            {
                Console.Clear();
                clsInputHelper.PrintHeader("DVLD Full System Test Center");
                Console.WriteLine("1. PERSONS    : List / Find / Add");
                Console.WriteLine("2. USERS      : List / Find / Add");
                Console.WriteLine("3. APP TYPES  : List / Update Fees");
                Console.WriteLine("4. LDLA       : Full Cycle (New App -> Test)");
                Console.WriteLine("5. Exit");

                int Choice = clsInputHelper.ReadInt("\nSelect Sector to Test");

                switch (Choice)
                {
                    case 1: _TestPersons(); break;
                    case 2: _TestUsers(); break;
                    case 3: _TestApplicationTypes(); break;
                    case 4: _TestLocalApplications(); break; // الكود السابق الذي كتبناه
                    case 5: return;
                    default: clsInputHelper.NotifyError("Invalid Choice."); break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        // --- 1. اختبار كلاس الأشخاص ---
        private static void _TestPersons()
        {
            clsInputHelper.PrintHeader("Testing clsPerson");
            DataTable dt = clsPerson.GetAllPeople();
            Console.WriteLine($"Total Persons in DB: {dt.Rows.Count}");

            int ID = clsInputHelper.ReadInt("Enter Person ID to Find");
            clsPerson Person = clsPerson.Find(ID);
            if (Person != null)
                clsInputHelper.NotifySuccess($"Found: {Person.FullName}");
            else
                clsInputHelper.NotifyError("Person not found.");
        }

        // --- 2. اختبار كلاس المستخدمين ---
        private static void _TestUsers()
        {
            clsInputHelper.PrintHeader("Testing clsUser");
            DataTable dt = clsUser.GetAllUsers();

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine(string.Format("| {0,-5} | {1,-15} | {2,-10} |", "ID", "UserName", "IsActive"));
            foreach (DataRow row in dt.Rows)
                Console.WriteLine(string.Format("| {0,-5} | {1,-15} | {2,-10} |", row["UserID"], row["UserName"], row["IsActive"]));
        }

        // --- 3. اختبار كلاس أنواع الطلبات (Application Types) ---
        private static void _TestApplicationTypes()
        {
            clsInputHelper.PrintHeader("Testing clsApplicationType");
            DataTable dt = clsApplicationType.GetAllApplicationTypes();

            foreach (DataRow row in dt.Rows)
                Console.WriteLine($"{row["ApplicationTypeID"]} - {row["ApplicationTypeTitle"]}: {row["ApplicationFees"]}$");

            int ID = clsInputHelper.ReadInt("\nEnter Type ID to Update Fees");
            clsApplicationType Type = clsApplicationType.Find(ID);
            if (Type != null)
            {
                decimal NewFees = decimal.Parse(clsInputHelper.ReadString("Enter New Fees"));
                Type.Fees = NewFees;
                if (Type.Save()) clsInputHelper.NotifySuccess("Fees Updated!");
            }
        }

        // --- 4. اختبار الطلبات المحلية (كلاس LDLA الموروث من clsApplication) ---
        private static void _TestLocalApplications()
        {
            // هنا نضع الكود السابق (List / Add / Find) 
            // لأنه يختبر clsApplication و clsLocalDrivingLicenseApplication معاً
            clsInputHelper.NotifySuccess("Testing LDLA & Base Applications...");
            // يمكنك استدعاء الدالة السابقة هنا
        }

    }
}