using System;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    /// <summary>
    /// Defines the contract for a caretaker in the Memento pattern.
    /// A caretaker manages the state of a resource, tracks its ownership, and provides restoration capability.
    /// </summary>
    internal interface ICaretaker
    {
        /// <summary>
        /// Gets the identifier of the parent snapshotter that owns this caretaker.
        /// </summary>
        string ParentID { get; }

        /// <summary>
        /// Gets the unique identifier for this caretaker/resource.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Gets the process ID associated with this caretaker.
        /// </summary>
        int ProcessID { get; }

        /// <summary>
        /// Gets the process start time associated with this caretaker.
        /// </summary>
        DateTime ProcessStartTime { get; }

        /// <summary>
        /// Restores the resource to the state managed by this caretaker.
        /// </summary>
        void Restore();
    }
}
