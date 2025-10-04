using DevOptimal.SystemUtilities.Common.StateManagement;
using System;
using System.Diagnostics;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    internal class EnvironmentVariableCaretaker : Caretaker<EnvironmentVariableOriginator, EnvironmentVariableMemento>
    {
        public EnvironmentVariableCaretaker(EnvironmentVariableOriginator originator, DatabaseConnection connection)
            : base(originator, connection)
        {
        }

        public EnvironmentVariableCaretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, EnvironmentVariableOriginator originator, EnvironmentVariableMemento memento)
            : base(id, parentID, processID, processStartTime, connection, originator, memento)
        {
        }

        public override void Restore()
        {
            if (Originator.Target == EnvironmentVariableTarget.Process)
            {
                var currentProcess = Process.GetCurrentProcess();
                if (ProcessID != currentProcess.Id || ProcessStartTime != currentProcess.StartTime)
                {
                    return; // Do not restore process-level environment variables for processes that are not the same as the one that created the caretaker.
                }
            }
            Originator.SetState(Memento);
        }
    }
}
