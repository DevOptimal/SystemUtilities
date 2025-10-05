using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.Comparers;
using System;
using System.Collections.Generic;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="DirectoryInfo"/>.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        extension(DirectoryInfo directory)
        {
            /// <summary>
            /// Determines whether two <see cref="DirectoryInfo"/> instances are equal using the specified comparer.
            /// </summary>
            /// <param name="other">The second directory.</param>
            /// <param name="comparer">The comparer to use.</param>
            /// <returns>True if the directories are equal; otherwise, false.</returns>
            public bool Equals(DirectoryInfo other, IEqualityComparer<DirectoryInfo> comparer)
            {
                if (comparer == null) throw new ArgumentNullException(nameof(comparer));

                return comparer.Equals(directory, other);
            }

            /// <summary>
            /// Determines whether the directory exists using the specified file system abstraction.
            /// </summary>
            /// <param name="fileSystem">The file system abstraction.</param>
            /// <returns>True if the directory exists; otherwise, false.</returns>
            public bool Exists(IFileSystem fileSystem)
            {
                if (directory == null) throw new ArgumentNullException(nameof(directory));
                if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));

                return fileSystem.DirectoryExists(directory.FullName);
            }

            /// <summary>
            /// Gets a subdirectory with the specified name.
            /// </summary>
            /// <param name="name">The name of the subdirectory.</param>
            /// <returns>A <see cref="DirectoryInfo"/> representing the subdirectory.</returns>
            public DirectoryInfo GetDirectory(string name)
            {
                if (directory == null) throw new ArgumentNullException(nameof(directory));
                if (name == null) throw new ArgumentNullException(nameof(name));

                // Calls the overload that accepts multiple names for path composition.
                return directory.GetDirectory([name]);
            }

            /// <summary>
            /// Gets a subdirectory by combining the specified path segments.
            /// </summary>
            /// <param name="names">The path segments to combine.</param>
            /// <returns>A <see cref="DirectoryInfo"/> representing the subdirectory.</returns>
            public DirectoryInfo GetDirectory(params string[] names)
            {
                if (directory == null) throw new ArgumentNullException(nameof(directory));
                if (names == null) throw new ArgumentNullException(nameof(names));

                return new DirectoryInfo(Path.Combine(directory.FullName, Path.Combine(names)));
            }

            /// <summary>
            /// Gets the drive that contains the specified directory.
            /// </summary>
            /// <returns>A <see cref="DriveInfo"/> representing the drive.</returns>
            public DriveInfo Drive
            {
                get
                {
                    if (directory == null) throw new ArgumentNullException(nameof(directory));

                    return new DriveInfo(Path.GetPathRoot(directory.FullName));
                }
            }

            /// <summary>
            /// Gets a file with the specified name in the directory.
            /// </summary>
            /// <param name="name">The name of the file.</param>
            /// <returns>A <see cref="FileInfo"/> representing the file.</returns>
            public FileInfo GetFile(string name)
            {
                if (directory == null) throw new ArgumentNullException(nameof(directory));
                if (name == null) throw new ArgumentNullException(nameof(name));

                // Calls the overload that accepts multiple names for path composition.
                return directory.GetFile([name]);
            }

            /// <summary>
            /// Gets a file by combining the specified path segments.
            /// </summary>
            /// <param name="names">The path segments to combine.</param>
            /// <returns>A <see cref="FileInfo"/> representing the file.</returns>
            public FileInfo GetFile(params string[] names)
            {
                if (directory == null) throw new ArgumentNullException(nameof(directory));
                if (names == null) throw new ArgumentNullException(nameof(names));

                return new FileInfo(Path.Combine(directory.FullName, Path.Combine(names)));
            }

            /// <summary>
            /// Determines whether a directory is an ancestor of another directory.
            /// </summary>
            /// <param name="descendant">The descendant to test.</param>
            /// <returns>True if <paramref name="directory"/> is an ancestor of <paramref name="descendant"/>, false otherwise.</returns>
            public bool IsAncestorOf(DirectoryInfo descendant)
            {
                // Uses IsDescendantOf for the actual logic.
                return descendant.IsDescendantOf(directory);
            }

            /// <summary>
            /// Determines whether a directory is a descendant of another directory.
            /// </summary>
            /// <param name="ancestor">The ancestor directory to test.</param>
            /// <returns>True if <paramref name="directory"/> is a descendant of <paramref name="ancestor"/>, false otherwise.</returns>
            public bool IsDescendantOf(DirectoryInfo ancestor)
            {
                return directory.IsDescendantOf(ancestor, new DirectoryInfoComparer());
            }

            /// <summary>
            /// Determines whether a directory is a descendant of another directory using a custom comparer.
            /// </summary>
            /// <param name="ancestor">The ancestor directory to test.</param>
            /// <param name="comparer">The comparer to use.</param>
            /// <returns>True if <paramref name="directory"/> is a descendant of <paramref name="ancestor"/>, false otherwise.</returns>
            internal bool IsDescendantOf(DirectoryInfo ancestor, DirectoryInfoComparer comparer)
            {
                if (directory == null || ancestor == null)
                {
                    return false;
                }

                var current = directory.Parent;
                // Traverse up the directory tree to check for ancestry.
                while (current != null)
                {
                    if (comparer.Equals(current, ancestor))
                    {
                        return true;
                    }
                    current = current.Parent;
                }

                return false;
            }
        }
    }
}
