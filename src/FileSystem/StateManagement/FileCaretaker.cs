// Provides caretaker functionality for file resources, enabling transactional state management and restoration.
using DevOptimal.SystemUtilities.Common.StateManagement;
using System;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    /// <summary>
    /// Manages the lifecycle and state of a file resource using the Memento pattern.
    /// Handles resource locking, state persistence, and restoration for files.
    /// </summary>
    internal class FileCaretaker : Caretaker<FileOriginator, FileMemento>
    {
        /// <summary>
        /// Initializes a new caretaker for a file, locks the resource, and persists the caretaker in the database.
        /// </summary>
        /// <param name="originator">The file originator object to manage.</param>
        /// <param name="snapshotter">The snapshotter managing the caretaker's lifecycle.</param>
        public FileCaretaker(FileOriginator originator, FileSystemSnapshotter snapshotter)
            : base(originator, snapshotter)
        {
        }

        /// <summary>
        /// Initializes a caretaker for deserialization, restoring all properties from persisted state.
        /// </summary>
        /// <param name="id">The caretaker's unique identifier.</param>
        /// <param name="parentID">The parent snapshotter's identifier.</param>
        /// <param name="processID">The process ID associated with the caretaker.</param>
        /// <param name="processStartTime">The process start time.</param>
        /// <param name="connection">The database connection.</param>
        /// <param name="originator">The file originator object.</param>
        /// <param name="memento">The memento representing the file's state.</param>
        public FileCaretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, FileOriginator originator, FileMemento memento)
            : base(id, parentID, processID, processStartTime, connection, originator, memento)
        {
        }
    }
}
