using System;

namespace bradselw.SystemResources.Environment
{
    public class DefaultEnvironmentProxy : IEnvironmentProxy
    {
        public string GetEnvironmentVariable(string name, EnvironmentVariableTarget target)
        {
            return System.Environment.GetEnvironmentVariable(name, target);
        }

        public void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target)
        {
            System.Environment.SetEnvironmentVariable(name, value, target);
        }
    }
}
