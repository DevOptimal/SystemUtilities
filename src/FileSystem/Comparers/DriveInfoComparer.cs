using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    public class DriveInfoComparer : IEqualityComparer<DriveInfo>
    {
        public static IEqualityComparer<DriveInfo> Default => new DriveInfoComparer();

        public static IEqualityComparer<DriveInfo> Linux => new LinuxDriveInfoComparer();

        public static IEqualityComparer<DriveInfo> OSX => new OSXDriveInfoComparer();

        public static IEqualityComparer<DriveInfo> Windows => new WindowsDriveInfoComparer();

        private readonly Lazy<IEqualityComparer<DriveInfo>> comparerLazy = new(() =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxDriveInfoComparer();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsDriveInfoComparer();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new OSXDriveInfoComparer();
            }
            else
            {
                throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
            }
        });

        public bool Equals(DriveInfo x, DriveInfo y)
        {
            return comparerLazy.Value.Equals(x, y);
        }

        public int GetHashCode(DriveInfo obj)
        {
            return comparerLazy.Value.GetHashCode(obj);
        }
    }
}
