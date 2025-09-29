using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching;
using System;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement.Serialization
{
    /// <summary>
    /// Goal of this serializer is efficient resource usage by streaming caretakers instead of loading all of them into memory at once.
    /// To accomplish this, we parse and yield return caretakers as we read them from the JSON stream.
    /// </summary>
    internal class FileSystemCaretakerSerializer(IFileSystem fileSystem, IFileCache fileCache) : CaretakerSerializer
    {
        private const string directoryResourceTypeName = "Directory";
        private const string fileResourceTypeName = "File";

        protected override ICaretaker ConvertDictionaryToCaretaker(IDictionary<string, object> dictionary, DatabaseConnection connection)
        {
            // Get caretaker fields
            var id = AsString(dictionary[nameof(ICaretaker.ID)]);
            var parentId = AsString(dictionary[nameof(ICaretaker.ParentID)]);
            var processId = AsInteger(dictionary[nameof(ICaretaker.ProcessID)]);
            var processStartTime = AsDateTime(dictionary[nameof(ICaretaker.ProcessStartTime)]);

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

                    return new Caretaker<DirectoryOriginator, DirectoryMemento>(id, parentId, processId, processStartTime, connection, directoryOriginator, directoryMemento);
                case fileResourceTypeName:
                    var filePath = AsString(dictionary[nameof(FileOriginator.Path)]);
                    var fileHash = AsString(dictionary[nameof(FileMemento.Hash)]);

                    var fileOriginator = new FileOriginator(filePath, fileCache, fileSystem);
                    var fileMemento = new FileMemento
                    {
                        Hash = fileHash
                    };

                    return new Caretaker<FileOriginator, FileMemento>(id, parentId, processId, processStartTime, connection, fileOriginator, fileMemento);
                default: throw new Exception();
            }
        }

        protected override IDictionary<string, object> ConvertCaretakerToDictionary(ICaretaker caretaker)
        {
            var result = new Dictionary<string, object>
            {
                [nameof(ICaretaker.ID)] = caretaker.ID,
                [nameof(ICaretaker.ParentID)] = caretaker.ParentID,
                [nameof(ICaretaker.ProcessID)] = caretaker.ProcessID,
                [nameof(ICaretaker.ProcessStartTime)] = caretaker.ProcessStartTime.Ticks
            };

            switch (caretaker)
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
