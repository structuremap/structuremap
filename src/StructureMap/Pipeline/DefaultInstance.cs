using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class DefaultInstance : Instance
    {
        protected override object build(Type pluginType, IBuildSession session)
        {
            if (EnumerableInstance.IsEnumerable(pluginType))
            {
                var enumerable = new EnumerableInstance(null);
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
                ? (IDependencySource) new AllPossibleValuesDependencySource(pluginType)
                : new DefaultDependencySource(pluginType);
        }
    }
}