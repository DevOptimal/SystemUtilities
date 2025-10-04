using DevOptimal.SystemUtilities.Environment.Abstractions;
using Microsoft.QualityTools.Testing.Fakes;
using System;
using System.Diagnostics.Fakes;

namespace DevOptimal.SystemUtilities.Environment.Tests
{
    public abstract class MockEnvironmentTestBase
    {
        protected MockEnvironment environment;
        protected const string name = "foo";
        protected const EnvironmentVariableTarget target = EnvironmentVariableTarget.Machine;
        protected const string expectedValue = "bar";

        [TestInitialize]
        public void TestInitialize()
        {
            environment = new MockEnvironment();
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
