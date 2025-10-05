using DevOptimal.SystemUtilities.FileSystem.Extensions;
using DevOptimal.SystemUtilities.FileSystem.StateManagement;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.StateManagement
{
    [TestClass]
    public class FileSystemSnapshotterTests : MockFileSystemTestBase
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Snapshot_RevertsDirectoryCreation()
        {
            var path = @"C:\foo\bar";

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotDirectory(path))
            {
                fileSystem.CreateDirectory(path);

                Assert.IsTrue(fileSystem.DirectoryExists(path));
            }

            Assert.IsFalse(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void Snapshot_RevertsDirectoryCreationWithChildren()
        {
            var path = @"C:\foo\bar";

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotDirectory(path))
            {
                fileSystem.CreateDirectory(path);
                fileSystem.CreateDirectory(Path.Combine(path, "blah"));
                fileSystem.CreateFile(Path.Combine(path, "log.txt"));
            }

            Assert.IsFalse(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void Snapshot_RevertsDirectoryDeletion()
        {
            var path = @"C:\foo\bar";
            fileSystem.CreateDirectory(path);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotDirectory(path))
            {
                fileSystem.DeleteDirectory(path, recursive: true);

                Assert.IsFalse(fileSystem.DirectoryExists(path));
            }

            Assert.IsTrue(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void Snapshotter_RevertsDirectoryCreation()
        {
            var path = @"C:\foo\bar";

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotDirectory(path);
                fileSystem.CreateDirectory(path);

                Assert.IsTrue(fileSystem.DirectoryExists(path));
            }

            Assert.IsFalse(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void Snapshotter_RevertsDirectoryCreationWithChildren()
        {
            var path = @"C:\foo\bar";

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotDirectory(path);
                fileSystem.CreateDirectory(path);
                fileSystem.CreateDirectory(Path.Combine(path, "blah"));
                fileSystem.CreateFile(Path.Combine(path, "log.txt"));
            }

            Assert.IsFalse(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void Snapshotter_RevertsDirectoryDeletion()
        {
            var path = @"C:\foo\bar";
            fileSystem.CreateDirectory(path);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotDirectory(path);
                fileSystem.DeleteDirectory(path, recursive: true);

                Assert.IsFalse(fileSystem.DirectoryExists(path));
            }

            Assert.IsTrue(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void Snapshot_RevertsFileAlteration()
        {
            var path = @"C:\foo\bar.dat";
            var expectedFileBytes = Guid.NewGuid().ToByteArray();
            WriteBytes(path, expectedFileBytes);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotFile(path))
            {
                WriteBytes(path, Guid.NewGuid().ToByteArray());
            }

            CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path));
        }

        [TestMethod]
        public void Snapshot_RevertsFileCreation()
        {
            var path = @"C:\foo\bar.dat";

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotFile(path))
            {
                fileSystem.WriteAllTextToFile(path, "foo bar");
            }

            Assert.IsFalse(fileSystem.FileExists(path));
        }

        [TestMethod]
        public void Snapshot_RevertsFileDeletion()
        {
            var path = @"C:\foo\bar.dat";
            var expectedFileBytes = Guid.NewGuid().ToByteArray();
            WriteBytes(path, expectedFileBytes);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotFile(path))
            {
                fileSystem.DeleteFile(path);
            }

            CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path));
        }

        [TestMethod]
        public void Snapshot_RevertsEmptyFileCreation()
        {
            var path = @"C:\foo\bar.dat";

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotFile(path))
            {
                fileSystem.CreateFile(path);
            }

            Assert.IsFalse(fileSystem.FileExists(path));
        }

        [TestMethod]
        public void Snapshot_RevertsEmptyFileDeletion()
        {
            var path = @"C:\foo\bar.dat";
            var expectedFileBytes = Array.Empty<byte>();
            WriteBytes(path, expectedFileBytes);

            using var snapshotter = CreateSnapshotter();
            using (snapshotter.SnapshotFile(path))
            {
                fileSystem.DeleteFile(path);
            }

            CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path));
        }

        [TestMethod]
        public void Snapshot_RevertsMultipleFileDeletionsWithSameContent()
        {
            var expectedFileBytes = Guid.NewGuid().ToByteArray();

            var path = @"C:\foo\bar.dat";
            WriteBytes(path, expectedFileBytes);

            var path2 = @"C:\foo\baz.dat";
            WriteBytes(path2, expectedFileBytes);

            using var snapshotter = CreateSnapshotter();
            using (var snapshot = snapshotter.SnapshotFile(path))
            {
                fileSystem.DeleteFile(path);
                Assert.IsFalse(fileSystem.FileExists(path));

                using (var snapshot2 = snapshotter.SnapshotFile(path2))
                {
                    fileSystem.DeleteFile(path2);
                    Assert.IsFalse(fileSystem.FileExists(path2));
                }

                CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path2));
            }

            CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path));
        }

        [TestMethod]
        public void Snapshotter_RevertsFileAlteration()
        {
            var path = @"C:\foo\bar.dat";
            var expectedFileBytes = Guid.NewGuid().ToByteArray();
            WriteBytes(path, expectedFileBytes);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotFile(path);
                WriteBytes(path, Guid.NewGuid().ToByteArray());
            }

            CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path));
        }

        [TestMethod]
        public void Snapshotter_RevertsFileCreation()
        {
            var path = @"C:\foo\bar.dat";

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotFile(path);
                fileSystem.CreateFile(path);
            }

            Assert.IsFalse(fileSystem.FileExists(path));
        }

        [TestMethod]
        public void Snapshotter_RevertsFileDeletion()
        {
            var path = @"C:\foo\bar.dat";
            var expectedFileBytes = Guid.NewGuid().ToByteArray();
            WriteBytes(path, expectedFileBytes);

            using (var snapshotter = CreateSnapshotter())
            {
                snapshotter.SnapshotFile(path);
                fileSystem.DeleteFile(path);
            }

            CollectionAssert.AreEqual(expectedFileBytes, ReadBytes(path));
        }

        #region Persistence Tests

        [TestMethod]
        public void ConcurrentlySnapshotsDirectories()
        {
            var concurrentThreads = 10;

            var paths = new string[concurrentThreads];

            for (var i = 0; i < concurrentThreads; i++)
            {
                var path = $@"C:\file{i}";
                fileSystem.CreateDirectory(path);
                paths[i] = path;
            }

            Parallel.For(0, concurrentThreads, i =>
            {
                var path = paths[i];
                using var snapshotter = CreateSnapshotter();
                using (var caretaker = snapshotter.SnapshotDirectory(path))
                {
                    fileSystem.DeleteDirectory(path, recursive: true);
                    Assert.IsFalse(fileSystem.DirectoryExists(path));
                }

                Assert.IsTrue(fileSystem.DirectoryExists(path));
            });
        }

        [TestMethod]
        public void ConcurrentlySnapshotsFiles()
        {
            var concurrentThreads = 10;

            var paths = new string[concurrentThreads];
            var expectedContent = new byte[concurrentThreads][];

            for (var i = 0; i < concurrentThreads; i++)
            {
                var path = $@"C:\file{i}.txt";
                fileSystem.CreateFile(path);
                paths[i] = path;

                var content = Guid.NewGuid().ToByteArray();
                using var stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Write, FileShare.None);
                stream.Write(content, 0, content.Length);
                expectedContent[i] = content;
            }

            Parallel.For(0, concurrentThreads, i =>
            {
                var path = paths[i];
                using var snapshotter = CreateSnapshotter();
                using (var caretaker = snapshotter.SnapshotFile(path))
                {
                    fileSystem.DeleteFile(path);
                    Assert.IsFalse(fileSystem.FileExists(path));
                }

                Assert.IsTrue(fileSystem.FileExists(path));
                var content = expectedContent[i];
                using var stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.None);
                var readContent = new byte[content.Length];
                stream.ReadExactly(readContent);
                Assert.IsTrue(readContent.SequenceEqual(content));
            });
        }

        [TestMethod]
        [TestCategory("OmitFromCI")] // Fakes require Visual Studio Enterprise, but agent machines only have Community installed.
        public void RestoresAbandonedDirectorySnapshots()
        {
            using var restoreSnapshotter = CreateSnapshotter();
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
                var snapshotter = CreateSnapshotter();
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
                var snapshotter = CreateSnapshotter();
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
            using var restoreSnapshotter = CreateSnapshotter();
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
                var snapshotter = CreateSnapshotter();
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
                var snapshotter = CreateSnapshotter();
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

        #endregion

        private FileSystemSnapshotter CreateSnapshotter()
        {
            return new FileSystemSnapshotter(fileSystem, new DirectoryInfo(Path.Join(TestContext.ResultsDirectory, "Persistence")));
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
