using DevOptimal.SystemUtilities.FileSystem.Comparers;
using DevOptimal.SystemUtilities.FileSystem.Extensions;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.Extensions
{
    [TestClass]
    public class DirectoryInfoExtensionsTests
    {
        public static DirectoryInfo DirectoryUnderTest => new(@"C:\foo");

        [TestMethod]
        public void GetsSubdirectory()
        {
            Assert.IsTrue(DirectoryUnderTest.GetDirectory("bar").Equals(new DirectoryInfo(@"C:\foo\bar"), DirectoryInfoComparer.Default));
        }

        [TestMethod]
        public void GetsNestedSubdirectory()
        {
            Assert.IsTrue(DirectoryUnderTest.GetDirectory("bar", "baz").Equals(new DirectoryInfo(@"C:\foo\bar\baz"), DirectoryInfoComparer.Default));
        }

        [TestMethod]
        public void GetsDrive()
        {
            Assert.IsTrue(DirectoryUnderTest.GetDrive().Equals(new DriveInfo(@"C:\"), DriveInfoComparer.Default));
        }

        [TestMethod]
        public void GetsFile()
        {
            Assert.IsTrue(DirectoryUnderTest.GetFile("bar.txt").Equals(new FileInfo(@"C:\foo\bar.txt"), FileInfoComparer.Default));
        }

        [TestMethod]
        public void GetsNestedFile()
        {
            Assert.IsTrue(DirectoryUnderTest.GetFile("bar", "baz.txt").Equals(new FileInfo(@"C:\foo\bar\baz.txt"), FileInfoComparer.Default));
        }

        [TestMethod]
        public void IdentifiesADescendantDirectory()
        {
            var descendant = new DirectoryInfo(@"C:\foo\bar");
            Assert.IsFalse(descendant.IsDescendantOf(null));
            Assert.IsFalse(descendant.IsDescendantOf(descendant));
            Assert.IsTrue(descendant.IsDescendantOf(new DirectoryInfo(@"C:\foo")));
            Assert.IsTrue(descendant.IsDescendantOf(new DirectoryInfo(@"C:\foo\bar\..")));
        }

        [TestMethod]
        public void IdentifiesAnAncestorDirectory()
        {
            var ancestor = new DirectoryInfo(@"C:\foo");
            Assert.IsFalse(ancestor.IsAncestorOf(null));
            Assert.IsFalse(ancestor.IsAncestorOf(ancestor));
            Assert.IsTrue(ancestor.IsAncestorOf(new DirectoryInfo(@"C:\foo\bar")));
            Assert.IsFalse(ancestor.IsAncestorOf(new DirectoryInfo(@"C:\foo\..\bar")));
        }
    }
}
