namespace bradselw.System.Resources.FileSystem.Tests
{
    [TestClass]
    public class DirectoryTests
    {
        private MockFileSystemProxy proxy;

        private const string path = @"C:\foo\bar";

        [TestInitialize]
        public void TestInitialize()
        {
            proxy = new MockFileSystemProxy();
        }

        [TestMethod]
        public void IdentifiedNonexistentDirectories()
        {
            Assert.IsFalse(proxy.DirectoryExists(path));
        }

        [TestMethod]
        public void IdentifiedExistentDirectories()
        {
            proxy.fileSystem[path] = null;

            Assert.IsTrue(proxy.DirectoryExists(path));
        }

        [TestMethod]
        public void CreatesDirectory()
        {
            proxy.CreateDirectory(path);

            Assert.IsTrue(proxy.fileSystem.ContainsKey(path));
            Assert.AreEqual(null, proxy.fileSystem[path]);
        }

        [TestMethod]
        public void DeletesDirectory()
        {
            proxy.fileSystem[path] = null;

            proxy.DeleteDirectory(path, recursive: true);

            Assert.IsFalse(proxy.fileSystem.ContainsKey(path));
        }
    }
}
