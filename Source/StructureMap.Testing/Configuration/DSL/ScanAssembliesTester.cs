using System.Reflection;
using NUnit.Framework;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ScanAssembliesTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            ObjectFactory.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            StructureMapConfiguration.ResetAll();
            ObjectFactory.Reset();
        }

        #endregion

        [Test]
        public void Combination1()
        {
            StructureMapConfiguration.ScanAssemblies()
                .IncludeAssemblyContainingType<IGateway>()
                .IncludeTheCallingAssembly();
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();

            Assembly assembly = typeof (IGateway).Assembly;
            Assert.IsTrue(graph.Assemblies.Contains(assembly.GetName().Name));

            Assert.IsTrue(graph.Assemblies.Contains(Assembly.GetExecutingAssembly().GetName().Name));
        }


        [Test]
        public void Combination2()
        {
            StructureMapConfiguration.ScanAssemblies()
                .IncludeTheCallingAssembly()
                .IncludeAssemblyContainingType<IGateway>();
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();

            Assembly assembly = typeof (IGateway).Assembly;
            Assert.IsTrue(graph.Assemblies.Contains(assembly.GetName().Name));

            Assert.IsTrue(graph.Assemblies.Contains(Assembly.GetExecutingAssembly().GetName().Name));
        }

        [Test]
        public void ScanAssemblyByName()
        {
            ScanAssembliesExpression expression = StructureMapConfiguration.ScanAssemblies()
                .IncludeAssembly(typeof (IGateway).Assembly.FullName);
            Assert.IsNotNull(expression);

            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();

            Assembly assembly = typeof (IGateway).Assembly;
            Assert.IsTrue(graph.Assemblies.Contains(assembly.GetName().Name));
        }

        [Test]
        public void ScanAssemblyContainingType()
        {
            StructureMapConfiguration.ScanAssemblies().IncludeAssemblyContainingType<IGateway>();
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();

            Assembly assembly = typeof (IGateway).Assembly;
            Assert.IsTrue(graph.Assemblies.Contains(assembly.GetName().Name));
        }

        [Test]
        public void ScanCallingAssembly()
        {
            StructureMapConfiguration.ScanAssemblies().IncludeTheCallingAssembly();
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();

            Assert.IsTrue(graph.Assemblies.Contains(Assembly.GetExecutingAssembly().GetName().Name));
        }
    }
}