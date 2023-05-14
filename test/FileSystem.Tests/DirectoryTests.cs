using System;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Tests
{
    [TestClass]
    public class DirectoryTests
    {
        private MockFileSystem fileSystem;

        private const string path = @"C:\foo";

        [TestInitialize]
        public void TestInitialize()
        {
            fileSystem = new MockFileSystem();
        }

        [TestMethod]
        public void IdentifiedNonexistentDirectories()
        {
            Assert.IsFalse(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void IdentifiedExistentDirectories()
        {
            fileSystem.data[path] = null;

            Assert.IsTrue(fileSystem.DirectoryExists(path));
        }

        [TestMethod]
        public void CreatesDirectory()
        {
            fileSystem.CreateDirectory(path);

            Assert.IsTrue(fileSystem.data.ContainsKey(path));
            Assert.AreEqual(null, fileSystem.data[path]);
        }

        [TestMethod]
        public void DeletesDirectory()
        {
            fileSystem.data[path] = null;

            fileSystem.DeleteDirectory(path, recursive: true);

            Assert.IsFalse(fileSystem.data.ContainsKey(path));
        }

        [TestMethod]
        public void GetsDirectoriesNonRecursively()
        {
            fileSystem.data[path] = null;
            fileSystem.data[Path.Combine(path, "bar")] = null;
            fileSystem.data[Path.Combine(path, "bar", "baz")] = null;

            Assert.AreEqual(1, fileSystem.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).Length);
        }

        [TestMethod]
        public void GetsDirectoriesRecursively()
        {
            fileSystem.data[path] = null;
            fileSystem.data[Path.Combine(path, "bar")] = null;
            fileSystem.data[Path.Combine(path, "bar", "baz")] = null;

            Assert.AreEqual(2, fileSystem.GetDirectories(path, "*", SearchOption.AllDirectories).Length);
        }

        [TestMethod]
        public void GetsFilesNonRecursively()
        {
            fileSystem.data[path] = null;
            fileSystem.data[Path.Combine(path, "bar.txt")] = Array.Empty<byte>();
            fileSystem.data[Path.Combine(path, "bar", "baz.txt")] = Array.Empty<byte>();

            Assert.AreEqual(1, fileSystem.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length);
        }

        [TestMethod]
        public void GetsFilesRecursively()
        {
            fileSystem.data[path] = null;
            fileSystem.data[Path.Combine(path, "bar.txt")] = Array.Empty<byte>();
            fileSystem.data[Path.Combine(path, "bar", "baz.txt")] = Array.Empty<byte>();

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
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowsOnSearchPatternEndingWithTwoDots()
        {
            fileSystem.CreateDirectory(@"C:\foo\bar");

            _ = fileSystem.GetDirectories(@"C:\foo", "*..", SearchOption.AllDirectories);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowsOnSearchPatternWithInvalidPathChars()
        {
            fileSystem.CreateDirectory(@"C:\foo\bar");

            _ = fileSystem.GetDirectories(@"C:\foo", "*bar|", SearchOption.AllDirectories);
        }
    }
}
