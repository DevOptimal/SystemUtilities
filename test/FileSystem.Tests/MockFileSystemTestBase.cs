using DevOptimal.SystemUtilities.FileSystem.Abstractions;

namespace DevOptimal.SystemUtilities.FileSystem.Tests
{
    public abstract class MockFileSystemTestBase
    {
        protected MockFileSystem fileSystem;

        [TestInitialize]
        public void TestInitialize()
        {
            fileSystem = new MockFileSystem();
        }
    }
}
