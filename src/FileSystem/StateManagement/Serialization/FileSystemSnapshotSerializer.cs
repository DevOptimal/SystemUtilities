using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement.Serialization
{
    /// <summary>
    /// Goal of this serializer is efficient resource usage by streaming snapshots instead of loading all of them into memory at once.
    /// To accomplish this, we parse and yield return snapshots as we read them from the JSON stream.
    /// </summary>
    internal class FileSystemSnapshotSerializer(IFileSystem fileSystem, IFileCache fileCache) : SnapshotSerializer
    {
        private const string directoryResourceTypeName = "Directory";
        private const string fileResourceTypeName = "File";

        protected override ISnapshot ConvertDictionaryToSnapshot(IDictionary<string, object> dictionary, Database database)
        {
            // Get snapshot fields
            var id = AsString(dictionary[nameof(ISnapshot.ID)]);
            var processId = AsInteger(dictionary[nameof(ISnapshot.ProcessID)]);
            var processStartTime = AsDateTime(dictionary[nameof(ISnapshot.ProcessStartTime)]);

            switch (dictionary[resourceTypePropertyName])
            {
                case directoryResourceTypeName:
                    var directoryPath = AsString(dictionary[nameof(DirectoryOriginator.Path)]);
                    var directoryExists = AsBoolean(dictionary[nameof(DirectoryMemento.Exists)]);

                    var directoryOriginator = new DirectoryOriginator(directoryPath, fileSystem);
                    var directoryMemento = new DirectoryMemento
                    {
                        Exists = directoryExists
                    };

                    return new Caretaker<DirectoryOriginator, DirectoryMemento>(id, processId, processStartTime, database, directoryOriginator, directoryMemento);
                case fileResourceTypeName:
                    var filePath = AsString(dictionary[nameof(FileOriginator.Path)]);
                    var fileHash = AsString(dictionary[nameof(FileMemento.Hash)]);

                    var fileOriginator = new FileOriginator(filePath, fileCache, fileSystem);
                    var fileMemento = new FileMemento
                    {
                        Hash = fileHash
                    };

                    return new Caretaker<FileOriginator, FileMemento>(id, processId, processStartTime, database, fileOriginator, fileMemento);
                default: throw new Exception();
            }
        }

        protected override IDictionary<string, object> ConvertSnapshotToDictionary(ISnapshot snapshot)
        {
            var result = new Dictionary<string, object>
            {
                [nameof(ISnapshot.ID)] = snapshot.ID,
                [nameof(ISnapshot.ProcessID)] = snapshot.ProcessID,
                [nameof(ISnapshot.ProcessStartTime)] = snapshot.ProcessStartTime.Ticks
            };

            switch (snapshot)
            {
                case Caretaker<DirectoryOriginator, DirectoryMemento> directoryCaretaker:
                    result[resourceTypePropertyName] = directoryResourceTypeName;
                    result[nameof(DirectoryOriginator.Path)] = directoryCaretaker.Originator.Path;
                    result[nameof(DirectoryMemento.Exists)] = directoryCaretaker.Memento.Exists;
                    break;
                case Caretaker<FileOriginator, FileMemento> fileCaretaker:
                    result[resourceTypePropertyName] = fileResourceTypeName;
                    result[nameof(FileOriginator.Path)] = fileCaretaker.Originator.Path;
                    result[nameof(FileMemento.Hash)] = fileCaretaker.Memento.Hash;
                    break;
                default: throw new Exception();
            }

            return result;
        }
    }
}
