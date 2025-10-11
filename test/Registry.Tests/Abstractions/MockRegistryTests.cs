using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace DevOptimal.SystemUtilities.Registry.Tests.Abstractions
{
    [TestClass]
    [SupportedOSPlatform("windows")]
    public class MockRegistryTests : MockRegistryTestBase
    {
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

        [TestMethod]
        public void IdentifiesNonexistentRegistryValue()
        {
            Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void IdentifiesNonexistentDefaultRegistryValue()
        {
            Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, null));
            Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, defaultValueName));
        }

        [TestMethod]
        public void IdentifiesExistentRegistryValue()
        {
            registry.data[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase)
            {
                [name] = (expectedValue, expectedKind)
            };

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void IdentifiesExistentDefaultRegistryValue()
        {
            registry.data[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase)
            {
                [defaultValueName] = (expectedValue, expectedKind)
            };

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, null));
            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, defaultValueName));
        }

        [TestMethod]
        public void CreatesRegistryValue()
        {
            registry.data[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase);
            registry.SetRegistryValue(hive, view, subKey, name, expectedValue, expectedKind);

            Assert.IsTrue(registry.data.ContainsKey(hive));
            Assert.IsTrue(registry.data[hive].ContainsKey(view));
            Assert.IsTrue(registry.data[hive][view].ContainsKey(subKey));
            Assert.IsTrue(registry.data[hive][view][subKey].ContainsKey(name));
            var (actualValue, actualKind) = registry.data[hive][view][subKey][name];
            Assert.AreEqual(expectedValue, actualValue);
            Assert.AreEqual(expectedKind, actualKind);
        }

        [TestMethod]
        public void DeletesRegistryValue()
        {
            registry.data[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase)
            {
                [name] = (expectedValue, expectedKind)
            };

            registry.DeleteRegistryValue(hive, view, subKey, name);

            Assert.IsFalse(registry.data[hive][view][subKey].ContainsKey(name));
        }

        [TestMethod]
        public void CreatesDefaultRegistryValue()
        {
            registry.data[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase);
            registry.SetRegistryValue(hive, view, subKey, defaultValueName, expectedValue, expectedKind);

            Assert.IsTrue(registry.data.ContainsKey(hive));
            Assert.IsTrue(registry.data[hive].ContainsKey(view));
            Assert.IsTrue(registry.data[hive][view].ContainsKey(subKey));
            Assert.IsTrue(registry.data[hive][view][subKey].ContainsKey(defaultValueName));
            var (actualValue, actualKind) = registry.data[hive][view][subKey][defaultValueName];
            Assert.AreEqual(expectedValue, actualValue);
            Assert.AreEqual(expectedKind, actualKind);
        }

        [TestMethod]
        public void DeletesDefaultRegistryValue()
        {
            registry.data[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase)
            {
                [defaultValueName] = (expectedValue, expectedKind)
            };

            registry.DeleteRegistryValue(hive, view, subKey, defaultValueName);

            Assert.IsFalse(registry.data[hive][view][subKey].ContainsKey(defaultValueName));
        }

        [TestMethod]
        public void ThrowsOnGetNonexistentRegistryKey()
        {
            Assert.ThrowsExactly<ArgumentException>(() => registry.GetRegistryValue(hive, view, subKey, name));
        }
    }
}