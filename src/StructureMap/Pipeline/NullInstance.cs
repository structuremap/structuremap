using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class NullInstance : Instance
    {
        public override string Description
        {
            get { return "NULL"; }
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new Constant(pluginType, null);
        }

        public override Type ReturnedType
        {
            get { return null; }
        }
    }
}