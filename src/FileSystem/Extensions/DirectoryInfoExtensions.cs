using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static bool Equals(this DirectoryInfo a, DirectoryInfo b, IEqualityComparer<DirectoryInfo> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            return comparer.Equals(a, b);
        }

        public static bool Exists(this DirectoryInfo directory, IFileSystem fileSystem)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.DirectoryExists(directory.FullName);
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo directory, string name)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return directory.GetDirectory(new[] { name });
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo directory, params string[] names)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (names == null) throw new ArgumentNullException(nameof(names));

            return new DirectoryInfo(Path.Combine(directory.FullName, Path.Combine(names)));
        }

        public static DriveInfo GetDrive(this DirectoryInfo directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));

            return new DriveInfo(Path.GetPathRoot(directory.FullName));
        }

        public static FileInfo GetFile(this DirectoryInfo directory, string name)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return directory.GetFile(new[] { name });
        }

        public static FileInfo GetFile(this DirectoryInfo directory, params string[] names)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (names == null) throw new ArgumentNullException(nameof(names));

            return new FileInfo(Path.Combine(directory.FullName, Path.Combine(names)));
        }
    }
}
