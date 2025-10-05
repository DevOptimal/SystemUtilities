// Implements a zip file cache for storing and retrieving file contents in a zip archive.
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.IO;
using System.IO.Compression;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching
{
    /// <summary>
    /// Provides a zip file cache for storing and retrieving file contents in a zip archive.
    /// </summary>
    /// <param name="zipFile">The zip file used as the cache.</param>
    /// <param name="fileSystem">The file system abstraction.</param>
    public class ZipFileCache(FileInfo zipFile, IFileSystem fileSystem) : IFileCache
    {
        /// <summary>
        /// Gets or sets the file system abstraction.
        /// </summary>
        public IFileSystem FileSystem { get; set; } = fileSystem;

        private readonly FileInfo zipFile = zipFile;

        /// <summary>
        /// Downloads a file from the zip cache to the specified destination path.
        /// </summary>
        /// <param name="id">The ID of the file in the zip archive.</param>
        /// <param name="destinationPath">The destination path to save the file.</param>
        public void DownloadFile(string id, string destinationPath)
        {
            using var zipStream = zipFile.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
            using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            var zipEntry = zipArchive.GetEntry(id);
            using var entryStream = zipEntry.Open();
            using var fileStream = File.Open(destinationPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            entryStream.CopyTo(fileStream);
        }

        /// <summary>
        /// Uploads a file to the zip cache and returns its unique identifier.
        /// </summary>
        /// <param name="sourcePath">The source file to upload.</param>
        /// <returns>The unique identifier for the cached file.</returns>
        public string UploadFile(string sourcePath)
        {
            var id = Guid.NewGuid().ToString();
            using var zipStream = zipFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update);
            var zipEntry = zipArchive.CreateEntry(id);
            using var fileStream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.None);
            using var entryStream = zipEntry.Open();
            fileStream.CopyTo(entryStream);
            return id;
        }
    }
}