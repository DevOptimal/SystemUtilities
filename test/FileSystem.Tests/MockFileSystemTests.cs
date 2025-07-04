using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOptimal.SystemUtilities.FileSystem.Tests
{
    [TestClass]
    public class MockFileSystemTests
    {
        private MockFileSystem fileSystem;

        [TestInitialize]
        public void TestInitialize()
        {
            fileSystem = new MockFileSystem();
        }


        [TestMethod]
        public void IdentifiesExistentAndNonexistentFiles()
        {
            var path = @"C:\temp\foo.bar";

            Assert.IsFalse(fileSystem.FileExists(path));

            fileSystem.CreateFile(path);

            Assert.IsTrue(fileSystem.FileExists(path));

            fileSystem.DeleteFile(path);

            Assert.IsFalse(fileSystem.FileExists(path));
        }

        [TestMethod]
        public void WritesFile()
        {
            var path = Path.GetFullPath(@"C:\temp\foo.bar");
            var expectedBytes = Encoding.UTF8.GetBytes("testing");

            using (var stream = fileSystem.OpenFile(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(expectedBytes);
            }

            var actualBytes = fileSystem.data[path];
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        [TestMethod]
        public void ReadsFile()
        {
            var path = Path.GetFullPath(@"C:\temp\foo.bar");
            var expectedBytes = Encoding.UTF8.GetBytes("testing").ToList();
            fileSystem.data[path] = expectedBytes;

            var actualBytes = new byte[expectedBytes.Count];
            using (var stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.ReadExactly(actualBytes);
            }

            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        [TestMethod]
        public void ConcurrentReads()
        {
            var path = Path.GetFullPath(@"C:\temp\foo.bar");
            var expectedBytes = Encoding.UTF8.GetBytes("testing").ToList();
            fileSystem.data[path] = expectedBytes;

            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var actualBytes = new byte[expectedBytes.Count];
                    using (var stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        stream.ReadExactly(actualBytes);
                    }

                    CollectionAssert.AreEqual(expectedBytes, actualBytes);
                }));
            }

            Task.WaitAll([.. tasks]);
        }

        [TestMethod]
        public void IdentifiesExistentAndNonexistentDirectories()
        {
            var path = @"C:\temp\foo";

            Assert.IsFalse(fileSystem.DirectoryExists(path));

            fileSystem.CreateDirectory(path);

            Assert.IsTrue(fileSystem.DirectoryExists(path));

            fileSystem.DeleteDirectory(path, recursive: false);

            Assert.IsFalse(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void CreatesDirectory()
        {
            var path = @"C:\temp\foo";

            fileSystem.CreateDirectory(path);

            Assert.IsTrue(fileSystem.data.ContainsKey(path));
            Assert.IsNull(fileSystem.data[path]);
        }

        [TestMethod]
        public void DeletesDirectory()
        {
            var path = @"C:\temp\foo";

            fileSystem.data[path] = null;

            fileSystem.DeleteDirectory(path, recursive: true);

            Assert.IsFalse(fileSystem.data.ContainsKey(path));
        }

        [TestMethod]
        public void GetsDirectoriesNonRecursively()
        {
            var path = @"C:\temp\foo";

            fileSystem.data[path] = null;
            fileSystem.data[Path.Combine(path, "bar")] = null;
            fileSystem.data[Path.Combine(path, "bar", "baz")] = null;

            Assert.AreEqual(1, fileSystem.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).Length);
        }

        [TestMethod]
        public void GetsDirectoriesRecursively()
        {
            var path = @"C:\temp\foo";

            fileSystem.data[path] = null;
            fileSystem.data[Path.Combine(path, "bar")] = null;
            fileSystem.data[Path.Combine(path, "bar", "baz")] = null;

            Assert.AreEqual(2, fileSystem.GetDirectories(path, "*", SearchOption.AllDirectories).Length);
        }

        [TestMethod]
        public void GetsFilesNonRecursively()
        {
            var path = @"C:\temp\foo";

            fileSystem.data[path] = null;
            fileSystem.data[Path.Combine(path, "bar.txt")] = [];
            fileSystem.data[Path.Combine(path, "bar", "baz.txt")] = [];

            Assert.AreEqual(1, fileSystem.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length);
        }

        [TestMethod]
        public void GetsFilesRecursively()
        {
            var path = @"C:\temp\foo";

            fileSystem.data[path] = null;
            fileSystem.data[Path.Combine(path, "bar.txt")] = [];
            fileSystem.data[Path.Combine(path, "bar", "baz.txt")] = [];

            Assert.AreEqual(2, fileSystem.GetFiles(path, "*", SearchOption.AllDirectories).Length);
        }

        [TestMethod]
        public void CreatesDirectoriesRecursively()
        {
            fileSystem.CreateDirectory(@"C:\foo\bar");

            Assert.IsTrue(fileSystem.DirectoryExists(@"C:"));
            Assert.IsTrue(fileSystem.DirectoryExists(@"C:\foo"));
            Assert.IsTrue(fileSystem.DirectoryExists(@"C:\foo\bar"));
        }

        [TestMethod]
        public void SearchPatternIsAppliedCorrectly()
        {
            fileSystem.CreateDirectory(@"C:\foo\bar");
            fileSystem.CreateDirectory(@"C:\foo\baz");
            fileSystem.CreateDirectory(@"C:\foo\baz\bar");
            fileSystem.CreateFile(@"C:\foo\bar.txt");
            fileSystem.CreateFile(@"C:\foo\baz.txt");
            fileSystem.CreateFile(@"C:\foo\baz\bar.txt");

            Assert.AreEqual(2, fileSystem.GetDirectories(@"C:\foo", "*bar*", SearchOption.AllDirectories).Length);
            Assert.AreEqual(2, fileSystem.GetFiles(@"C:\foo", "*bar*", SearchOption.AllDirectories).Length);
        }

        [TestMethod]
        public void ThrowsOnSearchPatternEndingWithTwoDots()
        {
            fileSystem.CreateDirectory(@"C:\foo\bar");

            Assert.ThrowsExactly<ArgumentException>(() => _ = fileSystem.GetDirectories(@"C:\foo", "*..", SearchOption.AllDirectories));
        }

        [TestMethod]
        public void ThrowsOnSearchPatternWithInvalidPathChars()
        {
            fileSystem.CreateDirectory(@"C:\foo\bar");

            Assert.ThrowsExactly<ArgumentException>(() => _ = fileSystem.GetDirectories(@"C:\foo", "*bar|", SearchOption.AllDirectories));
        }

        [TestMethod]
        public void HardLinksFileCorrectly()
        {
            var expectedBytes = Encoding.UTF8.GetBytes("testing");

            var sourcePath = Path.GetFullPath(@"C:\temp\foo.txt");
            using (var stream = fileSystem.OpenFile(sourcePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(expectedBytes);
            }
            CollectionAssert.AreEqual(expectedBytes, fileSystem.data[sourcePath]);

            var destinationPath = Path.GetFullPath(@"C:\temp\bar.txt");
            fileSystem.HardLinkFile(sourcePath, destinationPath, false);
            CollectionAssert.AreEqual(expectedBytes, fileSystem.data[destinationPath]);

            expectedBytes = Encoding.UTF8.GetBytes("newtesting");
            using (var stream = fileSystem.OpenFile(sourcePath, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                stream.Write(expectedBytes);
            }
            CollectionAssert.AreEqual(expectedBytes, fileSystem.data[destinationPath]);
        }
    }
}