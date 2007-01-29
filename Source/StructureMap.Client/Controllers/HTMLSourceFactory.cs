using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Client.Controllers
{
    public class HTMLSourceFactory : IHTMLSourceFactory
    {
        private InstanceManager _instanceManager;

        public HTMLSourceFactory()
        {
            PluginGraph pluginGraph = new PluginGraph();
            pluginGraph.Assemblies.Add(Assembly.GetExecutingAssembly());
            pluginGraph.Seal();
            _instanceManager = new InstanceManager(pluginGraph);
        }

        public IHTMLSource GetSource(string viewName)
        {
            return (IHTMLSource) _instanceManager.CreateInstance(typeof (IHTMLSource), viewName);
        }
    }
}