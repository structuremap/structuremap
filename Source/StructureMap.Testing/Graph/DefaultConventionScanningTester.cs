using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Assert.AreEqual(typeof(IConvention), new DefaultConventionScanner().FindPluginType(typeof(Convention)));
            Assert.IsNull(new DefaultConventionScanner().FindPluginType(this.GetType()));
        }


        [Test]
        public void Process_to_PluginGraph()
        {
            PluginGraph graph = new PluginGraph();
            DefaultConventionScanner scanner = new DefaultConventionScanner();
            scanner.Process(typeof(Convention), graph);

            Assert.IsFalse(graph.PluginFamilies.Contains(typeof(IServer)));
            Assert.IsTrue(graph.PluginFamilies.Contains(typeof(IConvention)));

            PluginFamily family = graph.FindFamily(typeof (IConvention));
            family.Seal();
            Assert.AreEqual(1, family.InstanceCount);
        }

        [Test]
        public void Process_to_Container()
        {
            Container container = new Container(registry => registry.ScanAssemblies().IncludeTheCallingAssembly()
                                                                .With(new DefaultConventionScanner()));

            Debug.WriteLine(container.WhatDoIHave());

            Assert.IsInstanceOfType(typeof(Convention), container.GetInstance<IConvention>());
        }


        [Test]
        public void Process_to_Container_2()
        {
            Container container = new Container(registry => registry.ScanAssemblies().IncludeTheCallingAssembly()
                                                                .With<DefaultConventionScanner>());

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
