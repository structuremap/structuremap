using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_247_ConnectOpenTypesToImplementations_doubling_up_registrations
    {
        [Test]
        public void Scanner_apply_should_only_register_two_instances()
        {
            var scanner = new GenericConnectionScanner(typeof (ISomeServiceOf<>));
            var registry = new Registry();
            var graph = new PluginGraph();

            scanner.Process(typeof (SomeService1), registry);
            scanner.Process(typeof (SomeService2), registry);
            scanner.Apply(graph);

            graph
                .AllInstances(typeof (ISomeServiceOf<string>))
                .Count()
                .ShouldBe(2);
        }

        public interface ISomeServiceOf<T>
        {
        }

        public class SomeService1 : ISomeServiceOf<string>
        {
        }

        public class SomeService2 : ISomeServiceOf<string>
        {
        }
    }
}