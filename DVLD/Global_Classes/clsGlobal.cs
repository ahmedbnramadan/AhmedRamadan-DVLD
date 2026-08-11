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
        public const int MaxImageSizeBytes = 2 * 1024 * 1024; // 2 MB

        // ── Logged-in user (set after successful login) ───────────────
        public static int CurrentUserID { get; set; } = -1;
        public static string CurrentUsername { get; set; } = string.Empty;

        // ── Shared UI colours ─────────────────────────────────────────
        public static readonly Color PrimaryRed = Color.FromArgb(192, 0, 0);
        public static readonly Color InputError = Color.FromArgb(255, 204, 204);
        public static readonly Color InputValid = Color.White;
        public static readonly Color GridHeaderBack = Color.FromArgb(68, 114, 196);
        public static readonly Color GridHeaderFore = Color.White;
        public static readonly Color GridSelectionBack = Color.FromArgb(41, 128, 185);

        // ── Paths ─────────────────────────────────────────────────────
        /// <summary>Folder where person photos are stored on disk.</summary>
        public static string ImagesFolder
            => Path.Combine(Application.StartupPath, "Images");

        public static void CreateImagesFolderIfDoesNotExist()
        {
            // فحص هل المجلد غير موجود؟
            if (!Directory.Exists(ImagesFolder))
            {
                // إذا غير موجود، قم بإنشائه فوراً
                Directory.CreateDirectory(ImagesFolder);
            }
        }
        public static string DefaultMalePath
            => Path.Combine(ImagesFolder, "men-line.png");
            
        public static string DefaultFemalePath
            => Path.Combine(ImagesFolder, "women-line.png");
        
        public static string PeopleImagesFolder 
            => Path.Combine(Application.StartupPath, "Images", "People");
    }

}