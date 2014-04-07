using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Samples
{
    [TestFixture]
    public class Lifecycles_Samples
    {
        // SAMPLE: singleton-in-action
        [Test]
        public void singletons()
        {
            var c = new Container(x => {
                x.For<IService>().Use<Service>().Singleton();
            });

            // It's always the same object instance
            c.GetInstance<IService>()
                .ShouldBeTheSameAs(c.GetInstance<IService>())
                .ShouldBeTheSameAs(c.GetInstance<IService>())
                .ShouldBeTheSameAs(c.GetInstance<IService>())
                .ShouldBeTheSameAs(c.GetInstance<IService>())
                .ShouldBeTheSameAs(c.GetInstance<IService>());
        }
        // ENDSAMPLE

        [Test]
        public void singletons_2()
        {
            var c = new Container(x =>
            {
                x.For<IService>().Singleton().Use<Service>();
            });

            // It's always the same object instance
            c.GetInstance<IService>()
                .ShouldBeTheSameAs(c.GetInstance<IService>())
                .ShouldBeTheSameAs(c.GetInstance<IService>())
                .ShouldBeTheSameAs(c.GetInstance<IService>())
                .ShouldBeTheSameAs(c.GetInstance<IService>())
                .ShouldBeTheSameAs(c.GetInstance<IService>());
        }


        // SAMPLE: how-transient-works
        [Test]
        public void Transient()
        {
            var c = new Container(x =>
            {
                x.For<IService>().Use<Service>().Transient();
            });

            // In a normal container, you get a new object
            // instance of the Service class in subsequent
            // requests
            c.GetInstance<IService>()
                .ShouldNotBeTheSameAs(c.GetInstance<IService>())
                .ShouldNotBeTheSameAs(c.GetInstance<IService>());

            // Within a nested container, 'Transient' now 
            // means within the Nested Container.
            // A nested container is effectively one request
            using (var nested = c.GetNestedContainer())
            {
                nested.GetInstance<IService>()
                    .ShouldBeTheSameAs(nested.GetInstance<IService>())
                    .ShouldBeTheSameAs(nested.GetInstance<IService>());
            }
        }
        // ENDSAMPLE

        // SAMPLE: how-always-unique
        [Test]
        public void Always_Unique()
        {
            var c = new Container(x =>
            {
                x.For<IService>().Use<Service>().AlwaysUnique();
            });

            // In a normal container, you get a new object
            // instance of the Service class in subsequent
            // requests
            c.GetInstance<IService>()
                .ShouldNotBeTheSameAs(c.GetInstance<IService>())
                .ShouldNotBeTheSameAs(c.GetInstance<IService>());

            // Within a nested container, 'Transient' now 
            // means within the Nested Container.
            // A nested container is effectively one request
            using (var nested = c.GetNestedContainer())
            {
                nested.GetInstance<IService>()
                    .ShouldNotBeTheSameAs(nested.GetInstance<IService>())
                    .ShouldNotBeTheSameAs(nested.GetInstance<IService>());
            }

            // Even in a single request, 
            var holder = c.GetInstance<ServiceUserHolder>();
            holder.Service.ShouldNotBeTheSameAs(holder.User.Service);
        }
        // ENDSAMPLE

        public class ServiceUser
        {
            public IService Service { get; set; }

            public ServiceUser(IService service)
            {
                Service = service;
            }
        }

        public class ServiceUserHolder
        {
            public IService Service { get; set; }
            public ServiceUser User { get; set; }

            public ServiceUserHolder(IService service, ServiceUser user)
            {
                Service = service;
                User = user;
            }
        }

        [Test]
        public void Transient_within_a_single_request()
        {
            var container = new Container(x => x.For<IService>().Use<Service>());

            var holder = container.GetInstance<ServiceUserHolder>();
            holder.Service.ShouldBeTheSameAs(holder.User.Service);
        }




        // SAMPLE: lifecycle-configuration-at-plugin-type
        public class LifecycleAtPluginTypeRegistry : Registry
        {
            public LifecycleAtPluginTypeRegistry()
            {
                For<IService>().Singleton();

                // This is the default behavior anyway
                For<IGateway>().Transient();


                For<IRule>().AlwaysUnique();

                // ThreadLocal scoping is so rare that SM does
                // not have a convenience method for setting
                // that as the lifecycle
                For<ICache>().LifecycleIs(Lifecycles.ThreadLocal);

                // Use a custom lifecycle
                For<IWeirdThing>().LifecycleIs<MyCustomLifecycle>();
            }
        }
        // ENDSAMPLE

        // SAMPLE: lifecycle-configuration-at-instance
        public class LifecycleByInstanceRegistry : Registry
        {
            public LifecycleByInstanceRegistry()
            {
                For<IService>().Use<Service>().Named("1").Singleton();
                For<IService>().Use<Service>().Named("2").Transient();
                For<IService>().Use<Service>().Named("3").AlwaysUnique();
                For<IService>().Use<Service>().Named("4").LifecycleIs<MyCustomLifecycle>();
            }
        }
        // ENDSAMPLE


        public class MyCustomLifecycle : ILifecycle
        {
            public string Description
            {
                get
                {
                    return "Some explanatory text for diagnostics";
                }
            }

            public void EjectAll(ILifecycleContext context)
            {
                // remove all stored objects from the instance
                // cache and call dispose anything that is IDisposable
            }

            public IObjectCache FindCache(ILifecycleContext context)
            {
                // Using the context, fetch the object cache
                // for the lifecycle
                return new LifecycleObjectCache();
            }
        }

    }

    public interface IService { }
    public class Service : IService{}

    public interface IGateway { }
    public class Gateway : IGateway{}

    public interface IRule { }
    public class Rule : IRule{}

    public interface ICache { }
    public class Cache{}

    public interface IWeirdThing{}
    public class WeirdThing{}
}