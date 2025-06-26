using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.Environment.Abstractions
{
    /// <summary>
    /// Provides an in-memory mock implementation of <see cref="IEnvironment"/> for testing purposes.
    /// </summary>
    public class MockEnvironment : IEnvironment
    {
        /// <summary>
        /// Stores environment variables for each <see cref="EnvironmentVariableTarget"/>.
        /// </summary>
        internal readonly IDictionary<EnvironmentVariableTarget, IDictionary<string, string>> data;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockEnvironment"/> class.
        /// Sets up a separate dictionary for each <see cref="EnvironmentVariableTarget"/>.
        /// </summary>
        public MockEnvironment()
        {
            data = new ConcurrentDictionary<EnvironmentVariableTarget, IDictionary<string, string>>();
            foreach (EnvironmentVariableTarget target in Enum.GetValues(typeof(EnvironmentVariableTarget)))
            {
                data.Add(target, new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }
        }

        /// <inheritdoc />
        public string GetEnvironmentVariable(string name, EnvironmentVariableTarget target)
        {
            if (!data[target].TryGetValue(name, out var value))
            {
                value = null;
            }

            return value;
        }

        /// <inheritdoc />
        public void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target)
        {
            if (value == null && data[target].ContainsKey(name))
            {
                data[target].Remove(name);
            }
            else
            {
                data[target][name] = value;
            }
        }
    }
}
