using System;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class PluginFamilyTester
    {
        public class TheGateway : IGateway
        {
            #region IGateway Members

            public string WhoAmI
            {
                get { throw new NotImplementedException(); }
            }

            public void DoSomething()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [Test]
        public void AddAPluggedType()
        {
            PluginFamily family = new PluginFamily(typeof (IWidget));
            family.DefaultInstanceKey = "DefaultKey";
            family.AddPlugin(typeof (NotPluggableWidget), "NotPlugged");

            Assert.AreEqual(1, family.PluginCount, "Plugin Count");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void AddAWrongType()
        {
            PluginFamily family = new PluginFamily(typeof (IWidget));
            family.DefaultInstanceKey = "DefaultKey";
            family.AddPlugin(typeof (Rule), "Rule");
        }


        [Test]
        public void Analyze_a_type_for_a_plugin_that_does_not_match()
        {
            PluginFamily family = new PluginFamily(typeof (ISingletonRepository));
            family.AnalyzeTypeForPlugin(typeof (RandomClass));

            Assert.AreEqual(0, family.PluginCount);
        }

        [Test]
        public void
            Analyze_a_type_for_a_plugin_that_is_not_explicitly_marked_when_the_family_is_not_considering_implicit_plugins
            ()
        {
            PluginFamily family = new PluginFamily(typeof (ISingletonRepository));
            family.SearchForImplicitPlugins = false;

            family.AnalyzeTypeForPlugin(typeof (SingletonRepositoryWithoutPluginAttribute));

            Assert.AreEqual(0, family.PluginCount);
        }

        [Test]
        public void Analyze_a_type_for_implicit_plugins()
        {
            PluginFamily family = new PluginFamily(typeof (ISingletonRepository));
            family.SearchForImplicitPlugins = true;

            family.AnalyzeTypeForPlugin(typeof (SingletonRepositoryWithoutPluginAttribute));

            Assert.AreEqual(1, family.PluginCount);
            Assert.IsTrue(family.HasPlugin(typeof (SingletonRepositoryWithoutPluginAttribute)));
        }

        [Test]
        public void Create_PluginFamily_for_concrete_type_that_can_be_autofilled_and_create_default_instance()
        {
            PluginFamily family = new PluginFamily(GetType());
            family.Seal();

            Assert.AreEqual(Plugin.DEFAULT, family.DefaultInstanceKey);
            Assert.AreEqual(1, family.PluginCount);
            Assert.AreEqual(1, family.InstanceCount);
            IConfiguredInstance instance = (IConfiguredInstance) family.FirstInstance();
            Assert.AreEqual(Plugin.DEFAULT, instance.Name);
            Assert.AreEqual(GetType(), instance.PluggedType);
        }

        [Test]
        public void Do_not_add_Plugin_that_already_exists()
        {
            PluginFamily family = new PluginFamily(typeof (ISingletonRepository));
            family.AddPlugin(typeof (SingletonRepositoryWithAttribute));


            family.AnalyzeTypeForPlugin(typeof (SingletonRepositoryWithAttribute));
            family.AnalyzeTypeForPlugin(typeof (SingletonRepositoryWithAttribute));
            family.AnalyzeTypeForPlugin(typeof (SingletonRepositoryWithAttribute));
            family.AnalyzeTypeForPlugin(typeof (SingletonRepositoryWithAttribute));

            Assert.AreEqual(1, family.PluginCount);
        }

        [Test]
        public void FillDefault_happy_path()
        {
            PluginFamily family = new PluginFamily(typeof (IWidget));
            family.Parent = new PluginGraph();
            family.AddInstance(new ConfiguredInstance(typeof(ColorWidget)).WithName("Default"));
            family.DefaultInstanceKey = "Default";


            family.FillDefault(new Profile("theProfile"));

            family.Parent.Log.AssertHasNoError(210);
        }

        [Test]
        public void FillDefault_sad_path_when_the_default_instance_key_does_not_exist_throws_210()
        {
            PluginFamily family = new PluginFamily(typeof (IWidget));
            family.Parent = new PluginGraph();

            family.DefaultInstanceKey = "something that cannot be found";
            family.FillDefault(new Profile("theProfile"));

            family.Parent.Log.AssertHasError(210);
        }

        [Test]
        public void If_PluginFamily_only_has_one_instance_make_that_the_default()
        {
            PluginFamily family = new PluginFamily(typeof (IGateway));
            string theInstanceKey = "the default";
            family.AddInstance(new ConfiguredInstance(typeof(TheGateway)).WithName(theInstanceKey));

            family.Seal();

            Assert.AreEqual(theInstanceKey, family.DefaultInstanceKey);
        }

        [Test]
        public void If_PluginType_is_concrete_automatically_add_a_plugin_called_default()
        {
            PluginFamily family = new PluginFamily(GetType());
            family.PluginCount.ShouldEqual(1);
            family.FindPlugin(Plugin.DEFAULT).PluggedType.ShouldEqual(GetType());
        }

        [Test]
        public void ImplicitPluginFamilyCreatesASingletonInterceptorWhenIsSingletonIsTrue()
        {
            PluginFamily family = new PluginFamily(typeof (ISingletonRepository));
            Assert.IsInstanceOfType(typeof (SingletonPolicy), family.Policy);

            PluginFamily family2 = new PluginFamily(typeof (IDevice));
            Assert.IsInstanceOfType(typeof (BuildPolicy), family2.Policy);
        }

        [Test]
        public void Log_104_if_instance_cannot_be_added_into_PluginFamily()
        {
            TestUtility.AssertErrorIsLogged(104, graph =>
            {
                PluginFamily family = new PluginFamily(typeof (IGateway), graph);
                ConfiguredInstance instance = new ConfiguredInstance(typeof (Rule));

                Assert.IsFalse(TypeRules.CanBeCast(typeof (IGateway), typeof (Rule)));

                family.AddInstance(instance);

                family.Seal();
            });
        }

        [Test]
        public void PluginFamily_adds_an_explicitly_marked_Plugin_when_only_looking_for_Explicit_plugins()
        {
            PluginFamily family = new PluginFamily(typeof (ISingletonRepository));
            family.SearchForImplicitPlugins = false;
            family.AnalyzeTypeForPlugin(typeof (SingletonRepositoryWithAttribute));

            Assert.AreEqual(1, family.PluginCount);
            Assert.IsTrue(family.HasPlugin(typeof (SingletonRepositoryWithAttribute)));
        }

        [Test]
        public void PluginFamily_only_looks_for_explicit_plugins_by_default()
        {
            PluginFamily family = new PluginFamily(typeof (ISingletonRepository));
            Assert.IsFalse(family.SearchForImplicitPlugins);
        }

        [Test]
        public void PluginFamilyImplicitlyConfiguredAsASingletonBehavesAsASingleton()
        {
            PluginGraph pluginGraph = new PluginGraph();
            pluginGraph.Assemblies.Add(Assembly.GetExecutingAssembly());
            pluginGraph.Seal();

            Container manager = new Container(pluginGraph);

            ISingletonRepository repository1 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));
            ISingletonRepository repository2 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));
            ISingletonRepository repository3 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));
            ISingletonRepository repository4 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));
            ISingletonRepository repository5 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));

            Assert.AreSame(repository1, repository2);
            Assert.AreSame(repository1, repository3);
            Assert.AreSame(repository1, repository4);
            Assert.AreSame(repository1, repository5);
        }

        [Test]
        public void SetScopeToHttpContext()
        {
            PluginFamily family = new PluginFamily(typeof (IServiceProvider));
            Assert.IsInstanceOfType(typeof (BuildPolicy), family.Policy);

            family.SetScopeTo(InstanceScope.HttpContext);
            Assert.IsInstanceOfType(typeof (HttpContextBuildPolicy), family.Policy);
        }


        [Test]
        public void SetScopeToHybrid()
        {
            PluginFamily family = new PluginFamily(typeof (IServiceProvider));
            Assert.IsInstanceOfType(typeof (BuildPolicy), family.Policy);

            family.SetScopeTo(InstanceScope.Hybrid);
            Assert.IsInstanceOfType(typeof (HybridBuildPolicy), family.Policy);
        }

        [Test]
        public void SetScopeToSingleton()
        {
            PluginFamily family = new PluginFamily(typeof (IServiceProvider));
            Assert.IsInstanceOfType(typeof (BuildPolicy), family.Policy);

            family.SetScopeTo(InstanceScope.Singleton);
            Assert.IsInstanceOfType(typeof (SingletonPolicy), family.Policy);
        }

        [Test]
        public void SetScopeToThreadLocal()
        {
            PluginFamily family = new PluginFamily(typeof (IServiceProvider));
            Assert.IsInstanceOfType(typeof (BuildPolicy), family.Policy);

            family.SetScopeTo(InstanceScope.ThreadLocal);
            Assert.IsInstanceOfType(typeof (ThreadLocalStoragePolicy), family.Policy);
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
    public class SingletonRepositoryWithAttribute : ISingletonRepository
    {
        private readonly Guid _id = Guid.NewGuid();

        public Guid Id
        {
            get { return _id; }
        }
    }

    public class SingletonRepositoryWithoutPluginAttribute : ISingletonRepository
    {
    }

    public class RandomClass
    {
    }

    [PluginFamily(IsSingleton = false)]
    public interface IDevice
    {
    }
}