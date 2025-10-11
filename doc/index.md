# DevOptimal.SystemUtilities Documentation

Welcome to the comprehensive documentation for DevOptimal.SystemUtilities! This collection of .NET libraries provides powerful abstractions for common system resources like environment variables, file systems, and the Windows Registry.

## 🚀 Quick Start

New to SystemUtilities? Start here:

- **[Getting Started Guide](getting-started.md)** - Installation, basic usage, and examples
- **[API Reference](api-reference.md)** - Complete API documentation

## 📚 Library Documentation

### Core Libraries

- **[Environment Library](environment.md)** - Environment variable abstractions and management
- **[FileSystem Library](filesystem.md)** - File system abstractions, extensions, and utilities  
- **[Registry Library](registry.md)** - Windows Registry abstractions and operations

### Core Concepts

- **[Abstractions Overview](abstractions.md)** - Understanding the abstraction pattern and dependency injection
- **[State Management](state-management.md)** - Snapshot and restore system state

## 🎯 What's Inside

### Environment Library (`DevOptimal.SystemUtilities.Environment`)
- `IEnvironment` interface for environment variable operations
- `DefaultEnvironment` and `MockEnvironment` implementations
- Environment variable snapshotting and restoration
- Support for Process, User, and Machine-level variables

### FileSystem Library (`DevOptimal.SystemUtilities.FileSystem`)
- `IFileSystem` interface for file and directory operations
- `DefaultFileSystem` and `MockFileSystem` implementations
- Rich extension methods for `DirectoryInfo`, `FileInfo`, and `DriveInfo`
- Platform-aware file system comparers
- File system snapshotting and restoration
- Temporary file management

### Registry Library (`DevOptimal.SystemUtilities.Registry`)
- `IRegistry` interface for Windows Registry operations
- `DefaultRegistry` and `MockRegistry` implementations
- Registry path utilities and normalization
- Registry key and value snapshotting
- Support for all registry hives and views

## 🛠️ Key Features

- ✅ **Testable by Design** - Mock implementations for all system resources
- ✅ **Dependency Injection Ready** - Perfect for modern .NET applications
- ✅ **Cross-Platform** - Works on Windows, macOS, and Linux (Registry is Windows-only)
- ✅ **State Management** - Snapshot and restore system state
- ✅ **Rich Extensions** - Enhanced functionality for standard .NET types
- ✅ **Type Safe** - Strongly-typed interfaces and implementations

## 📦 Installation

Install via NuGet Package Manager:

```bash
# Install individual libraries
dotnet add package DevOptimal.SystemUtilities.Environment
dotnet add package DevOptimal.SystemUtilities.FileSystem
dotnet add package DevOptimal.SystemUtilities.Registry

# Or use Package Manager Console
Install-Package DevOptimal.SystemUtilities.Environment
Install-Package DevOptimal.SystemUtilities.FileSystem
Install-Package DevOptimal.SystemUtilities.Registry
```

## 🎨 Quick Example

```csharp
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.Environment.Abstractions;

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
        // Testable file operations
        if (!_fileSystem.FileExists(inputPath))
            throw new FileNotFoundException($"File not found: {inputPath}");
            
        var content = _fileSystem.ReadAllText(inputPath);
        
        // Testable environment access
        var outputDir = _environment.GetEnvironmentVariable("OUTPUT_DIR", EnvironmentVariableTarget.Process)
                       ?? @"C:\temp";
                       
        var outputPath = Path.Combine(outputDir, "processed_" + Path.GetFileName(inputPath));
        _fileSystem.WriteAllText(outputPath, ProcessContent(content));
    }
}
```

## 🧪 Testing Made Easy

```csharp
[TestMethod]
public void ProcessDocument_CreatesProcessedFile()
{
    // Arrange - No real file system needed!
    var mockFileSystem = new MockFileSystem();
    var mockEnvironment = new MockEnvironment();
    var processor = new DocumentProcessor(mockFileSystem, mockEnvironment);
    
    mockEnvironment.SetEnvironmentVariable("OUTPUT_DIR", @"C:\output", EnvironmentVariableTarget.Process);
    mockFileSystem.CreateDirectory(@"C:\input");
    mockFileSystem.WriteAllText(@"C:\input\test.txt", "original content");
    
    // Act
    processor.ProcessDocument(@"C:\input\test.txt");
    
    // Assert
    Assert.IsTrue(mockFileSystem.FileExists(@"C:\output\processed_test.txt"));
}
```

## 📖 Documentation Structure

This documentation is organized to help you find what you need quickly:

1. **[Getting Started](getting-started.md)** - Begin here for installation and basic concepts
2. **[Library Guides](environment.md)** - Deep dives into each library's features
3. **[Core Concepts](abstractions.md)** - Understanding the architectural patterns
4. **[API Reference](api-reference.md)** - Complete method and class documentation

## 🎯 Target Frameworks

- **.NET Standard 2.0** - Compatible with .NET Framework 4.6.1+ and .NET Core 2.0+
- **Cross-Platform** - Works on Windows, macOS, and Linux
- **Registry Library** - Windows-only (other libraries work on all platforms)

## 🤝 Contributing

We welcome contributions! See our [contributing guidelines](../CONTRIBUTING.md) for details on:

- Setting up the development environment
- Building and testing the libraries
- Submitting pull requests
- Coding standards and best practices