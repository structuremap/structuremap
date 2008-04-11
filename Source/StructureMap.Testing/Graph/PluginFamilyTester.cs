using System;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Source;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class PluginFamilyTester
    {
        [Test]
        public void AddAPluggedType()
        {
            PluginFamily family = new PluginFamily(typeof (IWidget), "DefaultKey");
            family.Plugins.Add(typeof (NotPluggableWidget), "NotPlugged");

            Assert.AreEqual(1, family.Plugins.Count, "Plugin Count");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void AddAWrongType()
        {
            PluginFamily family = new PluginFamily(typeof (IWidget), "DefaultKey");
            family.Plugins.Add(typeof (Rule), "Rule");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void CreateExplicitWithNonexistentAssembly()
        {
            TypePath path = new TypePath("NonexistentAssembly", "NonexistentAssembly.Class1");

            PluginFamily family = new PluginFamily(path, "");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void CreateExplicitWithNonexistentClass()
        {
            TypePath path =
                new TypePath("StructureMap.Testing.Widget", "StructureMap.Testing.Widget.NonExistentInterface");

            PluginFamily family = new PluginFamily(path, "");
        }

        [Test]
        public void GetPlugins()
        {
            PluginFamily family = new PluginFamily(typeof (IWidget), "DefaultKey");

            AssemblyGraph graph = new AssemblyGraph("StructureMap.Testing.Widget");
            family.FindPlugins(graph);

            Assert.AreEqual(4, family.Plugins.Count, "Plugin Count");
            foreach (Plugin plugin in family.Plugins)
            {
                Assert.IsNotNull(plugin);
            }
        }

        [Test]
        public void HasANulloInterceptorUponConstruction()
        {
            PluginFamily family = new PluginFamily(typeof (IWidget));
            Assert.IsInstanceOfType(typeof (NulloInterceptor), family.InstanceInterceptor);
        }

        [Test]
        public void ImplicitPluginFamilyCreatesASingletonInterceptorWhenIsSingletonIsTrue()
        {
            PluginFamily family = new PluginFamily(typeof (ISingletonRepository));
            Assert.AreEqual(1, family.InterceptionChain.Count);

            InstanceFactoryInterceptor interceptor = family.InterceptionChain[0];
            Assert.IsTrue(interceptor is SingletonInterceptor);

            PluginFamily family2 = new PluginFamily(typeof (IDevice));
            Assert.AreEqual(0, family2.InterceptionChain.Count);
        }

        [Test]
        public void PluginFamilyImplicitlyConfiguredAsASingletonBehavesAsASingleton()
        {
            PluginGraph pluginGraph = new PluginGraph();
            pluginGraph.Assemblies.Add(Assembly.GetExecutingAssembly());
            pluginGraph.Seal();

            PluginFamily family = pluginGraph.PluginFamilies[typeof (ISingletonRepository)];
            Assert.AreEqual(1, family.InterceptionChain.Count);
            Assert.IsTrue(family.InterceptionChain[0] is SingletonInterceptor);

            InstanceManager manager = new InstanceManager(pluginGraph);

            ISingletonRepository repository1 =
                (ISingletonRepository) manager.CreateInstance(typeof (ISingletonRepository));
            ISingletonRepository repository2 =
                (ISingletonRepository) manager.CreateInstance(typeof (ISingletonRepository));
            ISingletonRepository repository3 =
                (ISingletonRepository) manager.CreateInstance(typeof (ISingletonRepository));
            ISingletonRepository repository4 =
                (ISingletonRepository) manager.CreateInstance(typeof (ISingletonRepository));
            ISingletonRepository repository5 =
                (ISingletonRepository) manager.CreateInstance(typeof (ISingletonRepository));

            Assert.AreSame(repository1, repository2);
            Assert.AreSame(repository1, repository3);
            Assert.AreSame(repository1, repository4);
            Assert.AreSame(repository1, repository5);
        }
    }

    /// <summary>
    /// Specifying the default instance is "Default" and marking the PluginFamily
    /// as an injected Singleton
    /// </summary>
    [PluginFamily("Default", IsSingleton = true)]
    public interface ISingletonRepository
    {
    }

    [Pluggable("Default")]
    public class SingletonRepository : ISingletonRepository
    {
        private Guid _id = Guid.NewGuid();

        public Guid Id
        {
            get { return _id; }
        }
    }

    [PluginFamily(IsSingleton = false)]
    public interface IDevice
    {
    }
}