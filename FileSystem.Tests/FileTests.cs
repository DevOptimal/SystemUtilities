using System.Text;

namespace bradselw.System.Resources.FileSystem.Tests
{
    [TestClass]
    public class FileTests
    {
        private MockFileSystemProxy proxy;

        [TestInitialize]
        public void TestInitialize()
        {
            proxy = new MockFileSystemProxy();
        }


        [TestMethod]
        public void CorrectlyIdentifiesExistentAndNonexistentFiles()
        {
            var path = @"C:\temp\foo.bar";

            Assert.IsFalse(proxy.FileExists(path));

            proxy.CreateFile(path);

            Assert.IsTrue(proxy.FileExists(path));

            proxy.DeleteFile(path);

            Assert.IsFalse(proxy.FileExists(path));
        }

        [TestMethod]
        public void CorrectlyWritesFile()
        {
            var path = Path.GetFullPath(@"C:\temp\foo.bar");
            var expectedBytes = Encoding.UTF8.GetBytes("testing");

            using (var stream = proxy.OpenFile(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(expectedBytes);
            }

            var actualBytes = proxy.fileSystem[path];
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        [TestMethod]
        public void CorrectlyReadsFile()
        {
            var path = Path.GetFullPath(@"C:\temp\foo.bar");
            var expectedBytes = Encoding.UTF8.GetBytes("testing");
            proxy.fileSystem[path] = expectedBytes;

            var actualBytes = new byte[expectedBytes.Length];
            using (var stream = proxy.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Read(actualBytes, 0, expectedBytes.Length);
            }

            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }
    }
}