using DevOptimal.SystemUtilities.Environment.Abstractions;
using System;

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
    }
}
