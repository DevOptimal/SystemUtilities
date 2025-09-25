using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Environment.Abstractions;
using DevOptimal.SystemUtilities.Environment.StateManagement.Serialization;
using System;
using System.IO;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    public class EnvironmentSnapshotter(IEnvironment environment, DirectoryInfo persistenceDirectory)
        : Snapshotter(new EnvironmentCaretakerSerializer(environment), persistenceDirectory)
    {
        public EnvironmentSnapshotter()
            : this(new DefaultEnvironment(), defaultPersistenceDirectory)
        { }

        public EnvironmentSnapshotter(IEnvironment environment)
            : this(environment, defaultPersistenceDirectory)
        { }

        public EnvironmentSnapshotter(DirectoryInfo persistenceDirectory)
            : this(new DefaultEnvironment(), persistenceDirectory)
        { }

        public ISnapshot SnapshotEnvironmentVariable(string name, EnvironmentVariableTarget target)
        {
            var originator = new EnvironmentVariableOriginator(name, target, environment);
            var snapshot = new Caretaker<EnvironmentVariableOriginator, EnvironmentVariableMemento>(originator, this);
            return snapshot;
        }
    }
}
