using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevOptimal.System.Resources.Registry
{
    public class MockRegistryProxy : IRegistryProxy
    {
        private const string defaultValueName = "(Default)";

        internal readonly IDictionary<RegistryHive, IDictionary<RegistryView, IDictionary<string, IDictionary<string, (object, RegistryValueKind)>>>> registry;

        public MockRegistryProxy()
        {
            registry = new ConcurrentDictionary<RegistryHive, IDictionary<RegistryView, IDictionary<string, IDictionary<string, (object, RegistryValueKind)>>>>();
            foreach (RegistryHive hive in Enum.GetValues(typeof(RegistryHive)))
            {
                registry.Add(hive, new ConcurrentDictionary<RegistryView, IDictionary<string, IDictionary<string, (object, RegistryValueKind)>>>());

                foreach (RegistryView view in Enum.GetValues(typeof(RegistryView)))
                {
                    registry[hive].Add(view, new ConcurrentDictionary<string, IDictionary<string, (object, RegistryValueKind)>>(StringComparer.OrdinalIgnoreCase));
                }
            }
        }

        public void CreateRegistryKey(RegistryHive hive, RegistryView view, string subKey)
        {
            subKey = RegistryPath.GetFullPath(subKey);

            if (!registry[hive][view].ContainsKey(subKey))
            {
                registry[hive][view][subKey] = new ConcurrentDictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public void DeleteRegistryKey(RegistryHive hive, RegistryView view, string subKey, bool recursive)
        {
            subKey = RegistryPath.GetFullPath(subKey);

            if (!registry[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            registry[hive][view].Remove(subKey);
        }

        public void DeleteRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            if (!registry[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            if (!registry[hive][view][subKey].ContainsKey(name))
            {
                throw new ArgumentException("name is not a valid reference to a value");
            }

            registry[hive][view][subKey].Remove(name);
        }

        public (object value, RegistryValueKind kind) GetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            if (!registry[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            if (!registry[hive][view][subKey].ContainsKey(name))
            {
                throw new ArgumentException("name is not a valid reference to a value");
            }

            return registry[hive][view][subKey][name];
        }

        public bool RegistryKeyExists(RegistryHive hive, RegistryView view, string subKey)
        {
            subKey = RegistryPath.GetFullPath(subKey);

            return registry[hive][view].ContainsKey(subKey);
        }

        public bool RegistryValueExists(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            return registry[hive][view].ContainsKey(subKey) && registry[hive][view][subKey].ContainsKey(name);
        }

        public void SetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name, object value, RegistryValueKind kind)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            if (!registry[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            registry[hive][view][subKey][name] = (value, kind);
        }
    }
}
