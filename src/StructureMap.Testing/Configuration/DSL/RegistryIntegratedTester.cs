using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget5;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class RegistryIntegratedTester
    {
        [Test]
        public void AutomaticallyFindRegistryFromAssembly()
        {
            ObjectFactory.Initialize(x =>
            {
                x.Scan(s =>
                {
                    s.AssemblyContainingType<RedGreenRegistry>();
                    s.LookForRegistries();
                });
            });

            var colors = new List<string>();
            foreach (IWidget widget in ObjectFactory.GetAllInstances<IWidget>())
            {
                if (!(widget is ColorWidget))
                {
                    continue;
                }

                var color = (ColorWidget) widget;
                colors.Add(color.Color);
            }

            Assert.Contains("Red", colors);
            Assert.Contains("Green", colors);
            Assert.Contains("Yellow", colors);
            Assert.Contains("Blue", colors);
            Assert.Contains("Brown", colors);
            Assert.Contains("Black", colors);
        }


        [Test]
        public void FindRegistriesWithinPluginGraphSeal()
        {
            var scanner = new AssemblyScanner();
            scanner.AssemblyContainingType(typeof (RedGreenRegistry));
            scanner.LookForRegistries();

            var graph = scanner.ToPluginGraph();

            var colors = new List<string>();
            var family = graph.Families[typeof (IWidget)];

            family.Instances.Each(instance => colors.Add(instance.Name));

            Assert.Contains("Red", colors);
            Assert.Contains("Green", colors);
            Assert.Contains("Yellow", colors);
            Assert.Contains("Blue", colors);
            Assert.Contains("Brown", colors);
            Assert.Contains("Black", colors);
        }
    }
}