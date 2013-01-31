using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Source;

namespace StructureMap
{
    internal class SystemRegistry : Registry
    {
        public SystemRegistry()
        {
            For<MementoSource>().Use<MemoryMementoSource>();

            AddLifecycleType<SingletonLifecycle>(InstanceScope.Singleton);
            AddLifecycleType<HttpContextLifecycle>(InstanceScope.HttpContext);
            AddLifecycleType<HttpSessionLifecycle>(InstanceScope.HttpSession);
            AddLifecycleType<HybridLifecycle>(InstanceScope.Hybrid);
            AddLifecycleType<HybridSessionLifecycle>(InstanceScope.HybridHttpSession);
            AddLifecycleType<ThreadLocalStorageLifecycle>(InstanceScope.ThreadLocal);
        }


        private void AddLifecycleType<T>(InstanceScope scope) where T : ILifecycle
        {
            addExpression(graph => graph.AddType(typeof (ILifecycle), typeof (T), scope.ToString()));
        }
    }
}