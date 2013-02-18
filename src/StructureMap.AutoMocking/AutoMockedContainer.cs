using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.AutoMocking
{
    public class AutoMockedContainer : Container, IFamilyPolicy
    {
        private readonly ServiceLocator _locator;

        public AutoMockedContainer()
            : this(new RhinoMocksAAAServiceLocator())
        {
        }

        public AutoMockedContainer(ServiceLocator locator)
        {
            nameContainer(this);

            _locator = locator;
            Model.PluginGraph.AddFamilyPolicy(this);
        }

        private void nameContainer(IContainer container)
        {
            container.Name = "AutoMocking-" + container.Name;
        }

        public PluginFamily Build(Type pluginType)
        {
            if (!pluginType.IsAbstract && pluginType.IsClass)
            {
                return null;
            }

            var family = new PluginFamily(pluginType);
            object service = _locator.Service(pluginType);

            var instance = new ObjectInstance(service);
            family.SetDefault(instance);

            return family;
        }
    }
}