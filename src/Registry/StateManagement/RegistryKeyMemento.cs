// Represents the memento for a registry key, capturing its existence state.
using DevOptimal.SystemUtilities.Common.StateManagement;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    /// <summary>
    /// Memento representing the state of a registry key (whether it exists).
    /// </summary>
    internal class RegistryKeyMemento : IMemento
    {
        /// <summary>
        /// Gets or sets a value indicating whether the registry key exists.
        /// </summary>
        public bool Exists { get; set;  }
    }
}
