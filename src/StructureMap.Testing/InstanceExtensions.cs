using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Building;

namespace StructureMap.Testing
{
    public static class InstanceExtensions
    {
        public static TPluginType Build<TPluginType>(this Instance instance, IBuildSession session = null) where TPluginType : class
        {
            var plan = instance.CreatePlan(typeof (TPluginType), new Policies());
            return plan.Build(session ?? new FakeBuildSession()).As<TPluginType>();
        }
    }
}