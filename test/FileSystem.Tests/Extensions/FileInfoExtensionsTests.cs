using DevOptimal.SystemUtilities.FileSystem.Comparers;
using DevOptimal.SystemUtilities.FileSystem.Extensions;
using System;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.Extensions
{
    [TestClass]
    public class FileInfoExtensionsTests
    {
        private static FileInfo FileUnderTest => new(@"C:\foo\bar.txt");

        [TestMethod]
        public void GetsDrive()
        {
            Assert.IsTrue(FileUnderTest.Drive.Equals(new DriveInfo(@"C:\"), DriveInfoComparer.Default));
        }

        [TestMethod]
        public void IdentifiesADescendantFile()
        {
            var ancestor = new DirectoryInfo(@"C:\foo");

            var descendant = new FileInfo(@"C:\foo\bar.txt");
            Assert.IsTrue(descendant.IsDescendantOf(ancestor));

            descendant = new FileInfo(@"C:\foo\..\bar.txt");
            Assert.IsFalse(descendant.IsDescendantOf(ancestor));

            Assert.IsFalse(descendant.IsDescendantOf(null));
        }
    }
}
