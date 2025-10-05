namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    /// <summary>
    /// Defines the contract for an originator in the Memento design pattern.
    /// An originator can create a memento representing its current state and restore its state from a memento.
    /// </summary>
    /// <typeparam name="TMemento">The type of memento used to capture and restore state.</typeparam>
    internal interface IOriginator<TMemento>
        where TMemento : IMemento
    {
        /// <summary>
        /// Gets the unique identifier for this originator.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Captures the current state of the originator as a memento.
        /// </summary>
        /// <returns>A memento representing the current state.</returns>
        TMemento GetState();

        /// <summary>
        /// Restores the originator's state from the provided memento.
        /// </summary>
        /// <param name="memento">The memento to restore state from.</param>
        void SetState(TMemento memento);
    }
}
