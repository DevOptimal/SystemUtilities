using Microsoft.Win32;
using System.Runtime.Versioning;

namespace DevOptimal.SystemUtilities.Registry.Tests
{
    [TestClass]
    [SupportedOSPlatform("windows")]
    public class RegistryValueTests
    {
        private MockRegistryProxy proxy;

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
            proxy = new MockRegistryProxy();

            proxy.CreateRegistryKey(hive, view, subKey);
        }

        [TestMethod]
        public void IdentifiesNonexistentRegistryValue()
        {
            Assert.IsFalse(proxy.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void IdentifiesNonexistentDefaultRegistryValue()
        {
            Assert.IsFalse(proxy.RegistryValueExists(hive, view, subKey, null));
            Assert.IsFalse(proxy.RegistryValueExists(hive, view, subKey, defaultValueName));
        }

        [TestMethod]
        public void IdentifiesExistentRegistryValue()
        {
            proxy.registry[hive][view][subKey][name] = (expectedValue, expectedKind);

            Assert.IsTrue(proxy.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void IdentifiesExistentDefaultRegistryValue()
        {
            proxy.registry[hive][view][subKey][defaultValueName] = (expectedValue, expectedKind);

            Assert.IsTrue(proxy.RegistryValueExists(hive, view, subKey, null));
            Assert.IsTrue(proxy.RegistryValueExists(hive, view, subKey, defaultValueName));
        }

        [TestMethod]
        public void CreatesRegistryValue()
        {
            proxy.SetRegistryValue(hive, view, subKey, name, expectedValue, expectedKind);

            Assert.IsTrue(proxy.registry.ContainsKey(hive));
            Assert.IsTrue(proxy.registry[hive].ContainsKey(view));
            Assert.IsTrue(proxy.registry[hive][view].ContainsKey(subKey));
            Assert.IsTrue(proxy.registry[hive][view][subKey].ContainsKey(name));
            var (actualValue, actualKind) = proxy.registry[hive][view][subKey][name];
            Assert.AreEqual(expectedValue, actualValue);
            Assert.AreEqual(expectedKind, actualKind);
        }

        [TestMethod]
        public void DeletesRegistryValue()
        {
            proxy.registry[hive][view][subKey][name] = (expectedValue, expectedKind);

            proxy.DeleteRegistryValue(hive, view, subKey, name);

            Assert.IsFalse(proxy.registry[hive][view][subKey].ContainsKey(name));
        }

        [TestMethod]
        public void CreatesDefaultRegistryValue()
        {
            proxy.SetRegistryValue(hive, view, subKey, defaultValueName, expectedValue, expectedKind);

            Assert.IsTrue(proxy.registry.ContainsKey(hive));
            Assert.IsTrue(proxy.registry[hive].ContainsKey(view));
            Assert.IsTrue(proxy.registry[hive][view].ContainsKey(subKey));
            Assert.IsTrue(proxy.registry[hive][view][subKey].ContainsKey(defaultValueName));
            var (actualValue, actualKind) = proxy.registry[hive][view][subKey][defaultValueName];
            Assert.AreEqual(expectedValue, actualValue);
            Assert.AreEqual(expectedKind, actualKind);
        }

        [TestMethod]
        public void DeletesDefaultRegistryValue()
        {
            proxy.registry[hive][view][subKey][defaultValueName] = (expectedValue, expectedKind);

            proxy.DeleteRegistryValue(hive, view, subKey, defaultValueName);

            Assert.IsFalse(proxy.registry[hive][view][subKey].ContainsKey(defaultValueName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowsOnGetNonexistentRegistryKey()
        {
            var (value, kind) = proxy.GetRegistryValue(hive, view, subKey, name);
        }
    }
}
