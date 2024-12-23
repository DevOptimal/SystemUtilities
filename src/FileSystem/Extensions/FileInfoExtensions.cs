using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class FileInfoExtensions
    {
        public static void CopyTo(this FileInfo file, FileInfo destFile)
        {
            CopyTo(file, destFile, false, new DefaultFileSystem());
        }

        public static void CopyTo(this FileInfo file, FileInfo destFile, bool overwrite)
        {
            CopyTo(file, destFile, overwrite, new DefaultFileSystem());
        }

        public static void CopyTo(this FileInfo file, FileInfo destFile, IFileSystem fileSystem)
        {
            CopyTo(file, destFile, false, fileSystem);
        }

        public static void CopyTo(this FileInfo file, FileInfo destFile, bool overwrite, IFileSystem fileSystem)
        {
            if (destFile == null) throw new ArgumentNullException(nameof(destFile));

            CopyTo(file, destFile.FullName, overwrite, fileSystem);
        }

        public static void CopyTo(this FileInfo file, string destFileName, IFileSystem fileSystem)
        {
            CopyTo(file, destFileName, false, fileSystem);
        }

        public static void CopyTo(this FileInfo file, string destFileName, bool overwrite, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.CopyFile(file.FullName, destFileName, overwrite);
        }

        public static bool Equals(this FileInfo a, FileInfo b, IEqualityComparer<FileInfo> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            return comparer.Equals(a, b);
        }

        public static bool Exists(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.FileExists(file.FullName);
        }

        public static DriveInfo GetDrive(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return new DriveInfo(Path.GetPathRoot(file.FullName));
        }

        public static void HardLinkTo(this FileInfo file, FileInfo destFile)
        {
            HardLinkTo(file, destFile, false, new DefaultFileSystem());
        }

        public static void HardLinkTo(this FileInfo file, FileInfo destFile, bool overwrite)
        {
            HardLinkTo(file, destFile, overwrite, new DefaultFileSystem());
        }

        public static void HardLinkTo(this FileInfo file, FileInfo destFile, IFileSystem fileSystem)
        {
            HardLinkTo(file, destFile, false, fileSystem);
        }

        public static void HardLinkTo(this FileInfo file, FileInfo destFile, bool overwrite, IFileSystem fileSystem)
        {
            if (destFile == null) throw new ArgumentNullException(nameof(destFile));

            HardLinkTo(file, destFile.FullName, overwrite, fileSystem);
        }

        public static void HardLinkTo(this FileInfo file, string destFileName)
        {
            HardLinkTo(file, destFileName, false, new DefaultFileSystem());
        }

        public static void HardLinkTo(this FileInfo file, string destFileName, bool overwrite)
        {
            HardLinkTo(file, destFileName, overwrite, new DefaultFileSystem());
        }

        public static void HardLinkTo(this FileInfo file, string destFileName, IFileSystem fileSystem)
        {
            HardLinkTo(file, destFileName, false , fileSystem);
        }

        public static void HardLinkTo(this FileInfo file, string destFileName, bool overwrite, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.HardLinkFile(file.FullName, destFileName, overwrite);
        }

        public static void MoveTo(this FileInfo file, FileInfo destFile)
        {
            MoveTo(file, destFile, false, new DefaultFileSystem());
        }

        public static void MoveTo(this FileInfo file, FileInfo destFile, bool overwrite)
        {
            MoveTo(file, destFile, overwrite, new DefaultFileSystem());
        }

        public static void MoveTo(this FileInfo file, FileInfo destFile, IFileSystem fileSystem)
        {
            MoveTo(file, destFile, false, fileSystem);
        }

        public static void MoveTo(this FileInfo file, FileInfo destFile, bool overwrite, IFileSystem fileSystem)
        {
            if (destFile == null) throw new ArgumentNullException(nameof(destFile));

            MoveTo(file, destFile.FullName, overwrite, fileSystem);
        }

        public static void MoveTo(this FileInfo file, string destFileName, bool overwrite)
        {
            MoveTo(file, destFileName, overwrite, new DefaultFileSystem());
        }

        public static void MoveTo(this FileInfo file, string destFileName, IFileSystem fileSystem)
        {
            MoveTo(file, destFileName, false, fileSystem);
        }

        public static void MoveTo(this FileInfo file, string destFileName, bool overwrite, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.MoveFile(file.FullName, destFileName, overwrite);
        }

        public static FileStream Open(this FileInfo file, FileMode mode, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(file.FullName, mode);
        }

        public static FileStream Open(this FileInfo file, FileMode mode, FileAccess access, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(file.FullName, mode, access);
        }

        public static FileStream Open(this FileInfo file, FileMode mode, FileAccess access, FileShare share, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(file.FullName, mode, access, share);
        }

        public static FileStream OpenRead(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFileRead(file.FullName);
        }

        public static StreamReader OpenText(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFileText(file.FullName);
        }

        public static FileStream OpenWrite(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFileWrite(file.FullName);
        }

        public static byte[] ReadAllBytes(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllBytes(file.FullName);
        }

        public static byte[] ReadAllBytes(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllBytesFromFile(file.FullName);
        }

        public static string[] ReadAllLines(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllLines(file.FullName);
        }

        public static string[] ReadAllLines(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllLinesFromFile(file.FullName);
        }

        public static string[] ReadAllLines(this FileInfo file, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllLines(file.FullName, encoding);
        }

        public static string[] ReadAllLines(this FileInfo file, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllLinesFromFile(file.FullName, encoding);
        }

        public static string ReadAllText(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllText(file.FullName);
        }

        public static string ReadAllText(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllTextFromFile(file.FullName);
        }

        public static string ReadAllText(this FileInfo file, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllText(file.FullName, encoding);
        }

        public static string ReadAllText(this FileInfo file, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllTextFromFile(file.FullName, encoding);
        }

        public static void WriteAllBytes(this FileInfo file, byte[] bytes)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllBytes(file.FullName, bytes);
        }

        public static void WriteAllBytes(this FileInfo file, byte[] bytes, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllBytesToFile(file.FullName, bytes);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllLines(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllLinesToFile(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllLines(file.FullName, lines, encoding);
        }

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllLinesToFile(file.FullName, lines, encoding);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllLines(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllLinesToFile(file.FullName, lines);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllLines(file.FullName, lines, encoding);
        }

        public static void WriteAllLines(this FileInfo file, string[] lines, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllLinesToFile(file.FullName, lines, encoding);
        }

        public static void WriteAllText(this FileInfo file, string contents)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllText(file.FullName, contents);
        }

        public static void WriteAllText(this FileInfo file, string contents, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllTextToFile(file.FullName, contents);
        }

        public static void WriteAllText(this FileInfo file, string contents, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllText(file.FullName, contents, encoding);
        }

        public static void WriteAllText(this FileInfo file, string contents, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllTextToFile(file.FullName, contents, encoding);
        }
    }
}
