using System;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    /// <summary>
    /// Represents a snapshot in the system, providing a contract for disposable snapshot resources.
    /// Implementations should encapsulate the logic for managing and cleaning up snapshot state.
    /// </summary>
    public interface ISnapshot : IDisposable
    {
    }
}
