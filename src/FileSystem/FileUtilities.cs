using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;

namespace DevOptimal.SystemUtilities.FileSystem
{
    public static class FileUtilities
    {
        public static void DeleteOnReboot(string path)
        {
            if (!PInvoke.MoveFileEx(@$"\\?\{path}", null, MOVE_FILE_FLAGS.MOVEFILE_DELAY_UNTIL_REBOOT))
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }

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

            if (overwrite && File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }

            if (!PInvoke.CreateHardLink(@$"\\?\{destFileName}", @$"\\?\{sourceFileName}"))
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }

        public static void Move(string sourceFileName, string destFileName, bool overwrite)
        {
            if (overwrite && File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }

            File.Move(sourceFileName, destFileName);
        }
    }
}
