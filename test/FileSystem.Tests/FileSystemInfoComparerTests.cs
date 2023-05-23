using DevOptimal.SystemUtilities.FileSystem.Comparers;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Tests
{
    [TestClass]
    public class FileSystemInfoComparerTests
    {
        [TestMethod]
        public void CorrectlyComparesFileInfos()
        {
            var files = new HashSet<FileInfo>(new FileSystemInfoComparer())
            {
                new FileInfo(@"C:\temp\foo.txt"),
                new FileInfo(@"C:\temp\FOO.txt")
            };
            Assert.AreEqual(1, files.Count);

            files.Add(new FileInfo(@"C:\temp\bar.txt"));
            Assert.AreEqual(2, files.Count);
        }

        [TestMethod]
        public void CorrectlyComparesDirectoryInfos()
        {
            var directories = new HashSet<DirectoryInfo>(new FileSystemInfoComparer())
            {
                new DirectoryInfo(@"C:\temp\foo"),
                new DirectoryInfo(@"C:\temp\FOO")
            };
            Assert.AreEqual(1, directories.Count);

            directories.Add(new DirectoryInfo(@"C:\temp\bar"));
            Assert.AreEqual(2, directories.Count);
        }

        [TestMethod]
        public void CorrectlyHandlesParentDirectoryReferences()
        {
            var files = new HashSet<FileInfo>(new FileSystemInfoComparer())
            {
                new FileInfo(@"C:\temp\bar\..\foo.txt"),
                new FileInfo(@"C:\temp\foo.txt")
            };
            Assert.AreEqual(1, files.Count);
        }

        [TestMethod]
        public void CorrectlyHandlesCurrentDirectoryReferences()
        {
            var files = new HashSet<FileInfo>(new FileSystemInfoComparer())
            {
                new FileInfo(@"C:\temp\\.\foo.txt"),
                new FileInfo(@"C:\temp\foo.txt")
            };
            Assert.AreEqual(1, files.Count);
        }
    }
}
