using System;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Acceptance;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_549_precedence_of_contructor_policies
    {
        [Fact]
        public void called_in_reverse_order_of_registration()
        {
            var firstPolicy = new CustomConstructorPolicy("first");
            var lastPolicy = new CustomConstructorPolicy("last");

            var container = new Container(_ =>
            {
                _.Policies.ConstructorSelector(firstPolicy);
                _.Policies.ConstructorSelector(lastPolicy);

                _.For<IWidget>().Use<AWidget>();
                _.For<IService>().Use<AService>();
            });

            container.GetInstance<Target>().ShouldNotBeNull();

            firstPolicy.WasCalled.ShouldBeFalse();
            lastPolicy.WasCalled.ShouldBeTrue();
        }

        public class Target
        {
            public Target(IWidget widget)
            {
            }

            public Target(IWidget widget, IService service)
            {
                
            }
        }
    }

    public class CustomConstructorPolicy : IConstructorSelector
    {
        public string Name { get; }

        public CustomConstructorPolicy(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}";
        }

        public ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph)
        {
            WasCalled = true;
            return new GreediestConstructorSelector().Find(pluggedType, dependencies, graph);
        }

        public bool WasCalled { get; set; }
    }
}