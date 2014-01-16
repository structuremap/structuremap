using System;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Building;
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
            recorder = new ContextRecorder();

            _container = new Container(r => {
                r.For<ContextRecorder>().Use(recorder);

                r.For<IService>().AddInstances(x => {
                    x.Type<ColorService>()
                        .OnCreation("last service",s => _lastService = s)
                        .Named("Intercepted")
                        .Ctor<string>("color").Is("Red");

                    x.Type<ColorService>()
                        .OnCreation("last touched", (c, s) => c.GetInstance<ContextRecorder>().WasTouched = true)
                        .Named("InterceptedWithContext")
                        .Ctor<string>("color").Is("Red");

                    x.Type<ColorService>()
                        .Named("NotIntercepted")
                        .Ctor<string>("color").Is("Blue");

                    x.Object(new ColorService("Yellow"))
                        .Named("Yellow")
                        .OnCreation<ColorService>("set the last service", s => _lastService = s);

                    x.ConstructedBy(() => new ColorService("Purple")).Named("Purple")
                        .DecorateWith<IService>(s => new DecoratorService(s));

                    x.ConstructedBy(() => new ColorService("Purple")).Named("DecoratedWithContext")
                        .DecorateWith<IService>("decorated with context", (c, s) => {
                            c.GetInstance<ContextRecorder>().WasTouched = true;
                            return new DecoratorService(s);
                        });

                    x.Type<ColorService>().Named("Decorated").DecorateWith<IService>(
                        s => new DecoratorService(s))
                        .Ctor<string>("color").Is("Orange");

                    x.Object(new ColorService("Yellow")).Named("Bad")
                        .OnCreation<ColorService>("throw exception", obj => { throw new ApplicationException("Bad!"); });
                });
            });
        }

        #endregion

        private ColorService _lastService;

        private IContainer _container;
        private ContextRecorder recorder;

        [Test]
        public void call_the_build_context_with_enrich()
        {
            _container.GetInstance<IService>("DecoratedWithContext");
            recorder.WasTouched.ShouldBeTrue();
        }

        [Test]
        public void call_the_build_context_with_startup()
        {
            _container.GetInstance<IService>("InterceptedWithContext");
            recorder.WasTouched.ShouldBeTrue();
        }

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
            var ex = Exception<StructureMapInterceptorException>.ShouldBeThrownBy(() => {
                _container.GetInstance<IService>("Bad");
            });

            ex.Title.ShouldContain("throw exception");
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

        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }

    public class ContextRecorder
    {
        public bool WasTouched { get; set; }
    }
}