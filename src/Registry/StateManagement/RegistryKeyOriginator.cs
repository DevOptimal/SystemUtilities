using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    /// <summary>
    /// Originator for a registry key resource, supporting state capture and restoration using the Memento pattern.
    /// </summary>
    internal class RegistryKeyOriginator : IOriginator<RegistryKeyMemento>
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
        /// Gets the registry abstraction.
        /// </summary>
        public IRegistry Registry { get; }

        /// <summary>
        /// Gets the unique identifier for this registry key originator.
        /// Lower-cased to provide case-insensitive semantics across platforms.
        /// </summary>
        public string ID => $@"{Hive}\{View}\{SubKey}".ToLower();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryKeyOriginator"/> class.
        /// </summary>
        /// <param name="hive">The registry hive.</param>
        /// <param name="view">The registry view.</param>
        /// <param name="subKey">The registry subkey path.</param>
        /// <param name="registry">The registry abstraction.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registry"/> is null.</exception>
        public RegistryKeyOriginator(RegistryHive hive, RegistryView view, string subKey, IRegistry registry)
        {
            Hive = hive;
            View = view;
            SubKey = subKey;
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        /// <summary>
        /// Captures the current state of the registry key as a memento.
        /// </summary>
        /// <returns>A memento representing the registry key's existence.</returns>
        public RegistryKeyMemento GetState()
        {
            return new RegistryKeyMemento
            {
                Exists = Registry.RegistryKeyExists(Hive, View, SubKey)
            };
        }

        /// <summary>
        /// Restores the registry key's state from the provided memento.
        /// </summary>
        /// <param name="memento">The memento to restore state from.</param>
        public void SetState(RegistryKeyMemento memento)
        {
            if (Registry.RegistryKeyExists(Hive, View, SubKey))
            {
                if (!memento.Exists)
                {
                    Registry.DeleteRegistryKey(Hive, View, SubKey, recursive: true);
                }
            }
            else
            {
                if (memento.Exists)
                {
                    Registry.CreateRegistryKey(Hive, View, SubKey);
                }
            }
        }
    }
}
