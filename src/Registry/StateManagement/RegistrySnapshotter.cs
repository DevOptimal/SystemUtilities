using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using DevOptimal.SystemUtilities.Registry.StateManagement.Serialization;
using Microsoft.Win32;
using System.IO;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    public class RegistrySnapshotter(IRegistry registry, DirectoryInfo persistenceDirectory)
        : Snapshotter(new RegistryCaretakerSerializer(registry), persistenceDirectory)
    {
        public RegistrySnapshotter()
            : this(new DefaultRegistry(), defaultPersistenceDirectory)
        { }

        public RegistrySnapshotter(IRegistry registry)
            : this(registry, defaultPersistenceDirectory)
        { }

        public RegistrySnapshotter(DirectoryInfo persistenceDirectory)
            : this(new DefaultRegistry(), persistenceDirectory)
        { }

        public ISnapshot SnapshotRegistryKey(RegistryHive hive, RegistryView view, string subKey)
        {
            var originator = new RegistryKeyOriginator(hive, view, subKey, registry);
            var snapshot = new Caretaker<RegistryKeyOriginator, RegistryKeyMemento>(originator, connection);
            return snapshot;
        }

        public ISnapshot SnapshotRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            var originator = new RegistryValueOriginator(hive, view, subKey, name, registry);
            var snapshot = new Caretaker<RegistryValueOriginator, RegistryValueMemento>(originator, connection);
            return snapshot;
        }
    }
}
