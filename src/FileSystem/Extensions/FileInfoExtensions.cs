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
        extension(FileInfo file)
        {
            /// <summary>
            /// Copies the file to the specified destination file.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            public void CopyTo(FileInfo destFile)
            {
                CopyTo(file, destFile, false, new DefaultFileSystem());
            }

            /// <summary>
            /// Copies the file to the specified destination file, optionally overwriting.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            public void CopyTo(FileInfo destFile, bool overwrite)
            {
                CopyTo(file, destFile, overwrite, new DefaultFileSystem());
            }

            /// <summary>
            /// Copies the file to the specified destination file using the provided file system abstraction.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void CopyTo(FileInfo destFile, IFileSystem fileSystem)
            {
                CopyTo(file, destFile, false, fileSystem);
            }

            /// <summary>
            /// Copies the file to the specified destination file using the provided file system abstraction, optionally overwriting.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void CopyTo(FileInfo destFile, bool overwrite, IFileSystem fileSystem)
            {
                if (destFile == null) throw new ArgumentNullException(nameof(destFile));

                CopyTo(file, destFile.FullName, overwrite, fileSystem);
            }

            /// <summary>
            /// Copies the file to the specified destination file name using the provided file system abstraction.
            /// </summary>
            /// <param name="destFileName">The destination file name.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void CopyTo(string destFileName, IFileSystem fileSystem)
            {
                CopyTo(file, destFileName, false, fileSystem);
            }

            /// <summary>
            /// Copies the file to the specified destination file name using the provided file system abstraction, optionally overwriting.
            /// </summary>
            /// <param name="destFileName">The destination file name.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void CopyTo(string destFileName, bool overwrite, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.CopyFile(file.FullName, destFileName, overwrite);
            }

            /// <summary>
            /// Determines whether two <see cref="FileInfo"/> instances are equal using the specified comparer.
            /// </summary>
            /// <param name="other">The file to compare.</param>
            /// <param name="comparer">The equality comparer to use.</param>
            public bool Equals(FileInfo other, IEqualityComparer<FileInfo> comparer)
            {
                if (comparer == null) throw new ArgumentNullException(nameof(comparer));

                return comparer.Equals(file, other);
            }

            /// <summary>
            /// Determines whether the file exists using the provided file system abstraction.
            /// </summary>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public bool Exists(IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.FileExists(file.FullName);
            }

            /// <summary>
            /// Gets the drive information for the file.
            /// </summary>
            /// <returns>The <see cref="DriveInfo"/> for the file.</returns>
            public DriveInfo Drive
            {
                get
                {
                    if (file == null) throw new ArgumentNullException(nameof(file));

                    return new DriveInfo(Path.GetPathRoot(file.FullName));
                }
            }

            /// <summary>
            /// Creates a hard link to the specified destination file.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            public void HardLinkTo(FileInfo destFile)
            {
                HardLinkTo(file, destFile, false, new DefaultFileSystem());
            }

            /// <summary>
            /// Creates a hard link to the specified destination file, optionally overwriting.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            public void HardLinkTo(FileInfo destFile, bool overwrite)
            {
                HardLinkTo(file, destFile, overwrite, new DefaultFileSystem());
            }

            /// <summary>
            /// Creates a hard link to the specified destination file using the provided file system abstraction.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void HardLinkTo(FileInfo destFile, IFileSystem fileSystem)
            {
                HardLinkTo(file, destFile, false, fileSystem);
            }

            /// <summary>
            /// Creates a hard link to the specified destination file using the provided file system abstraction, optionally overwriting.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void HardLinkTo(FileInfo destFile, bool overwrite, IFileSystem fileSystem)
            {
                if (destFile == null) throw new ArgumentNullException(nameof(destFile));

                HardLinkTo(file, destFile.FullName, overwrite, fileSystem);
            }

            /// <summary>
            /// Creates a hard link to the specified destination file name.
            /// </summary>
            /// <param name="destFileName">The destination file name.</param>
            public void HardLinkTo(string destFileName)
            {
                HardLinkTo(file, destFileName, false, new DefaultFileSystem());
            }

            /// <summary>
            /// Creates a hard link to the specified destination file name, optionally overwriting.
            /// </summary>
            /// <param name="destFileName">The destination file name.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            public void HardLinkTo(string destFileName, bool overwrite)
            {
                HardLinkTo(file, destFileName, overwrite, new DefaultFileSystem());
            }

            /// <summary>
            /// Creates a hard link to the specified destination file name using the provided file system abstraction.
            /// </summary>
            /// <param name="destFileName">The destination file name.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void HardLinkTo(string destFileName, IFileSystem fileSystem)
            {
                HardLinkTo(file, destFileName, false, fileSystem);
            }

            /// <summary>
            /// Creates a hard link to the specified destination file name using the provided file system abstraction, optionally overwriting.
            /// </summary>
            /// <param name="destFileName">The destination file name.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void HardLinkTo(string destFileName, bool overwrite, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.HardLinkFile(file.FullName, destFileName, overwrite);
            }

            /// <summary>
            /// Determines whether a file is a descendant of a directory.
            /// </summary>
            /// <param name="ancestor">The ancestor directory to test.</param>
            /// <returns>True if <paramref name="file"/> is a descendant of <paramref name="ancestor"/>, false otherwise.</returns>
            public bool IsDescendantOf(DirectoryInfo ancestor)
            {
                var comparer = new DirectoryInfoComparer();
                var directory = file.Directory;
                return comparer.Equals(directory, ancestor) || directory.IsDescendantOf(ancestor, comparer);
            }

            /// <summary>
            /// Moves the file to the specified destination file.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            public void MoveTo(FileInfo destFile)
            {
                MoveTo(file, destFile, false, new DefaultFileSystem());
            }

            /// <summary>
            /// Moves the file to the specified destination file, optionally overwriting.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            public void MoveTo(FileInfo destFile, bool overwrite)
            {
                MoveTo(file, destFile, overwrite, new DefaultFileSystem());
            }

            /// <summary>
            /// Moves the file to the specified destination file using the provided file system abstraction.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void MoveTo(FileInfo destFile, IFileSystem fileSystem)
            {
                MoveTo(file, destFile, false, fileSystem);
            }

            /// <summary>
            /// Moves the file to the specified destination file using the provided file system abstraction, optionally overwriting.
            /// </summary>
            /// <param name="destFile">The destination file.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void MoveTo(FileInfo destFile, bool overwrite, IFileSystem fileSystem)
            {
                if (destFile == null) throw new ArgumentNullException(nameof(destFile));

                MoveTo(file, destFile.FullName, overwrite, fileSystem);
            }

            /// <summary>
            /// Moves the file to the specified destination file name, optionally overwriting.
            /// </summary>
            /// <param name="destFileName">The destination file name.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            public void MoveTo(string destFileName, bool overwrite)
            {
                MoveTo(file, destFileName, overwrite, new DefaultFileSystem());
            }

            /// <summary>
            /// Moves the file to the specified destination file name using the provided file system abstraction.
            /// </summary>
            /// <param name="destFileName">The destination file name.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void MoveTo(string destFileName, IFileSystem fileSystem)
            {
                MoveTo(file, destFileName, false, fileSystem);
            }

            /// <summary>
            /// Moves the file to the specified destination file name using the provided file system abstraction, optionally overwriting.
            /// </summary>
            /// <param name="destFileName">The destination file name.</param>
            /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void MoveTo(string destFileName, bool overwrite, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.MoveFile(file.FullName, destFileName, overwrite);
            }

            /// <summary>
            /// Opens the file with the specified mode using the provided file system abstraction.
            /// </summary>
            /// <param name="mode">The file mode to use.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
            public FileStream Open(FileMode mode, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.OpenFile(file.FullName, mode);
            }

            /// <summary>
            /// Opens the file with the specified mode and access using the provided file system abstraction.
            /// </summary>
            /// <param name="mode">The file mode to use.</param>
            /// <param name="access">The file access to use.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
            public FileStream Open(FileMode mode, FileAccess access, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.OpenFile(file.FullName, mode, access);
            }

            /// <summary>
            /// Opens the file with the specified mode, access, and share using the provided file system abstraction.
            /// </summary>
            /// <param name="mode">The file mode to use.</param>
            /// <param name="access">The file access to use.</param>
            /// <param name="share">The file sharing mode to use.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
            public FileStream Open(FileMode mode, FileAccess access, FileShare share, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.OpenFile(file.FullName, mode, access, share);
            }

            /// <summary>
            /// Opens the file for reading using the provided file system abstraction.
            /// </summary>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
            public FileStream OpenRead(IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.OpenFileRead(file.FullName);
            }

            /// <summary>
            /// Opens the file for reading text using the provided file system abstraction.
            /// </summary>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>A <see cref="StreamReader"/> for the opened file.</returns>
            public StreamReader OpenText(IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.OpenFileText(file.FullName);
            }

            /// <summary>
            /// Opens the file for writing using the provided file system abstraction.
            /// </summary>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>A <see cref="FileStream"/> for the opened file.</returns>
            public FileStream OpenWrite(IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.OpenFileWrite(file.FullName);
            }

            /// <summary>
            /// Reads all bytes from the file.
            /// </summary>
            /// <returns>The file contents as a byte array.</returns>
            public byte[] ReadAllBytes()
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                return File.ReadAllBytes(file.FullName);
            }

            /// <summary>
            /// Reads all bytes from the file using the provided file system abstraction.
            /// </summary>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>The file contents as a byte array.</returns>
            public byte[] ReadAllBytes(IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.ReadAllBytesFromFile(file.FullName);
            }

            /// <summary>
            /// Reads all lines from the file.
            /// </summary>
            /// <returns>The file contents as an array of strings.</returns>
            public string[] ReadAllLines()
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                return File.ReadAllLines(file.FullName);
            }

            /// <summary>
            /// Reads all lines from the file using the provided file system abstraction.
            /// </summary>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>The file contents as an array of strings.</returns>
            public string[] ReadAllLines(IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.ReadAllLinesFromFile(file.FullName);
            }

            /// <summary>
            /// Reads all lines from the file using the specified encoding.
            /// </summary>
            /// <param name="encoding">The encoding to use.</param>
            /// <returns>The file contents as an array of strings.</returns>
            public string[] ReadAllLines(Encoding encoding)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                return File.ReadAllLines(file.FullName, encoding);
            }

            /// <summary>
            /// Reads all lines from the file using the specified encoding and file system abstraction.
            /// </summary>
            /// <param name="encoding">The encoding to use.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>The file contents as an array of strings.</returns>
            public string[] ReadAllLines(Encoding encoding, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.ReadAllLinesFromFile(file.FullName, encoding);
            }

            /// <summary>
            /// Reads all text from the file.
            /// </summary>
            /// <returns>The file contents as a string.</returns>
            public string ReadAllText()
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                return File.ReadAllText(file.FullName);
            }

            /// <summary>
            /// Reads all text from the file using the provided file system abstraction.
            /// </summary>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>The file contents as a string.</returns>
            public string ReadAllText(IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.ReadAllTextFromFile(file.FullName);
            }

            /// <summary>
            /// Reads all text from the file using the specified encoding.
            /// </summary>
            /// <param name="encoding">The encoding to use.</param>
            /// <returns>The file contents as a string.</returns>
            public string ReadAllText(Encoding encoding)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                return File.ReadAllText(file.FullName, encoding);
            }

            /// <summary>
            /// Reads all text from the file using the specified encoding and file system abstraction.
            /// </summary>
            /// <param name="encoding">The encoding to use.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            /// <returns>The file contents as a string.</returns>
            public string ReadAllText(Encoding encoding, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.ReadAllTextFromFile(file.FullName, encoding);
            }

            /// <summary>
            /// Writes the specified bytes to the file.
            /// </summary>
            /// <param name="bytes">The bytes to write.</param>
            public void WriteAllBytes(byte[] bytes)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                File.WriteAllBytes(file.FullName, bytes);
            }

            /// <summary>
            /// Writes the specified bytes to the file using the provided file system abstraction.
            /// </summary>
            /// <param name="bytes">The bytes to write.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void WriteAllBytes(byte[] bytes, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.WriteAllBytesToFile(file.FullName, bytes);
            }

            /// <summary>
            /// Writes the specified lines to the file.
            /// </summary>
            /// <param name="lines">The lines to write.</param>
            public void WriteAllLines(IEnumerable<string> lines)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                File.WriteAllLines(file.FullName, lines);
            }

            /// <summary>
            /// Writes the specified lines to the file using the provided file system abstraction.
            /// </summary>
            /// <param name="lines">The lines to write.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void WriteAllLines(IEnumerable<string> lines, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.WriteAllLinesToFile(file.FullName, lines);
            }

            /// <summary>
            /// Writes the specified lines to the file using the specified encoding.
            /// </summary>
            /// <param name="lines">The lines to write.</param>
            /// <param name="encoding">The encoding to use.</param>
            public void WriteAllLines(IEnumerable<string> lines, Encoding encoding)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                File.WriteAllLines(file.FullName, lines, encoding);
            }

            /// <summary>
            /// Writes the specified lines to the file using the specified encoding and file system abstraction.
            /// </summary>
            /// <param name="lines">The lines to write.</param>
            /// <param name="encoding">The encoding to use.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void WriteAllLines(IEnumerable<string> lines, Encoding encoding, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.WriteAllLinesToFile(file.FullName, lines, encoding);
            }

            /// <summary>
            /// Writes the specified lines to the file.
            /// </summary>
            /// <param name="lines">The lines to write.</param>
            public void WriteAllLines(string[] lines)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                File.WriteAllLines(file.FullName, lines);
            }

            /// <summary>
            /// Writes the specified lines to the file using the provided file system abstraction.
            /// </summary>
            /// <param name="lines">The lines to write.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void WriteAllLines(string[] lines, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.WriteAllLinesToFile(file.FullName, lines);
            }

            /// <summary>
            /// Writes the specified lines to the file using the specified encoding.
            /// </summary>
            /// <param name="lines">The lines to write.</param>
            /// <param name="encoding">The encoding to use.</param>
            public void WriteAllLines(string[] lines, Encoding encoding)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                File.WriteAllLines(file.FullName, lines, encoding);
            }

            /// <summary>
            /// Writes the specified lines to the file using the specified encoding and file system abstraction.
            /// </summary>
            /// <param name="lines">The lines to write.</param>
            /// <param name="encoding">The encoding to use.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void WriteAllLines(string[] lines, Encoding encoding, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.WriteAllLinesToFile(file.FullName, lines, encoding);
            }

            /// <summary>
            /// Writes the specified text to the file.
            /// </summary>
            /// <param name="contents">The text to write.</param>
            public void WriteAllText(string contents)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                File.WriteAllText(file.FullName, contents);
            }

            /// <summary>
            /// Writes the specified text to the file using the provided file system abstraction.
            /// </summary>
            /// <param name="contents">The text to write.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void WriteAllText(string contents, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.WriteAllTextToFile(file.FullName, contents);
            }

            /// <summary>
            /// Writes the specified text to the file using the specified encoding.
            /// </summary>
            /// <param name="contents">The text to write.</param>
            /// <param name="encoding">The encoding to use.</param>
            public void WriteAllText(string contents, Encoding encoding)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));

                File.WriteAllText(file.FullName, contents, encoding);
            }

            /// <summary>
            /// Writes the specified text to the file using the specified encoding and file system abstraction.
            /// </summary>
            /// <param name="contents">The text to write.</param>
            /// <param name="encoding">The encoding to use.</param>
            /// <param name="fileSystem">The file system abstraction to use.</param>
            public void WriteAllText(string contents, Encoding encoding, IFileSystem fileSystem)
            {
                if (file == null) throw new ArgumentNullException(nameof(file));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                fileSystem.WriteAllTextToFile(file.FullName, contents, encoding);
            }
        }
    }
}
