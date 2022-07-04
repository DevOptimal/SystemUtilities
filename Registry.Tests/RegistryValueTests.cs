using Microsoft.Win32;
using System.Runtime.Versioning;

namespace bradselw.System.Resources.Registry.Tests
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
        public void CorrectlyIdentifiesNonexistentRegistryValue()
        {
            Assert.IsFalse(proxy.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void CorrectlyIdentifiesNonexistentDefaultRegistryValue()
        {
            Assert.IsFalse(proxy.RegistryValueExists(hive, view, subKey, null));
            Assert.IsFalse(proxy.RegistryValueExists(hive, view, subKey, defaultValueName));
        }

        [TestMethod]
        public void CorrectlyIdentifiesExistentRegistryValue()
        {
            proxy.registry[hive][view][subKey][name] = (expectedValue, expectedKind);

            Assert.IsTrue(proxy.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void CorrectlyIdentifiesExistentDefaultRegistryValue()
        {
            proxy.registry[hive][view][subKey][defaultValueName] = (expectedValue, expectedKind);

            Assert.IsTrue(proxy.RegistryValueExists(hive, view, subKey, null));
            Assert.IsTrue(proxy.RegistryValueExists(hive, view, subKey, defaultValueName));
        }

        [TestMethod]
        public void CorrectlyCreatesNewRegistryValue()
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
        public void CorrectlyDeletesRegistryValue()
        {
            proxy.registry[hive][view][subKey][name] = (expectedValue, expectedKind);

            proxy.DeleteRegistryValue(hive, view, subKey, name);

            Assert.IsFalse(proxy.registry[hive][view][subKey].ContainsKey(name));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNonexistentRegistryKey()
        {
            var (value, kind) = proxy.GetRegistryValue(hive, view, subKey, name);
        }
    }
}
