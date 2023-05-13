using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static DirectoryInfo GetDirectory(this DirectoryInfo directory, string name)
        {
            return new DirectoryInfo(Path.Combine(directory.FullName, name));
        }

        public static DriveInfo GetDrive(this DirectoryInfo directory)
        {
            return new DriveInfo(Path.GetPathRoot(directory.FullName));
        }

        public static FileInfo GetFile(this DirectoryInfo directory, string name)
        {
            return new FileInfo(Path.Combine(directory.FullName, name));
        }
    }
}
