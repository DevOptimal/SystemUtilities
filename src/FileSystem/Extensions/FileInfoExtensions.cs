using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class FileInfoExtensions
    {
        public static DriveInfo GetDrive(this FileInfo file)
        {
            return new DriveInfo(Path.GetPathRoot(file.FullName));
        }

        public static FileStream Open(this FileInfo file, FileMode mode, IFileSystem fileSystem)
        {
            return Open(file, mode, FileAccess.ReadWrite, FileShare.None, fileSystem);
        }

        public static FileStream Open(this FileInfo file, FileMode mode, FileAccess access, IFileSystem fileSystem)
        {
            return Open(file, mode, access, FileShare.None, fileSystem);
        }

        public static FileStream Open(this FileInfo file, FileMode mode, FileAccess access, FileShare share, IFileSystem fileSystem)
        {
            return fileSystem.OpenFile(file.FullName, mode, access, share);
        }

        public static FileStream OpenRead(this FileInfo file, IFileSystem fileSystem)
        {
            return fileSystem.OpenFile(file.FullName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
        }

        public static StreamReader OpenText(this FileInfo file, IFileSystem fileSystem)
        {
            return new StreamReader(fileSystem.OpenFile(file.FullName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read));
        }

        public static FileStream OpenWrite(this FileInfo file, IFileSystem fileSystem)
        {
            return fileSystem.OpenFile(file.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public static byte[] ReadAllBytes(this FileInfo file)
        {
            return File.ReadAllBytes(file.FullName);
        }

        public static byte[] ReadAllBytes(this FileInfo file, IFileSystem fileSystem)
        {
            using (var fileStream = fileSystem.OpenFile(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var result = new byte[fileStream.Length];
                fileStream.Read(result, 0, result.Length);
                return result;
            }
        }

        public static string[] ReadAllLines(this FileInfo file)
        {
            return File.ReadAllLines(file.FullName);
        }

        public static string[] ReadAllLines(this FileInfo file, IFileSystem fileSystem)
        {
            using (var fileStream = fileSystem.OpenFile(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                var lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                return lines.ToArray();
            }
        }

        public static string[] ReadAllLines(this FileInfo file, Encoding encoding)
        {
            return File.ReadAllLines(file.FullName, encoding);
        }

        public static string[] ReadAllLines(this FileInfo file, Encoding encoding, IFileSystem fileSystem)
        {
            using (var fileStream = fileSystem.OpenFile(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream, encoding))
            {
                var lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                return lines.ToArray();
            }
        }

        public static string ReadAllText(this FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }

        public static string ReadAllText(this FileInfo file, IFileSystem fileSystem)
        {
            using (var fileStream = fileSystem.OpenFile(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string ReadAllText(this FileInfo file, Encoding encoding)
        {
            return File.ReadAllText(file.FullName, encoding);
        }

        public static string ReadAllText(this FileInfo file, Encoding encoding, IFileSystem fileSystem)
        {
            using (var fileStream = fileSystem.OpenFile(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        public static void WriteAllBytes(this FileInfo file, byte[] bytes)
        {
            File.WriteAllBytes(file.FullName, bytes);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines)
        {
            File.WriteAllLines(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines)
        {
            File.WriteAllLines(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, Encoding encoding)
        {
            File.WriteAllLines(file.FullName, lines, encoding);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines, Encoding encoding)
        {
            File.WriteAllLines(file.FullName, lines, encoding);
        }

        public static void WriteAllText(this FileInfo file, string contents)
        {
            File.WriteAllText(file.FullName, contents);
        }

        public static void WriteAllText(this FileInfo file, string contents, Encoding encoding)
        {
            File.WriteAllText(file.FullName, contents, encoding);
        }
    }
}
