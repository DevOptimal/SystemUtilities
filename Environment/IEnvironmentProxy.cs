using System;

namespace DevOptimal.System.Resources.Environment
{
    public interface IEnvironmentProxy
    {
        string GetEnvironmentVariable(string name, EnvironmentVariableTarget target);

        void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target);
    }
}
