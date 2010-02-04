using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Interceptors;
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

            _defaultRegistry = (registry =>
            {
                //registry.ForRequestedType<IService>()
                //    .AddInstances(
                //    Instance<ColorService>().WithName("Red").WithProperty("color").
                //        EqualTo(
                //        "Red"),
                //    Object<IService>(new ColorService("Yellow")).WithName("Yellow"),
                //    ConstructedBy<IService>(
                //        delegate { return new ColorService("Purple"); })
                //        .WithName("Purple"),
                //    Instance<ColorService>().WithName("Decorated").WithProperty("color")
                //        .
                //        EqualTo("Orange")
                //    );

                registry.ForRequestedType<IService>().AddInstances(x =>
                {
                    x.OfConcreteType<ColorService>().WithName("Red").WithProperty("color").EqualTo("Red");

                    x.Object(new ColorService("Yellow")).WithName("Yellow");

                    x.ConstructedBy(() => new ColorService("Purple")).WithName("Purple");

                    x.OfConcreteType<ColorService>().WithName("Decorated").WithProperty("color").EqualTo("Orange");
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
                _manager = new Container(registry =>
                {
                    _defaultRegistry(registry);
                    action(registry);
                });
            }

            return _manager.GetInstance<IService>(name);
        }

        public class MockInterceptor : InstanceInterceptor
        {
            public object Target { get; set; }

            public object Process(object target, IContext context)
            {
                Target = target;
                return target;
            }
        }

        [Test]
        public void custom_interceptor_for_all()
        {
            var interceptor = new MockInterceptor();
            IService service = getService("Green", r =>
            {
                r.ForRequestedType<IService>().InterceptWith(interceptor)
                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).WithName("Green"); });
            });

            interceptor.Target.ShouldBeTheSameAs(service);
        }

        [Test]
        public void EnrichForAll()
        {
            IService green = getService("Green", r =>
            {
                r.ForRequestedType<IService>().EnrichWith(s => new DecoratorService(s))
                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).WithName("Green"); });
            });

            green.ShouldBeOfType<DecoratorService>()
                .Inner.ShouldBeOfType<ColorService>().Color.ShouldEqual("Green");
        }

        [Test]
        public void OnStartupForAll()
        {
            Action<Registry> action = registry =>
            {
                registry.ForRequestedType<IService>().OnCreation(s => _lastService = s)
                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).WithName("Green"); });
            };


            IService red = getService("Red", action);
            Assert.AreSame(red, _lastService);

            IService purple = getService("Purple", action);
            Assert.AreSame(purple, _lastService);

            IService green = getService("Green", action);
            Assert.AreSame(green, _lastService);

            IService yellow = getService("Yellow", action);
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
                                registry.ForRequestedType<IService>().AddInstances(x =>
                                {
                                    x.OfConcreteType<ColorService>().WithName("Red")
                                        .WithCtorArg("color").EqualTo("Red");

                                    x.Object(new ColorService("Yellow")).WithName("Yellow");

                                    x.ConstructedBy(() => new ColorService("Purple")).WithName("Purple");

                                    x.OfConcreteType<ColorService>().WithName("Decorated").WithCtorArg("color").EqualTo(
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
                _manager = new Container(registry =>
                {
                    _defaultRegistry(registry);
                    action(registry);
                });
            }

            return _manager.GetInstance<IService>(name);
        }

        [Test]
        public void EnrichForAll()
        {
            Action<Registry> action = r =>
            {
                r.ForRequestedType<IService>().EnrichWith(s => new DecoratorService(s))
                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).WithName("Green"); });
            };


            IService green = getService(action, "Green");


            var decoratorService = (DecoratorService) green;
            var innerService = (ColorService) decoratorService.Inner;
            Assert.AreEqual("Green", innerService.Color);
        }

        [Test]
        public void OnStartupForAll()
        {
            Action<Registry> action = r =>
            {
                r.ForRequestedType<IService>().OnCreation(s => _lastService = s)
                    .AddInstances(x => { x.ConstructedBy(() => new ColorService("Green")).WithName("Green"); });
            };

            IService red = getService(action, "Red");
            Assert.AreSame(red, _lastService);

            IService purple = getService(action, "Purple");
            Assert.AreSame(purple, _lastService);

            IService green = getService(action, "Green");
            Assert.AreSame(green, _lastService);

            IService yellow = getService(action, "Yellow");
            Assert.AreEqual(yellow, _lastService);
        }
    }
}