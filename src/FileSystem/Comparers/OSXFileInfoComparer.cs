using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a file comparer for <see cref="FileInfo"/> objects on macOS (OSX),
    /// using the comparison logic defined in <see cref="OSXFileSystemInfoComparer"/>.
    /// </summary>
    public class OSXFileInfoComparer : OSXFileSystemInfoComparer, IEqualityComparer<FileInfo>
    {
        /// <summary>
        /// Determines whether the specified <see cref="FileInfo"/> objects are equal,
        /// using the base <see cref="OSXFileSystemInfoComparer"/> logic.
        /// </summary>
        /// <param name="x">The first <see cref="FileInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="FileInfo"/> to compare.</param>
        /// <returns>
        /// true if the specified <see cref="FileInfo"/> objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(FileInfo x, FileInfo y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="FileInfo"/>,
        /// using the base <see cref="OSXFileSystemInfoComparer"/> logic.
        /// </summary>
        /// <param name="obj">The <see cref="FileInfo"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified <see cref="FileInfo"/>.</returns>
        public int GetHashCode(FileInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
