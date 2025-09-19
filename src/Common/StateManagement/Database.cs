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
    internal class Database(SnapshotSerializer serializer)
    {
        private Mutex mutex;
        private readonly SnapshotSerializer serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        private JsonReader reader;
        private IEnumerable<ISnapshot> snapshots;

        public DirectoryInfo DatabaseDirectory => new(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                nameof(DevOptimal),
                nameof(SystemUtilities),
                nameof(StateManagement)));

        private FileInfo DatabaseFile => new(Path.Combine(DatabaseDirectory.FullName, $"{serializer.GetType().Name}.json"));

        private FileInfo TransactionFile => new(Path.Combine(DatabaseDirectory.FullName, $"{serializer.GetType().Name}.Transaction.json"));

        public void BeginTransaction(TimeSpan timeout)
        {
            // Get normalized file path
            var normalizedDatabaseID = Path.GetFullPath(DatabaseFile.FullName).Replace('\\', '/');
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                normalizedDatabaseID = normalizedDatabaseID.ToLower();
            }

            // unique id for global mutex - Global prefix means it is global to the machine
            var mutexId = $@"Global\{nameof(DevOptimal)}.{nameof(SystemUtilities)}.{nameof(StateManagement)}:/{normalizedDatabaseID}";

            //lock (mutex)
            //{
                if (mutex != null)
                {
                    throw new InvalidOperationException("Transaction already in progress");
                }
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
            //}

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

            var databaseDirectory = DatabaseFile.Directory;

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

            if (!DatabaseFile.Exists)
            {
                File.WriteAllText(DatabaseFile.FullName, "[]");
            }

            var stream = File.Open(DatabaseFile.FullName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
            reader = new JsonReader(stream);
            snapshots = serializer.ReadSnapshots(reader, this);
        }

        public void UpdateSnapshots(Func<IEnumerable<ISnapshot>, IEnumerable<ISnapshot>> snapshotFunction)
        {
            if (mutex == null)
            {
                throw new InvalidOperationException("No transaction in progress");
            }
            snapshots = snapshotFunction(snapshots);
        }

        public void RollbackTransaction()
        {
            if (mutex == null)
            {
                throw new InvalidOperationException("No transaction in progress");
            }
            reader.Dispose();
            snapshots = null;
            lock (mutex)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
                mutex = null;
            }
        }

        public void CommitTransaction()
        {
            if (mutex == null)
            {
                throw new InvalidOperationException("No transaction in progress");
            }
            using (var stream = File.Open(TransactionFile.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new JsonWriter(stream))
            using (reader)
            {
                serializer.WriteSnapshots(writer, snapshots);
            }
            DatabaseFile.Delete();
            TransactionFile.MoveTo(DatabaseFile.FullName);
            lock (mutex)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
                mutex = null;
            }
        }
    }
}
