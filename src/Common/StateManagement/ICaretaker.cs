using System;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    internal interface ICaretaker
    {
        string ParentID { get; }

        string ID { get; }

        int ProcessID { get; }

        DateTime ProcessStartTime { get; }

        void Restore();
    }
}
