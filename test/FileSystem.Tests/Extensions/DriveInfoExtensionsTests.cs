using DevOptimal.SystemUtilities.FileSystem.Comparers;
using DevOptimal.SystemUtilities.FileSystem.Extensions;
using System;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.Extensions
{
    [TestClass]
    public class DriveInfoExtensionsTests
    {
        private static DriveInfo DriveUnderTest => new(@"C:\");

        [TestMethod]
        public void GetsSubdirectory()
        {
            Assert.IsTrue(DriveUnderTest.GetDirectory("foo").Equals(new DirectoryInfo(@"C:\foo"), DirectoryInfoComparer.Default));
        }

        [TestMethod]
        public void GetsNestedSubdirectory()
        {
            Assert.IsTrue(DriveUnderTest.GetDirectory("foo", "bar").Equals(new DirectoryInfo(@"C:\foo\bar"), DirectoryInfoComparer.Default));
        }

        [TestMethod]
        public void GetsFile()
        {
            Assert.IsTrue(DriveUnderTest.GetFile("foo.txt").Equals(new FileInfo(@"C:\foo.txt"), FileInfoComparer.Default));
        }

        [TestMethod]
        public void GetsNestedFile()
        {
            Assert.IsTrue(DriveUnderTest.GetFile("foo", "bar.txt").Equals(new FileInfo(@"C:\foo\bar.txt"), FileInfoComparer.Default));
        }
    }
}
