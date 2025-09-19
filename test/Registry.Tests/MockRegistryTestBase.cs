using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;
using System.Runtime.Versioning;

namespace DevOptimal.SystemUtilities.Registry.Tests
{
    [SupportedOSPlatform("windows")]
    public abstract class MockRegistryTestBase
    {
        protected MockRegistry registry;

        protected const RegistryHive hive = RegistryHive.LocalMachine;

        protected const RegistryView view = RegistryView.Default;

        protected const string subKey = @"SOFTWARE\Microsoft\Windows";

        protected const string name = "foo";

        protected const string defaultValueName = "(Default)";

        protected const string expectedValue = "bar";

        protected const RegistryValueKind expectedKind = RegistryValueKind.String;

        [TestInitialize]
        public void TestInitialize()
        {
            registry = new MockRegistry();
        }
    }
}
