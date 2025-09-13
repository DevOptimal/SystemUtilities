﻿using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    internal class RegistryKeyOriginator : IOriginator<RegistryKeyMemento>
    {
        public RegistryHive Hive { get; }

        public RegistryView View { get; }

        public string SubKey { get; }

        public IRegistry Registry { get; }

        public RegistryKeyOriginator(RegistryHive hive, RegistryView view, string subKey, IRegistry registry)
        {
            Hive = hive;
            View = view;
            SubKey = subKey;
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

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
