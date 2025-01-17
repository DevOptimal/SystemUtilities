using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.Comparers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

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

            return directory.GetDirectory([name]);
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

            return directory.GetFile([name]);
        }

        public static FileInfo GetFile(this DirectoryInfo directory, params string[] names)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (names == null) throw new ArgumentNullException(nameof(names));

            return new FileInfo(Path.Combine(directory.FullName, Path.Combine(names)));
        }

        /// <summary>
        /// Determines whether a directory is an ancestor of another directory.
        /// </summary>
        /// <param name="directory">The directory to test.</param>
        /// <param name="descendant">The descendant to test.</param>
        /// <returns>True if <paramref name="directory"/> is an ancestor of <paramref name="descendant"/>, false otherwise.</returns>
        public static bool IsAncestorOf(this DirectoryInfo directory, DirectoryInfo descendant)
        {
            return descendant.IsDescendantOf(directory);
        }

        /// <summary>
        /// Determines whether a directory is a descendant of another directory.
        /// </summary>
        /// <param name="directory">The directory to test.</param>
        /// <param name="ancestor">The ancester directory to test.</param>
        /// <returns>True if <paramref name="directory"/> is a descendant of <paramref name="ancestor"/>, false otherwise.</returns>
        public static bool IsDescendantOf(this DirectoryInfo directory, DirectoryInfo ancestor)
        {
            return directory.IsDescendantOf(ancestor, new DirectoryInfoComparer());
        }

        internal static bool IsDescendantOf(this DirectoryInfo directory, DirectoryInfo ancestor, DirectoryInfoComparer comparer)
        {
            if (directory == null || ancestor == null)
            {
                return false;
            }

            var current = directory.Parent;
            while (current != null)
            {
                if (comparer.Equals(current, ancestor))
                {
                    return true;
                }
                current = current.Parent;
            }

            return false;
        }
    }
}
