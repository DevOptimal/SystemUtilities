# System Utilities

This repo consists of tools that are helpful when interacting with the host system.

## Abstractions

This repo provides a set of interfaces that can be used to add a layer of abstraction between your code and the host system's [environment](../src/Environment/IEnvironment.cs), [file system](../src/FileSystem/IFileSystem.cs), and/or [registry](../src/Registry/IRegistry.cs) (Windows OSes only). This facilitates inversion of control patterns such as [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection), making it easy to swap out the implementations of common system operations.

This is particularly useful in testing, because it greatly simplifies the code required to mock system resources without having to rely on complex fakes frameworks. This repo provides mock implementations for the system's [environment](../src/Environment/MockEnvironment.cs), [file system](../src/FileSystem/MockFileSystem.cs), and [registry](../src/Registry/MockRegistry.cs) (Windows OSes only).

### Usage

To leverage these abstractions in your code, simply replace any calls to functions that operate on a system resource with the correponding method on the interface representing that resource.

For example, suppose you have an app that needs to set an environment variable named "Foo". Normally, you might use .NET's [`System.Environment.SetEnvironmentVariable`](https://docs.microsoft.com/en-us/dotnet/api/system.environment.setenvironmentvariable) API:
```csharp
using System;

namespace MyCompany
{
    public class MyApp
    {
        public MyApp()
        {
        }

        public void InitFoo()
        {
            Environment.SetEnvironmentVariable("Foo", "Bar");
        }
    }
}
```
The problem with this approach is that it is difficult to mock the `SetEnvironmentVariable`. Unit testing this code would generally require the use of a heavy-weight framework, such as [Microsoft Fakes](https://docs.microsoft.com/en-us/visualstudio/test/isolating-code-under-test-with-microsoft-fakes).

A better approach would be to reference the `SetEnvironmentVariable` method on the `IEnvironment` interface provided by this repository:
```csharp
using DevOptimal.SystemUtilities.Environment;

namespace MyCompany
{
    public class MyApp
    {
        private readonly IEnvironment environment;

        public MyApp()
            : this(new DefaultEnvironment())
        {
        }

        public MyApp(IEnvironment environment)
        {
            this.environment = environment;
        }

        public void InitFoo()
        {
           environment.SetEnvironmentVariable("Foo", "Bar");
        }
    }
}
```
Which allows you to pass in any implementation of the `IEnvironment` interface.

Say that you would like to write a unit test that exercises the `InitFoo` method of your `MyApp` type. Your test would then look something like this:
```csharp
using DevOptimal.SystemUtilities.Environment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCompany;

namespace MyCompany.UnitTests
{
    [TestClass]
    public class MyAppTests
    {
        private readonly IEnvironment environment;

        public MyAppTests()
        {
            environment = new MockEnvironment();
        }

        [TestMethod]
        public void InitFoo_SetsEnvironmentVariable()
        {
            var app = new MyApp(environment);
            app.InitFoo();

            Assert.AreEqual("Bar", environment.GetEnvironmentVariable("Foo"));
        }
    }
}
```
