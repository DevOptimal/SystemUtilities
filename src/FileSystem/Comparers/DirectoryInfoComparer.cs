using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a platform-aware equality comparer for <see cref="DirectoryInfo"/> objects.
    /// </summary>
    /// <remarks>
    /// This comparer delegates equality and hash code operations to <see cref="FileSystemInfoComparer"/>,
    /// ensuring consistent behavior across different operating systems.
    /// </remarks>
    public class DirectoryInfoComparer : FileSystemInfoComparer, IEqualityComparer<DirectoryInfo>
    {
        /// <summary>
        /// Gets the default <see cref="IEqualityComparer{T}"/> for <see cref="DirectoryInfo"/>,
        /// which is platform-aware.
        /// </summary>
        public new static IEqualityComparer<DirectoryInfo> Default => new DirectoryInfoComparer();

        /// <summary>
        /// Gets a Linux-specific <see cref="IEqualityComparer{T}"/> for <see cref="DirectoryInfo"/>.
        /// </summary>
        public new static IEqualityComparer<DirectoryInfo> Linux => new LinuxDirectoryInfoComparer();

        /// <summary>
        /// Gets a macOS-specific <see cref="IEqualityComparer{T}"/> for <see cref="DirectoryInfo"/>.
        /// </summary>
        public new static IEqualityComparer<DirectoryInfo> OSX => new OSXDirectoryInfoComparer();

        /// <summary>
        /// Gets a Windows-specific <see cref="IEqualityComparer{T}"/> for <see cref="DirectoryInfo"/>.
        /// </summary>
        public new static IEqualityComparer<DirectoryInfo> Windows => new WindowsDirectoryInfoComparer();

        /// <summary>
        /// Determines whether the specified <see cref="DirectoryInfo"/> objects are equal.
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
        /// Returns a hash code for the specified <see cref="DirectoryInfo"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DirectoryInfo"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified <see cref="DirectoryInfo"/>.</returns>
        public int GetHashCode(DirectoryInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
