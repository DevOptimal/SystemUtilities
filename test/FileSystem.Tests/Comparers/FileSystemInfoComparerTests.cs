using DevOptimal.SystemUtilities.FileSystem.Comparers;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.Comparers
{
    [TestClass]
    public class FileSystemInfoComparerTests
    {
        public static IEnumerable<object[]> SameBehaviorDataSet
        {
            get
            {
                yield return new object[] { FileSystemInfoComparer.Linux };
                yield return new object[] { FileSystemInfoComparer.OSX };
                yield return new object[] { FileSystemInfoComparer.Windows };
            }
        }

        public static IEnumerable<object[]> DifferentCasingDataSet
        {
            get
            {
                yield return new object[] { FileSystemInfoComparer.Linux, 2 };
                yield return new object[] { FileSystemInfoComparer.OSX, 1 };
                yield return new object[] { FileSystemInfoComparer.Windows, 1 };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(SameBehaviorDataSet), DynamicDataSourceType.Property)]
        public void ComparesTwoDifferentFiles(IEqualityComparer<FileSystemInfo> comparer)
        {
            var files = new HashSet<FileInfo>(comparer)
            {
                new(@"C:\temp\foo.txt"),
                new(@"C:\temp\bar.txt")
            };
            Assert.AreEqual(2, files.Count);
        }

        [DataTestMethod]
        [DynamicData(nameof(DifferentCasingDataSet), DynamicDataSourceType.Property)]
        public void ComparesSameFileDifferentCasing(IEqualityComparer<FileSystemInfo> comparer, int expectedCount)
        {
            var files = new HashSet<FileInfo>(comparer)
            {
                new(@"C:\temp\foo.txt"),
                new(@"C:\temp\FOO.txt")
            };
            Assert.AreEqual(expectedCount, files.Count);
        }

        [DataTestMethod]
        [DynamicData(nameof(SameBehaviorDataSet), DynamicDataSourceType.Property)]
        public void ComparesTwoDifferentDirectories(IEqualityComparer<FileSystemInfo> comparer)
        {
            var directories = new HashSet<DirectoryInfo>(comparer)
            {
                new(@"C:\temp\foo"),
                new(@"C:\temp\bar")
            };
            Assert.AreEqual(2, directories.Count);
        }

        [DataTestMethod]
        [DynamicData(nameof(DifferentCasingDataSet), DynamicDataSourceType.Property)]
        public void ComparesSameDirectoryDifferentCasing(IEqualityComparer<FileSystemInfo> comparer, int expectedCount)
        {
            var directories = new HashSet<DirectoryInfo>(comparer)
            {
                new(@"C:\temp\foo"),
                new(@"C:\temp\FOO")
            };
            Assert.AreEqual(expectedCount, directories.Count);
        }

        [DataTestMethod]
        [DynamicData(nameof(SameBehaviorDataSet), DynamicDataSourceType.Property)]
        public void ComparesSameDirectoryTrailingSlash(IEqualityComparer<FileSystemInfo> comparer)
        {
            var directories = new HashSet<DirectoryInfo>(comparer)
            {
                new(@"C:\temp\foo"),
                new(@"C:\temp\foo\")
            };
            Assert.AreEqual(1, directories.Count);
        }

        [DataTestMethod]
        [DynamicData(nameof(SameBehaviorDataSet), DynamicDataSourceType.Property)]
        public void HandlesParentDirectoryReferences(IEqualityComparer<FileSystemInfo> comparer)
        {
            var files = new HashSet<FileInfo>(comparer)
            {
                new(@"C:\temp\bar\..\foo.txt"),
                new(@"C:\temp\foo.txt")
            };
            Assert.AreEqual(1, files.Count);
        }

        [DataTestMethod]
        [DynamicData(nameof(SameBehaviorDataSet), DynamicDataSourceType.Property)]
        public void HandlesCurrentDirectoryReferences(IEqualityComparer<FileSystemInfo> comparer)
        {
            var files = new HashSet<FileInfo>(comparer)
            {
                new(@"C:\temp\\.\foo.txt"),
                new(@"C:\temp\foo.txt")
            };
            Assert.AreEqual(1, files.Count);
        }
    }
}
