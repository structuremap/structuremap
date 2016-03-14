using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.Widget;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class setting_lifecycle_scope
    {
        [Fact]
        public void can_specify_at_family_level()
        {
            var container = new Container(x => { x.For<Rule>().Singleton().Use<ARule>(); });

            container.GetInstance<Rule>()
                .ShouldBeTheSameAs(container.GetInstance<Rule>())
                .ShouldBeTheSameAs(container.GetInstance<Rule>())
                .ShouldBeTheSameAs(container.GetInstance<Rule>());
        }

        // SAMPLE: lifecycle-rules
        [Fact]
        public void lifecycle_precedence()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Use<AWidget>();

                // Configure the default lifecycle for
                // a PluginType (Rule)
                x.For<Rule>().Singleton();
                x.For<Rule>().Add<ARule>().Named("C");

                // Configure the lifecycle for a single Instance
                x.For<Rule>().Add<ARule>().Named("A").Transient();
                x.For<Rule>().Add<ARule>().Named("B").AlwaysUnique();
            });

            // The default lifecycle is Transient
            container.Model.For<IWidget>().Default
                .Lifecycle.ShouldBeOfType<TransientLifecycle>();

            // Override at the Family
            container.Model.For<Rule>().Lifecycle.ShouldBeOfType<SingletonLifecycle>();
            container.Model.For<Rule>().Find("C").Lifecycle.ShouldBeOfType<SingletonLifecycle>();

            // 'C' is the default lifecycle for Rule (SingletonThing)
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

        // ENDSAMPLE
    }
}