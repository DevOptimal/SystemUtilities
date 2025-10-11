# API Reference

This document provides a comprehensive reference for all public APIs in the DevOptimal.SystemUtilities library collection.

## Library Overview

The SystemUtilities collection consists of three main libraries:

- **[DevOptimal.SystemUtilities.Environment](#environment-library-api)** - Environment variable abstractions
- **[DevOptimal.SystemUtilities.FileSystem](#filesystem-library-api)** - File system abstractions and utilities
- **[DevOptimal.SystemUtilities.Registry](#registry-library-api)** - Windows Registry abstractions

## Environment Library API

### Namespace: `DevOptimal.SystemUtilities.Environment.Abstractions`

#### IEnvironment Interface

Core interface for environment variable operations.

```csharp
public interface IEnvironment
{
    string GetEnvironmentVariable(string name, EnvironmentVariableTarget target);
    void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target);
}
```

**Methods:**

- **`GetEnvironmentVariable(string name, EnvironmentVariableTarget target)`**
  - **Description**: Retrieves the value of an environment variable from the specified target
  - **Parameters**:
    - `name`: The name of the environment variable
    - `target`: The location where the environment variable is stored
  - **Returns**: The value of the environment variable, or null if not found
  - **Example**:
    ```csharp
    string path = environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
    ```

- **`SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target)`**
  - **Description**: Sets the value of an environment variable for the specified target
  - **Parameters**:
    - `name`: The name of the environment variable
    - `value`: The value to assign (null to delete the variable)
    - `target`: The location where the environment variable is stored
  - **Example**:
    ```csharp
    environment.SetEnvironmentVariable("MY_VAR", "value", EnvironmentVariableTarget.Process);
    ```

#### DefaultEnvironment Class

Standard implementation using `System.Environment`.

```csharp
public class DefaultEnvironment : IEnvironment
{
    public string GetEnvironmentVariable(string name, EnvironmentVariableTarget target);
    public void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target);
}
```

#### MockEnvironment Class

In-memory implementation for testing.

```csharp
public class MockEnvironment : IEnvironment
{
    public MockEnvironment();
    public string GetEnvironmentVariable(string name, EnvironmentVariableTarget target);
    public void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target);
}
```

### Namespace: `DevOptimal.SystemUtilities.Environment.StateManagement`

#### EnvironmentSnapshotter Class

Provides snapshotting functionality for environment variables.

```csharp
public class EnvironmentSnapshotter : Snapshotter
{
    public EnvironmentSnapshotter();
    public EnvironmentSnapshotter(IEnvironment environment);
    public EnvironmentSnapshotter(DirectoryInfo persistenceDirectory);
    public EnvironmentSnapshotter(IEnvironment environment, DirectoryInfo persistenceDirectory);
    
    public ISnapshot SnapshotEnvironmentVariable(string name, EnvironmentVariableTarget target);
}
```

**Constructors:**
- **`EnvironmentSnapshotter()`**: Uses default environment and persistence directory
- **`EnvironmentSnapshotter(IEnvironment environment)`**: Uses specified environment
- **`EnvironmentSnapshotter(DirectoryInfo persistenceDirectory)`**: Uses specified persistence directory
- **`EnvironmentSnapshotter(IEnvironment environment, DirectoryInfo persistenceDirectory)`**: Full customization

**Methods:**
- **`SnapshotEnvironmentVariable(string name, EnvironmentVariableTarget target)`**: Creates a snapshot of the specified environment variable

---

## FileSystem Library API

### Namespace: `DevOptimal.SystemUtilities.FileSystem.Abstractions`

#### IFileSystem Interface

Core interface for file system operations.

```csharp
public interface IFileSystem
{
    // File operations
    void CopyFile(string sourcePath, string destinationPath, bool overwrite);
    void CreateFile(string path);
    void DeleteFile(string path);
    bool FileExists(string path);
    void HardLinkFile(string sourcePath, string destinationPath, bool overwrite);
    void MoveFile(string sourcePath, string destinationPath, bool overwrite);
    FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);
    
    // Directory operations
    void CreateDirectory(string path);
    void DeleteDirectory(string path, bool recursive);
    bool DirectoryExists(string path);
    string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);
    string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
}
```

**File Operation Methods:**

- **`CopyFile(string sourcePath, string destinationPath, bool overwrite)`**
  - Copies a file from source to destination
  - **Parameters**: source path, destination path, whether to overwrite

- **`CreateFile(string path)`**
  - Creates an empty file at the specified path

- **`DeleteFile(string path)`**
  - Deletes the file at the specified path

- **`FileExists(string path)`**
  - Returns true if the file exists

- **`HardLinkFile(string sourcePath, string destinationPath, bool overwrite)`**
  - Creates a hard link from source to destination

- **`MoveFile(string sourcePath, string destinationPath, bool overwrite)`**
  - Moves a file from source to destination

- **`OpenFile(string path, FileMode mode, FileAccess access, FileShare share)`**
  - Opens a file with specified mode, access, and sharing options

**Directory Operation Methods:**

- **`CreateDirectory(string path)`**
  - Creates a directory at the specified path

- **`DeleteDirectory(string path, bool recursive)`**
  - Deletes a directory, optionally recursively

- **`DirectoryExists(string path)`**
  - Returns true if the directory exists

- **`GetDirectories(string path, string searchPattern, SearchOption searchOption)`**
  - Gets subdirectory names matching the search pattern

- **`GetFiles(string path, string searchPattern, SearchOption searchOption)`**
  - Gets file names matching the search pattern

#### DefaultFileSystem Class

Standard implementation using `System.IO` APIs.

```csharp
public class DefaultFileSystem : IFileSystem
{
    // Implements all IFileSystem methods using standard .NET APIs
}
```

#### MockFileSystem Class

In-memory implementation for testing.

```csharp
public class MockFileSystem : IFileSystem
{
    public MockFileSystem();
    // Implements all IFileSystem methods using in-memory storage
}
```

### Namespace: `DevOptimal.SystemUtilities.FileSystem.Extensions`

#### DirectoryInfoExtensions Class

Extension methods for `DirectoryInfo`.

```csharp
public static class DirectoryInfoExtensions
{
    // Equality and comparison
    public static bool Equals(this DirectoryInfo directory, DirectoryInfo other, IEqualityComparer<DirectoryInfo> comparer);
    public static bool Exists(this DirectoryInfo directory, IFileSystem fileSystem);
    
    // Navigation
    public static DirectoryInfo GetDirectory(this DirectoryInfo directory, string name);
    public static DirectoryInfo GetDirectory(this DirectoryInfo directory, params string[] names);
    public static FileInfo GetFile(this DirectoryInfo directory, string name);
    public static FileInfo GetFile(this DirectoryInfo directory, params string[] names);
    public static DriveInfo Drive { get; }
    
    // File system operations
    public static void Create(this DirectoryInfo directory, IFileSystem fileSystem);
    public static void Delete(this DirectoryInfo directory, IFileSystem fileSystem, bool recursive);
    public static DirectoryInfo[] GetDirectories(this DirectoryInfo directory, IFileSystem fileSystem);
    public static DirectoryInfo[] GetDirectories(this DirectoryInfo directory, IFileSystem fileSystem, string searchPattern, bool recursive);
    public static FileInfo[] GetFiles(this DirectoryInfo directory, IFileSystem fileSystem);
    public static FileInfo[] GetFiles(this DirectoryInfo directory, IFileSystem fileSystem, string searchPattern, bool recursive);
    
    // Relationship methods
    public static bool IsAncestorOf(this DirectoryInfo directory, DirectoryInfo other);
    public static bool IsDescendantOf(this DirectoryInfo directory, DirectoryInfo other);
}
```

#### FileInfoExtensions Class

Extension methods for `FileInfo`.

```csharp
public static class FileInfoExtensions
{
    // Equality and existence
    public static bool Equals(this FileInfo file, FileInfo other, IEqualityComparer<FileInfo> comparer);
    public static bool Exists(this FileInfo file, IFileSystem fileSystem);
    
    // File operations
    public static void CopyTo(this FileInfo file, string destinationPath, IFileSystem fileSystem, bool overwrite);
    public static void Delete(this FileInfo file, IFileSystem fileSystem);
    public static void HardLinkTo(this FileInfo file, string destinationPath, IFileSystem fileSystem);
    public static void MoveTo(this FileInfo file, string destinationPath, IFileSystem fileSystem, bool overwrite);
    
    // Content operations
    public static string ReadAllText(this FileInfo file, IFileSystem fileSystem);
    public static string ReadAllText(this FileInfo file, Encoding encoding, IFileSystem fileSystem);
    public static void WriteAllText(this FileInfo file, string content, IFileSystem fileSystem);
    public static void WriteAllText(this FileInfo file, string content, Encoding encoding, IFileSystem fileSystem);
    
    // Stream operations
    public static FileStream OpenRead(this FileInfo file, IFileSystem fileSystem);
    public static FileStream OpenWrite(this FileInfo file, IFileSystem fileSystem);
}
```

#### DriveInfoExtensions Class

Extension methods for `DriveInfo`.

```csharp
public static class DriveInfoExtensions
{
    public static bool Equals(this DriveInfo drive, DriveInfo other, IEqualityComparer<DriveInfo> comparer);
    public static DirectoryInfo[] GetDirectories(this DriveInfo drive, IFileSystem fileSystem);
    public static FileInfo[] GetFiles(this DriveInfo drive, IFileSystem fileSystem, string searchPattern, bool recursive);
}
```

#### StringExtensions Class

Extension methods for converting strings to file system objects.

```csharp
public static class StringExtensions
{
    public static DirectoryInfo AsDirectoryInfo(this string path);
    public static FileInfo AsFileInfo(this string path);
    public static DriveInfo AsDriveInfo(this string path);
}
```

#### IFileSystemExtensions Class

Extension methods for `IFileSystem`.

```csharp
public static class IFileSystemExtensions
{
    // Simplified directory operations
    public static void DeleteDirectory(this IFileSystem fileSystem, string path);
    public static string[] GetDirectories(this IFileSystem fileSystem, string path, bool recursive);
    public static string[] GetFiles(this IFileSystem fileSystem, string path, bool recursive);
    
    // Content operations
    public static string ReadAllText(this IFileSystem fileSystem, string path);
    public static string ReadAllText(this IFileSystem fileSystem, string path, Encoding encoding);
    public static void WriteAllText(this IFileSystem fileSystem, string path, string content);
    public static void WriteAllText(this IFileSystem fileSystem, string path, string content, Encoding encoding);
}
```

### Namespace: `DevOptimal.SystemUtilities.FileSystem.Comparers`

#### FileSystemInfoComparer Class

Base comparer for file system objects with platform-aware comparison.

```csharp
public class FileSystemInfoComparer : IEqualityComparer<FileSystemInfo>
{
    public static FileSystemInfoComparer Default { get; }
    
    public bool Equals(FileSystemInfo x, FileSystemInfo y);
    public int GetHashCode(FileSystemInfo obj);
}
```

#### FileInfoComparer Class

Comparer specifically for `FileInfo` objects.

```csharp
public class FileInfoComparer : IEqualityComparer<FileInfo>
{
    public static FileInfoComparer Default { get; }
    
    public bool Equals(FileInfo x, FileInfo y);
    public int GetHashCode(FileInfo obj);
}
```

#### DirectoryInfoComparer Class

Comparer specifically for `DirectoryInfo` objects.

```csharp
public class DirectoryInfoComparer : IEqualityComparer<DirectoryInfo>
{
    public static DirectoryInfoComparer Default { get; }
    
    public bool Equals(DirectoryInfo x, DirectoryInfo y);
    public int GetHashCode(DirectoryInfo obj);
}
```

#### DriveInfoComparer Class

Comparer specifically for `DriveInfo` objects.

```csharp
public class DriveInfoComparer : IEqualityComparer<DriveInfo>
{
    public static DriveInfoComparer Default { get; }
    
    public bool Equals(DriveInfo x, DriveInfo y);
    public int GetHashCode(DriveInfo obj);
}
```

#### WindowsFileSystemInfoComparer Class

Windows-specific comparer that always uses case-insensitive comparison.

```csharp
public class WindowsFileSystemInfoComparer : IEqualityComparer<FileSystemInfo>
{
    public static WindowsFileSystemInfoComparer Default { get; }
    
    public bool Equals(FileSystemInfo x, FileSystemInfo y);
    public int GetHashCode(FileSystemInfo obj);
}
```

### Namespace: `DevOptimal.SystemUtilities.FileSystem.StateManagement`

#### FileSystemSnapshotter Class

Provides snapshotting functionality for file system resources.

```csharp
public class FileSystemSnapshotter : Snapshotter
{
    public FileSystemSnapshotter();
    public FileSystemSnapshotter(IFileSystem fileSystem);
    public FileSystemSnapshotter(IFileSystem fileSystem, DirectoryInfo persistenceDirectory);
    public FileSystemSnapshotter(IFileSystem fileSystem, IFileCache fileCache, DirectoryInfo persistenceDirectory);
    
    public ISnapshot SnapshotDirectory(string path);
    public ISnapshot SnapshotFile(string path);
}
```

### Namespace: `DevOptimal.SystemUtilities.FileSystem`

#### TemporaryFile Class

Represents a temporary file that is automatically cleaned up.

```csharp
public class TemporaryFile : IDisposable
{
    public TemporaryFile();
    public TemporaryFile(IFileSystem fileSystem);
    
    public string Path { get; }
    
    public void Dispose();
}
```

---

## Registry Library API

### Namespace: `DevOptimal.SystemUtilities.Registry.Abstractions`

#### IRegistry Interface

Core interface for Windows Registry operations.

```csharp
public interface IRegistry
{
    // Key operations
    void CreateRegistryKey(RegistryHive hive, RegistryView view, string subKey);
    void DeleteRegistryKey(RegistryHive hive, RegistryView view, string subKey, bool recursive);
    bool RegistryKeyExists(RegistryHive hive, RegistryView view, string subKey);
    
    // Value operations
    (object value, RegistryValueKind kind) GetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name);
    void SetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name, object value, RegistryValueKind kind);
    void DeleteRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name);
    bool RegistryValueExists(RegistryHive hive, RegistryView view, string subKey, string name);
}
```

**Key Operation Methods:**

- **`CreateRegistryKey(RegistryHive hive, RegistryView view, string subKey)`**
  - Creates a registry key at the specified location
  - **Parameters**: hive (e.g., CurrentUser), view (32/64-bit), subkey path

- **`DeleteRegistryKey(RegistryHive hive, RegistryView view, string subKey, bool recursive)`**
  - Deletes a registry key, optionally recursively

- **`RegistryKeyExists(RegistryHive hive, RegistryView view, string subKey)`**
  - Returns true if the registry key exists

**Value Operation Methods:**

- **`GetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)`**
  - Retrieves a value and its kind from a registry key
  - **Returns**: Tuple containing the value and its `RegistryValueKind`

- **`SetRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name, object value, RegistryValueKind kind)`**
  - Sets a value in a registry key

- **`DeleteRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)`**
  - Deletes a value from a registry key

- **`RegistryValueExists(RegistryHive hive, RegistryView view, string subKey, string name)`**
  - Returns true if the registry value exists

#### DefaultRegistry Class

Standard implementation using `Microsoft.Win32.Registry` APIs.

```csharp
public class DefaultRegistry : IRegistry
{
    // Implements all IRegistry methods using standard .NET Registry APIs
}
```

#### MockRegistry Class

In-memory implementation for testing.

```csharp
public class MockRegistry : IRegistry
{
    public MockRegistry();
    // Implements all IRegistry methods using in-memory storage
}
```

### Namespace: `DevOptimal.SystemUtilities.Registry`

#### RegistryPath Class

Utility methods for working with registry paths.

```csharp
public static class RegistryPath
{
    public static string GetFullPath(string subKey);
}
```

**Methods:**

- **`GetFullPath(string subKey)`**
  - Normalizes a registry subkey path by trimming separators and removing duplicates
  - **Example**: `GetFullPath(@"\Software\\MyApp\")` returns `"Software\MyApp"`

### Namespace: `DevOptimal.SystemUtilities.Registry.StateManagement`

#### RegistrySnapshotter Class

Provides snapshotting functionality for registry resources.

```csharp
public class RegistrySnapshotter : Snapshotter
{
    public RegistrySnapshotter();
    public RegistrySnapshotter(IRegistry registry);
    public RegistrySnapshotter(DirectoryInfo persistenceDirectory);
    public RegistrySnapshotter(IRegistry registry, DirectoryInfo persistenceDirectory);
    
    public ISnapshot SnapshotRegistryKey(RegistryHive hive, RegistryView view, string subKey);
    public ISnapshot SnapshotRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name);
}
```

**Methods:**

- **`SnapshotRegistryKey(RegistryHive hive, RegistryView view, string subKey)`**
  - Creates a snapshot of an entire registry key and all its values

- **`SnapshotRegistryValue(RegistryHive hive, RegistryView view, string subKey, string name)`**
  - Creates a snapshot of a specific registry value

---

## Common State Management API

### Namespace: `DevOptimal.SystemUtilities.Common.StateManagement`

#### ISnapshot Interface

Represents a snapshot of system state that can be restored.

```csharp
public interface ISnapshot : IDisposable
{
    void Dispose();
}
```

#### Snapshotter Base Class

Base class for all snapshotter implementations.

```csharp
public abstract class Snapshotter
{
    protected static DirectoryInfo defaultPersistenceDirectory { get; }
    
    public void RestoreAbandonedSnapshots();
}
```

**Methods:**

- **`RestoreAbandonedSnapshots()`**
  - Restores any snapshots that were not properly disposed in previous process runs

---

## Usage Examples

### Complete Example: Multi-Resource Configuration Manager

```csharp
using DevOptimal.SystemUtilities.Environment.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;

public class SystemConfigurationManager
{
    private readonly IEnvironment _environment;
    private readonly IFileSystem _fileSystem;
    private readonly IRegistry _registry;
    
    public SystemConfigurationManager(
        IEnvironment environment = null,
        IFileSystem fileSystem = null,
        IRegistry registry = null)
    {
        _environment = environment ?? new DefaultEnvironment();
        _fileSystem = fileSystem ?? new DefaultFileSystem();
        _registry = registry ?? new DefaultRegistry();
    }
    
    public void ConfigureApplication(string appName, string version, string installPath)
    {
        // Configure environment
        _environment.SetEnvironmentVariable($"{appName}_HOME", installPath, EnvironmentVariableTarget.User);
        _environment.SetEnvironmentVariable($"{appName}_VERSION", version, EnvironmentVariableTarget.User);
        
        // Create application directories
        var configDir = Path.Combine(installPath, "config");
        _fileSystem.CreateDirectory(configDir);
        
        // Write configuration file
        var configFile = Path.Combine(configDir, "app.config");
        _fileSystem.WriteAllText(configFile, $"Version={version}\nInstallPath={installPath}");
        
        // Register in Windows Registry
        var registryPath = $@"Software\{appName}";
        _registry.CreateRegistryKey(RegistryHive.CurrentUser, RegistryView.Default, registryPath);
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, registryPath, 
            "Version", version, RegistryValueKind.String);
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, registryPath, 
            "InstallPath", installPath, RegistryValueKind.String);
    }
}
```

### Testing Example Using All Mock Implementations

```csharp
[TestClass]
public class SystemConfigurationManagerTests
{
    private MockEnvironment _mockEnvironment;
    private MockFileSystem _mockFileSystem;
    private MockRegistry _mockRegistry;
    private SystemConfigurationManager _configManager;
    
    [TestInitialize]
    public void Setup()
    {
        _mockEnvironment = new MockEnvironment();
        _mockFileSystem = new MockFileSystem();
        _mockRegistry = new MockRegistry();
        _configManager = new SystemConfigurationManager(_mockEnvironment, _mockFileSystem, _mockRegistry);
    }
    
    [TestMethod]
    public void ConfigureApplication_ConfiguresAllResources()
    {
        // Act
        _configManager.ConfigureApplication("TestApp", "1.0.0", @"C:\TestApp");
        
        // Assert Environment
        Assert.AreEqual(@"C:\TestApp", _mockEnvironment.GetEnvironmentVariable("TestApp_HOME", EnvironmentVariableTarget.User));
        Assert.AreEqual("1.0.0", _mockEnvironment.GetEnvironmentVariable("TestApp_VERSION", EnvironmentVariableTarget.User));
        
        // Assert FileSystem
        Assert.IsTrue(_mockFileSystem.DirectoryExists(@"C:\TestApp\config"));
        Assert.IsTrue(_mockFileSystem.FileExists(@"C:\TestApp\config\app.config"));
        
        // Assert Registry
        Assert.IsTrue(_mockRegistry.RegistryKeyExists(RegistryHive.CurrentUser, RegistryView.Default, @"Software\TestApp"));
        var (version, _) = _mockRegistry.GetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, @"Software\TestApp", "Version");
        Assert.AreEqual("1.0.0", version);
    }
}
```

This API reference provides comprehensive coverage of all public interfaces, classes, and methods available in the DevOptimal.SystemUtilities library collection.