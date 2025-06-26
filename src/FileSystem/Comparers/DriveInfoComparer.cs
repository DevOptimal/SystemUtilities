using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides an <see cref="IEqualityComparer{DriveInfo}"/> implementation that selects
    /// the appropriate comparer for the current operating system at runtime.
    /// </summary>
    public class DriveInfoComparer : IEqualityComparer<DriveInfo>
    {
        /// <summary>
        /// Gets the default <see cref="DriveInfoComparer"/>, which selects the comparer based on the current OS.
        /// </summary>
        public static IEqualityComparer<DriveInfo> Default => new DriveInfoComparer();

        /// <summary>
        /// Gets a <see cref="DriveInfo"/> comparer for Linux platforms.
        /// </summary>
        public static IEqualityComparer<DriveInfo> Linux => new LinuxDriveInfoComparer();

        /// <summary>
        /// Gets a <see cref="DriveInfo"/> comparer for macOS platforms.
        /// </summary>
        public static IEqualityComparer<DriveInfo> OSX => new OSXDriveInfoComparer();

        /// <summary>
        /// Gets a <see cref="DriveInfo"/> comparer for Windows platforms.
        /// </summary>
        public static IEqualityComparer<DriveInfo> Windows => new WindowsDriveInfoComparer();

        // Lazily initializes the platform-specific comparer based on the current OS.
        private readonly Lazy<IEqualityComparer<DriveInfo>> comparerLazy = new(() =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxDriveInfoComparer();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsDriveInfoComparer();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new OSXDriveInfoComparer();
            }
            else
            {
                throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
            }
        });

        /// <summary>
        /// Determines whether the specified <see cref="DriveInfo"/> objects are equal using the platform-specific comparer.
        /// </summary>
        /// <param name="x">The first <see cref="DriveInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="DriveInfo"/> to compare.</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(DriveInfo x, DriveInfo y)
        {
            return comparerLazy.Value.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="DriveInfo"/> object using the platform-specific comparer.
        /// </summary>
        /// <param name="obj">The <see cref="DriveInfo"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(DriveInfo obj)
        {
            return comparerLazy.Value.GetHashCode(obj);
        }
    }
}
