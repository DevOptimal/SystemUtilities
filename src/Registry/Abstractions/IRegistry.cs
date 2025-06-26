using Microsoft.Win32;

namespace DevOptimal.SystemUtilities.Registry.Abstractions
{
    /// <summary>
    /// Provides an abstraction for interacting with the Windows Registry.
    /// </summary>
    public interface IRegistry
    {
        /// <summary>
        /// Creates a registry key at the specified location.
        /// </summary>
        /// <param name="hive">The root hive of the registry.</param>
        /// <param name="view">The registry view (32-bit or 64-bit).</param>
        /// <param name="subKey">The path of the subkey to create.</param>
        void CreateRegistryKey(RegistryHive hive, RegistryView view, string subKey);

        /// <summary>
        /// Deletes a registry key at the specified location.
        /// </summary>
        /// <param name="hive">The root hive of the registry.</param>
        /// <param name="view">The registry view (32-bit or 64-bit).</param>
        /// <param name="subKey">The path of the subkey to delete.</param>
        /// <param name="recursive">Whether to delete subkeys recursively.</param>
        void DeleteRegistryKey(RegistryHive hive, RegistryView view, string subKey, bool recursive);

        /// <summary>
        /// Deletes a value from a registry key.
        /// </summary>
        /// <param name="hive">The root hive of the registry.</param>
        /// <param name="view">The registry view (32-bit or 64-bit).</param>
        /// <param name="subKey">The path of the subkey containing the value.</param>
        /// <param name="name">The name of the value to delete.</param>
        void DeleteRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name);

        /// <summary>
        /// Retrieves a value and its kind from a registry key.
        /// </summary>
        /// <param name="hive">The root hive of the registry.</param>
        /// <param name="view">The registry view (32-bit or 64-bit).</param>
        /// <param name="subKey">The path of the subkey containing the value.</param>
        /// <param name="name">The name of the value to retrieve.</param>
        /// <returns>A tuple containing the value and its kind.</returns>
        (object value, RegistryValueKind kind) GetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name);

        /// <summary>
        /// Determines whether a registry key exists at the specified location.
        /// </summary>
        /// <param name="hive">The root hive of the registry.</param>
        /// <param name="view">The registry view (32-bit or 64-bit).</param>
        /// <param name="subKey">The path of the subkey to check.</param>
        /// <returns>True if the key exists; otherwise, false.</returns>
        bool RegistryKeyExists(RegistryHive hive, RegistryView view, string subKey);

        /// <summary>
        /// Determines whether a value exists in a registry key.
        /// </summary>
        /// <param name="hive">The root hive of the registry.</param>
        /// <param name="view">The registry view (32-bit or 64-bit).</param>
        /// <param name="subKey">The path of the subkey containing the value.</param>
        /// <param name="name">The name of the value to check.</param>
        /// <returns>True if the value exists; otherwise, false.</returns>
        bool RegistryValueExists(RegistryHive hive, RegistryView view, string subKey, string name);

        /// <summary>
        /// Sets a value in a registry key.
        /// </summary>
        /// <param name="hive">The root hive of the registry.</param>
        /// <param name="view">The registry view (32-bit or 64-bit).</param>
        /// <param name="subKey">The path of the subkey to modify.</param>
        /// <param name="name">The name of the value to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="kind">The kind of the value.</param>
        void SetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name, object value, RegistryValueKind kind);
    }
}
