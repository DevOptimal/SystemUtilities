using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class IFileSystemExtensions
    {
        public static string[] GetDirectories(this IFileSystem fileSystem, string path, bool recursive)
        {
            return fileSystem.GetDirectories(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public static string[] GetFiles(this IFileSystem fileSystem, string path, bool recursive)
        {
            return fileSystem.GetFiles(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
    }
}
