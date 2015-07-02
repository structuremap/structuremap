using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class DefaultConventionScanningTester
    {
        [Test]
        public void FindPluginType()
        {
            new DefaultConventionScanner().FindPluginType(typeof (Convention))
                .ShouldBe(typeof (IConvention));

            new DefaultConventionScanner().FindPluginType(GetType()).ShouldBeNull();
        }


        [Test]
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


        [Test]
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

        [Test]
        public void Process_to_PluginGraph()
        {
            var graph = new PluginGraph();
            var scanner = new DefaultConventionScanner();

            var registry = new Registry();

            scanner.Process(typeof (Convention), registry);

            registry.As<IPluginGraphConfiguration>().Configure(graph);

            graph.Families.Has(typeof (IServer)).ShouldBeFalse();
            graph.Families.Has(typeof (IConvention)).ShouldBeTrue();

            var family = graph.Families[typeof (IConvention)];
            family.Instances.Count().ShouldBe(1);
        }

        [Test]
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