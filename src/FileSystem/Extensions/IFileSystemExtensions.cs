using System.Collections.Generic;
using System.IO;
using System.Text;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;

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

        public static FileStream OpenFile(this IFileSystem fileSystem, string path, FileMode mode)
        {
            return fileSystem.OpenFile(path, mode, FileAccess.ReadWrite, FileShare.None);
        }

        public static FileStream OpenFile(this IFileSystem fileSystem, string path, FileMode mode, FileAccess access)
        {
            return fileSystem.OpenFile(path, mode, access, FileShare.None);
        }

        public static FileStream OpenFileRead(this IFileSystem fileSystem, string path)
        {
            return fileSystem.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
        }

        public static StreamReader OpenFileText(this IFileSystem fileSystem, string path)
        {
            return new StreamReader(fileSystem.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read));
        }

        public static FileStream OpenFileWrite(this IFileSystem fileSystem, string path)
        {
            return fileSystem.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public static byte[] ReadAllBytesFromFile(this IFileSystem fileSystem, string path)
        {
            using (var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var result = new byte[fileStream.Length];
                fileStream.Read(result, 0, result.Length);
                return result;
            }
        }

        public static string[] ReadAllLinesFromFile(this IFileSystem fileSystem, string path)
        {
            using (var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
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

        public static string[] ReadAllLinesFromFile(this IFileSystem fileSystem, string path, Encoding encoding)
        {
            using (var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
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

        public static string ReadAllTextFromFile(this IFileSystem fileSystem, string path)
        {
            using (var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string ReadAllTextFromFile(this IFileSystem fileSystem, string path, Encoding encoding)
        {
            using (var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        public static void WriteAllBytesToFile(this IFileSystem fileSystem, string path, byte[] bytes)
        {
            using (var stream = fileSystem.OpenFileWrite(path))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public static void WriteAllLinesToFile(this IFileSystem fileSystem, string path, IEnumerable<string> lines)
        {
            using (var stream = fileSystem.OpenFileWrite(path))
            using (var writer = new StreamWriter(stream))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public static void WriteAllLinesToFile(this IFileSystem fileSystem, string path, IEnumerable<string> lines, Encoding encoding)
        {
            using (var stream = fileSystem.OpenFileWrite(path))
            using (var writer = new StreamWriter(stream, encoding))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public static void WriteAllLinesToFile(this IFileSystem fileSystem, string path, string[] lines)
        {
            WriteAllLinesToFile(fileSystem, path, (IEnumerable<string>)lines);
        }

        public static void WriteAllLinesToFile(this IFileSystem fileSystem, string path, string[] lines, Encoding encoding)
        {
            WriteAllLinesToFile(fileSystem, path, (IEnumerable<string>)lines, encoding);
        }

        public static void WriteAllTextToFile(this IFileSystem fileSystem, string path, string contents)
        {
            using (var stream = fileSystem.OpenFileWrite(path))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(contents);
            }
        }

        public static void WriteAllTextToFile(this IFileSystem fileSystem, string path, string contents, Encoding encoding)
        {
            using (var stream = fileSystem.OpenFileWrite(path))
            using (var writer = new StreamWriter(stream, encoding))
            {
                writer.Write(contents);
            }
        }
    }
}
