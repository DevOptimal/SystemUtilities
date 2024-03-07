using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    public class FileSystemInfoComparer : IEqualityComparer<FileSystemInfo>
    {
        public static IEqualityComparer<FileSystemInfo> Default => new FileSystemInfoComparer();

        public static IEqualityComparer<FileSystemInfo> Linux => new LinuxFileSystemInfoComparer();

        public static IEqualityComparer<FileSystemInfo> OSX => new OSXFileSystemInfoComparer();

        public static IEqualityComparer<FileSystemInfo> Windows => new WindowsFileSystemInfoComparer();

        private readonly Lazy<IEqualityComparer<FileSystemInfo>> comparerLazy = new Lazy<IEqualityComparer<FileSystemInfo>>(() =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxFileSystemInfoComparer();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsFileSystemInfoComparer();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new OSXFileSystemInfoComparer();
            }
            else
            {
                throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
            }
        });

        public bool Equals(FileSystemInfo x, FileSystemInfo y)
        {
            return comparerLazy.Value.Equals(x, y);
        }

        public int GetHashCode(FileSystemInfo obj)
        {
            return comparerLazy.Value.GetHashCode(obj);
        }
    }
}
