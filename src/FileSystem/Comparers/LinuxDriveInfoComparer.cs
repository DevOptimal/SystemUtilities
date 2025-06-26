using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    /// <summary>
    /// Provides a mechanism for comparing <see cref="DriveInfo"/> objects for equality on Linux systems.
    /// Comparison is based on the drive's name using ordinal string comparison.
    /// </summary>
    public class LinuxDriveInfoComparer : IEqualityComparer<DriveInfo>
    {
        /// <summary>
        /// Determines whether the specified <see cref="DriveInfo"/> objects are equal.
        /// </summary>
        /// <param name="x">The first <see cref="DriveInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="DriveInfo"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified objects are equal or reference the same object; otherwise, <c>false</c>.
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
                // Compare drive names using ordinal comparison.
                return x.Name.Equals(y.Name, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="DriveInfo"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="DriveInfo"/> for which a hash code is to be returned.</param>
        /// <returns>
        /// A hash code for the specified object, or 0 if the object is <c>null</c>.
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
                // Use the hash code of the drive name.
                return obj.Name.GetHashCode();
            }
        }
    }
}
