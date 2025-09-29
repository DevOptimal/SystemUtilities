using DevOptimal.SystemUtilities.Common.StateManagement.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    internal class Caretaker<TOriginator, TMemento> : ICaretaker, ISnapshot
        where TOriginator : IOriginator<TMemento>
        where TMemento : IMemento
    {
        public string ID { get; set; }

        public string ParentID { get; }

        public int ProcessID { get; set; }

        public DateTime ProcessStartTime { get; set; }

        public TOriginator Originator { get; set; }

        public TMemento Memento { get; set; }

        public DatabaseConnection Connection { get; set; }

        private bool disposedValue;

        public Caretaker(TOriginator originator, DatabaseConnection connection)
        {
            Originator = originator ?? throw new ArgumentNullException(nameof(originator));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            ID = originator.GetID();
            ParentID = connection.ID;
            var currentProcess = Process.GetCurrentProcess();
            ProcessID = currentProcess.Id;
            ProcessStartTime = currentProcess.StartTime;

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

        // For serialization
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

        public IEnumerable<ICaretaker> AddCaretaker(IEnumerable<ICaretaker> caretakers)
        {
            foreach (var caretaker in caretakers)
            {
                if (caretaker.ID == ID)
                {
                    throw new ResourceLockedException();
                }
                yield return caretaker;
            }
            Memento = Originator.GetState();
            yield return this;
        }

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

        public void Restore()
        {
            Originator.SetState(Memento);
        }

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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
