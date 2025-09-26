using DevOptimal.SystemUtilities.FileSystem.StateManagement;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.StateManagement
{
    [TestClass]
    public class FileSystemSnapshotterTests : MockFileSystemTestBase
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void RevertsDirectoryCreation()
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
        public void RevertsDirectoryCreationWithChildren()
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
        public void RevertsDirectoryDeletion()
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
        public void RevertsFileAlteration()
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
        public void RevertsFileCreation()
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
        public void RevertsFileDeletion()
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
        public void RevertsMultipleFileDeletionsWithSameContent()
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

        #region Persistence Tests

        [TestMethod]
        public void ConcurrentlySnapshotsDirectories()
        {
            var concurrentThreads = 100;

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
            var concurrentThreads = 100;

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
