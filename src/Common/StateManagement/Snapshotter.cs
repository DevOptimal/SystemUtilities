using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    /// <summary>
    /// Abstract base class for managing the lifecycle of resource snapshots using caretakers.
    /// Handles transactional restoration of resources and cleanup of abandoned snapshots.
    /// </summary>
    public abstract class Snapshotter : IDisposable
    {
        /// <summary>
        /// The default directory for persisting caretaker data.
        /// </summary>
        protected static readonly DirectoryInfo defaultPersistenceDirectory = new DirectoryInfo(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                nameof(DevOptimal),
                nameof(SystemUtilities),
                nameof(StateManagement)));

        /// <summary>
        /// Unique identifier for this snapshotter instance.
        /// </summary>
        internal string ID { get; }

        /// <summary>
        /// The database connection used for caretaker persistence and transactions.
        /// </summary>
        internal DatabaseConnection Connection { get; }

        /// <summary>
        /// Indicates whether the object has been disposed.
        /// </summary>
        protected bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Snapshotter"/> class.
        /// </summary>
        /// <param name="serializer">The caretaker serializer to use for persistence.</param>
        /// <param name="persistenceDirectory">The directory for storing caretaker data.</param>
        internal Snapshotter(CaretakerSerializer serializer, DirectoryInfo persistenceDirectory)
        {
            ID = Guid.NewGuid().ToString();
            Connection = new DatabaseConnection(GetType().Name, serializer, persistenceDirectory);
        }

        /// <summary>
        /// Disposes the snapshotter, restoring all caretakers associated with this instance and releasing resources.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called from finalizer.</param>
        public void Dispose()
        {
            if (!disposedValue)
            {
                Connection.BeginTransaction();
                try
                {
                    Connection.UpdateCaretakers(RestoreCaretakers);
                    Connection.CommitTransaction();
                }
                catch
                {
                    Connection.RollbackTransaction();
                    throw;
                }

                Connection.Dispose();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Restores all caretakers associated with this snapshotter instance.
        /// Any exceptions during restoration are collected and aggregated.
        /// </summary>
        /// <param name="caretakers">The caretakers to process.</param>
        /// <returns>The caretakers not restored by this snapshotter.</returns>
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
                throw new AggregateException($"Unable to restore {exceptions.Count} snapshot(s).", exceptions);
            }
        }

        /// <summary>
        /// Restores caretakers whose owning process is no longer running (abandoned snapshots).
        /// </summary>
        /// <param name="caretakers">The caretakers to process.</param>
        /// <returns>The caretakers not restored (still owned by running processes).</returns>
        private IEnumerable<ICaretaker> RestoreAbandonedCaretakers(IEnumerable<ICaretaker> caretakers)
        {
            // Map process IDs to process start times; null means no permission to access process info.
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
                catch (InvalidOperationException) { } // The process has already exited.
            }

            var exceptions = new List<Exception>();
            foreach (var caretaker in caretakers)
            {
                // Restore if the process is not running or inaccessible, otherwise yield.
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
                throw new AggregateException($"Unable to restore {exceptions.Count} abandoned snapshot(s).", exceptions);
            }
        }

        /// <summary>
        /// Restores all abandoned snapshots (resources locked by processes that are no longer running).
        /// </summary>
        public void RestoreAbandonedSnapshots()
        {
            Connection.BeginTransaction();
            try
            {
                Connection.UpdateCaretakers(RestoreAbandonedCaretakers);
                Connection.CommitTransaction();
            }
            catch
            {
                Connection.RollbackTransaction();
                throw;
            }
        }
    }
}
