using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.Comparers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="FileInfo"/> to support additional file operations,
    /// including copying, moving, hard linking, and reading/writing with optional abstraction via <see cref="IFileSystem"/>.
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Copies the file to the specified destination file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        public static void CopyTo(this FileInfo file, FileInfo destFile)
        {
            CopyTo(file, destFile, false, new DefaultFileSystem());
        }

        /// <summary>
        /// Copies the file to the specified destination file, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        public static void CopyTo(this FileInfo file, FileInfo destFile, bool overwrite)
        {
            CopyTo(file, destFile, overwrite, new DefaultFileSystem());
        }

        /// <summary>
        /// Copies the file to the specified destination file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void CopyTo(this FileInfo file, FileInfo destFile, IFileSystem fileSystem)
        {
            CopyTo(file, destFile, false, fileSystem);
        }

        /// <summary>
        /// Copies the file to the specified destination file using the provided file system abstraction, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void CopyTo(this FileInfo file, FileInfo destFile, bool overwrite, IFileSystem fileSystem)
        {
            if (destFile == null) throw new ArgumentNullException(nameof(destFile));

            CopyTo(file, destFile.FullName, overwrite, fileSystem);
        }

        /// <summary>
        /// Copies the file to the specified destination file name using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void CopyTo(this FileInfo file, string destFileName, IFileSystem fileSystem)
        {
            CopyTo(file, destFileName, false, fileSystem);
        }

        /// <summary>
        /// Copies the file to the specified destination file name using the provided file system abstraction, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void CopyTo(this FileInfo file, string destFileName, bool overwrite, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.CopyFile(file.FullName, destFileName, overwrite);
        }

        /// <summary>
        /// Determines whether two <see cref="FileInfo"/> instances are equal using the specified comparer.
        /// </summary>
        /// <param name="a">The first file to compare.</param>
        /// <param name="b">The second file to compare.</param>
        /// <param name="comparer">The equality comparer to use.</param>
        public static bool Equals(this FileInfo a, FileInfo b, IEqualityComparer<FileInfo> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            return comparer.Equals(a, b);
        }

        /// <summary>
        /// Determines whether the file exists using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static bool Exists(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.FileExists(file.FullName);
        }

        /// <summary>
        /// Gets the drive information for the file.
        /// </summary>
        /// <param name="file">The file whose drive information to get.</param>
        /// <returns>The <see cref="DriveInfo"/> for the file.</returns>
        public static DriveInfo GetDrive(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return new DriveInfo(Path.GetPathRoot(file.FullName));
        }

        /// <summary>
        /// Creates a hard link to the specified destination file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        public static void HardLinkTo(this FileInfo file, FileInfo destFile)
        {
            HardLinkTo(file, destFile, false, new DefaultFileSystem());
        }

        /// <summary>
        /// Creates a hard link to the specified destination file, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        public static void HardLinkTo(this FileInfo file, FileInfo destFile, bool overwrite)
        {
            HardLinkTo(file, destFile, overwrite, new DefaultFileSystem());
        }

        /// <summary>
        /// Creates a hard link to the specified destination file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void HardLinkTo(this FileInfo file, FileInfo destFile, IFileSystem fileSystem)
        {
            HardLinkTo(file, destFile, false, fileSystem);
        }

        /// <summary>
        /// Creates a hard link to the specified destination file using the provided file system abstraction, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void HardLinkTo(this FileInfo file, FileInfo destFile, bool overwrite, IFileSystem fileSystem)
        {
            if (destFile == null) throw new ArgumentNullException(nameof(destFile));

            HardLinkTo(file, destFile.FullName, overwrite, fileSystem);
        }

        /// <summary>
        /// Creates a hard link to the specified destination file name.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFileName">The destination file name.</param>
        public static void HardLinkTo(this FileInfo file, string destFileName)
        {
            HardLinkTo(file, destFileName, false, new DefaultFileSystem());
        }

        /// <summary>
        /// Creates a hard link to the specified destination file name, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        public static void HardLinkTo(this FileInfo file, string destFileName, bool overwrite)
        {
            HardLinkTo(file, destFileName, overwrite, new DefaultFileSystem());
        }

        /// <summary>
        /// Creates a hard link to the specified destination file name using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void HardLinkTo(this FileInfo file, string destFileName, IFileSystem fileSystem)
        {
            HardLinkTo(file, destFileName, false, fileSystem);
        }

        /// <summary>
        /// Creates a hard link to the specified destination file name using the provided file system abstraction, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void HardLinkTo(this FileInfo file, string destFileName, bool overwrite, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.HardLinkFile(file.FullName, destFileName, overwrite);
        }

        /// <summary>
        /// Determines whether a file is a descendant of a directory.
        /// </summary>
        /// <param name="file">The file to test.</param>
        /// <param name="ancestor">The ancestor directory to test.</param>
        /// <returns>True if <paramref name="file"/> is a descendant of <paramref name="ancestor"/>, false otherwise.</returns>
        public static bool IsDescendantOf(this FileInfo file, DirectoryInfo ancestor)
        {
            var comparer = new DirectoryInfoComparer();
            var directory = file.Directory;
            return comparer.Equals(directory, ancestor) || directory.IsDescendantOf(ancestor, comparer);
        }

        /// <summary>
        /// Moves the file to the specified destination file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        public static void MoveTo(this FileInfo file, FileInfo destFile)
        {
            MoveTo(file, destFile, false, new DefaultFileSystem());
        }

        /// <summary>
        /// Moves the file to the specified destination file, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        public static void MoveTo(this FileInfo file, FileInfo destFile, bool overwrite)
        {
            MoveTo(file, destFile, overwrite, new DefaultFileSystem());
        }

        /// <summary>
        /// Moves the file to the specified destination file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void MoveTo(this FileInfo file, FileInfo destFile, IFileSystem fileSystem)
        {
            MoveTo(file, destFile, false, fileSystem);
        }

        /// <summary>
        /// Moves the file to the specified destination file using the provided file system abstraction, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFile">The destination file.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void MoveTo(this FileInfo file, FileInfo destFile, bool overwrite, IFileSystem fileSystem)
        {
            if (destFile == null) throw new ArgumentNullException(nameof(destFile));

            MoveTo(file, destFile.FullName, overwrite, fileSystem);
        }

        /// <summary>
        /// Moves the file to the specified destination file name, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        public static void MoveTo(this FileInfo file, string destFileName, bool overwrite)
        {
            MoveTo(file, destFileName, overwrite, new DefaultFileSystem());
        }

        /// <summary>
        /// Moves the file to the specified destination file name using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void MoveTo(this FileInfo file, string destFileName, IFileSystem fileSystem)
        {
            MoveTo(file, destFileName, false, fileSystem);
        }

        /// <summary>
        /// Moves the file to the specified destination file name using the provided file system abstraction, optionally overwriting.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void MoveTo(this FileInfo file, string destFileName, bool overwrite, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.MoveFile(file.FullName, destFileName, overwrite);
        }

        /// <summary>
        /// Opens the file with the specified mode using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <param name="mode">The file mode to use.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
        public static FileStream Open(this FileInfo file, FileMode mode, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(file.FullName, mode);
        }

        /// <summary>
        /// Opens the file with the specified mode and access using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <param name="mode">The file mode to use.</param>
        /// <param name="access">The file access to use.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
        public static FileStream Open(this FileInfo file, FileMode mode, FileAccess access, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(file.FullName, mode, access);
        }

        /// <summary>
        /// Opens the file with the specified mode, access, and share using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <param name="mode">The file mode to use.</param>
        /// <param name="access">The file access to use.</param>
        /// <param name="share">The file sharing mode to use.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
        public static FileStream Open(this FileInfo file, FileMode mode, FileAccess access, FileShare share, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(file.FullName, mode, access, share);
        }

        /// <summary>
        /// Opens the file for reading using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
        public static FileStream OpenRead(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFileRead(file.FullName);
        }

        /// <summary>
        /// Opens the file for reading text using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>A <see cref="StreamReader"/> for the opened file.</returns>
        public static StreamReader OpenText(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFileText(file.FullName);
        }

        /// <summary>
        /// Opens the file for writing using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
        public static FileStream OpenWrite(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFileWrite(file.FullName);
        }

        /// <summary>
        /// Reads all bytes from the file.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <returns>The file contents as a byte array.</returns>
        public static byte[] ReadAllBytes(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllBytes(file.FullName);
        }

        /// <summary>
        /// Reads all bytes from the file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>The file contents as a byte array.</returns>
        public static byte[] ReadAllBytes(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllBytesFromFile(file.FullName);
        }

        /// <summary>
        /// Reads all lines from the file.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <returns>The file contents as an array of strings.</returns>
        public static string[] ReadAllLines(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllLines(file.FullName);
        }

        /// <summary>
        /// Reads all lines from the file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>The file contents as an array of strings.</returns>
        public static string[] ReadAllLines(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllLinesFromFile(file.FullName);
        }

        /// <summary>
        /// Reads all lines from the file using the specified encoding.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>The file contents as an array of strings.</returns>
        public static string[] ReadAllLines(this FileInfo file, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllLines(file.FullName, encoding);
        }

        /// <summary>
        /// Reads all lines from the file using the specified encoding and file system abstraction.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>The file contents as an array of strings.</returns>
        public static string[] ReadAllLines(this FileInfo file, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllLinesFromFile(file.FullName, encoding);
        }

        /// <summary>
        /// Reads all text from the file.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <returns>The file contents as a string.</returns>
        public static string ReadAllText(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllText(file.FullName);
        }

        /// <summary>
        /// Reads all text from the file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>The file contents as a string.</returns>
        public static string ReadAllText(this FileInfo file, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllTextFromFile(file.FullName);
        }

        /// <summary>
        /// Reads all text from the file using the specified encoding.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>The file contents as a string.</returns>
        public static string ReadAllText(this FileInfo file, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            return File.ReadAllText(file.FullName, encoding);
        }

        /// <summary>
        /// Reads all text from the file using the specified encoding and file system abstraction.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>The file contents as a string.</returns>
        public static string ReadAllText(this FileInfo file, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.ReadAllTextFromFile(file.FullName, encoding);
        }

        /// <summary>
        /// Writes the specified bytes to the file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="bytes">The bytes to write.</param>
        public static void WriteAllBytes(this FileInfo file, byte[] bytes)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllBytes(file.FullName, bytes);
        }

        /// <summary>
        /// Writes the specified bytes to the file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="bytes">The bytes to write.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void WriteAllBytes(this FileInfo file, byte[] bytes, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllBytesToFile(file.FullName, bytes);
        }

        /// <summary>
        /// Writes the specified lines to the file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="lines">The lines to write.</param>
        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllLines(file.FullName, lines);
        }

        /// <summary>
        /// Writes the specified lines to the file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="lines">The lines to write.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllLinesToFile(file.FullName, lines);
        }

        /// <summary>
        /// Writes the specified lines to the file using the specified encoding.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="lines">The lines to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllLines(file.FullName, lines, encoding);
        }

        /// <summary>
        /// Writes the specified lines to the file using the specified encoding and file system abstraction.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="lines">The lines to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void WriteAllLines(this FileInfo file, IEnumerable<string> lines, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllLinesToFile(file.FullName, lines, encoding);
        }

        /// <summary>
        /// Writes the specified lines to the file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="lines">The lines to write.</param>
        public static void WriteAllLines(this FileInfo file, string[] lines)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllLines(file.FullName, lines);
        }

        /// <summary>
        /// Writes the specified lines to the file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="lines">The lines to write.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void WriteAllLines(this FileInfo file, string[] lines, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllLinesToFile(file.FullName, lines);
        }

        /// <summary>
        /// Writes the specified lines to the file using the specified encoding.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="lines">The lines to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        public static void WriteAllLines(this FileInfo file, string[] lines, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllLines(file.FullName, lines, encoding);
        }

        /// <summary>
        /// Writes the specified lines to the file using the specified encoding and file system abstraction.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="lines">The lines to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void WriteAllLines(this FileInfo file, string[] lines, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllLinesToFile(file.FullName, lines, encoding);
        }

        /// <summary>
        /// Writes the specified text to the file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="contents">The text to write.</param>
        public static void WriteAllText(this FileInfo file, string contents)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllText(file.FullName, contents);
        }

        /// <summary>
        /// Writes the specified text to the file using the provided file system abstraction.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="contents">The text to write.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void WriteAllText(this FileInfo file, string contents, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllTextToFile(file.FullName, contents);
        }

        /// <summary>
        /// Writes the specified text to the file using the specified encoding.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="contents">The text to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        public static void WriteAllText(this FileInfo file, string contents, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            File.WriteAllText(file.FullName, contents, encoding);
        }

        /// <summary>
        /// Writes the specified text to the file using the specified encoding and file system abstraction.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="contents">The text to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public static void WriteAllText(this FileInfo file, string contents, Encoding encoding, IFileSystem fileSystem)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.WriteAllTextToFile(file.FullName, contents, encoding);
        }
    }
}
