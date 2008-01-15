using System;
using StructureMap.Configuration.Mementos;

namespace StructureMap
{
    public delegate PLUGINTYPE BuildObjectDelegate<PLUGINTYPE>();

    public class ConstructorMemento<PLUGINTYPE> : MemoryInstanceMemento
    {
        private BuildObjectDelegate<PLUGINTYPE> _builder;


        public ConstructorMemento()
        {
        }

        public ConstructorMemento(string instanceKey, BuildObjectDelegate<PLUGINTYPE> builder)
            : base(instanceKey, instanceKey)
        {
            _builder = builder;
        }

        public ConstructorMemento(BuildObjectDelegate<PLUGINTYPE> builder)
            : this(Guid.NewGuid().ToString(), builder)
        {
        }


        public BuildObjectDelegate<PLUGINTYPE> Builder
        {
            get { return _builder; }
            set { _builder = value; }
        }

        protected override object buildInstance(IInstanceCreator creator)
        {
            return _builder();
        }
    }
}