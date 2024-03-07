using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class DriveInfoExtensions
    {
        public static bool Equals(this DriveInfo a, DriveInfo b, IEqualityComparer<DriveInfo> comparer)
        {
            return comparer.Equals(a, b);
        }

        public static DirectoryInfo GetDirectory(this DriveInfo drive, string name)
        {
            return drive.RootDirectory.GetDirectory(name);
        }

        public static DirectoryInfo GetDirectory(this DriveInfo drive, params string[] names)
        {
            return drive.RootDirectory.GetDirectory(names);
        }

        public static FileInfo GetFile(this DriveInfo drive, string name)
        {
            return drive.RootDirectory.GetFile(name);
        }

        public static FileInfo GetFile(this DriveInfo drive, params string[] names)
        {
            return drive.RootDirectory.GetFile(names);
        }
    }
}
