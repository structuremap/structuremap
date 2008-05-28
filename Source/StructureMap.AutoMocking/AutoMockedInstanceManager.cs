using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.AutoMocking
{
    public class AutoMockedInstanceManager : Container
    {
        private readonly ServiceLocator _locator;

        public AutoMockedInstanceManager(ServiceLocator locator)
        {
            _locator = locator;

            onMissingFactory = delegate(Type pluginType, ProfileManager profileManager)
                                   {
                                       if (!pluginType.IsAbstract && pluginType.IsClass)
                                       {
                                           return null;
                                       }

                                       object service = _locator.Service(pluginType);
                                       InstanceFactory factory = new InstanceFactory(new PluginFamily(pluginType));

                                       LiteralInstance instance = new LiteralInstance(service);

                                       profileManager.SetDefault(pluginType, instance);

                                       return factory;
                                   };
        }

    }
}