using DevOptimal.SystemUtilities.FileSystem.Comparers;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.Comparers
{
    [TestClass]
    public class DriveInfoComparerTests
    {
        public static IEnumerable<object[]> SameBehaviorDataSet
        {
            get
            {
                yield return new object[] { DriveInfoComparer.Linux };
                yield return new object[] { DriveInfoComparer.OSX };
                yield return new object[] { DriveInfoComparer.Windows };
            }
        }

        public static IEnumerable<object[]> DifferentCasingDataSet
        {
            get
            {
                yield return new object[] { DriveInfoComparer.Linux, 2 };
                yield return new object[] { DriveInfoComparer.OSX, 1 };
                yield return new object[] { DriveInfoComparer.Windows, 1 };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(SameBehaviorDataSet), DynamicDataSourceType.Property)]
        public void ComparesTwoDifferentDrives(IEqualityComparer<DriveInfo> comparer)
        {
            var drives = new HashSet<DriveInfo>(comparer)
            {
                new(@"C:\"),
                new(@"D:\")
            };
            Assert.AreEqual(2, drives.Count);
        }

        [DataTestMethod]
        [DynamicData(nameof(DifferentCasingDataSet), DynamicDataSourceType.Property)]
        public void ComparesSameDriveDifferentCasing(IEqualityComparer<DriveInfo> comparer, int expectedCount)
        {
            var drives = new HashSet<DriveInfo>(comparer)
            {
                new(@"C:\"),
                new(@"c:\")
            };
            Assert.AreEqual(expectedCount, drives.Count);
        }

        [DataTestMethod]
        [DynamicData(nameof(SameBehaviorDataSet), DynamicDataSourceType.Property)]
        public void ComparesSameDriveTrailingSlash(IEqualityComparer<DriveInfo> comparer)
        {
            var drives = new HashSet<DriveInfo>(comparer)
            {
                new(@"C:"),
                new(@"C:\")
            };
            Assert.AreEqual(1, drives.Count);
        }
    }
}
