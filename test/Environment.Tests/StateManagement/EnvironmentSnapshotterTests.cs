using DevOptimal.SystemUtilities.Common.StateManagement.Exceptions;
using DevOptimal.SystemUtilities.Environment.StateManagement;
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
        public void Snapshot_RevertsEnvironmentVariableCreation()
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
        public void Snapshot_RevertsEnvironmentVariableDeletion()
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
        public void Snapshot_RevertsEnvironmentVariableAlteration()
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
        public void Snapshotter_RevertsEnvironmentVariableCreation()
        {
            environment.SetEnvironmentVariable(name, null, target);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotEnvironmentVariable(name, target);
                environment.SetEnvironmentVariable(name, "bar", target);
            }

            Assert.IsNull(environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        public void Snapshotter_RevertsEnvironmentVariableDeletion()
        {
            environment.SetEnvironmentVariable(name, expectedValue, target);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotEnvironmentVariable(name, target);
                environment.SetEnvironmentVariable(name, null, target);
            }

            Assert.AreEqual(expectedValue, environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        public void Snapshotter_RevertsEnvironmentVariableAlteration()
        {
            environment.SetEnvironmentVariable(name, expectedValue, target);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotEnvironmentVariable(name, target);
                environment.SetEnvironmentVariable(name, "baz", target);
            }

            Assert.AreEqual(expectedValue, environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        public void ThrowsWhenSnapshotLockedResource()
        {
            using var snapshotter1 = CreateSnapshotter();
            using var snapshotter2 = CreateSnapshotter();

            snapshotter1.SnapshotEnvironmentVariable(name, target);
            Assert.ThrowsExactly<ResourceLockedException>(() => snapshotter2.SnapshotEnvironmentVariable(name, target));
        }

        #region Persistence Tests

        [TestMethod]
        public void ConcurrentlySnapshotsEnvironmentVariables()
        {
            var concurrentThreads = 10;

            var names = new string[concurrentThreads];
            var expectedValues = new string[concurrentThreads];

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

            /*
             * First, we will test restoring an environment variable that didn't exist when the snapshot was taken, but has since been created
             */
            // Delete the environment variable
            environment.SetEnvironmentVariable(name, null, target);

            // Simulate taking a snapshot of the environment variable from another process
            using (CreateShimsContext())
            {
                var systemStateManager = CreateSnapshotter();
                systemStateManager.SnapshotEnvironmentVariable(name, target);
            }

            // Create the environment variable
            environment.SetEnvironmentVariable(name, expectedValue, target);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the environment variable has been deleted
            Assert.IsNull(environment.GetEnvironmentVariable(name, target));

            /*
             * Next, we will test restoring an environment variable that did exist when the snapshot was taken, but has since been deleted
             */
            // Create the environment variable
            environment.SetEnvironmentVariable(name, expectedValue, target);

            // Simulate taking a snapshot of the environment variable from another process
            using (CreateShimsContext())
            {
                var systemStateManager = CreateSnapshotter();
                systemStateManager.SnapshotEnvironmentVariable(name, target);
            }

            // Delete the environment variable
            environment.SetEnvironmentVariable(name, null, target);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the environment variable has been created
            Assert.AreEqual(expectedValue, environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        public void DoesNotRestoreSnapshotsFromCurrentProcess()
        {
            using var snapshotter = CreateSnapshotter();

            //using var snapshot = snapshotter.SnapshotEnvironmentVariable(name, target);
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
            var target = EnvironmentVariableTarget.Process;

            // Create the environment variable
            environment.SetEnvironmentVariable(name, expectedValue, target);

            // Simulate taking a snapshot of the environment variable from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateSnapshotter();
                snapshotter.SnapshotEnvironmentVariable(name, target);
            }

            // Delete the environment variable
            environment.SetEnvironmentVariable(name, null, target);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the environment variable has not been recreated
            Assert.IsNull(environment.GetEnvironmentVariable(name, target));
        }

        #endregion

        private EnvironmentSnapshotter CreateSnapshotter()
        {
            return new EnvironmentSnapshotter(environment, new DirectoryInfo(Path.Join(TestContext.ResultsDirectory, "Persistence")));
        }
    }
}
