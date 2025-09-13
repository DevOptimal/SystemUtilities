using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using System;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    internal class Caretaker<TOriginator, TMemento> : ISnapshot
        where TOriginator : IOriginator<TMemento>
        where TMemento : IMemento
    {
        public string ID { get; set; }

        public int ProcessID { get; set; }

        public DateTime ProcessStartTime { get; set; }

        public SnapshotSerializer Serializer { get; set; }

        public TOriginator Originator { get; set; }

        public TMemento Memento { get; set; }

        private bool disposedValue;

        public Caretaker(string id, int processID, DateTime processStartTime, SnapshotSerializer serializer, TOriginator originator) : this(id, processID, processStartTime, serializer, originator, originator.GetState())
        {
            Serializer.AddSnapshot(this);
        }

        // For serialization
        public Caretaker(string id, int processID, DateTime processStartTime, SnapshotSerializer serializer, TOriginator originator, TMemento memento)
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
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            Originator = originator;
            Memento = memento;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Originator.SetState(Memento);
                    Serializer.RemoveSnapshot(this);
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
