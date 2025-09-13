using DevOptimal.SystemUtilities.Common.StateManagement;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    internal class RegistryKeyMemento : IMemento
    {
        public bool Exists { get; set;  }
    }
}
