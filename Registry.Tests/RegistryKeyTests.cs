using Microsoft.Win32;
using System.Runtime.Versioning;

namespace bradselw.System.Resources.Registry.Tests
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
        public void CorrectlyIdentifiesNonexistentRegistryKey()
        {
            Assert.IsFalse(proxy.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void CorrectlyIdentifiesExistentRegistryKey()
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