using System.Text;

namespace bradselw.System.Resources.FileSystem.Tests
{
    [TestClass]
    public class FileTests
    {
        private MockFileSystemProxy proxyUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            proxyUnderTest = new MockFileSystemProxy();
        }

        [TestMethod]
        public void CorrectlyIdentifiesExistentAndNonexistentFiles()
        {
            var path = @"C:\temp\foo.bar";

            Assert.IsFalse(proxyUnderTest.FileExists(path));

            proxyUnderTest.CreateFile(path);

            Assert.IsTrue(proxyUnderTest.FileExists(path));

            proxyUnderTest.DeleteFile(path);

            Assert.IsFalse(proxyUnderTest.FileExists(path));
        }

        [TestMethod]
        public void CorrectlyWritesFile()
        {
            var path = Path.GetFullPath(@"C:\temp\foo.bar");
            var expectedBytes = Encoding.UTF8.GetBytes("testing");

            using (var stream = proxyUnderTest.OpenFile(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(expectedBytes);
            }

            var actualBytes = proxyUnderTest.fileSystem[path];
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        [TestMethod]
        public void CorrectlyReadsFile()
        {
            var path = Path.GetFullPath(@"C:\temp\foo.bar");
            var expectedBytes = Encoding.UTF8.GetBytes("testing");
            proxyUnderTest.fileSystem[path] = expectedBytes;

            var actualBytes = new byte[expectedBytes.Length];
            using (var stream = proxyUnderTest.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Read(actualBytes, 0, expectedBytes.Length);
            }

            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }
    }
}