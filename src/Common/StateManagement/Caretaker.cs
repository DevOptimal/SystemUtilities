using DevOptimal.SystemUtilities.Common.StateManagement.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    /// <summary>
    /// Abstract base class for managing the lifecycle and state of an originator object using the Memento pattern.
    /// Handles resource locking, state persistence, and restoration through a database connection.
    /// </summary>
    /// <typeparam name="TOriginator">The type of the originator object.</typeparam>
    /// <typeparam name="TMemento">The type of the memento representing the originator's state.</typeparam>
    internal abstract class Caretaker<TOriginator, TMemento> : ICaretaker, ISnapshot
        where TOriginator : IOriginator<TMemento>
        where TMemento : IMemento
    {
        /// <summary>
        /// Gets or sets the unique identifier for this caretaker.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets the identifier of the parent snapshotter.
        /// </summary>
        public string ParentID { get; }

        /// <summary>
        /// Gets or sets the process ID associated with this caretaker.
        /// </summary>
        public int ProcessID { get; set; }

        /// <summary>
        /// Gets or sets the process start time associated with this caretaker.
        /// </summary>
        public DateTime ProcessStartTime { get; set; }

        /// <summary>
        /// Gets or sets the originator object whose state is managed.
        /// </summary>
        public TOriginator Originator { get; set; }

        /// <summary>
        /// Gets or sets the memento representing the originator's state.
        /// </summary>
        public TMemento Memento { get; set; }

        /// <summary>
        /// Gets or sets the database connection used for caretaker persistence.
        /// </summary>
        public DatabaseConnection Connection { get; set; }

        private bool disposedValue;

        /// <summary>
        /// Initializes a new caretaker, locks the resource, and persists the caretaker in the database.
        /// </summary>
        /// <param name="originator">The originator object to manage.</param>
        /// <param name="snapshotter">The snapshotter managing the caretaker's lifecycle.</param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is null.</exception>
        public Caretaker(TOriginator originator, Snapshotter snapshotter)
        {
            if (snapshotter == null)
            {
                throw new ArgumentNullException(nameof(snapshotter));
            }
            Originator = originator ?? throw new ArgumentNullException(nameof(originator));
            Connection = snapshotter.Connection ?? throw new ArgumentNullException(nameof(snapshotter.Connection));
            ID = originator.ID;
            ParentID = snapshotter.ID;
            var currentProcess = Process.GetCurrentProcess();
            ProcessID = currentProcess.Id;
            ProcessStartTime = currentProcess.StartTime;

            // Begin transaction to add caretaker and lock the resource
            Connection.BeginTransaction();
            try
            {
                Connection.UpdateCaretakers(AddCaretaker);
                Connection.CommitTransaction();
            }
            catch
            {
                Connection.RollbackTransaction();
                throw;
            }
        }

        /// <summary>
        /// Initializes a caretaker for deserialization, restoring all properties from persisted state.
        /// </summary>
        /// <param name="id">The caretaker's unique identifier.</param>
        /// <param name="parentID">The parent snapshotter's identifier.</param>
        /// <param name="processID">The process ID associated with the caretaker.</param>
        /// <param name="processStartTime">The process start time.</param>
        /// <param name="connection">The database connection.</param>
        /// <param name="originator">The originator object.</param>
        /// <param name="memento">The memento representing the originator's state.</param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is null.</exception>
        public Caretaker(string id, string parentID, int processID, DateTime processStartTime, DatabaseConnection connection, TOriginator originator, TMemento memento)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            ParentID = parentID ?? throw new ArgumentNullException(nameof(parentID));
            ProcessID = processID;
            ProcessStartTime = processStartTime;
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Originator = originator ?? throw new ArgumentNullException(nameof(originator));
            Memento = memento ?? throw new ArgumentNullException(nameof(memento));
        }

        /// <summary>
        /// Adds this caretaker to the collection, ensuring no duplicate resource lock exists.
        /// Throws if the resource is already locked by another caretaker.
        /// </summary>
        /// <param name="caretakers">The existing caretakers.</param>
        /// <returns>The updated collection of caretakers including this one.</returns>
        /// <exception cref="ResourceLockedException">Thrown if the resource is already locked.</exception>
        public IEnumerable<ICaretaker> AddCaretaker(IEnumerable<ICaretaker> caretakers)
        {
            foreach (var caretaker in caretakers)
            {
                if (caretaker.ID == ID)
                {
                    throw new ResourceLockedException($"The resource with ID '{ID}' is locked by another snapshotter (ID '{caretaker.ParentID}').");
                }
                yield return caretaker;
            }
            Memento = Originator.GetState();
            yield return this;
        }

        /// <summary>
        /// Removes this caretaker from the collection and restores the originator's state.
        /// </summary>
        /// <param name="caretakers">The existing caretakers.</param>
        /// <returns>The updated collection of caretakers without this one.</returns>
        public IEnumerable<ICaretaker> RemoveCaretaker(IEnumerable<ICaretaker> caretakers)
        {
            foreach (var caretaker in caretakers)
            {
                if (caretaker.ID == ID)
                {
                    Restore();
                }
                else
                {
                    yield return caretaker;
                }
            }
        }

        /// <summary>
        /// Restores the originator's state from the memento.
        /// </summary>
        public virtual void Restore()
        {
            Originator.SetState(Memento);
        }

        /// <summary>
        /// Disposes the caretaker, removing it from the database and releasing the resource lock.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Connection.BeginTransaction();
                    try
                    {
                        Connection.UpdateCaretakers(RemoveCaretaker);
                        Connection.CommitTransaction();
                    }
                    catch
                    {
                        Connection.RollbackTransaction();
                        throw;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Caretaker()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        /// <summary>
        /// Disposes the caretaker and suppresses finalization.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
