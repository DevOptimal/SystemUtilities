using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System;
using System.Runtime.Versioning;

namespace DevOptimal.SystemUtilities.Registry.Tests
{
    [TestClass]
    [SupportedOSPlatform("windows")]
    public class RegistryValueTests
    {
        private MockRegistry registry;

        private const RegistryHive hive = RegistryHive.LocalMachine;

        private const RegistryView view = RegistryView.Default;

        private const string subKey = @"SOFTWARE\Microsoft\Windows";

        private const string name = "foo";

        private const string defaultValueName = "(Default)";

        private const string expectedValue = "bar";

        private const RegistryValueKind expectedKind = RegistryValueKind.String;

        [TestInitialize]
        public void TestInitialize()
        {
            registry = new MockRegistry();

            registry.CreateRegistryKey(hive, view, subKey);
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
            registry.data[hive][view][subKey][name] = (expectedValue, expectedKind);

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void IdentifiesExistentDefaultRegistryValue()
        {
            registry.data[hive][view][subKey][defaultValueName] = (expectedValue, expectedKind);

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, null));
            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, defaultValueName));
        }

        [TestMethod]
        public void CreatesRegistryValue()
        {
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
            registry.data[hive][view][subKey][name] = (expectedValue, expectedKind);

            registry.DeleteRegistryValue(hive, view, subKey, name);

            Assert.IsFalse(registry.data[hive][view][subKey].ContainsKey(name));
        }

        [TestMethod]
        public void CreatesDefaultRegistryValue()
        {
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
            registry.data[hive][view][subKey][defaultValueName] = (expectedValue, expectedKind);

            registry.DeleteRegistryValue(hive, view, subKey, defaultValueName);

            Assert.IsFalse(registry.data[hive][view][subKey].ContainsKey(defaultValueName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowsOnGetNonexistentRegistryKey()
        {
            var (value, kind) = registry.GetRegistryValue(hive, view, subKey, name);
        }
    }
}
