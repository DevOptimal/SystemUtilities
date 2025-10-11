using DevOptimal.SystemUtilities.Environment.Abstractions;
using DevOptimal.SystemUtilities.Environment.StateManagement;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.StateManagement;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using DevOptimal.SystemUtilities.Registry.StateManagement;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.Fakes;
using System.IO;

namespace DevOptimal.SystemUtilities.Common.Tests
{
    public abstract class TestBase
    {
        public TestContext TestContext { get; set; }

        protected MockEnvironment environment;
        protected MockFileSystem fileSystem;
        protected MockRegistry registry;

        protected const string name = "foo";
        protected const EnvironmentVariableTarget target = EnvironmentVariableTarget.Machine;
        protected const string expectedValue = "bar";

        [TestInitialize]
        public void TestInitialize()
        {
            environment = new MockEnvironment();
            fileSystem = new MockFileSystem();
            registry = new MockRegistry();
        }
        protected static IDisposable CreateShimsContext()
        {
            var context = ShimsContext.Create();

            ShimProcess.AllInstances.IdGet = p => System.Environment.ProcessId + 1;
            ShimProcess.AllInstances.StartTimeGet = p => DateTime.Now;

            return context;
        }

        protected EnvironmentSnapshotter CreateEnvironmentSnapshotter()
        {
            return new EnvironmentSnapshotter(environment, new DirectoryInfo(Path.Join(TestContext.ResultsDirectory, "Persistence")));
        }

        protected FileSystemSnapshotter CreateFileSystemSnapshotter()
        {
            return new FileSystemSnapshotter(fileSystem, new DirectoryInfo(Path.Join(TestContext.ResultsDirectory, "Persistence")));
        }

        protected RegistrySnapshotter CreateRegistrySnapshotter()
        {
            return new RegistrySnapshotter(registry, new DirectoryInfo(Path.Join(TestContext.ResultsDirectory, "Persistence")));
        }
    }
}
