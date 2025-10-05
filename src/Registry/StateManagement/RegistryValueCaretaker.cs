using DevOptimal.SystemUtilities.Common.StateManagement;
using System;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    internal class RegistryValueCaretaker : Caretaker<RegistryValueOriginator, RegistryValueMemento>
    {
        public RegistryValueCaretaker(RegistryValueOriginator originator, RegistrySnapshotter snapshotter)
            : base(originator, snapshotter)
        {
        }

        public RegistryValueCaretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, RegistryValueOriginator originator, RegistryValueMemento memento)
            : base(id, parentID, processID, processStartTime, connection, originator, memento)
        {
        }
    }
}
