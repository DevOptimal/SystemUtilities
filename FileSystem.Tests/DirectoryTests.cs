namespace bradselw.System.Resources.FileSystem.Tests
{
    [TestClass]
    public class DirectoryTests
    {
        private MockFileSystemProxy proxyUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            proxyUnderTest = new MockFileSystemProxy();
        }

        [TestMethod]
        public void CorrectlyIdentifiesExistentAndNonexistentDirectories()
        {
            var path = @"C:\temp\foo";

            Assert.IsFalse(proxyUnderTest.DirectoryExists(path));

            proxyUnderTest.CreateDirectory(path);

            Assert.IsTrue(proxyUnderTest.DirectoryExists(path));

            proxyUnderTest.DeleteDirectory(path, recursive: true);

            Assert.IsFalse(proxyUnderTest.DirectoryExists(path));
        }
    }
}
