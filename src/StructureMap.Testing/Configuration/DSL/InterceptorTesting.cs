using System;
using NUnit.Framework;
using Shouldly;
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

            // SAMPLE: interceptors-by-instance
            _container = new Container(r =>
            {
                r.For<ContextRecorder>().Use(recorder);


                r.For<IService>().AddInstances(x =>
                {
                    x.Type<ColorService>()
                        // Registers an activation action on this Instance
                        .OnCreation("last service", s => _lastService = s)
                        .Named("Intercepted")
                        .Ctor<string>("color").Is("Red");

                    x.Type<ColorService>()
                        // Activation using IContext
                        .OnCreation("last touched", (c, s) => c.GetInstance<ContextRecorder>().WasTouched = true)
                        .Named("InterceptedWithContext")
                        .Ctor<string>("color").Is("Red");

                    x.Type<ColorService>()
                        .Named("NotIntercepted")
                        .Ctor<string>("color").Is("Blue");

                    x.Object(new ColorService("Yellow"))
                        .Named("Yellow")
                        .OnCreation("set the last service", s => _lastService = s);

                    x.ConstructedBy(() => new ColorService("Purple")).Named("Purple")
                        // Applies a decorator to this instance. Not sure *why*
                        // you'd want to do it this way
                        .DecorateWith(s => new DecoratorService(s));

                    x.ConstructedBy(() => new ColorService("Purple")).Named("DecoratedWithContext")
                        // Fancier decorator
                        .DecorateWith("decorated with context", (c, s) =>
                        {
                            c.GetInstance<ContextRecorder>().WasTouched = true;
                            return new DecoratorService(s);
                        });

                    x.Type<ColorService>().Named("Decorated").DecorateWith(
                        s => new DecoratorService(s))
                        .Ctor<string>("color").Is("Orange");

                    x.Object(new ColorService("Yellow")).Named("Bad")
                        .OnCreation("throw exception", obj => { throw new ApplicationException("Bad!"); });
                });
                // ENDSAMPLE
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
            innerService.Color.ShouldBe("Purple");
        }

        [Test]
        public void DecorateInline()
        {
            var service = _container.GetInstance<IService>("Decorated");
            var decoratorService = (DecoratorService) service;

            var innerService = (ColorService) decoratorService.Inner;
            innerService.Color.ShouldBe("Orange");
        }


        [Test]
        public void OnCreationWithAConstructedService()
        {
            _lastService.ShouldBeNull();
            var interceptedService = _container.GetInstance<IService>("Yellow");
            _lastService.ShouldBeTheSameAs(interceptedService);
        }

        [Test]
        public void RegisterAnOnCreationMethodForAnInstance()
        {
            // "Intercepted" should get intercepted and stored as _lastService.
            // "NotIntercepted" should not.

            _lastService.ShouldBeNull();
            _container.GetInstance<IService>("NotIntercepted");
            _lastService.ShouldBeNull();

            var interceptedService = _container.GetInstance<IService>("Intercepted");
            _lastService.ShouldBeTheSameAs(interceptedService);
        }

        [Test]
        public void TrapFailureInInterceptor()
        {
            var ex =
                Exception<StructureMapInterceptorException>.ShouldBeThrownBy(
                    () => { _container.GetInstance<IService>("Bad"); });

            ex.Title.ShouldBe(
                "Activator interceptor failed during object creation.  See the inner exception for details.");

            ex.Message.ShouldContain("throw exception");
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