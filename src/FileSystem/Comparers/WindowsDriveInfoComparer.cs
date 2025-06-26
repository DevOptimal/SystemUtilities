using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a case-insensitive equality comparison for <see cref="DriveInfo"/> objects based on their drive names.
    /// </summary>
    public class WindowsDriveInfoComparer : IEqualityComparer<DriveInfo>
    {
        /// <summary>
        /// Determines whether two <see cref="DriveInfo"/> instances are equal by comparing their drive names, ignoring case.
        /// </summary>
        /// <param name="x">The first <see cref="DriveInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="DriveInfo"/> to compare.</param>
        /// <returns>
        /// true if the drive names are equal (case-insensitive) or both references are the same; otherwise, false.
        /// </returns>
        public bool Equals(DriveInfo x, DriveInfo y)
        {
            if (x == y)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else
            {
                // Compare drive names using case-insensitive comparison
                return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="DriveInfo"/>, using a case-insensitive hash of the drive name.
        /// </summary>
        /// <param name="obj">The <see cref="DriveInfo"/> for which to get a hash code.</param>
        /// <returns>
        /// A hash code for the drive name (case-insensitive), or 0 if <paramref name="obj"/> is null.
        /// </returns>
        public int GetHashCode(DriveInfo obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                // Use lower-case drive name to ensure case-insensitive hash code
                return obj.Name.ToLower().GetHashCode();
            }
        }
    }
}
