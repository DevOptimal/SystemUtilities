using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    public class WindowsDirectoryInfoComparer : WindowsFileSystemInfoComparer, IEqualityComparer<DirectoryInfo>
    {
        public bool Equals(DirectoryInfo x, DirectoryInfo y)
        {
            return base.Equals(x, y);
        }
        public int GetHashCode(DirectoryInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
