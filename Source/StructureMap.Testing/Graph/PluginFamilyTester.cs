using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using StructureMap.TypeRules;
using Rule=StructureMap.Testing.Widget.Rule;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class PluginFamilyTester
    {
        public class TheGateway : IGateway
        {
            #region IGateway Members

            public string WhoAmI { get { throw new NotImplementedException(); } }

            public void DoSomething()
            {
                throw new NotImplementedException();
            }

            #endregion
        }


        [Test]
        public void add_plugins_at_seal_from_the_list_of_types()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            family.AddType(typeof (DataTable));

            // DataView, DataSet, and DataTable are all IServiceProvider implementations, and should get added
            // to the PluginFamily
            var pluggedTypes = new List<Type>
            {
                typeof (DataView),
                typeof (DataSet),
                typeof (DataTable),
                GetType()
            };

            family.AddTypes(pluggedTypes);

            family.PluginCount.ShouldEqual(3);

            family.FindPlugin(typeof (DataView)).ShouldNotBeNull();
            family.FindPlugin(typeof (DataTable)).ShouldNotBeNull();
            family.FindPlugin(typeof (DataSet)).ShouldNotBeNull();
        }

        [Test]
        public void add_type_by_name()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            family.AddType(typeof (DataTable), "table");

            family.PluginCount.ShouldEqual(1);

            family.FindPlugin("table").ShouldNotBeNull();
        }

        [Test]
        public void add_type_does_not_add_if_the_concrete_type_can_not_be_cast_to_plugintype()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            family.AddType(GetType());

            family.PluginCount.ShouldEqual(0);
        }

        [Test]
        public void add_type_works_if_the_concrete_type_can_be_cast_to_plugintype()
        {
            var family = new PluginFamily(typeof (IServiceProvider));

            family.AddType(typeof (DataTable));
            family.PluginCount.ShouldEqual(1);

            family.AddType(typeof (DataTable));
            family.PluginCount.ShouldEqual(1);
        }

        [Test]
        public void AddAPluggedType()
        {
            var family = new PluginFamily(typeof (IWidget));
            family.DefaultInstanceKey = "DefaultKey";
            family.AddPlugin(typeof (NotPluggableWidget), "NotPlugged");

            Assert.AreEqual(1, family.PluginCount, "Plugin Count");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void AddAWrongType()
        {
            var family = new PluginFamily(typeof (IWidget));
            family.DefaultInstanceKey = "DefaultKey";
            family.AddPlugin(typeof (Rule), "Rule");
        }

        [Test]
        public void can_get_configuration_object_from_PluginFamily()
        {
            var family = new PluginFamily(typeof (IWidget));

            SmartInstance<ColorWidget> instance1 = new SmartInstance<ColorWidget>().WithCtorArg("color").EqualTo("Red");
            SmartInstance<ColorWidget> instance2 = new SmartInstance<ColorWidget>().WithCtorArg("color").EqualTo("Blue");

            family.AddInstance(instance1);
            family.AddInstance(instance2);

            family.DefaultInstanceKey = instance1.Name;

            PluginTypeConfiguration configuration = family.GetConfiguration();

            configuration.Default.ShouldBeTheSameAs(instance1);
            configuration.PluginType.ShouldEqual(typeof (IWidget));
            configuration.Lifecycle.ShouldBeTheSameAs(family.Lifecycle);

            configuration.Instances.Count().ShouldEqual(2);
        }

        [Test]
        public void FillDefault_happy_path()
        {
            var family = new PluginFamily(typeof (IWidget));
            family.Parent = new PluginGraph();
            family.AddInstance(new ConfiguredInstance(typeof (ColorWidget)).WithName("Default"));
            family.DefaultInstanceKey = "Default";


            family.FillDefault(new Profile("theProfile"));

            family.Parent.Log.AssertHasNoError(210);
        }

        [Test]
        public void FillDefault_sad_path_when_the_default_instance_key_does_not_exist_throws_210()
        {
            var family = new PluginFamily(typeof (IWidget));
            family.Parent = new PluginGraph();

            family.DefaultInstanceKey = "something that cannot be found";
            family.FillDefault(new Profile("theProfile"));

            family.Parent.Log.AssertHasError(210);
        }

        [Test]
        public void If_PluginFamily_only_has_one_instance_make_that_the_default()
        {
            var family = new PluginFamily(typeof (IGateway));
            string theInstanceKey = "the default";
            family.AddInstance(new ConfiguredInstance(typeof (TheGateway)).WithName(theInstanceKey));

            family.Seal();

            Assert.AreEqual(theInstanceKey, family.DefaultInstanceKey);
        }

        [Test]
        public void ImplicitPluginFamilyCreatesASingletonInterceptorWhenIsSingletonIsTrue()
        {
            var family = new PluginFamily(typeof (ISingletonRepository));
            Assert.IsInstanceOfType(typeof (SingletonLifecycle), family.Lifecycle);

            var family2 = new PluginFamily(typeof (IDevice));
            family2.Lifecycle.ShouldBeNull();
        }

        [Test]
        public void Log_104_if_instance_cannot_be_added_into_PluginFamily()
        {
            TestUtility.AssertErrorIsLogged(104, graph =>
            {
                var family = new PluginFamily(typeof (IGateway), graph);
                var instance = new ConfiguredInstance(typeof (ColorRule));

                Assert.IsFalse(typeof (Rule).CanBeCastTo(typeof (IGateway)));

                family.AddInstance(instance);

                family.Seal();
            });
        }

        [Test]
        public void PluginFamilyImplicitlyConfiguredAsASingletonBehavesAsASingleton()
        {
            var pluginGraph = new PluginGraph();
            pluginGraph.Scan(x => { x.Assembly(Assembly.GetExecutingAssembly()); });

            pluginGraph.Seal();

            var manager = new Container(pluginGraph);

            var repository1 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));
            var repository2 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));
            var repository3 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));
            var repository4 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));
            var repository5 =
                (ISingletonRepository) manager.GetInstance(typeof (ISingletonRepository));

            Assert.AreSame(repository1, repository2);
            Assert.AreSame(repository1, repository3);
            Assert.AreSame(repository1, repository4);
            Assert.AreSame(repository1, repository5);
        }

        [Test]
        public void set_the_scope_to_session()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            family.SetScopeTo(InstanceScope.HttpSession);

            family.Lifecycle.ShouldBeOfType<HttpSessionLifecycle>();
        }

        [Test]
        public void set_the_scope_to_session_hybrid()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            family.SetScopeTo(InstanceScope.HybridHttpSession);

            family.Lifecycle.ShouldBeOfType<HybridSessionLifecycle>();
        }

        [Test]
        public void SetScopeToHttpContext()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            family.Lifecycle.ShouldBeNull();

            family.SetScopeTo(InstanceScope.HttpContext);
            Assert.IsInstanceOfType(typeof (HttpContextLifecycle), family.Lifecycle);
        }


        [Test]
        public void SetScopeToHybrid()
        {
            var family = new PluginFamily(typeof (IServiceProvider));


            family.SetScopeTo(InstanceScope.Hybrid);
            Assert.IsInstanceOfType(typeof (HybridLifecycle), family.Lifecycle);
        }

        [Test]
        public void SetScopeToSingleton()
        {
            var family = new PluginFamily(typeof (IServiceProvider));

            family.SetScopeTo(InstanceScope.Singleton);
            Assert.IsInstanceOfType(typeof (SingletonLifecycle), family.Lifecycle);
        }

        [Test]
        public void SetScopeToThreadLocal()
        {
            var family = new PluginFamily(typeof (IServiceProvider));

            family.SetScopeTo(InstanceScope.ThreadLocal);
            Assert.IsInstanceOfType(typeof (ThreadLocalStorageLifecycle), family.Lifecycle);
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

        public Guid Id { get { return _id; } }
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