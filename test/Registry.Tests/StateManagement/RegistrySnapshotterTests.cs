using DevOptimal.SystemUtilities.Registry.StateManagement;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace DevOptimal.SystemUtilities.Registry.Tests.StateManagement
{
    [TestClass]
    [SupportedOSPlatform("windows")]
    public class RegistrySnapshotterTests : MockRegistryTestBase
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Snapshot_RevertsRegistryKeyCreation()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotRegistryKey(hive, view, subKey))
            {
                registry.CreateRegistryKey(hive, view, subKey);
            }

            Assert.IsFalse(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void Snapshot_RevertsRegistryKeyCreationWithChildren()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotRegistryKey(hive, view, subKey))
            {
                registry.CreateRegistryKey(hive, view, subKey);
                registry.CreateRegistryKey(hive, view, Path.Combine(subKey, "foo"));
                registry.SetRegistryValue(hive, view, subKey, "bar", "Hello, world!", RegistryValueKind.String);
            }

            Assert.IsFalse(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void Snapshot_RevertsRegistryKeyDeletion()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";

            registry.CreateRegistryKey(hive, view, subKey);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotRegistryKey(hive, view, subKey))
            {
                registry.DeleteRegistryKey(hive, view, subKey, recursive: true);
            }

            Assert.IsTrue(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void Snapshotter_RevertsRegistryKeyCreation()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotRegistryKey(hive, view, subKey);
                registry.CreateRegistryKey(hive, view, subKey);
            }

            Assert.IsFalse(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void Snapshotter_RevertsRegistryKeyCreationWithChildren()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotRegistryKey(hive, view, subKey);
                registry.CreateRegistryKey(hive, view, subKey);
                registry.CreateRegistryKey(hive, view, Path.Combine(subKey, "foo"));
                registry.SetRegistryValue(hive, view, subKey, "bar", "Hello, world!", RegistryValueKind.String);
            }

            Assert.IsFalse(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void Snapshotter_RevertsRegistryKeyDeletion()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";

            registry.CreateRegistryKey(hive, view, subKey);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotRegistryKey(hive, view, subKey);
                registry.DeleteRegistryKey(hive, view, subKey, recursive: true);
            }

            Assert.IsTrue(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void Snapshot_RevertsRegistryValueAlteration()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var name = "foo";
            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, name, value, kind);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotRegistryValue(hive, view, subKey, name))
            {
                registry.SetRegistryValue(hive, view, subKey, name, 10, RegistryValueKind.DWord);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, name));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, name);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void Snapshot_RevertsRegistryValueCreation()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var name = "foo";

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotRegistryValue(hive, view, subKey, name))
            {
                registry.SetRegistryValue(hive, view, subKey, name, "bar", RegistryValueKind.String);
            }

            Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void Snapshot_RevertsRegistryValueDeletion()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var name = "foo";
            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, name, value, kind);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotRegistryValue(hive, view, subKey, name))
            {
                registry.DeleteRegistryValue(hive, view, subKey, name);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, name));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, name);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void Snapshot_RevertsDefaultRegistryValueAlteration()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, null, value, kind);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotRegistryValue(hive, view, subKey, null))
            {
                registry.SetRegistryValue(hive, view, subKey, null, 10, RegistryValueKind.DWord);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, null));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, null);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void Snapshot_RevertsDefaultRegistryValueCreation()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotRegistryValue(hive, view, subKey, null))
            {
                registry.SetRegistryValue(hive, view, subKey, null, "bar", RegistryValueKind.String);
            }

            Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, null));
        }

        [TestMethod]
        public void Snapshot_RevertsDefaultRegistryValueDeletion()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, null, value, kind);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotRegistryValue(hive, view, subKey, null))
            {
                registry.DeleteRegistryValue(hive, view, subKey, null);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, null));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, null);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void Snapshotter_RevertsRegistryValueAlteration()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var name = "foo";
            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, name, value, kind);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotRegistryValue(hive, view, subKey, name);
                registry.SetRegistryValue(hive, view, subKey, name, 10, RegistryValueKind.DWord);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, name));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, name);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void Snapshotter_RevertsRegistryValueCreation()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var name = "foo";

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotRegistryValue(hive, view, subKey, name);
                registry.SetRegistryValue(hive, view, subKey, name, "bar", RegistryValueKind.String);
            }

            Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void Snapshotter_RevertsRegistryValueDeletion()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var name = "foo";
            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, name, value, kind);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotRegistryValue(hive, view, subKey, name);
                registry.DeleteRegistryValue(hive, view, subKey, name);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, name));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, name);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void Snapshotter_RevertsDefaultRegistryValueAlteration()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, null, value, kind);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotRegistryValue(hive, view, subKey, null);
                registry.SetRegistryValue(hive, view, subKey, null, 10, RegistryValueKind.DWord);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, null));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, null);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void Snapshotter_RevertsDefaultRegistryValueCreation()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotRegistryValue(hive, view, subKey, null);
                registry.SetRegistryValue(hive, view, subKey, null, "bar", RegistryValueKind.String);
            }

            Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, null));
        }

        [TestMethod]
        public void Snapshotter_RevertsDefaultRegistryValueDeletion()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, null, value, kind);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotRegistryValue(hive, view, subKey, null);
                registry.DeleteRegistryValue(hive, view, subKey, null);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, null));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, null);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        #region Persistence Tests

        [TestMethod]
        public void ConcurrentlySnapshotsRegistryKeys()
        {
            var concurrentThreads = 10;

            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var key = @"SOFTWARE\Microsoft\Windows";

            var subKeys = new string[concurrentThreads];

            for (var i = 0; i < concurrentThreads; i++)
            {
                var subKey = @$"{key}\subKey{i}";
                registry.CreateRegistryKey(hive, view, subKey);
                subKeys[i] = subKey;
            }

            Parallel.For(0, concurrentThreads, i =>
            {
                var subKey = subKeys[i];
                using var snapshotter = CreateSnapshotter();
                using (var caretaker = snapshotter.SnapshotRegistryKey(hive, view, subKey))
                {
                    registry.DeleteRegistryKey(hive, view, subKey, recursive: true);
                    Assert.IsFalse(registry.RegistryKeyExists(hive, view, subKey));
                }

                Assert.IsTrue(registry.RegistryKeyExists(hive, view, subKey));
            });
        }

        [TestMethod]
        public void ConcurrentlySnapshotsRegistryValues()
        {
            var concurrentThreads = 10;

            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var kind = RegistryValueKind.String;

            var names = new string[concurrentThreads];
            var expectedValues = new string[concurrentThreads];
            for (var i = 0; i < concurrentThreads; i++)
            {
                var name = $"variable{i}";
                var value = Guid.NewGuid().ToString();
                registry.SetRegistryValue(hive, view, subKey, name, value, kind);
                names[i] = name;
                expectedValues[i] = value;
            }

            Parallel.For(0, concurrentThreads, i =>
            {
                var name = names[i];
                using var snapshotter = CreateSnapshotter();
                using (var caretaker = snapshotter.SnapshotRegistryValue(hive, view, subKey, name))
                {
                    registry.DeleteRegistryValue(hive, view, subKey, name);
                    Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, name));
                }

                Assert.AreEqual(expectedValues[i], registry.GetRegistryValue(hive, view, subKey, name).value);
            });
        }

        #endregion

        private RegistrySnapshotter CreateSnapshotter()
        {
            return new RegistrySnapshotter(registry, new DirectoryInfo(Path.Join(TestContext.ResultsDirectory, "Persistence")));
        }
    }
}
