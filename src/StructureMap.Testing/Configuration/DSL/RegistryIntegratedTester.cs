using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Pipeline;
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

        [Test]
        public void clear_all_via_strong_typed_expression()
        {
            var registry = new Registry();
            registry.For<ColorRule>().Use(new ColorRule("Red"));
            registry.For<ColorRule>().Add(new ColorRule("Orange"));
            registry.For<ColorRule>().Add(new ColorRule("Yellow"));

            registry.For<ColorRule>().ClearAll().Use(new ColorRule("Blue"));
            registry.For<ColorRule>().Add(new ColorRule("Green"));
            registry.For<ColorRule>().Add(new ColorRule("Purple"));

            var container = new Container(registry);
            container.GetInstance<ColorRule>().Color.ShouldBe("Blue");
            container.GetAllInstances<ColorRule>().OrderBy(x => x.Color).Select(x => x.Color)
                .ShouldHaveTheSameElementsAs("Blue", "Green", "Purple");
        }


        [Test]
        public void clear_all_via_generic_family_expression()
        {
            var registry = new Registry();
            registry.For(typeof(ColorRule)).Use(new ColorRule("Red"));
            registry.For(typeof(ColorRule)).Add(new ColorRule("Orange"));
            registry.For(typeof(ColorRule)).Add(new ColorRule("Yellow"));

            registry.For(typeof(ColorRule)).ClearAll().Use(new ColorRule("Blue"));
            registry.For(typeof(ColorRule)).Add(new ColorRule("Green"));
            registry.For(typeof(ColorRule)).Add(new ColorRule("Purple"));

            var container = new Container(registry);
            container.GetInstance<ColorRule>().Color.ShouldBe("Blue");
            container.GetAllInstances<ColorRule>().OrderBy(x => x.Color).Select(x => x.Color)
                .ShouldHaveTheSameElementsAs("Blue", "Green", "Purple");
        }

        [Test]
        public void configure_via_strong_typed_expression()
        {
            var registry = new Registry();
            registry.For<ColorRule>().Configure(x => x.SetDefault(new ObjectInstance(new ColorRule("Blue"))));

            new Container(registry).GetInstance<ColorRule>()
                .Color.ShouldBe("Blue");
        }

        [Test]
        public void configure_via_generic_type_expression()
        {
            var registry = new Registry();
            registry.For(typeof(ColorRule)).Configure(x => x.SetDefault(new ObjectInstance(new ColorRule("Blue"))));

            new Container(registry).GetInstance<ColorRule>()
                .Color.ShouldBe("Blue");
        }
    }
}