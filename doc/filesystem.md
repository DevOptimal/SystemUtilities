# FileSystem Library

The FileSystem library provides a comprehensive abstraction layer for file system operations, making it easy to write testable code that interacts with files and directories. It includes interfaces, implementations, extension methods, comparers, and state management features.

## Overview

The FileSystem library consists of several key components:

- **Abstractions**: Core interfaces and implementations for file system operations
- **Extensions**: Extension methods for enhanced functionality with standard .NET types
- **Comparers**: Equality comparers for file system objects
- **State Management**: Snapshot and restore capabilities for file system resources
- **Utilities**: Additional helper classes and methods

## Key Features

- ✅ **Testable Design**: Mock implementations for unit testing
- ✅ **Cross-Platform**: Works on Windows, macOS, and Linux
- ✅ **Rich Extensions**: Enhanced functionality for `DirectoryInfo`, `FileInfo`, and `DriveInfo`
- ✅ **State Management**: Snapshot and restore file system state
- ✅ **Type Safety**: Strongly-typed interfaces and implementations
- ✅ **Performance**: Efficient implementations with caching support

## Core Abstractions

### IFileSystem Interface

The `IFileSystem` interface provides a complete abstraction over file system operations:

```csharp
public interface IFileSystem
{
    // File operations
    void CopyFile(string sourcePath, string destinationPath, bool overwrite);
    void CreateFile(string path);
    void DeleteFile(string path);
    void MoveFile(string sourcePath, string destinationPath, bool overwrite);
    void HardLinkFile(string sourcePath, string destinationPath, bool overwrite);
    bool FileExists(string path);
    FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);
    
    // Directory operations
    void CreateDirectory(string path);
    void DeleteDirectory(string path, bool recursive);
    bool DirectoryExists(string path);
    string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);
    string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
}
```

### DefaultFileSystem

The `DefaultFileSystem` class provides the standard implementation that uses the built-in .NET file system APIs:

```csharp
using DevOptimal.SystemUtilities.FileSystem.Abstractions;

// Create a default file system instance
IFileSystem fileSystem = new DefaultFileSystem();

// Use it for file operations
fileSystem.CreateFile(@"C:\temp\myfile.txt");
fileSystem.CopyFile(@"C:\temp\myfile.txt", @"C:\temp\backup.txt", overwrite: true);
```

### MockFileSystem

The `MockFileSystem` class provides an in-memory implementation perfect for unit testing:

```csharp
using DevOptimal.SystemUtilities.FileSystem.Abstractions;

// Create a mock file system for testing
IFileSystem mockFileSystem = new MockFileSystem();

// Use it in tests without touching the real file system
mockFileSystem.CreateFile(@"C:\test\file.txt");
Assert.IsTrue(mockFileSystem.FileExists(@"C:\test\file.txt"));
```

## Extension Methods

The library provides rich extension methods for standard .NET file system types.

### DirectoryInfo Extensions

```csharp
using DevOptimal.SystemUtilities.FileSystem.Extensions;

var directory = new DirectoryInfo(@"C:\MyProject");

// Get subdirectories and files with enhanced syntax
var subDir = directory.GetDirectory("src", "models");  // C:\MyProject\src\models
var file = directory.GetFile("readme.txt");           // C:\MyProject\readme.txt

// Check existence using abstractions
bool exists = directory.Exists(fileSystem);

// Get the drive
DriveInfo drive = directory.Drive;

// Check relationships between directories
bool isAncestor = directory.IsAncestorOf(subDir);
bool isDescendant = subDir.IsDescendantOf(directory);
```

### FileInfo Extensions

```csharp
using DevOptimal.SystemUtilities.FileSystem.Extensions;

var file = new FileInfo(@"C:\data\document.txt");

// Enhanced file operations
file.CopyTo(@"C:\backup\document.txt", fileSystem, overwrite: true);
file.MoveTo(@"C:\archive\document.txt", fileSystem, overwrite: false);

// Create hard links
file.HardLinkTo(@"C:\links\document.txt", fileSystem);

// Read and write with encoding support
string content = file.ReadAllText(Encoding.UTF8, fileSystem);
file.WriteAllText("New content", Encoding.UTF8, fileSystem);

// Check existence and equality
bool exists = file.Exists(fileSystem);
bool isEqual = file.Equals(otherFile, FileInfoComparer.Default);
```

### DriveInfo Extensions

```csharp
using DevOptimal.SystemUtilities.FileSystem.Extensions;

var drive = new DriveInfo("C:");

// Get directories and files on the drive
var directories = drive.GetDirectories(fileSystem);
var files = drive.GetFiles(fileSystem, "*.txt", recursive: true);

// Compare drives
bool isEqual = drive.Equals(otherDrive, DriveInfoComparer.Default);
```

### String Extensions

```csharp
using DevOptimal.SystemUtilities.FileSystem.Extensions;

string path = @"C:\Users\John\Documents";

// Convert to file system objects
DirectoryInfo directory = path.AsDirectoryInfo();
FileInfo file = path.AsFileInfo();
DriveInfo drive = path.AsDriveInfo();  // Extracts drive from path
```

## Comparers

The library includes equality comparers for file system objects that handle platform-specific differences:

### FileSystemInfoComparer

Base comparer for file system objects with platform-aware comparison:

```csharp
using DevOptimal.SystemUtilities.FileSystem.Comparers;

var comparer = FileSystemInfoComparer.Default;

// On Windows: case-insensitive comparison
// On Unix: case-sensitive comparison
bool areEqual = comparer.Equals(fileInfo1, fileInfo2);
```

### Specific Comparers

```csharp
// File-specific comparer
var fileComparer = FileInfoComparer.Default;
bool filesEqual = fileComparer.Equals(file1, file2);

// Directory-specific comparer  
var dirComparer = DirectoryInfoComparer.Default;
bool dirsEqual = dirComparer.Equals(dir1, dir2);

// Drive-specific comparer
var driveComparer = DriveInfoComparer.Default;
bool drivesEqual = driveComparer.Equals(drive1, drive2);

// Windows-specific comparer (always case-insensitive)
var windowsComparer = WindowsFileSystemInfoComparer.Default;
```

## State Management

The FileSystem library includes powerful state management capabilities through the `FileSystemSnapshotter` class.

### Creating a Snapshotter

```csharp
using DevOptimal.SystemUtilities.FileSystem.StateManagement;

// Create a snapshotter with default settings
var snapshotter = new FileSystemSnapshotter();

// Or with a custom file system
var snapshotter = new FileSystemSnapshotter(mockFileSystem);

// Or with a custom persistence directory
var snapshotter = new FileSystemSnapshotter(fileSystem, persistenceDirectory);
```

### Snapshotting Files

```csharp
// Create a snapshot of a file
using var snapshot = snapshotter.SnapshotFile(@"C:\important\config.xml");

// Modify the file
File.WriteAllText(@"C:\important\config.xml", "new content");

// The original content is automatically restored when disposed
// (or call snapshot.Dispose() explicitly)
```

### Snapshotting Directories

```csharp
// Create a snapshot of an entire directory
using var snapshot = snapshotter.SnapshotDirectory(@"C:\project\settings");

// Make changes to files in the directory
File.Delete(@"C:\project\settings\user.config");
File.WriteAllText(@"C:\project\settings\app.config", "modified");

// All changes are reverted when the snapshot is disposed
```

### Handling Abandoned Snapshots

If your application crashes before snapshots are properly disposed, you can restore abandoned snapshots:

```csharp
// At application startup, restore any abandoned snapshots
snapshotter.RestoreAbandonedSnapshots();
```

## Temporary Files

The library provides a convenient way to work with temporary files:

```csharp
using DevOptimal.SystemUtilities.FileSystem;

// Create a temporary file that's automatically cleaned up
using var tempFile = new TemporaryFile();

// Use the temporary file
File.WriteAllText(tempFile.Path, "temporary data");
ProcessFile(tempFile.Path);

// File is automatically deleted when disposed
```

## Usage Examples

### Basic File Operations

```csharp
using DevOptimal.SystemUtilities.FileSystem.Abstractions;
using DevOptimal.SystemUtilities.FileSystem.Extensions;

public class DocumentProcessor
{
    private readonly IFileSystem _fileSystem;
    
    public DocumentProcessor(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? new DefaultFileSystem();
    }
    
    public void ProcessDocument(string inputPath, string outputPath)
    {
        // Check if input exists
        if (!_fileSystem.FileExists(inputPath))
            throw new FileNotFoundException($"Input file not found: {inputPath}");
            
        // Ensure output directory exists
        var outputDir = Path.GetDirectoryName(outputPath);
        if (!_fileSystem.DirectoryExists(outputDir))
            _fileSystem.CreateDirectory(outputDir);
            
        // Process the file (implementation details omitted)
        using var inputStream = _fileSystem.OpenFile(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var outputStream = _fileSystem.OpenFile(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
        
        // Copy and transform content
        ProcessStream(inputStream, outputStream);
    }
}
```

### Testing with MockFileSystem

```csharp
[TestMethod]
public void ProcessDocument_CreatesOutputFile()
{
    // Arrange
    var mockFileSystem = new MockFileSystem();
    var processor = new DocumentProcessor(mockFileSystem);
    
    // Create a mock input file
    var inputPath = @"C:\input\document.txt";
    var outputPath = @"C:\output\processed.txt";
    
    mockFileSystem.CreateDirectory(@"C:\input");
    mockFileSystem.CreateFile(inputPath);
    
    // Act
    processor.ProcessDocument(inputPath, outputPath);
    
    // Assert
    Assert.IsTrue(mockFileSystem.FileExists(outputPath));
    Assert.IsTrue(mockFileSystem.DirectoryExists(@"C:\output"));
}
```

### Using Extensions for Enhanced Functionality

```csharp
using DevOptimal.SystemUtilities.FileSystem.Extensions;

public void OrganizeFiles(DirectoryInfo sourceDir, DirectoryInfo targetDir)
{
    var fileSystem = new DefaultFileSystem();
    
    // Get all text files recursively
    var textFiles = sourceDir.GetFiles(fileSystem, "*.txt", recursive: true);
    
    foreach (var file in textFiles)
    {
        // Create year-based subdirectory
        var yearDir = targetDir.GetDirectory(file.CreationTime.Year.ToString());
        
        if (!yearDir.Exists(fileSystem))
            yearDir.Create(fileSystem);
            
        // Move file to organized location
        var targetFile = yearDir.GetFile(file.Name);
        file.MoveTo(targetFile.FullName, fileSystem, overwrite: true);
    }
}
```

## Installation

Install the FileSystem library via NuGet:

```bash
dotnet add package DevOptimal.SystemUtilities.FileSystem
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package DevOptimal.SystemUtilities.FileSystem
```

## Best Practices

1. **Use Dependency Injection**: Inject `IFileSystem` into your classes rather than using static file APIs directly
2. **Mock for Testing**: Use `MockFileSystem` for unit tests to avoid file system dependencies
3. **Handle Exceptions**: File operations can fail - always handle `IOException` and related exceptions
4. **Use State Management**: For operations that modify multiple files, consider using `FileSystemSnapshotter`
5. **Dispose Resources**: Always dispose file streams and snapshots properly
6. **Path Handling**: Use `Path.Combine()` and extension methods for robust path manipulation

## Platform Considerations

- **Windows**: Supports all features including hard links and Windows-specific comparers
- **macOS/Linux**: Full support except for some Windows-specific file attributes
- **Path Separators**: The library handles platform-specific path separators automatically
- **Case Sensitivity**: Comparers automatically adapt to platform conventions

## Related Documentation

- [Abstractions Overview](abstractions.md) - Core abstraction concepts
- [State Management](state-management.md) - Snapshot and restore functionality
- [API Reference](api-reference.md) - Complete API documentation
- [Getting Started](getting-started.md) - Quick start guide