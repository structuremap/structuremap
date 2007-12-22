using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class InterceptAllInstancesOfPluginTypeTester : Registry
    {
        private IService _lastService;
        private IInstanceManager _manager;
        private Registry _registry;

        [SetUp]
        public void SetUp()
        {
            _lastService = null;
            _manager = null;

            _registry = new Registry();
            _registry.ForRequestedType<IService>()
                .AddInstance(
                    Instance<IService>().UsingConcreteType<ColorService>()
                        .WithName("Red")
                        .WithProperty("color").EqualTo("Red")
                    )

                .AddInstance(
                    Object<IService>(new ColorService("Yellow"))
                        .WithName("Yellow"))

                .AddInstance(
                    ConstructedBy<IService>(delegate { return new ColorService("Purple"); })
                        .WithName("Purple")
                )

                .AddInstance(
                    Instance<IService>().UsingConcreteType<ColorService>()
                        .WithName("Decorated")
                        .WithProperty("color").EqualTo("Orange")
                );
        }

        private IService getService(string name)
        {
            if (_manager == null)
            {
                _manager = _registry.BuildInstanceManager();
            }

            return _manager.CreateInstance<IService>(name);
        }

        [Test]
        public void OnStartupForAll()
        {
            _registry.ForRequestedType<IService>()
                .OnCreation(delegate(IService s)
                                {
                                    _lastService = s;
                                })
                .AddInstance(
                    ConstructedBy<IService>(delegate { return new ColorService("Green"); })
                        .WithName("Green"))
                ;

            IService red = getService("Red");
            Assert.AreSame(red, _lastService);

            IService purple = getService("Purple");
            Assert.AreSame(purple, _lastService);

            IService green = getService("Green");
            Assert.AreSame(green, _lastService);

            IService yellow = getService("Yellow");
            Assert.AreEqual(yellow, _lastService);


        }

        [Test]
        public void EnrichForAll()
        {
            _registry.ForRequestedType<IService>()
                .EnrichWith(delegate(IService s) { return new DecoratorService(s); })
                .AddInstance(
                    ConstructedBy<IService>(delegate { return new ColorService("Green"); })
                        .WithName("Green"))
                ;

            IService green = getService("Green");
            DecoratorService decoratorService = (DecoratorService) green;
            ColorService innerService = (ColorService) decoratorService.Inner;
            Assert.AreEqual("Green", innerService.Color);
        }
    }
}
