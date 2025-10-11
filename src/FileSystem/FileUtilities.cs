using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;

namespace DevOptimal.SystemUtilities.FileSystem
{
    /// <summary>
    /// Provides utility methods for file system operations such as deleting files on reboot,
    /// creating hard links, and moving files with overwrite support.
    /// </summary>
    public static class FileUtilities
    {
        /// <summary>
        /// Schedules a file to be deleted when the system next reboots.
        /// </summary>
        /// <param name="path">The full path of the file to delete on reboot.</param>
        /// <exception cref="Win32Exception">Thrown if the operation fails at the Win32 API level.</exception>
        public static void DeleteOnReboot(string path)
        {
            // Use the Win32 API to schedule the file for deletion on reboot.
            if (!PInvoke.MoveFileEx($"\\\\?\\{path}", null, MOVE_FILE_FLAGS.MOVEFILE_DELAY_UNTIL_REBOOT))
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }

        /// <summary>
        /// Creates a hard link at the specified destination, optionally overwriting an existing file.
        /// </summary>
        /// <param name="sourceFileName">The full path of the source file.</param>
        /// <param name="destFileName">The full path where the hard link will be created.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        /// <exception cref="ArgumentNullException">Thrown if source or destination is null.</exception>
        /// <exception cref="Win32Exception">Thrown if the hard link creation fails at the Win32 API level.</exception>
        public static void HardLink(string sourceFileName, string destFileName, bool overwrite)
        {
            if (sourceFileName == null)
            {
                throw new ArgumentNullException(nameof(sourceFileName));
            }
            if (destFileName == null)
            {
                throw new ArgumentNullException(nameof(destFileName));
            }

            // Delete the destination file if overwrite is requested and it exists.
            if (overwrite && File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }

            // Use the Win32 API to create a hard link.
            if (!PInvoke.CreateHardLink($"\\\\?\\{destFileName}", $"\\\\?\\{sourceFileName}"))
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }

        /// <summary>
        /// Moves a file to a new location, optionally overwriting the destination file if it exists.
        /// </summary>
        /// <param name="sourceFileName">The full path of the file to move.</param>
        /// <param name="destFileName">The full path to move the file to.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        public static void Move(string sourceFileName, string destFileName, bool overwrite)
        {
            // Delete the destination file if overwrite is requested and it exists.
            if (overwrite && File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }

            // Move the file to the new location.
            File.Move(sourceFileName, destFileName);
        }
    }
}
