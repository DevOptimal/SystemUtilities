using DevOptimal.SystemUtilities.Common.StateManagement;
using System;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    internal class FileCaretaker : Caretaker<FileOriginator, FileMemento>
    {
        public FileCaretaker(FileOriginator originator, DatabaseConnection connection)
            : base(originator, connection)
        {
        }

        public FileCaretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, FileOriginator originator, FileMemento memento)
            : base(id, parentID, processID, processStartTime, connection, originator, memento)
        {
        }
    }
}
