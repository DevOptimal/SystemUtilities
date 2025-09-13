using System;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    internal class Caretaker<TOriginator, TMemento> : ISnapshot
        where TOriginator : IOriginator<TMemento>
        where TMemento : IMemento
    {
        public string ID { get; set; }

        public int ProcessID { get; set; }

        public DateTime ProcessStartTime { get; set; }

        public Database Database { get; set; }

        public TOriginator Originator { get; set; }

        public TMemento Memento { get; set; }

        private bool disposedValue;

        public Caretaker(string id, int processID, DateTime processStartTime, Database database, TOriginator originator) : this(id, processID, processStartTime, database, originator, originator.GetState())
        {
            Database.BeginTransaction(TimeSpan.FromSeconds(30));
            try 
            {
                Database.UpdateSnapshots(AddSnapshot);
                Database.CommitTransaction();
            }
            catch
            {
                Database.RollbackTransaction();
                throw;
            }
        }

        // For serialization
        public Caretaker(string id, int processID, DateTime processStartTime, Database database, TOriginator originator, TMemento memento)
        {
            if (originator == null)
            {
                throw new ArgumentNullException(nameof(originator));
            }

            if (memento == null)
            {
                throw new ArgumentNullException(nameof(memento));
            }

            ID = id ?? throw new ArgumentNullException(nameof(id));
            ProcessID = processID;
            ProcessStartTime = processStartTime;
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Originator = originator;
            Memento = memento;
        }

        private IEnumerable<ISnapshot> AddSnapshot(IEnumerable<ISnapshot> snapshots)
        {
            foreach (var snapshot in snapshots)
            {
                if (snapshot.ID == ID)
                {
                    throw new Exception();
                }
                yield return snapshot;
            }
            yield return this;
        }

        private IEnumerable<ISnapshot> RemoveSnapshot(IEnumerable<ISnapshot> snapshots)
        {
            foreach (var snapshot in snapshots)
            {
                if (snapshot.ID != ID)
                {
                    yield return snapshot;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Originator.SetState(Memento);
                    Database.BeginTransaction(TimeSpan.FromSeconds(30));
                    try
                    {
                        Database.UpdateSnapshots(RemoveSnapshot);
                        Database.CommitTransaction();
                    }
                    catch
                    {
                        Database.RollbackTransaction();
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
