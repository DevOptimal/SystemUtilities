// Implements a snapshotter for file system resources, supporting directory and file snapshots.
using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.Extensions;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Serialization;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    /// <summary>
    /// Manages transactional access and snapshots for file system resources (directories and files).
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="fileCache">The file cache for storing file contents.</param>
    /// <param name="persistenceDirectory">The directory for persisting caretaker data.</param>
    public class FileSystemSnapshotter(IFileSystem fileSystem, IFileCache fileCache, DirectoryInfo persistenceDirectory)
        : Snapshotter(new FileSystemCaretakerSerializer(fileSystem, fileCache), persistenceDirectory)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemSnapshotter"/> class with the default file system.
        /// </summary>
        public FileSystemSnapshotter()
            : this(new DefaultFileSystem())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemSnapshotter"/> class with the specified file system.
        /// </summary>
        /// <param name="fileSystem">The file system abstraction.</param>
        public FileSystemSnapshotter(IFileSystem fileSystem)
            : this(fileSystem, defaultPersistenceDirectory)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemSnapshotter"/> class with the specified file system and persistence directory.
        /// </summary>
        /// <param name="fileSystem">The file system abstraction.</param>
        /// <param name="persistenceDirectory">The directory for persisting caretaker data.</param>
        public FileSystemSnapshotter(IFileSystem fileSystem, DirectoryInfo persistenceDirectory)
            : this(fileSystem, new LocalFileCache(persistenceDirectory.GetDirectory("FileCache"), fileSystem), persistenceDirectory)
        { }

        /// <summary>
        /// Creates a snapshot of a directory resource.
        /// </summary>
        /// <param name="path">The directory path to snapshot.</param>
        /// <returns>An <see cref="ISnapshot"/> representing the directory's state.</returns>
        public ISnapshot SnapshotDirectory(string path)
        {
            var originator = new DirectoryOriginator(path, fileSystem);
            var snapshot = new DirectoryCaretaker(originator, this);
            return snapshot;
        }

        /// <summary>
        /// Creates a snapshot of a file resource.
        /// </summary>
        /// <param name="path">The file path to snapshot.</param>
        /// <returns>An <see cref="ISnapshot"/> representing the file's state.</returns>
        public ISnapshot SnapshotFile(string path)
        {
            var originator = new FileOriginator(path, fileCache, fileSystem);
            var snapshot = new FileCaretaker(originator, this);
            return snapshot;
        }
    }
}
