using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.Mementos
{
    [Obsolete("Eliminate!")]
    public class PrototypeMemento : InstanceMemento
    {
        private readonly string _instanceKey;
        private ICloneable _prototype;

        public PrototypeMemento(string instanceKey, ICloneable prototype)
        {
            _instanceKey = instanceKey;
            _prototype = prototype;
        }

        public override Plugin FindPlugin(PluginFamily family)
        {
            return null;
        }

        public ICloneable Prototype
        {
            get { return _prototype; }
            set { _prototype = value; }
        }

        protected override string innerConcreteKey
        {
            get { return string.Empty; }
        }

        protected override string innerInstanceKey
        {
            get { return _instanceKey; }
        }

        public override bool IsReference
        {
            get { return false; }
        }

        public override string ReferenceKey
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


        protected override Instance readInstance(PluginGraph pluginGraph, Type pluginType)
        {
            return new PrototypeInstance(_prototype);
        }
    }
}