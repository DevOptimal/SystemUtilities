﻿using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    internal class RegistryValueOriginator : IOriginator<RegistryValueMemento>
    {
        public RegistryHive Hive { get; }

        public RegistryView View { get; }

        public string SubKey { get; }

        public string Name { get; }

        public IRegistry Registry { get; }

        public RegistryValueOriginator(RegistryHive hive, RegistryView view, string subKey, string name, IRegistry registry)
        {
            Hive = hive;
            View = view;
            SubKey = subKey;
            Name = name;
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
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
