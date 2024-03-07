using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class FileInfoExtensions
    {
        public static bool Equals(this FileInfo a, FileInfo b, IEqualityComparer<FileInfo> comparer)
        {
            return comparer.Equals(a, b);
        }

        public static DriveInfo GetDrive(this FileInfo file)
        {
            return new DriveInfo(Path.GetPathRoot(file.FullName));
        }

        public static FileStream Open(this FileInfo file, FileMode mode, IFileSystem fileSystem)
        {
            return fileSystem.OpenFile(file.FullName, mode);
        }

        public static FileStream Open(this FileInfo file, FileMode mode, FileAccess access, IFileSystem fileSystem)
        {
            return fileSystem.OpenFile(file.FullName, mode, access);
        }

        public static FileStream Open(this FileInfo file, FileMode mode, FileAccess access, FileShare share, IFileSystem fileSystem)
        {
            return fileSystem.OpenFile(file.FullName, mode, access, share);
        }

        public static FileStream OpenRead(this FileInfo file, IFileSystem fileSystem)
        {
            return fileSystem.OpenFileRead(file.FullName);
        }

        public static StreamReader OpenText(this FileInfo file, IFileSystem fileSystem)
        {
            return fileSystem.OpenFileText(file.FullName);
        }

        public static FileStream OpenWrite(this FileInfo file, IFileSystem fileSystem)
        {
            return fileSystem.OpenFileWrite(file.FullName);
        }

        public static byte[] ReadAllBytes(this FileInfo file)
        {
            return File.ReadAllBytes(file.FullName);
        }

        public static byte[] ReadAllBytes(this FileInfo file, IFileSystem fileSystem)
        {
            return fileSystem.ReadAllBytesFromFile(file.FullName);
        }

        public static string[] ReadAllLines(this FileInfo file)
        {
            return File.ReadAllLines(file.FullName);
        }

        public static string[] ReadAllLines(this FileInfo file, IFileSystem fileSystem)
        {
            return fileSystem.ReadAllLinesFromFile(file.FullName);
        }

        public static string[] ReadAllLines(this FileInfo file, Encoding encoding)
        {
            return File.ReadAllLines(file.FullName, encoding);
        }

        public static string[] ReadAllLines(this FileInfo file, Encoding encoding, IFileSystem fileSystem)
        {
            return fileSystem.ReadAllLinesFromFile(file.FullName, encoding);
        }

        public static string ReadAllText(this FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }

        public static string ReadAllText(this FileInfo file, IFileSystem fileSystem)
        {
            return fileSystem.ReadAllTextFromFile(file.FullName);
        }

        public static string ReadAllText(this FileInfo file, Encoding encoding)
        {
            return File.ReadAllText(file.FullName, encoding);
        }

        public static string ReadAllText(this FileInfo file, Encoding encoding, IFileSystem fileSystem)
        {
            return fileSystem.ReadAllTextFromFile(file.FullName, encoding);
        }

        public static void WriteAllBytes(this FileInfo file, byte[] bytes)
        {
            File.WriteAllBytes(file.FullName, bytes);
        }

        public static void WriteAllBytes(this FileInfo file, byte[] bytes, IFileSystem fileSystem)
        {
            fileSystem.WriteAllBytesToFile(file.FullName, bytes);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines)
        {
            File.WriteAllLines(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, IFileSystem fileSystem)
        {
            fileSystem.WriteAllLinesToFile(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, Encoding encoding)
        {
            File.WriteAllLines(file.FullName, lines, encoding);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, Encoding encoding, IFileSystem fileSystem)
        {
            fileSystem.WriteAllLinesToFile(file.FullName, lines, encoding);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines)
        {
            File.WriteAllLines(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines, IFileSystem fileSystem)
        {
            fileSystem.WriteAllLinesToFile(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines, Encoding encoding)
        {
            File.WriteAllLines(file.FullName, lines, encoding);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines, Encoding encoding, IFileSystem fileSystem)
        {
            fileSystem.WriteAllLinesToFile(file.FullName, lines, encoding);
        }

        public static void WriteAllText(this FileInfo file, string contents)
        {
            File.WriteAllText(file.FullName, contents);
        }

        public static void WriteAllText(this FileInfo file, string contents, IFileSystem fileSystem)
        {
            fileSystem.WriteAllTextToFile(file.FullName, contents);
        }

        public static void WriteAllText(this FileInfo file, string contents, Encoding encoding)
        {
            File.WriteAllText(file.FullName, contents, encoding);
        }

        public static void WriteAllText(this FileInfo file, string contents, Encoding encoding, IFileSystem fileSystem)
        {
            fileSystem.WriteAllTextToFile(file.FullName, contents, encoding);
        }
    }
}
