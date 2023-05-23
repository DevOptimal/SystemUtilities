using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DevOptimal.SystemUtilities.FileSystem
{
    public class MockFileSystem : IFileSystem
    {
        internal readonly string id = Guid.NewGuid().ToString();

        internal readonly IDictionary<string, byte[]> data;

        private readonly static Regex[] invalidSearchPatternRegexes = Path.GetInvalidPathChars()
            .Select(c => Regex.Escape(c.ToString()))
            .Concat(new[] { $@"\.\.{Regex.Escape(Path.DirectorySeparatorChar.ToString())}", $@"\.\.{Regex.Escape(Path.AltDirectorySeparatorChar.ToString())}", @"\.\.$" })
            .Select(s => new Regex(s, RegexOptions.IgnoreCase | RegexOptions.Compiled))
            .ToArray();

        public MockFileSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                data = new ConcurrentDictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                data = new ConcurrentDictionary<string, byte[]>(StringComparer.Ordinal);
            }
            else
            {
                throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
            }
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
            CreateDirectoryRecurse(path);
        }

        public void CreateFile(string path)
        {
            path = Path.GetFullPath(path);

            if (!data.ContainsKey(path))
            {
                CreateDirectoryRecurse(Path.GetDirectoryName(path));
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

        public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return GetFileSystemEntries(path, searchPattern, searchOption, includeDirectories: true, includeFiles: false);
        }

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return GetFileSystemEntries(path, searchPattern, searchOption, includeDirectories: false, includeFiles: true);
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

        private void CreateDirectoryRecurse(string path)
        {
            CreateDirectoryRecurse(Path.GetFullPath(path).Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void CreateDirectoryRecurse(string[] pathParts)
        {
            if (pathParts.Length > 0)
            {
                CreateDirectoryRecurse(pathParts.Take(pathParts.Length - 1).ToArray());

                var path = Path.GetFullPath(Path.Combine(pathParts));

                if (data.ContainsKey(path) && data[path] != null)
                {
                    throw new IOException("The path is a file.");
                }

                data[path] = null;
            }
        }

        private string[] GetFileSystemEntries(string ancestorPath, string searchPattern, SearchOption searchOption, bool includeDirectories, bool includeFiles)
        {
            var searchPatternRegex = GetSearchPatternRegex(searchPattern);
            var ancestorPathParts = Path.GetFullPath(ancestorPath).Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            var result = new List<string>();
            foreach (var path in data.Keys)
            {
                if (includeDirectories == (data[path] == null) || includeFiles == (data[path] != null))
                {
                    var pathParts = Path.GetFullPath(path).Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                    if (pathParts.Length > ancestorPathParts.Length && (searchOption == SearchOption.AllDirectories || pathParts.Length == ancestorPathParts.Length + 1))
                    {
                        var match = true;
                        for (var i = 0; i < ancestorPathParts.Length; i++)
                        {
                            if (!ancestorPathParts[i].Equals(pathParts[i], StringComparison.OrdinalIgnoreCase))
                            {
                                match = false;
                                break;
                            }
                        }

                        if (match && searchPatternRegex.IsMatch(string.Join(Path.DirectorySeparatorChar.ToString(), pathParts.Skip(ancestorPathParts.Length))))
                        {
                            result.Add(path);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        private Regex GetSearchPatternRegex(string searchPattern)
        {
            foreach (var invalidSearchPatternRegex in invalidSearchPatternRegexes)
            {
                if (invalidSearchPatternRegex.IsMatch(searchPattern))
                {
                    throw new ArgumentException("Search pattern is invalid", nameof(searchPattern));
                }
            }

            return new Regex($"^{Regex.Escape(searchPattern.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)).Replace(@"\*", ".*").Replace(@"\?", ".")}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}
