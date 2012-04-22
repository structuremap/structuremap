using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.AutoMocking
{
    public class AutoMockedContainer : Container
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

            onMissingFactory = delegate(Type pluginType, ProfileManager profileManager)
            {
                if (!pluginType.IsAbstract && pluginType.IsClass)
                {
                    return null;
                }

                var factory = new InstanceFactory(new PluginFamily(pluginType));

                try
                {
                    object service = _locator.Service(pluginType);

                    var instance = new ObjectInstance(service);

                    profileManager.SetDefault(pluginType, instance);
                }
                catch (Exception)
                {
                    // ignore errors
                }

                return factory;
            };
        }

        private void nameContainer(IContainer container)
        {
            container.Name = "AutoMocking-" + container.Name;
        }
    }
}