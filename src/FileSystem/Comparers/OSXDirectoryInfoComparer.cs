using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides an equality comparer for <see cref="DirectoryInfo"/> objects on OSX platforms.
    /// Inherits comparison logic from <see cref="OSXFileSystemInfoComparer"/>.
    /// </summary>
    public class OSXDirectoryInfoComparer : OSXFileSystemInfoComparer, IEqualityComparer<DirectoryInfo>
    {
        /// <summary>
        /// Determines whether the specified <see cref="DirectoryInfo"/> objects are equal.
        /// </summary>
        /// <param name="x">The first <see cref="DirectoryInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="DirectoryInfo"/> to compare.</param>
        /// <returns>
        /// true if the specified <see cref="DirectoryInfo"/> objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(DirectoryInfo x, DirectoryInfo y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="DirectoryInfo"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DirectoryInfo"/> for which a hash code is to be returned.</param>
        /// <returns>
        /// A hash code for the specified <see cref="DirectoryInfo"/>.
        /// </returns>
        public int GetHashCode(DirectoryInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
