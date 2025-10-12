using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Abstractions
{
    /// <summary>
    /// Abstraction over basic file system operations used by the library. Implementations can wrap the real
    /// operating system (see <see cref="DefaultFileSystem"/>) or provide test doubles (see <see cref="MockFileSystem"/>).
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Copies a file from the specified source path to the destination path.
        /// </summary>
        /// <param name="sourcePath">The file to copy.</param>
        /// <param name="destinationPath">The location to copy the file to.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        void CopyFile(string sourcePath, string destinationPath, bool overwrite);

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="path">The directory path to create.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Creates an empty file at the specified path.
        /// </summary>
        /// <param name="path">The file path to create.</param>
        void CreateFile(string path);

        /// <summary>
        /// Deletes the directory at the specified path.
        /// </summary>
        /// <param name="path">The directory path to delete.</param>
        /// <param name="recursive">Whether to delete subdirectories and files recursively.</param>
        void DeleteDirectory(string path, bool recursive);

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="path">The file path to delete.</param>
        void DeleteFile(string path);

        /// <summary>
        /// Determines whether the specified directory exists.
        /// </summary>
        /// <param name="path">The directory path to check.</param>
        /// <returns>True if the directory exists; otherwise, false.</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file path to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Gets the names of subdirectories in the specified directory that match the search pattern.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <param name="searchPattern">The search string to match against subdirectory names.</param>
        /// <param name="searchOption">Specifies whether to search all subdirectories or only the top directory.</param>
        /// <returns>An array of subdirectory names that match the search pattern.</returns>
        string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);

        /// <summary>
        /// Gets the names of files in the specified directory that match the search pattern.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <param name="searchPattern">The search string to match against file names.</param>
        /// <param name="searchOption">Specifies whether to search all subdirectories or only the top directory.</param>
        /// <returns>An array of file names that match the search pattern.</returns>
        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);

        /// <summary>
        /// Creates a hard link from the source file to the destination path.
        /// </summary>
        /// <param name="sourcePath">The existing file to link from.</param>
        /// <param name="destinationPath">The new hard link path.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        void HardLinkFile(string sourcePath, string destinationPath, bool overwrite);

        /// <summary>
        /// Moves a file from the source path to the destination path.
        /// </summary>
        /// <param name="sourcePath">The file to move.</param>
        /// <param name="destinationPath">The location to move the file to.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it exists.</param>
        void MoveFile(string sourcePath, string destinationPath, bool overwrite);

        /// <summary>
        /// Opens a file with the specified mode, access, and sharing options.
        /// </summary>
        /// <param name="path">The file path to open.</param>
        /// <param name="mode">The file mode to use.</param>
        /// <param name="access">The file access to use.</param>
        /// <param name="share">The file sharing mode to use.</param>
        /// <returns>A FileStream for the opened file.</returns>
        FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);
    }
}
