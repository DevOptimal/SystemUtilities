using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a file comparer for <see cref="FileInfo"/> objects using Windows-specific comparison logic.
    /// Inherits comparison behavior from <see cref="WindowsFileSystemInfoComparer"/>.
    /// </summary>
    public class WindowsFileInfoComparer : WindowsFileSystemInfoComparer, IEqualityComparer<FileInfo>
    {
        /// <summary>
        /// Determines whether the specified <see cref="FileInfo"/> objects are equal using Windows file system rules.
        /// </summary>
        /// <param name="x">The first <see cref="FileInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="FileInfo"/> to compare.</param>
        /// <returns>
        /// true if the specified <see cref="FileInfo"/> objects are considered equal; otherwise, false.
        /// </returns>
        public bool Equals(FileInfo x, FileInfo y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="FileInfo"/> object using Windows file system rules.
        /// </summary>
        /// <param name="obj">The <see cref="FileInfo"/> for which a hash code is to be returned.</param>
        /// <returns>
        /// A hash code for the specified <see cref="FileInfo"/> object.
        /// </returns>
        public int GetHashCode(FileInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
