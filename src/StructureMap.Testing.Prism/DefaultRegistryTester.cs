using Microsoft.Practices.Composite;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Wpf.Regions;
using NUnit.Framework;
using StructureMap.Prism;

namespace StructureMap.Testing.Prism
{
    [TestFixture]
    public class DefaultRegistryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(new DefaultRegistry());
        }

        #endregion

        private Container container;

        [Test]
        public void sets_up_the_default_event_aggregator_as_a_singleton()
        {
            container.GetInstance<IEventAggregator>().ShouldBeOfType<EventAggregator>();
            container.GetInstance<IEventAggregator>().ShouldBeTheSameAs(container.GetInstance<IEventAggregator>());
        }

        [Test]
        public void sets_up_the_default_region_adapter_as_a_singleton()
        {
            container.GetInstance<RegionAdapterMappings>().ShouldBeOfType<RegionAdapterMappings>();
            container.GetInstance<RegionAdapterMappings>().ShouldBeTheSameAs(
                container.GetInstance<RegionAdapterMappings>());
        }

        [Test]
        public void sets_up_the_icontainerfacade_to_wrap_itself()
        {
            var theView = new TheView();
            container.Inject(theView);

            container.GetInstance<IContainerFacade>()
                .ShouldBeOfType<StructureMapContainerFacade>()
                .Container.ShouldBeTheSameAs(container);
        }
    }

    public class TheView
    {
    }
}