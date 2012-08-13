using Microsoft.Practices.Composite;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Wpf.Regions;
using StructureMap.Configuration.DSL;

namespace StructureMap.Prism
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            ForSingletonOf<IContainerFacade>().TheDefaultIsConcreteType<StructureMapContainerFacade>();
            ForSingletonOf<IEventAggregator>().TheDefaultIsConcreteType<EventAggregator>();
            ForSingletonOf<RegionAdapterMappings>().TheDefaultIsConcreteType<RegionAdapterMappings>();
        }
    }
}