using DevOptimal.SystemUtilities.Environment.StateManagement;

namespace DevOptimal.SystemUtilities.Environment.Tests.StateManagement
{
    [TestClass]
    public class EnvironmentSnapshotterTests : MockEnvironmentTestBase
    {
        [TestMethod]
        public void RevertsEnvironmentVariableCreation()
        {
            environment.SetEnvironmentVariable(name, null, target);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotEnvironmentVariable(name, target))
            {
                environment.SetEnvironmentVariable(name, "bar", target);
            }

            Assert.IsNull(environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        public void RevertsEnvironmentVariableDeletion()
        {
            environment.SetEnvironmentVariable(name, expectedValue, target);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotEnvironmentVariable(name, target))
            {
                environment.SetEnvironmentVariable(name, null, target);
            }

            Assert.AreEqual(expectedValue, environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        public void RevertsEnvironmentVariableAlteration()
        {
            environment.SetEnvironmentVariable(name, expectedValue, target);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotEnvironmentVariable(name, target))
            {
                environment.SetEnvironmentVariable(name, "baz", target);
            }

            Assert.AreEqual(expectedValue, environment.GetEnvironmentVariable(name, target));
        }

        private EnvironmentSnapshotter CreateSnapshotter()
        {
            return new EnvironmentSnapshotter(environment);
        }
    }
}
