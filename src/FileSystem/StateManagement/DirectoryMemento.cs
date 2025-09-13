using DevOptimal.SystemUtilities.Common.StateManagement;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    internal class DirectoryMemento : IMemento
    {
        public bool Exists { get; set; }
    }
}
