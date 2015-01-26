using System;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Compliance
{
    [TestFixture]
    public class Lifecycle_Scoping_Compliance
    {
        [Test]
        public void should_be_a_singleton_because_of_Cache_suffix()
        {
            var container = ContainerFacilitySource.New(x => {
                // Any concrete class suffixed with "Cache" is supposed to be a 
                // singleton
                x.Register(typeof (IService), ObjectDef.ForType<SingletonCache>());
            });

            // Use this static method to know whether or not a class
            // should be scoped as a singleton or by Http request
            ServiceRegistry.ShouldBeSingleton(typeof (SingletonCache))
                           .ShouldBeTrue();

            container.Get<IService>().ShouldBeTheSameAs(container.Get<IService>());
        }

        [Test]
        public void should_be_a_singleton_because_of_Singleton_attribute()
        {
            var container = ContainerFacilitySource.New(x =>
            {
                // Any concrete class suffixed with "Cache" is supposed to be a 
                // singleton
                x.Register(typeof(IService), ObjectDef.ForType<SingletonAttributeService>());
            });

            // Use this static method to know whether or not a class
            // should be scoped as a singleton or by Http request
            ServiceRegistry.ShouldBeSingleton(typeof(SingletonAttributeService))
                           .ShouldBeTrue();

            container.Get<IService>().ShouldBeTheSameAs(container.Get<IService>());
        }

        [Test]
        public void should_be_a_singleton_because_an_ObjectDef_says_that_it_should_be()
        {
            var container = ContainerFacilitySource.New(x => {
                // Any concrete class suffixed with "Cache" is supposed to be a 
                // singleton
                var objectDef = ObjectDef.ForType<SimpleService>();
                objectDef.IsSingleton = true;

                x.Register(typeof(IService), objectDef);
            });

            // Use this static method to know whether or not a class
            // should be scoped as a singleton or by Http request
            // SimpleService is NOT normally a singleton, but we can make
            // it be so by telling the ObjectDef to make it so
            ServiceRegistry.ShouldBeSingleton(typeof(SimpleService))
                           .ShouldBeFalse();

            container.Get<IService>().ShouldBeTheSameAs(container.Get<IService>());
        }

        [Test]
        public void if_not_a_singleton_it_should_be_request_scoped()
        {
            var id = Guid.NewGuid();

            var facility = ContainerFacilitySource.New(x => {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());


                x.Register(typeof(IActionBehavior), ObjectDef.ForType<Behavior1>().Named(id.ToString()));
            });

            var instance1 = facility.BuildBehavior(new ServiceArguments(), id);
            var instance2 = facility.BuildBehavior(new ServiceArguments(), id);

            instance1.ShouldNotBeTheSameAs(instance2);
        }

        [Test]
        public void nested_container_scoping_within_a_request()
        {
            var instance1 = ContainerFacilitySource.BuildBehavior(new ServiceArguments(), ObjectDef.ForType<Behavior1>(),
                x => {
                    x.Register(typeof (IService), ObjectDef.ForType<SimpleService>());
                }).As<Behavior1>();

            // "IService" is not a singleton, therefore, there should *only* be one created
            // and shared throughout the entire request.
            // This is vital for services like IFubuRequest
            instance1.Services.GetInstance<IService>().ShouldBeTheSameAs(instance1.Service);
            instance1.Service.ShouldBeTheSameAs(instance1.Guy.Service);
        }
    }

    public class Behavior1 : IActionBehavior
    {
        private readonly IService _service;
        private readonly IServiceLocator _services;
        private readonly GuyWithService _guy;

        public Behavior1(IService service, IServiceLocator services, GuyWithService guy)
        {
            _service = service;
            _services = services;
            _guy = guy;
        }

        public void Invoke()
        {
            
        }

        public void InvokePartial()
        {

        }

        public GuyWithService Guy
        {
            get { return _guy; }
        }

        public IService Service
        {
            get { return _service; }
        }

        public IServiceLocator Services
        {
            get { return _services; }
        }
    }

    public class SingletonCache : IService
    {
        
    }

    [Singleton]
    public class SingletonAttributeService : IService
    {
        
    }
}