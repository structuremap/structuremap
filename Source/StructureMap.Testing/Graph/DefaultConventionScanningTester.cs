using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class DefaultConventionScanningTester
    {
        [Test]
        public void FindPluginType()
        {
            Assert.AreEqual(typeof(IConvention), DefaultConventionScanner.FindPluginType(typeof(Convention)));
            Assert.IsNull(DefaultConventionScanner.FindPluginType(this.GetType()));
        }


        [Test]
        public void Process_to_PluginGraph()
        {
            Registry registry = new Registry();
            DefaultConventionScanner scanner = new DefaultConventionScanner();
            scanner.Process(typeof(Convention), registry);

            PluginGraph graph = registry.Build();

            Assert.IsFalse(graph.PluginFamilies.Contains(typeof(IServer)));
            Assert.IsTrue(graph.PluginFamilies.Contains(typeof(IConvention)));

            PluginFamily family = graph.FindFamily(typeof (IConvention));
            Assert.AreEqual(1, family.InstanceCount);
        }

        [Test]
        public void Process_to_Container()
        {
            Container container = new Container(delegate(Registry registry)
            {
                registry.ScanAssemblies().IncludeTheCallingAssembly()
                    .With(new DefaultConventionScanner());
            });

            Assert.IsInstanceOfType(typeof(Convention), container.GetInstance<IConvention>());
        }
    }

    public interface IConvention{}
    public interface IServer{}
    public class Convention : IConvention, IServer
    {
        public void Go()
        {
            throw new NotImplementedException();
        }
    }

    public class Something : IConvention{}
}
