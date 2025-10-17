// Implements a caretaker serializer for file system resources, supporting streaming and efficient resource usage.
using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching;
using System;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement.Serialization
{
    /// <summary>
    /// Serializes and deserializes caretakers for file system resources (directories and files).
    /// Designed for efficient resource usage by streaming caretakers from a JSON source.
    /// </summary>
    internal class FileSystemCaretakerSerializer : CaretakerSerializer
    {
        /// <summary>
        /// Discriminator value written to <see cref="CaretakerSerializer.resourceTypePropertyName"/> for directory caretakers.
        /// </summary>
        private const string directoryResourceTypeName = "Directory";
        /// <summary>
        /// Discriminator value written to <see cref="CaretakerSerializer.resourceTypePropertyName"/> for file caretakers.
        /// </summary>
        private const string fileResourceTypeName = "File";

        /// <summary>
        /// File system abstraction used to construct originators during deserialization.
        /// </summary>
        private readonly IFileSystem fileSystem;
        /// <summary>
        /// File cache used to rehydrate file originators / mementos during deserialization.
        /// </summary>
        private readonly IFileCache fileCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemCaretakerSerializer"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system abstraction to use when (re)creating originators.</param>
        /// <param name="fileCache">The file cache abstraction used for file content persistence.</param>
        public FileSystemCaretakerSerializer(IFileSystem fileSystem, IFileCache fileCache)
        {
            this.fileSystem = fileSystem;
            this.fileCache = fileCache;
        }

        /// <summary>
        /// Converts a dictionary representation of a caretaker to an ICaretaker instance.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <param name="connection">The database connection context.</param>
        /// <returns>The deserialized ICaretaker.</returns>
        /// <exception cref="NotSupportedException">Thrown if the resource type discriminator is unknown.</exception>
        protected override ICaretaker ConvertDictionaryToCaretaker(IDictionary<string, object> dictionary, DatabaseConnection connection)
        {
            // Common caretaker metadata
            var id = AsString(dictionary[nameof(ICaretaker.ID)]);
            var parentId = AsString(dictionary[nameof(ICaretaker.ParentID)]);
            var processId = AsInteger(dictionary[nameof(ICaretaker.ProcessID)]);
            var processStartTime = AsDateTime(dictionary[nameof(ICaretaker.ProcessStartTime)]);

            // Dispatch based on resource type discriminator.
            switch (dictionary[resourceTypePropertyName])
            {
                case directoryResourceTypeName:
                    var directoryPath = AsString(dictionary[nameof(DirectoryOriginator.Path)]);
                    var directoryExists = AsBoolean(dictionary[nameof(DirectoryMemento.Exists)]);

                    var directoryOriginator = new DirectoryOriginator(directoryPath, fileSystem);
                    var directoryMemento = new DirectoryMemento { Exists = directoryExists };

                    return new DirectoryCaretaker(id, parentId, processId, processStartTime, connection, directoryOriginator, directoryMemento);
                case fileResourceTypeName:
                    var filePath = AsString(dictionary[nameof(FileOriginator.Path)]);
                    var fileHash = AsString(dictionary[nameof(FileMemento.Hash)]);

                    var fileOriginator = new FileOriginator(filePath, fileCache, fileSystem);
                    var fileMemento = new FileMemento { Hash = fileHash };

                    return new FileCaretaker(id, parentId, processId, processStartTime, connection, fileOriginator, fileMemento);
                default:
                    throw new NotSupportedException($"The resource type '{dictionary[resourceTypePropertyName]}' is not supported.");
            }
        }

        /// <summary>
        /// Converts an ICaretaker instance to a dictionary representation.
        /// </summary>
        /// <param name="caretaker">The caretaker to convert.</param>
        /// <returns>The dictionary representation.</returns>
        /// <exception cref="NotSupportedException">Thrown if the caretaker type is unknown.</exception>
        protected override IDictionary<string, object> ConvertCaretakerToDictionary(ICaretaker caretaker)
        {
            var result = new Dictionary<string, object>
            {
                [nameof(ICaretaker.ID)] = caretaker.ID,
                [nameof(ICaretaker.ParentID)] = caretaker.ParentID,
                [nameof(ICaretaker.ProcessID)] = caretaker.ProcessID,
                [nameof(ICaretaker.ProcessStartTime)] = caretaker.ProcessStartTime.Ticks
            };

            // Persist discriminator + resource specific data.
            switch (caretaker)
            {
                case DirectoryCaretaker directoryCaretaker:
                    result[resourceTypePropertyName] = directoryResourceTypeName;
                    result[nameof(DirectoryOriginator.Path)] = directoryCaretaker.Originator.Path;
                    result[nameof(DirectoryMemento.Exists)] = directoryCaretaker.Memento.Exists;
                    break;
                case FileCaretaker fileCaretaker:
                    result[resourceTypePropertyName] = fileResourceTypeName;
                    result[nameof(FileOriginator.Path)] = fileCaretaker.Originator.Path;
                    result[nameof(FileMemento.Hash)] = fileCaretaker.Memento.Hash;
                    break;
                default:
                    throw new NotSupportedException($"The caretaker type '{caretaker.GetType().Name}' is not supported.");
            }

            return result;
        }
    }
}
