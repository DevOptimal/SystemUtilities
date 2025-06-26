using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a file system info comparer for macOS (OSX) that compares <see cref="FileSystemInfo"/> objects
    /// based on their full path, ignoring case and trailing directory separators.
    /// </summary>
    public class OSXFileSystemInfoComparer : IEqualityComparer<FileSystemInfo>
    {
        /// <summary>
        /// Determines whether the specified <see cref="FileSystemInfo"/> objects are equal.
        /// Comparison is based on the normalized full path (case-insensitive, trailing separators trimmed).
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
                // Compare normalized full paths, ignoring case.
                return x.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .Equals(
                        y.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                        StringComparison.OrdinalIgnoreCase
                    );
            }
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="FileSystemInfo"/> object.
        /// The hash code is based on the normalized full path (lowercase, trailing separators trimmed).
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
                // Use normalized, lowercased full path for hash code.
                return obj.FullName
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .ToLower()
                    .GetHashCode();
            }
        }
    }
}
