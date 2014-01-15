using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class DefaultInstance : Instance
    {
        protected override string getDescription()
        {
            return "Default";
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return EnumerableInstance.IsEnumerable(pluginType)
                ? (IDependencySource) new AllPossibleValuesDependencySource(pluginType)
                : new DefaultDependencySource(pluginType);
        }
    }
}