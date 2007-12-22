using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class InterceptorTesting : Registry
    {
        private ColorService _lastService;

        private IInstanceManager _manager;

        [SetUp]
        public void SetUp()
        {
            _lastService = null;

            Registry registry = new Registry();
            registry.ForRequestedType<IService>()
                .AddInstance(
                    Instance<IService>().UsingConcreteType<ColorService>()
                        .OnCreation<ColorService>(delegate(ColorService s) { _lastService = s; })
                        .WithName("Intercepted")
                        .WithProperty("color").EqualTo("Red")
                    )

                .AddInstance(
                    Instance<IService>().UsingConcreteType<ColorService>()
                        .WithName("NotIntercepted")
                        .WithProperty("color").EqualTo("Blue")
                    )

                .AddInstance(
                    Object<IService>(new ColorService("Yellow"))
                        .WithName("Yellow")
                        .OnCreation<ColorService>(delegate(ColorService s) { _lastService = s; }))

                .AddInstance(
                    ConstructedBy<IService>(delegate { return new ColorService("Purple"); })
                        .WithName("Purple")
                        .EnrichWith<IService>(delegate(IService s) { return new DecoratorService(s); })
                    )

                .AddInstance(
                    Instance<IService>().UsingConcreteType<ColorService>()
                        .WithName("Decorated")
                        .EnrichWith<IService>(delegate(IService s) { return new DecoratorService(s); })
                        .WithProperty("color").EqualTo("Orange")
                    )

                .AddInstance(
                    Object<IService>(new ColorService("Yellow"))
                        .WithName("Bad")
                        .OnCreation<ColorService>(delegate { throw new ApplicationException("Bad!"); }))

                    ;    

            _manager = registry.BuildInstanceManager();
        }

        [Test, ExpectedException(typeof(StructureMapException), "StructureMap Exception Code:  308\nA configured instance interceptor has failed for Instance 'Bad' and concrete type 'StructureMap.Testing.Widget3.ColorService,StructureMap.Testing.Widget3'")]
        public void TrapFailureInInterceptor()
        {
            _manager.CreateInstance<IService>("Bad");
        }

        [Test]
        public void DecorateInline()
        {
            IService service = _manager.CreateInstance<IService>("Decorated");
            DecoratorService decoratorService = (DecoratorService)service;

            ColorService innerService = (ColorService)decoratorService.Inner;
            Assert.AreEqual("Orange", innerService.Color);
        }

        [Test]
        public void DecorateAConstructedService()
        {
            IService service = _manager.CreateInstance<IService>("Purple");
            DecoratorService decoratorService = (DecoratorService) service;

            ColorService innerService = (ColorService) decoratorService.Inner;
            Assert.AreEqual("Purple", innerService.Color);
        }


        [Test]
        public void RegisterAnOnCreationMethodForAnInstance()
        {
            // "Intercepted" should get intercepted and stored as _lastService.
            // "NotIntercepted" should not.

            Assert.IsNull(_lastService);
            _manager.CreateInstance<IService>("NotIntercepted");
            Assert.IsNull(_lastService);

            IService interceptedService = _manager.CreateInstance<IService>("Intercepted");
            Assert.AreSame(_lastService, interceptedService);
        }

        [Test]
        public void OnCreationWithAConstructedService()
        {
            Assert.IsNull(_lastService);
            IService interceptedService = _manager.CreateInstance<IService>("Yellow");
            Assert.AreSame(_lastService, interceptedService);
        }
    }

    public class DecoratorService : IService
    {
        private readonly IService _inner;

        public DecoratorService(IService inner)
        {
            _inner = inner;
        }


        public IService Inner
        {
            get { return _inner; }
        }
    }
}
