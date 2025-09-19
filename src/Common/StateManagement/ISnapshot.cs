using System;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    public interface ISnapshot : IDisposable
    {
        string ID { get; }

        int ProcessID { get; }

        DateTime ProcessStartTime { get; }
    }
}
