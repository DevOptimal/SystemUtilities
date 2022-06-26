using Microsoft.Win32;

namespace bradselw.System.Resources.Registry.Tests
{
    [TestClass]
    public class CreateRegistryKeyTests
    {
        private MockRegistryProxy proxyUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            proxyUnderTest = new MockRegistryProxy();
        }

        [TestMethod]
        public void CorrectlyCreatesNewRegistryKey()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";

            proxyUnderTest.CreateRegistryKey(hive, view, subKey);

            Assert.IsTrue(proxyUnderTest.registry.ContainsKey(hive));
            Assert.IsTrue(proxyUnderTest.registry[hive].ContainsKey(view));
            Assert.IsTrue(proxyUnderTest.registry[hive][view].ContainsKey(subKey));
            Assert.IsTrue(proxyUnderTest.registry[hive][view][subKey].ContainsKey("(Default)"));
            Assert.IsTrue(proxyUnderTest.registry[hive][view][subKey].Count == 1);
        }
    }
}