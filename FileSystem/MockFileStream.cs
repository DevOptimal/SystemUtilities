using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace bradselw.System.Resources.FileSystem
{
    public class MockFileStream : FileStream
    {
        public static ConcurrentDictionary<string, int> fileReferenceCounter = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private bool disposedValue;

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {

                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                fileReferenceCounter.AddOrUpdate(temporaryFilePath, 0, (k, v) =>
                {
                    if (v == 1)
                    {
                        fileSystemProxy.fileSystem[path] = File.ReadAllBytes(k);
                        File.Delete(k);
                        return 0;
                    }
                    else
                    {
                        return v - 1;
                    }
                });

                // TODO: set large fields to null
                disposedValue = true;
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

            var temporaryFilePath = Path.Combine(Path.GetTempPath(), hash);

            if (fileReferenceCounter.AddOrUpdate(temporaryFilePath, 1, (k, v) => v + 1) == 1 && fileSystemProxy.FileExists(path))
            {
                File.WriteAllBytes(temporaryFilePath, fileSystemProxy.fileSystem[path]);
            }

            return temporaryFilePath;
        }
    }
}
