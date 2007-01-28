using System.Collections;

namespace StructureMap.Source
{
    /// <summary>
    /// An in-memory MementoSource
    /// </summary>
    [Pluggable("Default", "")]
    public class MemoryMementoSource : MementoSource
    {
        private Hashtable _mementos;

        public MemoryMementoSource()
        {
            _mementos = new Hashtable();
        }

        protected override InstanceMemento[] fetchInternalMementos()
        {
            InstanceMemento[] returnValue = new InstanceMemento[_mementos.Count];
            _mementos.Values.CopyTo(returnValue, 0);

            return returnValue;
        }

        public void AddMemento(InstanceMemento Memento)
        {
            _mementos.Add(Memento.InstanceKey, Memento);
        }

        protected override InstanceMemento retrieveMemento(string Key)
        {
            return _mementos[Key] as InstanceMemento;
        }


        protected internal override bool containsKey(string Key)
        {
            return _mementos.ContainsKey(Key);
        }

        public override string Description
        {
            get { return "DefaultMementoSource"; }
        }
    }
}