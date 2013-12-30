using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class DefaultInstance : Instance
    {
        protected override object build(Type pluginType, BuildSession session)
        {
            if (EnumerableInstance.IsEnumerable(pluginType))
            {
                var enumerable = new EnumerableInstance(pluginType, null);
                return enumerable.Build(pluginType, session);
            }

            return session.GetInstance(pluginType);
        }

        protected override string getDescription()
        {
            return "Default";
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return EnumerableInstance.IsEnumerable(pluginType)
                ? (IDependencySource) new AllPossibleValuesDependencySource(pluginType, EnumerableInstance.DetermineElementType(pluginType))
                : new DefaultDependencySource(pluginType);
        }
    }
}