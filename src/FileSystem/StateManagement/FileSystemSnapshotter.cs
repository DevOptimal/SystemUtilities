using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Serialization;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    public class FileSystemSnapshotter(IFileSystem fileSystem, IFileCache fileCache)
        : Snapshotter(new FileSystemSnapshotSerializer(fileSystem, fileCache))
    {
        public FileSystemSnapshotter()
            : this(new DefaultFileSystem())
        { }

        public FileSystemSnapshotter(IFileSystem fileSystem)
            : this(fileSystem, new LocalFileCache("", fileSystem))
        { }

        public ISnapshot SnapshotDirectory(string path)
        {
            var originator = new DirectoryOriginator(path, fileSystem);
            var snapshot = new Caretaker<DirectoryOriginator, DirectoryMemento>(originator, database);
            AddSnapshot(snapshot);
            return snapshot;
        }

        public ISnapshot SnapshotFile(string path)
        {
            var originator = new FileOriginator(path, fileCache, fileSystem);
            var snapshot = new Caretaker<FileOriginator, FileMemento>(originator, database);
            AddSnapshot(snapshot);
            return snapshot;
        }
    }
}
