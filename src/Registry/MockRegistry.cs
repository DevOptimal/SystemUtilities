using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.Registry
{
    public class MockRegistry : IRegistry
    {
        private const string defaultValueName = "(Default)";

        internal readonly IDictionary<RegistryHive, IDictionary<RegistryView, IDictionary<string, IDictionary<string, (object, RegistryValueKind)>>>> data;

        public MockRegistry()
        {
            data = new ConcurrentDictionary<RegistryHive, IDictionary<RegistryView, IDictionary<string, IDictionary<string, (object, RegistryValueKind)>>>>();
            foreach (RegistryHive hive in Enum.GetValues(typeof(RegistryHive)))
            {
                data.Add(hive, new ConcurrentDictionary<RegistryView, IDictionary<string, IDictionary<string, (object, RegistryValueKind)>>>());

                foreach (RegistryView view in Enum.GetValues(typeof(RegistryView)))
                {
                    data[hive].Add(view, new ConcurrentDictionary<string, IDictionary<string, (object, RegistryValueKind)>>(StringComparer.OrdinalIgnoreCase));
                }
            }
        }

        public void CreateRegistryKey(RegistryHive hive, RegistryView view, string subKey)
        {
            subKey = RegistryPath.GetFullPath(subKey);

            if (!data[hive][view].ContainsKey(subKey))
            {
                data[hive][view][subKey] = new ConcurrentDictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public void DeleteRegistryKey(RegistryHive hive, RegistryView view, string subKey, bool recursive)
        {
            subKey = RegistryPath.GetFullPath(subKey);

            if (!data[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            data[hive][view].Remove(subKey);
        }

        public void DeleteRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            if (!data[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            if (!data[hive][view][subKey].ContainsKey(name))
            {
                throw new ArgumentException("name is not a valid reference to a value");
            }

            data[hive][view][subKey].Remove(name);
        }

        public (object value, RegistryValueKind kind) GetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            if (!data[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            if (!data[hive][view][subKey].ContainsKey(name))
            {
                throw new ArgumentException("name is not a valid reference to a value");
            }

            return data[hive][view][subKey][name];
        }

        public bool RegistryKeyExists(RegistryHive hive, RegistryView view, string subKey)
        {
            subKey = RegistryPath.GetFullPath(subKey);

            return data[hive][view].ContainsKey(subKey);
        }

        public bool RegistryValueExists(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            return data[hive][view].ContainsKey(subKey) && data[hive][view][subKey].ContainsKey(name);
        }

        public void SetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name, object value, RegistryValueKind kind)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            if (!data[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            data[hive][view][subKey][name] = (value, kind);
        }
    }
}
