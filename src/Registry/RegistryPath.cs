namespace DevOptimal.SystemUtilities.Registry
{
    /// <summary>
    /// Provides utility methods for working with Windows Registry paths.
    /// </summary>
    public static class RegistryPath
    {
        /// <summary>
        /// Normalizes a registry subkey path by trimming leading and trailing backslashes
        /// and replacing any double backslashes with a single backslash.
        /// </summary>
        /// <param name="subKey">The registry subkey path to normalize.</param>
        /// <returns>The normalized registry subkey path, or null if the input is null.</returns>
        public static string GetFullPath(string subKey)
        {
            if (subKey != null)
            {
                // Remove leading and trailing backslashes
                subKey = subKey.Trim('\\');
                string lastSubKey;
                // Replace any double backslashes with a single backslash until none remain
                do
                {
                    lastSubKey = subKey;
                    subKey = subKey.Replace("\\\\", "\\");
                } while (subKey != lastSubKey);
            }

            return subKey;
        }
    }
}
