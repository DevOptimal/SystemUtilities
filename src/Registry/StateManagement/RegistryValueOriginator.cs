using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System;
using System.Xml.Linq;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    internal class RegistryValueOriginator(RegistryHive hive, RegistryView view, string subKey, string name, IRegistry registry) : IOriginator<RegistryValueMemento>
    {
        public RegistryHive Hive { get; } = hive;

        public RegistryView View { get; } = view;

        public string SubKey { get; } = subKey;

        public string Name { get; } = name;

        public IRegistry Registry { get; } = registry ?? throw new ArgumentNullException(nameof(registry));

        public string GetID()
        {
            return $@"{Hive}\{View}\{SubKey}\\{Name ?? "(Default)"}".ToLower();
        }

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
