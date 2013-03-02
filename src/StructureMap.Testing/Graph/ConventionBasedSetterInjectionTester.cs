using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture, Ignore("just for now")]
    public class ConventionBasedSetterInjectionTester
    {
        public class ClassWithNamedProperties
        {
            public int Age { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public IGateway Gateway { get; set; }
            public IService Service { get; set; }
        }

        private Plugin buildPlugin<T>(Action<Registry> configure)
        {
            var registry = new Registry();
            configure(registry);

            PluginGraph graph = registry.Build();

            return graph.Families[typeof (T)].GetDefaultInstance().As<ConstructorInstance>()
                                             .Plugin;
        }

        [Test]
        public void Plugin_uses_setter_rules_to_make_properties_settable()
        {
            var plugin = new Plugin(typeof (PluginTester.ClassWithProperties));

            plugin.Setters.IsMandatory("Engine").ShouldBeFalse();
            plugin.UseSetterRule(prop => prop.PropertyType == typeof (IEngine));
            plugin.Setters.IsMandatory("Engine").ShouldBeTrue();
        }

        [Test]
        public void fill_all_properties_matching_a_certain_name()
        {
            Plugin plugin = buildPlugin<ClassWithNamedProperties>(x => {
                x.SetAllProperties(policy => policy.NameMatches(name => name.EndsWith("Name")));
            });


            plugin.Setters.IsMandatory("FirstName").ShouldBeTrue();
            plugin.Setters.IsMandatory("LastName").ShouldBeTrue();
            plugin.Setters.IsMandatory("Gateway").ShouldBeFalse();
            plugin.Setters.IsMandatory("Service").ShouldBeFalse();
        }


        [Test]
        public void fill_all_properties_of_a_certain_type()
        {
            var plugin = buildPlugin<ClassWithNamedProperties>(x => {
                x.SetAllProperties(policy =>
                {
                    policy.OfType<string>();
                    policy.OfType<IGateway>();
                });
            });

            plugin.Setters.IsMandatory("Age").ShouldBeFalse();
            plugin.Setters.IsMandatory("FirstName").ShouldBeTrue();
            plugin.Setters.IsMandatory("LastName").ShouldBeTrue();
            plugin.Setters.IsMandatory("Gateway").ShouldBeTrue();
            plugin.Setters.IsMandatory("Service").ShouldBeFalse();
        }


        [Test]
        public void fill_all_properties_of_types_in_namespace()
        {
            var plugin = buildPlugin<ClassWithNamedProperties>(x => {
                x.SetAllProperties(policy => {
                    policy.WithAnyTypeFromNamespace("StructureMap.Testing.Widget3");
                });
            });


            plugin.Setters.IsMandatory("Age").ShouldBeFalse();
            plugin.Setters.IsMandatory("FirstName").ShouldBeFalse();
            plugin.Setters.IsMandatory("LastName").ShouldBeFalse();
            plugin.Setters.IsMandatory("Gateway").ShouldBeTrue();
            plugin.Setters.IsMandatory("Service").ShouldBeTrue();
        }

        [Test]
        public void fill_all_properties_of_types_in_namespace_by_generic()
        {
            var plugin = buildPlugin<ClassWithNamedProperties>(x => {
                x.SetAllProperties(policy => { policy.WithAnyTypeFromNamespaceContainingType<IService>(); });
            });

            plugin.Setters.IsMandatory("Age").ShouldBeFalse();
            plugin.Setters.IsMandatory("FirstName").ShouldBeFalse();
            plugin.Setters.IsMandatory("LastName").ShouldBeFalse();
            plugin.Setters.IsMandatory("Gateway").ShouldBeTrue();
            plugin.Setters.IsMandatory("Service").ShouldBeTrue();
        }

        [Test]
        public void specify_setter_policy_and_construct_an_object()
        {
            var theService = new ColorService("red");

            var container = new Container(x => {
                x.For<IService>().Use(theService);
                x.For<IGateway>().Use<DefaultGateway>();

                x.SetAllProperties(policy => { policy.WithAnyTypeFromNamespace("StructureMap.Testing.Widget3"); });
            });

            var target = container.GetInstance<ClassWithNamedProperties>();
            target.Service.ShouldBeTheSameAs(theService);
            target.Gateway.ShouldBeOfType<DefaultGateway>();
        }

        [Test]
        public void specify_setter_policy_by_a_predicate_on_property_type()
        {
            var theService = new ColorService("red");

            var container = new Container(x => {
                x.For<IService>().Use(theService);
                x.For<IGateway>().Use<DefaultGateway>();

                x.SetAllProperties(policy => { policy.TypeMatches(type => type == typeof (IService)); });
            });

            var target = container.GetInstance<ClassWithNamedProperties>();
            target.Service.ShouldBeTheSameAs(theService);
            target.Gateway.ShouldBeNull();
        }
    }
}