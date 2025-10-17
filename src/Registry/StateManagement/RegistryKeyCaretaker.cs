// Provides caretaker functionality for registry key resources, enabling transactional state management and restoration.
using DevOptimal.SystemUtilities.Common.StateManagement;
using System;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    /// <summary>
    /// Manages the lifecycle and state of a registry key resource using the Memento pattern.
    /// Handles resource locking, state persistence, and restoration for registry keys.
    /// </summary>
    internal class RegistryKeyCaretaker : Caretaker<RegistryKeyOriginator, RegistryKeyMemento>
    {
        /// <summary>
        /// Initializes a new caretaker for a registry key, locks the resource, and persists the caretaker in the database.
        /// </summary>
        /// <param name="originator">The registry key originator object to manage.</param>
        /// <param name="snapshotter">The snapshotter managing the caretaker's lifecycle.</param>
        public RegistryKeyCaretaker(RegistryKeyOriginator originator, RegistrySnapshotter snapshotter)
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
        /// <param name="originator">The registry key originator object.</param>
        /// <param name="memento">The memento representing the registry key's state.</param>
        public RegistryKeyCaretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, RegistryKeyOriginator originator, RegistryKeyMemento memento)
            : base(id, parentID, processID, processStartTime, connection, originator, memento)
        {
        }
    }
}
