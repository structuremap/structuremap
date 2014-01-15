using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class NullInstance : Instance
    {
        protected override string getDescription()
        {
            return "NULL";
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new Constant(pluginType, null);
        }
    }
}