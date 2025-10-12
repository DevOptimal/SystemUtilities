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
    internal class EnvironmentVariableOriginator : IOriginator<EnvironmentVariableMemento>
    {
        /// <summary>
        /// Gets the name of the environment variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the target location of the environment variable (e.g., Process, User, Machine).
        /// </summary>
        public EnvironmentVariableTarget Target { get; }

        /// <summary>
        /// Gets the environment abstraction used for variable operations.
        /// </summary>
        public IEnvironment Environment { get; }

        /// <summary>
        /// Gets a unique identifier for this originator, normalized for the current operating system.
        /// </summary>
        /// <remarks>
        /// On Windows the identifier is lower-cased to provide case-insensitive semantics consistent with
        /// the underlying platform's handling of environment variable names. On Linux and macOS the original
        /// casing is preserved, reflecting the case-sensitive behavior of those platforms.
        /// </remarks>
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
        /// Initializes a new instance of the <see cref="EnvironmentVariableOriginator"/> class.
        /// </summary>
        /// <param name="name">The name of the environment variable to manage.</param>
        /// <param name="target">The target scope in which the variable resides.</param>
        /// <param name="environment">The environment abstraction used for get/set operations.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="environment"/> is null.</exception>
        public EnvironmentVariableOriginator(string name, EnvironmentVariableTarget target, IEnvironment environment)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Target = target;
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
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
