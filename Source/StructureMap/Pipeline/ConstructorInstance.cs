using System;

namespace StructureMap.Pipeline
{
    public delegate object BuildObjectDelegate();

    public class ConstructorInstance : ExpressedInstance<ConstructorInstance>
    {
        private BuildObjectDelegate _builder;


        public ConstructorInstance(BuildObjectDelegate builder)
        {
            _builder = builder;
        }

        public BuildObjectDelegate Builder
        {
            get { return _builder; }
            set { _builder = value; }
        }

        protected override ConstructorInstance thisInstance
        {
            get { return this; }
        }

        protected override object build(Type pluginType, IBuildSession session)
        {
            return _builder();
        }
    }
}