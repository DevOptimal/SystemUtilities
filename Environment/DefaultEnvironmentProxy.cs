using System;

namespace DevOptimal.SystemUtilities.Environment
{
    public class DefaultEnvironmentProxy : IEnvironmentProxy
    {
        public string GetEnvironmentVariable(string name, EnvironmentVariableTarget target)
        {
            return global::System.Environment.GetEnvironmentVariable(name, target);
        }

        public void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target)
        {
            global::System.Environment.SetEnvironmentVariable(name, value, target);
        }
    }
}
