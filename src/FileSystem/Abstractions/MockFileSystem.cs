﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DevOptimal.SystemUtilities.FileSystem.Abstractions
{
    /// <summary>
    /// Provides an in-memory implementation of <see cref="IFileSystem"/> for testing purposes.
    /// </summary>
    public class MockFileSystem : IFileSystem
    {
        /// <summary>
        /// Unique identifier for this mock file system instance.
        /// </summary>
        internal readonly string id = Guid.NewGuid().ToString();

        /// <summary>
        /// Represents the file system as a mapping of file name to list of bytes. If the list of bytes is null, the entry is a directory,
        /// otherwise it contains the contents of the file.
        /// </summary>
        internal readonly IDictionary<string, List<byte>> data;

        private readonly static Regex[] invalidSearchPatternRegexes = [.. Path.GetInvalidPathChars()
            .Select(c => Regex.Escape(c.ToString()))
            .Concat([$@"\.\.{Regex.Escape(Path.DirectorySeparatorChar.ToString())}", $@"\.\.{Regex.Escape(Path.AltDirectorySeparatorChar.ToString())}", @"\.\.$"])
            .Select(s => new Regex(s, RegexOptions.IgnoreCase | RegexOptions.Compiled))];

        /// <summary>
        /// Initializes a new instance of the <see cref="MockFileSystem"/> class.
        /// </summary>
        public MockFileSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                data = new ConcurrentDictionary<string, List<byte>>(StringComparer.OrdinalIgnoreCase);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                data = new ConcurrentDictionary<string, List<byte>>(StringComparer.Ordinal);
            }
            else
            {
                throw new NotSupportedException($"The operating system '{RuntimeInformation.OSDescription}' is not supported.");
            }
        }

        /// <inheritdoc/>
        public void CopyFile(string sourcePath, string destinationPath, bool overwrite)
        {
            if (sourcePath == null) throw new ArgumentNullException(nameof(sourcePath));
            sourcePath = Path.GetFullPath(sourcePath);
            if (!data.ContainsKey(sourcePath) || data[sourcePath] == null)
            {
                throw new FileNotFoundException();
            }

            if (destinationPath == null) throw new ArgumentNullException(nameof(destinationPath));
            destinationPath = Path.GetFullPath(destinationPath);
            if (data.ContainsKey(destinationPath))
            {
                if (data[destinationPath] == null)
                {
                    throw new ArgumentException($"{nameof(destinationPath)} specifies a directory.");
                }
                else if (!overwrite)
                {
                    throw new IOException("Destination file already exists and overwrite was not specified.");
                }
            }

            data[destinationPath] = [.. data[sourcePath]];
        }

        /// <inheritdoc/>
        public void CreateDirectory(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            CreateDirectoryRecurse(path);
        }

        /// <inheritdoc/>
        public void CreateFile(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            path = Path.GetFullPath(path);

            if (!data.ContainsKey(path))
            {
                CreateDirectoryRecurse(Path.GetDirectoryName(path));
                data[path] = [];
            }
            else if (data[path] == null)
            {
                throw new IOException("The path is a directory.");
            }
        }

        /// <inheritdoc/>
        public void DeleteDirectory(string path, bool recursive)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            path = Path.GetFullPath(path);
            if (!data.ContainsKey(path)) throw new DirectoryNotFoundException();
            if (data[path] != null)
            {
                throw new IOException("A file with the same name and location specified by path exists.");
            }

            var children = GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

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

        /// <inheritdoc/>
        public void DeleteFile(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            path = Path.GetFullPath(path);
            if (!data.ContainsKey(path)) throw new FileNotFoundException();

            if (data[path] == null)
            {
                throw new UnauthorizedAccessException("path is a directory.");
            }

            data.Remove(path);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (searchPattern == null) throw new ArgumentNullException(nameof(searchPattern));
            return GetFileSystemEntries(path, searchPattern, searchOption, includeDirectories: true, includeFiles: false);
        }

        /// <inheritdoc/>
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (searchPattern == null) throw new ArgumentNullException(nameof(searchPattern));
            return GetFileSystemEntries(path, searchPattern, searchOption, includeDirectories: false, includeFiles: true);
        }

        /// <inheritdoc/>
        public void HardLinkFile(string sourcePath, string destinationPath, bool overwrite)
        {
            if (sourcePath == null) throw new ArgumentNullException(nameof(sourcePath));
            sourcePath = Path.GetFullPath(sourcePath);
            if (!data.ContainsKey(sourcePath) || data[sourcePath] == null)
            {
                throw new FileNotFoundException();
            }

            if (destinationPath == null) throw new ArgumentNullException(nameof(destinationPath));
            destinationPath = Path.GetFullPath(destinationPath);
            if (data.ContainsKey(destinationPath))
            {
                if (data[destinationPath] == null)
                {
                    throw new ArgumentException($"{nameof(destinationPath)} specifies a directory.");
                }
                else if (!overwrite)
                {
                    throw new IOException("Destination file already exists and overwrite was not specified.");
                }
            }

            data[destinationPath] = data[sourcePath];
        }

        /// <inheritdoc/>
        public void MoveFile(string sourcePath, string destinationPath, bool overwrite)
        {
            if (sourcePath == null) throw new ArgumentNullException(nameof(sourcePath));
            sourcePath = Path.GetFullPath(sourcePath);
            if (!data.ContainsKey(sourcePath) || data[sourcePath] == null)
            {
                throw new FileNotFoundException($"{nameof(sourcePath)} was not found.");
            }

            if (destinationPath == null) throw new ArgumentNullException(nameof(destinationPath));
            destinationPath = Path.GetFullPath(destinationPath);
            if (data.ContainsKey(destinationPath))
            {
                if (data[destinationPath] == null)
                {
                    throw new ArgumentException($"{nameof(destinationPath)} specifies a directory.");
                }
                else if (!overwrite)
                {
                    throw new IOException("Destination file already exists and overwrite was not specified.");
                }
            }

            data[destinationPath] = [.. data[sourcePath]];
            data.Remove(sourcePath);
        }

        /// <inheritdoc/>
        public FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            path = Path.GetFullPath(path);

            if (data.ContainsKey(path) && data[path] == null)
            {
                throw new UnauthorizedAccessException("path specified a directory.");
            }

            return new MockFileStream(this, path, mode, access, share);
        }

        /// <summary>
        /// Recursively creates directories for the specified path.
        /// </summary>
        /// <param name="path">The directory path to create.</param>
        private void CreateDirectoryRecurse(string path)
        {
            CreateDirectoryRecurse(Path.GetFullPath(path).Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Recursively creates directories for the specified path parts.
        /// </summary>
        /// <param name="pathParts">The parts of the directory path.</param>
        private void CreateDirectoryRecurse(string[] pathParts)
        {
            if (pathParts.Length > 0)
            {
                CreateDirectoryRecurse([.. pathParts.Take(pathParts.Length - 1)]);

                var path = Path.GetFullPath(Path.Combine(pathParts));

                if (data.ContainsKey(path) && data[path] != null)
                {
                    throw new IOException("The path is a file.");
                }

                data[path] = null;
            }
        }

        /// <summary>
        /// Gets file system entries (files or directories) matching the specified criteria.
        /// </summary>
        /// <param name="ancestorPath">The ancestor directory path.</param>
        /// <param name="searchPattern">The search pattern to match.</param>
        /// <param name="searchOption">The search option.</param>
        /// <param name="includeDirectories">Whether to include directories.</param>
        /// <param name="includeFiles">Whether to include files.</param>
        /// <returns>An array of matching file system entry paths.</returns>
        private string[] GetFileSystemEntries(string ancestorPath, string searchPattern, SearchOption searchOption, bool includeDirectories, bool includeFiles)
        {
            var searchPatternRegex = GetSearchPatternRegex(searchPattern);
            var ancestorPathParts = Path.GetFullPath(ancestorPath).Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);

            var result = new List<string>();
            foreach (var path in data.Keys)
            {
                if (includeDirectories == (data[path] == null) || includeFiles == (data[path] != null))
                {
                    var pathParts = Path.GetFullPath(path).Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);

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

            return [.. result];
        }

        /// <summary>
        /// Compiles a regular expression for the given search pattern.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>A <see cref="Regex"/> for the search pattern.</returns>
        /// <exception cref="ArgumentException">Thrown if the search pattern is invalid.</exception>
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
