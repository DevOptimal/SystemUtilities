# DevOptimal.SystemUtilities

[![Continuous Integration](https://github.com/DevOptimal/SystemUtilities/actions/workflows/ci.yml/badge.svg)](https://github.com/DevOptimal/SystemUtilities/actions/workflows/ci.yml)

[![NuGet package](https://img.shields.io/nuget/v/DevOptimal.SystemUtilities.Environment.svg?label=DevOptimal.SystemUtilities.Environment&logo=nuget)](https://nuget.org/packages/DevOptimal.SystemUtilities.Environment)
[![NuGet package](https://img.shields.io/nuget/v/DevOptimal.SystemUtilities.FileSystem.svg?label=DevOptimal.SystemUtilities.FileSystem&logo=nuget)](https://nuget.org/packages/DevOptimal.SystemUtilities.FileSystem)
[![NuGet package](https://img.shields.io/nuget/v/DevOptimal.SystemUtilities.Registry.svg?label=DevOptimal.SystemUtilities.Registry&logo=nuget)](https://nuget.org/packages/DevOptimal.SystemUtilities.Registry)

## Features

- System resource abstractions
    - Easily swap out the underlying implementations of common system resource operations (e.g. environment variables, files, and registry keys)
    - Mock implementations of system resources for easy testing
    - Serialization-friendly
- System resource state management
    - Programatically snapshot and restore the state of the following system resources:
        - Environment variables
        - Directories
        - Files
        - Registry keys
        - Registry values
    - Persisted to disk so that you never lose state, even if the process crashes!

## Documentation

Read the docs [here](https://github.com/DevOptimal/SystemUtilities/blob/main/doc/index.md).

## Target Platforms

- [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0)
