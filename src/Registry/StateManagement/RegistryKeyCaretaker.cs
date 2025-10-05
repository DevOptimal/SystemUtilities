using DevOptimal.SystemUtilities.Common.StateManagement;
using System;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    internal class RegistryKeyCaretaker : Caretaker<RegistryKeyOriginator, RegistryKeyMemento>
    {
        public RegistryKeyCaretaker(RegistryKeyOriginator originator, RegistrySnapshotter snapshotter)
            : base(originator, snapshotter)
        {
        }

        public RegistryKeyCaretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, RegistryKeyOriginator originator, RegistryKeyMemento memento)
            : base(id, parentID, processID, processStartTime, connection, originator, memento)
        {
        }
    }
}
