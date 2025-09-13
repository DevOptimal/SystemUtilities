using DevOptimal.SystemUtilities.Common.StateManagement;

namespace DevOptimal.SystemUtilities.Environment.StateManagement
{
    internal class EnvironmentVariableMemento : IMemento
    {
        public string Value { get; set; }
    }
}
