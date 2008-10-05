using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class InterceptorTesting : Registry
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _lastService = null;

            _container = new Container(r => r.ForRequestedType<IService>().AddInstances(x =>
            {
                x.OfConcreteType<ColorService>()
                    .OnCreation(s => _lastService = s)
                    .WithName("Intercepted")
                    .WithCtorArg("color").EqualTo("Red");

                x.OfConcreteType<ColorService>()
                    .WithName("NotIntercepted")
                    .WithCtorArg("color").EqualTo("Blue");

                x.Object(new ColorService("Yellow"))
                    .WithName("Yellow")
                    .OnCreation<ColorService>(s => _lastService = s);

                x.ConstructedBy(() => new ColorService("Purple")).WithName("Purple")
                    .EnrichWith<IService>(s => new DecoratorService(s));

                x.OfConcreteType<ColorService>().WithName("Decorated").EnrichWith<IService>(s => new DecoratorService(s))
                    .WithCtorArg("color").EqualTo("Orange");

                x.Object(new ColorService("Yellow")).WithName("Bad")
                    .OnCreation<ColorService>(obj => { throw new ApplicationException("Bad!"); });
            }));
        }

        #endregion

        private ColorService _lastService;

        private IContainer _container;

        [Test]
        public void DecorateAConstructedService()
        {
            var service = _container.GetInstance<IService>("Purple");
            var decoratorService = (DecoratorService) service;

            var innerService = (ColorService) decoratorService.Inner;
            Assert.AreEqual("Purple", innerService.Color);
        }

        [Test]
        public void DecorateInline()
        {
            var service = _container.GetInstance<IService>("Decorated");
            var decoratorService = (DecoratorService) service;

            var innerService = (ColorService) decoratorService.Inner;
            Assert.AreEqual("Orange", innerService.Color);
        }


        [Test]
        public void OnCreationWithAConstructedService()
        {
            Assert.IsNull(_lastService);
            var interceptedService = _container.GetInstance<IService>("Yellow");
            Assert.AreSame(_lastService, interceptedService);
        }

        [Test]
        public void RegisterAnOnCreationMethodForAnInstance()
        {
            // "Intercepted" should get intercepted and stored as _lastService.
            // "NotIntercepted" should not.

            Assert.IsNull(_lastService);
            _container.GetInstance<IService>("NotIntercepted");
            Assert.IsNull(_lastService);

            var interceptedService = _container.GetInstance<IService>("Intercepted");
            Assert.AreSame(_lastService, interceptedService);
        }

        [Test]
        public void TrapFailureInInterceptor()
        {
            try
            {
                _container.GetInstance<IService>("Bad");
                Assert.Fail("Should have thrown an error");
            }
            catch (StructureMapException e)
            {
                Assert.AreEqual(270, e.ErrorCode);
            }
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