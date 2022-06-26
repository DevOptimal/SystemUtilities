namespace bradselw.System.Resources.FileSystem.Tests
{
    [TestClass]
    public class DeleteFileTests
    {
        private MockFileSystemProxy proxyUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            proxyUnderTest = new MockFileSystemProxy();
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}