using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace bradselw.System.Resources.FileSystem
{
    public class MockFileStream : FileStream
    {
        private readonly static IDictionary<string, int> fileReferenceCounter = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private bool disposed;

        private readonly string path;

        private readonly string temporaryFilePath;

        private readonly MockFileSystemProxy fileSystemProxy;

        public MockFileStream(MockFileSystemProxy fileSystemProxy, string path, FileMode mode, FileAccess access, FileShare share)
            : this(GetTemporaryMockFile(path, fileSystemProxy), mode, access, share)
        {
            this.path = Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));
            this.fileSystemProxy = fileSystemProxy;
        }

        private MockFileStream(string path, FileMode mode, FileAccess access, FileShare share)
            : base(path, mode, access, share)
        {
            temporaryFilePath = Path.GetFullPath(path) ?? throw new ArgumentNullException(nameof(path));
        }

        ~MockFileStream()
        {
            Dispose(disposing: false);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                base.Dispose(disposing);

                fileSystemProxy.fileSystem[path] = File.ReadAllBytes(temporaryFilePath);

                // Clean up the temporary file on disk once all references to that file are gone.
                lock (fileReferenceCounter)
                {
                    fileReferenceCounter[temporaryFilePath]--;

                    if (fileReferenceCounter[temporaryFilePath] == 0)
                    {
                        File.Delete(temporaryFilePath);
                    }
                }

                // TODO: set large fields to null
                disposed = true;
            }
        }

        private static string GetTemporaryMockFile(string path, MockFileSystemProxy fileSystemProxy)
        {
            path = Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));

            string hash;
            using (var md5 = MD5.Create())
            {
                hash = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(path.ToLower()))).Replace("-", string.Empty);
            }

            var temporaryFilePath = Path.Combine(Path.GetTempPath(), string.Join(".", nameof(MockFileStream), fileSystemProxy.id, hash, "dat"));

            lock (fileReferenceCounter)
            {
                if (!fileReferenceCounter.ContainsKey(temporaryFilePath))
                {
                    fileReferenceCounter[temporaryFilePath] = 0;
                }

                if (fileReferenceCounter[temporaryFilePath] == 0 && fileSystemProxy.FileExists(path))
                {
                    File.WriteAllBytes(temporaryFilePath, fileSystemProxy.fileSystem[path]);
                }

                fileReferenceCounter[temporaryFilePath]++;
            }

            return temporaryFilePath;
        }
    }
}
