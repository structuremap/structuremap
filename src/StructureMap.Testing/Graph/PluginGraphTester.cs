using System;
using NUnit.Framework;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using System.Linq;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class PluginGraphTester
    {
        [Test]
        public void add_type_adds_an_instance_for_type_once_and_only_once()
        {
            var graph = new PluginGraph();

            graph.AddType(typeof (IThingy), typeof (BigThingy));

            PluginFamily family = graph.FindFamily(typeof (IThingy));
            family.Instances
                  .Single()
                  .ShouldBeOfType<ConstructorInstance>()
                  .PluggedType.ShouldEqual(typeof (BigThingy));

            graph.AddType(typeof (IThingy), typeof (BigThingy));

            family.Instances.Count().ShouldEqual(1);
        }

        [Test, ExpectedException(typeof (StructureMapConfigurationException))]
        public void AssertErrors_throws_StructureMapConfigurationException_if_there_is_an_error()
        {
            var graph = new PluginGraph();
            graph.Log.RegisterError(400, new ApplicationException("Bad!"));

            graph.Log.AssertFailures();
        }

        [Test]
        public void FindPluginFamilies()
        {
            var graph = new PluginGraph();
            graph.Scan(x => { x.Assembly("StructureMap.Testing.Widget"); });

            graph.Seal();


            foreach (PluginFamily family in graph.PluginFamilies)
            {
                Console.WriteLine(family.PluginType.AssemblyQualifiedName);
            }

            Assert.AreEqual(4, graph.FamilyCount);
        }

        [Test]
        public void FindPlugins()
        {
            var graph = new PluginGraph();
            graph.Scan(x =>
            {
                x.Assembly("StructureMap.Testing.Widget");
                x.Assembly("StructureMap.Testing.Widget2");
            });

            graph.FindFamily(typeof (Rule));

            graph.Seal();

            

            var family = graph.FindFamily(typeof (Rule));

            // there are 2 Rule classes in these assemblies that do not have
            // primitive arguments
            family.Instances.Count().ShouldEqual(2);
        }


        [Test]
        public void Seal_does_not_throw_an_exception_if_there_are_no_errors()
        {
            var graph = new PluginGraph();
            Assert.AreEqual(0, graph.Log.ErrorCount);

            graph.Seal();
        }
    }

    [PluginFamily]
    public interface IThingy
    {
        void Go();
    }

    [Pluggable("Big")]
    public class BigThingy : IThingy
    {
        #region IThingy Members

        public void Go()
        {
        }

        #endregion
    }
}