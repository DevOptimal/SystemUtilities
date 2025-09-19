using DevOptimal.SystemUtilities.Registry.StateManagement;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Versioning;

namespace DevOptimal.SystemUtilities.Registry.Tests.StateManagement
{
    [TestClass]
    [SupportedOSPlatform("windows")]
    public class RegistrySnapshotterTests : MockRegistryTestBase
    {
        [TestMethod]
        public void RevertsRegistryKeyCreation()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOTFWARE\Microsoft\Windows";

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotRegistryKey(hive, view, subKey))
            {
                registry.CreateRegistryKey(hive, view, subKey);
            }

            Assert.IsFalse(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void RevertsRegistryKeyCreationWithChildren()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOTFWARE\Microsoft\Windows";

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotRegistryKey(hive, view, subKey))
            {
                registry.CreateRegistryKey(hive, view, subKey);
                registry.CreateRegistryKey(hive, view, Path.Combine(subKey, "foo"));
                registry.SetRegistryValue(hive, view, subKey, "bar", "Hello, world!", RegistryValueKind.String);
            }

            Assert.IsFalse(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void RevertsRegistryKeyDeletion()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOTFWARE\Microsoft\Windows";

            registry.CreateRegistryKey(hive, view, subKey);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotRegistryKey(hive, view, subKey))
            {
                registry.DeleteRegistryKey(hive, view, subKey, recursive: true);
            }

            Assert.IsTrue(registry.RegistryKeyExists(hive, view, subKey));
        }

        [TestMethod]
        public void RevertsRegistryValueAlteration()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var name = "foo";
            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, name, value, kind);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotRegistryValue(hive, view, subKey, name))
            {
                registry.SetRegistryValue(hive, view, subKey, name, 10, RegistryValueKind.DWord);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, name));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, name);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void RevertsRegistryValueCreation()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var name = "foo";

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotRegistryValue(hive, view, subKey, name))
            {
                registry.SetRegistryValue(hive, view, subKey, name, "bar", RegistryValueKind.String);
            }

            Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, name));
        }

        [TestMethod]
        public void RevertsRegistryValueDeletion()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var name = "foo";
            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, name, value, kind);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotRegistryValue(hive, view, subKey, name))
            {
                registry.DeleteRegistryValue(hive, view, subKey, name);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, name));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, name);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void RevertsDefaultRegistryValueAlteration()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, null, value, kind);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotRegistryValue(hive, view, subKey, null))
            {
                registry.SetRegistryValue(hive, view, subKey, null, 10, RegistryValueKind.DWord);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, null));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, null);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        [TestMethod]
        public void RevertsDefaultRegistryValueCreation()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotRegistryValue(hive, view, subKey, null))
            {
                registry.SetRegistryValue(hive, view, subKey, null, "bar", RegistryValueKind.String);
            }

            Assert.IsFalse(registry.RegistryValueExists(hive, view, subKey, null));
        }

        [TestMethod]
        public void RevertsDefaultRegistryValueDeletion()
        {
            var hive = RegistryHive.LocalMachine;
            var view = RegistryView.Default;
            var subKey = @"SOFTWARE\Microsoft\Windows";
            registry.CreateRegistryKey(hive, view, subKey);

            var value = "bar";
            var kind = RegistryValueKind.String;
            registry.SetRegistryValue(hive, view, subKey, null, value, kind);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotRegistryValue(hive, view, subKey, null))
            {
                registry.DeleteRegistryValue(hive, view, subKey, null);
            }

            Assert.IsTrue(registry.RegistryValueExists(hive, view, subKey, null));
            var (actualValue, actualKind) = registry.GetRegistryValue(hive, view, subKey, null);
            Assert.AreEqual(value, actualValue);
            Assert.AreEqual(kind, actualKind);
        }

        private RegistrySnapshotter CreateSnapshotter()
        {
            return new RegistrySnapshotter(registry);
        }
    }
}
