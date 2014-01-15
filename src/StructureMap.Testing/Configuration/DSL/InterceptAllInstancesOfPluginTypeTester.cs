using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Building.Interception;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class InterceptAllInstancesOfPluginTypeTester : Registry
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _lastService = null;
            _manager = null;

            _defaultRegistry = (registry => {
                //registry.For<IService>()
                //    .AddInstances(
                //    Instance<ColorService>().Named("Red").Ctor<string>("color").
                //        EqualTo(
                //        "Red"),
                //    Object<IService>(new ColorService("Yellow")).Named("Yellow"),
                //    ConstructedBy<IService>(
                //        delegate { return new ColorService("Purple"); })
                //        .Named("Purple"),
                //    Instance<ColorService>().Named("Decorated").Ctor<string>("color")
                //        .
                //        EqualTo("Orange")
                //    );

                registry.For<IService>().AddInstances(x => {
                    x.Type<ColorService>().Named("Red").Ctor<string>("color").Is("Red");

                    x.Object(new ColorService("Yellow")).Named("Yellow");

                    x.ConstructedBy(() => new ColorService("Purple")).Named("Purple");

                    x.Type<ColorService>().Named("Decorated").Ctor<string>("color").Is("Orange");
                });
            });
        }

        #endregion

        private IService _lastService;
        private IContainer _manager;
        private Action<Registry> _defaultRegistry;

        private IService getService(string name, Action<Registry> action)
        {
            if (_manager == null)
            {
                _manager = new Container(registry => {
                    _defaultRegistry(registry);
                    action(registry);
                });
            }

            return _manager.GetInstance<IService>(name);
        }



        [Test]
        public void custom_interceptor_for_all()
        {
            Assert.Fail("NWO");
//            var interceptor = MockRepository.GenerateMock<IInterceptor>();
//            var service = getService("Green", r => {
//                r.For<IService>().InterceptWith(interceptor)
//                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).Named("Green"); });
//            });
//
//            interceptor.Target.ShouldBeTheSameAs(service);
        }

        [Test]
        public void EnrichForAll()
        {
            var green = getService("Green", r => {
                r.For<IService>().EnrichAllWith(s => new DecoratorService(s))
                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).Named("Green"); });
            });

            green.ShouldBeOfType<DecoratorService>()
                .Inner.ShouldBeOfType<ColorService>().Color.ShouldEqual("Green");
        }

        [Test]
        public void OnStartupForAll()
        {
            Action<Registry> action = registry => {
                registry.For<IService>().OnCreationForAll("setting the last service", s => _lastService = s)
                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).Named("Green"); });
            };


            var red = getService("Red", action);
            Assert.AreSame(red, _lastService);

            var purple = getService("Purple", action);
            Assert.AreSame(purple, _lastService);

            var green = getService("Green", action);
            Assert.AreSame(green, _lastService);

            var yellow = getService("Yellow", action);
            Assert.AreEqual(yellow, _lastService);
        }
    }

    [TestFixture]
    public class InterceptAllInstancesOfPluginTypeTester_with_SmartInstance : Registry
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _lastService = null;
            _manager = null;

            _defaultRegistry = (registry =>
                registry.For<IService>().AddInstances(x => {
                    x.Type<ColorService>().Named("Red")
                        .Ctor<string>("color").Is("Red");

                    x.Object(new ColorService("Yellow")).Named("Yellow");

                    x.ConstructedBy(() => new ColorService("Purple")).Named("Purple");

                    x.Type<ColorService>().Named("Decorated").Ctor<string>("color").Is(
                        "Orange");
                }));
        }

        #endregion

        private IService _lastService;
        private IContainer _manager;
        private Action<Registry> _defaultRegistry;

        private IService getService(Action<Registry> action, string name)
        {
            if (_manager == null)
            {
                _manager = new Container(registry => {
                    _defaultRegistry(registry);
                    action(registry);
                });
            }

            return _manager.GetInstance<IService>(name);
        }

        [Test]
        public void EnrichForAll()
        {
            Action<Registry> action = r => {
                r.For<IService>().EnrichAllWith(s => new DecoratorService(s))
                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).Named("Green"); });
            };


            var green = getService(action, "Green");


            var decoratorService = (DecoratorService) green;
            var innerService = (ColorService) decoratorService.Inner;
            Assert.AreEqual("Green", innerService.Color);
        }

        [Test]
        public void OnStartupForAll()
        {
            Action<Registry> action = r => {
                r.For<IService>().OnCreationForAll("setting the last service", s => _lastService = s)
                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).Named("Green"); });
            };

            var red = getService(action, "Red");
            Assert.AreSame(red, _lastService);

            var purple = getService(action, "Purple");
            Assert.AreSame(purple, _lastService);

            var green = getService(action, "Green");
            Assert.AreSame(green, _lastService);

            var yellow = getService(action, "Yellow");
            Assert.AreEqual(yellow, _lastService);
        }
    }
}