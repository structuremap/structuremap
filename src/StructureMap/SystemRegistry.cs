using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Source;

namespace StructureMap
{
    internal class SystemRegistry : Registry
    {
        public SystemRegistry()
        {
            addExpression(graph => graph.AddType(typeof (MementoSource), typeof (XmlFileMementoSource), "XmlFile"));

            For<MementoSource>().Use<MemoryMementoSource>();
            AddMementoSourceType<EmbeddedFolderXmlMementoSource>("EmbeddedXmlFolder");
            AddMementoSourceType<SingleEmbeddedXmlMementoSource>("EmbeddedXmlFile");
            AddMementoSourceType<XmlAttributeFileMementoSource>("XmlAttributeFile");
            AddMementoSourceType<XmlFileMementoSource>("XmlFile");

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

        private void AddMementoSourceType<T>(string name)
        {
            addExpression(graph => graph.AddType(typeof (MementoSource), typeof (T), name));
        }
    }
}