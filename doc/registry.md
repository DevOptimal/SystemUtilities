# Registry Library

The Registry library provides a clean abstraction layer for working with the Windows Registry, making it easy to write testable code that interacts with registry keys and values. It includes interfaces, implementations, utilities, and state management features.

## Overview

The Registry library consists of several key components:

- **Abstractions**: Core interfaces and implementations for registry operations
- **Utilities**: Helper classes for registry path manipulation
- **State Management**: Snapshot and restore capabilities for registry resources
- **Testing Support**: Mock implementations for unit testing

## Key Features

- ✅ **Testable Design**: Mock implementations for unit testing
- ✅ **Windows Registry Support**: Full access to all registry hives and views
- ✅ **State Management**: Snapshot and restore registry state
- ✅ **Type Safety**: Strongly-typed interfaces and implementations
- ✅ **Path Utilities**: Helper methods for registry path manipulation
- ✅ **Multiple Views**: Support for 32-bit and 64-bit registry views

## Core Abstractions

### IRegistry Interface

The `IRegistry` interface provides a complete abstraction over Windows Registry operations:

```csharp
public interface IRegistry
{
    // Key operations
    void CreateRegistryKey(RegistryHive hive, RegistryView view, string subKey);
    void DeleteRegistryKey(RegistryHive hive, RegistryView view, string subKey, bool recursive);
    bool RegistryKeyExists(RegistryHive hive, RegistryView view, string subKey);
    
    // Value operations
    void SetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name, object value, RegistryValueKind kind);
    (object value, RegistryValueKind kind) GetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name);
    void DeleteRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name);
    bool RegistryValueExists(RegistryHive hive, RegistryView view, string subKey, string name);
}
```

### Registry Hives and Views

The library supports all standard Windows Registry hives and views:

**Registry Hives:**
- **`RegistryHive.CurrentUser`**: HKEY_CURRENT_USER - Settings for the current user
- **`RegistryHive.LocalMachine`**: HKEY_LOCAL_MACHINE - System-wide settings
- **`RegistryHive.ClassesRoot`**: HKEY_CLASSES_ROOT - File associations and COM registration
- **`RegistryHive.Users`**: HKEY_USERS - Settings for all users
- **`RegistryHive.CurrentConfig`**: HKEY_CURRENT_CONFIG - Current hardware profile

**Registry Views:**
- **`RegistryView.Registry32`**: 32-bit registry view
- **`RegistryView.Registry64`**: 64-bit registry view (on 64-bit systems)
- **`RegistryView.Default`**: Default view based on the process architecture

### DefaultRegistry

The `DefaultRegistry` class provides the standard implementation that uses the built-in .NET registry APIs:

```csharp
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;

// Create a default registry instance
IRegistry registry = new DefaultRegistry();

// Create a registry key
registry.CreateRegistryKey(RegistryHive.CurrentUser, RegistryView.Default, @"Software\MyApp");

// Set a string value
registry.SetRegistryValue(
    RegistryHive.CurrentUser, 
    RegistryView.Default, 
    @"Software\MyApp", 
    "Version", 
    "1.0.0", 
    RegistryValueKind.String);

// Get a value
var (value, kind) = registry.GetRegistryValue(
    RegistryHive.CurrentUser, 
    RegistryView.Default, 
    @"Software\MyApp", 
    "Version");
```

### MockRegistry

The `MockRegistry` class provides an in-memory implementation perfect for unit testing:

```csharp
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;

// Create a mock registry for testing
IRegistry mockRegistry = new MockRegistry();

// Use it in tests without affecting the real registry
mockRegistry.CreateRegistryKey(RegistryHive.CurrentUser, RegistryView.Default, @"Software\TestApp");
mockRegistry.SetRegistryValue(
    RegistryHive.CurrentUser, 
    RegistryView.Default, 
    @"Software\TestApp", 
    "TestValue", 
    "test data", 
    RegistryValueKind.String);

Assert.IsTrue(mockRegistry.RegistryValueExists(
    RegistryHive.CurrentUser, 
    RegistryView.Default, 
    @"Software\TestApp", 
    "TestValue"));
```

## Registry Path Utilities

The `RegistryPath` class provides utility methods for working with registry paths:

```csharp
using DevOptimal.SystemUtilities.Registry;

// Normalize registry paths
string normalizedPath = RegistryPath.GetFullPath(@"\Software\\MyApp\");
// Result: "Software\MyApp"

// Handles various path formats
string path1 = RegistryPath.GetFullPath(@"Software\MyApp");     // "Software\MyApp"
string path2 = RegistryPath.GetFullPath(@"\Software\MyApp\");   // "Software\MyApp"
string path3 = RegistryPath.GetFullPath(@"\\Software\\MyApp\\"); // "Software\MyApp"
```

## State Management

The Registry library includes powerful state management capabilities through the `RegistrySnapshotter` class.

### Creating a Snapshotter

```csharp
using DevOptimal.SystemUtilities.Registry.StateManagement;

// Create a snapshotter with default settings
var snapshotter = new RegistrySnapshotter();

// Or with a custom registry
var snapshotter = new RegistrySnapshotter(mockRegistry);

// Or with a custom persistence directory
var snapshotter = new RegistrySnapshotter(registry, persistenceDirectory);
```

### Snapshotting Registry Keys

```csharp
// Create a snapshot of a registry key
using var snapshot = snapshotter.SnapshotRegistryKey(
    RegistryHive.CurrentUser, 
    RegistryView.Default, 
    @"Software\MyApp");

// Modify or delete the key
Registry.CurrentUser.DeleteSubKeyTree(@"Software\MyApp", false);

// The original key and all its values are automatically restored when disposed
```

### Snapshotting Registry Values

```csharp
// Create a snapshot of a specific registry value
using var snapshot = snapshotter.SnapshotRegistryValue(
    RegistryHive.CurrentUser, 
    RegistryView.Default, 
    @"Software\MyApp", 
    "Version");

// Modify the value
Registry.SetValue(@"HKEY_CURRENT_USER\Software\MyApp", "Version", "2.0.0");

// The original value is automatically restored when disposed
```

### Handling Abandoned Snapshots

If your application crashes before snapshots are properly disposed, you can restore abandoned snapshots:

```csharp
// At application startup, restore any abandoned snapshots
snapshotter.RestoreAbandonedSnapshots();
```

## Usage Examples

### Application Configuration Manager

```csharp
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;

public class AppConfigurationManager
{
    private readonly IRegistry _registry;
    private const string ConfigKeyPath = @"Software\MyCompany\MyApp";
    
    public AppConfigurationManager(IRegistry registry)
    {
        _registry = registry ?? new DefaultRegistry();
    }
    
    public void InitializeConfiguration()
    {
        // Ensure the configuration key exists
        if (!_registry.RegistryKeyExists(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath))
        {
            _registry.CreateRegistryKey(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath);
            SetDefaultConfiguration();
        }
    }
    
    private void SetDefaultConfiguration()
    {
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath, 
            "Language", "en-US", RegistryValueKind.String);
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath, 
            "AutoSave", true, RegistryValueKind.DWord);
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath, 
            "MaxRecentFiles", 10, RegistryValueKind.DWord);
    }
    
    public string GetLanguage()
    {
        var (value, _) = _registry.GetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath, "Language");
        return value?.ToString() ?? "en-US";
    }
    
    public void SetLanguage(string language)
    {
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath, 
            "Language", language, RegistryValueKind.String);
    }
    
    public bool GetAutoSave()
    {
        var (value, _) = _registry.GetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath, "AutoSave");
        return value is int intValue && intValue != 0;
    }
    
    public void SetAutoSave(bool autoSave)
    {
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath, 
            "AutoSave", autoSave ? 1 : 0, RegistryValueKind.DWord);
    }
    
    public void ResetConfiguration()
    {
        if (_registry.RegistryKeyExists(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath))
        {
            _registry.DeleteRegistryKey(RegistryHive.CurrentUser, RegistryView.Default, ConfigKeyPath, recursive: true);
        }
        InitializeConfiguration();
    }
}
```

### Testing with MockRegistry

```csharp
[TestClass]
public class AppConfigurationManagerTests
{
    private MockRegistry _mockRegistry;
    private AppConfigurationManager _configManager;
    
    [TestInitialize]
    public void Setup()
    {
        _mockRegistry = new MockRegistry();
        _configManager = new AppConfigurationManager(_mockRegistry);
    }
    
    [TestMethod]
    public void InitializeConfiguration_CreatesDefaultSettings()
    {
        // Act
        _configManager.InitializeConfiguration();
        
        // Assert
        Assert.IsTrue(_mockRegistry.RegistryKeyExists(RegistryHive.CurrentUser, RegistryView.Default, @"Software\MyCompany\MyApp"));
        Assert.AreEqual("en-US", _configManager.GetLanguage());
        Assert.IsTrue(_configManager.GetAutoSave());
    }
    
    [TestMethod]
    public void SetLanguage_UpdatesRegistryValue()
    {
        // Arrange
        _configManager.InitializeConfiguration();
        
        // Act
        _configManager.SetLanguage("fr-FR");
        
        // Assert
        Assert.AreEqual("fr-FR", _configManager.GetLanguage());
        
        var (value, kind) = _mockRegistry.GetRegistryValue(
            RegistryHive.CurrentUser, RegistryView.Default, 
            @"Software\MyCompany\MyApp", "Language");
        Assert.AreEqual("fr-FR", value);
        Assert.AreEqual(RegistryValueKind.String, kind);
    }
    
    [TestMethod]
    public void ResetConfiguration_RestoresDefaults()
    {
        // Arrange
        _configManager.InitializeConfiguration();
        _configManager.SetLanguage("de-DE");
        _configManager.SetAutoSave(false);
        
        // Act
        _configManager.ResetConfiguration();
        
        // Assert
        Assert.AreEqual("en-US", _configManager.GetLanguage());
        Assert.IsTrue(_configManager.GetAutoSave());
    }
}
```

### Software Installation Registry Manager

```csharp
using DevOptimal.SystemUtilities.Registry.Abstractions;
using DevOptimal.SystemUtilities.Registry.StateManagement;
using Microsoft.Win32;

public class SoftwareRegistryManager
{
    private readonly IRegistry _registry;
    private readonly RegistrySnapshotter _snapshotter;
    private const string UninstallKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
    
    public SoftwareRegistryManager(IRegistry registry)
    {
        _registry = registry ?? new DefaultRegistry();
        _snapshotter = new RegistrySnapshotter(_registry);
    }
    
    public void RegisterApplication(string appId, string displayName, string version, string installPath)
    {
        var appKeyPath = $@"{UninstallKeyPath}\{appId}";
        
        // Create the application registry key
        _registry.CreateRegistryKey(RegistryHive.LocalMachine, RegistryView.Default, appKeyPath);
        
        // Set application information
        _registry.SetRegistryValue(RegistryHive.LocalMachine, RegistryView.Default, appKeyPath, 
            "DisplayName", displayName, RegistryValueKind.String);
        _registry.SetRegistryValue(RegistryHive.LocalMachine, RegistryView.Default, appKeyPath, 
            "DisplayVersion", version, RegistryValueKind.String);
        _registry.SetRegistryValue(RegistryHive.LocalMachine, RegistryView.Default, appKeyPath, 
            "InstallLocation", installPath, RegistryValueKind.String);
        _registry.SetRegistryValue(RegistryHive.LocalMachine, RegistryView.Default, appKeyPath, 
            "Publisher", "My Company", RegistryValueKind.String);
        _registry.SetRegistryValue(RegistryHive.LocalMachine, RegistryView.Default, appKeyPath, 
            "UninstallString", $"{installPath}\\uninstall.exe", RegistryValueKind.String);
    }
    
    public void UnregisterApplication(string appId)
    {
        var appKeyPath = $@"{UninstallKeyPath}\{appId}";
        
        if (_registry.RegistryKeyExists(RegistryHive.LocalMachine, RegistryView.Default, appKeyPath))
        {
            _registry.DeleteRegistryKey(RegistryHive.LocalMachine, RegistryView.Default, appKeyPath, recursive: true);
        }
    }
    
    public bool IsApplicationRegistered(string appId)
    {
        var appKeyPath = $@"{UninstallKeyPath}\{appId}";
        return _registry.RegistryKeyExists(RegistryHive.LocalMachine, RegistryView.Default, appKeyPath);
    }
    
    public void PerformSafeRegistryOperation(Action registryOperation)
    {
        // Snapshot the entire uninstall key before making changes
        using var snapshot = _snapshotter.SnapshotRegistryKey(
            RegistryHive.LocalMachine, 
            RegistryView.Default, 
            UninstallKeyPath);
        
        try
        {
            registryOperation();
        }
        catch
        {
            // If an exception occurs, the snapshot will automatically restore the registry state
            throw;
        }
    }
}
```

### Registry Configuration with Backup and Restore

```csharp
using DevOptimal.SystemUtilities.Registry.Abstractions;
using DevOptimal.SystemUtilities.Registry.StateManagement;

public class RegistryConfigurationManager
{
    private readonly IRegistry _registry;
    private readonly RegistrySnapshotter _snapshotter;
    
    public RegistryConfigurationManager(IRegistry registry)
    {
        _registry = registry ?? new DefaultRegistry();
        _snapshotter = new RegistrySnapshotter(_registry);
    }
    
    public async Task UpdateConfigurationSafelyAsync(
        RegistryHive hive, 
        RegistryView view, 
        string keyPath, 
        Dictionary<string, (object value, RegistryValueKind kind)> newValues)
    {
        var snapshots = new List<ISnapshot>();
        
        try
        {
            // Create snapshots for all values that will be modified
            foreach (var kvp in newValues)
            {
                var snapshot = _snapshotter.SnapshotRegistryValue(hive, view, keyPath, kvp.Key);
                snapshots.Add(snapshot);
            }
            
            // Ensure the key exists
            if (!_registry.RegistryKeyExists(hive, view, keyPath))
            {
                _registry.CreateRegistryKey(hive, view, keyPath);
            }
            
            // Apply all changes
            foreach (var kvp in newValues)
            {
                _registry.SetRegistryValue(hive, view, keyPath, kvp.Key, kvp.Value.value, kvp.Value.kind);
            }
            
            // Simulate some processing that might fail
            await ProcessConfigurationAsync();
            
            // If we reach here, commit the changes by disposing snapshots in reverse order
            for (int i = snapshots.Count - 1; i >= 0; i--)
            {
                snapshots[i].Dispose();
            }
            snapshots.Clear();
        }
        catch
        {
            // If anything fails, restore all snapshots
            foreach (var snapshot in snapshots)
            {
                snapshot.Dispose();
            }
            throw;
        }
    }
    
    private async Task ProcessConfigurationAsync()
    {
        // Simulate some async work that might fail
        await Task.Delay(100);
    }
}
```

## Installation

Install the Registry library via NuGet:

```bash
dotnet add package DevOptimal.SystemUtilities.Registry
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package DevOptimal.SystemUtilities.Registry
```

## Best Practices

1. **Use Dependency Injection**: Inject `IRegistry` into your classes rather than using `Microsoft.Win32.Registry` directly
2. **Mock for Testing**: Use `MockRegistry` for unit tests to avoid affecting the real registry
3. **Handle Permissions**: Registry operations may require elevated privileges, especially for HKEY_LOCAL_MACHINE
4. **Use State Management**: For operations that modify multiple registry entries, use `RegistrySnapshotter`
5. **Normalize Paths**: Use `RegistryPath.GetFullPath()` to normalize registry key paths
6. **Choose Appropriate Views**: Use the correct `RegistryView` for your application's architecture
7. **Dispose Resources**: Always dispose snapshots properly to restore original values

## Security Considerations

- **Administrative Rights**: Operations on HKEY_LOCAL_MACHINE typically require administrative privileges
- **Registry Views**: Be aware of registry redirection on 64-bit systems when using different views
- **Sensitive Data**: Be cautious about storing sensitive information in the registry
- **Backup Important Keys**: Always backup important registry keys before making modifications

## Platform Considerations

- **Windows Only**: The Registry library is Windows-specific and will not work on other platforms
- **Registry Views**: 32-bit and 64-bit views behave differently on 64-bit systems
- **Permissions**: Different registry hives have different permission requirements
- **Registry Redirection**: Windows automatically redirects some registry keys for 32-bit applications on 64-bit systems

## Registry Value Types

The library supports all standard Windows Registry value types:

- **`RegistryValueKind.String`**: REG_SZ - Null-terminated string
- **`RegistryValueKind.ExpandString`**: REG_EXPAND_SZ - Expandable string with environment variables
- **`RegistryValueKind.DWord`**: REG_DWORD - 32-bit number
- **`RegistryValueKind.QWord`**: REG_QWORD - 64-bit number
- **`RegistryValueKind.Binary`**: REG_BINARY - Binary data
- **`RegistryValueKind.MultiString`**: REG_MULTI_SZ - Array of strings

## Related Documentation

- [Abstractions Overview](abstractions.md) - Core abstraction concepts
- [State Management](state-management.md) - Snapshot and restore functionality
- [API Reference](api-reference.md) - Complete API documentation
- [Getting Started](getting-started.md) - Quick start guide