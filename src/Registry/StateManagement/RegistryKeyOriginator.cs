using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    internal class RegistryKeyOriginator(RegistryHive hive, RegistryView view, string subKey, IRegistry registry) : IOriginator<RegistryKeyMemento>
    {
        public RegistryHive Hive { get; } = hive;

        public RegistryView View { get; } = view;

        public string SubKey { get; } = subKey;

        public IRegistry Registry { get; } = registry ?? throw new ArgumentNullException(nameof(registry));

        public string ID => $@"{Hive}\{View}\{SubKey}".ToLower();

        public RegistryKeyMemento GetState()
        {
            return new RegistryKeyMemento
            {
                Exists = Registry.RegistryKeyExists(Hive, View, SubKey)
            };
        }

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
