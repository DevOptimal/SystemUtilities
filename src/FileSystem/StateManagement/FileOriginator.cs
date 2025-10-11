// Implements the originator for a file, supporting state capture and restoration with file caching.
using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching;
using System;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    /// <summary>
    /// Originator for a file resource, supporting state capture and restoration using the Memento pattern.
    /// Handles file content persistence via a file cache.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="fileCache">The file cache for storing file contents.</param>
    /// <param name="fileSystem">The file system abstraction.</param>
    internal class FileOriginator : IOriginator<FileMemento>
    {
        /// <summary>
        /// Gets the full path of the file.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the file cache for storing file contents.
        /// </summary>
        public IFileCache FileCache { get; }

        /// <summary>
        /// Gets the file system abstraction.
        /// </summary>
        public IFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the unique identifier for this file originator, normalized by platform.
        /// </summary>
        public string ID
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Path;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Path.ToLower();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Path;
                }
                else
                {
                    throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
                }
            }
        }

        public FileOriginator(string path, IFileCache fileCache, IFileSystem fileSystem)
        {
            Path = System.IO.Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));
            FileCache = fileCache ?? throw new ArgumentNullException(nameof(fileCache));
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        /// <summary>
        /// Captures the current state of the file as a memento, storing its content hash if it exists.
        /// </summary>
        /// <returns>A memento representing the file's content hash or null if the file does not exist.</returns>
        public FileMemento GetState()
        {
            if (!FileSystem.FileExists(Path))
            {
                return new FileMemento
                {
                    Hash = null
                };
            }

            return new FileMemento
            {
                Hash = FileCache.UploadFile(Path)
            };
        }

        /// <summary>
        /// Restores the file's state from the provided memento, downloading or deleting as needed.
        /// </summary>
        /// <param name="memento">The memento to restore state from.</param>
        public void SetState(FileMemento memento)
        {
            var directoryPath = System.IO.Path.GetDirectoryName(Path);
            if (!FileSystem.DirectoryExists(directoryPath))
            {
                FileSystem.CreateDirectory(directoryPath);
            }

            if (memento.Hash == null)
            {
                if (FileSystem.FileExists(Path))
                {
                    FileSystem.DeleteFile(Path);
                }
            }
            else
            {
                FileCache.DownloadFile(memento.Hash, Path);
            }
        }
    }
}
