using System;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_248_do_not_allow_lifecycle_registrations_to_nested_container
    {
        private Container container;

        [SetUp]
        public void SetUp()
        {
            container = new Container(x => {
                x.For<IWidget>().Use<AWidget>();
            });
        }

        [Test]
        public void okay_to_register_default()
        {
            container.GetNestedContainer().Configure(x => {
                x.For<IWidget>().Use<MoneyWidget>();
            });
        }

        [Test]
        public void okay_to_register_unique()
        {
            container.GetNestedContainer().Configure(x =>
            {
                x.For<IWidget>().Use<MoneyWidget>().AlwaysUnique();
            });
        }

        [Test]
        public void okay_to_register_a_pre_built_object()
        {
            container.GetNestedContainer().Configure(x => {
                x.For<IWidget>().Use(new MoneyWidget());
            });
        }

        [Test]
        public void not_okay_to_register_a_singleton()
        {
            IContainer nestedContainer = container.GetNestedContainer();

            var ex = Exception<InvalidOperationException>.ShouldBeThrownBy(() => {
                nestedContainer.Configure(x =>
                {
                    x.For<IWidget>().Use<MoneyWidget>().Singleton();
                });
            });

            Debug.WriteLine(ex);
        }
    }
}