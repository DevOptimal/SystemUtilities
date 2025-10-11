using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides an <see cref="IEqualityComparer{T}"/> implementation for <see cref="FileSystemInfo"/>
    /// that delegates comparison to an OS-specific comparer.
    /// </summary>
    public class FileSystemInfoComparer : IEqualityComparer<FileSystemInfo>
    {
        /// <summary>
        /// Gets the default comparer for the current operating system.
        /// </summary>
        public static IEqualityComparer<FileSystemInfo> Default => new FileSystemInfoComparer();

        /// <summary>
        /// Gets a comparer that uses Linux-specific file system comparison logic.
        /// </summary>
        public static IEqualityComparer<FileSystemInfo> Linux => new LinuxFileSystemInfoComparer();

        /// <summary>
        /// Gets a comparer that uses macOS-specific file system comparison logic.
        /// </summary>
        public static IEqualityComparer<FileSystemInfo> OSX => new OSXFileSystemInfoComparer();

        /// <summary>
        /// Gets a comparer that uses Windows-specific file system comparison logic.
        /// </summary>
        public static IEqualityComparer<FileSystemInfo> Windows => new WindowsFileSystemInfoComparer();

        // Lazily initializes the appropriate OS-specific comparer based on the current platform.
        private readonly Lazy<IEqualityComparer<FileSystemInfo>> comparerLazy = new Lazy<IEqualityComparer<FileSystemInfo>>(() =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxFileSystemInfoComparer();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsFileSystemInfoComparer();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new OSXFileSystemInfoComparer();
            }
            else
            {
                throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
            }
        });

        /// <summary>
        /// Determines whether the specified <see cref="FileSystemInfo"/> objects are equal using the OS-specific comparer.
        /// </summary>
        /// <param name="x">The first <see cref="FileSystemInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="FileSystemInfo"/> to compare.</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(FileSystemInfo x, FileSystemInfo y)
        {
            return comparerLazy.Value.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="FileSystemInfo"/> object using the OS-specific comparer.
        /// </summary>
        /// <param name="obj">The <see cref="FileSystemInfo"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(FileSystemInfo obj)
        {
            return comparerLazy.Value.GetHashCode(obj);
        }
    }
}
