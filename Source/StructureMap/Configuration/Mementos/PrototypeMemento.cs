using System;

namespace StructureMap.Configuration.Mementos
{
    public class PrototypeMemento : InstanceMemento
    {
        private readonly string _instanceKey;
        private ICloneable _prototype;

        public PrototypeMemento(string instanceKey, ICloneable prototype)
        {
            _instanceKey = instanceKey;
            _prototype = prototype;
        }


        public ICloneable Prototype
        {
            get { return _prototype; }
            set { _prototype = value; }
        }

        protected override object buildInstance(IInstanceCreator creator)
        {
            return _prototype.Clone();
        }

        protected override string innerConcreteKey
        {
            get { return string.Empty; }
        }

        protected override string innerInstanceKey
        {
            get { return _instanceKey; }
        }

        protected override string getPropertyValue(string Key)
        {
            throw new NotImplementedException();
        }

        protected override InstanceMemento getChild(string Key)
        {
            throw new NotImplementedException();
        }

        public override InstanceMemento[] GetChildrenArray(string Key)
        {
            throw new NotImplementedException();
        }

        public override bool IsReference
        {
            get { return false; }
        }

        public override string ReferenceKey
        {
            get { throw new NotImplementedException(); }
        }
    }
}