using System;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;

namespace DevOptimal.SystemUtilities.FileSystem
{
    /// <summary>
    /// Represents a temporary file that is deleted when disposed.
    /// </summary>
    public class TemporaryFile(string path, IFileSystem fileSystem) : IDisposable
    {
        private bool disposedValue;

        // The file system abstraction used for file operations.
        private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

        /// <summary>
        /// Gets the full path of the temporary file.
        /// </summary>
        public string Path { get; } = System.IO.Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFile"/> class using the default file system.
        /// </summary>
        public TemporaryFile()
            : this(new DefaultFileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFile"/> class with a unique temporary file name.
        /// </summary>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        public TemporaryFile(IFileSystem fileSystem)
            : this(GetUniqueTemporaryFileName(fileSystem), fileSystem)
        {
        }

        /// <summary>
        /// Disposes the temporary file, deleting it from the file system.
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects) if needed.
                }

                // Delete the temporary file if it exists.
                if (fileSystem.FileExists(Path))
                {
                    fileSystem.DeleteFile(Path);
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer to ensure the temporary file is deleted if Dispose is not called.
        /// </summary>
        ~TemporaryFile()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        /// Disposes the object and suppresses finalization.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Generates a unique temporary file name that does not currently exist.
        /// </summary>
        /// <param name="fileSystem">The file system abstraction to use.</param>
        /// <returns>A unique temporary file path.</returns>
        private static string GetUniqueTemporaryFileName(IFileSystem fileSystem)
        {
            var temporaryDirectory = System.IO.Path.GetTempPath();
            if (!fileSystem.DirectoryExists(temporaryDirectory))
            {
                fileSystem.CreateDirectory(temporaryDirectory);
            }

            string path;
            do
            {
                path = System.IO.Path.Combine(
                    temporaryDirectory,
                    System.IO.Path.GetRandomFileName());
            } while (fileSystem.FileExists(path));

            return path;
        }
    }
}
