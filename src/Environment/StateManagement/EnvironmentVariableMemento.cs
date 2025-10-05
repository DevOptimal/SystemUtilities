using DevOptimal.SystemUtilities.Common.StateManagement;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    /// <summary>
    /// Represents a memento for an environment variable.
    /// Encapsulates the value of the environment variable for state capture and restoration.
    /// </summary>
    internal class EnvironmentVariableMemento : IMemento
    {
        /// <summary>
        /// Gets or sets the value of the environment variable at the time of snapshot.
        /// </summary>
        public string Value { get; set; }
    }
}
