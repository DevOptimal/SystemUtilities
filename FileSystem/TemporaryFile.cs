using System;

namespace DevOptimal.SystemUtilities.FileSystem
{
    public class TemporaryFile : IDisposable
    {
        private bool disposedValue;

        private readonly IFileSystemProxy fileSystemProxy;

        public string Path { get; }

        public TemporaryFile()
            : this(new DefaultFileSystemProxy())
        {
        }

        public TemporaryFile(IFileSystemProxy fileSystemProxy)
            : this(GetUniqueTemporaryFileName(fileSystemProxy), fileSystemProxy)
        {
        }

        public TemporaryFile(string path, IFileSystemProxy fileSystemProxy)
        {
            Path = global::System.IO.Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));
            this.fileSystemProxy = fileSystemProxy ?? throw new ArgumentNullException(nameof(fileSystemProxy));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (fileSystemProxy.FileExists(Path))
                {
                    fileSystemProxy.DeleteFile(Path);
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

        private static string GetUniqueTemporaryFileName(IFileSystemProxy fileSystemProxy)
        {
            var temporaryDirectory = global::System.IO.Path.GetTempPath();
            if (!fileSystemProxy.DirectoryExists(temporaryDirectory))
            {
                fileSystemProxy.CreateDirectory(temporaryDirectory);
            }

            string path;
            do
            {
                path = global::System.IO.Path.Combine(
                    temporaryDirectory,
                    global::System.IO.Path.GetRandomFileName());
            } while (fileSystemProxy.FileExists(path));

            return path;
        }
    }
}
