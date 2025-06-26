using System;
using System.IO;

namespace DevOptimal.SystemUtilities.FileSystem.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveInvalidPathChars(this string path) =>
            ReplaceInvalidPathChars(path, string.Empty);

        public static string ReplaceInvalidPathChars(this string path, char replaceWith) =>
            ReplaceInvalidPathChars(path, replaceWith.ToString());

        public static string ReplaceInvalidPathChars(this string path, string replaceWith)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            char[] invalidChars = Path.GetInvalidPathChars();
            foreach (char c in invalidChars)
            {
                path = path.Replace(c.ToString(), replaceWith);
            }
            return path;
        }

        public static string[] SplitOnDirectorySeparator(this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return path.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitOnPathSeparator(this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return path.Split([Path.PathSeparator], StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
