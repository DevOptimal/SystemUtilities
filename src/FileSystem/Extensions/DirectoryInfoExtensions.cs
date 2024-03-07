using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static bool Equals(this DirectoryInfo a, DirectoryInfo b, IEqualityComparer<DirectoryInfo> comparer)
        {
            return comparer.Equals(a, b);
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo directory, string name)
        {
            return directory.GetDirectory(new[] { name });
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo directory, params string[] names)
        {
            return new DirectoryInfo(Path.Combine(directory.FullName, Path.Combine(names)));
        }

        public static DriveInfo GetDrive(this DirectoryInfo directory)
        {
            return new DriveInfo(Path.GetPathRoot(directory.FullName));
        }

        public static FileInfo GetFile(this DirectoryInfo directory, string name)
        {
            return directory.GetFile(new[] { name });
        }

        public static FileInfo GetFile(this DirectoryInfo directory, params string[] names)
        {
            return new FileInfo(Path.Combine(directory.FullName, Path.Combine(names)));
        }
    }
}
