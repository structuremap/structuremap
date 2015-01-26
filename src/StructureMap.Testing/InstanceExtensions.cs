using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Building;

namespace StructureMap.Testing
{
    public static class InstanceExtensions
    {
        public static TPluginType Build<TPluginType>(this Instance instance, IBuildSession session = null) where TPluginType : class
        {
            var plan = instance.ResolveBuildPlan(typeof (TPluginType), new Policies());
            var buildSession = session ?? new FakeBuildSession();
            return plan.Build(buildSession, buildSession.As<IContext>()).As<TPluginType>();
        }
    }
}