using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class DriveInfoExtensions
    {
        public static bool Equals(this DriveInfo a, DriveInfo b, IEqualityComparer<DriveInfo> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            return comparer.Equals(a, b);
        }

        public static DirectoryInfo GetDirectory(this DriveInfo drive, string name)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return drive.RootDirectory.GetDirectory(name);
        }

        public static DirectoryInfo GetDirectory(this DriveInfo drive, params string[] names)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            if (names == null) throw new ArgumentNullException(nameof(names));

            return drive.RootDirectory.GetDirectory(names);
        }

        public static FileInfo GetFile(this DriveInfo drive, string name)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return drive.RootDirectory.GetFile(name);
        }

        public static FileInfo GetFile(this DriveInfo drive, params string[] names)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            if (names == null) throw new ArgumentNullException(nameof(names));

            return drive.RootDirectory.GetFile(names);
        }
    }
}
