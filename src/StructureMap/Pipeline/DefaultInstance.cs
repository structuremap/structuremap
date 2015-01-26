using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class DefaultInstance : Instance
    {
        public override Type ReturnedType
        {
            get { return null; }
        }

        public override string Description
        {
            get { return "Default"; }
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return EnumerableInstance.IsEnumerable(pluginType)
                ? (IDependencySource) new AllPossibleValuesDependencySource(pluginType)
                : new DefaultDependencySource(pluginType);
        }
    }
}