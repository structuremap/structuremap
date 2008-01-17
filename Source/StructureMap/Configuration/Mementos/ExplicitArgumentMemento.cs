using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Graph;

namespace StructureMap.Configuration.Mementos
{
    public class ExplicitArgumentMemento : InstanceMemento
    {
        private readonly ExplicitArguments _args;
        private InstanceMemento _inner;

        public ExplicitArgumentMemento(ExplicitArguments args, InstanceMemento inner)
        {
            _args = args;
            _inner = inner;
        }


        protected override object buildInstance(IInstanceCreator creator)
        {
            if (_inner == null)
            {
                _inner = creator.DefaultMemento;
            }

            return base.buildInstance(creator);
        }

        protected override string innerConcreteKey
        {
            get { return _inner.ConcreteKey; }
        }

        protected override string innerInstanceKey
        {
            get { return _inner.InstanceKey; }
        }

        public override bool IsReference
        {
            get { return false; }
        }

        public override string ReferenceKey
        {
            get { return _inner.ReferenceKey; }
        }

        protected override string getPropertyValue(string Key)
        {
            return _args.GetArg(Key) ?? _inner.GetProperty(Key);
        }

        protected override InstanceMemento getChild(string Key)
        {
            return _inner.GetChildMemento(Key);
        }

        public override object GetChild(string key, string typeName, InstanceManager manager)
        {
            Type type = Type.GetType(typeName, true);
            return _args.Get(type) ?? base.GetChild(key, typeName, manager);
        }

        public override InstanceMemento[] GetChildrenArray(string Key)
        {
            return _inner.GetChildrenArray(Key);
        }
    }
}
