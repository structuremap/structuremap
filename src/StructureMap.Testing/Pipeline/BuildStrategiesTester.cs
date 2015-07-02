using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class BuildStrategiesTester
    {
        public class StubInstance : Instance
        {
            private readonly object _constructedObject;


            public StubInstance(object constructedObject)
            {
                _constructedObject = constructedObject;
            }

            public override IDependencySource ToDependencySource(Type pluginType)
            {
                return new Constant(pluginType, _constructedObject);
            }

            public override Type ReturnedType
            {
                get { return _constructedObject.GetType(); }
            }

            public override string Description
            {
                get { return "Stubbed"; }
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

            red1.ShouldBeTheSameAs(red2);
            red1.ShouldBeTheSameAs(red3);
            green1.ShouldBeTheSameAs(green2);
            green1.ShouldBeTheSameAs(green3);
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

            pipeline = PipelineGraph.BuildRoot(new PluginGraph());

            lifecycle.FindCache(pipeline)
                .As<LifecycleObjectCache>()
                .Set(typeof (IGateway), new StubInstance("a"), disposable1);
            lifecycle.FindCache(pipeline)
                .As<LifecycleObjectCache>()
                .Set(typeof (IGateway), new StubInstance("b"), disposable2);
            lifecycle.FindCache(pipeline)
                .As<LifecycleObjectCache>()
                .Set(typeof (IGateway), new StubInstance("c"), new object());


            lifecycle.EjectAll(pipeline);
        }

        #endregion

        private SingletonLifecycle lifecycle;
        private StubDisposable disposable1;
        private StubDisposable disposable2;
        private IPipelineGraph pipeline;

        public class StubInstance : Instance
        {
            public StubInstance(string name)
            {
                Name = name;
            }

            public override Type ReturnedType
            {
                get { return null; }
            }

            public override IDependencySource ToDependencySource(Type pluginType)
            {
                throw new NotImplementedException();
            }

            public override string Description
            {
                get { throw new NotImplementedException(); }
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
            lifecycle.FindCache(pipeline).Count.ShouldBe(0);
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