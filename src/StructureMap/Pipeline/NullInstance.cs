using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class NullInstance : Instance
    {
        public NullInstance()
        {
        }

        protected override string getDescription()
        {
            return "NULL";
        }

        protected override object build(Type pluginType, IBuildSession session)
        {
            return null;
        }
    }
}