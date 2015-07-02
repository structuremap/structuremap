using System.Collections.Generic;
using NUnit.Framework;
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
            var container = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.AssemblyContainingType<RedGreenRegistry>();
                    s.LookForRegistries();
                });
            });

            var colors = new List<string>();
            foreach (var widget in container.GetAllInstances<IWidget>())
            {
                if (!(widget is ColorWidget))
                {
                    continue;
                }

                var color = (ColorWidget) widget;
                colors.Add(color.Color);
            }

            colors.Sort();
            colors.ShouldHaveTheSameElementsAs("Black", "Blue", "Brown", "Green", "Red", "Yellow");
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

            colors.Sort();
            colors.ShouldHaveTheSameElementsAs("Black", "Blue", "Brown", "Green", "Red", "Yellow");
        }
    }
}