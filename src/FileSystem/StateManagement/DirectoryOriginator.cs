// Implements the originator for a directory, supporting state capture and restoration.
using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    /// <summary>
    /// Originator for a directory resource, supporting state capture and restoration using the Memento pattern.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <param name="fileSystem">The file system abstraction.</param>
    internal class DirectoryOriginator : IOriginator<DirectoryMemento>
    {
        /// <summary>
        /// Gets the full path of the directory.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the file system abstraction.
        /// </summary>
        public IFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the unique identifier for this directory originator, normalized by platform.
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

        public DirectoryOriginator(string path, IFileSystem fileSystem)
        {
            Path = System.IO.Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        /// <summary>
        /// Captures the current state of the directory as a memento.
        /// </summary>
        /// <returns>A memento representing the directory's existence.</returns>
        public DirectoryMemento GetState()
        {
            return new DirectoryMemento
            {
                Exists = FileSystem.DirectoryExists(Path)
            };
        }

        /// <summary>
        /// Restores the directory's state from the provided memento.
        /// </summary>
        /// <param name="memento">The memento to restore state from.</param>
        public void SetState(DirectoryMemento memento)
        {
            if (FileSystem.DirectoryExists(Path))
            {
                if (!memento.Exists)
                {
                    FileSystem.DeleteDirectory(Path, recursive: true);
                }
            }
            else // Path does not exist
            {
                if (memento.Exists)
                {
                    FileSystem.CreateDirectory(Path);
                }
            }
        }
    }
}
