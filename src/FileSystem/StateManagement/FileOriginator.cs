using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching;
using System;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    internal class FileOriginator(string path, IFileCache fileCache, IFileSystem fileSystem) : IOriginator<FileMemento>
    {
        public string Path { get; } = System.IO.Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));

        /// <summary>
        /// Files can be big, so their contents cannot be stored in memory. Instead, persist the content to a blob
        /// store, indexed by its hash. The hash will be stored in the FileMemento.
        /// </summary>
        public IFileCache FileCache { get; } = fileCache ?? throw new ArgumentNullException(nameof(fileCache));

        public IFileSystem FileSystem { get; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

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
