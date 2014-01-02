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

        protected override object build(Type pluginType, BuildSession session)
        {
            return null;
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotSupportedException();
        }
    }
}