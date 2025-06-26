using System;

namespace DevOptimal.SystemUtilities.Environment.Abstractions
{
    public interface IEnvironment
    {
        /// <summary>
        /// Retrieves the value of an environment variable from the specified target.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="target">The location where the environment variable is stored.</param>
        /// <returns>The value of the environment variable, or null if the variable is not found.</returns>
        string GetEnvironmentVariable(string name, EnvironmentVariableTarget target);

        /// <summary>
        /// Sets the value of an environment variable for the specified target.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="value">The value to assign to the environment variable. If null, the variable is deleted.</param>
        /// <param name="target">The location where the environment variable is stored.</param>
        void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target);
    }
}
