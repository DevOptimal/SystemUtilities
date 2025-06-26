using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a Windows-specific equality comparer for <see cref="FileSystemInfo"/> objects.
    /// Compares file system entries by their full path, ignoring case and trailing directory separators.
    /// </summary>
    public class WindowsFileSystemInfoComparer : IEqualityComparer<FileSystemInfo>
    {
        /// <summary>
        /// Determines whether two <see cref="FileSystemInfo"/> objects represent the same file system entry.
        /// Comparison is case-insensitive and ignores trailing directory separator characters.
        /// </summary>
        /// <param name="x">The first <see cref="FileSystemInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="FileSystemInfo"/> to compare.</param>
        /// <returns>
        /// true if the objects represent the same file system entry; otherwise, false.
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
                // Compare full paths, ignoring case and trailing directory separators.
                return x.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .Equals(
                        y.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                        StringComparison.OrdinalIgnoreCase
                    );
            }
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="FileSystemInfo"/> object.
        /// The hash code is based on the full path, lowercased and without trailing directory separators.
        /// </summary>
        /// <param name="obj">The <see cref="FileSystemInfo"/> for which to get a hash code.</param>
        /// <returns>
        /// A hash code for the specified object, or 0 if the object is null.
        /// </returns>
        public int GetHashCode(FileSystemInfo obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                // Use lowercased, trimmed full path for hash code to match Equals logic.
                return obj.FullName
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .ToLower()
                    .GetHashCode();
            }
        }
    }
}
