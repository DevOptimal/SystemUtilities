using System;

namespace DevOptimal.SystemUtilities.Environment.Abstractions
{
    /// <summary>
    /// Provides a default implementation of the <see cref="IEnvironment"/> interface,
    /// using <see cref="System.Environment"/> to manage environment variables.
    /// </summary>
    public class DefaultEnvironment : IEnvironment
    {
        /// <inheritdoc />
        public string GetEnvironmentVariable(string name, EnvironmentVariableTarget target)
        {
            return System.Environment.GetEnvironmentVariable(name, target);
        }

        /// <inheritdoc />
        public void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target)
        {
            System.Environment.SetEnvironmentVariable(name, value, target);
        }
    }
}
