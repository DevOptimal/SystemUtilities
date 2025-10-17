// Implements a zip file cache for storing and retrieving file contents in a zip archive.
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using System;
using System.IO;
using System.IO.Compression;

namespace DevOptimal.SystemUtilities.FileSystem.StateManagement.Caching
{
    /// <summary>
    /// Provides a simple GUID-addressed zip file based cache for storing and retrieving file contents.
    /// Each uploaded file is stored as an individual entry whose name is a newly generated GUID string.
    /// </summary>
    /// <remarks>
    /// Design notes:
    ///  - Isolation: A single physical zip file is used; entries never overwrite existing content because a new GUID is
    ///    generated for each upload. (No deduplication or content-addressing is performed.)
    ///  - Concurrency: FileShare.None is used when opening the underlying zip file which means concurrent callers will
    ///    block/fail. This keeps implementation simple but limits scalability.
    ///  - Integrity: No checksum is stored; integrity relies on the ZIP container's structure only.
    ///  - Lifetime: Entries are never deleted here; call sites are responsible for reclaiming space (e.g., recreating zip).
    /// Potential enhancements (future):
    ///  - Add existence checks / graceful handling if an entry is not found in DownloadFile.
    ///  - Introduce a strategy for duplicate content (content hashing) to reduce space.
    ///  - Add optional compression level selection instead of default ("Stored" vs compressed) when creating entries.
    ///  - Support async I/O (currently synchronous, may block threads for large files).
    /// </remarks>
    /// <param name="zipFile">Physical zip file used as backing store.</param>
    /// <param name="fileSystem">File system abstraction used externally (not used internally here aside from exposure).</param>
    public class ZipFileCache : IFileCache
    {
        // Backing ZIP file reference. Immutable after construction.
        private readonly FileInfo zipFile;

        /// <summary>
        /// Gets or sets the file system abstraction (exposed for broader system operations outside of direct caching).
        /// </summary>
        public IFileSystem FileSystem { get; set; }

        /// <summary>
        /// Creates a new zip file cache bound to the provided zip file path.
        /// </summary>
        /// <param name="zipFile">Existing or to-be-created zip file path.</param>
        /// <param name="fileSystem">File system abstraction to expose via <see cref="FileSystem"/>.</param>
        public ZipFileCache(FileInfo zipFile, IFileSystem fileSystem)
        {
            this.zipFile = zipFile; // Assumes non-null; callers are expected to validate.
            FileSystem = fileSystem; // Abstraction stored; not directly leveraged by methods yet.
        }

        /// <summary>
        /// Downloads the content for the provided identifier into a new destination file.
        /// </summary>
        /// <param name="id">The GUID-like identifier previously returned by <see cref="UploadFile"/>.</param>
        /// <param name="destinationPath">Full path where the file will be created; must not already exist.</param>
        /// <exception cref="FileNotFoundException">If the zip entry with the specified ID does not exist (currently a NullReferenceException would be thrown; improvement suggested).</exception>
        /// <exception cref="IOException">If destinationPath exists or share/lock conflicts occur.</exception>
        /// <remarks>
        /// Implementation details:
        ///  - Opens the zip file with FileMode.OpenOrCreate & FileAccess.Read. If the file does not exist it will be created
        ///    (empty) which means GetEntry will return null. This is a potential pitfall; callers should ensure the cache
        ///    has been populated first.
        ///  - Creates destination with FileMode.CreateNew to enforce non-overwrite semantics.
        ///  - Uses entryStream.CopyTo for buffered copy (synchronous).
        /// </remarks>
        public void DownloadFile(string id, string destinationPath)
        {
            using var zipStream = zipFile.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.None); // Exclusive access.
            using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            var zipEntry = zipArchive.GetEntry(id); // May be null if entry absent.
            using var entryStream = zipEntry.Open(); // Would throw if zipEntry is null.
            using var fileStream = File.Open(destinationPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            entryStream.CopyTo(fileStream); // Stream-to-stream copy.
        }

        /// <summary>
        /// Uploads a file from the given source path, storing it as a new entry whose name is a generated GUID.
        /// Returns the identifier (entry name) for later retrieval.
        /// </summary>
        /// <param name="sourcePath">Path of the existing file to read and cache.</param>
        /// <returns>String identifier for the stored file (entry name in zip archive).</returns>
        /// <exception cref="FileNotFoundException">If the source file does not exist.</exception>
        /// <exception cref="IOException">On I/O or sharing conflicts writing to the zip file.</exception>
        /// <remarks>
        /// Implementation details:
        ///  - Opens zip in Update mode so that existing entries are retained and new one is appended.
        ///  - Uses default compression level (library decides); could expose configuration later.
        ///  - Returns a GUID string (e.g., "3f6c0d47-..."), ensuring extremely low collision probability.
        /// </remarks>
        public string UploadFile(string sourcePath)
        {
            var id = Guid.NewGuid().ToString(); // Unique entry name.
            using var zipStream = zipFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None); // Exclusive write.
            using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update); // Allows adding entries.
            var zipEntry = zipArchive.CreateEntry(id); // New empty entry.
            using var fileStream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.None); // Source file.
            using var entryStream = zipEntry.Open(); // Destination entry stream.
            fileStream.CopyTo(entryStream); // Write content.
            return id;
        }
    }
}