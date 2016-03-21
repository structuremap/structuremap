using StructureMap.Testing.Widget;
using System;
using System.Diagnostics;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_248_do_not_allow_lifecycle_registrations_to_nested_container
    {
        private readonly Container container;

        public Bug_248_do_not_allow_lifecycle_registrations_to_nested_container()
        {
            container = new Container(x => { x.For<IWidget>().Use<AWidget>(); });
        }

        [Fact]
        public void okay_to_register_default()
        {
            container.GetNestedContainer().Configure(x => { x.For<IWidget>().Use<MoneyWidget>(); });
        }

        [Fact]
        public void okay_to_register_unique()
        {
            container.GetNestedContainer().Configure(x => { x.For<IWidget>().Use<MoneyWidget>().AlwaysUnique(); });
        }

        [Fact]
        public void okay_to_register_a_pre_built_object()
        {
            container.GetNestedContainer().Configure(x => { x.For<IWidget>().Use(new MoneyWidget()); });
        }

        [Fact]
        public void not_okay_to_register_a_singleton()
        {
            var nestedContainer = container.GetNestedContainer();

            var ex =
                Exception<InvalidOperationException>.ShouldBeThrownBy(
                    () => { nestedContainer.Configure(x => { x.For<IWidget>().Use<MoneyWidget>().Singleton(); }); });

            Debug.WriteLine(ex);
        }
    }
}