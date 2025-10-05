using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.Environment.Abstractions;
using System;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    /// <summary>
    /// Originator implementation for environment variables in the Memento pattern.
    /// Encapsulates the logic for capturing and restoring the value of an environment variable.
    /// </summary>
    internal class EnvironmentVariableOriginator(string name, EnvironmentVariableTarget target, IEnvironment environment) : IOriginator<EnvironmentVariableMemento>
    {
        /// <summary>
        /// Gets the name of the environment variable.
        /// </summary>
        public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

        /// <summary>
        /// Gets the target location of the environment variable (e.g., Process, User, Machine).
        /// </summary>
        public EnvironmentVariableTarget Target { get; } = target;

        /// <summary>
        /// Gets the environment abstraction used for variable operations.
        /// </summary>
        public IEnvironment Environment { get; } = environment ?? throw new ArgumentNullException(nameof(environment));

        /// <summary>
        /// Gets a unique identifier for this originator, normalized for the current operating system.
        /// </summary>
        public string ID
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return $@"{Target}\{Name}";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return $@"{Target}\{Name}".ToLower();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return $@"{Target}\{Name}";
                }
                else
                {
                    throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
                }
            }
        }

        /// <summary>
        /// Captures the current value of the environment variable as a memento.
        /// </summary>
        /// <returns>A memento containing the current value of the environment variable.</returns>
        public EnvironmentVariableMemento GetState()
        {
            return new EnvironmentVariableMemento
            {
                Value = Environment.GetEnvironmentVariable(Name, Target)
            };
        }

        /// <summary>
        /// Restores the environment variable's value from the provided memento.
        /// </summary>
        /// <param name="memento">The memento containing the value to restore.</param>
        public void SetState(EnvironmentVariableMemento memento)
        {
            Environment.SetEnvironmentVariable(Name, memento.Value, Target);
        }
    }
}
