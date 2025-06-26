using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a Windows-specific equality comparer for <see cref="DirectoryInfo"/> objects.
    /// Compares directories using the logic defined in <see cref="WindowsFileSystemInfoComparer"/>.
    /// </summary>
    public class WindowsDirectoryInfoComparer : WindowsFileSystemInfoComparer, IEqualityComparer<DirectoryInfo>
    {
        /// <summary>
        /// Determines whether the specified <see cref="DirectoryInfo"/> objects are equal,
        /// using Windows-specific comparison logic.
        /// </summary>
        /// <param name="x">The first directory to compare.</param>
        /// <param name="y">The second directory to compare.</param>
        /// <returns>
        /// true if the specified directories are equal; otherwise, false.
        /// </returns>
        public bool Equals(DirectoryInfo x, DirectoryInfo y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="DirectoryInfo"/>,
        /// using Windows-specific hashing logic.
        /// </summary>
        /// <param name="obj">The directory for which a hash code is to be returned.</param>
        /// <returns>
        /// A hash code for the specified directory.
        /// </returns>
        public int GetHashCode(DirectoryInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
