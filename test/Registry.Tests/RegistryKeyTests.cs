using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace DevOptimal.SystemUtilities.Registry.Tests
{
    [TestClass]
    [SupportedOSPlatform("windows")]
    public class RegistryKeyTests
    {
        private MockRegistry registry;

        private const RegistryHive hive = RegistryHive.LocalMachine;

        private const RegistryView view = RegistryView.Default;

        private const string subKey = @"SOFTWARE\Microsoft\Windows";

        [TestInitialize]
        public void TestInitialize()
        {
            registry = new MockRegistry();
        }

        [TestMethod]
        public void IdentifiesNonexistentRegistryKey()
        {
            Assert.IsFalse(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void IdentifiesExistentRegistryKey()
        {
            registry.data[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase);

            Assert.IsTrue(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void CreatesNewRegistryKey()
        {
            registry.CreateRegistryKey(hive, view, subKey);

            Assert.IsTrue(registry.data.ContainsKey(hive));
            Assert.IsTrue(registry.data[hive].ContainsKey(view));
            Assert.IsTrue(registry.data[hive][view].ContainsKey(subKey));
        }

        [TestMethod]
        public void DeletesRegistryKey()
        {
            registry.data[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase);

            registry.DeleteRegistryKey(hive, view, subKey, recursive: true);

            Assert.IsFalse(registry.data[hive][view].ContainsKey(subKey));
        }
    }
}