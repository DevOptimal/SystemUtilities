using DevOptimal.SystemUtilities.Common.StateManagement;
using Microsoft.Win32;

namespace DevOptimal.SystemUtilities.Registry.StateManagement
{
    internal class RegistryValueMemento : IMemento
    {
        public object Value { get; set; }

        public RegistryValueKind Kind { get; set; }
    }
}
