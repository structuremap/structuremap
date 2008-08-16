using System;

namespace StructureMap.Pipeline
{


    public class ConstructorInstance : ExpressedInstance<ConstructorInstance>
    {
        private Func<object> _builder;

        public ConstructorInstance(Func<object> builder)
        {
            _builder = builder;
        }

        public Func<object> Builder
        {
            get { return _builder; }
            set { _builder = value; }
        }

        protected override ConstructorInstance thisInstance
        {
            get { return this; }
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            try
            {
                return _builder();
            }
            catch (Exception ex)
            {
                throw new StructureMapException(207, ex, Name, pluginType);
            }
        }

        protected override string getDescription()
        {
            return "Instance is created by Func<object> function:  " + _builder.ToString();
        }
    }
}