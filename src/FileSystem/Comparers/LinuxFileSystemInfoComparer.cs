using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a file system info comparer for Linux-like file systems.
    /// Compares <see cref="FileSystemInfo"/> objects by their normalized full path.
    /// </summary>
    public class LinuxFileSystemInfoComparer : IEqualityComparer<FileSystemInfo>
    {
        /// <summary>
        /// Determines whether two <see cref="FileSystemInfo"/> instances are equal,
        /// based on their full path, ignoring trailing directory separator characters.
        /// </summary>
        /// <param name="x">The first <see cref="FileSystemInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="FileSystemInfo"/> to compare.</param>
        /// <returns>
        /// true if the full paths are equal (ignoring trailing separators); otherwise, false.
        /// </returns>
        public bool Equals(FileSystemInfo x, FileSystemInfo y)
        {
            if (x == y)
            {
                // Reference equality or both are null.
                return true;
            }
            else if (x == null || y == null)
            {
                // One is null, the other is not.
                return false;
            }
            else
            {
                // Compare normalized full paths using ordinal comparison.
                return x.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .Equals(
                        y.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                        StringComparison.Ordinal
                    );
            }
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="FileSystemInfo"/>,
        /// based on its normalized full path.
        /// </summary>
        /// <param name="obj">The <see cref="FileSystemInfo"/> for which to get a hash code.</param>
        /// <returns>
        /// A hash code for the object's normalized full path, or 0 if the object is null.
        /// </returns>
        public int GetHashCode(FileSystemInfo obj)
        {
            if (obj == null)
            {
                // Null objects return a hash code of 0.
                return 0;
            }
            else
            {
                // Use the hash code of the normalized full path.
                return obj.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).GetHashCode();
            }
        }
    }
}
