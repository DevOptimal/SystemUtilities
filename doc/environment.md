# Environment Library

The Environment library provides a clean abstraction layer for working with environment variables, making it easy to write testable code that interacts with system environment variables. It includes interfaces, implementations, and state management features.

## Overview

The Environment library consists of several key components:

- **Abstractions**: Core interfaces and implementations for environment variable operations
- **State Management**: Snapshot and restore capabilities for environment variables
- **Testing Support**: Mock implementations for unit testing

## Key Features

- ✅ **Testable Design**: Mock implementations for unit testing
- ✅ **Cross-Platform**: Works on Windows, macOS, and Linux
- ✅ **State Management**: Snapshot and restore environment variable state
- ✅ **Type Safety**: Strongly-typed interfaces and implementations
- ✅ **Multiple Targets**: Support for Process, User, and Machine-level variables

## Core Abstractions

### IEnvironment Interface

The `IEnvironment` interface provides a complete abstraction over environment variable operations:

```csharp
public interface IEnvironment
{
    /// <summary>
    /// Retrieves the value of an environment variable from the specified target.
    /// </summary>
    /// <param name="name">The name of the environment variable.</param>
    /// <param name="target">The location where the environment variable is stored.</param>
    /// <returns>The value of the environment variable, or null if the variable is not found.</returns>
    string GetEnvironmentVariable(string name, EnvironmentVariableTarget target);

    /// <summary>
    /// Sets the value of an environment variable for the specified target.
    /// </summary>
    /// <param name="name">The name of the environment variable.</param>
    /// <param name="value">The value to assign to the environment variable. If null, the variable is deleted.</param>
    /// <param name="target">The location where the environment variable is stored.</param>
    void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget target);
}
```

### Environment Variable Targets

The library supports all standard .NET environment variable targets:

- **`EnvironmentVariableTarget.Process`**: Variables for the current process only
- **`EnvironmentVariableTarget.User`**: Variables for the current user
- **`EnvironmentVariableTarget.Machine`**: System-wide variables (requires administrative privileges)

### DefaultEnvironment

The `DefaultEnvironment` class provides the standard implementation that uses the built-in .NET environment APIs:

```csharp
using DevOptimal.SystemUtilities.Environment.Abstractions;

// Create a default environment instance
IEnvironment environment = new DefaultEnvironment();

// Set a process-level environment variable
environment.SetEnvironmentVariable("MY_APP_CONFIG", "production", EnvironmentVariableTarget.Process);

// Get the variable value
string configValue = environment.GetEnvironmentVariable("MY_APP_CONFIG", EnvironmentVariableTarget.Process);
```

### MockEnvironment

The `MockEnvironment` class provides an in-memory implementation perfect for unit testing:

```csharp
using DevOptimal.SystemUtilities.Environment.Abstractions;

// Create a mock environment for testing
IEnvironment mockEnvironment = new MockEnvironment();

// Use it in tests without affecting the real environment
mockEnvironment.SetEnvironmentVariable("TEST_VAR", "test_value", EnvironmentVariableTarget.Process);
Assert.AreEqual("test_value", mockEnvironment.GetEnvironmentVariable("TEST_VAR", EnvironmentVariableTarget.Process));
```

## State Management

The Environment library includes powerful state management capabilities through the `EnvironmentSnapshotter` class.

### Creating a Snapshotter

```csharp
using DevOptimal.SystemUtilities.Environment.StateManagement;

// Create a snapshotter with default settings
var snapshotter = new EnvironmentSnapshotter();

// Or with a custom environment
var snapshotter = new EnvironmentSnapshotter(mockEnvironment);

// Or with a custom persistence directory
var snapshotter = new EnvironmentSnapshotter(environment, persistenceDirectory);
```

### Snapshotting Environment Variables

```csharp
// Create a snapshot of an environment variable
using var snapshot = snapshotter.SnapshotEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);

// Modify the environment variable
Environment.SetEnvironmentVariable("PATH", "/new/path", EnvironmentVariableTarget.Process);

// The original value is automatically restored when disposed
// (or call snapshot.Dispose() explicitly)
```

### Advanced Snapshotting Scenarios

```csharp
public void ConfigureApplicationEnvironment()
{
    var snapshotter = new EnvironmentSnapshotter();
    
    // Snapshot multiple variables that will be modified
    using var pathSnapshot = snapshotter.SnapshotEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
    using var configSnapshot = snapshotter.SnapshotEnvironmentVariable("CONFIG_FILE", EnvironmentVariableTarget.Process);
    using var debugSnapshot = snapshotter.SnapshotEnvironmentVariable("DEBUG_MODE", EnvironmentVariableTarget.Process);
    
    // Configure the environment for the application
    var currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
    Environment.SetEnvironmentVariable("PATH", $"{currentPath};C:\\MyApp\\bin", EnvironmentVariableTarget.Process);
    Environment.SetEnvironmentVariable("CONFIG_FILE", "C:\\MyApp\\config.json", EnvironmentVariableTarget.Process);
    Environment.SetEnvironmentVariable("DEBUG_MODE", "true", EnvironmentVariableTarget.Process);
    
    // Run the application with the modified environment
    RunApplication();
    
    // All environment variables are automatically restored when the snapshots are disposed
}
```

### Handling Abandoned Snapshots

If your application crashes before snapshots are properly disposed, you can restore abandoned snapshots:

```csharp
// At application startup, restore any abandoned snapshots
snapshotter.RestoreAbandonedSnapshots();
```

## Usage Examples

### Basic Environment Variable Operations

```csharp
using DevOptimal.SystemUtilities.Environment.Abstractions;

public class ConfigurationManager
{
    private readonly IEnvironment _environment;
    
    public ConfigurationManager(IEnvironment environment)
    {
        _environment = environment ?? new DefaultEnvironment();
    }
    
    public string GetDatabaseConnectionString()
    {
        // Try different targets in order of preference
        var connectionString = _environment.GetEnvironmentVariable("DB_CONNECTION_STRING", EnvironmentVariableTarget.Process)
                            ?? _environment.GetEnvironmentVariable("DB_CONNECTION_STRING", EnvironmentVariableTarget.User)
                            ?? _environment.GetEnvironmentVariable("DB_CONNECTION_STRING", EnvironmentVariableTarget.Machine);
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string not found in environment variables");
        }
        
        return connectionString;
    }
    
    public void SetApplicationConfiguration(string environment, string logLevel)
    {
        _environment.SetEnvironmentVariable("APP_ENVIRONMENT", environment, EnvironmentVariableTarget.Process);
        _environment.SetEnvironmentVariable("LOG_LEVEL", logLevel, EnvironmentVariableTarget.Process);
    }
    
    public void ClearApplicationConfiguration()
    {
        // Setting to null removes the variable
        _environment.SetEnvironmentVariable("APP_ENVIRONMENT", null, EnvironmentVariableTarget.Process);
        _environment.SetEnvironmentVariable("LOG_LEVEL", null, EnvironmentVariableTarget.Process);
    }
}
```

### Testing with MockEnvironment

```csharp
[TestClass]
public class ConfigurationManagerTests
{
    private MockEnvironment _mockEnvironment;
    private ConfigurationManager _configManager;
    
    [TestInitialize]
    public void Setup()
    {
        _mockEnvironment = new MockEnvironment();
        _configManager = new ConfigurationManager(_mockEnvironment);
    }
    
    [TestMethod]
    public void GetDatabaseConnectionString_ReturnsProcessVariable_WhenSet()
    {
        // Arrange
        var expectedConnectionString = "Server=localhost;Database=TestDB;";
        _mockEnvironment.SetEnvironmentVariable("DB_CONNECTION_STRING", expectedConnectionString, EnvironmentVariableTarget.Process);
        
        // Act
        var result = _configManager.GetDatabaseConnectionString();
        
        // Assert
        Assert.AreEqual(expectedConnectionString, result);
    }
    
    [TestMethod]
    public void GetDatabaseConnectionString_FallsBackToUserVariable_WhenProcessNotSet()
    {
        // Arrange
        var expectedConnectionString = "Server=prod;Database=ProdDB;";
        _mockEnvironment.SetEnvironmentVariable("DB_CONNECTION_STRING", expectedConnectionString, EnvironmentVariableTarget.User);
        
        // Act
        var result = _configManager.GetDatabaseConnectionString();
        
        // Assert
        Assert.AreEqual(expectedConnectionString, result);
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetDatabaseConnectionString_ThrowsException_WhenNotFound()
    {
        // Act & Assert
        _configManager.GetDatabaseConnectionString();
    }
    
    [TestMethod]
    public void SetApplicationConfiguration_SetsVariables()
    {
        // Act
        _configManager.SetApplicationConfiguration("production", "info");
        
        // Assert
        Assert.AreEqual("production", _mockEnvironment.GetEnvironmentVariable("APP_ENVIRONMENT", EnvironmentVariableTarget.Process));
        Assert.AreEqual("info", _mockEnvironment.GetEnvironmentVariable("LOG_LEVEL", EnvironmentVariableTarget.Process));
    }
}
```

### Environment-Aware Application Runner

```csharp
using DevOptimal.SystemUtilities.Environment.Abstractions;
using DevOptimal.SystemUtilities.Environment.StateManagement;

public class ApplicationRunner
{
    private readonly IEnvironment _environment;
    private readonly EnvironmentSnapshotter _snapshotter;
    
    public ApplicationRunner(IEnvironment environment)
    {
        _environment = environment ?? new DefaultEnvironment();
        _snapshotter = new EnvironmentSnapshotter(_environment);
    }
    
    public async Task RunWithEnvironmentAsync(Dictionary<string, string> environmentVariables, Func<Task> applicationLogic)
    {
        var snapshots = new List<ISnapshot>();
        
        try
        {
            // Snapshot all variables that will be modified
            foreach (var kvp in environmentVariables)
            {
                var snapshot = _snapshotter.SnapshotEnvironmentVariable(kvp.Key, EnvironmentVariableTarget.Process);
                snapshots.Add(snapshot);
            }
            
            // Set the new environment variables
            foreach (var kvp in environmentVariables)
            {
                _environment.SetEnvironmentVariable(kvp.Key, kvp.Value, EnvironmentVariableTarget.Process);
            }
            
            // Run the application logic
            await applicationLogic();
        }
        finally
        {
            // Restore all snapshots
            foreach (var snapshot in snapshots)
            {
                snapshot.Dispose();
            }
        }
    }
}

// Usage example
var runner = new ApplicationRunner(new DefaultEnvironment());

await runner.RunWithEnvironmentAsync(
    new Dictionary<string, string>
    {
        ["NODE_ENV"] = "test",
        ["API_URL"] = "https://test-api.example.com",
        ["DEBUG"] = "true"
    },
    async () =>
    {
        // Your application logic here
        await ProcessDataAsync();
    });
```

### Integration Testing with Environment Snapshots

```csharp
[TestClass]
public class IntegrationTests
{
    private EnvironmentSnapshotter _snapshotter;
    
    [TestInitialize]
    public void Setup()
    {
        _snapshotter = new EnvironmentSnapshotter();
        // Restore any abandoned snapshots from previous test runs
        _snapshotter.RestoreAbandonedSnapshots();
    }
    
    [TestMethod]
    public void TestWithSpecificEnvironment()
    {
        // Snapshot the current environment
        using var snapshot = _snapshotter.SnapshotEnvironmentVariable("TEST_MODE", EnvironmentVariableTarget.Process);
        
        // Set up test environment
        Environment.SetEnvironmentVariable("TEST_MODE", "integration", EnvironmentVariableTarget.Process);
        
        // Run your integration test
        var result = RunIntegrationTest();
        
        // Assert results
        Assert.IsTrue(result.Success);
        
        // Environment is automatically restored when snapshot is disposed
    }
}
```

## Installation

Install the Environment library via NuGet:

```bash
dotnet add package DevOptimal.SystemUtilities.Environment
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package DevOptimal.SystemUtilities.Environment
```

## Best Practices

1. **Use Dependency Injection**: Inject `IEnvironment` into your classes rather than using `System.Environment` directly
2. **Mock for Testing**: Use `MockEnvironment` for unit tests to avoid affecting the real environment
3. **Handle Missing Variables**: Always check for null when getting environment variables
4. **Use State Management**: For operations that modify environment variables temporarily, use `EnvironmentSnapshotter`
5. **Target Appropriately**: Choose the right `EnvironmentVariableTarget` for your use case
6. **Clean Up**: Always dispose snapshots properly to restore original values

## Security Considerations

- **Machine-Level Variables**: Setting machine-level environment variables requires administrative privileges
- **Sensitive Data**: Be cautious about storing sensitive information in environment variables, especially at User or Machine level
- **Testing Isolation**: Always use `MockEnvironment` in tests to avoid affecting the development environment

## Platform Considerations

- **Windows**: Supports all three targets (Process, User, Machine)
- **macOS/Linux**: Machine-level variables may behave differently depending on the system configuration
- **Persistence**: User and Machine variables persist across process restarts, Process variables do not

## Related Documentation

- [Abstractions Overview](abstractions.md) - Core abstraction concepts
- [State Management](state-management.md) - Snapshot and restore functionality
- [API Reference](api-reference.md) - Complete API documentation
- [Getting Started](getting-started.md) - Quick start guide