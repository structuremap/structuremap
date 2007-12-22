using System;

namespace StructureMap.AutoMocking
{
    public interface ServiceLocator
    {
        T Service<T>();
        object Service(Type serviceType);
    }
    
    public class AutoMockedInstanceManager : InstanceManager
    {
        private readonly ServiceLocator _locator;

        public AutoMockedInstanceManager(ServiceLocator locator)
        {
            _locator = locator;
        }

        public override IInstanceFactory this[Type pluginType]
        {
            get
            {
                return getOrCreateFactory(pluginType, 
                    delegate
                      {
                          object service = _locator.Service(pluginType);
                          InstanceFactory factory
                              = InstanceFactory.CreateFactoryWithDefault(pluginType, service);
                          
                          return factory;
                      });
                
                
            }
            set { base[pluginType] = value; }
        }
    }
}
