using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.Registry.Abstractions
{
    /// <summary>
    /// Provides an in-memory mock implementation of the <see cref="IRegistry"/> interface for testing purposes.
    /// </summary>
    public class MockRegistry : IRegistry
    {
        /// <summary>
        /// The name used for the default value in a registry key.
        /// </summary>
        private const string defaultValueName = "(Default)";

        /// <summary>
        /// The in-memory data structure representing the registry.
        /// Structure:
        ///   Hive -> View -> SubKeyPath -> (ValueName -> (Value, ValueKind))
        /// </summary>
        internal readonly IDictionary<RegistryHive, IDictionary<RegistryView, IDictionary<string, IDictionary<string, (object, RegistryValueKind)>>>> data;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockRegistry"/> class,
        /// setting up the in-memory structure for all hives and views.
        /// </summary>
        public MockRegistry()
        {
            data = new ConcurrentDictionary<RegistryHive, IDictionary<RegistryView, IDictionary<string, IDictionary<string, (object, RegistryValueKind)>>>>();
            foreach (RegistryHive hive in Enum.GetValues(typeof(RegistryHive)))
            {
                data.Add(hive, new ConcurrentDictionary<RegistryView, IDictionary<string, IDictionary<string, (object, RegistryValueKind)>>>()); // Initialize hive

                foreach (RegistryView view in Enum.GetValues(typeof(RegistryView)))
                {
                    // Initialize view for each hive
                    data[hive].Add(view, new ConcurrentDictionary<string, IDictionary<string, (object, RegistryValueKind)>>(StringComparer.OrdinalIgnoreCase));
                }
            }
        }

        /// <inheritdoc />
        public void CreateRegistryKey(RegistryHive hive, RegistryView view, string subKey)
        {
            subKey = RegistryPath.GetFullPath(subKey);

            // Create the subkey if it does not exist
            if (!data[hive][view].ContainsKey(subKey))
            {
                data[hive][view][subKey] = new ConcurrentDictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <inheritdoc />
        public void DeleteRegistryKey(RegistryHive hive, RegistryView view, string subKey, bool recursive)
        {
            subKey = RegistryPath.GetFullPath(subKey);

            // Throw if the subkey does not exist
            if (!data[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            // Remove the subkey
            data[hive][view].Remove(subKey);
        }

        /// <inheritdoc />
        public void DeleteRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            // Throw if the subkey does not exist
            if (!data[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            // Throw if the value does not exist
            if (!data[hive][view][subKey].ContainsKey(name))
            {
                throw new ArgumentException("name is not a valid reference to a value");
            }

            // Remove the value from the subkey
            data[hive][view][subKey].Remove(name);
        }

        /// <inheritdoc />
        public (object value, RegistryValueKind kind) GetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            // Throw if the subkey does not exist
            if (!data[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            // Throw if the value does not exist
            if (!data[hive][view][subKey].ContainsKey(name))
            {
                throw new ArgumentException("name is not a valid reference to a value");
            }

            // Return the value and its kind
            return data[hive][view][subKey][name];
        }

        /// <inheritdoc />
        public bool RegistryKeyExists(RegistryHive hive, RegistryView view, string subKey)
        {
            subKey = RegistryPath.GetFullPath(subKey);

            // Check if the subkey exists
            return data[hive][view].ContainsKey(subKey);
        }

        /// <inheritdoc />
        public bool RegistryValueExists(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            // Check if the subkey and value exist
            return data[hive][view].ContainsKey(subKey) && data[hive][view][subKey].ContainsKey(name);
        }

        /// <inheritdoc />
        public void SetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name, object value, RegistryValueKind kind)
        {
            subKey = RegistryPath.GetFullPath(subKey);
            name = name ?? defaultValueName;

            // Throw if the subkey does not exist
            if (!data[hive][view].ContainsKey(subKey))
            {
                throw new ArgumentException("subkey does not specify a valid registry subkey.");
            }

            // Set or update the value in the subkey
            data[hive][view][subKey][name] = (value, kind);
        }
    }
}
