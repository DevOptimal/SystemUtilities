using DevOptimal.SystemUtilities.Common.StateManagement;
using System;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    /// <summary>
    /// Manages the lifecycle and state of a directory resource using the Memento pattern.
    /// Handles resource locking, state persistence, and restoration for directories.
    /// </summary>
    internal class DirectoryCaretaker : Caretaker<DirectoryOriginator, DirectoryMemento>
    {
        /// <summary>
        /// Initializes a new caretaker for a directory, locks the resource, and persists the caretaker in the database.
        /// </summary>
        /// <param name="originator">The directory originator object to manage.</param>
        /// <param name="snapshotter">The snapshotter managing the caretaker's lifecycle.</param>
        public DirectoryCaretaker(DirectoryOriginator originator, FileSystemSnapshotter snapshotter)
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
        /// <param name="originator">The directory originator object.</param>
        /// <param name="memento">The memento representing the directory's state.</param>
        public DirectoryCaretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, DirectoryOriginator originator, DirectoryMemento memento)
            : base(id, parentID, processID, processStartTime, connection, originator, memento)
        {
        }
    }
}
