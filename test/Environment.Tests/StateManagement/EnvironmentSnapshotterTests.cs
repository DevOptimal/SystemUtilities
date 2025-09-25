using DevOptimal.SystemUtilities.Environment.StateManagement;
using System;
using System.Collections.Generic;
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
        public void EnvironmentVariableSnapshotIsThreadSafe()
        {
            using var snapshotter = CreateSnapshotter();

            var target = EnvironmentVariableTarget.Machine;
            var expectedValue = "bar";

            var tasks = new List<Task>();

            for (var i = 0; i < 100; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var name = Guid.NewGuid().ToString();
                    using (snapshotter.SnapshotEnvironmentVariable(name, target))
                    {
                        environment.SetEnvironmentVariable(name, expectedValue, target);
                        Assert.AreEqual(expectedValue, environment.GetEnvironmentVariable(name, target));
                    }
                    Assert.IsNull(environment.GetEnvironmentVariable(name, target));
                }));
            }

            Task.WaitAll([.. tasks]);
        }

        [TestMethod]
        public void ConcurrentSnapshottersCanSnapshotEnvironmentVariables()
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
                var value = expectedValues[i];
                using var snapshotter = CreateSnapshotter();
                using (var caretaker = snapshotter.SnapshotEnvironmentVariable(name, target))
                {
                    environment.SetEnvironmentVariable(name, null, target);
                    Assert.IsNull(environment.GetEnvironmentVariable(name, target));
                }

                Assert.AreEqual(expectedValues[i], environment.GetEnvironmentVariable(name, target));
            });
        }

        private EnvironmentSnapshotter CreateSnapshotter()
        {
            return new EnvironmentSnapshotter(environment, new DirectoryInfo(Path.Join(TestContext.ResultsDirectory, "Persistence")));
        }
    }
}
