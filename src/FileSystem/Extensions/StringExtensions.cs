using System;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    /// <summary>
    /// Provides extension methods for string manipulation related to file system paths.
    /// </summary>
    public static class StringExtensions
    {
        extension(string path)
        {
            /// <summary>
            /// Removes all invalid path characters from the specified string.
            /// </summary>
            /// <returns>
            /// A new string with all invalid path characters removed.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <paramref name="path"/> is <c>null</c>.
            /// </exception>
            public string RemoveInvalidPathChars() =>
                ReplaceInvalidPathChars(path, string.Empty);

            /// <summary>
            /// Replaces all invalid path characters in the specified string with the given character.
            /// </summary>
            /// <param name="replaceWith">The character to replace invalid path characters with.</param>
            /// <returns>
            /// A new string with all invalid path characters replaced by <paramref name="replaceWith"/>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <paramref name="path"/> is <c>null</c>.
            /// </exception>
            public string ReplaceInvalidPathChars(char replaceWith) =>
                ReplaceInvalidPathChars(path, replaceWith.ToString());

            /// <summary>
            /// Replaces all invalid path characters in the specified string with the given replacement string.
            /// </summary>
            /// <param name="replaceWith">The string to replace invalid path characters with.</param>
            /// <returns>
            /// A new string with all invalid path characters replaced by <paramref name="replaceWith"/>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <paramref name="path"/> is <c>null</c>.
            /// </exception>
            public string ReplaceInvalidPathChars(string replaceWith)
            {
                if (path == null) throw new ArgumentNullException(nameof(path));
                char[] invalidChars = Path.GetInvalidPathChars();
                foreach (char c in invalidChars)
                {
                    path = path.Replace(c.ToString(), replaceWith);
                }
                return path;
            }

            /// <summary>
            /// Splits the specified path string into segments using directory separator characters (e.g. '/' or '\').
            /// </summary>
            /// <returns>
            /// An array of path segments, split on directory separator characters.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <paramref name="path"/> is <c>null</c>.
            /// </exception>
            public string[] SplitOnDirectorySeparator()
            {
                if (path == null) throw new ArgumentNullException(nameof(path));
                return path.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);
            }

            /// <summary>
            /// Splits the specified path string into segments using the path separator character (e.g. ';').
            /// </summary>
            /// <returns>
            /// An array of path segments, split on the path separator character.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <paramref name="path"/> is <c>null</c>.
            /// </exception>
            public string[] SplitOnPathSeparator()
            {
                if (path == null) throw new ArgumentNullException(nameof(path));
                return path.Split([Path.PathSeparator], StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
