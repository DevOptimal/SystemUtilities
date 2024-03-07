using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    public class DirectoryInfoComparer : FileSystemInfoComparer, IEqualityComparer<DirectoryInfo>
    {
        public new static IEqualityComparer<DirectoryInfo> Default => new DirectoryInfoComparer();

        public new static IEqualityComparer<DirectoryInfo> Linux => new LinuxDirectoryInfoComparer();

        public new static IEqualityComparer<DirectoryInfo> OSX => new OSXDirectoryInfoComparer();

        public new static IEqualityComparer<DirectoryInfo> Windows => new WindowsDirectoryInfoComparer();

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
