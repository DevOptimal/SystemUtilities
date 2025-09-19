using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Environment.Abstractions;
using DevOptimal.SystemUtilities.Environment.StateManagement.Serialization;
using System;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    public class EnvironmentSnapshotter(IEnvironment environment)
        : Snapshotter(new EnvironmentSnapshotSerializer(environment))
    {
        public EnvironmentSnapshotter() : this(new DefaultEnvironment())
        { }

        public ISnapshot SnapshotEnvironmentVariable(string name, EnvironmentVariableTarget target)
        {
            var originator = new EnvironmentVariableOriginator(name, target, environment);
            var snapshot = new Caretaker<EnvironmentVariableOriginator, EnvironmentVariableMemento>(originator, database);
            AddSnapshot(snapshot);
            return snapshot;
        }
    }
}
