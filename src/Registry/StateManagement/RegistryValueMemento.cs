// Represents the memento for a registry value, capturing its value and kind.
using DevOptimal.SystemUtilities.Common.StateManagement;
using Microsoft.Win32;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    /// <summary>
    /// Memento representing the state of a registry value (its value and kind).
    /// </summary>
    internal class RegistryValueMemento : IMemento
    {
        /// <summary>
        /// Gets or sets the value of the registry entry.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the kind of the registry value.
        /// </summary>
        public RegistryValueKind Kind { get; set; }
    }
}
