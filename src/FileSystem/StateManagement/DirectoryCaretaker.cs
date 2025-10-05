using DevOptimal.SystemUtilities.Common.StateManagement;
using System;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    internal class DirectoryCaretaker : Caretaker<DirectoryOriginator, DirectoryMemento>
    {
        public DirectoryCaretaker(DirectoryOriginator originator, FileSystemSnapshotter snapshotter)
            : base(originator, snapshotter)
        {
        }

        public DirectoryCaretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, DirectoryOriginator originator, DirectoryMemento memento)
            : base(id, parentID, processID, processStartTime, connection, originator, memento)
        {
        }
    }
}
