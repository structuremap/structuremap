using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.AutoMocking
{
    public class AutoMockedInstanceManager : InstanceManager
    {
        private readonly ServiceLocator _locator;

        public AutoMockedInstanceManager(ServiceLocator locator)
        {
            _locator = locator;
        }

        
        protected override InstanceFactory createFactory(Type pluginType)
        {
            if (!pluginType.IsAbstract && pluginType.IsClass)
            {
                return base.createFactory(pluginType);
            }

            object service = _locator.Service(pluginType);
            InstanceFactory factory = new InstanceFactory(new PluginFamily(pluginType));
            LiteralInstance instance = new LiteralInstance(service);
            SetDefault(pluginType, instance);

            return factory;
        }
    }
}