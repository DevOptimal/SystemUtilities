using System;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    public interface ISnapshot
    {
        string ID { get; }

        int ProcessID { get; }

        DateTime ProcessStartTime { get; }
    }
}
