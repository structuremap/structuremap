using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.AutoMocking
{
    public class AutoMockedContainer : Container
    {
        private readonly ServiceLocator _locator;

        public AutoMockedContainer() : this(new RhinoMocksServiceLocator())
        {
        }

        public AutoMockedContainer(ServiceLocator locator)
        {
            _locator = locator;

            onMissingFactory = delegate(Type pluginType, ProfileManager profileManager)
            {
                if (!pluginType.IsAbstract && pluginType.IsClass)
                {
                    return null;
                }

                InstanceFactory factory = new InstanceFactory(new PluginFamily(pluginType));

                try
                {
                    object service = _locator.Service(pluginType);

                    LiteralInstance instance = new LiteralInstance(service);

                    profileManager.SetDefault(pluginType, instance);
                }
                catch (Exception)
                {
                    // ignore errors
                }

                return factory;
            };
        }
    }
}