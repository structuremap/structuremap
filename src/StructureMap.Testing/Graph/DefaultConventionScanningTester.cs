using Shouldly;
using StructureMap.Graph;
using System;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class DefaultConventionScanningTester
    {
        [Fact]
        public void FindPluginType()
        {
            new DefaultConventionScanner().FindPluginType(typeof(Convention))
                .ShouldBe(typeof(IConvention));

            new DefaultConventionScanner().FindPluginType(GetType()).ShouldBeNull();
        }

        [Fact]
        public void Process_to_Container()
        {
            var container = new Container(registry =>
            {
                registry.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.Convention<DefaultConventionScanner>();
                });
            });

            container.GetInstance<IConvention>().ShouldBeOfType<Convention>();
        }

        [Fact]
        public void Process_to_Container_2()
        {
            var container = new Container(registry =>
            {
                registry.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.With(new DefaultConventionScanner());
                });
            });

            container.GetInstance<IConvention>().ShouldBeOfType<Convention>();
        }

        [Fact]
        public void can_configure_plugin_families_via_dsl()
        {
            var container = new Container(registry => registry.Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions().OnAddedPluginTypes(t => t.Singleton());
            }));

            var firstInstance = container.GetInstance<IConvention>();
            var secondInstance = container.GetInstance<IConvention>();
            secondInstance.ShouldBeTheSameAs(firstInstance);
        }
    }

    public interface IConvention
    {
    }

    public interface IServer
    {
    }

    public class Convention : IConvention, IServer
    {
        public void Go()
        {
            throw new NotImplementedException();
        }
    }

    public class Something : IConvention
    {
    }
}