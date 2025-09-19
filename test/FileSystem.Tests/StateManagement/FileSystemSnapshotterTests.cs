using DevOptimal.SystemUtilities.FileSystem.StateManagement;
using System;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.StateManagement
{
    [TestClass]
    public class FileSystemSnapshotterTests : MockFileSystemTestBase
    {
        [TestMethod]
        public void RevertsDirectoryCreation()
        {
            var path = @"C:\foo\bar";

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotDirectory(path))
            {
                fileSystem.CreateDirectory(path);

                Assert.IsTrue(fileSystem.DirectoryExists(path));
            }

            Assert.IsFalse(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void RevertsDirectoryCreationWithChildren()
        {
            var path = @"C:\foo\bar";

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotDirectory(path))
            {
                fileSystem.CreateDirectory(path);
                fileSystem.CreateDirectory(Path.Combine(path, "blah"));
                fileSystem.CreateFile(Path.Combine(path, "log.txt"));
            }

            Assert.IsFalse(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void RevertsDirectoryDeletion()
        {
            var path = @"C:\foo\bar";
            fileSystem.CreateDirectory(path);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotDirectory(path))
            {
                fileSystem.DeleteDirectory(path, recursive: true);

                Assert.IsFalse(fileSystem.DirectoryExists(path));
            }

            Assert.IsTrue(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void RevertsFileAlteration()
        {
            var path = @"C:\foo\bar.dat";
            var expectedFileBytes = Guid.NewGuid().ToByteArray();
            WriteBytes(path, expectedFileBytes);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotFile(path))
            {
                WriteBytes(path, Guid.NewGuid().ToByteArray());
            }

            CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path));
        }

        [TestMethod]
        public void RevertsFileCreation()
        {
            var path = @"C:\foo\bar.dat";

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotFile(path))
            {
                fileSystem.CreateFile(path);
            }

            Assert.IsFalse(fileSystem.FileExists(path));
        }

        [TestMethod]
        public void RevertsFileDeletion()
        {
            var path = @"C:\foo\bar.dat";
            var expectedFileBytes = Guid.NewGuid().ToByteArray();
            WriteBytes(path, expectedFileBytes);

            using var systemStateManager = CreateSnapshotter();
            using (systemStateManager.SnapshotFile(path))
            {
                fileSystem.DeleteFile(path);
            }

            CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path));
        }

        [TestMethod]
        public void RevertsMultipleFileDeletionsWithSameContent()
        {
            var expectedFileBytes = Guid.NewGuid().ToByteArray();

            var path = @"C:\foo\bar.dat";
            WriteBytes(path, expectedFileBytes);

            var path2 = @"C:\foo\baz.dat";
            WriteBytes(path2, expectedFileBytes);

            using var systemStateManager = CreateSnapshotter();
            using (var snapshot = systemStateManager.SnapshotFile(path))
            {
                fileSystem.DeleteFile(path);
                Assert.IsFalse(fileSystem.FileExists(path));

                using (var snapshot2 = systemStateManager.SnapshotFile(path2))
                {
                    fileSystem.DeleteFile(path2);
                    Assert.IsFalse(fileSystem.FileExists(path2));
                }

                CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path2));
            }

            CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path));
        }

        private FileSystemSnapshotter CreateSnapshotter()
        {
            return new FileSystemSnapshotter(fileSystem);
        }

        private void WriteBytes(string path, byte[] bytes)
        {
            using var stream = fileSystem.OpenFile(path, FileMode.Create, FileAccess.Write, FileShare.None);

            stream.Write(bytes, 0, bytes.Length);
        }

        private byte[] ReadBytes(string path)
        {
            using var stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.None);
            var result = new byte[stream.Length];
            stream.ReadExactly(result);
            return result;
        }
    }
}
