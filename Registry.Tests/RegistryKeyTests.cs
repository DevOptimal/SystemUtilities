using Microsoft.Win32;
using System.Runtime.Versioning;

namespace DevOptimal.System.Resources.Registry.Tests
{
    [TestClass]
    [SupportedOSPlatform("windows")]
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
        public void IdentifiesNonexistentRegistryKey()
        {
            Assert.IsFalse(proxy.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void IdentifiesExistentRegistryKey()
        {
            proxy.registry[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase);

            Assert.IsTrue(proxy.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void CreatesNewRegistryKey()
        {
            proxy.CreateRegistryKey(hive, view, subKey);

            Assert.IsTrue(proxy.registry.ContainsKey(hive));
            Assert.IsTrue(proxy.registry[hive].ContainsKey(view));
            Assert.IsTrue(proxy.registry[hive][view].ContainsKey(subKey));
        }

        [TestMethod]
        public void DeletesRegistryKey()
        {
            proxy.registry[hive][view][subKey] = new Dictionary<string, (object, RegistryValueKind)>(StringComparer.OrdinalIgnoreCase);

            proxy.DeleteRegistryKey(hive, view, subKey, recursive: true);

            Assert.IsFalse(proxy.registry[hive][view].ContainsKey(subKey));
        }
    }
}