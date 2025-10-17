﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using System.Xml.Linq;

namespace DevOptimal.SystemUtilities.Common.Tests.StateManagement
{
    [TestClass]
    public class RestoreAbandonedSnapshotsTests : TestBase
    {
        [TestMethod]
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        public void RestoresAbandonedEnvironmentVariableSnapshots()
        {
            using var restoreSnapshotter = CreateEnvironmentSnapshotter();

            /*
             * First, we will test restoring an environment variable that didn't exist when the snapshot was taken, but has since been created
             */
            // Delete the environment variable
            environment.SetEnvironmentVariable(name, null, target);

            // Simulate taking a snapshot of the environment variable from another process
            using (CreateShimsContext())
            {
                var systemStateManager = CreateEnvironmentSnapshotter();
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
                var systemStateManager = CreateEnvironmentSnapshotter();
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
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        public void DoesNotRestoreProcessScopedEnvironmentVariableSnapshots()
        {
            using var restoreSnapshotter = CreateEnvironmentSnapshotter();
            var target = EnvironmentVariableTarget.Process;

            // Create the environment variable
            environment.SetEnvironmentVariable(name, expectedValue, target);

            // Simulate taking a snapshot of the environment variable from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateEnvironmentSnapshotter();
                snapshotter.SnapshotEnvironmentVariable(name, target);
            }

            // Delete the environment variable
            environment.SetEnvironmentVariable(name, null, target);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the environment variable has not been recreated
            Assert.IsNull(environment.GetEnvironmentVariable(name, target));
        }

        [TestMethod]
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        public void RestoresAbandonedDirectorySnapshots()
        {
            using var restoreSnapshotter = CreateFileSystemSnapshotter();
            var directoryPath = @"C:\foo";

            /*
             * First, we will test restoring a directory that didn't exist when the snapshot was taken, but has since been created
             */
            // Delete the directory, if it exists
            if (fileSystem.DirectoryExists(directoryPath))
            {
                fileSystem.DeleteDirectory(directoryPath, recursive: true);
            }

            // Simulate taking a snapshot of the directory from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateFileSystemSnapshotter();
                snapshotter.SnapshotDirectory(directoryPath);
            }

            // Create the directory
            fileSystem.CreateDirectory(directoryPath);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the directory has been deleted
            Assert.IsFalse(fileSystem.DirectoryExists(directoryPath));

            /*
             * Next, we will test restoring a directory that did exist when the snapshot was taken, but has since been deleted
             */
            // Create the directory
            fileSystem.CreateDirectory(directoryPath);

            // Simulate taking a snapshot of the directory from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateFileSystemSnapshotter();
                snapshotter.SnapshotDirectory(directoryPath);
            }

            // Delete the directory
            fileSystem.DeleteDirectory(directoryPath, recursive: true);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the directory has been created
            Assert.IsTrue(fileSystem.DirectoryExists(directoryPath));
        }

        [TestMethod]
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        public void RestoresAbandonedFileSnapshots()
        {
            using var restoreSnapshotter = CreateFileSystemSnapshotter();
            var filePath = @"C:\foo\bar.txt";
            var expectedFileBytes = Encoding.UTF8.GetBytes("Hello, world!");

            /*
             * First, we will test restoring a file that didn't exist when the snapshot was taken, but has since been created
             */
            // Delete the file, if it exists
            if (fileSystem.FileExists(filePath))
            {
                fileSystem.DeleteFile(filePath);
            }

            // Simulate taking a snapshot of the file from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateFileSystemSnapshotter();
                snapshotter.SnapshotFile(filePath);
            }

            // Create the file
            using (var stream = fileSystem.OpenFile(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(expectedFileBytes);
            }

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the file has been deleted
            Assert.IsFalse(fileSystem.FileExists(filePath));

            /*
             * Next, we will test restoring a file that did exist when the snapshot was taken, but has since been deleted
             */
            // Create the file
            using (var stream = fileSystem.OpenFile(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(expectedFileBytes);
            }

            // Simulate taking a snapshot of the file from another process
            using (CreateShimsContext())
            {
                var snapshotter = CreateFileSystemSnapshotter();
                snapshotter.SnapshotFile(filePath);
            }

            // Delete the file
            fileSystem.DeleteFile(filePath);

            // Restore the snapshot
            restoreSnapshotter.RestoreAbandonedSnapshots();

            // Verify that the file has been created
            var actualFileBytes = new byte[expectedFileBytes.Length];
            using (var stream = fileSystem.OpenFile(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.ReadExactly(actualFileBytes, 0, expectedFileBytes.Length);
            }
            CollectionAssert.AreEqual(expectedFileBytes, actualFileBytes);
        }

        [TestMethod]
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        [SupportedOSPlatform("windows")]
        public void RestoresAbandonedRegistryKeySnapshots()
        {
            using var restoreSnapshotter = CreateRegistrySnapshotter();
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
                var snapshotter = CreateRegistrySnapshotter();
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
                var snapshotter = CreateRegistrySnapshotter();
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
        [SupportedOSPlatform("windows")]
        public void RestoresAbandonedRegistryValueSnapshots()
        {
            using var restoreSnapshotter = CreateRegistrySnapshotter();
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
                var snapshotter = CreateRegistrySnapshotter();
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
                var snapshotter = CreateRegistrySnapshotter();
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
    }
}
