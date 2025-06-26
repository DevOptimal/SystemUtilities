using Microsoft.Win32;
using System.IO;
using System.Linq;

namespace DevOptimal.SystemUtilities.Registry.Abstractions
{
    /// <summary>
    /// Provides a default implementation of the <see cref="IRegistry"/> interface
    /// for interacting with the Windows Registry.
    /// </summary>
    public class DefaultRegistry : IRegistry
    {
        /// <inheritdoc />
        public void CreateRegistryKey(RegistryHive hive, RegistryView view, string subKey)
        {
            RegistryKey.OpenBaseKey(hive, view).CreateSubKey(subKey);
        }

        /// <inheritdoc />
        public void DeleteRegistryKey(RegistryHive hive, RegistryView view, string subKey, bool recursive)
        {
            var baseKey = RegistryKey.OpenBaseKey(hive, view);
            var key = baseKey.OpenSubKey(subKey);

            if (!recursive && (key.GetSubKeyNames().Any() || key.GetValueNames().Any()))
            {
                throw new IOException("The registry key is not empty.");
            }

            baseKey.DeleteSubKeyTree(subKey, throwOnMissingSubKey: true);
        }

        /// <inheritdoc />
        public void DeleteRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            RegistryKey.OpenBaseKey(hive, view).OpenSubKey(subKey, writable: true).DeleteValue(name, throwOnMissingValue: false);
        }

        /// <inheritdoc />
        public (object value, RegistryValueKind kind) GetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            var regKey = RegistryKey.OpenBaseKey(hive, view).OpenSubKey(subKey);
            return (regKey?.GetValue(name), regKey?.GetValueKind(name) ?? RegistryValueKind.None);
        }

        /// <inheritdoc />
        public bool RegistryKeyExists(RegistryHive hive, RegistryView view, string subKey)
        {
            return RegistryKey.OpenBaseKey(hive, view).OpenSubKey(subKey) != null;
        }

        /// <inheritdoc />
        public bool RegistryValueExists(RegistryHive hive, RegistryView view, string subKey, string name)
        {
            return RegistryKey.OpenBaseKey(hive, view).OpenSubKey(subKey).GetValue(name) != null;
        }

        /// <inheritdoc />
        public void SetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name, object value, RegistryValueKind kind)
        {
            RegistryKey.OpenBaseKey(hive, view).OpenSubKey(subKey, writable: true).SetValue(name, value, kind);
        }
    }
}
