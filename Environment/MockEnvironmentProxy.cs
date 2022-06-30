using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace bradselw.System.Resources.Environment
{
    public class MockEnvironmentProxy : IEnvironmentProxy
    {
        internal readonly IDictionary<EnvironmentVariableTarget, IDictionary<string, string>> environmentVariables;

        public MockEnvironmentProxy()
        {
            environmentVariables = new ConcurrentDictionary<EnvironmentVariableTarget, IDictionary<string, string>>();
            foreach (EnvironmentVariableTarget target in Enum.GetValues(typeof(EnvironmentVariableTarget)))
            {
                environmentVariables.Add(target, new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }
        }

        public string GetEnvironmentVariable(string name, EnvironmentVariableTarget target)
        {
            if (!environmentVariables[target].TryGetValue(name, out var value))
            {
                value = null;
            }

            return value;
        }

        public void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target)
        {
            if (value == null && environmentVariables[target].ContainsKey(name))
            {
                environmentVariables[target].Remove(name);
            }
            else
            {
                environmentVariables[target][name] = value;
            }
        }
    }
}
