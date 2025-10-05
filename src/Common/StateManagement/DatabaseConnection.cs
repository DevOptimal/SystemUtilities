using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    internal class DatabaseConnection : IDisposable
    {
        private Mutex mutex;
        private readonly CaretakerSerializer serializer;

        private JsonReader reader;
        private IEnumerable<ICaretaker> transaction;
        private bool disposedValue;

        private readonly FileInfo databaseFile;
        private readonly FileInfo transactionFile;

        public DatabaseConnection(string name, CaretakerSerializer serializer, DirectoryInfo persistenceDirectory)
        {
            if (persistenceDirectory == null)
            {
                throw new ArgumentNullException(nameof(persistenceDirectory));
            }

            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            databaseFile = new(Path.Combine(persistenceDirectory.FullName, $"{name}.json"));
            transactionFile = new(Path.Combine(persistenceDirectory.FullName, $"{name}.Transaction.json"));

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

        public void BeginTransaction() => BeginTransaction(TimeSpan.FromSeconds(30));

        public void BeginTransaction(TimeSpan timeout)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(DatabaseConnection));
            }
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
                if (transactionFile.Exists)
                {
                    // Try to recover from failed commit transaction
                    File.Move(transactionFile.FullName, databaseFile.FullName);
                }
                else
                {
                    File.WriteAllText(databaseFile.FullName, "[]");
                }
            }

            var stream = File.Open(databaseFile.FullName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
            reader = new JsonReader(stream);
            transaction = serializer.ReadCaretakers(reader, this);
        }

        public void UpdateCaretakers(Func<IEnumerable<ICaretaker>, IEnumerable<ICaretaker>> updateFunction)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(DatabaseConnection));
            }
            transaction = updateFunction(transaction);
        }

        public void RollbackTransaction()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(DatabaseConnection));
            }
            reader.Dispose();
            reader = null;
            transaction = null;
            mutex.ReleaseMutex();
        }

        public void CommitTransaction()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(DatabaseConnection));
            }
            using (reader)
            using (var stream = File.Open(transactionFile.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new JsonWriter(stream))
            {
                serializer.WriteCaretakers(writer, transaction);
            }
            reader = null;
            transaction = null;
            databaseFile.Delete();
            File.Move(transactionFile.FullName, databaseFile.FullName);
            mutex.ReleaseMutex();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
                transaction = null;
                mutex.Dispose();
                mutex = null;

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
    }
}
