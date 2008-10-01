using System;
using NUnit.Framework;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class PluginGraphTester
    {


        [Test]
        public void add_type_adds_a_plugin_for_type_once_and_only_once()
        {
            var graph = new PluginGraph();

            graph.AddType(typeof (IThingy), typeof (BigThingy));

            PluginFamily family = graph.FindFamily(typeof (IThingy));
            family.PluginCount.ShouldEqual(1);
            family.FindPlugin(typeof (BigThingy)).ShouldNotBeNull();

            graph.AddType(typeof (IThingy), typeof (BigThingy));

            family.PluginCount.ShouldEqual(1);
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

            graph.FindFamily(typeof (IWidget)).DefaultInstanceKey = "Blue";
            graph.CreateFamily(typeof (WidgetMaker));

            graph.Seal();


            foreach (PluginFamily family in graph.PluginFamilies)
            {
                Console.WriteLine(family.PluginType.AssemblyQualifiedName);
            }

            Assert.AreEqual(5, graph.FamilyCount);
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

            PluginFamily family = graph.FindFamily(typeof (Rule));
            Assert.IsNotNull(family);
            Assert.AreEqual(5, family.PluginCount, "There are 5 Rule classes in the two assemblies");
        }


        [Test]
        public void PicksUpManuallyAddedPlugin()
        {
            var graph = new PluginGraph();

            graph.Scan(x => { x.Assembly("StructureMap.Testing.Widget"); });

            graph.FindFamily(typeof (IWidget)).DefaultInstanceKey = "Blue";


            PluginFamily family = graph.FindFamily(typeof (IWidget));
            family.AddPlugin(typeof (NotPluggableWidget), "NotPluggable");

            graph.Seal();


            Assert.IsNotNull(family);

            Assert.AreEqual(
                5,
                family.PluginCount,
                "5 different IWidget classes are marked as Pluggable, + the manual add");
        }

        [Test]
        public void PutsRightNumberOfPluginsIntoAFamily()
        {
            var graph = new PluginGraph();

            graph.Scan(x => { x.Assembly("StructureMap.Testing.Widget"); });

            graph.FindFamily(typeof (IWidget)).DefaultInstanceKey = "Blue";
            graph.Seal();

            PluginFamily family = graph.FindFamily(typeof (IWidget));
            Assert.IsNotNull(family);

            Assert.AreEqual("Blue", family.DefaultInstanceKey);

            Assert.AreEqual(4, family.PluginCount, "3 different IWidget classes are marked as Pluggable");
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