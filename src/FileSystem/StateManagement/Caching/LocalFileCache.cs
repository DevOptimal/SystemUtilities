// Implements a local file cache for storing and retrieving file contents on disk.
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching
{
    /// <summary>
    /// Provides a local file cache for storing and retrieving file contents on disk.
    /// </summary>
    public class LocalFileCache : IFileCache
    {
        /// <summary>
        /// Gets the root path of the file cache.
        /// </summary>
        public string RootPath { get; }

        /// <summary>
        /// Gets the file system abstraction.
        /// </summary>
        public IFileSystem FileSystem { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileCache"/> class with a root directory and file system.
        /// </summary>
        /// <param name="rootDirectory">The root directory for the file cache.</param>
        /// <param name="fileSystem">The file system abstraction.</param>
        public LocalFileCache(DirectoryInfo rootDirectory, IFileSystem fileSystem)
            : this(rootDirectory.FullName, fileSystem)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileCache"/> class with a root directory path and file system.
        /// </summary>
        /// <param name="rootDirectoryPath">The root directory path for the file cache.</param>
        /// <param name="fileSystem">The file system abstraction.</param>
        public LocalFileCache(string rootDirectoryPath, IFileSystem fileSystem)
        {
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

            RootPath = Path.GetFullPath(rootDirectoryPath);
            if (!fileSystem.DirectoryExists(RootPath))
            {
                fileSystem.CreateDirectory(RootPath);
            }
        }

        /// <summary>
        /// Downloads a file from the cache to the specified destination path.
        /// </summary>
        /// <param name="id">The ID of the file in the cache.</param>
        /// <param name="destinationPath">The destination path to save the file.</param>
        public void DownloadFile(string id, string destinationPath)
        {
            var blobPath = Path.Combine(RootPath, id);
            if (!FileSystem.FileExists(blobPath))
            {
                throw new FileNotFoundException("The file does not exist in the cache.", blobPath);
            }

            FileSystem.CopyFile(blobPath, destinationPath, overwrite: true);

            FileSystem.DeleteFile(blobPath);
        }

        /// <summary>
        /// Uploads a file to the cache and returns its unique identifier.
        /// </summary>
        /// <param name="sourcePath">The source file to upload.</param>
        /// <returns>The unique identifier for the cached file.</returns>
        public string UploadFile(string sourcePath)
        {
            if (!FileSystem.FileExists(sourcePath))
            {
                throw new FileNotFoundException("The source file does not exist.", sourcePath);
            }

            var id = Guid.NewGuid().ToString();

            var blobPath = Path.Combine(RootPath, id);

            FileSystem.CopyFile(sourcePath, blobPath, overwrite: false);

            return id;
        }
    }
}
