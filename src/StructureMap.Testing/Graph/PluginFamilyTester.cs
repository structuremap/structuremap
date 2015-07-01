using System;
using System.Data;
using System.Linq;
using NUnit.Framework;
using Shouldly;
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

        public class DataTableProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }
        }


        [Test]
        public void throw_exception_if_instance_is_not_compatible_with_PluginType_in_AddInstance()
        {
            var family = new PluginFamily(typeof(IGateway));
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                family.AddInstance(new SmartInstance<ColorRule>());
            });
        }

        [Test]
        public void If_PluginFamily_only_has_one_instance_make_that_the_default()
        {
            var family = new PluginFamily(typeof (IGateway));
            var theInstanceKey = "the default";
            var instance = new ConfiguredInstance(typeof (TheGateway)).Named(theInstanceKey);
            family.AddInstance(instance);

            family.GetDefaultInstance().ShouldBeTheSameAs(instance);
        }


        public class LoggingFamilyAttribute : FamilyAttribute
        {
            public static bool Called { get; set; }

            public override void Alter(PluginFamily family)
            {
                Called = true;
            }
        }

        [LoggingFamily]
        public class FamilyGateway
        {
        }

        [LoggingFamily]
        public interface IFamilyInterface
        {
        }

        [Test]
        public void PluginFamily_uses_family_attribute_when_present_on_class()
        {
            LoggingFamilyAttribute.Called = false;

            var testFamily = typeof (FamilyGateway);

            var family = new PluginFamily(testFamily);

            LoggingFamilyAttribute.Called.ShouldBeTrue();
        }

        [Test]
        public void PluginFamily_uses_family_attribute_when_present_on_interface()
        {
            LoggingFamilyAttribute.Called = false;

            var testFamily = typeof (IFamilyInterface);

            var family = new PluginFamily(testFamily);
            LoggingFamilyAttribute.Called.ShouldBeTrue();
        }


        [Test]
        public void set_the_lifecycle_to_Singleton()
        {
            var family = new PluginFamily(typeof (IServiceProvider));

            family.SetLifecycleTo(Lifecycles.Singleton);
            family.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }

        [Test]
        public void set_the_lifecycle_to_ThreadLocal()
        {
            var family = new PluginFamily(typeof (IServiceProvider));

            family.SetLifecycleTo(Lifecycles.ThreadLocal);
            family.Lifecycle.ShouldBeOfType<ThreadLocalStorageLifecycle>();
        }

        [Test]
        public void add_instance_fills_and_is_idempotent()
        {
            var instance = new ConstructorInstance(GetType());
            var family = new PluginFamily(GetType());

            family.AddInstance(instance);
            family.SetDefault(instance);
            family.AddInstance(instance);
            family.SetDefault(instance);

            family.Instances.Single().ShouldBeTheSameAs(instance);
        }

        [Test]
        public void add_type_by_name()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            family.AddType(typeof (DataTableProvider), "table");

            family.Instances.Single()
                .ShouldBeOfType<ConstructorInstance>()
                .PluggedType.ShouldBe(typeof (DataTableProvider));
        }

        [Test]
        public void add_type_does_not_add_if_the_concrete_type_can_not_be_cast_to_plugintype()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            family.AddType(GetType());

            family.Instances.Any().ShouldBeFalse();
        }

        [Test]
        public void add_type_works_if_the_concrete_type_can_be_cast_to_plugintype()
        {
            var family = new PluginFamily(typeof (IServiceProvider));

            family.AddType(typeof (DataTableProvider));
            family.Instances.Single()
                .ShouldBeOfType<ConstructorInstance>()
                .PluggedType.ShouldBe(typeof (DataTableProvider));


            family.AddType(typeof (DataTable));
            family.Instances.Count().ShouldBe(1);
        }

        [Test]
        public void remove_all_clears_the_defaul_and_removes_all_plugins_instances()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            var instance = new SmartInstance<DataSet>();

            family.Fallback = new SmartInstance<DataSet>();
            family.SetDefault(instance);

            family.AddInstance(new SmartInstance<FakeServiceProvider>());
            family.AddType(typeof (DataSet));

            family.RemoveAll();

            family.GetDefaultInstance().ShouldBeNull();

            family.Instances.Count().ShouldBe(0);
        }

        public class FakeServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void remove_all_clears_the_default()
        {
            var instance = new ConstructorInstance(GetType());
            var family = new PluginFamily(GetType());

            family.SetDefault(instance);
            family.SetDefault(new ConstructorInstance(GetType()));
            family.SetDefault(new ConstructorInstance(GetType()));

            family.RemoveAll();

            family.Instances.Any().ShouldBeFalse();
            family.GetDefaultInstance().ShouldNotBeTheSameAs(instance);
        }

        [Test]
        public void removing_the_default_will_change_the_default()
        {
            var instance = new ConstructorInstance(GetType());
            var family = new PluginFamily(GetType());

            family.SetDefault(instance);

            family.RemoveInstance(instance);

            family.Instances.Any().ShouldBeFalse();
            family.GetDefaultInstance().ShouldNotBeTheSameAs(instance);
        }

        [Test]
        public void set_default()
        {
            var family = new PluginFamily(typeof (IServiceProvider));
            var instance = new SmartInstance<DataSet>();

            family.SetDefault(instance);

            family.GetDefaultInstance().ShouldBeTheSameAs(instance);
        }
    }

    /// <summary>
    ///     Specifying the default instance is "Default" and marking the PluginFamily
    ///     as an injected Singleton
    /// </summary>
    //[PluginFamily("Default", IsSingleton = true)]
    public interface ISingletonRepository
    {
    }

    //[Pluggable("Default")]
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

    //[PluginFamily(IsSingleton = false)]
    public interface IDevice
    {
    }

    public class GenericType<T>
    {
    }
}