using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    public class FileSystemInfoComparer : IEqualityComparer<FileSystemInfo>
    {
        public bool Equals(FileSystemInfo x, FileSystemInfo y)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return x.FullName.Equals(y.FullName, StringComparison.OrdinalIgnoreCase);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return x.FullName.Equals(y.FullName, StringComparison.Ordinal);
            }
            else
            {
                throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
            }
        }

        public int GetHashCode(FileSystemInfo obj)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return obj.FullName.ToLower().GetHashCode();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return obj.FullName.GetHashCode();
            }
            else
            {
                throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
            }
        }
    }
}
