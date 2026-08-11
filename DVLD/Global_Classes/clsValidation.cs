using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

// Claede:
// using System;
using System.Diagnostics;
//using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
// using System.Windows.Forms;


namespace DVLD
{
    public static class clsValidation
    {
        // ── Primitive checks ──────────────────────────────────────────

        public static bool IsEmpty(string value)
            => string.IsNullOrWhiteSpace(value);

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email.Trim(),
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone.Trim(), @"^\+?[\d\s\-\(\)]{7,15}$");
        }

        public static bool IsValidNationalNo(string nationalNo)
            => !string.IsNullOrWhiteSpace(nationalNo) &&
               nationalNo.Trim().Length >= 2;

        public static bool IsValidAge(DateTime dob, int minAge = 18)
            => dob <= DateTime.Now.AddYears(-minAge);

        public static bool IsValidName(string name)
            => !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 2;

        // ── Control highlighters ──────────────────────────────────────

        /// <summary>
        /// Colours the TextBox background red on failure, white on success.
        /// Returns <paramref name="isValid"/> so it can be used inline.
        /// </summary>
        public static bool Highlight(TextBox tb, bool isValid)
        {
            tb.BackColor = isValid ? clsGlobal.InputValid : clsGlobal.InputError;
            return isValid;
        }

        /// <summary>Same highlight logic for a ComboBox.</summary>
        public static bool Highlight(ComboBox cb, bool isValid)
        {
            cb.BackColor = isValid ? clsGlobal.InputValid : clsGlobal.InputError;
            return isValid;
        }

        /// <summary>
        /// Validates every required field in the Add/Edit person form at once.
        /// Returns <c>true</c> only when ALL fields pass.
        /// </summary>
        public static bool ValidatePersonForm(
            TextBox txtFirstName,
            TextBox txtLastName,
            TextBox txtNationalNo,
            TextBox txtPhone,
            TextBox txtEmail,
            ComboBox cbCountry,
            DateTimePicker dtpDOB)
        {
            bool ok = true;

            ok &= Highlight(txtFirstName, IsValidName(txtFirstName.Text));
            ok &= Highlight(txtLastName, IsValidName(txtLastName.Text));
            ok &= Highlight(txtNationalNo, IsValidNationalNo(txtNationalNo.Text));

            // Phone is optional but must be valid if provided
            bool phoneOk = string.IsNullOrWhiteSpace(txtPhone.Text) ||
                           IsValidPhone(txtPhone.Text);
            ok &= Highlight(txtPhone, phoneOk);

            // Email is optional but must be valid if provided
            bool emailOk = string.IsNullOrWhiteSpace(txtEmail.Text) ||
                           IsValidEmail(txtEmail.Text);
            ok &= Highlight(txtEmail, emailOk);

            // Country must be selected
            ok &= Highlight(cbCountry, cbCountry.SelectedIndex >= 0);

            // Age check
            if (!IsValidAge(dtpDOB.Value))
            {
                clsUtil.ShowWarning(
                    $"Person must be at least {clsGlobal.MinimumDriverAge} years old.",
                    "Invalid Date of Birth");
                ok = false;
            }

            return ok;
        }
    }
}