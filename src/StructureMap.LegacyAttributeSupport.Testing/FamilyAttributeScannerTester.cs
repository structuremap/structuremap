using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing;
using StructureMap.Testing.Widget5;

namespace StructureMap.LegacyAttributeSupport.Testing
{
    [TestFixture]
    public class FamilyAttributeScannerTester
    {

        [Test]
        public void test_the_family_attribute_scanner()
        {
            var scanner = new FamilyAttributeScanner();
            var graph = new PluginGraph();

            var registry = new Registry();

            scanner.Process(typeof(ITypeThatHasAttributeButIsNotInRegistry), registry);
            registry.ShouldBeOfType<IPluginGraphConfiguration>().Configure(graph);

            graph.Families.Has(typeof(ITypeThatHasAttributeButIsNotInRegistry)).ShouldBeTrue();

            graph = new PluginGraph();
            registry = new Registry();

            scanner.Process(GetType(), registry);
            registry.ShouldBeOfType<IPluginGraphConfiguration>().Configure(graph);

            graph.Families.Has(GetType()).ShouldBeFalse();
        }
    }
}