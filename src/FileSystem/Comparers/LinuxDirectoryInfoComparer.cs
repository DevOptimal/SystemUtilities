using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a Linux-specific equality comparer for <see cref="DirectoryInfo"/> objects.
    /// </summary>
    /// <remarks>
    /// This comparer uses the logic defined in <see cref="LinuxFileSystemInfoComparer"/>
    /// to compare <see cref="DirectoryInfo"/> instances, ensuring that directory comparisons
    /// are consistent with Linux file system semantics.
    /// </remarks>
    public class LinuxDirectoryInfoComparer : LinuxFileSystemInfoComparer, IEqualityComparer<DirectoryInfo>
    {
        /// <summary>
        /// Determines whether the specified <see cref="DirectoryInfo"/> objects are equal
        /// according to Linux file system rules.
        /// </summary>
        /// <param name="x">The first <see cref="DirectoryInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="DirectoryInfo"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="DirectoryInfo"/> objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(DirectoryInfo x, DirectoryInfo y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="DirectoryInfo"/> object,
        /// consistent with Linux file system rules.
        /// </summary>
        /// <param name="obj">The <see cref="DirectoryInfo"/> for which to get a hash code.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(DirectoryInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
