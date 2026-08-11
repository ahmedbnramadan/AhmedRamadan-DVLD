using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;


namespace DVLD
{
    public class clsFormat : Form
    {
        /// <summary>Returns a date as "MM/dd/yyyy".</summary>
        public static string DateShort(DateTime date)
            => date.ToString("MM/dd/yyyy");

        /// <summary>Returns a date as "MMMM dd, yyyy"  e.g. April 21, 2026.</summary>
        public static string DateLong(DateTime date)
            => date.ToString("MMMM dd, yyyy");

        /// <summary>Capitalises first letter, lowercases the rest.</summary>
        public static string NameCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            name = name.Trim();
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

        /// <summary>Title-cases every word in a sentence.</summary>
        public static string TitleCase(string text)
            => System.Globalization.CultureInfo
                      .CurrentCulture.TextInfo
                      .ToTitleCase((text?.Trim() ?? string.Empty).ToLower());

        /// <summary>Returns "Male" or "Female" from the numeric gender code (0 = Male).</summary>
        public static string Gender(short genderCode)
            => genderCode == 0 ? "Male" : "Female";

        /// <summary>Trims a phone number string.</summary>
        public static string Phone(string phone)
            => phone?.Trim() ?? string.Empty;

        /// <summary>Trims and lowercases an e-mail address.</summary>
        public static string Email(string email)
            => email?.Trim().ToLower() ?? string.Empty;

        // إذا كانت القيمة Null، تعيد نصاً فارغاً بدلاً من الانهيار
        public static string ReplaceIfNull(object value, string defaultValue = "")
            => value == DBNull.Value || value == null ? defaultValue : value.ToString();

        public static string FormatCurrency(decimal amount)
            => amount.ToString("C"); // حرف C يعني Currency

        public static int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;
            // للتأكد إذا كان يوم ميلاده مرّ فعلياً في السنة الحالية
            if (birthDate.Date > DateTime.Now.AddYears(-age)) age--;
            return age;
        }

        public static string StatusText(bool isActive)
            => isActive ? "Active" : "Inactive";
    }
}