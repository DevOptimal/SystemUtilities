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
            var subKey = @"SOTFWARE\Microsoft\Windows";

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
            var subKey = @"SOTFWARE\Microsoft\Windows";

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
            var subKey = @"SOTFWARE\Microsoft\Windows";

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
            var subKey = @"SOTFWARE\Microsoft\Windows";

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
            var subKey = @"SOTFWARE\Microsoft\Windows";

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
            var subKey = @"SOTFWARE\Microsoft\Windows";

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

        [TestMethod]
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        public void RestoresAbandonedRegistryKeySnapshots()
        {
            using var restoreSnapshotter = CreateSnapshotter();
            var registryHive = RegistryHive.LocalMachine;
            var registryView = RegistryView.Default;
            var registrySubKey = @"SOFTWARE\Microsoft\StrongName\Verification";

            /*
             * First, we will test restoring a registry key that didn't exist when the snapshot was taken, but has since been created
             */
            // Delete the registry key, if it exists
            if (registry.RegistryKeyExists(registryHive, registryView, registrySubKey))
            {
                registry.DeleteRegistryKey(registryHive, registryView, registrySubKey, recursive: true);
            }

            // Simulate taking a snapshot of the registry key from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateSnapshotter();
                snapshotter.SnapshotRegistryKey(registryHive, registryView, registrySubKey);
            }

            // Create the registry key
            registry.CreateRegistryKey(registryHive, registryView, registrySubKey);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the registry key has been deleted
            Assert.IsFalse(registry.RegistryKeyExists(registryHive, registryView, registrySubKey));

            /*
             * Next, we will test restoring a registry key that did exist when the snapshot was taken, but has since been deleted
             */
            // Create the registry key
            registry.CreateRegistryKey(registryHive, registryView, registrySubKey);

            // Simulate taking a snapshot of the registry key from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateSnapshotter();
                snapshotter.SnapshotRegistryKey(registryHive, registryView, registrySubKey);
            }

            // Delete the registry key
            registry.DeleteRegistryKey(registryHive, registryView, registrySubKey, recursive: true);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the registry key has been created
            Assert.IsTrue(registry.RegistryKeyExists(registryHive, registryView, registrySubKey));
        }

        [TestMethod]
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        public void RestoresAbandonedRegistryValueSnapshots()
        {
            using var restoreSnapshotter = CreateSnapshotter();
            // Create registry key
            var registryHive = RegistryHive.LocalMachine;
            var registryView = RegistryView.Default;
            var registrySubKey = @"SOFTWARE\Microsoft\StrongName\Verification";
            registry.CreateRegistryKey(registryHive, registryView, registrySubKey);

            // One registry key value for each supported type
            var stringRegistryValueName = RegistryValueKind.String.ToString();
            var stringRegistryValueExpectedValue = "foo";
            var stringRegistryValueExpectedKind = RegistryValueKind.String;
            var expandStringRegistryValueName = RegistryValueKind.ExpandString.ToString();
            var expandStringRegistryValueExpectedValue = "bar";
            var expandStringRegistryValueExpectedKind = RegistryValueKind.ExpandString;
            var binaryRegistryValueName = RegistryValueKind.Binary.ToString();
            var binaryRegistryValueExpectedValue = Encoding.UTF8.GetBytes("Hello, world!");
            var binaryRegistryValueExpectedKind = RegistryValueKind.Binary;
            var dwordRegistryValueName = RegistryValueKind.DWord.ToString();
            var dwordRegistryValueExpectedValue = (int)3;
            var dwordRegistryValueExpectedKind = RegistryValueKind.DWord;
            var qwordRegistryValueName = RegistryValueKind.QWord.ToString();
            var qwordRegistryValueExpectedValue = (long)3;
            var qwordRegistryValueExpectedKind = RegistryValueKind.QWord;
            var multiStringRegistryValueName = RegistryValueKind.MultiString.ToString();
            var multiStringRegistryValueExpectedValue = new[] { "hello", "world" };
            var multiStringRegistryValueExpectedKind = RegistryValueKind.MultiString;

            /*
             * First, we will test restoring values that didn't exist when the snapshot was taken, but have since been created
             */
            // Delete the registry values, if they exist
            if (registry.RegistryValueExists(registryHive, registryView, registrySubKey, stringRegistryValueName))
            {
                registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, stringRegistryValueName);
            }
            if (registry.RegistryValueExists(registryHive, registryView, registrySubKey, expandStringRegistryValueName))
            {
                registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, expandStringRegistryValueName);
            }
            if (registry.RegistryValueExists(registryHive, registryView, registrySubKey, binaryRegistryValueName))
            {
                registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, binaryRegistryValueName);
            }
            if (registry.RegistryValueExists(registryHive, registryView, registrySubKey, dwordRegistryValueName))
            {
                registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, dwordRegistryValueName);
            }
            if (registry.RegistryValueExists(registryHive, registryView, registrySubKey, qwordRegistryValueName))
            {
                registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, qwordRegistryValueName);
            }
            if (registry.RegistryValueExists(registryHive, registryView, registrySubKey, multiStringRegistryValueName))
            {
                registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, multiStringRegistryValueName);
            }

            // Simulate taking snapshots of the registry values from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateSnapshotter();
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, stringRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, expandStringRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, binaryRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, dwordRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, qwordRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, multiStringRegistryValueName);
            }

            // Create the registry values
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, stringRegistryValueName, stringRegistryValueExpectedValue, stringRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, expandStringRegistryValueName, expandStringRegistryValueExpectedValue, expandStringRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, binaryRegistryValueName, binaryRegistryValueExpectedValue, binaryRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, dwordRegistryValueName, dwordRegistryValueExpectedValue, dwordRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, qwordRegistryValueName, qwordRegistryValueExpectedValue, qwordRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, multiStringRegistryValueName, multiStringRegistryValueExpectedValue, multiStringRegistryValueExpectedKind);

            // Restore the snapshots
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the registry values have been deleted
            Assert.IsFalse(registry.RegistryValueExists(registryHive, registryView, registrySubKey, stringRegistryValueName));
            Assert.IsFalse(registry.RegistryValueExists(registryHive, registryView, registrySubKey, expandStringRegistryValueName));
            Assert.IsFalse(registry.RegistryValueExists(registryHive, registryView, registrySubKey, binaryRegistryValueName));
            Assert.IsFalse(registry.RegistryValueExists(registryHive, registryView, registrySubKey, dwordRegistryValueName));
            Assert.IsFalse(registry.RegistryValueExists(registryHive, registryView, registrySubKey, qwordRegistryValueName));
            Assert.IsFalse(registry.RegistryValueExists(registryHive, registryView, registrySubKey, multiStringRegistryValueName));

            /*
             * Next, we will test restoring values that did exist when the snapshot was taken, but have since been deleted
             */
            // Create a bunch of registry values
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, stringRegistryValueName, stringRegistryValueExpectedValue, stringRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, expandStringRegistryValueName, expandStringRegistryValueExpectedValue, expandStringRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, binaryRegistryValueName, binaryRegistryValueExpectedValue, binaryRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, dwordRegistryValueName, dwordRegistryValueExpectedValue, dwordRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, qwordRegistryValueName, qwordRegistryValueExpectedValue, qwordRegistryValueExpectedKind);
            registry.SetRegistryValue(registryHive, registryView, registrySubKey, multiStringRegistryValueName, multiStringRegistryValueExpectedValue, multiStringRegistryValueExpectedKind);

            // Simulate taking snapshots of the registry values from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateSnapshotter();
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, stringRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, expandStringRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, binaryRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, dwordRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, qwordRegistryValueName);
                snapshotter.SnapshotRegistryValue(registryHive, registryView, registrySubKey, multiStringRegistryValueName);
            }

            // Delete the registry values
            registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, stringRegistryValueName);
            registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, expandStringRegistryValueName);
            registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, binaryRegistryValueName);
            registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, dwordRegistryValueName);
            registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, qwordRegistryValueName);
            registry.DeleteRegistryValue(registryHive, registryView, registrySubKey, multiStringRegistryValueName);

            // Restore the snapshots
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the registry values have been restored
            var (stringRegistryValueActualValue, stringRegistryValueActualKind) = registry.GetRegistryValue(registryHive, registryView, registrySubKey, stringRegistryValueName);
            Assert.AreEqual(stringRegistryValueExpectedValue, stringRegistryValueActualValue);
            Assert.AreEqual(stringRegistryValueExpectedKind, stringRegistryValueActualKind);
            var (expandStringRegistryValueActualValue, expandStringRegistryValueActualKind) = registry.GetRegistryValue(registryHive, registryView, registrySubKey, expandStringRegistryValueName);
            Assert.AreEqual(expandStringRegistryValueExpectedValue, expandStringRegistryValueActualValue);
            Assert.AreEqual(expandStringRegistryValueExpectedKind, expandStringRegistryValueActualKind);
            var (binaryRegistryValueActualValue, binaryRegistryValueActualKind) = registry.GetRegistryValue(registryHive, registryView, registrySubKey, binaryRegistryValueName);
            CollectionAssert.AreEqual(binaryRegistryValueExpectedValue, (byte[])binaryRegistryValueActualValue);
            Assert.AreEqual(binaryRegistryValueExpectedKind, binaryRegistryValueActualKind);
            var (dwordRegistryValueActualValue, dwordRegistryValueActualKind) = registry.GetRegistryValue(registryHive, registryView, registrySubKey, dwordRegistryValueName);
            Assert.AreEqual(dwordRegistryValueExpectedValue, dwordRegistryValueActualValue);
            Assert.AreEqual(dwordRegistryValueExpectedKind, dwordRegistryValueActualKind);
            var (qwordRegistryValueActualValue, qwordRegistryValueActualKind) = registry.GetRegistryValue(registryHive, registryView, registrySubKey, qwordRegistryValueName);
            Assert.AreEqual(qwordRegistryValueExpectedValue, qwordRegistryValueActualValue);
            Assert.AreEqual(qwordRegistryValueExpectedKind, qwordRegistryValueActualKind);
            var (multiStringRegistryValueActualValue, multiStringRegistryValueActualKind) = registry.GetRegistryValue(registryHive, registryView, registrySubKey, multiStringRegistryValueName);
            CollectionAssert.AreEqual(multiStringRegistryValueExpectedValue, (object[])multiStringRegistryValueActualValue);
            Assert.AreEqual(multiStringRegistryValueExpectedKind, multiStringRegistryValueActualKind);
        }

        #endregion

        private RegistrySnapshotter CreateSnapshotter()
        {
            return new RegistrySnapshotter(registry, new DirectoryInfo(Path.Join(TestContext.ResultsDirectory, "Persistence")));
        }
    }
}
