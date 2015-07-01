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
            Assert.AreEqual(typeof (IConvention), new DefaultConventionScanner().FindPluginType(typeof (Convention)));
            Assert.IsNull(new DefaultConventionScanner().FindPluginType(GetType()));
        }


        [Test]
        public void Process_to_Container()
        {
            var container = new Container(registry => {
                registry.Scan(x => {
                    x.TheCallingAssembly();
                    x.Convention<DefaultConventionScanner>();
                });
            });

            container.GetInstance<IConvention>().ShouldBeOfType<Convention>();
        }


        [Test]
        public void Process_to_Container_2()
        {
            var container = new Container(registry => {
                registry.Scan(x => {
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

            Assert.IsFalse(graph.Families.Has(typeof (IServer)));
            Assert.IsTrue(graph.Families.Has(typeof (IConvention)));

            var family = graph.Families[typeof (IConvention)];
            Assert.AreEqual(1, family.Instances.Count());
        }

        [Test]
        public void can_configure_plugin_families_via_dsl()
        {
            var container = new Container(registry => registry.Scan(x => {
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