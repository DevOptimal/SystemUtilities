using System;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;

namespace DevOptimal.SystemUtilities.FileSystem
{
    public class TemporaryFile : IDisposable
    {
        private bool disposedValue;

        private readonly IFileSystem fileSystem;

        public string Path { get; }

        public TemporaryFile()
            : this(new DefaultFileSystem())
        {
        }

        public TemporaryFile(IFileSystem fileSystem)
            : this(GetUniqueTemporaryFileName(fileSystem), fileSystem)
        {
        }

        public TemporaryFile(string path, IFileSystem fileSystem)
        {
            Path = System.IO.Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (fileSystem.FileExists(Path))
                {
                    fileSystem.DeleteFile(Path);
                }

                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~TemporaryFile()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

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
