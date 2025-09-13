using DevOptimal.SystemUtilities.Common.StateManagement.Serialization;
using System;

namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    public abstract class Snapshotter : IDisposable
    {
        private readonly SnapshotSerializer serializer;
        private bool disposedValue;

        internal Snapshotter(SnapshotSerializer serializer)
        {
            this.serializer = serializer;
        }

        protected void PersistSnapshot(ISnapshot snapshot)
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
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
