// Represents the memento for a directory, capturing its existence state.
using DevOptimal.SystemUtilities.Common.StateManagement;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    /// <summary>
    /// Memento representing the state of a directory (whether it exists).
    /// </summary>
    internal class DirectoryMemento : IMemento
    {
        /// <summary>
        /// Gets or sets a value indicating whether the directory exists.
        /// </summary>
        public bool Exists { get; set; }
    }
}
