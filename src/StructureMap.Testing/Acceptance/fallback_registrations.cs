using System;
using Shouldly;
using StructureMap.TypeRules;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class fallback_registrations
    {
        // SAMPLE: fallback_registrations
        public class DefaultServices : Registry
        {
            public DefaultServices()
            {
                // If nobody else provides a default
                // for IWidget, use AWidget
                For<IWidget>().UseIfNone<AWidget>();
            }
        }

        public class SpecificServices : Registry
        {
            public SpecificServices()
            {
                // Use BWidget for IWidget, period
                For<IWidget>().Use<BWidget>();
            }
        }

        // ENDSAMPLE

        // SAMPLE: use-if-none-modularity
        [AttributeUsage(AttributeTargets.Assembly)]
        public class ProductModuleAttribute : Attribute
        {
        }

        public class ApplicationRegistry : Registry
        {
            public ApplicationRegistry()
            {
                // Use the default services as fallbacks
                IncludeRegistry<DefaultServices>();

                // Dependending on what assemblies are present,
                // this might find specific registrations that
                // will take precedence over the UseIfNone()
                // registrations in DefaultServices
                Scan(_ =>
                {
                    _.AssembliesFromApplicationBaseDirectory(
                        assem => assem.HasAttribute<ProductModuleAttribute>());

                    _.LookForRegistries();
                });
            }
        }

        [Fact]
        public void see_use_if_none_in_action()
        {
            var container1 = Container.For<DefaultServices>();

            // No other registrations, so fallback
            // to AWidget
            container1.GetInstance<IWidget>()
                .ShouldBeOfType<AWidget>();

            var container2 = new Container(_ =>
            {
                // add both registries above
                // NOTE: the order does not matter for IWidget

                _.IncludeRegistry<SpecificServices>();
                _.IncludeRegistry<DefaultServices>();
            });

            // The registration in SpecificServices
            // should win out
            container2.GetInstance<IWidget>()
                .ShouldBeOfType<BWidget>();
        }

        // ENDSAMPLE
    }
}