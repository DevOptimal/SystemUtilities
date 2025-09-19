using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using System;
using System.Collections.Generic;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    // TODO: Can we combine the database and snapshotter into one class?
    public abstract class Snapshotter : IDisposable
    {
        internal readonly Database database;
        private readonly List<ISnapshot> snapshots;
        private bool disposedValue;

        internal Snapshotter(SnapshotSerializer serializer)
        {
            database = new Database(serializer);
            snapshots = [];
        }

        protected void AddSnapshot(ISnapshot snapshot)
        {
            snapshots.Add(snapshot);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var snapshot in snapshots)
                    {
                        snapshot.Dispose();
                    }
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
    }
}
