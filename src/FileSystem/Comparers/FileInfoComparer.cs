using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Comparers
{
    public class FileInfoComparer : FileSystemInfoComparer, IEqualityComparer<FileInfo>
    {
        public new static IEqualityComparer<FileInfo> Default => new FileInfoComparer();

        public new static IEqualityComparer<FileInfo> Linux => new LinuxFileInfoComparer();

        public new static IEqualityComparer<FileInfo> OSX => new OSXFileInfoComparer();

        public new static IEqualityComparer<FileInfo> Windows => new WindowsFileInfoComparer();

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
