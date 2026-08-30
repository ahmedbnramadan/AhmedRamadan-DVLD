using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Business;

namespace DVLD
{
    public static class clsGlobal
    {
        // ── App identity ─────────────────────────────────────────────
        public const string AppName = "DVLD – Driver & Vehicle Licensing";
        public const string AppVersion = "1.0.0";

        // ── Business rules ────────────────────────────────────────────
        public const int MinimumDriverAge = 18;
        public const int MaximumDriverAge = 100;
        public const int MaxImageSizeBytes = 2 * 1024 * 1024; // 2 MB

        // ── Logged-in user (set after successful login) ───────────────
        public static int CurrentUserID { get; set; } = -1;
        public static string CurrentUsername { get; set; } = string.Empty;

        // ── User preference: last selected country ────────────────────
        public static int LastSelectedCountryID { get; set; } = -1;

        // ── Shared UI colours ─────────────────────────────────────────
        public static readonly Color PrimaryRed = Color.FromArgb(192, 0, 0);
        public static readonly Color PrimaryBlue = Color.FromArgb(0, 120, 215);   // ← added
        public static readonly Color InputError = Color.FromArgb(255, 204, 204);
        public static readonly Color InputValid = Color.White;
        public static readonly Color GridHeaderBack = Color.FromArgb(68, 114, 196);
        public static readonly Color GridHeaderFore = Color.White;
        public static readonly Color GridSelectionBack = Color.FromArgb(41, 128, 185);
        public static Color LinkBlue = Color.SteelBlue;
        public static Color DangerRed = Color.FromArgb(192, 50, 50);

        // ── Paths ─────────────────────────────────────────────────────
        /// <summary>Folder where person photos are stored on disk.</summary>
        public static string ImagesFolder
            => Path.Combine(Application.StartupPath, "Images");

        public static string IconsFolder
            => Path.Combine(Application.StartupPath, "Icons");

        public static void CreateImagesFolderIfDoesNotExist()
        {
            // فحص هل المجلد غير موجود؟
            if (!Directory.Exists(ImagesFolder))
            {
                // إذا غير موجود، قم بإنشائه فوراً
                Directory.CreateDirectory(ImagesFolder);
            }
        }

        public static string PeopleImagesFolder
            => Path.Combine(Application.StartupPath, "Images", "People");

        public static string DefaultImagesFolder
            => Path.Combine(Application.StartupPath, "Icons");
        public static string DefaultMalePath
            => Path.Combine(DefaultImagesFolder, "Male 512.png");
            
        public static string DefaultFemalePath
            => Path.Combine(DefaultImagesFolder, "Female 512.png");
        

        public static string GetDefaultPersonImagePath(int? gender)
        {
            if (gender == null) return DefaultMalePath; // fallback to male if unknown

            return gender == 0 ? DefaultMalePath : DefaultFemalePath;
        }
    }

}