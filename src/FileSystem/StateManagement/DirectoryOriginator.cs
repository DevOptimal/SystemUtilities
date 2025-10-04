using DevOptimal.SystemUtilities.Common.StateManagement;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.Runtime.InteropServices;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement
{
    internal class DirectoryOriginator(string path, IFileSystem fileSystem) : IOriginator<DirectoryMemento>
    {
        public string Path { get; } = System.IO.Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));

        public IFileSystem FileSystem { get; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

        public string ID
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Path;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Path.ToLower();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Path;
                }
                else
                {
                    throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
                }
            }
        }

        public DirectoryMemento GetState()
        {
            return new DirectoryMemento
            {
                Exists = FileSystem.DirectoryExists(Path)
            };
        }

        public void SetState(DirectoryMemento memento)
        {
            if (FileSystem.DirectoryExists(Path))
            {
                if (!memento.Exists)
                {
                    FileSystem.DeleteDirectory(Path, recursive: true);
                }
            }
            else // Path does not exist
            {
                if (memento.Exists)
                {
                    FileSystem.CreateDirectory(Path);
                }
            }
        }
    }
}
