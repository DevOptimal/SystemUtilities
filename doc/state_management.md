# Introduction

These libraries contain tools for safely and effectively managing the state of system resources. These tools can be found in the `StateManagement` namespace of each system resource area (environment, file system, and registry).

## What does it do?

The primary state management mechanism in these libraries is referred to as a __snapshotter__. A snapshotter is like a camera that can be used to take pictures (or __snapshots__) of supported system resources at certain points in time. These snapshots can later be used to restore the resource back to the state it was in when the snapshot was taken.

## Why do I need it?

This is particularly useful in automation that needs to make temporary changes to the state of the system it runs on, but is expected to leave the system in the same state it was in before the automation ran.

## How about an example?

For example, during automated regression testing, sometimes a test needs to configure the product under test by changing system resources (such as an environment variable, configuration file, or registry key). Other times, the product will change system resources itself. In either case, the altered state of those system resources can interfere with subsequent tests, leading to failures that are difficult to debug.

This is where snapshotters come in: Prior to executing the test, snapshot the state of any system resources that the test or product might alter. During execution of the test, those system resources can be altered however needed. At the completion of the test, the snapshots can be used to restore those system resources to the state they were in at the beginning of the test.

# Persistence

It is possible for a snapshot to not be restored before its process terminates. This happens when the `Dispose` method on the snapshot doesn't get called by the time the process exits, such as when a process is killed prematurely.

This can be devastating because system state could be lost. For example, consider an application that uses the `FileSystemSnapshotter` to snapshot a file, then overwrites the file, and finally uses the snapshot to restore the original contents of the file. If the process crashes _after_ the application overwrites the file but _before_ the snapshot can restore its contents, then data loss has occurred.

For this reason, all snapshotters save the state of system resources to disk when a snapshot is created. In the event that your program terminates prematurely, these snapshots become "abandoned" as the objects that they back are no longer stored in memory. The abandoned snapshots can be restored via the `RestoreAbandonedSnapshots` method on a snapshotter.

It is good practice to call `RestoreAbandonedSnapshots` periodically (such as at the beginning of your application) to ensure that state from previous processes gets cleaned up.

# Usage

To begin using snapshots, all you need to do is create a new snapshotter instance:
```csharp
using DevOptimal.SystemUtilities.StateManagement;

var snapshotter = new FileSystemSnapshotter();
```

Once you have a snapshotter instance, you can use it to snapshot system resources:
```csharp
var snapshot = snapshotter.SnapshotFile(@"C:\foo\bar.txt");
```

When you snapshot a system resource, you get back an `ISnapshot` object. This object can be used to restore the state of the system resource later by calling its `Dispose` method:
```csharp
snapshot.Dispose();
```

You can restore any abandoned snapshots left behind by old processes by calling the `RestoreAbandonedSnapshots` method:
```csharp
snapshotter.RestoreAbandonedSnapshots();
```