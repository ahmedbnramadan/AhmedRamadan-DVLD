using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Business;

// Claede:
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;


namespace DVLD
{
    public static class clsUtil
    {
        // ── Image ─────────────────────────────────────────────────────

        /// <summary>
        /// Loads an image file into a PictureBox without locking the file on disk.
        /// Shows <paramref name="fallback"/> (or clears the box) if the path is
        /// missing or empty.
        /// </summary>
        public static void LoadPersonImage(PictureBox pb, string imagePath,
                                           Image fallback = null)
        {
            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                using (var ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                    pb.Image = Image.FromStream(ms);
            }
            else
            {
                // إذا لم يمرر المستخدم صورة مخصصة، نبحث عن الافتراضية
                if (fallback == null)
                {
                    string defaultPath = Path.Combine(clsGlobal.ImagesFolder, "default.png");
                    if (File.Exists(defaultPath))
                        pb.Image = Image.FromFile(defaultPath);
                    else
                        pb.Image = null; // أو أي تصرف آخر
                }
                else
                {
                    pb.Image = fallback;
                }
            }
        }

        /// <summary>
        /// Loads an image file and returns it as an Image without locking the file on disk.
        /// Returns <c>null</c> if the path is missing, empty, or the file does not exist.
        /// </summary>
        public static Image LoadImage(string imagePath)
        {
            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                using (var ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                    return Image.FromStream(ms);
            }

            return null;
        }

        /// <summary>
        /// Opens a file-picker for images and returns the chosen path,
        /// or <c>null</c> if the user cancelled.
        /// </summary>
        public static string PickImagePath()
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Person Photo";
                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                dlg.Multiselect = false;
                return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
            }
        }

        /// <summary>Deletes an old image file if it exists and is not a default image.</summary>
        public static void DeleteOldImageFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            if (!File.Exists(imagePath))
                return;

            // Don't delete default gender images from Icons folder
            if (imagePath.EndsWith("men-line.png") || imagePath.EndsWith("women-line.png"))
                return;

            try
            {
                File.Delete(imagePath);
            }
            catch
            {
                // Ignore delete errors.
            }
        }

        /// <summary>
        /// Copies an image into the application's Images/People folder and
        /// returns the new path. Creates the folder if it does not exist.
        /// Returns the original path unchanged if copying fails.
        /// </summary>
        public static string CopyImageToAppFolder(string sourcePath)
        {
            try
            {
                if (!Directory.Exists(clsGlobal.ImagesFolder))
                    Directory.CreateDirectory(clsGlobal.ImagesFolder);

                string ext = Path.GetExtension(sourcePath);
                string newName = $"{Guid.NewGuid()}{ext}";
                string dest = Path.Combine(clsGlobal.ImagesFolder, newName);

                File.Copy(sourcePath, dest, overwrite: true);
                return dest;
            }
            catch
            {
                return sourcePath; // fallback: keep original path
            }
        }

        // ── Shell ─────────────────────────────────────────────────────

        /// <summary>Opens the default mail client addressed to <paramref name="email"/>.</summary>
        public static void SendEmail(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
                Process.Start(new ProcessStartInfo($"mailto:{email}")
                { UseShellExecute = true });
        }

        /// <summary>Shows a simple "Calling …" info box (extend with VOIP if needed).</summary>
        public static void MakePhoneCall(string phone)
        {
            if (!string.IsNullOrWhiteSpace(phone))
                ShowInfo($"Initiating call to: {phone}", "Phone Call");
        }

        // ── MessageBox shortcuts ──────────────────────────────────────

        public static void ShowInfo(string message, string title = "Information")
            => MessageBox.Show(message, title,
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void ShowError(string message, string title = "Error")
            => MessageBox.Show(message, title,
                               MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static void ShowWarning(string message, string title = "Warning")
            => MessageBox.Show(message, title,
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);

        /// <summary>
        /// Asks the user to confirm a destructive action.
        /// Returns <c>true</c> only if the user clicks Yes.
        /// </summary>
        public static bool ConfirmDelete(string itemDescription = "this record")
            => MessageBox.Show(
                   $"Are you sure you want to delete {itemDescription}?\n" +
                   "This action cannot be undone.",
                   "Confirm Delete",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Warning) == DialogResult.Yes;
    }
}