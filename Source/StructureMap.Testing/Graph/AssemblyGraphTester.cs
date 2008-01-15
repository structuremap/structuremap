using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class AssemblyGraphTester
    {
        [Test]
        public void CanFindAssembly()
        {
            AssemblyGraph graph = new AssemblyGraph("StructureMap.Testing.Widget");
            Assert.AreEqual("StructureMap.Testing.Widget", graph.AssemblyName);
        }


        [Test,
         ExpectedException(typeof (StructureMapException),
             "StructureMap Exception Code:  101\nAssembly DoesNotExist referenced by an <Assembly> node in StructureMap.config cannot be loaded into the current AppDomain"
             )]
        public void CannotFindAssembly()
        {
            AssemblyGraph graph = new AssemblyGraph("DoesNotExist");
        }


        [Test]
        public void CanFindFamilies()
        {
            AssemblyGraph graph = new AssemblyGraph("StructureMap.Testing.Widget");
            PluginFamily[] families = graph.FindPluginFamilies();

            Assert.IsNotNull(families);
            Assert.AreEqual(4, families.Length);
            Assert.AreEqual(DefinitionSource.Implicit, families[0].DefinitionSource);
        }

        [Test]
        public void CanFindPlugins()
        {
            AssemblyGraph graph = new AssemblyGraph("StructureMap.Testing.Widget");
            PluginFamily family = new PluginFamily(typeof(IWidget));
            Plugin[] plugins = family.FindPlugins(graph);
            Assert.IsNotNull(plugins);
            Assert.AreEqual(4, plugins.Length);
            Assert.AreEqual(DefinitionSource.Implicit, plugins[0].DefinitionSource);
        }

        [Test]
        public void GetAllAssembliesAtPath()
        {
            string[] assemblies = AssemblyGraph.GetAllAssembliesAtPath(".");
            ArrayList list = new ArrayList(assemblies);

            Assert.IsTrue(list.Contains("StructureMap.Testing"));
            Assert.IsTrue(list.Contains("StructureMap"));
            Assert.IsTrue(list.Contains("StructureMap.Testing.Widget"));
            Assert.IsTrue(list.Contains("StructureMap.Testing.Widget2"));
            Assert.IsTrue(list.Contains("StructureMap.Testing.Widget3"));
            Assert.IsTrue(list.Contains("StructureMap.Testing.Widget4"));
        }

        [Test]
        public void FindTypeByFullNameSuccess()
        {
            AssemblyGraph assemblyGraph = new AssemblyGraph("StructureMap.Testing.Widget");
            Type type = typeof (IWidget);

            Type actualType = assemblyGraph.FindTypeByFullName(type.FullName);

            Assert.AreEqual(type, actualType);
        }

        [Test]
        public void FindTypeByFullNameReturnsNullIfTypeNotFound()
        {
            AssemblyGraph assemblyGraph = new AssemblyGraph("StructureMap.Testing.Widget");
            Assert.IsNull(assemblyGraph.FindTypeByFullName("something that does not exist"));
        }


    }


}