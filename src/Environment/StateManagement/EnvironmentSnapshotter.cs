using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Environment.Abstractions;
using DevOptimal.SystemUtilities.Environment.StateManagement.Serialization;
using System;
using System.IO;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    /// <summary>
    /// Provides snapshotting functionality for environment variables.
    /// Supports capturing and restoring environment variable state using the Memento pattern.
    /// </summary>
    public class EnvironmentSnapshotter(IEnvironment environment, DirectoryInfo persistenceDirectory)
        : Snapshotter(new EnvironmentCaretakerSerializer(environment), persistenceDirectory)
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EnvironmentSnapshotter"/> using the default environment and default persistence directory.
        /// </summary>
        public EnvironmentSnapshotter()
            : this(new DefaultEnvironment(), defaultPersistenceDirectory)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="EnvironmentSnapshotter"/> using the specified environment and the default persistence directory.
        /// </summary>
        /// <param name="environment">The environment abstraction to use for variable operations.</param>
        public EnvironmentSnapshotter(IEnvironment environment)
            : this(environment, defaultPersistenceDirectory)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="EnvironmentSnapshotter"/> using the default environment and the specified persistence directory.
        /// </summary>
        /// <param name="persistenceDirectory">The directory for persisting caretaker data.</param>
        public EnvironmentSnapshotter(DirectoryInfo persistenceDirectory)
            : this(new DefaultEnvironment(), persistenceDirectory)
        { }

        /// <summary>
        /// Captures a snapshot of the specified environment variable and returns a disposable snapshot object.
        /// </summary>
        /// <param name="name">The name of the environment variable to snapshot.</param>
        /// <param name="target">The target location of the environment variable.</param>
        /// <returns>An <see cref="ISnapshot"/> representing the captured state of the environment variable.</returns>
        public ISnapshot SnapshotEnvironmentVariable(string name, EnvironmentVariableTarget target)
        {
            var originator = new EnvironmentVariableOriginator(name, target, environment);
            var snapshot = new EnvironmentVariableCaretaker(originator, this);
            return snapshot;
        }
    }
}
