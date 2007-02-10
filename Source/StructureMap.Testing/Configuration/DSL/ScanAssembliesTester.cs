using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ScanAssembliesTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void ScanCallingAssembly()
        {
            PluginGraph graph = new PluginGraph();


            using (Registry registry = new Registry(graph))
            {
                registry.ScanAssemblies().IncludeTheCallingAssembly();
            }

            AssemblyGraph assembly = new AssemblyGraph(Assembly.GetExecutingAssembly());
            Assert.IsTrue(graph.Assemblies.Contains(assembly.AssemblyName));
        }

        [Test]
        public void ScanAssemblyContainingType()
        {
            PluginGraph graph = new PluginGraph();


            using (Registry registry = new Registry(graph))
            {
                registry.ScanAssemblies()
                    .IncludeAssemblyContainingType<IGateway>();
            }

            AssemblyGraph assembly = AssemblyGraph.ContainingType<IGateway>();
            Assert.IsTrue(graph.Assemblies.Contains(assembly.AssemblyName));
        }

        [Test]
        public void Combination1()
        {
            PluginGraph graph = new PluginGraph();


            using (Registry registry = new Registry(graph))
            {
                registry.ScanAssemblies()
                    .IncludeAssemblyContainingType<IGateway>()
                    .IncludeTheCallingAssembly();
            }

            AssemblyGraph assembly = AssemblyGraph.ContainingType<IGateway>();
            Assert.IsTrue(graph.Assemblies.Contains(assembly.AssemblyName));

            assembly = new AssemblyGraph(Assembly.GetExecutingAssembly());
            Assert.IsTrue(graph.Assemblies.Contains(assembly.AssemblyName));
        }


        [Test]
        public void Combination2()
        {
            PluginGraph graph = new PluginGraph();


            using (Registry registry = new Registry(graph))
            {
                registry.ScanAssemblies()
                    .IncludeTheCallingAssembly()
                    .IncludeAssemblyContainingType<IGateway>();

            }

            AssemblyGraph assembly = AssemblyGraph.ContainingType<IGateway>();
            Assert.IsTrue(graph.Assemblies.Contains(assembly.AssemblyName));

            assembly = new AssemblyGraph(Assembly.GetExecutingAssembly());
            Assert.IsTrue(graph.Assemblies.Contains(assembly.AssemblyName));
        }

    }
}
