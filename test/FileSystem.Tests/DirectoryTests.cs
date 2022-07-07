namespace DevOptimal.SystemUtilities.FileSystem.Tests
{
    [TestClass]
    public class DirectoryTests
    {
        private MockFileSystem fileSystem;

        private const string path = @"C:\foo\bar";

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
    }
}
