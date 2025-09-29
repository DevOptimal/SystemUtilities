using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    // TODO: Can we combine the database and snapshotter into one class?
    public abstract class Snapshotter : IDisposable
    {
        protected static readonly DirectoryInfo defaultPersistenceDirectory = new(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                nameof(DevOptimal),
                nameof(SystemUtilities),
                nameof(StateManagement)));

        protected string ID { get; set; }

        protected bool disposedValue;

        internal readonly DatabaseConnection connection;

        internal Snapshotter(CaretakerSerializer serializer, DirectoryInfo persistenceDirectory)
        {
            ID = Guid.NewGuid().ToString();
            connection = new DatabaseConnection(serializer, persistenceDirectory);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    connection.BeginTransaction();
                    try
                    {
                        connection.UpdateCaretakers(RestoreCaretakers);
                        connection.CommitTransaction();
                    }
                    catch
                    {
                        connection.RollbackTransaction();
                        throw;
                    }

                    connection.Dispose();
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
            connection.BeginTransaction();
            try
            {
                connection.UpdateCaretakers(RestoreAbandonedCaretakers);
                connection.CommitTransaction();
            }
            catch
            {
                connection.RollbackTransaction();
                throw;
            }
        }
    }
}
