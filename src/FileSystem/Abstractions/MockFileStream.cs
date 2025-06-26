using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DevOptimal.SystemUtilities.FileSystem.Abstractions
{
    /// <summary>
    /// Represents a mock implementation of <see cref="FileStream"/> for use with <see cref="MockFileSystem"/>.
    /// This class manages temporary files on disk to simulate file operations in memory, 
    /// synchronizing changes with the mock file system's data store.
    /// </summary>
    public class MockFileStream : FileStream
    {
        /// <summary>
        /// Tracks the number of active references to each temporary file path.
        /// Used to determine when a temporary file can be safely deleted.
        /// </summary>
        private readonly static IDictionary<string, int> fileReferenceCounter = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Indicates whether the stream has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The original file path in the mock file system.
        /// </summary>
        private readonly string path;

        /// <summary>
        /// The path to the temporary file on disk used to back this stream.
        /// </summary>
        private readonly string temporaryFilePath;

        /// <summary>
        /// The associated mock file system instance.
        /// </summary>
        private readonly MockFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockFileStream"/> class for the specified file in the mock file system.
        /// </summary>
        /// <param name="fileSystem">The mock file system.</param>
        /// <param name="path">The file path in the mock file system.</param>
        /// <param name="mode">The file mode to use.</param>
        /// <param name="access">The file access to use.</param>
        /// <param name="share">The file sharing mode to use.</param>
        public MockFileStream(MockFileSystem fileSystem, string path, FileMode mode, FileAccess access, FileShare share)
            : this(GetTemporaryMockFile(path, fileSystem), mode, access, share)
        {
            this.path = Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockFileStream"/> class for the specified temporary file path.
        /// </summary>
        /// <param name="path">The temporary file path on disk.</param>
        /// <param name="mode">The file mode to use.</param>
        /// <param name="access">The file access to use.</param>
        /// <param name="share">The file sharing mode to use.</param>
        private MockFileStream(string path, FileMode mode, FileAccess access, FileShare share)
            : base(path, mode, access, share)
        {
            temporaryFilePath = Path.GetFullPath(path) ?? throw new ArgumentNullException(nameof(path));
        }

        /// <summary>
        /// Finalizer to ensure resources are released.
        /// </summary>
        ~MockFileStream()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MockFileStream"/> and optionally releases the managed resources.
        /// Synchronizes the temporary file's contents back to the mock file system and deletes the temporary file if no longer needed.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                base.Dispose(disposing);

                lock (fileReferenceCounter)
                {
                    // Read the contents of the temporary file and update the mock file system's data.
                    var bytes = File.ReadAllBytes(temporaryFilePath);
                    if (fileSystem.data.ContainsKey(path))
                    {
                        fileSystem.data[path].Clear();
                        fileSystem.data[path].AddRange(bytes);
                    }
                    else
                    {
                        fileSystem.data[path] = new List<byte>(bytes);
                    }
                }

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

        /// <summary>
        /// Creates or retrieves a temporary file on disk to back the specified mock file.
        /// Ensures the temporary file is initialized with the mock file's contents if it exists.
        /// </summary>
        /// <param name="path">The file path in the mock file system.</param>
        /// <param name="fileSystem">The mock file system.</param>
        /// <returns>The path to the temporary file on disk.</returns>
        private static string GetTemporaryMockFile(string path, MockFileSystem fileSystem)
        {
            path = Path.GetFullPath(path ?? throw new ArgumentNullException(nameof(path)));

            string hash;
            using (var md5 = MD5.Create())
            {
                // Generate a unique hash for the file path (case-insensitive).
                hash = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(path.ToLower()))).Replace("-", string.Empty);
            }

            var temporaryFilePath = Path.Combine(Path.GetTempPath(), string.Join(".", nameof(MockFileStream), fileSystem.id, hash, "dat"));

            lock (fileReferenceCounter)
            {
                if (!fileReferenceCounter.ContainsKey(temporaryFilePath))
                {
                    fileReferenceCounter[temporaryFilePath] = 0;
                }

                // If this is the first reference and the file exists in the mock file system, initialize the temp file.
                if (fileReferenceCounter[temporaryFilePath] == 0 && fileSystem.FileExists(path))
                {
                    File.WriteAllBytes(temporaryFilePath, [.. fileSystem.data[path]]);
                }

                fileReferenceCounter[temporaryFilePath]++;
            }

            return temporaryFilePath;
        }
    }
}
