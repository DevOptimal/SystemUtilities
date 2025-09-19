namespace DevOptimal.SystemUtilities.Common.StateManagement
{
    internal interface IOriginator<TMemento>
        where TMemento : IMemento
    {
        string GetID();

        TMemento GetState();

        void SetState(TMemento memento);
    }
}
