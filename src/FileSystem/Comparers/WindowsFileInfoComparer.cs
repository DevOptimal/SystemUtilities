using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    public class WindowsFileInfoComparer : WindowsFileSystemInfoComparer, IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo x, FileInfo y)
        {
            return base.Equals(x, y);
        }
        public int GetHashCode(FileInfo obj)
        {
            return base.GetHashCode(obj);
        }
    }
}
