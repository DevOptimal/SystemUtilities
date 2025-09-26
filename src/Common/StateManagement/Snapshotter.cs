using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    // TODO: Can we combine the database and snapshotter into one class?
    public abstract class Snapshotter : IDisposable
    {
        internal string ID { get; set; }

        protected static DirectoryInfo defaultPersistenceDirectory = new(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                nameof(DevOptimal),
                nameof(SystemUtilities),
                nameof(StateManagement)));

        private Mutex mutex;
        private readonly CaretakerSerializer serializer;

        private JsonReader reader;
        private IEnumerable<ICaretaker> transaction;
        private bool disposedValue;

        private readonly FileInfo databaseFile;
        private readonly FileInfo transactionFile;

        internal Snapshotter(CaretakerSerializer serializer, DirectoryInfo persistenceDirectory)
        {
            if (persistenceDirectory == null)
            {
                throw new ArgumentNullException(nameof(persistenceDirectory));
            }

            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            databaseFile = new(Path.Combine(persistenceDirectory.FullName, $"{GetType().Name}.json"));
            transactionFile = new(Path.Combine(persistenceDirectory.FullName, $"{GetType().Name}.Transaction.json"));
            ID = Guid.NewGuid().ToString();

            // Get normalized file path
            var normalizedDatabaseID = Path.GetFullPath(databaseFile.FullName).Replace('\\', '/');
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                normalizedDatabaseID = normalizedDatabaseID.ToLower();
            }

            // unique id for global mutex - Global prefix means it is global to the machine
            var mutexId = $@"Global\{nameof(DevOptimal)}.{nameof(SystemUtilities)}.{nameof(StateManagement)}:/{normalizedDatabaseID}";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
                // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
                var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, domainSid: null), MutexRights.FullControl, AccessControlType.Allow);
                var securitySettings = new MutexSecurity();
                securitySettings.AddAccessRule(allowEveryoneRule);

                // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
#if NETSTANDARD2_0
                mutex = MutexAcl.Create(false, mutexId, out var createdNew, securitySettings);
#else
                mutex = new Mutex(false, mutexId, out var createdNew, securitySettings);
#endif
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                mutex = new Mutex(false, mutexId, out var createdNew);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        internal void BeginTransaction(TimeSpan timeout)
        {
            try
            {
                if (!mutex.WaitOne(timeout, exitContext: false))
                {
                    throw new TimeoutException("Timeout waiting for exclusive access");
                }
            }
            catch (AbandonedMutexException)
            {
                // Log the fact that the mutex was abandoned in another process,
                // it will still get acquired
            }

            var databaseDirectory = databaseFile.Directory;

            if (!databaseDirectory.Exists)
            {
                databaseDirectory.Create();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var directorySecurity = databaseDirectory.GetAccessControl();
                    directorySecurity.AddAccessRule(new FileSystemAccessRule(
                        identity: new SecurityIdentifier(WellKnownSidType.WorldSid, domainSid: null),
                        fileSystemRights: FileSystemRights.FullControl,
                        inheritanceFlags: InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        propagationFlags: PropagationFlags.NoPropagateInherit,
                        type: AccessControlType.Allow));
                    databaseDirectory.SetAccessControl(directorySecurity);
                }
            }

            if (!databaseFile.Exists)
            {
                File.WriteAllText(databaseFile.FullName, "[]");
            }

            var stream = File.Open(databaseFile.FullName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
            reader = new JsonReader(stream);
            transaction = serializer.ReadSnapshots(reader, this);
        }

        internal void UpdateCaretakers(Func<IEnumerable<ICaretaker>, IEnumerable<ICaretaker>> updateFunction)
        {
            if (mutex == null)
            {
                throw new InvalidOperationException("No transaction in progress");
            }
            transaction = updateFunction(transaction);
        }

        internal void RollbackTransaction()
        {
            if (mutex == null)
            {
                throw new InvalidOperationException("No transaction in progress");
            }
            reader.Dispose();
            reader = null;
            transaction = null;
            lock (mutex)
            {
                mutex.ReleaseMutex();
            }
        }

        internal void CommitTransaction()
        {
            if (mutex == null)
            {
                throw new InvalidOperationException("No transaction in progress");
            }
            using (var stream = File.Open(transactionFile.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new JsonWriter(stream))
            using (reader)
            {
                serializer.WriteSnapshots(writer, transaction);
            }
            reader = null;
            transaction = null;
            databaseFile.Delete();
            File.Move(transactionFile.FullName, databaseFile.FullName);
            lock (mutex)
            {
                mutex.ReleaseMutex();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BeginTransaction(TimeSpan.FromSeconds(30));
                    try
                    {
                        UpdateCaretakers(RestoreCaretakers);
                        CommitTransaction();
                    }
                    catch
                    {
                        RollbackTransaction();
                        throw;
                    }

                    mutex.Dispose();
                    mutex = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Snapshotter()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private IEnumerable<ICaretaker> RestoreCaretakers(IEnumerable<ICaretaker> caretakers)
        {
            var exceptions = new List<Exception>();
            foreach (var caretaker in caretakers)
            {
                if (caretaker.ParentID == ID)
                {
                    try
                    {
                        caretaker.Restore();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                else
                {
                    yield return caretaker;
                }
            }
            if (exceptions.Any())
            {
                new AggregateException($"Unable to restore {exceptions.Count} snapshot(s).", exceptions);
            }
        }

        private IEnumerable<ICaretaker> RestoreAbandonedCaretakers(IEnumerable<ICaretaker> caretakers)
        {
            // Create a dictionary that maps process IDs to process start times, which will be used to uniquely identify a currently running process.
            // A null value indicates that the current process does not have permission to the corresponding process - try rerunning in an elevated process.
            var processes = new Dictionary<int, DateTime?>();
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    processes[process.Id] = process.StartTime;
                }
                catch (Win32Exception)
                {
                    processes[process.Id] = null;
                }
                catch (InvalidOperationException) { } // The process has already exited, so don't add it.
            }

            var exceptions = new List<Exception>();
            foreach (var caretaker in caretakers)
            {
                if (!(processes.ContainsKey(caretaker.ProcessID) && (processes[caretaker.ProcessID] == caretaker.ProcessStartTime || processes[caretaker.ProcessID] == null)))
                {
                    try
                    {
                        caretaker.Restore();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                else
                {
                    yield return caretaker;
                }
            }
            if (exceptions.Any())
            {
                new AggregateException($"Unable to restore {exceptions.Count} abandoned snapshot(s).", exceptions);
            }
        }

        public void RestoreAbandonedSnapshots()
        {
            BeginTransaction(TimeSpan.FromSeconds(30));
            try
            {
                UpdateCaretakers(RestoreAbandonedCaretakers);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
        }
    }
}
