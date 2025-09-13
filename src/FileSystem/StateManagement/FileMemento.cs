using DevOptimal.SystemUtilities.Common.StateManagement;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    internal class FileMemento : IMemento
    {
        public string Hash { get; set; }
    }
}
