# Getting Started with DevOptimal.SystemUtilities

Welcome to DevOptimal.SystemUtilities! This guide will help you get up and running with the library collection and understand how to use it effectively in your .NET applications.

## What is DevOptimal.SystemUtilities?

DevOptimal.SystemUtilities is a collection of .NET libraries that provide abstractions for common system resources like environment variables, file systems, and the Windows Registry. These abstractions make your code more testable, maintainable, and flexible by enabling dependency injection and mocking.

## Why Use These Libraries?

### Before: Tightly Coupled to System Resources

```csharp
public class DocumentProcessor
{
    public void ProcessDocument(string inputPath)
    {
        // Directly coupled to System.IO
        if (!File.Exists(inputPath))
            throw new FileNotFoundException($"File not found: {inputPath}");
            
        var content = File.ReadAllText(inputPath);
        
        // Directly coupled to System.Environment
        var outputDir = Environment.GetEnvironmentVariable("OUTPUT_DIR");
        if (string.IsNullOrEmpty(outputDir))
            outputDir = @"C:\temp";
            
        var outputPath = Path.Combine(outputDir, "processed_" + Path.GetFileName(inputPath));
        File.WriteAllText(outputPath, ProcessContent(content));
    }
}
```

**Problems with this approach:**
- ❌ Hard to unit test (requires real files and environment variables)
- ❌ Difficult to mock system behavior
- ❌ Tightly coupled to system dependencies
- ❌ Can't easily test error scenarios

### After: Using SystemUtilities Abstractions

```csharp
public class DocumentProcessor
{
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironment _environment;
    
    public DocumentProcessor(IFileSystem fileSystem, IEnvironment environment)
    {
        _fileSystem = fileSystem ?? new DefaultFileSystem();
        _environment = environment ?? new DefaultEnvironment();
    }
    
    public void ProcessDocument(string inputPath)
    {
        // Uses abstractions - easy to mock and test
        if (!_fileSystem.FileExists(inputPath))
            throw new FileNotFoundException($"File not found: {inputPath}");
            
        var content = _fileSystem.ReadAllText(inputPath);
        
        var outputDir = _environment.GetEnvironmentVariable("OUTPUT_DIR", EnvironmentVariableTarget.Process);
        if (string.IsNullOrEmpty(outputDir))
            outputDir = @"C:\temp";
            
        var outputPath = Path.Combine(outputDir, "processed_" + Path.GetFileName(inputPath));
        _fileSystem.WriteAllText(outputPath, ProcessContent(content));
    }
}
```

**Benefits of this approach:**
- ✅ Easy to unit test with mock implementations
- ✅ Loosely coupled through dependency injection
- ✅ Can test error scenarios and edge cases
- ✅ Follows SOLID principles

## Installation

Each library is available as a separate NuGet package, allowing you to install only what you need.

### Install via .NET CLI

```bash
# Install all libraries
dotnet add package DevOptimal.SystemUtilities.Environment
dotnet add package DevOptimal.SystemUtilities.FileSystem
dotnet add package DevOptimal.SystemUtilities.Registry

# Or install only what you need
dotnet add package DevOptimal.SystemUtilities.FileSystem
```

### Install via Package Manager Console

```powershell
# Install all libraries
Install-Package DevOptimal.SystemUtilities.Environment
Install-Package DevOptimal.SystemUtilities.FileSystem
Install-Package DevOptimal.SystemUtilities.Registry

# Or install only what you need
Install-Package DevOptimal.SystemUtilities.FileSystem
```

### Install via PackageReference

Add to your `.csproj` file:

```xml
<PackageReference Include="DevOptimal.SystemUtilities.Environment" Version="*" />
<PackageReference Include="DevOptimal.SystemUtilities.FileSystem" Version="*" />
<PackageReference Include="DevOptimal.SystemUtilities.Registry" Version="*" />
```

## Quick Start Examples

### 1. Basic File Operations

Let's start with a simple example that demonstrates file operations:

```csharp
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.Extensions;

public class ConfigurationManager
{
    private readonly IFileSystem _fileSystem;
    private readonly string _configPath;
    
    public ConfigurationManager(string configPath, IFileSystem fileSystem = null)
    {
        _configPath = configPath;
        _fileSystem = fileSystem ?? new DefaultFileSystem();
    }
    
    public void SaveConfiguration(Dictionary<string, string> settings)
    {
        // Ensure the directory exists
        var configDir = Path.GetDirectoryName(_configPath);
        if (!_fileSystem.DirectoryExists(configDir))
        {
            _fileSystem.CreateDirectory(configDir);
        }
        
        // Serialize and save the configuration
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        _fileSystem.WriteAllText(_configPath, json);
    }
    
    public Dictionary<string, string> LoadConfiguration()
    {
        if (!_fileSystem.FileExists(_configPath))
        {
            return new Dictionary<string, string>();
        }
        
        var json = _fileSystem.ReadAllText(_configPath);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
    }
}

// Usage
var configManager = new ConfigurationManager(@"C:\MyApp\config.json");
configManager.SaveConfiguration(new Dictionary<string, string>
{
    ["Theme"] = "Dark",
    ["Language"] = "en-US"
});
```

### 2. Environment Variable Management

```csharp
using DevOptimal.SystemUtilities.Environment.Abstractions;

public class ApplicationEnvironment
{
    private readonly IEnvironment _environment;
    
    public ApplicationEnvironment(IEnvironment environment = null)
    {
        _environment = environment ?? new DefaultEnvironment();
    }
    
    public void SetupDevelopmentEnvironment()
    {
        _environment.SetEnvironmentVariable("NODE_ENV", "development", EnvironmentVariableTarget.Process);
        _environment.SetEnvironmentVariable("DEBUG", "true", EnvironmentVariableTarget.Process);
        _environment.SetEnvironmentVariable("LOG_LEVEL", "verbose", EnvironmentVariableTarget.Process);
    }
    
    public bool IsProduction()
    {
        var nodeEnv = _environment.GetEnvironmentVariable("NODE_ENV", EnvironmentVariableTarget.Process);
        return string.Equals(nodeEnv, "production", StringComparison.OrdinalIgnoreCase);
    }
    
    public string GetConnectionString()
    {
        // Check multiple targets in order of preference
        return _environment.GetEnvironmentVariable("DB_CONNECTION", EnvironmentVariableTarget.Process)
            ?? _environment.GetEnvironmentVariable("DB_CONNECTION", EnvironmentVariableTarget.User)
            ?? _environment.GetEnvironmentVariable("DB_CONNECTION", EnvironmentVariableTarget.Machine)
            ?? throw new InvalidOperationException("Database connection string not configured");
    }
}
```

### 3. Registry Operations (Windows)

```csharp
using DevOptimal.SystemUtilities.Registry.Abstractions;
using Microsoft.Win32;

public class ApplicationRegistry
{
    private readonly IRegistry _registry;
    private const string AppKeyPath = @"Software\MyCompany\MyApp";
    
    public ApplicationRegistry(IRegistry registry = null)
    {
        _registry = registry ?? new DefaultRegistry();
    }
    
    public void RegisterApplication(string version, string installPath)
    {
        // Create the application key
        _registry.CreateRegistryKey(RegistryHive.CurrentUser, RegistryView.Default, AppKeyPath);
        
        // Set application information
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, AppKeyPath, 
            "Version", version, RegistryValueKind.String);
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, AppKeyPath, 
            "InstallPath", installPath, RegistryValueKind.String);
        _registry.SetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, AppKeyPath, 
            "InstalledDate", DateTime.Now.ToString("yyyy-MM-dd"), RegistryValueKind.String);
    }
    
    public string GetInstalledVersion()
    {
        var (value, _) = _registry.GetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, AppKeyPath, "Version");
        return value?.ToString() ?? "Unknown";
    }
    
    public bool IsApplicationRegistered()
    {
        return _registry.RegistryKeyExists(RegistryHive.CurrentUser, RegistryView.Default, AppKeyPath);
    }
}
```

## Writing Unit Tests

One of the main benefits of using these libraries is how easy it becomes to write unit tests. Here's how to test the examples above:

### Testing File Operations

```csharp
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ConfigurationManagerTests
{
    private MockFileSystem _mockFileSystem;
    private ConfigurationManager _configManager;
    private const string ConfigPath = @"C:\TestApp\config.json";
    
    [TestInitialize]
    public void Setup()
    {
        _mockFileSystem = new MockFileSystem();
        _configManager = new ConfigurationManager(ConfigPath, _mockFileSystem);
    }
    
    [TestMethod]
    public void SaveConfiguration_CreatesDirectoryAndFile()
    {
        // Arrange
        var settings = new Dictionary<string, string>
        {
            ["Theme"] = "Dark",
            ["Language"] = "en-US"
        };
        
        // Act
        _configManager.SaveConfiguration(settings);
        
        // Assert
        Assert.IsTrue(_mockFileSystem.DirectoryExists(@"C:\TestApp"));
        Assert.IsTrue(_mockFileSystem.FileExists(ConfigPath));
        
        var content = _mockFileSystem.ReadAllText(ConfigPath);
        var deserializedSettings = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
        Assert.AreEqual("Dark", deserializedSettings["Theme"]);
        Assert.AreEqual("en-US", deserializedSettings["Language"]);
    }
    
    [TestMethod]
    public void LoadConfiguration_ReturnsEmptyDictionary_WhenFileDoesNotExist()
    {
        // Act
        var result = _configManager.LoadConfiguration();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }
    
    [TestMethod]
    public void LoadConfiguration_ReturnsStoredSettings()
    {
        // Arrange
        var expectedSettings = new Dictionary<string, string>
        {
            ["Theme"] = "Light",
            ["Language"] = "fr-FR"
        };
        
        _mockFileSystem.CreateDirectory(@"C:\TestApp");
        var json = JsonSerializer.Serialize(expectedSettings);
        _mockFileSystem.WriteAllText(ConfigPath, json);
        
        // Act
        var result = _configManager.LoadConfiguration();
        
        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Light", result["Theme"]);
        Assert.AreEqual("fr-FR", result["Language"]);
    }
}
```

### Testing Environment Variables

```csharp
using DevOptimal.SystemUtilities.Environment.Abstractions;

[TestClass]
public class ApplicationEnvironmentTests
{
    private MockEnvironment _mockEnvironment;
    private ApplicationEnvironment _appEnvironment;
    
    [TestInitialize]
    public void Setup()
    {
        _mockEnvironment = new MockEnvironment();
        _appEnvironment = new ApplicationEnvironment(_mockEnvironment);
    }
    
    [TestMethod]
    public void SetupDevelopmentEnvironment_SetsAllVariables()
    {
        // Act
        _appEnvironment.SetupDevelopmentEnvironment();
        
        // Assert
        Assert.AreEqual("development", _mockEnvironment.GetEnvironmentVariable("NODE_ENV", EnvironmentVariableTarget.Process));
        Assert.AreEqual("true", _mockEnvironment.GetEnvironmentVariable("DEBUG", EnvironmentVariableTarget.Process));
        Assert.AreEqual("verbose", _mockEnvironment.GetEnvironmentVariable("LOG_LEVEL", EnvironmentVariableTarget.Process));
    }
    
    [TestMethod]
    public void IsProduction_ReturnsFalse_InDevelopment()
    {
        // Arrange
        _mockEnvironment.SetEnvironmentVariable("NODE_ENV", "development", EnvironmentVariableTarget.Process);
        
        // Act & Assert
        Assert.IsFalse(_appEnvironment.IsProduction());
    }
    
    [TestMethod]
    public void GetConnectionString_FallsBackToUserVariable()
    {
        // Arrange
        _mockEnvironment.SetEnvironmentVariable("DB_CONNECTION", "UserConnection", EnvironmentVariableTarget.User);
        
        // Act
        var result = _appEnvironment.GetConnectionString();
        
        // Assert
        Assert.AreEqual("UserConnection", result);
    }
}
```

## State Management and Snapshots

One of the powerful features of these libraries is the ability to snapshot and restore system state. This is particularly useful for integration tests and scenarios where you need to make temporary changes.

### Using FileSystem Snapshots

```csharp
using DevOptimal.SystemUtilities.FileSystem.StateManagement;

public class DocumentProcessor
{
    private readonly FileSystemSnapshotter _snapshotter;
    
    public DocumentProcessor()
    {
        _snapshotter = new FileSystemSnapshotter();
    }
    
    public void ProcessDocumentsWithBackup(string directoryPath)
    {
        // Create a snapshot of the entire directory
        using var snapshot = _snapshotter.SnapshotDirectory(directoryPath);
        
        try
        {
            // Process all documents (this might modify or delete files)
            var files = Directory.GetFiles(directoryPath, "*.txt");
            foreach (var file in files)
            {
                ProcessDocument(file);
            }
            
            // If processing succeeds, we can let the snapshot dispose normally
            // If any exception occurs, the directory state will be automatically restored
        }
        catch (Exception ex)
        {
            // Log the error - the snapshot will automatically restore the directory
            Console.WriteLine($"Processing failed: {ex.Message}");
            throw;
        }
    }
}
```

### Using Environment Variable Snapshots

```csharp
using DevOptimal.SystemUtilities.Environment.StateManagement;

public class TestRunner
{
    private readonly EnvironmentSnapshotter _snapshotter;
    
    public TestRunner()
    {
        _snapshotter = new EnvironmentSnapshotter();
    }
    
    public async Task RunTestWithEnvironmentAsync(Dictionary<string, string> testEnvironment, Func<Task> testAction)
    {
        var snapshots = new List<ISnapshot>();
        
        try
        {
            // Snapshot all environment variables that will be modified
            foreach (var kvp in testEnvironment)
            {
                var snapshot = _snapshotter.SnapshotEnvironmentVariable(kvp.Key, EnvironmentVariableTarget.Process);
                snapshots.Add(snapshot);
            }
            
            // Set up the test environment
            foreach (var kvp in testEnvironment)
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value, EnvironmentVariableTarget.Process);
            }
            
            // Run the test
            await testAction();
        }
        finally
        {
            // Restore all environment variables
            foreach (var snapshot in snapshots)
            {
                snapshot.Dispose();
            }
        }
    }
}
```

## Dependency Injection Setup

These libraries work great with dependency injection. Here's how to set them up in different frameworks:

### ASP.NET Core

```csharp
// Program.cs or Startup.cs
using DevOptimal.SystemUtilities.Environment.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.Registry.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Register system utilities
builder.Services.AddSingleton<IEnvironment, DefaultEnvironment>();
builder.Services.AddSingleton<IFileSystem, DefaultFileSystem>();

// Only register Registry on Windows
if (OperatingSystem.IsWindows())
{
    builder.Services.AddSingleton<IRegistry, DefaultRegistry>();
}

// Register your application services
builder.Services.AddScoped<ConfigurationManager>();
builder.Services.AddScoped<ApplicationEnvironment>();

var app = builder.Build();

// Your application setup...
```

### Generic Host / Console Application

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    // Register system utilities
    services.AddSingleton<IEnvironment, DefaultEnvironment>();
    services.AddSingleton<IFileSystem, DefaultFileSystem>();
    
    if (OperatingSystem.IsWindows())
    {
        services.AddSingleton<IRegistry, DefaultRegistry>();
    }
    
    // Register your services
    services.AddTransient<DocumentProcessor>();
    services.AddTransient<ApplicationEnvironment>();
});

var host = builder.Build();

// Use your services
using var scope = host.Services.CreateScope();
var documentProcessor = scope.ServiceProvider.GetRequiredService<DocumentProcessor>();
```

### Manual DI Container Setup

```csharp
// Using any DI container (example with simple constructor injection)
public class ServiceProvider
{
    private readonly IEnvironment _environment = new DefaultEnvironment();
    private readonly IFileSystem _fileSystem = new DefaultFileSystem();
    private readonly IRegistry _registry = new DefaultRegistry();
    
    public ConfigurationManager GetConfigurationManager(string configPath)
    {
        return new ConfigurationManager(configPath, _fileSystem);
    }
    
    public ApplicationEnvironment GetApplicationEnvironment()
    {
        return new ApplicationEnvironment(_environment);
    }
    
    public ApplicationRegistry GetApplicationRegistry()
    {
        return new ApplicationRegistry(_registry);
    }
}
```

## Common Patterns and Best Practices

### 1. Factory Pattern for Cross-Platform Code

```csharp
public static class SystemUtilitiesFactory
{
    public static IFileSystem CreateFileSystem()
    {
        return new DefaultFileSystem();
    }
    
    public static IEnvironment CreateEnvironment()
    {
        return new DefaultEnvironment();
    }
    
    public static IRegistry CreateRegistry()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Registry is only supported on Windows");
        }
        
        return new DefaultRegistry();
    }
}
```

### 2. Configuration Management Pattern

```csharp
public class SystemConfiguration
{
    private readonly IEnvironment _environment;
    private readonly IFileSystem _fileSystem;
    private readonly IRegistry _registry;
    
    public SystemConfiguration(IEnvironment environment, IFileSystem fileSystem, IRegistry registry = null)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _registry = registry; // Optional on non-Windows platforms
    }
    
    public T GetConfiguration<T>(string key, T defaultValue = default)
    {
        // Try environment first
        var envValue = _environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        if (!string.IsNullOrEmpty(envValue))
        {
            return ConvertValue<T>(envValue);
        }
        
        // Try config file
        var configPath = GetConfigFilePath();
        if (_fileSystem.FileExists(configPath))
        {
            var config = LoadConfigFile(configPath);
            if (config.ContainsKey(key))
            {
                return ConvertValue<T>(config[key]);
            }
        }
        
        // Try registry (Windows only)
        if (_registry != null && OperatingSystem.IsWindows())
        {
            if (_registry.RegistryValueExists(RegistryHive.CurrentUser, RegistryView.Default, @"Software\MyApp", key))
            {
                var (value, _) = _registry.GetRegistryValue(RegistryHive.CurrentUser, RegistryView.Default, @"Software\MyApp", key);
                return ConvertValue<T>(value?.ToString());
            }
        }
        
        return defaultValue;
    }
}
```

### 3. Testing Helper Pattern

```csharp
public static class TestSystemUtilities
{
    public static (MockEnvironment env, MockFileSystem fs, MockRegistry reg) CreateMockSystem()
    {
        return (new MockEnvironment(), new MockFileSystem(), new MockRegistry());
    }
    
    public static void SetupTestEnvironment(MockEnvironment env, Dictionary<string, string> variables)
    {
        foreach (var kvp in variables)
        {
            env.SetEnvironmentVariable(kvp.Key, kvp.Value, EnvironmentVariableTarget.Process);
        }
    }
    
    public static void SetupTestFileSystem(MockFileSystem fs, Dictionary<string, string> files)
    {
        foreach (var kvp in files)
        {
            var directory = Path.GetDirectoryName(kvp.Key);
            if (!string.IsNullOrEmpty(directory) && !fs.DirectoryExists(directory))
            {
                fs.CreateDirectory(directory);
            }
            fs.WriteAllText(kvp.Key, kvp.Value);
        }
    }
}

// Usage in tests
[TestMethod]
public void TestComplexScenario()
{
    // Arrange
    var (env, fs, reg) = TestSystemUtilities.CreateMockSystem();
    
    TestSystemUtilities.SetupTestEnvironment(env, new Dictionary<string, string>
    {
        ["APP_ENV"] = "test",
        ["DEBUG"] = "true"
    });
    
    TestSystemUtilities.SetupTestFileSystem(fs, new Dictionary<string, string>
    {
        [@"C:\config\app.json"] = "{\"theme\": \"dark\"}",
        [@"C:\data\input.txt"] = "test data"
    });
    
    var service = new MyService(env, fs, reg);
    
    // Act & Assert
    // Your test logic here...
}
```

## Next Steps

Now that you've learned the basics, here are some resources to help you go deeper:

1. **[API Reference](api-reference.md)** - Complete API documentation for all libraries
2. **[FileSystem Library](filesystem.md)** - Deep dive into file system abstractions and extensions
3. **[Environment Library](environment.md)** - Complete guide to environment variable management
4. **[Registry Library](registry.md)** - Windows Registry operations and utilities
5. **[State Management](state-management.md)** - Advanced snapshotting and restoration features
6. **[Abstractions Overview](abstractions.md)** - Core concepts and design patterns

## Getting Help

If you encounter issues or have questions:

1. Check the [API Reference](api-reference.md) for detailed method documentation
2. Review the comprehensive examples in each library's documentation
3. Look at the unit tests in the source code for usage patterns
4. Open an issue on the GitHub repository for bugs or feature requests

Happy coding with DevOptimal.SystemUtilities! 🚀