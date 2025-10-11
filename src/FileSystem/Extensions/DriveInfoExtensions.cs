using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="DriveInfo"/> class.
    /// </summary>
    public static class DriveInfoExtensions
    {
        /// <summary>
        /// Determines whether two <see cref="DriveInfo"/> instances are equal using the specified comparer.
        /// </summary>
        /// <param name="a">The first <see cref="DriveInfo"/> to compare.</param>
        /// <param name="b">The second <see cref="DriveInfo"/> to compare.</param>
        /// <param name="comparer">The equality comparer to use for comparison.</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is <c>null</c>.</exception>
        public static bool Equals(this DriveInfo a, DriveInfo b, IEqualityComparer<DriveInfo> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            return comparer.Equals(a, b);
        }

        /// <summary>
        /// Gets a <see cref="DirectoryInfo"/> representing a subdirectory of the drive's root directory with the specified name.
        /// </summary>
        /// <param name="drive">The <see cref="DriveInfo"/> instance.</param>
        /// <param name="name">The name of the subdirectory.</param>
        /// <returns>A <see cref="DirectoryInfo"/> for the specified subdirectory.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="drive"/> or <paramref name="name"/> is <c>null</c>.</exception>
        public static DirectoryInfo GetDirectory(this DriveInfo drive, string name)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return drive.RootDirectory.GetDirectory(name);
        }

        /// <summary>
        /// Gets a <see cref="DirectoryInfo"/> representing a nested subdirectory of the drive's root directory, specified by a sequence of names.
        /// </summary>
        /// <param name="drive">The <see cref="DriveInfo"/> instance.</param>
        /// <param name="names">An array of subdirectory names representing the path.</param>
        /// <returns>A <see cref="DirectoryInfo"/> for the specified nested subdirectory.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="drive"/> or <paramref name="names"/> is <c>null</c>.</exception>
        public static DirectoryInfo GetDirectory(this DriveInfo drive, params string[] names)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            if (names == null) throw new ArgumentNullException(nameof(names));

            return drive.RootDirectory.GetDirectory(names);
        }

        /// <summary>
        /// Gets a <see cref="FileInfo"/> representing a file in the drive's root directory with the specified name.
        /// </summary>
        /// <param name="drive">The <see cref="DriveInfo"/> instance.</param>
        /// <param name="name">The name of the file.</param>
        /// <returns>A <see cref="FileInfo"/> for the specified file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="drive"/> or <paramref name="name"/> is <c>null</c>.</exception>
        public static FileInfo GetFile(this DriveInfo drive, string name)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return drive.RootDirectory.GetFile(name);
        }

        /// <summary>
        /// Gets a <see cref="FileInfo"/> representing a file in a nested subdirectory of the drive's root directory, specified by a sequence of names.
        /// </summary>
        /// <param name="drive">The <see cref="DriveInfo"/> instance.</param>
        /// <param name="names">An array of names representing the path to the file.</param>
        /// <returns>A <see cref="FileInfo"/> for the specified file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="drive"/> or <paramref name="names"/> is <c>null</c>.</exception>
        public static FileInfo GetFile(this DriveInfo drive, params string[] names)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            if (names == null) throw new ArgumentNullException(nameof(names));

            return drive.RootDirectory.GetFile(names);
        }
    }
}
