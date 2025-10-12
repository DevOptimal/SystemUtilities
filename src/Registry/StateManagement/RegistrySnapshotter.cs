// Implements a snapshotter for registry resources, supporting registry key and value snapshots.
using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using DevOptimal.SystemUtilities.Registry.StateManagement.Serialization;
using Microsoft.Win32;
using System.IO;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    /// <summary>
    /// Manages transactional access and snapshots for registry resources (keys and values).
    /// </summary>
    public class RegistrySnapshotter : Snapshotter
    {
        /// <summary>
        /// Registry abstraction used to query and mutate registry keys/values.
        /// </summary>
        private readonly IRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrySnapshotter"/> class with the default registry.
        /// </summary>
        public RegistrySnapshotter()
            : this(new DefaultRegistry(), defaultPersistenceDirectory)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrySnapshotter"/> class with the specified registry.
        /// </summary>
        /// <param name="registry">The registry abstraction.</param>
        public RegistrySnapshotter(IRegistry registry)
            : this(registry, defaultPersistenceDirectory)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrySnapshotter"/> class with the specified persistence directory.
        /// Uses the default registry implementation.
        /// </summary>
        /// <param name="persistenceDirectory">The directory for persisting caretaker data.</param>
        public RegistrySnapshotter(DirectoryInfo persistenceDirectory)
            : this(new DefaultRegistry(), persistenceDirectory)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrySnapshotter"/> class with explicit registry and persistence directory.
        /// </summary>
        /// <param name="registry">The registry abstraction used for key/value operations.</param>
        /// <param name="persistenceDirectory">The directory backing caretaker persistence.</param>
        public RegistrySnapshotter(IRegistry registry, DirectoryInfo persistenceDirectory)
            : base(new RegistryCaretakerSerializer(registry), persistenceDirectory)
        {
            this.registry = registry;
        }

        /// <summary>
        /// Creates a snapshot of a registry key resource.
        /// </summary>
        /// <param name="hive">The registry hive.</param>
        /// <param name="view">The registry view.</param>
        /// <param name="subKey">The registry subkey path.</param>
        /// <returns>An <see cref="ISnapshot"/> representing the registry key's state.</returns>
        public ISnapshot SnapshotRegistryKey(RegistryHive hive, RegistryView view, string subKey)
        {
            var originator = new RegistryKeyOriginator(hive, view, subKey, registry);
            var snapshot = new RegistryKeyCaretaker(originator, this);
            return snapshot;
        }

        /// <summary>
        /// Creates a snapshot of a registry value resource.
        /// </summary>
        /// <param name="hive">The registry hive.</param>
        /// <param name="view">The registry view.</param>
        /// <param name="subKey">The registry subkey path.</param>
        /// <param name="name">The registry value name.</param>
        /// <returns>An <see cref="ISnapshot"/> representing the registry value's state.</returns>
        public ISnapshot SnapshotRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            var originator = new RegistryValueOriginator(hive, view, subKey, name, registry);
            var snapshot = new RegistryValueCaretaker(originator, this);
            return snapshot;
        }
    }
}
