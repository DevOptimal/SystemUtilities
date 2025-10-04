using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Win32;
using System;
using System.Diagnostics.Fakes;
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

        protected static IDisposable CreateShimsContext()
        {
            var context = ShimsContext.Create();

            ShimProcess.AllInstances.IdGet = p => System.Environment.ProcessId + 1;
            ShimProcess.AllInstances.StartTimeGet = p => DateTime.Now;

            return context;
        }
    }
}
