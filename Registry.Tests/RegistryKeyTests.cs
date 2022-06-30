using Microsoft.Win32;

namespace bradselw.System.Resources.Registry.Tests
{
    [TestClass]
    public class RegistryKeyTests
    {
        private MockRegistryProxy proxy;

        private const RegistryHive hive = RegistryHive.LocalMachine;

        private const RegistryView view = RegistryView.Default;

        private const string subKey = @"SOFTWARE\Microsoft\Windows";

        [TestInitialize]
        public void TestInitialize()
        {
            proxy = new MockRegistryProxy();
        }

        [TestMethod]
        public void CorrectlyIdentifiesNonexistentRegistryKey()
        {
            Assert.IsFalse(proxy.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void CorrectlyIdentifiesExistentRegistryKey()
        {
            proxy.registry[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase)
            {
                ["(Default)"] = (null, RegistryValueKind.String)
            };

            Assert.IsTrue(proxy.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void CreatesNewRegistryKey()
        {
            proxy.CreateRegistryKey(hive, view, subKey);

            Assert.IsTrue(proxy.registry.ContainsKey(hive));
            Assert.IsTrue(proxy.registry[hive].ContainsKey(view));
            Assert.IsTrue(proxy.registry[hive][view].ContainsKey(subKey));
            Assert.IsTrue(proxy.registry[hive][view][subKey].ContainsKey("(Default)"));
            Assert.IsTrue(proxy.registry[hive][view][subKey].Count == 1);
        }

        [TestMethod]
        public void DeletesRegistryKey()
        {
            proxy.registry[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase)
            {
                ["(Default)"] = (null, RegistryValueKind.String)
            };

            proxy.DeleteRegistryKey(hive, view, subKey, recursive: true);

            Assert.IsFalse(proxy.registry[hive][view].ContainsKey(subKey));
        }
    }
}