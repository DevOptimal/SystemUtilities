using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevOptimal.SystemUtilities.FileSystem.Tests
{
    [TestClass]
    public class FileTests
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
            var expectedBytes = Encoding.UTF8.GetBytes("testing");
            fileSystem.data[path] = expectedBytes;

            var actualBytes = new byte[expectedBytes.Length];
            using (var stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Read(actualBytes, 0, expectedBytes.Length);
            }

            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        [TestMethod]
        public void ConcurrentReads()
        {
            var path = Path.GetFullPath(@"C:\temp\foo.bar");
            var expectedBytes = Encoding.UTF8.GetBytes("testing");
            fileSystem.data[path] = expectedBytes;

            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var actualBytes = new byte[expectedBytes.Length];
                    using (var stream = fileSystem.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        stream.Read(actualBytes, 0, expectedBytes.Length);
                    }

                    CollectionAssert.AreEqual(expectedBytes, actualBytes);
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}