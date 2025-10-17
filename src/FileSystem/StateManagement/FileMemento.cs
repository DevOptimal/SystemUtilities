// Represents the memento for a file, capturing its content hash.
using DevOptimal.SystemUtilities.Common.StateManagement;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    /// <summary>
    /// Memento representing the state of a file (its content hash).
    /// </summary>
    internal class FileMemento : IMemento
    {
        /// <summary>
        /// Gets or sets the hash of the file's content.
        /// </summary>
        public string Hash { get; set; }
    }
}
