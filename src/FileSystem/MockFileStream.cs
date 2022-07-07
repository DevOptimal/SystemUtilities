using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DevOptimal.SystemUtilities.FileSystem
{
    public class MockFileStream : FileStream
    {
        private readonly static IDictionary<string, int> fileReferenceCounter = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private bool disposed;

        private readonly string path;

        private readonly string temporaryFilePath;

        private readonly MockFileSystem fileSystem;

        public MockFileStream(MockFileSystem fileSystem, string path, FileMode mode, FileAccess access, FileShare share)
            : this(GetTemporaryMockFile(path, fileSystem), mode, access, share)
        {
            this.path = Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));
            this.fileSystem = fileSystem;
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

                fileSystem.data[path] = File.ReadAllBytes(temporaryFilePath);

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

        private static string GetTemporaryMockFile(string path, MockFileSystem fileSystem)
        {
            path = Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));

            string hash;
            using (var md5 = MD5.Create())
            {
                hash = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(path.ToLower()))).Replace("-", string.Empty);
            }

            var temporaryFilePath = Path.Combine(Path.GetTempPath(), string.Join(".", nameof(MockFileStream), fileSystem.id, hash, "dat"));

            lock (fileReferenceCounter)
            {
                if (!fileReferenceCounter.ContainsKey(temporaryFilePath))
                {
                    fileReferenceCounter[temporaryFilePath] = 0;
                }

                if (fileReferenceCounter[temporaryFilePath] == 0 && fileSystem.FileExists(path))
                {
                    File.WriteAllBytes(temporaryFilePath, fileSystem.data[path]);
                }

                fileReferenceCounter[temporaryFilePath]++;
            }

            return temporaryFilePath;
        }
    }
}
