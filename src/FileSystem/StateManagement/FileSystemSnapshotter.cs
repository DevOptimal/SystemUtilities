using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.Extensions;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Serialization;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    public class FileSystemSnapshotter(IFileSystem fileSystem, IFileCache fileCache, DirectoryInfo persistenceDirectory)
        : Snapshotter(new FileSystemCaretakerSerializer(fileSystem, fileCache), persistenceDirectory)
    {
        public FileSystemSnapshotter()
            : this(new DefaultFileSystem())
        { }

        public FileSystemSnapshotter(IFileSystem fileSystem)
            : this(fileSystem, defaultPersistenceDirectory)
        { }

        public FileSystemSnapshotter(IFileSystem fileSystem, DirectoryInfo persistenceDirectory)
            : this(fileSystem, new LocalFileCache(persistenceDirectory.GetDirectory("FileCache"), fileSystem), persistenceDirectory)
        { }

        public ISnapshot SnapshotDirectory(string path)
        {
            var originator = new DirectoryOriginator(path, fileSystem);
            var snapshot = new DirectoryCaretaker(originator, connection);
            return snapshot;
        }

        public ISnapshot SnapshotFile(string path)
        {
            var originator = new FileOriginator(path, fileCache, fileSystem);
            var snapshot = new FileCaretaker(originator, connection);
            return snapshot;
        }
    }
}
