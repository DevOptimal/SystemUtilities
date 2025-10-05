using DevOptimal.SystemUtilities.Common.StateManagement;
using System;
using System.Diagnostics;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    /// <summary>
    /// Caretaker implementation for environment variables.
    /// Manages the lifecycle and restoration of an environment variable's state using the Memento pattern.
    /// </summary>
    internal class EnvironmentVariableCaretaker : Caretaker<EnvironmentVariableOriginator, EnvironmentVariableMemento>
    {
        /// <summary>
        /// Initializes a new caretaker for an environment variable, locks the resource, and persists the caretaker in the database.
        /// </summary>
        /// <param name="originator">The environment variable originator to manage.</param>
        /// <param name="snapshotter">The snapshotter managing the caretaker's lifecycle.</param>
        public EnvironmentVariableCaretaker(EnvironmentVariableOriginator originator, EnvironmentSnapshotter snapshotter)
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
        /// <param name="originator">The environment variable originator.</param>
        /// <param name="memento">The memento representing the environment variable's state.</param>
        public EnvironmentVariableCaretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, EnvironmentVariableOriginator originator, EnvironmentVariableMemento memento)
            : base(id, parentID, processID, processStartTime, connection, originator, memento)
        {
        }

        /// <summary>
        /// Restores the environment variable's state from the memento.
        /// For process-level variables, only restores if the current process matches the original process.
        /// </summary>
        public override void Restore()
        {
            if (Originator.Target == EnvironmentVariableTarget.Process)
            {
                var currentProcess = Process.GetCurrentProcess();
                if (ProcessID != currentProcess.Id || ProcessStartTime != currentProcess.StartTime)
                {
                    // Do not restore process-level environment variables for processes that are not the same as the one that created the caretaker.
                    return;
                }
            }
            Originator.SetState(Memento);
        }
    }
}
