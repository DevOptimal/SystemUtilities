using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.Environment.Abstractions
{
    public class MockEnvironment : IEnvironment
    {
        internal readonly IDictionary<EnvironmentVariableTarget, IDictionary<string, string>> data;

        public MockEnvironment()
        {
            data = new ConcurrentDictionary<EnvironmentVariableTarget, IDictionary<string, string>>();
            foreach (EnvironmentVariableTarget target in Enum.GetValues(typeof(EnvironmentVariableTarget)))
            {
                data.Add(target, new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }
        }

        public string GetEnvironmentVariable(string name, EnvironmentVariableTarget target)
        {
            if (!data[target].TryGetValue(name, out var value))
            {
                value = null;
            }

            return value;
        }

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
