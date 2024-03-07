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
            Assert.IsTrue(FileUnderTest.GetDrive().Equals(new DriveInfo(@"C:\"), DriveInfoComparer.Default));
        }
    }
}
