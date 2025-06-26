using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="IFileSystem"/> interface to simplify common file and directory operations.
    /// </summary>
    public static class IFileSystemExtensions
    {
        /// <summary>
        /// Copies a file from the source path to the destination path without overwriting existing files.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="sourcePath">The file to copy.</param>
        /// <param name="destinationPath">The location to copy the file to.</param>
        public static void CopyFile(this IFileSystem fileSystem, string sourcePath, string destinationPath)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.CopyFile(sourcePath, destinationPath, overwrite: false);
        }

        /// <summary>
        /// Deletes the specified directory without deleting subdirectories or files recursively.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The directory path to delete.</param>
        public static void DeleteDirectory(this IFileSystem fileSystem, string path)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.DeleteDirectory(path, recursive: false);
        }

        /// <summary>
        /// Gets the names of subdirectories in the specified directory.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The directory to search.</param>
        /// <param name="recursive">Whether to search all subdirectories.</param>
        /// <returns>An array of subdirectory names.</returns>
        public static string[] GetDirectories(this IFileSystem fileSystem, string path, bool recursive)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.GetDirectories(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Gets the names of files in the specified directory.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The directory to search.</param>
        /// <param name="recursive">Whether to search all subdirectories.</param>
        /// <returns>An array of file names.</returns>
        public static string[] GetFiles(this IFileSystem fileSystem, string path, bool recursive)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.GetFiles(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Creates a hard link from the source file to the destination path without overwriting existing files.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="sourcePath">The existing file to link from.</param>
        /// <param name="destinationPath">The new hard link path.</param>
        public static void HardLinkFile(this IFileSystem fileSystem, string sourcePath, string destinationPath)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.HardLinkFile(sourcePath, destinationPath, overwrite: false);
        }

        /// <summary>
        /// Moves a file from the source path to the destination path without overwriting existing files.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="sourcePath">The file to move.</param>
        /// <param name="destinationPath">The location to move the file to.</param>
        public static void MoveFile(this IFileSystem fileSystem, string sourcePath, string destinationPath)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.MoveFile(sourcePath, destinationPath, overwrite: false);
        }

        /// <summary>
        /// Opens a file with read/write access and no sharing.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to open.</param>
        /// <param name="mode">The file mode to use.</param>
        /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
        public static FileStream OpenFile(this IFileSystem fileSystem, string path, FileMode mode)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(path, mode, FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>
        /// Opens a file with the specified access and no sharing.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to open.</param>
        /// <param name="mode">The file mode to use.</param>
        /// <param name="access">The file access to use.</param>
        /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
        public static FileStream OpenFile(this IFileSystem fileSystem, string path, FileMode mode, FileAccess access)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(path, mode, access, FileShare.None);
        }

        /// <summary>
        /// Opens a file for reading, creating it if it does not exist.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to open.</param>
        /// <returns>A <see cref="FileStream"/> for reading the file.</returns>
        public static FileStream OpenFileRead(this IFileSystem fileSystem, string path)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Opens a file for reading text, creating it if it does not exist.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to open.</param>
        /// <returns>A <see cref="StreamReader"/> for reading the file.</returns>
        public static StreamReader OpenFileText(this IFileSystem fileSystem, string path)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return new StreamReader(fileSystem.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read));
        }

        /// <summary>
        /// Opens a file for writing, creating it if it does not exist.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to open.</param>
        /// <returns>A <see cref="FileStream"/> for writing to the file.</returns>
        public static FileStream OpenFileWrite(this IFileSystem fileSystem, string path)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            return fileSystem.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        /// <summary>
        /// Reads all bytes from the specified file.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to read.</param>
        /// <returns>A byte array containing the file contents.</returns>
        public static byte[] ReadAllBytesFromFile(this IFileSystem fileSystem, string path)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = new byte[fileStream.Length];
            fileStream.Read(result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Reads all lines from the specified file using the default encoding.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to read.</param>
        /// <returns>An array of lines from the file.</returns>
        public static string[] ReadAllLinesFromFile(this IFileSystem fileSystem, string path)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(fileStream);
            var lines = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
            return [.. lines];
        }

        /// <summary>
        /// Reads all lines from the specified file using the given encoding.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to read.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>An array of lines from the file.</returns>
        public static string[] ReadAllLinesFromFile(this IFileSystem fileSystem, string path, Encoding encoding)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(fileStream, encoding);
            var lines = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
            return [.. lines];
        }

        /// <summary>
        /// Reads all text from the specified file using the default encoding.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to read.</param>
        /// <returns>The file contents as a string.</returns>
        public static string ReadAllTextFromFile(this IFileSystem fileSystem, string path)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(fileStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Reads all text from the specified file using the given encoding.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to read.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>The file contents as a string.</returns>
        public static string ReadAllTextFromFile(this IFileSystem fileSystem, string path, Encoding encoding)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var fileStream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(fileStream, encoding);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Writes the specified bytes to a file, overwriting if it exists.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="bytes">The byte array to write.</param>
        public static void WriteAllBytesToFile(this IFileSystem fileSystem, string path, byte[] bytes)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var stream = fileSystem.OpenFileWrite(path);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes the specified lines to a file using the default encoding, overwriting if it exists.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="lines">The lines to write.</param>
        public static void WriteAllLinesToFile(this IFileSystem fileSystem, string path, IEnumerable<string> lines)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var stream = fileSystem.OpenFileWrite(path);
            using var writer = new StreamWriter(stream);
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        /// <summary>
        /// Writes the specified lines to a file using the given encoding, overwriting if it exists.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="lines">The lines to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        public static void WriteAllLinesToFile(this IFileSystem fileSystem, string path, IEnumerable<string> lines, Encoding encoding)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var stream = fileSystem.OpenFileWrite(path);
            using var writer = new StreamWriter(stream, encoding);
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        /// <summary>
        /// Writes the specified lines to a file using the default encoding, overwriting if it exists.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="lines">The lines to write.</param>
        public static void WriteAllLinesToFile(this IFileSystem fileSystem, string path, string[] lines)
        {
            WriteAllLinesToFile(fileSystem, path, (IEnumerable<string>)lines);
        }

        /// <summary>
        /// Writes the specified lines to a file using the given encoding, overwriting if it exists.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="lines">The lines to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        public static void WriteAllLinesToFile(this IFileSystem fileSystem, string path, string[] lines, Encoding encoding)
        {
            WriteAllLinesToFile(fileSystem, path, (IEnumerable<string>)lines, encoding);
        }

        /// <summary>
        /// Writes the specified text to a file using the default encoding, overwriting if it exists.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="contents">The text to write.</param>
        public static void WriteAllTextToFile(this IFileSystem fileSystem, string path, string contents)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var stream = fileSystem.OpenFileWrite(path);
            using var writer = new StreamWriter(stream);
            writer.Write(contents);
        }

        /// <summary>
        /// Writes the specified text to a file using the given encoding, overwriting if it exists.
        /// </summary>
        /// <param name="fileSystem">The file system instance.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="contents">The text to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        public static void WriteAllTextToFile(this IFileSystem fileSystem, string path, string contents, Encoding encoding)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

            using var stream = fileSystem.OpenFileWrite(path);
            using var writer = new StreamWriter(stream, encoding);
            writer.Write(contents);
        }
    }
}
