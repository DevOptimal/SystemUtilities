using DevOptimal.SystemUtilities.Common.StateManagement.Exceptions;
using DevOptimal.SystemUtilities.Environment.StateManagement;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DevOptimal.SystemUtilities.Environment.Tests.StateManagement
{
    [TestClass]
    public class EnvironmentSnapshotterTests : MockEnvironmentTestBase
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void RevertsEnvironmentVariableCreation()
        {
            environment.SetEnvironmentVariable(name, null, target);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotEnvironmentVariable(name, target))
            {
                environment.SetEnvironmentVariable(name, "bar", target);
            }

            Assert.IsNull(environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        public void RevertsEnvironmentVariableDeletion()
        {
            environment.SetEnvironmentVariable(name, expectedValue, target);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotEnvironmentVariable(name, target))
            {
                environment.SetEnvironmentVariable(name, null, target);
            }

            Assert.AreEqual(expectedValue, environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        public void RevertsEnvironmentVariableAlteration()
        {
            environment.SetEnvironmentVariable(name, expectedValue, target);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotEnvironmentVariable(name, target))
            {
                environment.SetEnvironmentVariable(name, "baz", target);
            }

            Assert.AreEqual(expectedValue, environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        public void ThrowsWhenSnapshotLockedResource()
        {
            var name = "foo";
            var target = EnvironmentVariableTarget.Machine;

            using var snapshotter1 = CreateSnapshotter();
            using var snapshotter2 = CreateSnapshotter();

            snapshotter1.SnapshotEnvironmentVariable(name, target);
            Assert.ThrowsExactly<ResourceLockedException>(() => snapshotter2.SnapshotEnvironmentVariable(name, target));
        }

        #region Persistence Tests

        [TestMethod]
        public void ConcurrentlySnapshotsEnvironmentVariables()
        {
            var concurrentThreads = 100;

            var names = new string[concurrentThreads];
            var expectedValues = new string[concurrentThreads];
            var target = EnvironmentVariableTarget.Machine;

            for (var i = 0; i < concurrentThreads; i++)
            {
                var name = $"variable{i}";
                var value = Guid.NewGuid().ToString();
                environment.SetEnvironmentVariable(name, value, target);
                names[i] = name;
                expectedValues[i] = value;
            }

            Parallel.For(0, concurrentThreads, i =>
            {
                var name = names[i];
                using var snapshotter = CreateSnapshotter();
                using (var caretaker = snapshotter.SnapshotEnvironmentVariable(name, target))
                {
                    environment.SetEnvironmentVariable(name, null, target);
                    Assert.IsNull(environment.GetEnvironmentVariable(name, target));
                }

                Assert.AreEqual(expectedValues[i], environment.GetEnvironmentVariable(name, target));
            });
        }

        [TestMethod]
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        public void RestoresAbandonedEnvironmentVariableSnapshots()
        {
            using var restoreSnapshotter = CreateSnapshotter();
            var environmentVariableName = "foo";
            var environmentVariableTarget = EnvironmentVariableTarget.Machine;
            var expectedEnvironmentVariableValue = "bar";

            /*
             * First, we will test restoring an environment variable that didn't exist when the snapshot was taken, but has since been created
             */
            // Delete the environment variable
            environment.SetEnvironmentVariable(environmentVariableName, null, environmentVariableTarget);

            // Simulate taking a snapshot of the environment variable from another process
            using (CreateShimsContext())
            {
                var systemStateManager = CreateSnapshotter();
                systemStateManager.SnapshotEnvironmentVariable(environmentVariableName, environmentVariableTarget);
            }

            // Create the environment variable
            environment.SetEnvironmentVariable(environmentVariableName, expectedEnvironmentVariableValue, environmentVariableTarget);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the environment variable has been deleted
            Assert.IsNull(environment.GetEnvironmentVariable(environmentVariableName, environmentVariableTarget));

            /*
             * Next, we will test restoring an environment variable that did exist when the snapshot was taken, but has since been deleted
             */
            // Create the environment variable
            environment.SetEnvironmentVariable(environmentVariableName, expectedEnvironmentVariableValue, environmentVariableTarget);

            // Simulate taking a snapshot of the environment variable from another process
            using (CreateShimsContext())
            {
                var systemStateManager = CreateSnapshotter();
                systemStateManager.SnapshotEnvironmentVariable(environmentVariableName, environmentVariableTarget);
            }

            // Delete the environment variable
            environment.SetEnvironmentVariable(environmentVariableName, null, environmentVariableTarget);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the environment variable has been created
            Assert.AreEqual(expectedEnvironmentVariableValue, environment.GetEnvironmentVariable(environmentVariableName, environmentVariableTarget));
        }

        [TestMethod]
        public void DoesNotRestoreSnapshotsFromCurrentProcess()
        {
            var name = "foo";
            var target = EnvironmentVariableTarget.Machine;
            var expectedValue = "bar";

            using var snapshotter = CreateSnapshotter();

            snapshotter.SnapshotEnvironmentVariable(name, target);

            environment.SetEnvironmentVariable(name, null, target);

            snapshotter.RestoreAbandonedSnapshots();
            Assert.AreNotEqual(expectedValue, environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        public void DoesNotRestoreProcessScopedEnvironmentVariableSnapshots()
        {
            using var restoreSnapshotter = CreateSnapshotter();
            var environmentVariableName = "foo";
            var environmentVariableTarget = EnvironmentVariableTarget.Process;
            var environmentVariableValue = "bar";

            // Create the environment variable
            environment.SetEnvironmentVariable(environmentVariableName, environmentVariableValue, environmentVariableTarget);

            // Simulate taking a snapshot of the environment variable from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateSnapshotter();
                snapshotter.SnapshotEnvironmentVariable(environmentVariableName, environmentVariableTarget);
            }

            // Delete the environment variable
            environment.SetEnvironmentVariable(environmentVariableName, null, environmentVariableTarget);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the environment variable has not been recreated
            Assert.IsNull(environment.GetEnvironmentVariable(environmentVariableName, environmentVariableTarget));
        }

        #endregion

        private EnvironmentSnapshotter CreateSnapshotter()
        {
            return new EnvironmentSnapshotter(environment, new DirectoryInfo(Path.Join(TestContext.ResultsDirectory, "Persistence")));
        }
    }
}
