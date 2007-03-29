using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget5;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class RegistryIntegratedTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            StructureMapConfiguration.ResetAll();
            ObjectFactory.Reset();
        }


        [Test]
        public void FindRegistries()
        {
            AssemblyGraph assembly = new AssemblyGraph("StructureMap.Testing.Widget5");
            List<Registry> list = assembly.FindRegistries();

            Assert.AreEqual(3, list.Count);
            Assert.Contains(new RedGreenRegistry(), list);
            Assert.Contains(new YellowBlueRegistry(), list);
            Assert.Contains(new BrownBlackRegistry(), list);
        }

        [Test]
        public void FindRegistriesWithinPluginGraphSeal()
        {
            PluginGraph graph = new PluginGraph();
            graph.Assemblies.Add("StructureMap.Testing.Widget5");
            graph.Seal();

            List<string> colors = new List<string>();
            foreach (InstanceMemento memento in graph.PluginFamilies[typeof (IWidget)].Source.GetAllMementos())
            {
                colors.Add(memento.InstanceKey);
            }

            Assert.Contains("Red", colors);
            Assert.Contains("Green", colors);
            Assert.Contains("Yellow", colors);
            Assert.Contains("Blue", colors);
            Assert.Contains("Brown", colors);
            Assert.Contains("Black", colors);
        }

        [Test]
        public void AutomaticallyFindRegistryFromAssembly()
        {
            StructureMapConfiguration.ResetAll();
            StructureMapConfiguration.ScanAssemblies().IncludeAssemblyContainingType<RedGreenRegistry>();
            ObjectFactory.Reset();

            List<string> colors = new List<string>();
            foreach (IWidget widget in ObjectFactory.GetAllInstances<IWidget>())
            {
                if (!(widget is ColorWidget))
                {
                    continue;
                }

                ColorWidget color = (ColorWidget) widget;
                colors.Add(color.Color);
            }

            Assert.Contains("Red", colors);
            Assert.Contains("Green", colors);
            Assert.Contains("Yellow", colors);
            Assert.Contains("Blue", colors);
            Assert.Contains("Brown", colors);
            Assert.Contains("Black", colors);
        }
    }
}