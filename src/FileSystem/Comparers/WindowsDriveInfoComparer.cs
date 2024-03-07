using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    public class WindowsDriveInfoComparer : IEqualityComparer<DriveInfo>
    {
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
                return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
            }
        }

        public int GetHashCode(DriveInfo obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.Name.ToLower().GetHashCode();
            }
        }
    }
}
