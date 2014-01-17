using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class setting_lifecycle_scope
    {
        [Test]
        public void can_specify_at_family_level()
        {
            var container = new Container(x => {
                x.For<Rule>().Singleton().Use<ARule>();
            });

            container.GetInstance<Rule>()
                .ShouldBeTheSameAs(container.GetInstance<Rule>())
                .ShouldBeTheSameAs(container.GetInstance<Rule>())
                .ShouldBeTheSameAs(container.GetInstance<Rule>());
        }

        [Test]
        public void can_override_lifecycle_at_instance()
        {
            var container = new Container(x => {
                x.For<Rule>().Singleton();

                x.For<Rule>().Add<ARule>().Named("A").Transient();
                x.For<Rule>().Add<ARule>().Named("B").AlwaysUnique();
                x.For<Rule>().Add<ARule>().Named("C");
            });

            container.Model.For<Rule>().Lifecycle.ShouldBeOfType<SingletonLifecycle>();
            container.Model.For<Rule>().Find("C").Lifecycle.ShouldBeOfType<SingletonLifecycle>();

            // 'C' is the default lifecycle for Rule (Singleton)
            container.GetInstance<Rule>("C")
                .ShouldBeTheSameAs(container.GetInstance<Rule>("C"));

            // 'A' is a transient
            container.GetInstance<Rule>("A")
                .ShouldNotBeTheSameAs(container.GetInstance<Rule>("A"));

            using (var nested = container.GetNestedContainer())
            {
                // 'B' is always unique
                nested.GetInstance<Rule>("B")
                    .ShouldNotBeTheSameAs(nested.GetInstance<Rule>("B"));
            }
        }
    }
}