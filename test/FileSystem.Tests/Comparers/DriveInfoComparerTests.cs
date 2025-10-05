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

        [TestMethod]
        [DynamicData(nameof(SameBehaviorDataSet))]
        public void ComparesTwoDifferentDrives(IEqualityComparer<DriveInfo> comparer)
        {
            var drives = new HashSet<DriveInfo>(comparer)
            {
                new(@"C:\"),
                new(@"D:\")
            };
            Assert.HasCount(2, drives);
        }

        [TestMethod]
        [DynamicData(nameof(DifferentCasingDataSet))]
        public void ComparesSameDriveDifferentCasing(IEqualityComparer<DriveInfo> comparer, int expectedCount)
        {
            var drives = new HashSet<DriveInfo>(comparer)
            {
                new(@"C:\"),
                new(@"c:\")
            };
            Assert.HasCount(expectedCount, drives);
        }

        [TestMethod]
        [DynamicData(nameof(SameBehaviorDataSet))]
        public void ComparesSameDriveTrailingSlash(IEqualityComparer<DriveInfo> comparer)
        {
            var drives = new HashSet<DriveInfo>(comparer)
            {
                new(@"C:"),
                new(@"C:\")
            };
            Assert.HasCount(1, drives);
        }
    }
}
