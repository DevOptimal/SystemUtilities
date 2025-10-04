using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using Microsoft.QualityTools.Testing.Fakes;
using System;
using System.Diagnostics.Fakes;

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

        protected static IDisposable CreateShimsContext()
        {
            var context = ShimsContext.Create();

            ShimProcess.AllInstances.IdGet = p => System.Environment.ProcessId + 1;
            ShimProcess.AllInstances.StartTimeGet = p => DateTime.Now;

            return context;
        }
    }
}
