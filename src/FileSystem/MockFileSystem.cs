using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevOptimal.SystemUtilities.FileSystem
{
    public class MockFileSystem : IFileSystem
    {
        internal readonly string id = Guid.NewGuid().ToString();

        internal readonly IDictionary<string, byte[]> data;

        public MockFileSystem()
        {
            data = new ConcurrentDictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        }

        public void CopyFile(string sourcePath, string destinationPath, bool overwrite)
        {
            sourcePath = Path.GetFullPath(sourcePath);

            if (!data.ContainsKey(sourcePath) || data[sourcePath] == null)
            {
                throw new FileNotFoundException();
            }

            destinationPath = Path.GetFullPath(destinationPath);

            if (data.ContainsKey(destinationPath))
            {
                if (data[destinationPath] == null)
                {
                    throw new ArgumentException("destFileName specifies a directory.");
                }
                else if (!overwrite)
                {
                    throw new IOException("Destination file already exists and overwrite was not specified.");
                }
            }

            data[destinationPath] = data[sourcePath].ToArray();
        }

        public void CreateDirectory(string path)
        {
            path = Path.GetFullPath(path);

            if (!data.ContainsKey(path))
            {
                data[path] = null;
            }
            else if (data[path] != null)
            {
                throw new IOException("The path is a file.");
            }
        }

        public void CreateFile(string path)
        {
            path = Path.GetFullPath(path);

            if (!data.ContainsKey(path))
            {
                data[path] = new byte[0];
            }
            else if (data[path] == null)
            {
                throw new IOException("The path is a directory.");
            }
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            path = Path.GetFullPath(path);

            if (data.ContainsKey(path))
            {
                if (data[path] != null)
                {
                    throw new IOException("A file with the same name and location specified by path exists.");
                }

                var children = data.Keys.Where(p => Path.GetDirectoryName(p).Equals(path, StringComparison.OrdinalIgnoreCase));

                if (children.Any())
                {
                    if (recursive)
                    {
                        foreach (var child in children)
                        {
                            if (data[child] == null)
                            {
                                DeleteDirectory(child, recursive);
                            }
                            else
                            {
                                DeleteFile(child);
                            }
                        }
                    }
                    else
                    {
                        throw new IOException("The directory specified by path is not empty.");
                    }
                }

                data.Remove(path);
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        public void DeleteFile(string path)
        {
            path = Path.GetFullPath(path);

            if (data.ContainsKey(path))
            {
                if (data[path] == null)
                {
                    throw new UnauthorizedAccessException("path is a directory.");
                }

                data.Remove(path);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public bool DirectoryExists(string path)
        {
            try
            {
                path = Path.GetFullPath(path);
                return data.ContainsKey(path) && data[path] == null;
            }
            catch
            {
                return false;
            }
        }

        public bool FileExists(string path)
        {
            try
            {
                path = Path.GetFullPath(path);
                return data.ContainsKey(path) && data[path] != null;
            }
            catch
            {
                return false;
            }
        }

        public FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
        {
            path = Path.GetFullPath(path);

            if (data.ContainsKey(path) && data[path] == null)
            {
                throw new UnauthorizedAccessException("path specified a directory.");
            }

            return new MockFileStream(this, path, mode, access, share);
        }
    }
}
