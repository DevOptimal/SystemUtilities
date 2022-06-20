using System;

namespace SelianSoft.System.Resources.Environment.Proxy
{
    public interface IEnvironmentProxy
    {
        string GetEnvironmentVariable(string name, EnvironmentVariableTarget target);

        void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target);
    }
}
