using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a Linux-specific implementation of <see cref="IEqualityComparer{T}"/> for comparing <see cref="FileInfo"/> objects.
    /// Inherits comparison logic from <see cref="LinuxFileSystemInfoComparer"/>, which handles <see cref="FileSystemInfo"/> comparison.
    /// </summary>
    public class LinuxFileInfoComparer : LinuxFileSystemInfoComparer, IEqualityComparer<FileInfo>
    {
        /// <summary>
        /// Determines whether the specified <see cref="FileInfo"/> objects are equal using Linux file system semantics.
        /// </summary>
        /// <param name="x">The first <see cref="FileInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="FileInfo"/> to compare.</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(FileInfo x, FileInfo y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="FileInfo"/> object using Linux file system semantics.
        /// </summary>
        /// <param name="obj">The <see cref="FileInfo"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(FileInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
