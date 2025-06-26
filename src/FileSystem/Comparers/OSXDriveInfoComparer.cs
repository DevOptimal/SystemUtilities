using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a case-insensitive equality comparison for <see cref="DriveInfo"/> objects on macOS (OSX).
    /// </summary>
    public class OSXDriveInfoComparer : IEqualityComparer<DriveInfo>
    {
        /// <summary>
        /// Determines whether two <see cref="DriveInfo"/> instances are equal based on their drive names, ignoring case.
        /// </summary>
        /// <param name="x">The first <see cref="DriveInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="DriveInfo"/> to compare.</param>
        /// <returns>
        /// true if both <see cref="DriveInfo"/> objects are the same instance or have equal names (case-insensitive); otherwise, false.
        /// </returns>
        public bool Equals(DriveInfo x, DriveInfo y)
        {
            if (x == y)
            {
                // Reference equality or both are null.
                return true;
            }
            else if (x == null || y == null)
            {
                // One is null, the other is not.
                return false;
            }
            else
            {
                // Compare drive names, ignoring case.
                return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="DriveInfo"/>, using a case-insensitive hash of the drive name.
        /// </summary>
        /// <param name="obj">The <see cref="DriveInfo"/> for which to get the hash code.</param>
        /// <returns>
        /// A hash code for the drive name (case-insensitive), or 0 if <paramref name="obj"/> is null.
        /// </returns>
        public int GetHashCode(DriveInfo obj)
        {
            if (obj == null)
            {
                // Null objects return a hash code of 0.
                return 0;
            }
            else
            {
                // Use lower-case drive name for case-insensitive hash code.
                return obj.Name.ToLower().GetHashCode();
            }
        }
    }
}
