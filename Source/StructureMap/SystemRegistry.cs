using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Source;

namespace StructureMap
{
    internal class SystemRegistry : Registry
    {
        public SystemRegistry()
        {
            addExpression(graph => graph.AddType(typeof(MementoSource), typeof(XmlFileMementoSource), "XmlFile"));

            ForRequestedType<MementoSource>().TheDefaultIsConcreteType<MemoryMementoSource>();
            AddMementoSourceType<DirectoryXmlMementoSource>("DirectoryXml");
            AddMementoSourceType<EmbeddedFolderXmlMementoSource>("EmbeddedXmlFolder");
            AddMementoSourceType<SingleEmbeddedXmlMementoSource>("EmbeddedXmlFile");
            AddMementoSourceType<TemplatedMementoSource>("Templated");
            AddMementoSourceType<XmlAttributeFileMementoSource>("XmlAttributeFile");
            AddMementoSourceType<XmlFileMementoSource>("XmlFile");


            AddInterceptorType<SingletonPolicy>(InstanceScope.Singleton);
            AddInterceptorType<ThreadLocalStoragePolicy>(InstanceScope.ThreadLocal);
            AddInterceptorType<HttpContextBuildPolicy>(InstanceScope.HttpContext);
            AddInterceptorType<HttpSessionBuildPolicy>(InstanceScope.HttpSession);
            AddInterceptorType<HybridBuildPolicy>(InstanceScope.Hybrid);

        }

        private void AddMementoSourceType<T>(string name)
        {
            addExpression(graph => graph.AddType(typeof(MementoSource), typeof(T), name));
        }

        private void AddInterceptorType<T>(InstanceScope scope)
        {
            addExpression(graph => graph.AddType(typeof(IBuildInterceptor), typeof(T), scope.ToString()));
        }
    }
}