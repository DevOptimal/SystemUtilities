// Implements the originator for a registry value, supporting state capture and restoration.
using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    /// <summary>
    /// Originator for a registry value resource, supporting state capture and restoration using the Memento pattern.
    /// </summary>
    /// <param name="hive">The registry hive.</param>
    /// <param name="view">The registry view.</param>
    /// <param name="subKey">The registry subkey path.</param>
    /// <param name="name">The registry value name.</param>
    /// <param name="registry">The registry abstraction.</param>
    internal class RegistryValueOriginator : IOriginator<RegistryValueMemento>
    {
        /// <summary>
        /// Gets the registry hive.
        /// </summary>
        public RegistryHive Hive { get; }

        /// <summary>
        /// Gets the registry view.
        /// </summary>
        public RegistryView View { get; }

        /// <summary>
        /// Gets the registry subkey path.
        /// </summary>
        public string SubKey { get; }

        /// <summary>
        /// Gets the registry value name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the registry abstraction.
        /// </summary>
        public IRegistry Registry { get; }

        /// <summary>
        /// Gets the unique identifier for this registry value originator.
        /// </summary>
        public string ID => $@"{Hive}\{View}\{SubKey}\\{Name ?? "(Default)"}".ToLower();

        public RegistryValueOriginator(RegistryHive hive, RegistryView view, string subKey, string name, IRegistry registry)
        {
            Hive = hive;
            View = view;
            SubKey = subKey;
            Name = name;
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        /// <summary>
        /// Captures the current state of the registry value as a memento.
        /// </summary>
        /// <returns>A memento representing the registry value's data and kind.</returns>
        public RegistryValueMemento GetState()
        {
            if (Registry.RegistryValueExists(Hive, View, SubKey, Name))
            {
                var (value, kind) = Registry.GetRegistryValue(Hive, View, SubKey, Name);
                return new RegistryValueMemento
                {
                    Value = value,
                    Kind = kind
                };
            }
            else
            {
                return new RegistryValueMemento
                {
                    Value = null,
                    Kind = RegistryValueKind.None
                };
            }
        }

        /// <summary>
        /// Restores the registry value's state from the provided memento.
        /// </summary>
        /// <param name="memento">The memento to restore state from.</param>
        public void SetState(RegistryValueMemento memento)
        {
            if (!Registry.RegistryKeyExists(Hive, View, SubKey))
            {
                Registry.CreateRegistryKey(Hive, View, SubKey);
            }

            if (memento.Value == null)
            {
                if (Registry.RegistryValueExists(Hive, View, SubKey, Name))
                {
                    Registry.DeleteRegistryValue(Hive, View, SubKey, Name);
                }
            }
            else
            {
                Registry.SetRegistryValue(Hive, View, SubKey, Name, memento.Value, memento.Kind);
            }
        }
    }
}
