using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Abstractions
{
    /// <summary>
    /// Provides a default implementation of the <see cref="IFileSystem"/> interface,
    /// using the standard <see cref="System.IO"/> APIs for file and directory operations.
    /// </summary>
    public class DefaultFileSystem : IFileSystem
    {
        /// <inheritdoc />
        public void CopyFile(string sourcePath, string destinationPath, bool overwrite)
        {
            File.Copy(sourcePath, destinationPath, overwrite);
        }

        /// <inheritdoc />
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc />
        public void CreateFile(string path)
        {
            File.Create(path);
        }

        /// <inheritdoc />
        public void DeleteDirectory(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        /// <inheritdoc />
        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        /// <inheritdoc />
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc />
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <inheritdoc />
        public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetDirectories(path, searchPattern, searchOption);
        }

        /// <inheritdoc />
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }

        /// <inheritdoc />
        public void HardLinkFile(string sourcePath, string destinationPath, bool overwrite)
        {
            FileUtilities.HardLink(sourcePath, destinationPath, overwrite);
        }

        /// <inheritdoc />
        public void MoveFile(string sourcePath, string destinationPath, bool overwrite)
        {
            FileUtilities.Move(sourcePath, destinationPath, overwrite);
        }

        /// <inheritdoc />
        public FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return File.Open(path, mode, access, share);
        }
    }
}
