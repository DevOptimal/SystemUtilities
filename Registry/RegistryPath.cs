namespace DevOptimal.System.Resources.Registry
{
    public static class RegistryPath
    {
        public static string GetFullPath(string subKey)
        {
            if (subKey != null)
            {
                subKey = subKey.Trim('\\');
                string lastSubKey;
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
