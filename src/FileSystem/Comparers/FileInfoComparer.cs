using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a set of <see cref="IEqualityComparer{T}"/> implementations for comparing <see cref="FileInfo"/> objects.
    /// Inherits comparison logic from <see cref="FileSystemInfoComparer"/> and adapts it for <see cref="FileInfo"/>.
    /// </summary>
    public class FileInfoComparer : FileSystemInfoComparer, IEqualityComparer<FileInfo>
    {
        /// <summary>
        /// Gets the default <see cref="IEqualityComparer{T}"/> for <see cref="FileInfo"/> objects.
        /// The default comparer is platform-aware and selects the appropriate implementation based on the current OS.
        /// </summary>
        public new static IEqualityComparer<FileInfo> Default => new FileInfoComparer();

        /// <summary>
        /// Gets a <see cref="IEqualityComparer{T}"/> for <see cref="FileInfo"/> objects that uses Linux-specific comparison logic.
        /// </summary>
        public new static IEqualityComparer<FileInfo> Linux => new LinuxFileInfoComparer();

        /// <summary>
        /// Gets a <see cref="IEqualityComparer{T}"/> for <see cref="FileInfo"/> objects that uses macOS-specific comparison logic.
        /// </summary>
        public new static IEqualityComparer<FileInfo> OSX => new OSXFileInfoComparer();

        /// <summary>
        /// Gets a <see cref="IEqualityComparer{T}"/> for <see cref="FileInfo"/> objects that uses Windows-specific comparison logic.
        /// </summary>
        public new static IEqualityComparer<FileInfo> Windows => new WindowsFileInfoComparer();

        /// <summary>
        /// Determines whether the specified <see cref="FileInfo"/> objects are equal using the base comparison logic.
        /// </summary>
        /// <param name="x">The first <see cref="FileInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="FileInfo"/> to compare.</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(FileInfo x, FileInfo y)
        {
            return base.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="FileInfo"/> object using the base comparison logic.
        /// </summary>
        /// <param name="obj">The <see cref="FileInfo"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(FileInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
