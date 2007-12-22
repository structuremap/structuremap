using System;

namespace StructureMap.Configuration.DSL
{
    public class LiteralMemento : InstanceMemento
    {
        private object _instance;

        public LiteralMemento(object instance)
        {
            _instance = instance;
            InstanceKey = Guid.NewGuid().ToString();
        }

        public LiteralMemento Named(string name)
        {
            InstanceKey = name;
            return this;
        }

        public object Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        protected override string innerConcreteKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string innerInstanceKey
        {
            get { throw new NotImplementedException(); }
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

        protected override object buildInstance(IInstanceCreator creator)
        {
            return _instance;
        }
    }
}