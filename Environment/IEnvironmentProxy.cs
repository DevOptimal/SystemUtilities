using System;

namespace bradselw.SystemResources.Environment
{
    public interface IEnvironmentProxy
    {
        string GetEnvironmentVariable(string name, EnvironmentVariableTarget target);

        void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target);
    }
}
