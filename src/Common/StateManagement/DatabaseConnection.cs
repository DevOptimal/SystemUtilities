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
    /// <summary>
    /// Manages transactional access to a JSON-based caretaker database with cross-process locking.
    /// Handles reading, writing, and updating caretaker state with file-based persistence and a global mutex.
    /// </summary>
    internal class DatabaseConnection : IDisposable
    {
        private Mutex mutex;
        private readonly CaretakerSerializer serializer;

        private JsonReader reader;
        private IEnumerable<ICaretaker> transaction;
        private bool disposedValue;

        private readonly FileInfo databaseFile;
        private readonly FileInfo transactionFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnection"/> class.
        /// Sets up file paths and a global mutex for cross-process synchronization.
        /// </summary>
        /// <param name="name">The logical name for the database (used in file naming).</param>
        /// <param name="serializer">The caretaker serializer to use for reading/writing.</param>
        /// <param name="persistenceDirectory">The directory for database files.</param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is null.</exception>
        /// <exception cref="PlatformNotSupportedException">Thrown if the OS is not supported.</exception>
        public DatabaseConnection(string name, CaretakerSerializer serializer, DirectoryInfo persistenceDirectory)
        {
            if (persistenceDirectory == null)
            {
                throw new ArgumentNullException(nameof(persistenceDirectory));
            }

            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            databaseFile = new(Path.Combine(persistenceDirectory.FullName, $"{name}.json"));
            transactionFile = new(Path.Combine(persistenceDirectory.FullName, $"{name}.Transaction.json"));

            // Get normalized file path for mutex ID
            var normalizedDatabaseID = Path.GetFullPath(databaseFile.FullName).Replace('\\', '/');
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                normalizedDatabaseID = normalizedDatabaseID.ToLower();
            }

            // Unique id for global mutex - Global prefix means it is global to the machine
            var mutexId = $@"Global\{nameof(DevOptimal)}.{nameof(SystemUtilities)}.{nameof(StateManagement)}:/{normalizedDatabaseID}";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Set up security for multi-user usage (allow "Everyone" full control)
                var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, domainSid: null), MutexRights.FullControl, AccessControlType.Allow);
                var securitySettings = new MutexSecurity();
                securitySettings.AddAccessRule(allowEveryoneRule);

                // Create the mutex with security settings
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

        /// <summary>
        /// Begins a transaction with a default timeout of 30 seconds.
        /// </summary>
        public void BeginTransaction() => BeginTransaction(TimeSpan.FromSeconds(30));

        /// <summary>
        /// Begins a transaction, acquiring the mutex and preparing the caretaker data for update.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for the mutex.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the connection is disposed.</exception>
        /// <exception cref="TimeoutException">Thrown if the mutex cannot be acquired in time.</exception>
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
                // The mutex was abandoned in another process, but is now acquired
            }

            var databaseDirectory = databaseFile.Directory;

            // Ensure the database directory exists and set permissions
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

            // Ensure the database file exists, recover from failed commit if needed
            if (!databaseFile.Exists)
            {
                if (transactionFile.Exists)
                {
                    // Recover from failed commit transaction
                    File.Move(transactionFile.FullName, databaseFile.FullName);
                }
                else
                {
                    File.WriteAllText(databaseFile.FullName, "[]");
                }
            }

            // Open the database file for reading and initialize the transaction
            var stream = File.Open(databaseFile.FullName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
            reader = new JsonReader(stream);
            transaction = serializer.ReadCaretakers(reader, this);
        }

        /// <summary>
        /// Applies an update function to the current transaction's caretakers.
        /// </summary>
        /// <param name="updateFunction">A function that takes and returns a collection of caretakers.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the connection is disposed.</exception>
        public void UpdateCaretakers(Func<IEnumerable<ICaretaker>, IEnumerable<ICaretaker>> updateFunction)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(DatabaseConnection));
            }
            transaction = updateFunction(transaction);
        }

        /// <summary>
        /// Rolls back the current transaction, discarding changes and releasing the mutex.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the connection is disposed.</exception>
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

        /// <summary>
        /// Commits the current transaction, writing changes to disk and releasing the mutex.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the connection is disposed.</exception>
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

        /// <summary>
        /// Releases resources used by the <see cref="DatabaseConnection"/>.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called from finalizer.</param>
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

        /// <summary>
        /// Disposes the <see cref="DatabaseConnection"/> and suppresses finalization.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
