using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class BuildStrategiesTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public class StubInstance : Instance
        {
            private readonly object _constructedObject;


            public StubInstance(object constructedObject)
            {
                _constructedObject = constructedObject;
            }

            protected override object build(Type pluginType, BuildSession session)
            {
                return _constructedObject;
            }

            protected override string getDescription()
            {
                return "Stubbed";
            }
        }


        [Test]
        public void Singleton_build_policy()
        {
            var container = new Container(x =>
            {
                x.For<IService>().Singleton().AddInstances(o =>
                {
                    o.Is.ConstructedBy(() => new ColorService("Red")).Named("Red");
                    o.Is.ConstructedBy(() => new ColorService("Green")).Named("Green");
                });
            });


            var red1 = container.GetInstance<IService>("Red");
            var green1 = container.GetInstance<IService>("Green");
            var red2 = container.GetInstance<IService>("Red");
            var green2 = container.GetInstance<IService>("Green");
            var red3 = container.GetInstance<IService>("Red");
            var green3 = container.GetInstance<IService>("Green");

            Assert.AreSame(red1, red2);
            Assert.AreSame(red1, red3);
            Assert.AreSame(green1, green2);
            Assert.AreSame(green1, green3);
        }
    }

    [TestFixture]
    public class when_the_singleton_Lifecycle_ejects_all
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            lifecycle = new SingletonLifecycle();

            disposable1 = new StubDisposable();
            disposable2 = new StubDisposable();

            pipeline = new PipelineGraph(new PluginGraph());

            lifecycle.FindCache(pipeline).As<LifecycleObjectCache>().Set(typeof (IGateway), new StubInstance("a"), disposable1);
            lifecycle.FindCache(pipeline).As<LifecycleObjectCache>().Set(typeof(IGateway), new StubInstance("b"), disposable2);
            lifecycle.FindCache(pipeline).As<LifecycleObjectCache>().Set(typeof(IGateway), new StubInstance("c"), new object());


            lifecycle.EjectAll(pipeline);
        }

        #endregion

        private SingletonLifecycle lifecycle;
        private StubDisposable disposable1;
        private StubDisposable disposable2;
        private PipelineGraph pipeline;

        public class StubInstance : Instance
        {
            public StubInstance(string name)
            {
                Name = name;
            }

            protected override string getDescription()
            {
                throw new NotImplementedException();
            }

            protected override object build(Type pluginType, BuildSession session)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void should_have_called_dispose_if_it_was_there()
        {
            disposable1.DisposedWasCalled.ShouldBeTrue();
            disposable2.DisposedWasCalled.ShouldBeTrue();
        }

        [Test]
        public void the_count_should_be_zero()
        {
            lifecycle.FindCache(pipeline).Count.ShouldEqual(0);
        }
    }

    public class StubDisposable : IDisposable
    {
        public bool DisposedWasCalled;

        public void Dispose()
        {
            DisposedWasCalled = true;
        }
    }
}