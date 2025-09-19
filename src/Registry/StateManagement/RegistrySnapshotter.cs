using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using DevOptimal.SystemUtilities.Registry.StateManagement.Serialization;
using Microsoft.Win32;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    public class RegistrySnapshotter(IRegistry registry)
        : Snapshotter(new RegistrySnapshotSerializer(registry))
    {
        public RegistrySnapshotter() : this(new DefaultRegistry())
        { }

        public ISnapshot SnapshotRegistryKey(RegistryHive hive, RegistryView view, string subKey)
        {
            var originator = new RegistryKeyOriginator(hive, view, subKey, registry);
            var snapshot = new Caretaker<RegistryKeyOriginator, RegistryKeyMemento>(originator, database);
            AddSnapshot(snapshot);
            return snapshot;
        }

        public ISnapshot SnapshotRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            var originator = new RegistryValueOriginator(hive, view, subKey, name, registry);
            var snapshot = new Caretaker<RegistryValueOriginator, RegistryValueMemento>(originator, database);
            AddSnapshot(snapshot);
            return snapshot;
        }
    }
}
